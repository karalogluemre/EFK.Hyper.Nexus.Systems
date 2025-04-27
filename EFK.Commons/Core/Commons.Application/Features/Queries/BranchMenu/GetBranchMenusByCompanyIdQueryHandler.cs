using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Application.Features.Queries.BranchMenu
{
    public class GetBranchMenusByCompanyIdQueryHandler<TDbContext>(
    IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> companyElasticReadRepository,
    IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> packageMenuElasticReadRepository,
    IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuElasticReadRepository
) : IRequestHandler<GetBranchMenusByCompanyIdQueryRequest, BaseResponse>
    where TDbContext : DbContext
    {
        readonly private IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> companyElasticReadRepository = companyElasticReadRepository;
        readonly private IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> packageMenuElasticReadRepository = packageMenuElasticReadRepository;
        readonly private IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuElasticReadRepository = menuElasticReadRepository;

        public async Task<BaseResponse> Handle(GetBranchMenusByCompanyIdQueryRequest request, CancellationToken cancellationToken)
        {
            // 1. Şirketi getir
            var companies = await this.companyElasticReadRepository.GetAllFromElasticSearchAsync();
            var company = companies.FirstOrDefault(c => c.Id == Guid.Parse( request.CompanyId));
            if (company == null)
                return new() { Message = "Kayıt boş gelmiştir.", Succeeded = false };

            // 2. Company'nin PackageId'sini al
            var packageId = company.PackageId;

            // 3. O packageId'ye bağlı PackageMenu kayıtlarını getir
            var allPackageMenus = await this.packageMenuElasticReadRepository.GetAllFromElasticSearchAsync();
            var packageMenus = allPackageMenus.Where(pm => pm.PackageId == packageId).ToList();

            if (!packageMenus.Any())
                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Paket menüleri başarıyla getirildi.",
                };
            // 4. İlgili MenuId'leri topla
            var menuIds = packageMenus.Select(pm => pm.MenuId).Distinct().ToList();

            // 5. Menuleri getir
            var allMenus = await this.menuElasticReadRepository.GetAllFromElasticSearchAsync();
            var filteredMenus = allMenus.Where(m => menuIds.Contains(m.Id)).ToList();

            if (filteredMenus.Count > 0)
                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Paket menüleri başarıyla getirildi.",
                    Data = filteredMenus
                };
            else
            {
                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Şirket menüleri getirilirken bir hata meydana geldi.",
                };
            }
        }
    }
}