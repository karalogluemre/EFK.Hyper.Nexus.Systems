using Microsoft.AspNetCore.Http;

namespace Commons.Application.Abstract.Dto.Company
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }

        public string Name { get; set; }
        public string TaxNumber { get; set; } 
        public string TaxOffice { get; set; } 
        public string RegistrationNumber { get; set; } 

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string Iban { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccountNumber { get; set; }

        public string CompanyType { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime? EstablishedDate { get; set; }
        public IFormFile LogoUrl { get; set; }

    }
}
