using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Commands.CreateTypeContrat;

public class CreateTypeContratCommandHandler : IRequestHandler<CreateTypeContratCommand, TypeContrat>
{
    private readonly ITypeContratRepository _repository;

    public CreateTypeContratCommandHandler(ITypeContratRepository repository)
    {
        _repository = repository;
    }

    public async Task<TypeContrat> Handle(CreateTypeContratCommand request, CancellationToken cancellationToken)
    {
        var typeContrat = new TypeContrat
        {
            Id = Guid.NewGuid(),
            Libelle = request.Libelle,
            Description = request.Description ?? ""
        };

        await _repository.AddAsync(typeContrat);
        return typeContrat;
    }
}
