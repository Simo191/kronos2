using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Cagnottes.Commands.CreateCagnotte;

public record CreateCagnotteCommand(int Annee, decimal BudgetAnnuel) : IRequest<Cagnotte>;
