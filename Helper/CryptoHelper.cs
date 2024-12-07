using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ClassLibrary.BusinessObject;

public static class CryptoHelper
{
    private static readonly string Key = AppEnv.IsRunningInContainer ? Environment.GetEnvironmentVariable("AES_KEY") : "YourStrongEncryptionKey" ;  // 32 chars for AES-256

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(Key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            ms.Write(aes.IV, 0, aes.IV.Length); // Save the IV at the beginning of the encrypted data
            using var writer = new StreamWriter(cryptoStream);
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            throw new ArgumentNullException(nameof(encryptedText));

        var buffer = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(Key);

        // Extract the IV from the encrypted data
        using var ms = new MemoryStream(buffer);
        var iv = new byte[aes.BlockSize / 8];
        ms.Read(iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);

        return reader.ReadToEnd();
    }
}
