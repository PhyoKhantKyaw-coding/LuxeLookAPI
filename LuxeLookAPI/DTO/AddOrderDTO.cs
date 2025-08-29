namespace LuxeLookAPI.DTO;

public class OrderDetailDTO
{
    public Guid? ProductId { get; set; }
    public int? Qty { get; set; }
}

public class AddOrderDTO
{
    public List<OrderDetailDTO>? OrderDetails { get; set; }
    public string? OrderPlace { get; set; }
    public string? OrderStartPoint { get; set; }
    public string? OrderEndPoint { get; set; }
    public string? PaymentType { get; set; }
    public decimal? DeliFee { get; set; }
}
public class AddToCardDTO
{
    public List<OrderDetailDTO>? OrderDetails { get; set; }
}

public class AddToFavoriteDTO
{
    public List<OrderDetailDTO>? OrderDetails { get; set; }
}
public class getproductatcDTO
{
    public string productName { get; set; }
    public int qty { get; set; }
    public decimal totalprice { get; set; }
    public decimal eachprice { get; set; }
    public string producturl { get; set; }
}
public class getatcDTO
{
    public List<getproductatcDTO>? productatc { get; set; }
}
public class getproductfavoriteDTO
{
    public string productName { get; set; }
    public string categoryName { get; set; }
    public string producturl { get; set; }
    public string productdescription { get; set; }
}
public class getfavoriteDTO
{
    public List<getproductfavoriteDTO>? productfavorite { get; set; }
}