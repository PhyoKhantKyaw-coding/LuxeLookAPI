using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models
{
    [Table("tblCategory")]
    public class CategoryModel : CommonFields
    {
        [Key]
        public Guid? CatId { get; set; }
        public string? CatName { get; set; }
    }
}
