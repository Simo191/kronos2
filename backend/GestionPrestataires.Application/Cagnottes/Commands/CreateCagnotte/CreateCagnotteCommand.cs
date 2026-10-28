using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.CreateCagnotte;

public record CreateCagnotteCommand(int Annee, decimal BudgetAnnuel) : IRequest<Cagnotte>;
