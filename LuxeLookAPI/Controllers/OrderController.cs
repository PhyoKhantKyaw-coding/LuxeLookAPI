using LuxeLookAPI.DTO;
using LuxeLookAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuxeLookAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // 1. Add Order
    [HttpPost("add")]
    public async Task<IActionResult> AddOrder([FromBody] AddOrderDTO dto)
    {
        if (dto == null || dto.OrderDetails == null || !dto.OrderDetails.Any())
            return BadRequest(new ResponseDTO
            {
                Status = APIStatus.Error,
                Message = Messages.InvalidPostedData
            });

        var result = await _orderService.AddOrderAsync(dto);
        return result
            ? Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.AddSucess })
            : StatusCode(500, new ResponseDTO { Status = APIStatus.SystemError, Message = Messages.ErrorWhileFetchingData });
    }

    // 2. Add To Cart
    [HttpPost("addtocart")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCardDTO dto)
    {
        if (dto == null || dto.OrderDetails == null || !dto.OrderDetails.Any())
            return BadRequest(new ResponseDTO
            {
                Status = APIStatus.Error,
                Message = Messages.InvalidPostedData
            });

        var result = await _orderService.AddToCartAsync(dto);
        return result
            ? Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.AddSucess })
            : StatusCode(500, new ResponseDTO { Status = APIStatus.SystemError, Message = Messages.ErrorWhileFetchingData });
    }

    // 3. Add To Favorite
    [HttpPost("addtofavorite")]
    public async Task<IActionResult> AddToFavorite([FromBody] AddToFavoriteDTO dto)
    {
        if (dto == null || dto.OrderDetails == null || !dto.OrderDetails.Any())
            return BadRequest(new ResponseDTO
            {
                Status = APIStatus.Error,
                Message = Messages.InvalidPostedData
            });

        var result = await _orderService.AddToFavoriteAsync(dto);
        return result
            ? Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.AddSucess })
            : StatusCode(500, new ResponseDTO { Status = APIStatus.SystemError, Message = Messages.ErrorWhileFetchingData });
    }
    [HttpGet("getallcart")]
    public async Task<IActionResult> GetAllCart([FromQuery] string currency = "us")
    {
        var data = await _orderService.GetAllAddToCart(currency);
        return Ok(new ResponseDTO
        {
            Status = APIStatus.Successful,
            Message = Messages.Successfully,
            Data = data
        });
    }
    [HttpGet("getallfavorite")]
    public async Task<IActionResult> GetAllFavorite([FromQuery] string currency = "us")
    {
        var data = await _orderService.GetAllFavorite(currency);
        return Ok(new ResponseDTO
        {
            Status = APIStatus.Successful,
            Message = Messages.Successfully,
            Data = data
        });
    }


    // 4. Get All Orders
    [HttpGet("allwithuserid")]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        if (orders == null || !orders.Any())
            return Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.NoData, Data = new List<object>() });

        return Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.Result, Data = orders });
    }
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrderswithstatus( string status)
    {
        var orders = await _orderService.GetAllOrdersByStatusAsync(status);
        if (orders == null || !orders.Any())
            return Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.NoData, Data = new List<object>() });

        return Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.Result, Data = orders });
    }


    // 5. Get Order Details by OrderId
    [HttpGet("{orderId}/details")]
    public async Task<IActionResult> GetOrderDetails(Guid orderId)
    {
        var details = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);
        if (details == null || !details.Any())
            return Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.NoData, Data = new List<object>() });

        return Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.Result, Data = details });
    }

    // 6. Update Order Status
    [HttpPatch("{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromQuery] string status)
    {
        if (string.IsNullOrEmpty(status))
            return BadRequest(new ResponseDTO { Status = APIStatus.Error, Message = Messages.InvalidPostedData });

        var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
        return result
            ? Ok(new ResponseDTO { Status = APIStatus.Successful, Message = Messages.UpdateSucess })
            : StatusCode(500, new ResponseDTO { Status = APIStatus.SystemError, Message = Messages.UpdateFail });
    }
    //7.access delivery
    [Authorize(Roles = "Delivery")]
    [HttpPost("delivery-access")]
    public async Task<IActionResult> DeliveryAccess([FromBody] DeliveryAccessDTO dto)
    {
        if (dto == null || dto.OrderId == null)
            return BadRequest(new ResponseDTO
            {
                Status = APIStatus.Error,
                Message = Messages.InvalidPostedData
            });

        var result = await _orderService.AssignOrderToDeliveryAsync(dto);

        if (!result)
            return StatusCode(403, new ResponseDTO
            {
                Status = APIStatus.Error,
                Message = "You are not authorized to access this order or order does not exist."
            });

        return Ok(new ResponseDTO
        {
            Status = APIStatus.Successful,
            Message = "Order successfully assigned to delivery."
        });
    }
}
