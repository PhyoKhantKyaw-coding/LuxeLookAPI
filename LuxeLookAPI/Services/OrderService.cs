using LuxeLookAPI.DTO;
using LuxeLookAPI.Models;
using LuxeLookAPI.Share;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Services;

public class OrderService
{
    private readonly DataContext _context;
    private readonly CommonTokenReader _tokenReader;

    public OrderService(DataContext context, CommonTokenReader tokenReader)
    {
        _context = context;
        _tokenReader = tokenReader;
    }

    // 1. Add Order + Order Details + Payment
    public async Task<bool> AddOrderAsync(AddOrderDTO dto)
    {
        if (dto == null || dto.OrderDetails == null || !dto.OrderDetails.Any())
            return false;

        var (userId, _, _) = _tokenReader.GetUserFromContext();
        if (userId == null)
            return false;

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Save Payment
            var payment = new PaymentModel
            {
                PaymentId = Guid.NewGuid(),
                PaymentType = dto.PaymentType,
                PaymentAmount = dto.PaymentAmount,
                DeliFee = dto.DeliFee,
                Status = "Pending",
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow,
                ActiveFlag = true
            };
            await _context.Payments.AddAsync(payment);

            // Create Order first
            var order = new OrderModel
            {
                OrderId = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                OrderPlace = dto.OrderPlace,
                OrderStartPoint = dto.OrderStartPoint,
                OrderEndPoint = dto.OrderEndPoint,
                Status = dto.Status ?? "ordered",
                UserId = userId.Value,
                DeliveryId = dto.DeliveryId,
                PaymentId = payment.PaymentId,
                CreatedAt = DateTime.UtcNow,
                ActiveFlag = true
            };
            await _context.Orders.AddAsync(order);

            decimal totalCost = 0;
            decimal totalProfit = 0;
            int totalQty = 0;

            // Save OrderDetails
            foreach (var detail in dto.OrderDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product == null) continue;

                var cost = product.Cost * detail.Qty;
                var profit = (product.Price - product.Cost) * detail.Qty;
                var amount = product.Price * detail.Qty;

                totalCost += (decimal)cost;
                totalProfit += (decimal)profit;
                totalQty += (int)detail.Qty;

                var orderDetail = new OrderDetailModel
                {
                    OrderDetailId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Qty = detail.Qty,
                    Price = product.Price,
                    Cost = product.Cost,
                    Profit = profit,
                    CreatedAt = DateTime.UtcNow,
                    ActiveFlag = true
                };
                await _context.OrderDetails.AddAsync(orderDetail);
            }

            // Update order totals
            order.TotalQTY = totalQty;
            order.TotalCost = totalCost;
            order.TotalProfit = totalProfit;
            order.TotalAmount = dto.PaymentAmount; // or you could recalc from details if needed

            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    // 2. Add To Cart
    public async Task<bool> AddToCartAsync(AddToCardDTO dto)
    {
        if (dto == null || dto.OrderDetails == null || !dto.OrderDetails.Any())
            return false;

        var (userId, _, _) = _tokenReader.GetUserFromContext();
        if (userId == null)
            return false;

        foreach (var item in dto.OrderDetails)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
            if (product == null) continue;

            var cart = new AddToCartModel
            {
                ATCId = Guid.NewGuid(),
                ProductId = product.ProductId,
                Qty = item.Qty,
                Amount = product.Price * item.Qty,
                 UserId = userId.Value,
                CreatedAt = DateTime.UtcNow,
                ActiveFlag = true
            };
            await _context.AddToCarts.AddAsync(cart);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    // 3. Add To Favorite
    public async Task<bool> AddToFavoriteAsync(AddToFavoriteDTO dto)
    {
        if (dto == null || dto.OrderDetails == null || !dto.OrderDetails.Any())
            return false;

        var (userId, _, _) = _tokenReader.GetUserFromContext();
        if (userId == null)
            return false;

        foreach (var item in dto.OrderDetails)
        {
            var favorite = new FavoriteModel
            {
                FavoriteId = Guid.NewGuid(),
                UserId = userId.Value,
                ProductId = item.ProductId,
                CreatedAt = DateTime.UtcNow,
                ActiveFlag = true
            };
            await _context.Favorites.AddAsync(favorite);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    // 4. Get All Orders
    public async Task<List<GetOrderDTO>> GetAllOrdersAsync()
    {
        // Read user info from token
        var (userId, roleName, _) = _tokenReader.GetUserFromContext();
        if (userId == null)
            return new List<GetOrderDTO>();

        // Start query
        var ordersQuery = from o in _context.Orders
                          join p in _context.Payments
                              on o.PaymentId equals p.PaymentId into op
                          from pay in op.DefaultIfEmpty()
                          where o.ActiveFlag == true
                          select new GetOrderDTO
                          {
                              OrderId = o.OrderId,
                              OrderDate = o.OrderDate,
                              OrderPlace = o.OrderPlace,
                              OrderStartPoint = o.OrderStartPoint,
                              OrderEndPoint = o.OrderEndPoint,
                              TotalAmount = o.TotalAmount,
                              TotalQTY = o.TotalQTY,
                              TotalProfit = o.TotalProfit,
                              TotalCost = o.TotalCost,
                              Status = o.Status,
                              UserName = o.UserId != null ? _context.Users
                                             .Where(u => u.UserId == o.UserId)
                                             .Select(u => u.UserName)
                                             .FirstOrDefault() : null,
                              DeliveryName = o.DeliveryId != null ? _context.Users
                                             .Where(d => d.UserId == o.DeliveryId)
                                             .Select(d => d.UserName)
                                             .FirstOrDefault() : null,
                              PaymentType = pay.PaymentType,
                              PaymentAmount = pay.PaymentAmount,
                              DeliFee = pay.DeliFee,
                              PaymentStatus = pay.Status
                          };

        // Filter based on role
        if (roleName != null && roleName.ToLower() != "admin")
        {
            ordersQuery = ordersQuery.Where(o => o.UserName ==
                               _context.Users.Where(u => u.UserId == userId)
                                             .Select(u => u.UserName)
                                             .FirstOrDefault());
        }

        return await ordersQuery.ToListAsync();
    }

    // 5. Get Order Details by OrderId
    public async Task<List<OrderDetailModel>> GetOrderDetailsByOrderIdAsync(Guid orderId)
    {
        return await _context.OrderDetails
            .Where(od => od.OrderId == orderId && od.ActiveFlag == true)
            .ToListAsync();
    }

    // 6. Update Order Status
    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Update(order);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<List<GetOrderDTO>> GetDeliveryOrdersAsync()
    {
        var (userId, roleName, _) = _tokenReader.GetUserFromContext();
        if (userId == null || roleName == null || roleName.ToLower() != "delivery")
            return new List<GetOrderDTO>();

        var ordersQuery = from o in _context.Orders
                          join p in _context.Payments
                              on o.PaymentId equals p.PaymentId into op
                          from pay in op.DefaultIfEmpty()
                          where o.ActiveFlag == true &&
                                (o.Status == "ordered" || (o.Status == "delivering" && o.DeliveryId == userId))
                          select new GetOrderDTO
                          {
                              OrderId = o.OrderId,
                              OrderDate = o.OrderDate,
                              OrderPlace = o.OrderPlace,
                              OrderStartPoint = o.OrderStartPoint,
                              OrderEndPoint = o.OrderEndPoint,
                              TotalAmount = o.TotalAmount,
                              TotalQTY = o.TotalQTY,
                              TotalProfit = o.TotalProfit,
                              TotalCost = o.TotalCost,
                              Status = o.Status,
                              UserName = o.UserId != null ? _context.Users
                                             .Where(u => u.UserId == o.UserId)
                                             .Select(u => u.UserName)
                                             .FirstOrDefault() : null,
                              DeliveryName = o.DeliveryId != null ? _context.Users
                                             .Where(d => d.UserId == o.DeliveryId)
                                             .Select(d => d.UserName)
                                             .FirstOrDefault() : null,
                              PaymentType = pay.PaymentType,
                              PaymentAmount = pay.PaymentAmount,
                              DeliFee = pay.DeliFee,
                              PaymentStatus = pay.Status
                          };

        return await ordersQuery.ToListAsync();
    }

    public async Task<bool> AssignOrderToDeliveryAsync(DeliveryAccessDTO dto)
    {
        if (dto == null || dto.OrderId == null)
            return false;

        // Get current user from token
        var (userId, role, _) = _tokenReader.GetUserFromContext();
        if (userId == null || role != "Delivery") // Only Delivery role allowed
            return false;

        // Get the order
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.OrderId && o.ActiveFlag);
        if (order == null) return false;

        // Assign delivery
        order.DeliveryId = userId;
        order.Status = "delivering";
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Update(order);
        return await _context.SaveChangesAsync() > 0;
    }

}
