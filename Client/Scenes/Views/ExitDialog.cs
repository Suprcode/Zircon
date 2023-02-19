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
    public sealed class ExitDialog : DXWindow
    {
        #region Properties
        public DXButton ToSelectButton, ExitButton;

        public override WindowType Type => WindowType.ExitBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public ExitDialog()
        {
            TitleLabel.Text = CEnvir.Language.ExitDialogTitle;

            SetClientSize(new Size(200, 50 + DefaultHeight + DefaultHeight));
            ToSelectButton = new DXButton
            {
                Location = new Point(ClientArea.X + 35, ClientArea.Y + 20),
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
                Location = new Point(ClientArea.X + 35, ClientArea.Y + 30 + DefaultHeight),
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

                CEnvir.Target.Close();
            };

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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
