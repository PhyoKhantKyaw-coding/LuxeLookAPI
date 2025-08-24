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
