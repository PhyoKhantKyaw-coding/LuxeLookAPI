namespace LuxeLookAPI.DTO
{
    public class AddCategoryWithInstancesDto
    {
        public string CatName { get; set; }
        public List<string> InstanceNames { get; set; } = new();
    }
}
