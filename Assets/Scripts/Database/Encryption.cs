using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class Encryption
{
    private static int _saltLengthLimit = 16;


    public static byte[] Hash(byte[] value, byte[] salt)
    {
        byte[] saltedValue = value.Concat(salt).ToArray();

        return new SHA256Managed().ComputeHash(saltedValue);
    }

    public static byte[] HashValue(string input, ref byte[] salt)
    {
        if (salt == null)
            salt = GetSalt();

        return Hash(Encoding.ASCII.GetBytes(input), salt);
    }

    private static byte[] GetSalt()
    {
        return GetSalt(_saltLengthLimit);
    }

    private static byte[] GetSalt(int maximumSaltLength)
    {
        var salt = new byte[maximumSaltLength];
        using (var random = new RNGCryptoServiceProvider())
        {
            random.GetNonZeroBytes(salt);
        }

        return salt;
    }    


}
