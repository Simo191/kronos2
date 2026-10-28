using MediatR;

namespace GestionPrestataires.Application.Authentication.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;

// ✅ SÉCURITÉ : la réponse ne contient QUE le token.
// Le rôle, l'id, l'email sont signés dans le JWT et lus côté Angular
// via decodeToken() — jamais exposés en clair dans le body HTTP.
public record LoginResult(
    string Token
);