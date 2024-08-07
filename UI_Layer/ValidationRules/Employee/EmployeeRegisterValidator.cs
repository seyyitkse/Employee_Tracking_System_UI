﻿using FluentValidation;
using UI_Layer.Dtos.EmployeeDto;

namespace UI_Layer.ValidationRules.Employee
{
    public class EmployeeRegisterValidator : AbstractValidator<CreateEmployeeDto>
    {
        public EmployeeRegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Mail boş olamaz.")
                .NotEmpty().WithMessage("Mail boş olamaz.")
                .MaximumLength(25).WithMessage("Mail en fazla 8 karakter olabilir.")
                .MinimumLength(10).WithMessage("Mail en az 10 karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.Password)
                 .NotNull().WithMessage("Şifre boş olamaz.")
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MaximumLength(15).WithMessage("Şifre en fazla 15 karakter olabilir.")
                .MinimumLength(5).WithMessage("Şifre en az 5 karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.ConfirmPassword)
                 .NotNull().WithMessage("Şifreyi tekrar giriniz!")
                .NotEmpty().WithMessage("Şifreyi tekrar giriniz!")
                .MaximumLength(15).WithMessage("Şifre en fazla 15 karakter olabilir.")
                .MinimumLength(5).WithMessage("Şifre en az 5 karakter uzunluğunda olmalıdır.");
        }
    }
}
