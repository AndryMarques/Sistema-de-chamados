using FluentValidation;
using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Validators
{
    public class ChamadoAtualizarValidator : AbstractValidator<ChamadoAtualizarDTO>
    {
        public ChamadoAtualizarValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id deve ser maior que 0");

            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("Título é obrigatório")
                .MinimumLength(3).WithMessage("Título deve ter no mínimo 3 caracteres")
                .MaximumLength(150).WithMessage("Título deve ter no máximo 150 caracteres");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .MinimumLength(10).WithMessage("Descrição deve ter no mínimo 10 caracteres");

            RuleFor(x => x.Prioridade)
                .IsInEnum().WithMessage("Prioridade inválida");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status inválido");
        }
    }
}
