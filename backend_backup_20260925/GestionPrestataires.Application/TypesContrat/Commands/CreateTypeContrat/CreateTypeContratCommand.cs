using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.TypesContrat.Commands.CreateTypeContrat;

public record CreateTypeContratCommand(string Libelle, string? Description) : IRequest<TypeContrat>;
