namespace Commons.Domain.Models.Adreses
{
    public class Province : BaseEntity
    {
        public int ExternalId { get; set; } // TürkiyeAPI'den gelen ID
        public string Name { get; set; }
        public int Area { get; set; }
        public ICollection<District> Districts { get; set; }
    }
}
