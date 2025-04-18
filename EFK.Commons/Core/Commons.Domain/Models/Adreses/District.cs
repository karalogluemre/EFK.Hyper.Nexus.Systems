namespace Commons.Domain.Models.Adreses
{
    public class District : BaseEntity
    {
        public int ExternalId { get; set; } // TürkiyeAPI'den gelen ID
        public string Name { get; set; }
        public Guid ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}
