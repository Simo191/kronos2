using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
 using MediatR;
 
namespace GestionPrestataires.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPrestataireRepository _context;

    public CreateUserCommandHandler(IUserRepository userRepository, IPrestataireRepository context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        //   Vérifier email unique
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("Un utilisateur avec cet email existe déjà");

        //   Si PrestataireId fourni, vérifier contraintes
        if (request.PrestataireId.HasValue)
        {
            // Vérifier que le prestataire existe
            var prestataire = await _context.GetByIdAsync(request.PrestataireId.Value);

            if (prestataire == null)
                throw new Exception("Prestataire non trouvé");

            //   Vérifier que le prestataire n'a pas déjà un user
            if (prestataire.UserId != null)
                throw new Exception("Ce prestataire est déjà lié à un utilisateur");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleId = request.RoleId,
            IsActive = true, //   ACTIF par défaut
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        //   Si PrestataireId fourni, mettre à jour le prestataire
        if (request.PrestataireId.HasValue)
        {
            var prestataire = await _context.GetByIdAsync(request.PrestataireId.Value);


            if (prestataire != null)
            {
                prestataire.UserId = user.Id;
                prestataire.UpdatedAt = DateTime.UtcNow;
                await _context.UpdateAsync(prestataire);
            }
        }

        return user.Id;
    }
}