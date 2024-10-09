using Client.Controls;
using Client.Models;
using Client.Scenes;
using Client.Scenes.Views;
using Library;
using Library.Network;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using G = Library.Network.GeneralPackets;
using S = Library.Network.ServerPackets;

namespace Client.Envir
{
    public sealed class CConnection : BaseConnection
    {
        protected override TimeSpan TimeOutDelay => Config.TimeOutDuration;

        public bool ServerConnected { get; set; }

        public int Ping;

        public CConnection(TcpClient client)
            : base(client)
        {
            OnException += (o, e) => CEnvir.SaveException(e);

            UpdateTimeOut();

            AdditionalLogging = true;

            BeginReceive();
        }

        public override void TryDisconnect()
        {
            Disconnect();
        }
        public override void Disconnect()
        {
            base.Disconnect();

            if (CEnvir.Connection == this)
            {
                CEnvir.Connection = null;

                LoginScene scene = DXControl.ActiveScene as LoginScene;
                if (scene != null)
                {
                    scene.Disconnected();
                }
                else
                {
                    DXMessageBox.Show("Disconnected from server\nReason: Connection timed out.", "Disconnected", DialogAction.ReturnToLogin);
                }
            }

            CEnvir.Storage = null;
            CEnvir.PartsStorage = null;
        }
        public override void TrySendDisconnect(Packet p)
        {
            SendDisconnect(p);
        }

        public void Process(G.Disconnect p)
        {
            Disconnecting = true;

            LoginScene scene = DXControl.ActiveScene as LoginScene;

            if (scene != null)
            {
                if (p.Reason == DisconnectReason.WrongVersion)
                {
                    CEnvir.WrongVersion = true;

                    DXMessageBox.Show("Disconnected from server\nReason: Wrong Version.", "Disconnected", DialogAction.Close).Modal = false;
                }

                scene.Disconnected();
                return;
            }


            switch (p.Reason)
            {
                case DisconnectReason.Unknown:
                    DXMessageBox.Show("Disconnected from server\nReason: Unknown", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.TimedOut:
                    DXMessageBox.Show("Disconnected from server\nReason: Connection Timed out.", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.ServerClosing:
                    DXMessageBox.Show("Disconnected from server\nReason: Server shutting down.", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.AnotherUser:
                    DXMessageBox.Show("Disconnected from server\nReason: Another user logged onto your account.", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.AnotherUserAdmin:
                    DXMessageBox.Show("Disconnected from server\nReason: An admin logged onto your account.", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.Banned:
                    DXMessageBox.Show("Disconnected from server\nReason: You have been banned.", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.Kicked:
                    DXMessageBox.Show("Disconnected from server\nReason: You have been kicked.", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.Crashed:
                    DXMessageBox.Show("Disconnected from server\nReason: Server Crashed.", "Disconnected", DialogAction.ReturnToLogin);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (this == CEnvir.Connection)
                CEnvir.Connection = null;
        }

        public void Process(G.Connected p)
        {
            Enqueue(new G.Connected());
            ServerConnected = true;

        }
        public void Process(G.CheckVersion p)
        {
            byte[] clientHash;
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(Application.ExecutablePath))
                    clientHash = md5.ComputeHash(stream);
            }

            Enqueue(new G.Version { ClientHash = clientHash });
        }
        public void Process(G.GoodVersion p)
        {
            Encryption.SetKey(p.DatabaseKey);

            LoginScene scene = DXControl.ActiveScene as LoginScene;

            scene?.LoadDatabase();

            Enqueue(new C.SelectLanguage { Language = Config.Language });
        }
        public void Process(G.Ping p)
        {
            Enqueue(new G.Ping());
        }
        public void Process(G.PingResponse p)
        {
            Ping = p.Ping;
        }

        public void Process(S.NewAccount p)
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.AccountBox.CreateAttempted = false;

            switch (p.Result)
            {
                case NewAccountResult.Disabled:
                    login.AccountBox.Clear();
                    DXMessageBox.Show("Account Creation is currently disabled.", "Account Creation");
                    break;
                case NewAccountResult.BadEMail:
                    login.AccountBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail address is not acceptable.", "Account Creation");
                    break;
                case NewAccountResult.BadPassword:
                    login.AccountBox.Password1TextBox.SetFocus();
                    DXMessageBox.Show("Password is not acceptable.", "Account Creation");
                    break;
                case NewAccountResult.BadRealName:
                    login.AccountBox.RealNameTextBox.SetFocus();
                    DXMessageBox.Show("Real Name is not acceptable.", "Account Creation");
                    break;
                case NewAccountResult.AlreadyExists:
                    login.AccountBox.EMailTextBox.TextBox.Text = string.Empty;
                    login.AccountBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail address already in use.", "Account Creation");
                    break;
                case NewAccountResult.BadReferral:
                    login.AccountBox.ReferralTextBox.SetFocus();
                    DXMessageBox.Show("Referral's E-Mail address is not acceptable.", "Account Creation");
                    break;
                case NewAccountResult.ReferralNotFound:
                    login.AccountBox.ReferralTextBox.SetFocus();
                    DXMessageBox.Show("Referral's E-Mail address was not found.", "Account Creation");
                    break;
                case NewAccountResult.ReferralNotActivated:
                    login.AccountBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Referral's E-Mail address has not been activated.", "Account Creation");
                    break;
                case NewAccountResult.Success:
                    login.LoginBox.EMailTextBox.TextBox.Text = login.AccountBox.EMailTextBox.TextBox.Text;
                    login.LoginBox.PasswordTextBox.TextBox.Text = login.AccountBox.Password1TextBox.TextBox.Text;
                    login.AccountBox.Clear();
                    DXMessageBox.Show("Your account was created successfully.\n" +
                                      "Please follow the instructions sent to your E-Mail to activate.", "Account Creation");
                    break;
            }

        }
        public void Process(S.ChangePassword p)
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.ChangeBox.ChangeAttempted = false;

            switch (p.Result)
            {
                case ChangePasswordResult.Disabled:
                    login.ChangeBox.Clear();
                    DXMessageBox.Show("Change Password is currently disabled.", "Change Password");
                    break;
                case ChangePasswordResult.BadEMail:
                    login.ChangeBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail is not acceptable.", "Change Password");
                    break;
                case ChangePasswordResult.BadCurrentPassword:
                    login.ChangeBox.CurrentPasswordTextBox.SetFocus();
                    DXMessageBox.Show("Current Password is not acceptable.", "Change Password");
                    break;
                case ChangePasswordResult.BadNewPassword:
                    login.ChangeBox.NewPassword1TextBox.SetFocus();
                    DXMessageBox.Show("New Password is not acceptable.", "Change Password");
                    break;
                case ChangePasswordResult.AccountNotFound:
                    login.ChangeBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Account does not exist.", "Change Password");
                    break;
                case ChangePasswordResult.AccountNotActivated:
                    login.ShowActivationBox(login.ChangeBox);
                    break;
                case ChangePasswordResult.WrongPassword:
                    login.ChangeBox.CurrentPasswordTextBox.SetFocus();
                    DXMessageBox.Show("Incorrect Password.", "Change Password");
                    break;
                case ChangePasswordResult.Banned:
                    DateTime expiry = CEnvir.Now.Add(p.Duration);
                    DXMessageBox box = DXMessageBox.Show($"This account is banned.\n\nReason: {p.Message}\n" +
                                                         $"Expiary Date: {expiry}\n" +
                                                         $"Duration: {Math.Floor(p.Duration.TotalHours):#,##0} Hours, {p.Duration.Minutes} Minutes, {p.Duration.Seconds} Seconds", "Change Password");

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.ChangeBox.CanChange)
                                login.ChangeBox.Change();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"This account is banned.\n\n" +
                                         $"Reason: {p.Message}\n" +
                                         $"Expiary Date: {expiry}\n" +
                                         $"Duration: {Math.Floor(remaining.TotalHours):#,##0} Hours, {remaining.Minutes} Minutes, {remaining.Seconds} Seconds";
                    };
                    break;
                case ChangePasswordResult.Success:
                    login.ChangeBox.Clear();
                    DXMessageBox.Show("Password changed successfully.", "Change Password");
                    break;
            }

        }
        public void Process(S.RequestPasswordReset p)
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.RequestPasswordBox.RequestAttempted = false;

            DateTime expiry;
            DXMessageBox box;
            switch (p.Result)
            {
                case RequestPasswordResetResult.Disabled:
                    login.RequestPasswordBox.Clear();
                    DXMessageBox.Show("Password Reset is currently disabled.", "Reset Password");
                    break;
                case RequestPasswordResetResult.BadEMail:
                    login.RequestPasswordBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail is not acceptable.", "Reset Password");
                    break;
                case RequestPasswordResetResult.AccountNotFound:
                    login.RequestPasswordBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Account does not exist.", "Reset Password");
                    break;
                case RequestPasswordResetResult.AccountNotActivated:
                    login.ShowActivationBox(login.RequestPasswordBox);
                    break;
                case RequestPasswordResetResult.ResetDelay:
                    expiry = CEnvir.Now.Add(p.Duration);
                    box = DXMessageBox.Show($"You cannot request another password reset so soon.\n" +
                                            $"Next available Reset: {expiry}\n" +
                                            $"Duration: {Math.Floor(p.Duration.TotalHours):#,##0} Hours, {p.Duration.Minutes} Minutes, {p.Duration.Seconds} Seconds", "Reset Password");

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.RequestPasswordBox.CanReset)
                                login.RequestPasswordBox.Request();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"You cannot request another password reset so soon.\n" +
                                         $"Next Possible Reset: {expiry}\n" +
                                         $"Duration: {Math.Floor(remaining.TotalHours):#,##0} Hours, {remaining.Minutes} Minutes, {remaining.Seconds} Seconds";
                    };
                    break;
                case RequestPasswordResetResult.Banned:
                    expiry = CEnvir.Now.Add(p.Duration);
                    box = DXMessageBox.Show($"This account is banned.\n\nReason: {p.Message}\n" +
                                            $"Expiary Date: {expiry}\n" +
                                            $"Duration: {Math.Floor(p.Duration.TotalHours):#,##0} Hours, {p.Duration.Minutes} Minutes, {p.Duration.Seconds} Seconds", "Reset Password");

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.RequestPasswordBox.CanReset)
                                login.RequestPasswordBox.Request();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"This account is banned.\n\n" +
                                         $"Reason: {p.Message}\n" +
                                         $"Expiary Date: {expiry}\n" +
                                         $"Duration: {Math.Floor(remaining.TotalHours):#,##0} Hours, {remaining.Minutes} Minutes, {remaining.Seconds} Seconds";
                    };
                    break;
                case RequestPasswordResetResult.Success:
                    login.RequestPasswordBox.Clear();
                    DXMessageBox.Show("Password reset request success\n" +
                                      "Please check your E-Mail for further instructions.", "Reset Password");
                    break;
            }

        }
        public void Process(S.ResetPassword p)
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.ResetBox.ResetAttempted = false;

            switch (p.Result)
            {
                case ResetPasswordResult.Disabled:
                    login.ResetBox.Clear();
                    DXMessageBox.Show("Manual password reset is currently disabled.", "Reset Password");
                    break;
                case ResetPasswordResult.BadNewPassword:
                    login.ResetBox.NewPassword1TextBox.SetFocus();
                    DXMessageBox.Show("New Password is not acceptable.", "Reset Password");
                    break;
                case ResetPasswordResult.AccountNotFound:
                    login.ResetBox.ResetKeyTextBox.SetFocus();
                    DXMessageBox.Show("Account could not be found.", "Reset Password");
                    break;
                case ResetPasswordResult.Success:
                    login.ResetBox.Clear();
                    DXMessageBox.Show("Password changed successfully.", "Reset Password");
                    break;
            }

        }
        public void Process(S.Activation p)
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.ActivationBox.ActivationAttempted = false;

            switch (p.Result)
            {
                case ActivationResult.Disabled:
                    login.ActivationBox.Clear();
                    DXMessageBox.Show("Manual Activation is currently disabled.", "Activation");
                    break;
                case ActivationResult.AccountNotFound:
                    login.ActivationBox.ActivationKeyTextBox.SetFocus();
                    DXMessageBox.Show("Account could not be found.", "Activation");
                    break;
                case ActivationResult.Success:
                    login.ActivationBox.Clear();
                    DXMessageBox.Show("Your account has been activated successfully\n", "Activation");
                    break;
            }

        }
        public void Process(S.RequestActivationKey p)
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.RequestActivationBox.RequestAttempted = false;

            DateTime expiry;
            DXMessageBox box;
            switch (p.Result)
            {
                case RequestActivationKeyResult.Disabled:
                    login.RequestActivationBox.Clear();
                    DXMessageBox.Show("Password Reset is currently disabled.", "Request Activation Key");
                    break;
                case RequestActivationKeyResult.BadEMail:
                    login.RequestActivationBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail is not acceptable.", "Request Activation Key");
                    break;
                case RequestActivationKeyResult.AccountNotFound:
                    login.RequestActivationBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Account does not exist.", "Request Activation Key");
                    break;
                case RequestActivationKeyResult.AlreadyActivated:
                    login.RequestActivationBox.Clear();
                    DXMessageBox.Show("Account is already activated.", "Request Activation Key");
                    break;
                case RequestActivationKeyResult.RequestDelay:
                    expiry = CEnvir.Now.Add(p.Duration);
                    box = DXMessageBox.Show($"Cannot request another activation e-mail so soon.\n" +
                                            $"Next available request: {expiry}\n" +
                                            $"Duration: {Math.Floor(p.Duration.TotalHours):#,##0} Hours, {p.Duration.Minutes} Minutes, {p.Duration.Seconds} Seconds", "Request Activation Key");

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.RequestActivationBox.CanRequest)
                                login.RequestActivationBox.Request();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"Cannot request another activation e-mail so soon.\n" +
                                         $"Next Possible request: {expiry}\n" +
                                         $"Duration: {Math.Floor(remaining.TotalHours):#,##0} Hours, {remaining.Minutes} Minutes, {remaining.Seconds} Seconds";
                    };
                    break;
                case RequestActivationKeyResult.Success:
                    login.RequestActivationBox.Clear();
                    DXMessageBox.Show("Activation e-mail request was successful\n" +
                                      "Please check your E-Mail for further instructions.", "Request Activation Key");
                    break;
            }

        }
        public void Process(S.Login p)
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.LoginBox.LoginAttempted = false;

            SelectScene scene;
            switch (p.Result)
            {
                case LoginResult.Disabled:
                    DXMessageBox.Show("Logging in is currently disabled.", "Log In");
                    break;
                case LoginResult.BadEMail:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Username is not acceptable.", "Log In");
                    break;
                case LoginResult.BadPassword:
                    login.LoginBox.PasswordTextBox.SetFocus();
                    DXMessageBox.Show("Current Password is not acceptable.", "Log In");
                    break;
                case LoginResult.AccountNotExists:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Account does not exist.", "Log In");
                    break;
                case LoginResult.AccountNotActivated:
                    login.ShowActivationBox(login.LoginBox);
                    break;
                case LoginResult.WrongPassword:
                    login.LoginBox.PasswordTextBox.SetFocus();
                    DXMessageBox.Show("Incorrect Password.", "Log In");
                    break;
                case LoginResult.Banned:
                    DateTime expiry = CEnvir.Now.Add(p.Duration);

                    DXMessageBox box = DXMessageBox.Show($"This account is banned.\n\n" +
                                                         $"Reason: {p.Message}\n" +
                                                         $"Expiary Date: {expiry}\n" +
                                                         $"Duration: {Math.Floor(p.Duration.TotalHours):#,##0} Hours, {p.Duration.Minutes} Minutes, {p.Duration.Seconds} Seconds", "Log In");

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.LoginBox.CanLogin)
                                login.LoginBox.Login();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"This account is banned.\n\n" +
                                         $"Reason: {p.Message}\n" +
                                         $"Expiary Date: {expiry}\n" +
                                         $"Duration: {Math.Floor(remaining.TotalHours):#,##0} Hours, {remaining.Minutes} Minutes, {remaining.Seconds} Seconds";
                    };
                    break;
                case LoginResult.AlreadyLoggedIn:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Account is currently in use, please try again later.", "Log In");
                    break;
                case LoginResult.AlreadyLoggedInPassword:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Account is currently in use\n" +
                                      "New Password has been sent to the E-Mail Addresss..", "Log In");
                    break;
                case LoginResult.AlreadyLoggedInAdmin:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("Account is currently in use by an admin", "Log In");
                    break;
                case LoginResult.Success:
                    login.LoginBox.Visible = false;
                    login.AccountBox.Visible = false;
                    login.ChangeBox.Visible = false;
                    login.RequestPasswordBox.Visible = false;
                    login.ResetBox.Visible = false;
                    login.ActivationBox.Visible = false;
                    login.RequestActivationBox.Visible = false;

                    CEnvir.TestServer = p.TestServer;

                    if (Config.RememberDetails)
                    {
                        Config.RememberedEMail = login.LoginBox.EMailTextBox.TextBox.Text;
                        Config.RememberedPassword = login.LoginBox.PasswordTextBox.TextBox.Text;
                    }

                    login.Dispose();
                    DXSoundManager.Stop(SoundIndex.LoginScene2);
                    DXSoundManager.Play(SoundIndex.SelectScene);

                    p.Characters.Sort((x1, x2) => x2.LastLogin.CompareTo(x1.LastLogin));

                    DXControl.ActiveScene = scene = new SelectScene(Config.IntroSceneSize)
                    {
                        SelectBox = { CharacterList = p.Characters },
                    };

                    scene.SelectBox.UpdateCharacters();

                    CEnvir.BuyAddress = p.Address;
                    CEnvir.FillStorage(p.Items, false);

                    CEnvir.BlockList = p.BlockList;

                    if (!string.IsNullOrEmpty(p.Message)) DXMessageBox.Show(p.Message, "Login Message");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void Process(S.SelectLogout p)
        {
            CEnvir.ReturnToLogin();
            ((LoginScene)DXControl.ActiveScene).LoginBox.Visible = true;
        }
        public void Process(S.GameLogout p)
        {
            DXSoundManager.StopAllSounds();

            GameScene.Game.Dispose();

            DXSoundManager.Play(SoundIndex.SelectScene);

            SelectScene scene;

            p.Characters.Sort((x1, x2) => x2.LastLogin.CompareTo(x1.LastLogin));

            DXControl.ActiveScene = scene = new SelectScene(Config.IntroSceneSize)
            {
                SelectBox = { CharacterList = p.Characters },
            };

            CEnvir.Storage = CEnvir.MainStorage;
            CEnvir.PartsStorage = CEnvir.MainPartsStorage;

            scene.SelectBox.UpdateCharacters();
        }

        public void Process(S.NewCharacter p)
        {
            SelectScene select = DXControl.ActiveScene as SelectScene;
            if (select == null) return;

            select.CharacterBox.CreateAttempted = false;

            switch (p.Result)
            {
                case NewCharacterResult.Disabled:
                    select.CharacterBox.Clear();
                    DXMessageBox.Show("Character creation is currently disabled.", "Character Creation");
                    break;
                case NewCharacterResult.BadCharacterName:
                    select.CharacterBox.CharacterNameTextBox.SetFocus();
                    DXMessageBox.Show("Character Name is not acceptable.", "Character Creation");
                    break;
                case NewCharacterResult.BadHairType:
                    select.CharacterBox.HairNumberBox.Value = 1;
                    DXMessageBox.Show("Error: Invalid Hair Type.", "Character Creation");
                    break;
                case NewCharacterResult.BadHairColour:
                    DXMessageBox.Show("Error: Invalid Hair Colour.", "Character Creation");
                    break;
                case NewCharacterResult.BadArmourColour:
                    DXMessageBox.Show("Error: Invalid Armour Colour.", "Character Creation");
                    break;
                case NewCharacterResult.BadGender:
                    select.CharacterBox.SelectedGender = MirGender.Male;
                    DXMessageBox.Show("Error: Invalid gender selected.", "Character Creation");
                    break;
                case NewCharacterResult.BadClass:
                    select.CharacterBox.SelectedClass = MirClass.Warrior;
                    DXMessageBox.Show("Error: Invalid class selected.", "Character Creation");
                    break;
                case NewCharacterResult.ClassDisabled:
                    DXMessageBox.Show("The selected class is currently not available.", "Character Creation");
                    break;
                case NewCharacterResult.MaxCharacters:
                    select.CharacterBox.Clear();
                    DXMessageBox.Show("Character limit already reached.", "Character Creation");
                    break;
                case NewCharacterResult.AlreadyExists:
                    select.CharacterBox.CharacterNameTextBox.SetFocus();
                    DXMessageBox.Show("Character already exists.", "Character Creation");
                    break;
                case NewCharacterResult.Success:
                    select.CharacterBox.Clear();

                    select.SelectBox.CharacterList.Add(p.Character);
                    select.SelectBox.UpdateCharacters();
                    select.SelectBox.SelectedButton = select.SelectBox.SelectButtons[select.SelectBox.CharacterList.Count - 1];

                    DXMessageBox.Show("Character has been created.", "Character Creation");
                    break;
            }
        }
        public void Process(S.DeleteCharacter p)
        {
            SelectScene select = DXControl.ActiveScene as SelectScene;
            if (select == null) return;

            switch (p.Result)
            {
                case DeleteCharacterResult.Disabled:
                    DXMessageBox.Show("Character deletion is currently disabled.", "Delete Character");
                    break;
                case DeleteCharacterResult.AlreadyDeleted:
                    DXMessageBox.Show("Character has already been deleted.", "Delete Character");
                    break;
                case DeleteCharacterResult.NotFound:
                    DXMessageBox.Show("Character was not found.", "Delete Character");
                    break;
                case DeleteCharacterResult.Success:
                    for (int i = select.SelectBox.CharacterList.Count - 1; i >= 0; i--)
                    {
                        if (select.SelectBox.CharacterList[i].CharacterIndex != p.DeletedIndex) continue;

                        select.SelectBox.CharacterList.RemoveAt(i);
                        break;
                    }
                    select.SelectBox.UpdateCharacters();
                    DXMessageBox.Show("Character has been deleted.", "Delete Character");
                    break;
            }
        }
        public void Process(S.StartGame p)
        {
            try
            {

                SelectScene select = DXControl.ActiveScene as SelectScene;
                if (select == null) return;

                select.SelectBox.StartGameAttempted = false;


                DXMessageBox box;
                DateTime expiry;
                switch (p.Result)
                {
                    case StartGameResult.Disabled:
                        DXMessageBox.Show("Starting the game is currently disabled.", "Start Game");
                        break;
                    case StartGameResult.Deleted:
                        DXMessageBox.Show("You cannot start the game on a deleted character.", "Start Game");
                        break;
                    case StartGameResult.Delayed:
                        expiry = CEnvir.Now.Add(p.Duration);

                        box = DXMessageBox.Show($"This character has recently logged out please wait.\n" +
                                                $"Duration: {Math.Floor(p.Duration.TotalHours):#,##0} Hours, {p.Duration.Minutes} Minutes, {p.Duration.Seconds} Seconds", "Start Game");

                        box.ProcessAction = () =>
                        {
                            if (CEnvir.Now > expiry)
                            {
                                if (select.SelectBox.CanStartGame)
                                    select.SelectBox.StartGame();
                                box.ProcessAction = null;
                                return;
                            }

                            TimeSpan remaining = expiry - CEnvir.Now;

                            box.Label.Text = $"This character has recently logged out please wait.\n" +
                                             $"Duration: {Math.Floor(remaining.TotalHours):#,##0} Hours, {remaining.Minutes:#,##0} Minutes, {remaining.Seconds} Seconds";
                        };
                        break;
                    case StartGameResult.UnableToSpawn:
                        DXMessageBox.Show("Unable to start the game, failed to spawn character.", "Start Game");
                        break;
                    case StartGameResult.NotFound:
                        DXMessageBox.Show("Unable to start the game, character not found.", "Start Game");
                        break;
                    case StartGameResult.Success:
                        select.Dispose();
                        DXSoundManager.StopAllSounds();

                        GameScene scene = new GameScene(Config.GameSize);
                        DXControl.ActiveScene = scene;

                        scene.MapControl.MapInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == p.StartInformation.MapIndex);
                        scene.MapControl.InstanceInfo = Globals.InstanceInfoList.Binding.FirstOrDefault(x => x.Index == p.StartInformation.InstanceIndex);

                        GameScene.Game.QuestLog = p.StartInformation.Quests;

                        GameScene.Game.NPCAdoptCompanionBox.AvailableCompanions = p.StartInformation.AvailableCompanions;
                        GameScene.Game.NPCAdoptCompanionBox.RefreshUnlockButton();

                        GameScene.Game.NPCCompanionStorageBox.Companions = p.StartInformation.Companions;
                        GameScene.Game.NPCCompanionStorageBox.UpdateScrollBar();

                        GameScene.Game.Companion = GameScene.Game.NPCCompanionStorageBox.Companions.FirstOrDefault(x => x.Index == p.StartInformation.Companion);

                        scene.User = new UserObject(p.StartInformation);

                        GameScene.Game.BuffBox.BuffsChanged();
                        GameScene.Game.RankingBox.Observable = p.StartInformation.Observable;

                        GameScene.Game.StorageSize = p.StartInformation.StorageSize;

                        if (!string.IsNullOrEmpty(p.Message)) DXMessageBox.Show(p.Message, "Start Game");

                        GameScene.Game.CompanionBox.RefreshFilter();

                        GameScene.Game.CharacterBox.UpdateDiscipline();

                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void Process(S.MapChanged p)
        {
            GameScene.Game.MapControl.MapInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == p.MapIndex);

            GameScene.Game.MapControl.InstanceInfo = Globals.InstanceInfoList.Binding.FirstOrDefault(x => x.Index == p.InstanceIndex);

            MapObject.User.NameChanged();
        }
        public void Process(S.DayChanged p)
        {
            GameScene.Game.DayTime = p.DayTime;
        }
        public void Process(S.UserLocation p)
        {
            GameScene.Game.Displacement(p.Direction, p.Location);
        }
        public void Process(S.ObjectRemove p)
        {
            if (p.ObjectID == GameScene.Game.NPCID)
                GameScene.Game.NPCBox.Visible = false;

            if (MapObject.TargetObject != null && MapObject.TargetObject.ObjectID == p.ObjectID)
                MapObject.TargetObject = null;

            if (MapObject.MouseObject != null && MapObject.MouseObject.ObjectID == p.ObjectID)
                MapObject.MouseObject = null;

            if (MapObject.MagicObject != null && MapObject.MagicObject.ObjectID == p.ObjectID)
                MapObject.MagicObject = null;

            if (GameScene.Game.FocusObject != null && GameScene.Game.FocusObject.ObjectID == p.ObjectID)
                GameScene.Game.FocusObject = null;

            if (GameScene.Game.MonsterBox.Monster != null && GameScene.Game.MonsterBox.Monster.ObjectID == p.ObjectID)
                GameScene.Game.MonsterBox.Monster = null;

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Remove();
                return;
            }

        }
        public void Process(S.ObjectPlayer p)
        {
            new PlayerObject(p);
        }
        public void Process(S.ObjectItem p)
        {
            new ItemObject(p);
        }
        public void Process(S.ObjectMonster p)
        {
            new MonsterObject(p);

        }
        public void Process(S.ObjectNPC p)
        {
            new NPCObject(p);

        }
        public void Process(S.ObjectSpell p)
        {
            new SpellObject(p);
        }
        public void Process(S.ObjectSpellChanged p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                SpellObject spell = (SpellObject)ob;
                spell.Power = p.Power;
                spell.UpdateLibraries();
                return;
            }
        }
        public void Process(S.PlayerUpdate p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.Player || ob.ObjectID != p.ObjectID) continue;

                PlayerObject player = (PlayerObject)ob;

                player.LibraryWeaponShape = p.Weapon;

                player.ArmourShape = p.Armour;
                player.ArmourColour = p.ArmourColour;

                player.CostumeShape = p.Costume;

                player.HelmetShape = p.Helmet;
                player.HorseShape = p.HorseArmour;

                player.ShieldShape = p.Shield;

                player.HideHead = p.HideHead;

                player.ArmourEffect = p.ArmourEffect;
                player.EmblemEffect = p.EmblemEffect;
                player.WeaponEffect = p.WeaponEffect;
                player.ShieldEffect = p.ShieldEffect;

                player.Light = p.Light;
                if (player == MapObject.User)
                {
                    player.Light = Math.Max(p.Light, 3);

                    if (p.Light == 0)
                    {
                        player.LightColour = Globals.PlayerLightColour;
                    }
                    else 
                    {
                        player.LightColour = Globals.NoneColour;
                    }
                }

                player.UpdateLibraries();
                return;
            }
        }

        public void Process(S.ObjectTurn p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Standing, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectHarvest p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;
                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Harvest, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectShow p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob.Race == ObjectType.Monster)
                {
                    switch (((MonsterObject)ob).Image)
                    {
                        case MonsterImage.VoraciousGhost:
                        case MonsterImage.DevouringGhost:
                        case MonsterImage.CorpseRaisingGhost:
                            ob.Dead = false;
                            break;
                    }
                }

                ob.ActionQueue.Add(new ObjectAction(MirAction.Show, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectHide p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Hide, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectMove p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Moving, p.Direction, p.Location, p.Distance, MagicType.None));
                return;
            }
        }
        public void Process(S.ObjectPushed p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Pushed, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectNameColour p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.NameColour = p.Colour;
                return;
            }
        }
        public void Process(S.ObjectMount p)
        {
            if (MapObject.User.ObjectID == p.ObjectID)
            {
                MapObject.User.ServerTime = DateTime.MinValue;
                MapObject.User.NextActionTime = CEnvir.Now + Globals.TurnTime;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.Race != ObjectType.Player) return;

                PlayerObject player = (PlayerObject)ob;

                player.Horse = p.Horse;

                if (player.Interupt)
                    player.FrameStart = DateTime.MinValue;

                return;
            }
        }
        public void Process(S.MountFailed p)
        {
            MapObject.User.ServerTime = DateTime.MinValue;
            GameScene.Game.User.Horse = p.Horse;
        }

        public void Process(S.ObjectFishing p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob == MapObject.User)
                {
                    MapObject.User.ServerTime = DateTime.MinValue;

                    GameScene.Game.MapControl.FishingState = p.State;

                    MapObject.User.FishingState = p.State;
                    MapObject.User.FishFound = p.FishFound;
                    MapObject.User.FloatLocation = p.FloatLocation;

                    if (p.State == FishingState.Cancel)
                        GameScene.Game.MapControl.FishingState = FishingState.None;

                    GameScene.Game.FishingCatchBox.Visible = p.State == FishingState.Cast;

                    if (p.State == FishingState.Reel)
                    {
                        if (GameScene.Game.FishingCatchBox.AutoCastCheckBox.Checked)
                            GameScene.Game.MapControl.AutoCast = true;

                        MapObject.User.ActionQueue.Add(new ObjectAction(MirAction.Fishing, p.Direction, MapObject.User.CurrentLocation, p.State, p.FloatLocation, p.FishFound));
                    }
                }
                else
                {
                    ob.ActionQueue.Add(new ObjectAction(MirAction.Fishing, p.Direction, ob.CurrentLocation, p.State, p.FloatLocation, p.FishFound));
                }

                return;
            }
        }

        public void Process(S.FishingStats p)
        {
            GameScene.Game.FishingCatchBox.Update(p);
        }

        public void Process(S.ObjectStruck p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob == MapObject.User)
                {
                    if (GameScene.Game.MapControl.FishingState != FishingState.None)
                        GameScene.Game.MapControl.FishingState = FishingState.Cancel;

                    GameScene.Game.CanRun = false;
                    
                    if (GameScene.Game.StruckEnabled)
                    {
                        MapObject.User.NextRunTime = CEnvir.Now.AddMilliseconds(600);
                        MapObject.User.NextActionTime = CEnvir.Now.AddMilliseconds(300);

                        if (MapObject.User.ServerTime > DateTime.MinValue) //fix desyncing attack timers and being struck
                        {
                            switch (MapObject.User.CurrentAction)
                            {
                                case MirAction.Attack:
                                case MirAction.RangeAttack:
                                    MapObject.User.AttackTime += TimeSpan.FromMilliseconds(300);
                                    break;
                                case MirAction.Spell:
                                    MapObject.User.NextMagicTime += TimeSpan.FromMilliseconds(300);
                                    break;
                            }
                        }
                    }
                }

                if (GameScene.Game.StruckEnabled)
                {
                    Point loc = ob.ActionQueue.Count > 0 ? ob.ActionQueue[^1].Location : ob.CurrentLocation;

                    ob.ActionQueue.Add(new ObjectAction(MirAction.Struck, p.Direction, loc, p.AttackerID, p.Element));
                }
                else
                {
                    ob.Struck(p.AttackerID, p.Element);
                }
                return;
            }
        }
        public void Process(S.ObjectDash p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
                MapObject.User.ServerTime = DateTime.MinValue;

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.StanceTime = CEnvir.Now.AddSeconds(3);
                ob.ActionQueue.Add(new ObjectAction(MirAction.Standing, p.Direction, Functions.Move(p.Location, p.Direction, -p.Distance)));

                for (int i = 1; i <= p.Distance; i++)
                    ob.ActionQueue.Add(new ObjectAction(MirAction.Moving, p.Direction, Functions.Move(p.Location, p.Direction, i - p.Distance), 1, p.Magic));

                return;
            }
        }
        public void Process(S.ObjectAttack p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer && p.AttackMagic != MagicType.DanceOfSwallow)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Attack, p.Direction, p.Location, p.TargetID, p.AttackMagic, p.AttackElement));
                return;
            }
        }

        public void Process(S.ObjectMining p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                MapObject.User.MiningEffect = p.Effect;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Mining, p.Direction, p.Location, p.Effect));
                return;
            }
        }
        public void Process(S.ObjectRangeAttack p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.RangeAttack, p.Direction, p.Location, p.Targets, p.AttackMagic, p.AttackElement));
                return;
            }
        }
        public void Process(S.ObjectMagic p)
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.CurrentLocation || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.CurrentLocation);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.AttackTargets = new List<MapObject>();

                foreach (uint target in p.Targets)
                {
                    MapObject attackTarget = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == target);

                    if (attackTarget == null) continue;

                    MapObject.User.AttackTargets.Add(attackTarget);
                }

                MapObject.User.MagicLocations = p.Locations;
                MapObject.User.MagicCast = p.Cast;
                MapObject.User.NextActionTime += p.Slow;
                MapObject.User.AttackElement = p.AttackElement;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Spell, p.Direction, p.CurrentLocation, p.Type, p.Targets, p.Locations, p.Cast, p.AttackElement));
                return;
            }
        }
        public void Process(S.ObjectProjectile p)
        {
            MapObject source = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == p.ObjectID);

            if (source == null) return;

            var targets = new List<MapObject>();
            var locations = p.Locations;

            foreach (uint target in p.Targets)
            {
                MapObject attackTarget = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == target);

                if (attackTarget == null) continue;

                targets.Add(attackTarget);
            }

            MirEffect spell;

            switch (p.Type)
            {
                case MagicType.ChainLightning:
                    {
                        if (locations.Count > 0)
                            DXSoundManager.Play(SoundIndex.ChainLightningEnd);
                    }
                    break;
                case MagicType.LightningStrike:
                    {
                        foreach (MapObject attackTarget in targets)
                        {
                            source.Effects.Add(spell = new MirProjectile(500, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 35, 35, Globals.LightningColour, source.CurrentLocation)
                            {
                                Blend = true,
                                Target = attackTarget,
                                Skip = 0
                            });
                            spell.Process();
                        }

                        if (targets.Count > 0)
                            DXSoundManager.Play(SoundIndex.LightningBeamEnd);
                    }
                    break;
                case MagicType.FireBounce:
                    {
                        foreach (MapObject attackTarget in targets)
                        {
                            source.Effects.Add(spell = new MirProjectile(1640, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.FireColour, source.CurrentLocation, typeof(Client.Models.Particles.FireballTrail))
                            {
                                Blend = true,
                                Target = attackTarget,
                            });

                            spell.CompleteAction += () =>
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(1800, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();

                                DXSoundManager.Play(SoundIndex.GreaterFireBallEnd);
                            };
                            spell.Process();
                        }

                        if (targets.Count > 0)
                            DXSoundManager.Play(SoundIndex.GreaterFireBallTravel);
                    }
                    break;
            }
        }

        public void Process(S.ObjectDied p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Dead = true;
                ob.ActionQueue.Add(new ObjectAction(MirAction.Die, p.Direction, p.Location));

                if (ob == MapObject.User)
                    GameScene.Game.ReceiveChat(MessageAction.Revive);

                return;
            }
        }
        public void Process(S.ObjectHarvested p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Skeleton = true;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Dead, p.Direction, p.Location));

                return;
            }
        }
        public void Process(S.ObjectEffect p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                switch (p.Effect)
                {
                    case Effect.TeleportOut:
                        ob.Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 30, 60, Color.White)
                        {
                            MapTarget = ob.CurrentLocation,
                            Blend = true,
                            Reversed = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.TeleportOut);
                        break;
                    case Effect.TeleportIn:
                        ob.Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.TeleportIn);
                        break;
                    case Effect.ThunderBolt:
                        ob.Effects.Add(new MirEffect(1450, 3, TimeSpan.FromMilliseconds(150), LibraryFile.Magic, 150, 50, Globals.LightningColour)
                        {
                            Blend = true,
                            Target = ob
                        });

                        DXSoundManager.Play(SoundIndex.LightningStrikeEnd);
                        break;
                    case Effect.FullBloom:
                        ob.Effects.Add(new MirEffect(1700, 4, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.FullBloom);
                        break;
                    case Effect.WhiteLotus:
                        ob.Effects.Add(new MirEffect(1600, 12, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.WhiteLotus);
                        break;
                    case Effect.RedLotus:
                        ob.Effects.Add(new MirEffect(1700, 12, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.RedLotus);
                        break;
                    case Effect.SweetBrier:
                        ob.Effects.Add(new MirEffect(1900, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.SweetBrier);
                        break;
                    case Effect.Karma:
                        ob.Effects.Add(new MirEffect(1800, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.Karma);
                        break;

                    case Effect.Puppet:
                        ob.Effects.Add(new MirEffect(820, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.FireColour)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetFire:
                        ob.Effects.Add(new MirEffect(1546, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.FireColour)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetIce:
                        ob.Effects.Add(new MirEffect(2700, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.IceColour)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetLightning:
                        ob.Effects.Add(new MirEffect(2800, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.LightningColour)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetWind:
                        ob.Effects.Add(new MirEffect(2900, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.WindColour)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.DanceOfSwallow:
                        ob.Effects.Add(new MirEffect(1300, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.NoneColour)
                        {
                            Blend = true,
                            Target = ob,
                        });

                        DXSoundManager.Play(SoundIndex.DanceOfSwallowsEnd);
                        break;
                    case Effect.FlashOfLight:
                        ob.Effects.Add(new MirEffect(2400, 5, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.NoneColour)
                        {
                            Blend = true,
                            Target = ob,
                        });

                        DXSoundManager.Play(SoundIndex.FlashOfLightEnd);
                        break;
                    case Effect.DemonExplosion:
                        ob.Effects.Add(new MirEffect(3300, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx8, 30, 60, Globals.PhantomColour)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        //DXSoundManager.Play(SoundIndex.FlashOfLightEnd);
                        break;
                    case Effect.ParasiteExplode:
                        ob.Effects.Add(new MirEffect(700, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 30, 60, Globals.NoneColour)
                        {
                            Target = ob,
                            Blend = true
                        });

                        DXSoundManager.Play(SoundIndex.ParasiteExplode);
                        break;
                    case Effect.FrostBiteEnd:
                        ob.Effects.Add(new MirEffect(700, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 30, 60, Globals.IceColour)
                        {
                            Target = ob,
                            Blend = true,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.FireStormEnd);
                        break;
                    case Effect.ChainOfFireExplode:
                        var effect = new MirEffect(600, 12, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx10, 30, 60, Globals.FireColour)
                        {
                            Target = ob,
                            Blend = true
                        };

                        effect.FrameIndexAction = () =>
                        {
                            if (effect.FrameIndex == 8)
                                DXSoundManager.Play(SoundIndex.ChainofFireExplode);
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return;
            }
        }
        public void Process(S.MapEffect p)
        {
            switch (p.Effect)
            {
                case Effect.SummonSkeleton:
                    new MirEffect(750, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 30, 60, Globals.PhantomColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };

                    DXSoundManager.Play(SoundIndex.SummonSkeletonEnd);
                    break;
                case Effect.SummonShinsu:
                    new MirEffect(9640, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Mon_9, 30, 60, Globals.PhantomColour)
                    {
                        MapTarget = p.Location,
                        Direction = p.Direction,
                    };

                    DXSoundManager.Play(SoundIndex.SummonShinsuEnd);
                    break;
                case Effect.MirrorImage:
                    new MirEffect(1280, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 30, 60, Globals.NoneColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };

                    DXSoundManager.Play(SoundIndex.SummonSkeletonEnd);
                    break;
                case Effect.CursedDoll:
                    new MirEffect(700, 13, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 30, 60, Globals.NoneColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };
                    
                    DXSoundManager.Play(SoundIndex.CursedDollEnd);
                    break;
                case Effect.UndeadSoul:
                    new MirEffect(3300, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx20, 35, 10, Globals.NoneColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };
                    new MirEffect(400, 13, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx10, 35, 10, Globals.NoneColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };

                    DXSoundManager.Play(SoundIndex.SummonDeadEnd);
                    break;
                case Effect.BurningFireExplode:
                    new MirEffect(1100, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 30, 60, Globals.FireColour)
                    {
                        MapTarget = p.Location,
                        Blend = true
                    };

                    DXSoundManager.Play(SoundIndex.FireStormEnd);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void Process(S.ObjectBuffAdd p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.VisibleBuffs.Add(p.Type);

                if (p.Type == BuffType.SuperiorMagicShield)
                    ob.EndMagicEffect(MagicEffect.MagicShield);

                return;
            }
        }
        public void Process(S.ObjectBuffRemove p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.VisibleBuffs.Remove(p.Type);
                return;
            }
        }
        public void Process(S.ObjectPoison p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Poison = p.Poison;
                return;
            }
        }
        public void Process(S.ObjectPetOwnerChanged p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.Race != ObjectType.Monster) return;

                MonsterObject mob = (MonsterObject)ob;
                mob.PetOwner = p.PetOwner;
                return;
            }
        }
        public void Process(S.ObjectStats p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.Race == ObjectType.Monster)
                {
                    MonsterObject mob = (MonsterObject)ob;
                    p.Stats.Add(mob.MonsterInfo.Stats);
                }

                ob.Stats = p.Stats;
                return;
            }
        }
        public void Process(S.HealthChanged p)
        {


            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.CurrentHP += p.Change;
                ob.DrawHealthTime = CEnvir.Now.AddSeconds(5);
                ob.DamageList.Add(new DamageInfo { Value = p.Change, Block = p.Block, Critical = p.Critical, Miss = p.Miss });

                return;
            }
        }

        public void Process(S.NewMagic p)
        {
            MapObject.User.Magics[p.Magic.Info] = p.Magic;

            if (GameScene.Game.MagicBox.Magics.ContainsKey(p.Magic.Info))
                GameScene.Game.MagicBox.Magics[p.Magic.Info].Refresh();

            if (GameScene.Game.CharacterBox.DisciplineMagics.ContainsKey(p.Magic.Info))
                GameScene.Game.CharacterBox.DisciplineMagics[p.Magic.Info].Refresh();
        }

        public void Process(S.MagicLeveled p)
        {
            MapObject.User.Magics[p.Info].Level = p.Level;
            MapObject.User.Magics[p.Info].Experience = p.Experience;

            if (GameScene.Game.MagicBox.Magics.ContainsKey(p.Info))
                GameScene.Game.MagicBox.Magics[p.Info].Refresh();

            if (GameScene.Game.CharacterBox.DisciplineMagics.ContainsKey(p.Info))
                GameScene.Game.CharacterBox.DisciplineMagics[p.Info].Refresh();
        }

        public void Process(S.MagicCooldown p)
        {
            MapObject.User.Magics[p.Info].NextCast = CEnvir.Now.AddMilliseconds(p.Delay);
        }
        public void Process(S.MagicToggle p)
        {
            switch (p.Magic)
            {
                case MagicType.Slaying:
                    GameScene.Game.User.CanPowerAttack = p.CanUse;
                    break;
                case MagicType.Thrusting:
                    GameScene.Game.User.CanThrusting = p.CanUse;
                    break;
                case MagicType.HalfMoon:
                    GameScene.Game.User.CanHalfMoon = p.CanUse;
                    break;
                case MagicType.DestructiveSurge:
                    GameScene.Game.User.CanDestructiveSurge = p.CanUse;
                    break;
                case MagicType.FlamingSword:
                    GameScene.Game.User.CanFlamingSword = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat(CEnvir.Language.WeaponEnergyFlamingSword, MessageType.Hint);
                    break;
                case MagicType.DragonRise:
                    GameScene.Game.User.CanDragonRise = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat(CEnvir.Language.WeaponEnergyDragonRise, MessageType.Hint);
                    break;
                case MagicType.BladeStorm:
                    GameScene.Game.User.CanBladeStorm = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat(CEnvir.Language.WeaponEnergyBladeStorm, MessageType.Hint);
                    break;
                case MagicType.FlameSplash:
                    GameScene.Game.User.CanFlameSplash = p.CanUse;
                    break;
                case MagicType.DefensiveBlow:
                    GameScene.Game.User.CanDefensiveBlow = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat(CEnvir.Language.WeaponEnergyDefensiveBlow, MessageType.Hint);
                    break;
                case MagicType.FullBloom:
                case MagicType.WhiteLotus:
                case MagicType.RedLotus:
                case MagicType.SweetBrier:
                case MagicType.Karma:
                    if (GameScene.Game.User.AttackMagic == p.Magic)
                        GameScene.Game.User.AttackMagic = MagicType.None;
                    break;
                case MagicType.DragonBlood:
                    GameScene.Game.User.CanPoisonAttack = p.CanUse;
                    break;
                case MagicType.CalamityOfFullMoon:
                    GameScene.Game.User.CanFullMoonAttack = p.CanUse;
                    break;
                case MagicType.WaningMoon:
                    GameScene.Game.User.CanWaningMoonAttack = p.CanUse;
                    break;
            }
        }

        public void Process(S.ManaChanged p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.CurrentMP += p.Change;
                //ob.DamageList.Add(new DamageInfo(p.Change));
                return;
            }
        }

        public void Process(S.FocusChanged p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.CurrentFP += p.Change;

                return;
            }
        }

        public void Process(S.InformMaxExperience p)
        {
            MapObject.User.MaxExperience = p.MaxExperience;
        }
        public void Process(S.LevelChanged p)
        {
            MapObject.User.Level = p.Level;
            MapObject.User.Experience = p.Experience;
            MapObject.User.MaxExperience = p.MaxExperience;

            GameScene.Game.ReceiveChat(CEnvir.Language.LevelIncreased, MessageType.System);
        }
        public void Process(S.GainedExperience p)
        {
            MapObject.User.Experience += p.Amount;

            ClientUserItem weapon = GameScene.Game.Equipment[(int)EquipmentSlot.Weapon];

            if (p.Amount < 0)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.LostExperience, p.Amount), MessageType.Combat);
                return;
            }

            if (weapon != null && weapon.Info.ItemEffect != ItemEffect.PickAxe && (weapon.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable && (weapon.Flags & UserItemFlags.NonRefinable) != UserItemFlags.NonRefinable && weapon.Level < Globals.WeaponExperienceList.Count)
            {
                weapon.Experience += p.Amount / 10;

                if (weapon.Experience >= Globals.WeaponExperienceList[weapon.Level])
                {
                    weapon.Experience = 0;
                    weapon.Level++;
                    weapon.Flags |= UserItemFlags.Refinable;

                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GainedExperienceAndReadyForRefine, p.Amount), MessageType.Combat);
                }
                else
                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GainedExperienceAndWeaponExperience, p.Amount, p.Amount / 10), MessageType.Combat);
            }
        }
        public void Process(S.ObjectLeveled p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Effects.Add(new MirEffect(2030, 16, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 120, Color.DeepSkyBlue)
                {
                    Blend = true,
                    DrawColour = Color.RosyBrown,
                    Target = ob
                });

                return;
            }
        }
        public void Process(S.ObjectRevive p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Dead = false;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Standing, ob.Direction, p.Location));

                if (p.Effect)
                    ob.Effects.Add(new MirEffect(1110, 25, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 50, 90, Color.White)
                    {
                        Blend = true,
                        Target = ob
                    });

                GameScene.Game.MapControl.FLayer.TextureValid = false;

                return;
            }
        }

        public void Process(S.ItemsGained p)
        {
            foreach (ClientUserItem item in p.Items)
            {
                ItemInfo displayInfo = item.Info;

                if (item.Info.ItemEffect == ItemEffect.ItemPart)
                    displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == item.AddedStats[Stat.ItemIndex]);

                item.New = true;
                string text = item.Count > 1 ? string.Format(CEnvir.Language.ItemsGained, displayInfo.ItemName, item.Count) : string.Format(CEnvir.Language.ItemGained, displayInfo.ItemName);

                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem)
                    text += " (Quest)";

                if (item.Info.ItemEffect == ItemEffect.ItemPart)
                    text += " [Part]";

                GameScene.Game.ReceiveChat(text, MessageType.Combat);
            }

            GameScene.Game.AddItems(p.Items);
        }
        public void Process(S.ItemMove p)
        {
            DXItemCell fromCell, toCell;

            switch (p.FromGrid)
            {
                case GridType.Inventory:
                    fromCell = GameScene.Game.InventoryBox.Grid.Grid[p.FromSlot];
                    break;
                case GridType.Equipment:
                    fromCell = GameScene.Game.CharacterBox.Grid[p.FromSlot];
                    break;
                case GridType.Storage:
                    fromCell = GameScene.Game.StorageBox.Grid.Grid[p.FromSlot];
                    break;
                case GridType.PartsStorage:
                    fromCell = GameScene.Game.StorageBox.PartGrid.Grid[p.FromSlot];
                    break;
                case GridType.GuildStorage:
                    fromCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.FromSlot];
                    break;
                case GridType.CompanionInventory:
                    fromCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.FromSlot];
                    break;
                case GridType.CompanionEquipment:
                    fromCell = GameScene.Game.CompanionBox.EquipmentGrid[p.FromSlot];
                    break;
                default: return;
            }

            switch (p.ToGrid)
            {
                case GridType.Inventory:
                    toCell = GameScene.Game.InventoryBox.Grid.Grid[p.ToSlot];
                    break;
                case GridType.Equipment:
                    toCell = GameScene.Game.CharacterBox.Grid[p.ToSlot];
                    break;
                case GridType.Storage:
                    toCell = GameScene.Game.StorageBox.Grid.Grid[p.ToSlot];
                    break;
                case GridType.PartsStorage:
                    toCell = GameScene.Game.StorageBox.PartGrid.Grid[p.ToSlot];
                    break;
                case GridType.GuildStorage:
                    toCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.ToSlot];
                    break;
                case GridType.CompanionInventory:
                    toCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.ToSlot];
                    break;
                case GridType.CompanionEquipment:
                    toCell = GameScene.Game.CompanionBox.EquipmentGrid[p.ToSlot];
                    break;
                default:
                    return;
            }

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;


            if (p.FromGrid != p.ToGrid)
            {
                if (p.FromGrid == GridType.Inventory) //Moving FROM bag
                {
                    if (!fromCell.Item.Info.ShouldLinkInfo)
                    {
                        for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                        {
                            ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                            if (link.LinkItemIndex != fromCell.Item.Index) continue;

                            link.LinkItemIndex = -1;

                            if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                                GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                            if (p.ToGrid == GridType.Equipment && toCell.Item != null) //Replace item with other item (if equipment)
                            {
                                link.LinkItemIndex = toCell.Item.Index;

                                if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                                    GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = toCell.Item; //set belt to item
                            }

                            if (!GameScene.Game.Observer)
                                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                        }
                    }
                }
                else if (p.ToGrid == GridType.Inventory && toCell.Item != null) //Moving TO bag
                {
                    if (!toCell.Item.Info.ShouldLinkInfo)
                    {
                        for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                        {
                            ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                            if (link.LinkItemIndex != toCell.Item.Index) continue;

                            link.LinkItemIndex = -1;

                            if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                                GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                            if (!GameScene.Game.Observer)
                                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                        }
                    }
                }
            }

            if (p.MergeItem)
            {
                if (toCell.Item.Count + fromCell.Item.Count <= toCell.Item.Info.StackSize)
                {
                    toCell.Item.Count += fromCell.Item.Count;
                    fromCell.Item = null;
                    toCell.RefreshItem();

                    return;
                }

                fromCell.Item.Count -= fromCell.Item.Info.StackSize - toCell.Item.Count;
                toCell.Item.Count = toCell.Item.Info.StackSize;
                fromCell.RefreshItem();
                toCell.RefreshItem();
                return;
            }

            ClientUserItem temp = toCell.Item;

            toCell.Item = fromCell.Item;
            fromCell.Item = temp;

            //if (p.ToGrid == GridType.GuildStorage || p.FromGrid == GridType.GuildStorage)
            //    GameScene.Game.GuildPanel.StorageControl.ItemCount = GameScene.Game.GuildStorage.Count(x => x != null);
        }

        public void Process(S.CurrencyChanged p)
        {
            var currency = GameScene.Game.User.Currencies.First(x => x.Info.Index == p.CurrencyIndex);

            currency.Amount = p.Amount;

            GameScene.Game.CurrencyChanged();

            if (currency.Info.Type == CurrencyType.Gold)
                DXSoundManager.Play(SoundIndex.GoldGained);
        }

        public void Process(S.ItemChanged p)
        {
            DXItemCell[] grid;

            switch (p.Link.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                case GridType.GuildStorage:
                    grid = GameScene.Game.GuildBox.StorageGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            DXItemCell fromCell = grid[p.Link.Slot];

            fromCell.Locked = false;

            if (!p.Success) return;


            if (!fromCell.Item.Info.ShouldLinkInfo)
            {
                for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                {
                    ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                    if (link.LinkItemIndex != fromCell.Item.Index) continue;

                    link.LinkItemIndex = -1;

                    if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                        GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                    if (!GameScene.Game.Observer)
                        CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                }
            }


            if (p.Link.Count == 0)
                fromCell.Item = null;
            else
                fromCell.Item.Count = p.Link.Count;

            fromCell.RefreshItem();
        }
        public void Process(S.ItemsChanged p)
        {
            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;
                fromCell.Selected = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            DXItemCell.SelectedCell = null;
        }

        public void Process(S.ItemStatsChanged p)
        {
            DXItemCell[] grid;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item.AddedStats.Add(p.NewStats);

            if (p.NewStats.Count == 0)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.NothingHappen, fromCell.Item.Info.ItemName), MessageType.Hint);
                return;
            }

            foreach (KeyValuePair<Stat, int> pair in p.NewStats.Values)
            {
                if (pair.Key == Stat.WeaponElement)
                {
                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.ItemStatsNewElement, fromCell.Item.Info.ItemName, (Element)fromCell.Item.AddedStats[Stat.WeaponElement]), MessageType.Hint);
                    continue;
                }

                string msg = p.NewStats.GetDisplay(pair.Key);

                if (string.IsNullOrEmpty(msg)) continue;

                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.ItemStatsEffected, fromCell.Item.Info.ItemName, msg), MessageType.Hint);
            }

            fromCell.RefreshItem();
        }
        public void Process(S.ItemStatsRefreshed p)
        {
            DXItemCell[] grid;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item.AddedStats = p.NewStats;

            fromCell.RefreshItem();
        }
        public void Process(S.ItemDurability p)
        {
            DXItemCell[] grid;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item.CurrentDurability = p.CurrentDurability;


            if (p.CurrentDurability == 0)
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.ItemDurabilityDrop, fromCell.Item.Info.ItemName), MessageType.System);

            fromCell.RefreshItem();
        }
        public void Process(S.StatsUpdate p)
        {
            MapObject.User.HermitPoints = p.HermitPoints;
            MapObject.User.Stats = p.Stats;
            MapObject.User.HermitStats = p.HermitStats;
        }
        public void Process(S.ItemUseDelay p)
        {
            GameScene.Game.UseItemTime = CEnvir.Now + p.Delay;
        }
        public void Process(S.ItemSplit p)
        {
            DXItemCell fromCell;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    fromCell = GameScene.Game.InventoryBox.Grid.Grid[p.Slot];
                    break;
                case GridType.Storage:
                    fromCell = GameScene.Game.StorageBox.Grid.Grid[p.Slot];
                    break;
                case GridType.PartsStorage:
                    fromCell = GameScene.Game.StorageBox.PartGrid.Grid[p.Slot];
                    break;
                case GridType.GuildStorage:
                    fromCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.Slot];
                    break;
                case GridType.CompanionInventory:
                    fromCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Slot];
                    break;
                case GridType.CompanionEquipment:
                    fromCell = GameScene.Game.CompanionBox.EquipmentGrid[p.Slot];
                    break;
                default: return;
            }

            fromCell.Locked = false;

            if (!p.Success) return;

            DXItemCell toCell;
            switch (p.Grid)
            {
                case GridType.Inventory:
                    toCell = GameScene.Game.InventoryBox.Grid.Grid[p.NewSlot];
                    break;
                case GridType.Storage:
                    toCell = GameScene.Game.StorageBox.Grid.Grid[p.NewSlot];
                    break;
                case GridType.PartsStorage:
                    toCell = GameScene.Game.StorageBox.PartGrid.Grid[p.NewSlot];
                    break;
                case GridType.GuildStorage:
                    toCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.NewSlot];
                    break;
                case GridType.CompanionInventory:
                    toCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.NewSlot];
                    break;
                case GridType.CompanionEquipment:
                    toCell = GameScene.Game.CompanionBox.EquipmentGrid[p.NewSlot];
                    break;
                default: return;
            }


            ClientUserItem item = new ClientUserItem(fromCell.Item, p.Count) { Slot = p.NewSlot };// { Count = p.Count, Info = cell.Item.Info, GridSlot = p.NewSlot, AddedStats = new Stats(), };

            toCell.Item = item;
            toCell.RefreshItem();

            if (p.Count == fromCell.Item.Count)
                fromCell.Item = null;
            else
                fromCell.Item.Count -= p.Count;

            fromCell.RefreshItem();
        }

        public void Process(S.ItemLock p)
        {
            DXItemCell cell;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    cell = GameScene.Game.InventoryBox.Grid.Grid[p.Slot];
                    break;
                case GridType.Equipment:
                    cell = GameScene.Game.CharacterBox.Grid[p.Slot];
                    break;
                case GridType.Storage:
                    cell = GameScene.Game.StorageBox.Grid.Grid[p.Slot];
                    break;
                case GridType.PartsStorage:
                    cell = GameScene.Game.StorageBox.PartGrid.Grid[p.Slot];
                    break;
                /*    case GridType.GuildStorage:
                      fromCell = GameScene.Game.GuildPanel.StorageControl.Grid[p.FromSlot];
                      break;*/
                case GridType.CompanionInventory:
                    cell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Slot];
                    break;
                case GridType.CompanionEquipment:
                    cell = GameScene.Game.CompanionBox.EquipmentGrid[p.Slot];
                    break;
                default: return;
            }

            if (cell.Item == null) return;

            if (p.Locked)
                cell.Item.Flags |= UserItemFlags.Locked;
            else
                cell.Item.Flags &= ~UserItemFlags.Locked;

            cell.RefreshItem();
        }
        public void Process(S.ItemSort p)
        {
            DXItemCell[] grid;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                default:
                    return;
            }

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i].Locked = false;
                grid[i].Selected = false;
                grid[i].Item = null;
            }

            foreach (var item in p.Items)
            {
                switch (p.Grid)
                {
                    case GridType.Inventory:
                        grid[item.Slot].Item = item;
                        break;
                    case GridType.Storage:
                        grid[item.Slot].Item = item;
                        break;
                    case GridType.PartsStorage:
                        grid[item.Slot - Globals.PartsStorageOffset].Item = item;
                        break;
                }
            }
        }

        public void Process(S.ItemDelete p)
        {
            DXItemCell cell;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    cell = GameScene.Game.InventoryBox.Grid.Grid[p.Slot];
                    break;
                default:
                    return;
            }

            cell.Locked = false;
            cell.Selected = false;
            DXItemCell.SelectedCell = null;

            if (!p.Success) return;

            if (!cell.Item.Info.ShouldLinkInfo)
            {
                for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                {
                    ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                    if (link.LinkItemIndex != cell.Item.Index) continue;

                    link.LinkItemIndex = -1;

                    if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                        GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                    if (!GameScene.Game.Observer)
                        CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                }
            }

            cell.Item = null;
            cell.RefreshItem();
        }

        public void Process(S.ItemExperience p)
        {
            DXItemCell cell;

            switch (p.Target.GridType)
            {
                case GridType.Inventory:
                    cell = GameScene.Game.InventoryBox.Grid.Grid[p.Target.Slot];
                    break;
                case GridType.Equipment:
                    cell = GameScene.Game.CharacterBox.Grid[p.Target.Slot];
                    break;
                case GridType.Storage:
                    cell = GameScene.Game.StorageBox.Grid.Grid[p.Target.Slot];
                    break;
                case GridType.PartsStorage:
                    cell = GameScene.Game.StorageBox.PartGrid.Grid[p.Target.Slot];
                    break;
                case GridType.CompanionInventory:
                    cell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Target.Slot];
                    break;
                case GridType.CompanionEquipment:
                    cell = GameScene.Game.CompanionBox.EquipmentGrid[p.Target.Slot];
                    break;
                default: return;
            }

            if (cell.Item == null) return;

            cell.Item.Experience = p.Experience;
            cell.Item.Level = p.Level;
            cell.Item.Flags = p.Flags;

            cell.RefreshItem();
        }

        public void Process(S.Chat p)
        {
            if (GameScene.Game == null) return;

            GameScene.Game.ReceiveChat(p.Text, p.Type, p.LinkedItems);

            if (p.Type != MessageType.Normal || p.ObjectID <= 0) return;

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Chat(p.Text);

                return;
            }
        }

        public void Process(S.NPCResponse p)
        {
            GameScene.Game.NPCBox.Response(p);
        }
        public void Process(S.NPCRepair p)
        {
            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {

                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.GuildStorage:
                        if (GameScene.Game.Observer) continue;

                        grid = GameScene.Game.GuildBox.StorageGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }



                DXItemCell cell = grid[cellLinkInfo.Slot];

                cell.Locked = false;

                if (!p.Success) continue;

                cell.Link = null;

                if (p.Special)
                {
                    cell.Item.CurrentDurability = cell.Item.MaxDurability;
                    if (cell.Item.Info.ItemType != ItemType.Weapon && p.SpecialRepairDelay > TimeSpan.Zero)
                        cell.Item.NextSpecialRepair = CEnvir.Now.Add(p.SpecialRepairDelay);
                }
                else
                {
                    cell.Item.MaxDurability = Math.Max(0, cell.Item.MaxDurability - (cell.Item.MaxDurability - cell.Item.CurrentDurability) / Globals.DuraLossRate);
                    cell.Item.CurrentDurability = cell.Item.MaxDurability;
                }

                cell.RefreshItem();
            }
        }
        public void Process(S.NPCRefine p)
        {
            foreach (CellLinkInfo cellLinkInfo in p.Ores)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }



                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Items)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }



                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Specials)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            if (p.Success)
            {
                DXItemCell fromCell = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Weapon];
                fromCell.Item = null;
                fromCell.RefreshItem();

                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.RefineSuccess, Functions.ToString(Globals.RefineTimes[p.RefineQuality], false)), MessageType.System);
            }
        }
        public void Process(S.NPCMasterRefine p)
        {
            foreach (CellLinkInfo cellLinkInfo in p.Fragment1s)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }



                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Fragment2s)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }



                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Fragment3s)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Stones)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Specials)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }


                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }
        }
        public void Process(S.RefineList p)
        {
            GameScene.Game.NPCRefineRetrieveBox.Refines.AddRange(p.List);
        }
        public void Process(S.NPCRefineRetrieve p)
        {
            foreach (ClientRefineInfo info in GameScene.Game.NPCRefineRetrieveBox.Refines)
            {
                if (info.Index != p.Index) continue;

                GameScene.Game.NPCRefineRetrieveBox.Refines.Remove(info);
                break;
            }

            GameScene.Game.NPCRefineRetrieveBox.RefreshList();
        }
        public void Process(S.NPCClose p)
        {


            GameScene.Game.NPCBox.Visible = false;
        }
        public void Process(S.NPCAccessoryLevelUp p)
        {
            if (p.Target != null)
                p.Links.Add(p.Target);

            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;
            }
        }

        public void Process(S.GroupSwitch p)
        {
            GameScene.Game.GroupBox.AllowGroup = p.Allow;
        }
        public void Process(S.GroupMember p)
        {
            GameScene.Game.GroupBox.Members.Add(new ClientPlayerInfo { ObjectID = p.ObjectID, Name = p.Name });

            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GroupJoin, p.Name), MessageType.Group);

            GameScene.Game.GroupBox.UpdateMembers();

            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }
        public void Process(S.GroupRemove p)
        {
            ClientPlayerInfo info = GameScene.Game.GroupBox.Members.First(x => x.ObjectID == p.ObjectID);

            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GroupRemove, info.Name), MessageType.Group);

            HashSet<uint> checks = new HashSet<uint>();

            if (p.ObjectID == MapObject.User.ObjectID)
            {
                foreach (ClientPlayerInfo member in GameScene.Game.GroupBox.Members)
                    checks.Add(member.ObjectID);

                GameScene.Game.GroupBox.Members.Clear();
            }
            else
            {
                checks.Add(p.ObjectID);
                GameScene.Game.GroupBox.Members.Remove(info);
            }

            GameScene.Game.GroupBox.UpdateMembers();

            foreach (uint objectID in checks)
            {
                ClientObjectData data;
                if (!GameScene.Game.DataDictionary.TryGetValue(objectID, out data)) return;

                GameScene.Game.BigMapBox.Update(data);
                GameScene.Game.MiniMapBox.Update(data);
            }
        }
        public void Process(S.GroupInvite p)
        {


            DXMessageBox messageBox = new DXMessageBox($"Do you want to group with {p.Name}?", "Group Invitation", DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GroupResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GroupResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GroupResponse { Accept = false });
            messageBox.Modal = false;
            messageBox.CloseButton.Visible = false;

        }

        public void Process(S.BuffAdd p)
        {
            MapObject.User.AddBuff(p.Buff);

            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffRemove p)
        {
            foreach (ClientBuffInfo buff in MapObject.User.Buffs)
            {
                if (buff.Index != p.Index) continue;

                MapObject.User.Buffs.Remove(buff);
                MapObject.User.VisibleBuffs.Remove(buff.Type);
                break;
            }

            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffChanged p)
        {
            MapObject.User.Buffs.First(x => x.Index == p.Index).Stats = p.Stats;

            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffTime p)
        {
            MapObject.User.Buffs.First(x => x.Index == p.Index).RemainingTime = p.Time;

            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffPaused p)
        {
            MapObject.User.Buffs.First(x => x.Index == p.Index).Pause = p.Paused;

            GameScene.Game.BuffBox.BuffsChanged();
        }

        public void Process(S.SafeZoneChanged P)
        {
            MapObject.User.InSafeZone = P.InSafeZone;
        }

        public void Process(S.CombatTime p)
        {
            GameScene.Game.User.CombatTime = CEnvir.Now;
        }

        public void Process(S.Inspect p)
        {
            if (p.Ranking)
                GameScene.Game.RankingBox.NewInformation(p);
            else
                GameScene.Game.InspectBox.NewInformation(p);
        }

        public void Process(S.Rankings p)
        {
            (DXControl.ActiveScene as LoginScene)?.RankingBox.Update(p);
            GameScene.Game?.RankingBox.Update(p);
        }

        public void Process(S.RankSearch p)
        {
            (DXControl.ActiveScene as LoginScene)?.RankingBox.Update(p);
            GameScene.Game?.RankingBox.Update(p);
        }

        public void Process(S.StartObserver p)
        {
            CEnvir.FillStorage(p.Items, true);

            int index = 0;

            if (GameScene.Game != null)
                index = GameScene.Game.RankingBox.StartIndex;

            DXControl.ActiveScene.Dispose();

            DXSoundManager.StopAllSounds();

            GameScene scene = new GameScene(Config.GameSize);
            DXControl.ActiveScene = scene;
            GameScene.Game.Observer = true;

            scene.MapControl.MapInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == p.StartInformation.MapIndex);
            scene.MapControl.InstanceInfo = Globals.InstanceInfoList.Binding.FirstOrDefault(x => x.Index == p.StartInformation.InstanceIndex);

            GameScene.Game.QuestLog = p.StartInformation.Quests;

            GameScene.Game.NPCAdoptCompanionBox.AvailableCompanions = p.StartInformation.AvailableCompanions;
            GameScene.Game.NPCAdoptCompanionBox.RefreshUnlockButton();

            GameScene.Game.NPCCompanionStorageBox.Companions = p.StartInformation.Companions;
            GameScene.Game.NPCCompanionStorageBox.UpdateScrollBar();

            GameScene.Game.Companion = GameScene.Game.NPCCompanionStorageBox.Companions.FirstOrDefault(x => x.Index == p.StartInformation.Companion);

            scene.User = new UserObject(p.StartInformation);

            GameScene.Game.StorageSize = p.StartInformation.StorageSize;

            GameScene.Game.BuffBox.BuffsChanged();

            GameScene.Game.RankingBox.StartIndex = index;

            GameScene.Game.CompanionBox.RefreshFilter();

            GameScene.Game.CharacterBox.UpdateDiscipline();
        }
        public void Process(S.ObservableSwitch p)
        {
            GameScene.Game.RankingBox.Observable = p.Allow;
        }

        public void Process(S.MarketPlaceHistory p)
        {

            switch (p.Display)
            {
                case 1:
                    GameScene.Game.MarketPlaceBox.SearchNumberSoldBox.TextBox.Text = p.SaleCount > 0 ? p.SaleCount.ToString("#,##0") : "No Records";
                    GameScene.Game.MarketPlaceBox.SearchAveragePriceBox.TextBox.Text = p.SaleCount > 0 ? p.AveragePrice.ToString("#,##0") : "No Records";
                    GameScene.Game.MarketPlaceBox.SearchLastPriceBox.TextBox.Text = p.SaleCount > 0 ? p.LastPrice.ToString("#,##0") : "No Records";
                    break;
                case 2:
                    GameScene.Game.MarketPlaceBox.NumberSoldBox.TextBox.Text = p.SaleCount > 0 ? p.SaleCount.ToString("#,##0") : "No Records";
                    GameScene.Game.MarketPlaceBox.AveragePriceBox.TextBox.Text = p.SaleCount > 0 ? p.AveragePrice.ToString("#,##0") : "No Records";
                    GameScene.Game.MarketPlaceBox.LastPriceBox.TextBox.Text = p.SaleCount > 0 ? p.LastPrice.ToString("#,##0") : "No Records";
                    break;

            }
        }
        public void Process(S.MarketPlaceConsign p)
        {
            GameScene.Game.MarketPlaceBox.ConsignItems.AddRange(p.Consignments);
            GameScene.Game.MarketPlaceBox.RefreshConsignList();
        }
        public void Process(S.MarketPlaceSearch p)
        {
            GameScene.Game.MarketPlaceBox.SearchResults = new ClientMarketPlaceInfo[p.Count];

            for (int i = 0; i < p.Results.Count; i++)
                GameScene.Game.MarketPlaceBox.SearchResults[i] = p.Results[i];

            GameScene.Game.MarketPlaceBox.RefreshList();
        }
        public void Process(S.MarketPlaceSearchCount p)
        {


            Array.Resize(ref GameScene.Game.MarketPlaceBox.SearchResults, p.Count);

            GameScene.Game.MarketPlaceBox.RefreshList();
        }
        public void Process(S.MarketPlaceSearchIndex p)
        {


            if (GameScene.Game.MarketPlaceBox.SearchResults == null) return;

            GameScene.Game.MarketPlaceBox.SearchResults[p.Index] = p.Result;

            GameScene.Game.MarketPlaceBox.RefreshList();
        }
        public void Process(S.MarketPlaceConsignChanged p)
        {


            ClientMarketPlaceInfo info = GameScene.Game.MarketPlaceBox.ConsignItems.FirstOrDefault(x => x.Index == p.Index);

            if (info == null) return;

            if (p.Count > 0)
                info.Item.Count = p.Count;
            else
                GameScene.Game.MarketPlaceBox.ConsignItems.Remove(info);

            GameScene.Game.MarketPlaceBox.RefreshConsignList();
        }
        public void Process(S.MarketPlaceBuy p)
        {
            GameScene.Game.MarketPlaceBox.BuyButton.Enabled = true;

            if (!p.Success) return;

            ClientMarketPlaceInfo info = GameScene.Game.MarketPlaceBox.SearchResults.FirstOrDefault(x => x != null && x.Index == p.Index);

            if (info == null) return;

            if (p.Count > 0)
                info.Item.Count = p.Count;
            else
                info.Item = null;

            GameScene.Game.MarketPlaceBox.RefreshList();
        }
        public void Process(S.MarketPlaceStoreBuy p)
        {
            GameScene.Game.MarketPlaceBox.StoreBuyButton.Enabled = true;
        }

        public void Process(S.MailList p)
        {
            GameScene.Game.CommunicationBox.ReceivedMailList.AddRange(p.Mail);
            GameScene.Game.CommunicationBox.UpdateIcon();
        }

        public void Process(S.MailNew p)
        {
            GameScene.Game.CommunicationBox.ReceivedMailList.Insert(0, p.Mail);
            GameScene.Game.CommunicationBox.RefreshList();
            GameScene.Game.CommunicationBox.UpdateIcon();
            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.MailNew, p.Mail.Sender), MessageType.System);
        }

        public void Process(S.MailDelete p)
        {
            ClientMailInfo mail = GameScene.Game.CommunicationBox.ReceivedMailList.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null) return;

            GameScene.Game.CommunicationBox.ReceivedMailList.Remove(mail);
            GameScene.Game.CommunicationBox.RefreshList();
            GameScene.Game.CommunicationBox.UpdateIcon();

            if (mail == GameScene.Game.CommunicationBox.ReadMail)
                GameScene.Game.CommunicationBox.ReadMail = null;
        }

        public void Process(S.MailItemDelete p)
        {
            ClientMailInfo mail = GameScene.Game.CommunicationBox.ReceivedMailList.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null) return;

            ClientUserItem item = mail.Items.FirstOrDefault(x => x.Slot == p.Slot);

            if (item != null)
            {
                mail.Items.Remove(item);

                foreach (CommunicationReceivedRow row in GameScene.Game.CommunicationBox.ReceivedRows)
                {
                    if (row.Mail != mail) continue;

                    row.RefreshIcon();
                    break;
                }
            }

            GameScene.Game.CommunicationBox.UpdateIcon();

            if (mail != GameScene.Game.CommunicationBox.ReadMail) return;

            foreach (DXItemCell cell in GameScene.Game.CommunicationBox.ReadGrid.Grid)
            {
                if (cell.Slot != p.Slot) continue;

                cell.Item = null;
                break;
            }
        }

        public void Process(S.MailSend p)
        {
            GameScene.Game.CommunicationBox.SendAttempted = false;
        }

        public void Process(S.ChangeAttackMode p)
        {
            GameScene.Game.User.AttackMode = p.Mode;

            GameScene.Game.ReceiveChat(GameScene.Game.MainPanel.AttackModeLabel.Text, MessageType.System);
        }

        public void Process(S.ChangePetMode p)
        {
            GameScene.Game.User.PetMode = p.Mode;

            GameScene.Game.ReceiveChat(GameScene.Game.MainPanel.PetModeLabel.Text, MessageType.System);
        }

        public void Process(S.WeightUpdate p)
        {
            GameScene.Game.User.BagWeight = p.BagWeight;
            GameScene.Game.User.WearWeight = p.WearWeight;
            GameScene.Game.User.HandWeight = p.HandWeight;

            GameScene.Game.WeightChanged();
        }

        public void Process(S.TradeRequest p)
        {
            DXMessageBox messageBox = new DXMessageBox($"{p.Name} wishes to trade with you, Do you want to accept?", "Trade Request", DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.TradeRequestResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.TradeRequestResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.TradeRequestResponse { Accept = false });
        }
        public void Process(S.TradeOpen p)
        {
            GameScene.Game.TradeBox.Visible = true;
            GameScene.Game.TradeBox.IsTrading = true;
            GameScene.Game.TradeBox.PlayerLabel.Text = p.Name;
        }
        public void Process(S.TradeClose p)
        {
            GameScene.Game.TradeBox.Visible = false;
            GameScene.Game.TradeBox.Clear();
        }
        public void Process(S.TradeAddItem p)
        {
            DXItemCell fromCell;

            switch (p.Cell.GridType)
            {
                case GridType.Inventory:
                    fromCell = GameScene.Game.InventoryBox.Grid.Grid[p.Cell.Slot];
                    break;
                case GridType.Equipment:
                    fromCell = GameScene.Game.CharacterBox.Grid[p.Cell.Slot];
                    break;
                case GridType.Storage:
                    fromCell = GameScene.Game.StorageBox.Grid.Grid[p.Cell.Slot];
                    break;
                case GridType.PartsStorage:
                    fromCell = GameScene.Game.StorageBox.PartGrid.Grid[p.Cell.Slot];
                    break;
                /*    case GridType.GuildStorage:
                      fromCell = GameScene.Game.GuildPanel.StorageControl.Grid[p.FromSlot];
                      break;*/
                case GridType.CompanionInventory:
                    fromCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Cell.Slot];
                    break;
                case GridType.CompanionEquipment:
                    fromCell = GameScene.Game.CompanionBox.EquipmentGrid[p.Cell.Slot];
                    break;
                default: return;
            }


            if (!p.Success)
            {
                fromCell.Link = null;
                return;
            }

            if (fromCell.Link != null) return;

            foreach (DXItemCell cell in GameScene.Game.TradeBox.UserGrid.Grid)
            {
                if (cell.Item != null) continue;

                cell.LinkedCount = p.Cell.Count;
                cell.Link = fromCell;
                return;
            }

        }

        public void Process(S.TradeItemAdded p)
        {
            foreach (DXItemCell cell in GameScene.Game.TradeBox.PlayerGrid.Grid)
            {
                if (cell.Item != null) continue;

                cell.Item = p.Item;
                return;
            }
        }

        public void Process(S.TradeAddGold p)
        {
            GameScene.Game.TradeBox.UserGoldLabel.Text = p.Gold.ToString("#,##0");
        }

        public void Process(S.TradeGoldAdded p)
        {
            GameScene.Game.TradeBox.PlayerGoldLabel.Text = p.Gold.ToString("#,##0");
        }

        public void Process(S.TradeUnlock p)
        {
            GameScene.Game.TradeBox.ConfirmButton.Enabled = true;
        }

        public void Process(S.GuildCreate p)
        {
            GameScene.Game.GuildBox.CreateAttempted = false;
        }
        public void Process(S.GuildInfo p)
        {
            HashSet<uint> checks = new HashSet<uint>();

            if (GameScene.Game.GuildBox.GuildInfo != null)
            {
                foreach (ClientGuildMemberInfo member in GameScene.Game.GuildBox.GuildInfo.Members)
                    if (member.ObjectID > 0)
                        checks.Add(member.ObjectID);
            }

            GameScene.Game.GuildBox.GuildInfo = p.Guild;

            if (GameScene.Game.GuildBox.GuildInfo != null)
            {
                foreach (ClientGuildMemberInfo member in GameScene.Game.GuildBox.GuildInfo.Members)
                    if (member.ObjectID > 0)
                        checks.Add(member.ObjectID);
            }

            foreach (uint objectID in checks)
            {
                ClientObjectData data;
                if (!GameScene.Game.DataDictionary.TryGetValue(objectID, out data)) return;

                GameScene.Game.BigMapBox.Update(data);
                GameScene.Game.MiniMapBox.Update(data);
            }
        }

        public void Process(S.GuildNoticeChanged p)
        {
            GameScene.Game.GuildBox.GuildInfo.Notice = p.Notice;

            if (!GameScene.Game.GuildBox.NoticeTextBox.Editable)
                GameScene.Game.GuildBox.NoticeTextBox.TextBox.Text = p.Notice;
        }

        public void Process(S.GuildGetItem p)
        {
            DXItemCell[] grid;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                case GridType.GuildStorage:
                    grid = GameScene.Game.GuildBox.StorageGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item = p.Item;
        }

        public void Process(S.GuildNewItem p)
        {
            GameScene.Game.GuildBox.StorageGrid.Grid[p.Slot].Item = p.Item;
        }

        public void Process(S.GuildUpdate p)
        {
            GameScene.Game.GuildBox.GuildInfo.GuildFunds = p.GuildFunds;
            GameScene.Game.GuildBox.GuildInfo.DailyGrowth = p.DailyGrowth;

            GameScene.Game.GuildBox.GuildInfo.TotalContribution = p.TotalContribution;
            GameScene.Game.GuildBox.GuildInfo.DailyContribution = p.DailyContribution;


            GameScene.Game.GuildBox.GuildInfo.MemberLimit = p.MemberLimit;
            GameScene.Game.GuildBox.GuildInfo.StorageLimit = p.StorageLimit;

            GameScene.Game.GuildBox.GuildInfo.Tax = p.Tax;

            GameScene.Game.GuildBox.GuildInfo.DefaultPermission = p.DefaultPermission;
            GameScene.Game.GuildBox.GuildInfo.DefaultRank = p.DefaultRank;

            GameScene.Game.GuildBox.GuildInfo.Colour = p.Colour;
            GameScene.Game.GuildBox.GuildInfo.Flag = p.Flag;

            foreach (ClientGuildMemberInfo member in p.Members)
            {
                ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.FirstOrDefault(x => x.Index == member.Index);

                if (info == null)
                {
                    info = new ClientGuildMemberInfo
                    {
                        Index = member.Index,
                        Name = member.Name,
                    };
                    GameScene.Game.GuildBox.GuildInfo.Members.Add(info);
                }


                info.TotalContribution = member.TotalContribution;
                info.DailyContribution = member.DailyContribution;
                info.LastOnline = member.LastOnline;
                info.Permission = member.Permission;
                info.Rank = member.Rank;
                info.ObjectID = member.ObjectID;

                if (info.Index == GameScene.Game.GuildBox.GuildInfo.UserIndex)
                    GameScene.Game.GuildBox.PermissionChanged();

                ClientObjectData data;
                if (!GameScene.Game.DataDictionary.TryGetValue(member.ObjectID, out data)) continue;

                GameScene.Game.BigMapBox.Update(data);
                GameScene.Game.MiniMapBox.Update(data);
            }


            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildKick p)
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.First(x => x.Index == p.Index);

            GameScene.Game.GuildBox.GuildInfo.Members.Remove(info);

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();


            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(info.ObjectID, out data)) return;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }
        public void Process(S.GuildIncreaseMember p)
        {
            GameScene.Game.GuildBox.IncreaseMemberButton.Enabled = true;
        }

        public void Process(S.GuildIncreaseStorage p)
        {
            GameScene.Game.GuildBox.IncreaseStorageButton.Enabled = true;
        }

        public void Process(S.GuildInviteMember p)
        {
            GameScene.Game.GuildBox.AddMemberButton.Enabled = true;
        }

        public void Process(S.GuildTax p)
        {
            GameScene.Game.GuildBox.SetTaxButton.Enabled = true;
        }

        public void Process(S.GuildMemberOffline p)
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.First(x => x.Index == p.Index);

            info.LastOnline = CEnvir.Now;
            info.ObjectID = 0;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();

        }
        public void Process(S.GuildInvite p)
        {
            DXMessageBox messageBox = new DXMessageBox($"{p.Name} has invited you to the guild {p.GuildName}\n" +
                                                       $"Do you want to join the guild?", "Guild Invitation", DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildResponse { Accept = false });
            messageBox.Modal = false;
            messageBox.CloseButton.Visible = false;

        }
        public void Process(S.GuildMemberOnline p)
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.First(x => x.Index == p.Index);

            info.LastOnline = DateTime.MaxValue;
            info.Name = p.Name;
            info.ObjectID = p.ObjectID;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildMemberContribution p)
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.First(x => x.Index == p.Index);

            info.DailyContribution += p.Contribution;
            info.TotalContribution += p.Contribution;

            GameScene.Game.GuildBox.GuildInfo.GuildFunds += p.Contribution;
            GameScene.Game.GuildBox.GuildInfo.DailyGrowth += p.Contribution;

            GameScene.Game.GuildBox.GuildInfo.TotalContribution += p.Contribution;
            GameScene.Game.GuildBox.GuildInfo.DailyContribution += p.Contribution;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildDayReset p)
        {
            foreach (ClientGuildMemberInfo member in GameScene.Game.GuildBox.GuildInfo.Members)
                member.DailyContribution = 0;

            GameScene.Game.GuildBox.GuildInfo.DailyGrowth = 0;
            GameScene.Game.GuildBox.GuildInfo.DailyContribution = 0;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }

        public void Process(S.GuildFundsChanged p)
        {
            GameScene.Game.GuildBox.GuildInfo.GuildFunds += p.Change;
            GameScene.Game.GuildBox.GuildInfo.DailyGrowth += p.Change;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }

        public void Process(S.GuildChanged p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ((PlayerObject)ob).Title = p.GuildName;
                ((PlayerObject)ob).GuildRank = p.GuildRank;
                return;
            }
        }

        public void Process(S.GuildWar p)
        {

        }

        public void Process(S.GuildWarStarted p)
        {
            GameScene.Game.GuildWars.Add(p.GuildName);

            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GuildWarStarted, p.GuildName, Functions.ToString(p.Duration, true)), MessageType.Hint);

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }

        public void Process(S.GuildWarFinished p)
        {
            GameScene.Game.GuildWars.Remove(p.GuildName);

            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GuildWarFinished, p.GuildName), MessageType.Hint);

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }

        public void Process(S.GuildConquestStarted p)
        {
            GameScene.Game.ConquestWars.Add(CEnvir.CastleInfoList.Binding.First(x => x.Index == p.Index));

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }
        public void Process(S.GuildConquestFinished p)
        {
            GameScene.Game.ConquestWars.Remove(CEnvir.CastleInfoList.Binding.First(x => x.Index == p.Index));

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }
        public void Process(S.GuildCastleInfo p)
        {
            CastleInfo castle = CEnvir.CastleInfoList.Binding.First(x => x.Index == p.Index);
            GameScene.Game.CastleOwners[castle] = p.Owner;

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();

            GameScene.Game.GuildBox.CastlePanels[castle].Update();

            GameScene.Game.GuildBox.RefreshCastleControls();
        }
        public void Process(S.GuildConquestDate p)
        {
            CastleInfo castle = CEnvir.CastleInfoList.Binding.First(x => x.Index == p.Index);

            castle.WarDate = p.WarDate;
        }

        public void Process(S.ReviveTimers p)
        {
            GameScene.Game.ItemReviveTime = CEnvir.Now + p.ItemReviveTime;
            GameScene.Game.ReincarnationPillTime = CEnvir.Now + p.ReincarnationPillTime;
        }

        public void Process(S.QuestChanged p)
        {
            foreach (ClientUserQuest quest in GameScene.Game.QuestLog)
            {
                if (quest.Quest != p.Quest.Quest) continue;

                quest.Completed = p.Quest.Completed;
                quest.DateCompleted = p.Quest.DateCompleted;
                quest.Track = p.Quest.Track;
                quest.SelectedReward = p.Quest.SelectedReward;
                quest.Tasks.Clear();
                quest.Tasks.AddRange(p.Quest.Tasks);

                if (quest.Completed)
                {
                    DXSoundManager.Play(SoundIndex.QuestComplete);
                }
            
                GameScene.Game.QuestChanged(p.Quest);
                return;
            }

            GameScene.Game.QuestLog.Add(p.Quest);
            GameScene.Game.QuestChanged(p.Quest);

            DXSoundManager.Play(SoundIndex.QuestTake);
        }

        public void Process(S.QuestCancelled p)
        {
            foreach (ClientUserQuest quest in GameScene.Game.QuestLog)
            {
                if (quest.Index != p.Index) continue;

                GameScene.Game.CancelQuest(quest.Quest);
                GameScene.Game.QuestChanged(quest);
                return;
            }

        }

        public void Process(S.CompanionUnlock p)
        {
            GameScene.Game.NPCAdoptCompanionBox.UnlockButton.Enabled = true;
            if (p.Index == 0) return;

            GameScene.Game.NPCAdoptCompanionBox.AvailableCompanions.Add(Globals.CompanionInfoList.Binding.First(x => x.Index == p.Index));

            GameScene.Game.NPCAdoptCompanionBox.RefreshUnlockButton();
        }
        public void Process(S.CompanionAdopt p)
        {
            GameScene.Game.NPCAdoptCompanionBox.AdoptAttempted = false;

            if (p.UserCompanion == null) return;

            GameScene.Game.NPCCompanionStorageBox.Companions.Add(p.UserCompanion);
            GameScene.Game.NPCCompanionStorageBox.UpdateScrollBar();
            GameScene.Game.NPCAdoptCompanionBox.CompanionNameTextBox.TextBox.Text = string.Empty;
        }
        public void Process(S.CompanionStore p)
        {
            if (GameScene.Game.Companion != null)
                GameScene.Game.Companion.CharacterName = null;

            GameScene.Game.Companion = null;
        }
        public void Process(S.CompanionRetrieve p)
        {
            GameScene.Game.Companion = GameScene.Game.NPCCompanionStorageBox.Companions.FirstOrDefault(x => x.Index == p.Index);
        }
        public void Process(S.CompanionWeightUpdate p)
        {
            GameScene.Game.CompanionBox.BagWeight = p.BagWeight;
            GameScene.Game.CompanionBox.MaxBagWeight = p.MaxBagWeight;
            GameScene.Game.CompanionBox.InventorySize = p.InventorySize;

            GameScene.Game.CompanionBox.Refresh();
        }
        public void Process(S.CompanionShapeUpdate p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.Monster || ob.ObjectID != p.ObjectID) continue;

                MonsterObject monster = (MonsterObject)ob;

                if (monster.CompanionObject == null) continue;

                monster.CompanionObject.HeadShape = p.HeadShape;
                monster.CompanionObject.BackShape = p.BackShape;
                return;
            }
        }
        public void Process(S.CompanionUpdate p)
        {
            GameScene.Game.Companion.Hunger = p.Hunger;
            GameScene.Game.Companion.Level = p.Level;
            GameScene.Game.Companion.Experience = p.Experience;

            GameScene.Game.CompanionBox.Refresh();
        }
        public void Process(S.CompanionItemsGained p)
        {
            foreach (ClientUserItem item in p.Items)
            {
                ItemInfo displayInfo = item.Info;

                if (item.Info.ItemEffect == ItemEffect.ItemPart)
                    displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == item.AddedStats[Stat.ItemIndex]);

                item.New = true;
                string text = item.Count > 1 ? string.Format(CEnvir.Language.CompanionItemsGained, displayInfo.ItemName, item.Count) : string.Format(CEnvir.Language.CompanionItemGained, displayInfo.ItemName);

                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem)
                    text += " (Quest)";

                if (item.Info.ItemEffect == ItemEffect.ItemPart)
                    text += " [Part]";

                GameScene.Game.ReceiveChat(text, MessageType.Combat);
            }

            GameScene.Game.AddCompanionItems(p.Items);
        }
        public void Process(S.CompanionSkillUpdate p)
        {
            GameScene.Game.Companion.Level3 = p.Level3;
            GameScene.Game.Companion.Level5 = p.Level5;
            GameScene.Game.Companion.Level7 = p.Level7;
            GameScene.Game.Companion.Level10 = p.Level10;
            GameScene.Game.Companion.Level11 = p.Level11;
            GameScene.Game.Companion.Level13 = p.Level13;
            GameScene.Game.Companion.Level15 = p.Level15;

            GameScene.Game.CompanionBox.Refresh();
        }

        public void Process(S.MarriageInvite p)
        {
            DXMessageBox messageBox = new DXMessageBox($"{p.Name} proposed to you.\n" +
                                                       $"Do you want to marry them?", "Marraige Proposal", DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.MarriageResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.MarriageResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.MarriageResponse { Accept = false });
            messageBox.Modal = false;
            messageBox.CloseButton.Visible = false;

        }
        public void Process(S.MarriageInfo p)
        {
            ClientObjectData data;

            GameScene.Game.DataDictionary.TryGetValue(GameScene.Game.Partner?.ObjectID ?? p.Partner.ObjectID, out data);

            GameScene.Game.Partner = p.Partner;

            if (data == null) return;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }

        public void Process(S.MarriageRemoveRing p)
        {


            GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].Item.Flags &= ~UserItemFlags.Marriage;
        }
        public void Process(S.MarriageMakeRing p)
        {


            GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].Item.Flags |= UserItemFlags.Marriage;
        }
        public void Process(S.MarriageOnlineChanged p)
        {


            ClientObjectData data;

            GameScene.Game.DataDictionary.TryGetValue(GameScene.Game.Partner.ObjectID > 0 ? GameScene.Game.Partner.ObjectID : p.ObjectID, out data);

            GameScene.Game.Partner.ObjectID = p.ObjectID;

            if (data == null) return;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }

        public void Process(S.DataObjectPlayer p)
        {
            ClientObjectData data = new ClientObjectData
            {
                ObjectID = p.ObjectID,

                MapIndex = p.MapIndex,
                Location = p.CurrentLocation,

                Name = p.Name,
        
                Health = p.Health,
                MaxHealth = p.MaxHealth,
                Dead = p.Dead,

                Mana = p.Mana,
                MaxMana = p.MaxMana,
            };

            GameScene.Game.DataDictionary[p.ObjectID] = data;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }
        public void Process(S.DataObjectMonster p)
        {
            ClientObjectData data = new ClientObjectData
            {
                ObjectID = p.ObjectID,

                MapIndex = p.MapIndex,
                Location = p.CurrentLocation,

                MonsterInfo = p.MonsterInfo,

                Health = p.Health,
                MaxHealth = p.Stats[Stat.Health],
                Stats = p.Stats,
                Dead = p.Dead,

                PetOwner = p.PetOwner,
            };

            GameScene.Game.DataDictionary[p.ObjectID] = data;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }
        public void Process(S.DataObjectItem p)
        {
            ClientObjectData data = new ClientObjectData
            {
                ObjectID = p.ObjectID,

                MapIndex = p.MapIndex,
                Location = p.CurrentLocation,

                ItemInfo = p.ItemInfo,
                Name = p.ItemInfo.ItemName,
            };

            GameScene.Game.DataDictionary[p.ObjectID] = data;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }
        public void Process(S.DataObjectRemove p)
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            GameScene.Game.DataDictionary.Remove(p.ObjectID);

            GameScene.Game.BigMapBox.Remove(data);
            GameScene.Game.MiniMapBox.Remove(data);
        }
        public void Process(S.DataObjectLocation p)
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            data.Location = p.CurrentLocation;
            data.MapIndex = p.MapIndex;

            GameScene.Game.BigMapBox.Update(data);
            GameScene.Game.MiniMapBox.Update(data);
        }
        public void Process(S.DataObjectHealthMana p)
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            data.Health = p.Health;
            data.Mana = p.Mana;

            if (GameScene.Game.MonsterBox.Monster != null && GameScene.Game.MonsterBox.Monster.ObjectID == p.ObjectID)
                GameScene.Game.MonsterBox.RefreshHealth();

            if (data.Dead != p.Dead)
            {
                data.Dead = p.Dead;
                GameScene.Game.BigMapBox.Update(data);
                GameScene.Game.MiniMapBox.Update(data);
            }
        }
        public void Process(S.DataObjectMaxHealthMana p)
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            if (p.Stats != null)
            {
                data.MaxHealth = p.Stats[Stat.Health];
                data.MaxMana = p.Stats[Stat.Mana];
                data.Stats = p.Stats;
            }
            else
            {
                data.MaxHealth = p.MaxHealth;
                data.MaxMana = p.MaxMana;
            }
            if (GameScene.Game.MonsterBox.Monster != null && GameScene.Game.MonsterBox.Monster.ObjectID == p.ObjectID)
                GameScene.Game.MonsterBox.RefreshStats();
        }

        public void Process(S.BlockAdd p)
        {
            CEnvir.BlockList.Add(p.Info);

            GameScene.Game.CommunicationBox.RefreshBlockList();
        }
        public void Process(S.BlockRemove p)
        {
            ClientBlockInfo block = CEnvir.BlockList.First(x => x.Index == p.Index);

            CEnvir.BlockList.Remove(block);

            GameScene.Game.CommunicationBox.RefreshBlockList();
        }

        public void Process(S.FriendAdd p)
        {
            GameScene.Game.CommunicationBox.FriendList.Add(p.Info);
            GameScene.Game.CommunicationBox.RefreshFriendList();
        }
        public void Process(S.FriendRemove p)
        {
            ClientFriendInfo friend = GameScene.Game.CommunicationBox.FriendList.First(x => x.Index == p.Index);

            GameScene.Game.CommunicationBox.FriendList.Remove(friend);

            GameScene.Game.CommunicationBox.RefreshFriendList();
        }

        public void Process(S.FriendUpdate p)
        {
            GameScene.Game.CommunicationBox.FriendList.RemoveAll(x => x.Name == p.Info.Name);
            GameScene.Game.CommunicationBox.FriendList.Add(p.Info);
            GameScene.Game.CommunicationBox.RefreshFriendList();
        }

        public void Process(S.DisciplineUpdate p)
        {
            GameScene.Game.User.Discipline = p.Discipline;

            if (GameScene.Game.User.Discipline != null)
            {
                GameScene.Game.User.Discipline.DisciplineInfo = Globals.DisciplineInfoList.Binding.First(x => x.Index == p.Discipline.InfoIndex);
            }

            GameScene.Game.CharacterBox.UpdateDiscipline();
        }

        public void Process(S.DisciplineExperienceChanged p)
        {
            GameScene.Game.User.Discipline.Experience = p.Experience;

            GameScene.Game.CharacterBox.UpdateDiscipline();
        }

        public void Process(S.NPCRoll p)
        {
            GameScene.Game.NPCRollBox.Setup(p.Type, p.Result, false);
        }

        public void Process(S.SetTimer p)
        {
            GameScene.Game.TimerBox.AddTimer(p);
        }

        public void Process(S.HelmetToggle p)
        {
            GameScene.Game.ConfigBox.DisplayHelmetCheckBox.Checked = !p.HideHelmet;
        }

        public void Process(S.StorageSize p)
        {
            GameScene.Game.StorageSize = p.Size;
        }
        public void Process(S.PlayerChangeUpdate p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.Player || ob.ObjectID != p.ObjectID) continue;

                PlayerObject player = (PlayerObject)ob;

                player.Name = p.Name;
                player.Caption = p.Caption;
                player.Gender = p.Gender;
                player.HairType = p.HairType;
                player.HairColour = p.HairColour;
                player.ArmourColour = p.ArmourColour;

                player.UpdateLibraries();
                return;
            }
        }

        public void Process(S.FortuneUpdate p)
        {
            foreach (ClientFortuneInfo fortune in p.Fortunes)
            {
                ClientFortuneInfo info;
                if (!GameScene.Game.FortuneDictionary.TryGetValue(fortune.ItemInfo, out info))
                {
                    GameScene.Game.FortuneDictionary[fortune.ItemInfo] = fortune;
                    continue;
                }

                info.DropCount = fortune.DropCount;
                info.Progress = fortune.Progress;
                info.CheckDate = fortune.CheckDate;
            }

            if (!GameScene.Game.FortuneCheckerBox.Visible) return;

            GameScene.Game.FortuneCheckerBox.RefreshList();
        }

        public void Process(S.NPCWeaponCraft p)
        {
            #region Template

            DXItemCell[] grid;

            switch (p.Template.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DXItemCell fromCell = grid[p.Template.Slot];
            fromCell.Locked = false;

            if (p.Success)
            {
                if (p.Template.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= p.Template.Count;

                fromCell.RefreshItem();
            }
            #endregion

            #region Yellow

            if (p.Yellow != null)
            {
                switch (p.Yellow.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Yellow.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Yellow.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Yellow.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Blue

            if (p.Blue != null)
            {
                switch (p.Blue.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Blue.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Blue.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Blue.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Red

            if (p.Red != null)
            {
                switch (p.Red.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Red.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Red.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Red.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Purple

            if (p.Purple != null)
            {
                switch (p.Purple.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Purple.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Purple.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Purple.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Green

            if (p.Green != null)
            {
                switch (p.Green.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Green.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Green.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Green.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Grey

            if (p.Grey != null)
            {
                switch (p.Grey.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Grey.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Grey.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Grey.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion
        }
        public void Process(S.NPCAccessoryRefine p)
        {
            if (p.Target != null)
                p.Links.Add(p.Target);
            if (p.OreTarget != null)
                p.Links.Add(p.OreTarget);

            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.PartsStorage:
                        grid = GameScene.Game.StorageBox.PartGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;
            }
        }

        public void Process(S.ItemAcessoryRefined p)
        {
            DXItemCell[] grid;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.PartsStorage:
                    grid = GameScene.Game.StorageBox.PartGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item.AddedStats.Add(p.NewStats);

            if (p.NewStats.Count == 0)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.NothingHappen, fromCell.Item.Info.ItemName), MessageType.Hint);
                return;
            }



            fromCell.RefreshItem();
        }

        public void Process(S.JoinInstance p)
        {
        }

        public void Process(S.SendCompanionFilters p)
        {

        }
    }
}

