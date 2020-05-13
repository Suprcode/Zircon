using Library;
using Server.DBModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Server.Envir
{
    public static class EmailService
    {
        public static int EMailsSent;

        public static void SendActivationEmail(AccountInfo account)
        {
            account.ActivationKey = Functions.RandomString(SEnvir.Random, 20);
            account.ActivationTime = SEnvir.Now.AddMinutes(5);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {

                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,

                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "Zircon Account Activation",
                        IsBodyHtml = true,

                        Body = $"Dear {account.RealName}, <br><br>" +
                               $"Thank you for registering a Zircon account, before you can log in to the game, you are required to activate your account.<br><br>" +
                               $"To complete your registration and activate the account please visit the following link:<br>" +
                               $"<a href=\"{Config.WebCommandLink}?Type={WebServer.ActivationCommand}&{WebServer.ActivationKey}={account.ActivationKey}\">Click here to Activate</a><br><br>" +
                               $"If the above link does not work please use the following Activation Key when you next attempt to log in to your account<br>" +
                               $"Activation Key: {account.ActivationKey}<br><br>" +
                               (account.Referral != null ? $"You were referred by: {account.Referral.EMailAddress}<br><br>" : "") +
                               $"If you did not create this account and want to cancel the registration to delete this account please visit the following link:<br>" +
                               $"<a href=\"{Config.WebCommandLink}?Type={WebServer.DeleteCommand}&{WebServer.DeleteKey}={account.ActivationKey}\">Click here to Delete Account</a><br><br>" +
                               $"We'll see you in game<br>" +
                               $"<a href=\"http://www.zirconserver.com\">Zircon Server</a>"
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.Message);
                    SEnvir.Log(ex.StackTrace);
                }
            });
        }
        public static void ResendActivationEmail(AccountInfo account)
        {
            if (string.IsNullOrEmpty(account.ActivationKey))
                account.ActivationKey = Functions.RandomString(SEnvir.Random, 20);

            account.ActivationTime = SEnvir.Now.AddMinutes(15);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "Zircon Account Activation",
                        IsBodyHtml = false,

                        Body = $"Dear {account.RealName}\n" +
                               $"\n" +
                               $"Thank you for registering a Zircon account, before you can log in to the game, you are required to activate your account.\n" +
                               $"\n" +
                               $"Please use the following Activation Key when you next attempt to log in to your account\n" +
                               $"Activation Key: {account.ActivationKey}\n\n" +
                               $"We'll see you in game\n" +
                               $"Zircon Server\n" +
                               $"\n" +
                               $"This E-Mail has been sent without formatting to reduce failure",
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.Message);
                    SEnvir.Log(ex.StackTrace);
                }
            });
        }
        public static void SendChangePasswordEmail(AccountInfo account, string ipAddress)
        {
            if (SEnvir.Now < account.PasswordTime)
                return;

            account.PasswordTime = Time.Now.AddMinutes(60);

            EMailsSent++;
            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "Zircon Password Changed",
                        IsBodyHtml = true,

                        Body = $"Dear {account.RealName}, <br><br>" +
                               $"This is an E-Mail to inform you that your password for Zircon has been changed.<br>" +
                               $"IP Address: {ipAddress}<br><br>" +
                               $"If you did not make this change please contact an administrator immediately.<br><br>" +
                               $"We'll see you in game<br>" +
                               $"<a href=\"http://www.zirconserver.com\">Zircon Server</a>"
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.Message);
                    SEnvir.Log(ex.StackTrace);
                }
            });
        }
        public static void SendResetPasswordRequestEmail(AccountInfo account, string ipAddress)
        {
            account.ResetKey = Functions.RandomString(SEnvir.Random, 20);
            account.ResetTime = SEnvir.Now.AddMinutes(5);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "Zircon Password Reset",
                        IsBodyHtml = true,

                        Body = $"Dear {account.RealName}, <br><br>" +
                               $"A request to reset your password has been made.<br>" +
                               $"IP Address: {ipAddress}<br><br>" +
                               $"To reset your password please click on the following link:<br>" +
                               $"<a href=\"{Config.WebCommandLink}?Type={WebServer.ResetCommand}&{WebServer.ResetKey}={account.ResetKey}\">Reset Password</a><br><br>" +
                               $"If the above link does not work please use the following Reset Key to reset your password<br>" +
                               $"Reset Key: {account.ResetKey}<br><br>" +
                               $"If you did not request this reset, please ignore this email as your password will not be changed.<br><br>" +
                               $"We'll see you in game<br>" +
                               $"<a href=\"http://www.zirconserver.com\">Zircon Server</a>"
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.Message);
                    SEnvir.Log(ex.StackTrace);
                }
            });
        }
        public static void SendResetPasswordEmail(AccountInfo account, string password)
        {
            account.ResetKey = Functions.RandomString(SEnvir.Random, 20);
            account.ResetTime = SEnvir.Now.AddMinutes(5);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "Zircon Password has been Reset.",
                        IsBodyHtml = true,

                        Body = $"Dear {account.RealName}, <br><br>" +
                               $"This is an E-Mail to inform you that your password for Zircon has been reset.<br>" +
                               $"Your new password: {password}<br><br>" +
                               $"If you did not make this reset please contact an administrator immediately.<br><br>" +
                               $"We'll see you in game<br>" +
                               $"<a href=\"http://www.zirconserver.com\">Zircon Server</a>"
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.Message);
                    SEnvir.Log(ex.StackTrace);
                }
            });
        }

    }
}
