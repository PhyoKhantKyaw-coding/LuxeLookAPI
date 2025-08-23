using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models;

[Table("tblBrand")]
public class BrandModel
{
    [Key]
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
}
