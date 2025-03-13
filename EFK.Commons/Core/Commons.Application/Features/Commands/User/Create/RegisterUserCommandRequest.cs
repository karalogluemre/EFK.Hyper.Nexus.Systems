using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.User.Create
{
    public class RegisterUserCommandRequest : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public string PlaceOfBirth { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string BloodGroup { get; set; }
        public Guid RoleId { get; set; }
        public string Password { get; set; }
    }
}
