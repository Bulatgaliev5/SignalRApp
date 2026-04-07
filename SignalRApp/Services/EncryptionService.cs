using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SignalRApp.Services
{
    //public class EncryptionService
    //{
    //    private readonly byte[] _key;
    //    private readonly byte[] _iv;
    //    public string ivSecret;

    //    public EncryptionService(string key, string ivSecret)
    //    {
    //        this.ivSecret = ivSecret;
    //        _key = GetKey(key);
    //        _iv = GetIV(ivSecret);
    //    }

    //    private byte[] GetIV(string ivSecret)
    //    {
    //        using MD5 md5 = MD5.Create();
    //        return md5.ComputeHash(Encoding.UTF8.GetBytes(ivSecret));
    //    }

    //    private byte[] GetKey(string key)
    //    {
    //        using SHA256 sha256 = SHA256.Create();
    //        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    //    }
    //    /// <summary>
    //    /// Метод для зашифровки
    //    /// </summary>
    //    /// <param name="plainText"></param>
    //    /// <returns></returns>
    //    public string Encrypt(string plainText)
    //    {
    //        if (string.IsNullOrEmpty(plainText))
    //            return plainText;

    //        using Aes aes = Aes.Create();
    //        aes.Key = _key;
    //        aes.IV = _iv;

    //        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

    //        using MemoryStream msEncrypt = new MemoryStream();
    //        using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
    //        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
    //        {
    //            swEncrypt.Write(plainText);
    //        }

    //        return Convert.ToBase64String(msEncrypt.ToArray());
    //    }
    //    /// <summary>
    //    /// Метод для расшифровки
    //    /// </summary>
    //    /// <param name="cipherText"></param>
    //    /// <returns></returns>
    //    public string Decrypt(string cipherText)
    //    {
    //        if (string.IsNullOrEmpty(cipherText))
    //            return cipherText;

    //        try
    //        {
    //            using Aes aes = Aes.Create();
    //            aes.Key = _key;
    //            aes.IV = _iv;

    //            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

    //            byte[] buffer = Convert.FromBase64String(cipherText);

    //            using MemoryStream msDecrypt = new MemoryStream(buffer);
    //            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
    //            using StreamReader srDecrypt = new StreamReader(csDecrypt);

    //            return srDecrypt.ReadToEnd();
    //        }
    //        catch
    //        {
    //            // Если расшифровка не удалась, возвращаем исходную строку
    //            return "Не удалось расшифровать сообщение! " + cipherText;
    //        }
    //    }
    //}


    //public class RSAEncryptionService
    //{

    //    public (string publicKey, string privateKey) GenerateKeys()
    //    {
    //        using var rsa = RSA.Create();
    //        return (
    //            Convert.ToBase64String(rsa.ExportRSAPublicKey()),
    //            Convert.ToBase64String(rsa.ExportRSAPrivateKey())
    //        );
    //    }

    //    //Метод для зашифровки
    //    public string Encrypt(string message, string publicKey)
    //    {
    //        using var rsa = RSA.Create();

    //        rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

    //        byte[] data = Encoding.UTF8.GetBytes(message);

    //        byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

    //        return Convert.ToBase64String(encrypted);
    //    }
    //    //Метод для расшифровки
    //    //public string Decrypt(string encryptedMessage, string privateKey)
    //    //{
    //    //    using var rsa = RSA.Create(2048);

    //    //    rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

    //    //    byte[] data = Convert.FromBase64String(encryptedMessage);

    //    //    byte[] decrypted = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);

    //    //    return Encoding.UTF8.GetString(decrypted);
    //    //}

    //    public string Decrypt(string encryptedMessage, string privateKey)
    //    {
    //        using var rsa = RSA.Create(); // Не нужно указывать 2048 при импорте

    //        // Используем ImportPkcs8PrivateKey
    //        rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

    //        byte[] data = Convert.FromBase64String(encryptedMessage);
    //        byte[] decrypted = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);

    //        return Encoding.UTF8.GetString(decrypted);
    //    }
    //}

    class SimpleRSA
    {
        private BigInteger p; // Простое число
        private BigInteger q; // Простое число
        private BigInteger n; // Модуль
        private BigInteger phi; // Функция Эйлера
        private BigInteger e; // Открытая экспонента
        private BigInteger d; // Закрытая экспонента

        private static Random rand = new Random();

        public SimpleRSA()
        {

        }

        public ((BigInteger e, BigInteger n) publicKey, (BigInteger d, BigInteger n) privateKey) GenerateKeys()
        {
            p = GenerateRandomPrime(1000, 5000);
            q = GenerateRandomPrime(1000, 5000);

            n = p * q;
            phi = (p - 1) * (q - 1);

            e = 17;
            d = ModInverse(e, phi);

            return ((e, n), (d, n));
        }

        public async Task LoadOrGenerateKeys(string storedPrivate, string storedPublic)
        {
            //var storedPrivate = await SecureStorage.GetAsync("PrivateKey");
            //var storedPublic = await SecureStorage.GetAsync("PublicKey");

            if (storedPrivate != null && storedPublic != null)
            {
                // Загружаем ключи
                var privateParts = storedPrivate.Split('|');
                d = BigInteger.Parse(privateParts[0]);
                n = BigInteger.Parse(privateParts[1]);

                var publicParts = storedPublic.Split('|');
                e = BigInteger.Parse(publicParts[0]);
            }
            //else
            //{
            //    // Генерируем ключи
            //    GenerateKeys();

            //    // Сохраняем
            //    await SecureStorage.SetAsync("PrivateKey", $"{d}|{n}");
            //    await SecureStorage.SetAsync("PublicKey", $"{e}|{n}");
            //}
        }
        private BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m, t, q;
            BigInteger x0 = 0, x1 = 1;

            if (m == 1) return 0;

            while (a > 1)
            {
                q = a / m;
                t = m;

                m = a % m;
                a = t;
                t = x0;

                x0 = x1 - q * x0;
                x1 = t;
            }

            if (x1 < 0) x1 += m0;

            return x1;
        }

        private BigInteger GenerateRandomPrime(int min, int max)
        {
            BigInteger primeCandidate;

            do
            {
                primeCandidate = GenerateRandomNumber(min, max);
            } while (!IsPrime(primeCandidate));

            return primeCandidate;
        }

        private BigInteger GenerateRandomNumber(int min, int max)
        {
            byte[] bytes = new byte[4];
            rand.NextBytes(bytes);
            BigInteger randomNumber = new BigInteger(bytes);
            randomNumber = BigInteger.Abs(randomNumber);

            randomNumber = (randomNumber % (max - min + 1)) + min;
            return randomNumber;
        }

        private bool IsPrime(BigInteger number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            BigInteger limit = (BigInteger)Math.Sqrt((double)number) + 1;

            for (BigInteger i = 3; i < limit; i += 2)
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }

        public string Encrypt(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            int blockSize = (int)(Math.Floor(BigInteger.Log(n, 256)) - 1);
            StringBuilder encryptedMessage = new StringBuilder();

            for (int i = 0; i < messageBytes.Length; i += blockSize)
            {
                byte[] block = new byte[Math.Min(blockSize, messageBytes.Length - i)];
                Array.Copy(messageBytes, i, block, 0, block.Length);

                BigInteger blockInt = new BigInteger(block);
                BigInteger encryptedBlock = BigInteger.ModPow(blockInt, e, n);
                encryptedMessage.Append(encryptedBlock.ToString() + ",");
            }

            return encryptedMessage.ToString();
        }

        public string Decrypt(string encryptedMessage)
        {
            string[] encryptedBlocks = encryptedMessage.Split(',');
            StringBuilder decryptedMessage = new StringBuilder();

            foreach (string block in encryptedBlocks)
            {
                if (string.IsNullOrEmpty(block)) continue;

                BigInteger encryptedBlock = BigInteger.Parse(block);
                BigInteger decryptedBlock = BigInteger.ModPow(encryptedBlock, d, n);
                byte[] blockBytes = decryptedBlock.ToByteArray();

                decryptedMessage.Append(Encoding.UTF8.GetString(blockBytes));
            }

            return decryptedMessage.ToString();
        }
    }
}
