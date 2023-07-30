using Autofac;
using Library;
using Server.Envir;
using System;
using System.Reflection;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetAssembly(typeof(Config));
            ConfigReader.Load(assembly);
            Config.LoadVersion();
            try
            {
                if (!string.IsNullOrEmpty(Config.EncryptionKey))
                    SEnvir.CryptoKey = Convert.FromBase64String(Config.EncryptionKey);
            }
            catch (Exception)
            {
                throw new ApplicationException($"Invalid format encryption key, expected a base64 with 32 bytes");
            }

            if (Config.EncryptionEnabled && SEnvir.CryptoKey == null)
                throw new ApplicationException($"Encryption is enabled but not specified key [System] => DatabaseKey");

            if (Config.EncryptionEnabled)
                Encryption.SetKey(SEnvir.CryptoKey);

            SEnvir.UseLogConsole = true;
            SEnvir.StartServer();

            Console.CancelKeyPress += Console_CancelKeyPress;

            // We check EnvirThread why when SEnvir is full stoped, set this to null...
            while (SEnvir.EnvirThread != null)
            {
                var command = Console.ReadLine();

            }

            ConfigReader.Save(typeof(Config).Assembly);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            SEnvir.Started = false;
        }
    }
}
