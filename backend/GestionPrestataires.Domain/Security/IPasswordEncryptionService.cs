namespace GestionPrestataires.Domain.Security;

/// <summary>
///   Chiffrement/déchiffrement symétrique des mots de passe stockés.
///   Implémentation AES-256-GCM (authentifié, anti-tampering).
/// </summary>
public interface IPasswordEncryptionService
{
    /// <summary>Chiffre une chaîne. Retour : base64 de [nonce(12) | tag(16) | cipher(N)].</summary>
    string Encrypt(string plaintext);

    /// <summary>Déchiffre. Lève si tag invalide / clé incorrecte / format corrompu.</summary>
    string Decrypt(string encrypted);
}
