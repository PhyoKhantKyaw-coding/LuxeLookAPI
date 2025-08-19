namespace LuxeLookAPI.DTO
{
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
        public Guid? UserId { get; set; }
        public Guid? DeliveryId { get; set; }
        public string? PaymentType { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? DeliFee { get; set; }
        public string? Status { get; set; }
    }
    public class AddToCardDTO
    {
        public List<OrderDetailDTO>? OrderDetails { get; set; }
    }
}
