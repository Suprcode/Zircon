using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public class FilterDropDialog : DXWindow
    {
        public Dictionary<int, DXTextBox> DropFiltersMap = new Dictionary<int, DXTextBox>();

        public override WindowType Type => WindowType.FilterDropBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        public FilterDropDialog()
        {
            HasTitle = true;
            TitleLabel.Text = CEnvir.Language.FilterDialogTitle;

            SetClientSize(new Size(266, 371));

            for (int i = 0; i < 10; i++)
            {
                DXLabel filterLabel = new DXLabel
                {
                    Parent = this,
                    Text = string.Format(CEnvir.Language.FilterDialogFilterLabel, (i + 1))
                };
                filterLabel.Location = new Point(20, 50 + (10 + filterLabel.Size.Height) * i);
                DropFiltersMap[i] = new DXTextBox
                {
                    Parent = this,
                    Border = true,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Location = new Point(90, filterLabel.Location.Y),
                    Size = new Size(150, 18)
                };
            }

            DXButton filterButton = new DXButton
            {
                Parent = this,
                Label = { Text = CEnvir.Language.FilterDialogSaveButtonLabel, },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            filterButton.Location = new Point(100, Size.Height - 30);
            filterButton.MouseClick += (o, e) =>
            {
                List<string> dropItems = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    dropItems.Add(DropFiltersMap[i].TextBox.Text);
                }
                Config.HighlightedItems = String.Join(",", dropItems);
                GameScene.Game.ReceiveChat(CEnvir.Language.FilterConfigSaved, MessageType.System);
            };
        }

        public void UpdateDropFilters()
        {
            if (Config.HighlightedItems != string.Empty)
            {
                string[] items = Config.HighlightedItems.Split(',');
                for (int i = 0; i < 10; i++)
                {
                    DropFiltersMap[i].TextBox.Text = items[i];
                }
            }
        }

    }
}
