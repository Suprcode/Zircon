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

            SEnvir.UseLogConsole = true;
            SEnvir.StartServer();

            Console.CancelKeyPress += Console_CancelKeyPress;

            // We check EnvirThread why when SEnvir is full stoped, set this to null...
            while (SEnvir.EnvirThread != null)
            {
                var command = Console.ReadLine();

            }

            ConfigReader.Save();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            SEnvir.Started = false;
        }
    }
}
