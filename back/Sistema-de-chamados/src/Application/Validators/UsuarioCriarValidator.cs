using FluentValidation;
using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Validators
{
    public class UsuarioCriarValidator : AbstractValidator<UsuarioCriarDTO>
    {
        public UsuarioCriarValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
                .Matches(@"[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
                .Matches(@"[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
                .Matches(@"[0-9]").WithMessage("Senha deve conter pelo menos um número");

            RuleFor(x => x.Telefone)
                .Matches(@"^\d{10,11}$").WithMessage("Telefone deve ter 10 ou 11 dígitos")
                .When(x => !string.IsNullOrEmpty(x.Telefone));
        }
    }
}
