using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models
{
    [Table("tblPayment")]
    public class PaymentModel : CommonFields
    {
        [Key]
        public Guid? PaymentId { get; set; }
        public string? PaymentType { get; set; } // pay arrive home, kpay, wpay, cbpay
        public decimal? PaymentAmount { get; set; }
        public decimal? DeliFee { get; set; }
        public string? Status { get; set; } // Deli finish, payment completed
        public Guid? UserId { get; set; }
    }
}
