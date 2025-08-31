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
            decimal totalCost = 0;
            decimal totalProfit = 0;
            int totalQty = 0;
            decimal totalAmount = 0;

            // Create Payment first (amount set later)
            var payment = new PaymentModel
            {
                PaymentId = Guid.NewGuid(),
                PaymentType = dto.PaymentType,
                PaymentAmount = 0,
                DeliFee = dto.DeliFee,
                Status = "Pending",
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow,
                ActiveFlag = true
            };
            await _context.Payments.AddAsync(payment);

            // Create Order
            var order = new OrderModel
            {
                OrderId = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                OrderPlace = dto.OrderPlace,
                OrderStartPoint = dto.OrderStartPoint,
                OrderEndPoint = dto.OrderEndPoint,
                Status = "ordered",
                UserId = userId.Value,
                PaymentId = payment.PaymentId,
                CreatedAt = DateTime.UtcNow,
                ActiveFlag = true
            };
            await _context.Orders.AddAsync(order);

            // Save OrderDetails & Update Stock
            foreach (var detail in dto.OrderDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product == null) continue;

                // 🔹 Check stock availability
                if (product.StockQTY < detail.Qty)
                {
                    EmailHelper.SendStockAlertEmail("admin_email@example.com", product.ProductName, detail.Qty ?? 0, product.StockQTY ?? 0);

                    // Optionally: skip this product or stop the order
                    throw new InvalidOperationException($"Not enough stock for {product.ProductName}");
                }

                // 🔹 Deduct stock
                product.StockQTY -= detail.Qty;
                _context.Products.Update(product);

                // Calculate financials
                var cost = product.Cost * detail.Qty;
                var profit = (product.Price - product.Cost) * detail.Qty;
                var amount = product.Price * detail.Qty;

                totalCost += (decimal)cost;
                totalProfit += (decimal)profit;
                totalQty += (int)detail.Qty;
                totalAmount += (decimal)amount;

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

            // Update order & payment values
            order.TotalQTY = totalQty;
            order.TotalCost = totalCost;
            order.TotalProfit = totalProfit;
            order.TotalAmount = totalAmount + (dto.DeliFee ?? 0);

            payment.PaymentAmount = order.TotalAmount;

            // Save everything
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // 🔹 Send confirmation email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                EmailHelper.SendOrderSuccessEmail(user.Email, user.UserName ?? "Customer", order.OrderId ?? Guid.Empty);
            }

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
public async Task<getatcDTO> GetAllAddToCart(string currency = "us")
{
    var (userId, _, _) = _tokenReader.GetUserFromContext();
    if (userId == null)
        return new getatcDTO(); // empty list

    var data = await (from atc in _context.AddToCarts
                      join p in _context.Products
                          on atc.ProductId equals p.ProductId
                      where atc.UserId == userId
                      select new
                      {
                          p.ProductName,
                          atc.Qty,
                          Price = p.Price ?? 0,
                          p.ProductImageUrl
                      })
                      .ToListAsync();

    // Apply currency conversion
    var convertedData = data.Select(d =>
    {
        var (convertedPrice, symbol) = CurrencyHelper.Convert(currency, d.Price);
        return new getproductatcDTO
        {
            productName = d.ProductName,
            qty = d.Qty ?? 0,
            eachprice = convertedPrice,
            totalprice = convertedPrice * d.Qty ?? 0,
            producturl = d.ProductImageUrl
        };
    }).ToList();

    return new getatcDTO
    {
        productatc = convertedData
    };
}

public async Task<getfavoriteDTO> GetAllFavorite(string currency = "us")
{
    var (userId, _, _) = _tokenReader.GetUserFromContext();
    if (userId == null)
        return new getfavoriteDTO(); // empty list

    var data = await (from f in _context.Favorites
                      join p in _context.Products
                          on f.ProductId equals p.ProductId
                      join c in _context.Categories
                          on p.CatInstanceId equals c.CatId
                      where f.UserId == userId
                      select new
                      {
                          p.ProductName,
                          CategoryName = c.CatName,
                          Price = p.Price ?? 0,
                          p.ProductImageUrl,
                          p.ProductDescription
                      })
                      .ToListAsync();

    var convertedData = data.Select(d =>
    {
        var (convertedPrice, symbol) = CurrencyHelper.Convert(currency, d.Price);
        return new getproductfavoriteDTO
        {
            productName = d.ProductName,
            categoryName = d.CategoryName,
            producturl = d.ProductImageUrl,
            productdescription = d.ProductDescription
        };
    }).ToList();

    return new getfavoriteDTO
    {
        productfavorite = convertedData
    };
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
    public async Task<List<GetOrderDTO>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10)
    {
        var (userId, roleName, _) = _tokenReader.GetUserFromContext();
        if (userId == null)
            return new List<GetOrderDTO>();

        var ordersQuery = from o in _context.Orders
                          join p in _context.Payments
                              on o.PaymentId equals p.PaymentId into op
                          from pay in op.DefaultIfEmpty()
                          join u in _context.Users
                              on o.UserId equals u.UserId into ou
                          from user in ou.DefaultIfEmpty()
                          join d in _context.Users
                              on o.DeliveryId equals d.UserId into od
                          from delivery in od.DefaultIfEmpty()
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
                              UserName = user != null ? user.UserName : null,
                              DeliveryName = delivery != null ? delivery.UserName : null,
                              PaymentType = pay.PaymentType,
                              PaymentAmount = pay.PaymentAmount,
                              DeliFee = pay.DeliFee,
                              PaymentStatus = pay.Status
                          };

        // Role-based filter
        if (!string.IsNullOrEmpty(roleName) && roleName.ToLower() != "admin")
        {
            if (roleName.ToLower() == "user")
            {
                ordersQuery = ordersQuery.Where(o => o.UserName ==
                                                     _context.Users
                                                             .Where(u => u.UserId == userId)
                                                             .Select(u => u.UserName)
                                                             .FirstOrDefault());
            }
            else if (roleName.ToLower() == "delivery")
            {
                ordersQuery = ordersQuery.Where(o => o.DeliveryName ==
                                                     _context.Users
                                                             .Where(u => u.UserId == userId)
                                                             .Select(u => u.UserName)
                                                             .FirstOrDefault());
            }
        }

        // Pagination
        ordersQuery = ordersQuery
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

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
    public async Task<OrderWithVoucherDTO?> GetVoucherByOrderIdAsync(Guid orderId)
    {
        // Get the order
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null) return null;

        // Get order details
        var orderDetails = await (from od in _context.OrderDetails
                                  join p in _context.Products
                                  on od.ProductId equals p.ProductId
                                  where od.OrderId == orderId
                                  select new OrderDetailWithVoucherDTO
                                  {
                                      OrderDetailId = od.OrderDetailId,
                                      ProductName = p.ProductName,
                                      Qty = od.Qty ?? 0,
                                      Price = od.Price ?? 0,
                                      DiscountAmount = od.Profit ?? 0 // treat Profit as discount
                                  }).ToListAsync();

        // Calculate total discount from order details
        var totalDiscount = orderDetails.Sum(od => od.DiscountAmount);

        var dto = new OrderWithVoucherDTO
        {
            OrderId = order.OrderId,
            TotalAmount = order.TotalAmount ?? 0,
            DiscountAmount = totalDiscount, // sum of all discounts in details
            DiscountPercent = order.TotalAmount.HasValue && totalDiscount > 0
                ? (totalDiscount / order.TotalAmount.Value) * 100
                : 0,
            Description = "Derived from order details", // optional note
            OrderDetails = orderDetails
        };

        return dto;
    }


    public async Task<bool> AssignOrderToDeliveryAsync(DeliveryAccessDTO dto)
    {
        if (dto == null || dto.OrderId == null)
            return false;

        // Get current user from token
        var (userId, role, _) = _tokenReader.GetUserFromContext();
        if (userId == null || role != "Delivery")
            return false;

        // Get the order
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.OrderId && o.ActiveFlag);
        if (order == null) return false;

        // Assign delivery
        order.DeliveryId = userId;
        order.Status = "delivering";
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Update(order);
        var saved = await _context.SaveChangesAsync() > 0;

        if (saved)
        {
            // 🔹 Notify user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == order.UserId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                EmailHelper.SendDeliveryAccessEmail(user.Email, user.UserName ?? "Customer", order.OrderId ?? Guid.Empty);
            }
        }

        return saved;
    }
    public async Task<List<GetOrderDTO>> GetAllOrdersByStatusAsync(string status)
    {
        var ordersQuery = from o in _context.Orders
                          join p in _context.Payments
                              on o.PaymentId equals p.PaymentId into op
                          from pay in op.DefaultIfEmpty()
                          join u in _context.Users
                              on o.UserId equals u.UserId into ou
                          from user in ou.DefaultIfEmpty()
                          join d in _context.Users
                              on o.DeliveryId equals d.UserId into od
                          from delivery in od.DefaultIfEmpty()
                          where o.ActiveFlag == true && o.Status == status
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
                              UserName = user != null ? user.UserName : null,
                              DeliveryName = delivery != null ? delivery.UserName : null,
                              PaymentType = pay.PaymentType,
                              PaymentAmount = pay.PaymentAmount,
                              DeliFee = pay.DeliFee,
                              PaymentStatus = pay.Status
                          };

        return await ordersQuery
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }


}
