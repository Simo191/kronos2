using GestionPrestataires.Domain.Repositories;
 using MediatR;
 
namespace GestionPrestataires.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _repository;
    private readonly IPrestataireRepository _context;

    public DeleteUserCommandHandler(IUserRepository repository, IPrestataireRepository context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);
        if (user == null)
            throw new Exception("Utilisateur non trouvé");

        //   VÉRIFIER SI USER LIÉ À UN PRESTATAIRE
        var linkedPrestataire = await _context.GetByIdUserAsync(user.Id);

        if (linkedPrestataire != null)
        {
            throw new Exception($"Impossible de supprimer cet utilisateur : il est lié au prestataire '{linkedPrestataire.Prenom} {linkedPrestataire.Nom}'. Veuillez d'abord délier le prestataire.");
        }

        await _repository.DeleteAsync(request.Id);
    }
}