using Commons.Domain.Models.Branches;
using Commons.Domain.Models.Packages;

namespace Commons.Domain.Models.Companies
{
    public class Company : BaseEntity
    {
        public Guid PackageId { get; set; } 
        public Package Package { get; set; }

        // Temel Bilgiler
        public string Name { get; set; }
        public string TaxNumber { get; set; } // Vergi Numarası
        public string TaxOffice { get; set; } // Vergi Dairesi
        public string RegistrationNumber { get; set; } // Ticaret Sicil Numarası

        // İletişim Bilgileri
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        // Adres Bilgileri
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Finansal ve Banka Bilgileri
        public string Iban { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccountNumber { get; set; }

        // Firma Tipi ve Durumu
        public string CompanyType { get; set; } // Enum olabilir (Şahıs, Limited, Anonim vs.)
        public bool IsActive { get; set; } = true;

        // Ek Bilgiler
        public DateTime? EstablishedDate { get; set; } // Kuruluş Tarihi

        public ICollection<CompanyFile> CompanyFiles { get; set; }
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();

    }
}
