
using System;
using System.Security.Cryptography;
using System.Text;

public static class HashingDemo
{
    public static void Main()
    {
        string password = "Password123!";
        string wrongPassword = "WrongPassword!";

        DemoFastHashSha256(password);

        PasswordRecord stored = RegisterUser(password);

        Console.WriteLine("Stored Salt: " + Convert.ToBase64String(stored.Salt));
        Console.WriteLine("Stored Hash: " + Convert.ToBase64String(stored.Hash));

        bool correct = LoginVerify(password, stored);
        bool wrong = LoginVerify(wrongPassword, stored);

        Console.WriteLine($"Correct Password: {correct}");
        Console.WriteLine($"Wrong Password: {wrong}");

        DemoCannotReverseHash(stored);
    }

    private static void DemoFastHashSha256(string password)
    {
        string hash = Sha256Hex(password);

        Console.WriteLine("SHA-256 Hash: " + hash);
    }

    private static PasswordRecord RegisterUser(string password)
    {
        byte[] salt = RandomBytes(16);
        int iterations = 100_000;

        byte[] hash = Pbkdf2Hash(password, salt, iterations, 32);

        return new PasswordRecord(salt, hash, iterations);
    }

    private static bool LoginVerify(string password, PasswordRecord stored)
    {
        byte[] attemptHash = Pbkdf2Hash(
            password,
            stored.Salt,
            stored.Iterations,
            stored.Hash.Length
        );

        return CryptographicOperations.FixedTimeEquals(
            attemptHash,
            stored.Hash
        );
    }

    private static void DemoCannotReverseHash(PasswordRecord stored)
    {
        Console.WriteLine("\nHash: " + Convert.ToBase64String(stored.Hash));
        Console.WriteLine("A hash cannot be decrypted.");
    }

    private static byte[] Pbkdf2Hash(
        string password,
        byte[] salt,
        int iterations,
        int hashLength)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256
        );

        return pbkdf2.GetBytes(hashLength);
    }

    private static string Sha256Hex(string input)
    {
        using SHA256 sha = SHA256.Create();

        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(hash);
    }

    private static byte[] RandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);

        return bytes;
    }

    private sealed record PasswordRecord(
        byte[] Salt,
        byte[] Hash,
        int Iterations
    );
}
