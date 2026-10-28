using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Commands.CreateTypeContrat;

public record CreateTypeContratCommand(string Libelle, string? Description) : IRequest<TypeContrat>;
