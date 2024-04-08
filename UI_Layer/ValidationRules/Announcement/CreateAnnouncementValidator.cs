using FluentValidation;
using UI_Layer.Dtos.AnnouncementDto;

namespace UI_Layer.ValidationRules.Announcement
{
    public class CreateAnnouncementValidator : AbstractValidator<CreateAnnouncementDto>
    {
        public CreateAnnouncementValidator()
        {
            RuleFor(x => x.Content)
                .NotNull().WithMessage("İçerik boş olamaz.")
                .NotEmpty().WithMessage("İçerik boş olamaz.")
                .MaximumLength(150).WithMessage("İçerik en fazla 250 karakter olabilir.")
                .MinimumLength(10).WithMessage("İçerik en az 20 karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.Title)
                 .NotNull().WithMessage("Başlık boş olamaz.")
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(50).WithMessage("Başlık en fazla 100 karakter olabilir.")
                .MinimumLength(5).WithMessage("Başlık en az 5 karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.Date)
                 .NotNull().WithMessage("Tarih boş olamaz.")
                .NotEmpty().WithMessage("Tarih boş olamaz.");

            RuleFor(x => x.TypeID)
                .NotNull().WithMessage("Tip ID boş olamaz.")
                .NotEmpty().WithMessage("Tip ID boş olamaz.");

        }
    }
}
