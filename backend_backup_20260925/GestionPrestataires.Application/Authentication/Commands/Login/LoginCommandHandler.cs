using GestionPrestataires.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionPrestataires.Application.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // ✅ SÉCURITÉ : message générique — ne révèle pas si l'email existe
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Email ou mot de passe incorrect");

        // ✅ Vérification compte désactivé (après vérification du mot de passe)
        if (!user.IsActive)
            throw new Exception("Ce compte est désactivé. Veuillez contacter un administrateur.");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user);

        // ✅ SÉCURITÉ : on ne renvoie QUE le token dans la réponse HTTP.
        // Le rôle, l'id, l'email sont déjà signés dans le JWT.
        // Le frontend les lit via decodeToken() — rien de sensible en clair dans le body.
        return new LoginResult(token);
    }

    private string GenerateJwtToken(Domain.Entities.User user)
    {
        // ✅ Même clé que Program.cs (Jwt:Key) — indispensable pour la validation
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key non configuré dans appsettings.json");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            // Claims standard JWT
            new Claim(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email,      user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName,  user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),

            // Rôle — utilisé par [Authorize(Roles = "...")] côté ASP.NET
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),

            // Claims custom lus par le frontend Angular via decodeToken()
            new Claim("roleId",        user.Role?.Id.ToString() ?? ""),
            new Claim("prestataireId", user.Prestataire?.Id.ToString() ?? ""),
            new Claim("isActive",      user.IsActive.ToString().ToLower()),

            // Compatibilité ASP.NET (NameIdentifier utilisé dans les controllers)
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "GestionPrestataires",
            audience: _configuration["Jwt:Audience"] ?? "GestionPrestataires",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}