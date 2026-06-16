using FluentValidation;
using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Validators
{
    public class UsuarioAtualizarValidator : AbstractValidator<UsuarioAtualizarDTO>
    {
        public UsuarioAtualizarValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id deve ser maior que 0");

            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

            RuleFor(x => x.Telefone)
                .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Telefone));
        }
    }
}
