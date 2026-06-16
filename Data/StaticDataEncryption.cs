using System.Security.Cryptography;
using System.Text;

namespace PMS.Data;

public static class StaticDataEncryption
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("PMS-static-key-32-bytes-12345678");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("PMS-static-iv-16");

    public static string Encrypt(string value)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(value);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string value)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var decryptor = aes.CreateDecryptor();
        var encryptedBytes = Convert.FromBase64String(value);
        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
