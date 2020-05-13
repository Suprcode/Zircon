﻿using Library;
using Server.DBModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using G = Library.Network.GeneralPackets;
using S = Library.Network.ServerPackets;
using C = Library.Network.ClientPackets;
using System.Net.Http;
using MirDB;
using System.IO.Compression;

namespace Server.Envir
{
    public static class WebServer
    {
        public static ConcurrentQueue<WebCommand> WebCommandQueue;
        public static bool WebServerStarted { get; set; }

        private static HttpListener WebListener;
        public const string ActivationCommand = "Activation", ResetCommand = "Reset", DeleteCommand = "Delete",
            SystemDBSyncCommand = "SystemDBSync";
        public const string ActivationKey = "ActivationKey", ResetKey = "ResetKey", DeleteKey = "DeleteKey";

        public const string Completed = "Completed";
        public const string Currency = "GBP";

        public static Dictionary<decimal, int> GoldTable = new Dictionary<decimal, int>
        {
            [5M] = 500,
            [10M] = 1030,
            [15M] = 1590,
            [20M] = 2180,
            [30M] = 3360,
            [50M] = 5750,
            [100M] = 12000,
        };

        public const string VerifiedPath = @".\Database\Store\Verified\",
            InvalidPath = @".\Database\Store\Invalid\",
            CompletePath = @".\Database\Store\Complete\";

        private static HttpListener BuyListener, IPNListener;
        public static ConcurrentQueue<IPNMessage> Messages = new ConcurrentQueue<IPNMessage>();
        public static List<IPNMessage> PaymentList = new List<IPNMessage>(), HandledPayments = new List<IPNMessage>();

        static WebServer()
        {
            Messages = new ConcurrentQueue<IPNMessage>();

            PaymentList.Clear();

            if (Directory.Exists(VerifiedPath))
            {
                string[] files = Directory.GetFiles(VerifiedPath);

                foreach (string file in files)
                    Messages.Enqueue(new IPNMessage { FileName = file, Message = File.ReadAllText(file), Verified = true });
            }
        }

        public static void StartWebServer(bool log = true)
        {
            try
            {
                WebCommandQueue = new ConcurrentQueue<WebCommand>();

                WebListener = new HttpListener();
                WebListener.Prefixes.Add(Config.WebPrefix);

                WebListener.Start();
                WebListener.BeginGetContext(WebConnection, null);

                BuyListener = new HttpListener();
                BuyListener.Prefixes.Add(Config.BuyPrefix);

                IPNListener = new HttpListener();
                IPNListener.Prefixes.Add(Config.IPNPrefix);

                BuyListener.Start();
                BuyListener.BeginGetContext(BuyConnection, null);

                IPNListener.Start();
                IPNListener.BeginGetContext(IPNConnection, null);



                WebServerStarted = true;

                if (log) SEnvir.Log("Web Server Started.");
            }
            catch (Exception ex)
            {
                WebServerStarted = false;
                SEnvir.Log(ex.ToString());

                if (WebListener != null && WebListener.IsListening)
                    WebListener?.Stop();
                WebListener = null;

                if (BuyListener != null && BuyListener.IsListening)
                    BuyListener?.Stop();
                BuyListener = null;

                if (IPNListener != null && IPNListener.IsListening)
                    IPNListener?.Stop();
                IPNListener = null;
            }
        }
        public static void StopWebServer(bool log = true)
        {
            HttpListener expiredWebListener = WebListener;
            WebListener = null;

            HttpListener expiredBuyListener = BuyListener;
            BuyListener = null;
            HttpListener expiredIPNListener = IPNListener;
            IPNListener = null;


            WebServerStarted = false;
            expiredWebListener?.Stop();
            expiredBuyListener?.Stop();
            expiredIPNListener?.Stop();

            if (log) SEnvir.Log("Web Server Stopped.");
        }

        private static void WebConnection(IAsyncResult result)
        {
            try
            {
                HttpListenerContext context = WebListener.EndGetContext(result);

                string command = context.Request.QueryString["Type"];

                switch (command)
                {
                    case ActivationCommand:
                        Activation(context);
                        break;
                    case ResetCommand:
                        ResetPassword(context);
                        break;
                    case DeleteCommand:
                        DeleteAccount(context);
                        break;
                    case SystemDBSyncCommand:
                        SystemDBSync(context);
                        break;
                }
            }
            catch { }
            finally
            {
                if (WebListener != null && WebListener.IsListening)
                    WebListener.BeginGetContext(WebConnection, null);
            }
        }

        private static void SystemDBSync(HttpListenerContext context)
        {
            if (!Config.AllowSystemDBSync)
                return;

            if (context.Request.HttpMethod != "POST" || !context.Request.HasEntityBody)
                return;

            if (context.Request.ContentLength64 > 1024 * 1024 * 10)
                return;

            var masterPassword = context.Request.QueryString["mp"];
            if (string.IsNullOrEmpty(masterPassword) || !masterPassword.Equals(Config.MasterPassword))
                return;

            var buffer = new byte[context.Request.ContentLength64];
            context.Request.InputStream.Read(buffer, 0, buffer.Length);

            var tmpPath = SEnvir.Session.SystemPath + Session.TempExtension;

            using (var ms = new MemoryStream(buffer))
            using (var oms = File.Create(tmpPath))
            using (GZipStream compress = new GZipStream(oms, CompressionMode.Decompress))
                ms.CopyTo(compress);

            if (SEnvir.Session.BackUp && !Directory.Exists(SEnvir.Session.SystemBackupPath))
                Directory.CreateDirectory(SEnvir.Session.SystemBackupPath);

            if (File.Exists(SEnvir.Session.SystemPath))
            {
                if (SEnvir.Session.BackUp)
                {
                    using (FileStream sourceStream = File.OpenRead(SEnvir.Session.SystemPath))
                    using (FileStream destStream = File.Create(SEnvir.Session.SystemBackupPath + "System " + SEnvir.Session.ToBackUpFileName(DateTime.UtcNow) + Session.Extension + Session.CompressExtension))
                    using (GZipStream compress = new GZipStream(destStream, CompressionMode.Compress))
                        sourceStream.CopyTo(compress);
                }

                File.Delete(SEnvir.Session.SystemPath);
            }

            File.Move(tmpPath, SEnvir.Session.SystemPath);
        }

        private static void Activation(HttpListenerContext context)
        {
            string key = context.Request.QueryString[ActivationKey];

            if (string.IsNullOrEmpty(key)) return;

            AccountInfo account = null;
            for (int i = 0; i < SEnvir.AccountInfoList.Count; i++)
            {
                AccountInfo temp = SEnvir.AccountInfoList[i]; //Different Threads, Caution must be taken to prevent errors
                if (string.Compare(temp.ActivationKey, key, StringComparison.Ordinal) != 0) continue;

                account = temp;
                break;
            }

            if (Config.AllowWebActivation && account != null)
            {
                WebCommandQueue.Enqueue(new WebCommand(CommandType.Activation, account));
                context.Response.Redirect(Config.ActivationSuccessLink);
            }
            else
                context.Response.Redirect(Config.ActivationFailLink);

            context.Response.Close();
        }
        private static void ResetPassword(HttpListenerContext context)
        {
            string key = context.Request.QueryString[ResetKey];

            if (string.IsNullOrEmpty(key)) return;

            AccountInfo account = null;
            for (int i = 0; i < SEnvir.AccountInfoList.Count; i++)
            {
                AccountInfo temp = SEnvir.AccountInfoList[i]; //Different Threads, Caution must be taken to prevent errors
                if (string.Compare(temp.ResetKey, key, StringComparison.Ordinal) != 0) continue;

                account = temp;
                break;
            }

            if (Config.AllowWebResetPassword && account != null && account.ResetTime.AddMinutes(25) > SEnvir.Now)
            {
                WebCommandQueue.Enqueue(new WebCommand(CommandType.PasswordReset, account));
                context.Response.Redirect(Config.ResetSuccessLink);
            }
            else
                context.Response.Redirect(Config.ResetFailLink);

            context.Response.Close();
        }
        private static void DeleteAccount(HttpListenerContext context)
        {
            string key = context.Request.QueryString[DeleteKey];

            AccountInfo account = null;
            for (int i = 0; i < SEnvir.AccountInfoList.Count; i++)
            {
                AccountInfo temp = SEnvir.AccountInfoList[i]; //Different Threads, Caution must be taken to prevent errors
                if (string.Compare(temp.ActivationKey, key, StringComparison.Ordinal) != 0) continue;

                account = temp;
                break;
            }

            if (Config.AllowDeleteAccount && account != null)
            {
                WebCommandQueue.Enqueue(new WebCommand(CommandType.AccountDelete, account));
                context.Response.Redirect(Config.DeleteSuccessLink);
            }
            else
                context.Response.Redirect(Config.DeleteFailLink);

            context.Response.Close();
        }

        private static void BuyConnection(IAsyncResult result)
        {
            try
            {
                HttpListenerContext context = BuyListener.EndGetContext(result);

                string characterName = context.Request.QueryString["Character"];

                CharacterInfo character = null;
                for (int i = 0; i < SEnvir.CharacterInfoList.Count; i++)
                {
                    if (string.Compare(SEnvir.CharacterInfoList[i].CharacterName, characterName, StringComparison.OrdinalIgnoreCase) != 0) continue;

                    character = SEnvir.CharacterInfoList[i];
                    break;
                }

                if (character?.Account.Key != context.Request.QueryString["Key"])
                    character = null;

                string response = character == null ? Properties.Resources.CharacterNotFound : Properties.Resources.BuyGameGold.Replace("$CHARACTERNAME$", character.CharacterName);

                using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, context.Request.ContentEncoding))
                    writer.Write(response);
            }
            catch { }
            finally
            {
                if (BuyListener != null && BuyListener.IsListening) //IsBound ?
                    BuyListener.BeginGetContext(BuyConnection, null);
            }

        }
        private static void IPNConnection(IAsyncResult result)
        {
            const string LiveURL = @"https://ipnpb.paypal.com/cgi-bin/webscr";

            const string verified = "VERIFIED";

            try
            {
                if (IPNListener == null || !IPNListener.IsListening) return;

                HttpListenerContext context = IPNListener.EndGetContext(result);

                string rawMessage;
                using (StreamReader readStream = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                    rawMessage = readStream.ReadToEnd();


                Task.Run(() =>
                {
                    string data = "cmd=_notify-validate&" + rawMessage;

                    HttpWebRequest wRequest = (HttpWebRequest)WebRequest.Create(LiveURL);

                    wRequest.Method = "POST";
                    wRequest.ContentType = "application/x-www-form-urlencoded";
                    wRequest.ContentLength = data.Length;

                    using (StreamWriter writer = new StreamWriter(wRequest.GetRequestStream(), Encoding.ASCII))
                        writer.Write(data);

                    using (StreamReader reader = new StreamReader(wRequest.GetResponse().GetResponseStream()))
                    {
                        IPNMessage message = new IPNMessage { Message = rawMessage, Verified = reader.ReadToEnd() == verified };


                        if (!Directory.Exists(VerifiedPath))
                            Directory.CreateDirectory(VerifiedPath);

                        if (!Directory.Exists(InvalidPath))
                            Directory.CreateDirectory(InvalidPath);

                        string path = (message.Verified ? VerifiedPath : InvalidPath) + Path.GetRandomFileName();

                        File.WriteAllText(path, message.Message);

                        message.FileName = path;


                        Messages.Enqueue(message);
                    }
                });

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
            finally
            {
                if (IPNListener != null && IPNListener.IsListening) //IsBound ?
                    IPNListener.BeginGetContext(IPNConnection, null);
            }
        }

        public static void Process()
        {
            while (!WebCommandQueue.IsEmpty)
            {
                if (!WebCommandQueue.TryDequeue(out WebCommand webCommand)) continue;

                switch (webCommand.Command)
                {
                    case CommandType.None:
                        break;
                    case CommandType.Activation:
                        webCommand.Account.Activated = true;
                        webCommand.Account.ActivationKey = string.Empty;
                        break;
                    case CommandType.PasswordReset:
                        string password = Functions.RandomString(SEnvir.Random, 10);

                        webCommand.Account.Password = SEnvir.CreateHash(password);
                        webCommand.Account.ResetKey = string.Empty;
                        webCommand.Account.WrongPasswordCount = 0;
                        EmailService.SendResetPasswordEmail(webCommand.Account, password);
                        break;
                    case CommandType.AccountDelete:
                        if (webCommand.Account.Activated) continue;

                        webCommand.Account.Delete();
                        break;
                }
            }
        }

        public static void Save()
        {
            HandledPayments.AddRange(PaymentList);
        }

        public static void CommitChanges(object data)
        {
            foreach (IPNMessage message in HandledPayments)
            {
                if (message.Duplicate)
                {
                    File.Delete(message.FileName);
                    continue;
                }

                if (!Directory.Exists(CompletePath))
                    Directory.CreateDirectory(CompletePath);

                File.Move(message.FileName, CompletePath + Path.GetFileName(message.FileName) + ".txt");
                PaymentList.Remove(message);
            }
            HandledPayments.Clear();
        }
    }
}
