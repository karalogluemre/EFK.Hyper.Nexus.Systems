namespace Commons.Application.Abstract.Dto
{
    public class AdresesDto
    {
        public class ProvinceWithDistrictsDto
        {
            public Guid Id { get; set; } // Province'in int olan ExternalId'si gelecek
            public int ExternalId { get; set; } // Province'in int olan ExternalId'si gelecek
            public string Name { get; set; } = string.Empty;
            public int Area { get; set; }
            public List<DistrictDto> Districts { get; set; } = new();
        }

        public class DistrictDto
        {
            public Guid Id { get; set; } // District'in int olan ExternalId'si gelecek
            public string Name { get; set; } = string.Empty;
            public int ExternalId { get; set; } // Province'in int olan ExternalId'si gelecek

        }
    }
}
