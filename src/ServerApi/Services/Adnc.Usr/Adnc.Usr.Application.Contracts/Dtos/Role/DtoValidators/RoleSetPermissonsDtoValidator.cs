﻿using FluentValidation;
using Adnc.Usr.Application.Contracts.Dtos;

namespace Adnc.Usr.Application.Contracts.DtoValidators
{
    public class RoleSetPermissonsDtoValidator : AbstractValidator<RoleSetPermissonsDto>
    {
        public RoleSetPermissonsDtoValidator()
        {
            RuleFor(x => x.RoleId).GreaterThan(0);
            RuleFor(x => x.Permissions).NotNull();
        }
    }
}
