using FluentValidation;
using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Validators
{
    public class AcompanhamentoCriarValidator : AbstractValidator<AcompanhamentoCriarDTO>
    {
        public AcompanhamentoCriarValidator()
        {
            RuleFor(x => x.ChamadoId)
                .GreaterThan(0).WithMessage("ChamadoId deve ser maior que 0");

            RuleFor(x => x.ResponsavelId)
                .GreaterThan(0).WithMessage("ResponsavelId deve ser maior que 0");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .MinimumLength(5).WithMessage("Descrição deve ter no mínimo 5 caracteres")
                .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres");
        }
    }
}
