using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models;

[Table("tblCategoryInstance")]
public class CategoryInstance
{
    [Key]
    public Guid? CatInstanceId { get; set; }
    public Guid? CatId { get; set; }
    public string? CatInstanceName { get; set; }
}
