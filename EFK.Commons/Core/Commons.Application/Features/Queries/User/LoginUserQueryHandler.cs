using Commons.Domain.Models;
using Commons.Domain.Models.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Commons.Application.Features.Queries.User
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQueryRequest, BaseResponse>
    {
        private readonly SignInManager<AppUser> signInManager;

        public LoginUserQueryHandler(SignInManager<AppUser> signInManager)
        {
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public async Task<BaseResponse> Handle(LoginUserQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await this.signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);
            return new BaseResponse { Succeeded = result.Succeeded };
        }
    }
}