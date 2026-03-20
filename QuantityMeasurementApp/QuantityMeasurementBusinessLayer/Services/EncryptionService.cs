using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        private readonly ILogger<EncryptionService> _logger;

        public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
        {
            _logger = logger;

            // Get encryption key from configuration (should be in User Secrets/Environment)
            var encryptionKey =
                Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
                ?? configuration["Encryption:Key"];

            if (string.IsNullOrEmpty(encryptionKey) || encryptionKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "Valid encryption key (32+ characters) is required"
                );
            }

            // Use SHA256 to derive a 256-bit key from the provided string
            using var sha256 = SHA256.Create();
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));

            // Use a fixed IV for simplicity (in production, generate random IV per encryption)
            _iv = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 16));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(cipherBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encryption failed");
                throw;
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                var cipherBytes = Convert.FromBase64String(cipherText);
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed");
                throw;
            }
        }

        public bool TryDecrypt(string cipherText, out string plainText)
        {
            try
            {
                plainText = Decrypt(cipherText);
                return true;
            }
            catch
            {
                plainText = string.Empty;
                return false;
            }
        }
    }
}
