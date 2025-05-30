﻿using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Package.Create
{
    public class CreatePackageCommandRequest : IRequest<BaseResponse>
    {
        public string? Id { get; set; }
        public string Name { get; set; }
    }
}
