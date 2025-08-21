using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models
{
    [Table("tblFavoriteModel")]
    public class FavoriteModel : CommonFields
    {
        [Key]
        public Guid? FavoriteId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ProductId { get; set; }

    }
}
