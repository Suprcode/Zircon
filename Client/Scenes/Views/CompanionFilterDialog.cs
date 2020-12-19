using System;
using System.Collections.Generic;
using System.Drawing;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Client.Models;
using Library;
using C = Library.Network.ClientPackets;
using System.Linq;
//Cleaned
namespace Client.Scenes.Views
{
    public sealed class CompanionFilterDialog : DXWindow
    {
        #region Properties

        public override WindowType Type => WindowType.CompanionFilterBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;

        public Dictionary<MirClass, DXCheckBox> FilterClass = new Dictionary<MirClass, DXCheckBox>();
        public Dictionary<Rarity, DXCheckBox> FilterRarity = new Dictionary<Rarity, DXCheckBox>();
        public Dictionary<ItemType, DXCheckBox> FilterType = new Dictionary<ItemType, DXCheckBox>();

        public DXCheckBox FilterTypeCommon;
        public DXCheckBox FilterTypeElite;
        public DXCheckBox FilterTypeSuperior;
        #endregion

        public CompanionFilterDialog()
        {
            CloseButton.Visible = false;
            TitleLabel.Text = "Filter Companion Pick";
            SetClientSize(new Size(350, 341));

            DXLabel label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Class",
            };
            label.Location = new Point(20, 40);

            DrawClassFilter();

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Rarity Type",
            };
            label.Location = new Point(20, 100);

            DrawRarityFilter();

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Item Type",
            };
            label.Location = new Point(20, 160);

            DrawItemTypeFilter();

            DXButton filterButton = new DXButton
            {
                Parent = this,
                Label = { Text = "Save settings", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            filterButton.Location = new Point(Size.Width/2 - filterButton.Size.Width/2, Size.Height - 30);
            filterButton.MouseClick += (o, e) =>
            {
                List<MirClass> fClass = GetCheckedItemsClass();
                List<Rarity> fRarity = GetCheckedItemsRarity();
                List<ItemType> fType = GetCheckedItemsType();

                GameScene.Game.User.FiltersClass = String.Join(",", fClass);
                GameScene.Game.User.FiltersRarity = String.Join(",", fRarity);
                GameScene.Game.User.FiltersItemType = String.Join(",", fType);
                CEnvir.Enqueue(new C.SendCompanionFilters { FilterClass = GetCheckedItemsClass(), FilterRarity = GetCheckedItemsRarity(), FilterItemType = GetCheckedItemsType() });
            };
        }

        #region Methods
        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            GameScene.Game.CompanionBox.Location = new Point(nValue.X - GameScene.Game.CompanionBox.Size.Width, nValue.Y);
        }

        private List<MirClass> GetCheckedItemsClass()
        {
            List<MirClass> selected = new List<MirClass>();
            foreach(KeyValuePair<MirClass, DXCheckBox> pair in FilterClass)
            {
                if (pair.Value.Checked)
                {
                    selected.Add(pair.Key);
                }
            }
            return selected;
        }


        private List<Rarity> GetCheckedItemsRarity()
        {
            List<Rarity> selected = new List<Rarity>();
            foreach (KeyValuePair<Rarity, DXCheckBox> pair in FilterRarity)
            {
                if (pair.Value.Checked)
                {
                    selected.Add(pair.Key);
                }
            }
            return selected;
        }

        private List<ItemType> GetCheckedItemsType()
        {
            List<ItemType> selected = new List<ItemType>();
            foreach (KeyValuePair<ItemType, DXCheckBox> pair in FilterType)
            {
                if (pair.Value.Checked)
                {
                    selected.Add(pair.Key);
                }
            }
            return selected;
        }
        private void DrawItemTypeFilter()
        {
            Array itemTypes = Enum.GetValues(typeof(ItemType));
            int index = 0;
            int row = 0;
            foreach (ItemType itemType in itemTypes)
            {
                string item = itemType.ToString();
                if (item == "Nothing" || item == "Consumable" || item == "Torch" || item == "Poison" || item == "Amulet" || item == "Meat" || item == "Ore"
                || item == "DarkStone" || item == "RefineSpecial" || item == "HorseArmour" || item == "CompanionFood" || item == "System" || item == "ItemPart")
                {
                    continue;
                }

                FilterType[itemType] = new DXCheckBox
                {
                    Parent = this,
                    Hint = "Pick " + item.ToLower() + " items",
                };
                FilterType[itemType].Location = new Point(20  + (110 * index), 190 + (18 * row));

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Outline = true,
                    ForeColour = Color.AntiqueWhite,
                    OutlineColour = Color.Black,
                    IsControl = false,
                    Text = char.ToUpper(item[0]) + item.Substring(1)
                };
                label.Location = new Point(35 + (110 * index++), 190 + (18 * row));
                if (index % 3 == 0)
                {
                    row++;
                    index = 0;
                }
            }
        }

        private void DrawClassFilter()
        {
            Array classes = Enum.GetValues(typeof(MirClass));
            int index = 0;
            foreach (MirClass mirClass in classes)
            {
                FilterClass[mirClass] = new DXCheckBox
                {
                    Parent = this,
                    Hint = "Pick " + mirClass.ToString().ToLower() + " items",
                };
                FilterClass[mirClass].Location = new Point(20 + (70*index), 70);

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Outline = true,
                    ForeColour = Color.AntiqueWhite,
                    OutlineColour = Color.Black,
                    IsControl = false,
                    Text = char.ToUpper(mirClass.ToString()[0]) + mirClass.ToString().Substring(1)
                };
                label.Location = new Point(35 + (70*index++), 70);
            }
        }

        private void DrawRarityFilter()
        {
            Array rarities = Enum.GetValues(typeof(Rarity));
            int index = 0;
            foreach (Rarity rarity in rarities)
            {
                FilterRarity[rarity] = new DXCheckBox
                {
                    Parent = this,
                    Hint = "Pick " + rarity.ToString().ToLower() + " items",
                };
                FilterRarity[rarity].Location = new Point(20 + (70 * index), 130);
                Color rarityLabelColor = Color.AntiqueWhite;
                switch (rarity) {
                    case Rarity.Elite:
                        rarityLabelColor = Color.MediumPurple;
                        break;
                    case Rarity.Superior:
                        rarityLabelColor = Color.PaleGreen;
                        break;
                }
                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Outline = true,
                    ForeColour = rarityLabelColor,
                    OutlineColour = Color.Black,
                    IsControl = false,
                    Text = char.ToUpper(rarity.ToString()[0]) + rarity.ToString().Substring(1)
                };
                label.Location = new Point(35 + (70 * index++), 130);

            }

        }

        public void Refresh()
        {
            if (GameScene.Game.User.FiltersClass != String.Empty)
            {
                List<string> list = GameScene.Game.User.FiltersClass.Split(',').ToList();
                foreach (KeyValuePair<MirClass, DXCheckBox> pair in FilterClass)
                {
                    string result = list.Find(x => x == pair.Key.ToString());
                    if (result != null)
                    {
                        pair.Value.Checked = true;
                    }
                }
            }

            if (GameScene.Game.User.FiltersRarity != String.Empty)
            {
                List<string> list = GameScene.Game.User.FiltersRarity.Split(',').ToList();
                foreach (KeyValuePair<Rarity, DXCheckBox> pair in FilterRarity)
                {
                    string result = list.Find(x => x == pair.Key.ToString());
                    if (result != null)
                    {
                        pair.Value.Checked = true;
                    }
                }
            }

            if (GameScene.Game.User.FiltersItemType != String.Empty)
            {
                List<string> list = GameScene.Game.User.FiltersItemType.Split(',').ToList();
                foreach (KeyValuePair<ItemType, DXCheckBox> pair in FilterType)
                {
                    string result = list.Find(x => x == pair.Key.ToString());
                    if (result != null)
                    {
                        pair.Value.Checked = true;
                    }
                }
            }
        }

        #endregion
    }
}
