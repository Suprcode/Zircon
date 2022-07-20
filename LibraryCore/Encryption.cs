using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Library
{
    public class Encryption
    {
        private static byte[] _cryptoKey = null;
        private readonly static SymmetricAlgorithm _algorithm = new RijndaelManaged() { KeySize = 256 };
        private readonly static RandomNumberGenerator _randomNumberGenerator = new RNGCryptoServiceProvider();

        private static bool IsEncrypted(Stream stream)
        {
            var buffer = new byte[21];
            stream.Read(buffer, 0, 21);

            var stringToCompare = Encoding.UTF8.GetString(buffer, 5, 16);

            return !new string[] { "Server.DBModels.", "Library.SystemMo", "Client.UserModel" }.Contains(stringToCompare);
        }

        public static BinaryReader GetReader(Stream stream)
        {
            var isEncrypted = IsEncrypted(stream);

            if (isEncrypted && _cryptoKey == null)
                throw new ApplicationException("Database is encrypted but not specified Crypto Key");

            stream.Seek(0, SeekOrigin.Begin);

            BinaryReader reader;

            if (isEncrypted)
            {
                var iv = new byte[16];
                stream.Read(iv, 0, 16);

                var decryptor = _algorithm.CreateDecryptor(_cryptoKey, iv);
                var decStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
                reader = new BinaryReader(decStream);
            }
            else
            {
                reader = new BinaryReader(stream);
            }

            return reader;
        }

        public static BinaryWriter GetWriter(Stream stream)
        {
            if (_cryptoKey == null) return new BinaryWriter(stream);
            var iv = new byte[16];
            _randomNumberGenerator.GetNonZeroBytes(iv);
            stream.Write(iv, 0, iv.Length);
            var encryptor = _algorithm.CreateEncryptor(_cryptoKey, iv);
            return new BinaryWriter(new CryptoStream(stream, encryptor, CryptoStreamMode.Write));
        }

        public static void SetKey(byte[] databaseKey)
        {
            _cryptoKey = databaseKey;
        }

        public static void SetKey(string databaseKey)
        {
            _cryptoKey = Convert.FromBase64String(databaseKey);
        }
    }
}
