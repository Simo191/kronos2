using System.Security.Cryptography;
using System.Text;
using GestionPrestataires.Domain.Security;
using Microsoft.Extensions.Configuration;

namespace GestionPrestataires.Infrastructure.Security;

/// <summary>
///   AES-256-GCM. La clé (32 octets / 256 bits) est lue depuis la config
///   sous "Encryption:Key" en base64.
///
///   Génération d'une nouvelle clé pour appsettings :
///   <code>Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))</code>
///
///   Format du payload chiffré (base64) :
///   [ nonce (12) | tag (16) | ciphertext (N) ]
/// </summary>
public class PasswordEncryptionService : IPasswordEncryptionService
{
    private const int NonceSize = 12;   // 96 bits — recommandé pour AES-GCM
    private const int TagSize = 16;     // 128 bits
    private readonly byte[] _key;

    public PasswordEncryptionService(IConfiguration config)
    {
        var keyBase64 = config["Encryption:Key"]
            ?? throw new InvalidOperationException(
                "Encryption:Key non configuré. Générer une clé 32 octets base64 et la placer dans appsettings.");

        _key = Convert.FromBase64String(keyBase64);
        if (_key.Length != 32)
            throw new InvalidOperationException(
                $"Encryption:Key doit faire 32 octets (256 bits), reçu {_key.Length}.");
    }

    public string Encrypt(string plaintext)
    {
        if (plaintext == null) throw new ArgumentNullException(nameof(plaintext));

        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipher = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipher, tag);

        // Concatène nonce | tag | cipher
        var output = new byte[NonceSize + TagSize + cipher.Length];
        Buffer.BlockCopy(nonce, 0, output, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, output, NonceSize, TagSize);
        Buffer.BlockCopy(cipher, 0, output, NonceSize + TagSize, cipher.Length);

        return Convert.ToBase64String(output);
    }

    public string Decrypt(string encrypted)
    {
        if (string.IsNullOrEmpty(encrypted))
            throw new ArgumentException("Payload chiffré vide.", nameof(encrypted));

        var data = Convert.FromBase64String(encrypted);
        if (data.Length < NonceSize + TagSize)
            throw new ArgumentException("Payload chiffré invalide (trop court).", nameof(encrypted));

        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var cipher = new byte[data.Length - NonceSize - TagSize];

        Buffer.BlockCopy(data, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(data, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(data, NonceSize + TagSize, cipher, 0, cipher.Length);

        var plain = new byte[cipher.Length];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipher, tag, plain); // lève CryptographicException si tag invalide

        return Encoding.UTF8.GetString(plain);
    }
}
