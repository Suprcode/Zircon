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

        public static BinaryReader GetReader(Stream stream)
        {
            stream.Seek(5, SeekOrigin.Begin);

            var buffer = new byte[16];
            stream.Read(buffer, 0, 16);

            var stringToCompare = System.Text.Encoding.UTF8.GetString(buffer);

            var isEncrypted = !new string[] { "Server.DBModels.", "Library.SystemMo", "Client.UserModel" }.Contains(stringToCompare);

            if (isEncrypted && _cryptoKey == null)
                throw new ApplicationException("Database is encrypted but not specified Crypto Key");

            stream.Seek(0, SeekOrigin.Begin);

            BinaryReader reader;

            if (isEncrypted)
            {
                stream.Read(buffer, 0, 16); // read IV
                var decryptor = _algorithm.CreateDecryptor(_cryptoKey, buffer);
                reader = new BinaryReader(new CryptoStream(stream, decryptor, CryptoStreamMode.Read));
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
    }
}
