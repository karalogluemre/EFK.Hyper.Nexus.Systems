using Commons.Application.Repositories.Commands;
using Commons.Application.Token;
using Commons.Domain.Models;
using Commons.Domain.Models.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WatchDog;

namespace Commons.Application.Features.Queries.User
{
    public class LoginUserQueryHandler<TDbContext>(
        IWriteRepository<TDbContext, RefreshToken> refreshTokenWriteRepository,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        GenerateJwtToken generateJwtToken
        ) : IRequestHandler<LoginUserQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly UserManager<AppUser> userManager = userManager;
        readonly IWriteRepository<TDbContext, RefreshToken> refreshTokenWriteRepository = refreshTokenWriteRepository;
        readonly SignInManager<AppUser> signInManager = signInManager;
        readonly GenerateJwtToken generateJwtToken = generateJwtToken;


        public async Task<BaseResponse> Handle(LoginUserQueryRequest request, CancellationToken cancellationToken)
        {
            WatchLogger.Log("Giriş işlemi başlatıldı.");

            // Kullanıcıyı e-posta ile bul
            var user = await this.userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                WatchLogger.Log($"E-posta adresi için kullanıcı bulunamadı: {request.Email}");
                return new BaseResponse
                {
                    Message = "Geçersiz e-posta adresi veya şifre.",
                    Succeeded = false
                };
            }

            WatchLogger.Log($"E-posta adresi için kullanıcı bulundu: {request.Email}");

            // Kullanıcı hesabının kilitli olup olmadığını kontrol et
            if (await this.userManager.IsLockedOutAsync(user))
            {
                WatchLogger.Log($"Kullanıcı hesabı kilitli: {request.Email}");
                return new BaseResponse
                {
                    Message = "Hesap kilitli. Lütfen daha sonra tekrar deneyiniz.",
                    Succeeded = false
                };
            }

            // Şifreyi kontrol et
            var result = await this.signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                WatchLogger.Log($"Geçersiz şifre denemesi: {request.Email}");

                // Başarısız giriş sayısını artır
                await this.userManager.AccessFailedAsync(user);

                // Kullanıcıyı kilitle
                if (await this.userManager.IsLockedOutAsync(user))
                {
                    WatchLogger.Log($"Çok sayıda başarısız denemeden dolayı kullanıcı hesabı kilitlendi: {request.Email}");
                    return new BaseResponse
                    {
                        Message = "Çok sayıda başarısız giriş denemesi nedeniyle hesap kilitlendi.",
                        Succeeded = false
                    };
                }

                return new BaseResponse
                {
                    Message = "Geçersiz e-posta adresi veya şifre.",
                    Succeeded = false
                };
            }

            WatchLogger.Log($"Şifre doğrulandı: {request.Email}");

            // Başarılı giriş durumunda başarısız giriş sayısını sıfırla
            await this.userManager.ResetAccessFailedCountAsync(user);

            // Token oluştur ve döndür


            // Token oluştur ve Refresh Token kaydet
            var userToken = this.generateJwtToken.JwtTokenGenerate(user);
            var refreshToken = this.generateJwtToken.GenerateRefreshToken();

            // Refresh Token'ı kaydet
            await this.refreshTokenWriteRepository.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                AppUserId = user.Id,
                IsRevoked = false,
                Revoked = null
            });

            WatchLogger.Log($"JWT token ve Refresh Token oluşturuldu: {request.Email}");

            return new BaseResponse
            {
                Message = "Giriş başarılı.",
                Succeeded = true,
                Data = new
                {
                    AccessToken = userToken.Result,
                    RefreshToken = refreshToken
                }
            };

        }
    }
}