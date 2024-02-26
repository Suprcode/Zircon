using System.Drawing;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class ExitDialog : DXImageControl
    {
        #region Properties

        public DXLabel TitleLabel;
        public DXButton CloseButton;

        public DXButton ToSelectButton, ExitButton;

        public bool Exiting { get; set; }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            BringToFront();
        }

        #endregion

        public ExitDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 281;
            Sort = true;
            Modal = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(252 - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.ExitDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            ToSelectButton = new DXButton
            {
                Location = new Point(61, 45),
                Size = new Size(130, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.ExitDialogToSelectButtonLabel },
            };
            ToSelectButton.MouseClick += (o, e) =>
            {
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.LogoutInCombat, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.Logout());
            };

            ExitButton = new DXButton
            {
                Location = new Point(61, 55 + DefaultHeight),
                Size = new Size(130, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.ExitDialogExitButtonLabel },
            };
            ExitButton.MouseClick += (o, e) =>
            {
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.ExitInCombat, MessageType.System);
                    return;
                }

                Exiting = true;
                CEnvir.Target.Close();
            };

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (ToSelectButton != null)
                {
                    if (!ToSelectButton.IsDisposed)
                        ToSelectButton.Dispose();

                    ToSelectButton = null;
                }

                if (ExitButton != null)
                {
                    if (!ExitButton.IsDisposed)
                        ExitButton.Dispose();

                    ExitButton = null;
                }
            }

        }

        #endregion
    }
}
