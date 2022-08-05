using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class CompanionDialog : DXWindow
    {
        #region Properties

        public DXItemCell[] EquipmentGrid;
        public DXItemGrid InventoryGrid;

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;

        public DXLabel WeightLabel, HungerLabel, NameLabel, LevelLabel, ExperienceLabel, Level3Label, Level5Label, Level7Label, Level10Label, Level11Label, Level13Label, Level15Label;
        public DXComboBox ModeComboBox;

        public int BagWeight, MaxBagWeight, InventorySize;


        public override WindowType Type => WindowType.CompanionBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        public CompanionDialog()
        {
            TitleLabel.Text = "Companion";
            SetClientSize(new Size(352, 341));

            CompanionDisplayPoint = new Point(ClientArea.X + 60, ClientArea.Y + 50);

            InventoryGrid = new DXItemGrid
            {
                GridSize = new Size(10, 4),
                Parent = this,
                GridType = GridType.CompanionInventory,
                Location = new Point(ClientArea.X, ClientArea.Y + 200),
            };

            EquipmentGrid = new DXItemCell[Globals.CompanionEquipmentSize];
            DXItemCell cell;
            EquipmentGrid[(int)CompanionSlot.Bag] = cell = new DXItemCell
            {
                Location = new Point(ClientArea.X + 146, ClientArea.Y + 5),
                Parent = this,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Bag,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 99);

            EquipmentGrid[(int)CompanionSlot.Head] = cell = new DXItemCell
            {
                Location = new Point(ClientArea.X + 186, ClientArea.Y + 5),
                Parent = this,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Head,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 100);

            EquipmentGrid[(int)CompanionSlot.Back] = cell = new DXItemCell
            {
                Location = new Point(ClientArea.X + 226, ClientArea.Y + 5),
                Parent = this,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Back,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 101);

            EquipmentGrid[(int)CompanionSlot.Food] = cell = new DXItemCell
            {
                Location = new Point(ClientArea.X + 266, ClientArea.Y + 5),
                Parent = this,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Food,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 102);

            // start companion filter
            DXButton CharacterButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 358,
                Parent = this,
                Location = new Point(ClientArea.X + 316, ClientArea.Y + 5),
                Hint = "Filter Companion Pick"
            };
            CharacterButton.MouseClick += (o, e) =>
            {
                GameScene.Game.CompanionFilterBox.Visible = !GameScene.Game.CompanionFilterBox.Visible;
            };
            // end companion filter

            DXCheckBox PickUpCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Pick up items:" },
                Visible = false
            };
            PickUpCheckBox.Location = new Point(ClientArea.Right - PickUpCheckBox.Size.Width +3, ClientArea.Y + 45);

            /*
            new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Abilities",
                Location = new Point(ClientArea.X + 196, CompanionDisplayPoint.Y - 20),
                Size = new Size(156, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
            */

            DXLabel label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 3",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y );

            Level3Label = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 3),
                Text = "Not Available"
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 5",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 20);

            Level5Label = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 23),
                Text = "Not Available"
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 7",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 40);

            Level7Label = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 43),
                Text = "Not Available"
            };


            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 10",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 60);

            Level10Label = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 63),
                Text = "Not Available"
            };


            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 11",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 80);

            Level11Label = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 83),
                Text = "Not Available"
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 13",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 100);

            Level13Label = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 103),
                Text = "Not Available"
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 15",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 120);

            Level15Label = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 123),
                Text = "Not Available"
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 43)
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Name",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 40);

            LevelLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 63)
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 60);

            ExperienceLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 83)
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Experience",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 80);

            HungerLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 103)
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Hunger",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 100);

            WeightLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 123)
            };

            label = new DXLabel
            {
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Weight",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 120);
        }

        #region Methods
        public void CompanionChanged()
        {
            if (GameScene.Game.Companion == null)
            {
                Visible = false;
                return;
            }

            InventoryGrid.ItemGrid = GameScene.Game.Companion.InventoryArray;

            foreach (DXItemCell cell in EquipmentGrid)
                cell.ItemGrid = GameScene.Game.Companion.EquipmentArray;

            CompanionDisplay = new MonsterObject(GameScene.Game.Companion.CompanionInfo);
            NameLabel.Text = GameScene.Game.Companion.Name;

            Refresh();

        }

        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }

        public override void Process()
        {
            base.Process();

            CompanionDisplay?.Process();
        }

        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();


            if (CompanionDisplay == null) return;

            int x = DisplayArea.X + CompanionDisplayPoint.X;
            int y = DisplayArea.Y + CompanionDisplayPoint.Y;

            if (CompanionDisplay.Image == MonsterImage.Companion_Donkey)
            {
                x += 10;
                y -= 5;
            }


            CompanionDisplay.DrawShadow(x, y);
            CompanionDisplay.DrawBody(x, y);
        }

        public void Refresh()
        {
            LevelLabel.Text = GameScene.Game.Companion.Level.ToString();

            CompanionLevelInfo info = Globals.CompanionLevelInfoList.Binding.First(x => x.Level == GameScene.Game.Companion.Level);

            ExperienceLabel.Text = info.MaxExperience > 0 ? $"{GameScene.Game.Companion.Experience/(decimal) info.MaxExperience:p2}" : "100%";

            HungerLabel.Text = $"{GameScene.Game.Companion.Hunger} of {info.MaxHunger}";

            WeightLabel.Text = $"{BagWeight} / {MaxBagWeight}";

            WeightLabel.ForeColour = BagWeight >= MaxBagWeight ? Color.Red : Color.White;

            Level3Label.Text = GameScene.Game.Companion.Level3 == null ? "Not Available" : GameScene.Game.Companion.Level3.GetDisplay(GameScene.Game.Companion.Level3.Values.Keys.First());

            Level5Label.Text = GameScene.Game.Companion.Level5 == null ? "Not Available" : GameScene.Game.Companion.Level5.GetDisplay(GameScene.Game.Companion.Level5.Values.Keys.First());
            
            Level7Label.Text = GameScene.Game.Companion.Level7 == null ? "Not Available" : GameScene.Game.Companion.Level7.GetDisplay(GameScene.Game.Companion.Level7.Values.Keys.First());

            Level10Label.Text = GameScene.Game.Companion.Level10 == null ? "Not Available" : GameScene.Game.Companion.Level10.GetDisplay(GameScene.Game.Companion.Level10.Values.Keys.First());

            Level11Label.Text = GameScene.Game.Companion.Level11 == null ? "Not Available" : GameScene.Game.Companion.Level11.GetDisplay(GameScene.Game.Companion.Level11.Values.Keys.First());

            Level13Label.Text = GameScene.Game.Companion.Level13 == null ? "Not Available" : GameScene.Game.Companion.Level13.GetDisplay(GameScene.Game.Companion.Level13.Values.Keys.First());

            Level15Label.Text = GameScene.Game.Companion.Level15 == null ? "Not Available" : GameScene.Game.Companion.Level15.GetDisplay(GameScene.Game.Companion.Level15.Values.Keys.First());

            for (int i = 0; i < InventoryGrid.Grid.Length; i++)
                InventoryGrid.Grid[i].Enabled = i < InventorySize;
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            GameScene.Game.CompanionFilterBox.Location = new Point(nValue.X + GameScene.Game.CompanionBox.Size.Width, nValue.Y);
        }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!IsVisible && GameScene.Game.CompanionFilterBox != null)
            {
                GameScene.Game.CompanionFilterBox.Visible = false;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;
                
                if (EquipmentGrid != null)
                {
                    for (int i = 0; i < EquipmentGrid.Length; i++)
                    {
                        if (EquipmentGrid[i] != null)
                        {
                            if (!EquipmentGrid[i].IsDisposed)
                                EquipmentGrid[i].Dispose();

                            EquipmentGrid[i] = null;
                        }
                    }

                    EquipmentGrid = null;
                }

                if (InventoryGrid != null)
                {
                    if (!InventoryGrid.IsDisposed)
                        InventoryGrid.Dispose();

                    InventoryGrid = null;
                }

                if (WeightLabel != null)
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }

                if (HungerLabel != null)
                {
                    if (!HungerLabel.IsDisposed)
                        HungerLabel.Dispose();

                    HungerLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (ExperienceLabel != null)
                {
                    if (!ExperienceLabel.IsDisposed)
                        ExperienceLabel.Dispose();

                    ExperienceLabel = null;
                }

                if (Level3Label != null)
                {
                    if (!Level3Label.IsDisposed)
                        Level3Label.Dispose();

                    Level3Label = null;
                }

                if (Level5Label != null)
                {
                    if (!Level5Label.IsDisposed)
                        Level5Label.Dispose();

                    Level5Label = null;
                }

                if (Level7Label != null)
                {
                    if (!Level7Label.IsDisposed)
                        Level7Label.Dispose();

                    Level7Label = null;
                }

                if (Level10Label != null)
                {
                    if (!Level10Label.IsDisposed)
                        Level10Label.Dispose();

                    Level10Label = null;
                }

                if (ModeComboBox != null)
                {
                    if (!ModeComboBox.IsDisposed)
                        ModeComboBox.Dispose();

                    ModeComboBox = null;
                }

                BagWeight = 0;
                MaxBagWeight = 0;
                InventorySize = 0;
            }

        }

        #endregion
    }
}
