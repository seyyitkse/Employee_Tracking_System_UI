using FluentValidation;
using UI_Layer.Dtos.DepartmentDto;

namespace UI_Layer.ValidationRules.Department
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Ad kısmı boş olamaz.")
                .NotEmpty().WithMessage("Ad kısmı boş olamaz.")
                .MaximumLength(50).WithMessage("Ad kısmı en fazla 50 karakter olabilir.")
                .MinimumLength(5).WithMessage("Ad kısmı en az 5 karakter uzunluğunda olmalıdır.");
        }
    }
}
