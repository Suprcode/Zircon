using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class CompanionDialog : DXImageControl
    {
        #region Properties

        #region Main

        private DXTabControl TabControl;
        private DXTab CompanionTab;

        public DXItemCell[] EquipmentGrid;
        private CompanionLevelInfo Info;
        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;

        public DXLabel NameLabelTitle, LevelLabelTitle, ExperienceLabelTitle, HungerLabelTitle;

        public DXButton CloseButton, BonusButton, FilterButton, BagButton;

        public DXLabel TitleLabel, HungerLabel, HealthLabel, NameLabel, LevelLabel, ExperienceLabel;

        public DXControl HealthBar, ExperienceBar, HungerBar;

        #endregion

        #region Bonus

        public DXVScrollBar BonusScrollBar;
        public DXControl BonusControl;

        public List<CompanionBonusStat> BonusStats = new();

        #endregion

        #region Filter

        public DXImageControl FilterControl;

        public DXButton SaveFilterButton;

        public Dictionary<MirClass, DXCheckBox> FilterClass = new Dictionary<MirClass, DXCheckBox>();
        public Dictionary<Rarity, DXCheckBox> FilterRarity = new Dictionary<Rarity, DXCheckBox>();
        public Dictionary<ItemType, DXCheckBox> FilterType = new Dictionary<ItemType, DXCheckBox>();

        public DXCheckBox FilterTypeCommon;
        public DXCheckBox FilterTypeElite;
        public DXCheckBox FilterTypeSuperior;

        #endregion

        #region Bag

        public DXImageControl BagControl;

        public DXLabel WeightLabel;
        public DXControl WeightBar;

        public int BagWeight, MaxBagWeight, InventorySize;

        public DXItemGrid InventoryGrid;

        #endregion

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
                BringToFront();

            if (Settings != null)
                Settings.Visible = nValue;

            base.OnIsVisibleChanged(oValue, nValue);
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (CloseButton.Visible)
                    {
                        CloseButton.InvokeMouseClick();
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    break;
            }
        }

        #region Settings

        public WindowSetting Settings;
        public  WindowType Type => WindowType.CompanionBox;

        public void LoadSettings()
        {
            if (Type == WindowType.None || !CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);

            if (Settings != null)
            {
                ApplySettings();
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = Visible;
            Settings.Location = Location;
        }

        public void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            Visible = Settings.Visible;

            ChangeView("Bag");
        }

        #endregion

        #endregion

        public CompanionDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 141;
            Movable = true;
            Sort = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.CompanionDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            #region Main

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(0, 38),
                Size = new Size(DisplayArea.Width, DisplayArea.Height - 38),
                MarginLeft = 15
            };
            CompanionTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CompanionDialogCompanionTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 24)
            };

            CompanionDisplayPoint = new Point(90, 140);

            BonusButton = new DXButton
            {
                Parent = CompanionTab,
                Label = { Text = CEnvir.Language.CompanionDialogCompanionTabBonusButtonLabel },
                Location = new Point(10, 263),
                Size = new Size(70, DefaultHeight),
                Enabled = false,
            };
            BonusButton.MouseClick += (o, e) => ChangeView("Bonus");

            FilterButton = new DXButton
            {
                Parent = CompanionTab,
                Label = { Text = CEnvir.Language.CompanionDialogCompanionTabFilterButtonLabel },
                Location = new Point(90, 263),
                Size = new Size(70, DefaultHeight),
                Enabled = false,
            };
            FilterButton.MouseClick += (o, e) => ChangeView("Filter");

            BagButton = new DXButton
            {
                Parent = CompanionTab,
                Label = { Text = CEnvir.Language.CompanionDialogCompanionTabBagButtonLabel },
                Location = new Point(170, 263),
                Size = new Size(70, DefaultHeight),
                Enabled = false,
            };
            BagButton.MouseClick += (o, e) => ChangeView("Bag");

            EquipmentGrid = new DXItemCell[Globals.CompanionEquipmentSize];

            DXItemCell cell;
            EquipmentGrid[(int)CompanionSlot.Bag] = cell = new DXItemCell
            {
                Parent = CompanionTab,
                Location = new Point(198, 17),
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Bag,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 99);

            EquipmentGrid[(int)CompanionSlot.Head] = cell = new DXItemCell
            {
                Parent = CompanionTab,
                Location = new Point(198, 59),
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Head,
                GridType = GridType.CompanionEquipment,
            };
            cell.ItemChanged += (o, e) => CompanionEquipmentChanged();
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 100);

            EquipmentGrid[(int)CompanionSlot.Back] = cell = new DXItemCell
            {
                Parent = CompanionTab,
                Location = new Point(198, 103),
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Back,
                GridType = GridType.CompanionEquipment,
            };
            cell.ItemChanged += (o, e) => CompanionEquipmentChanged();
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 101);

            EquipmentGrid[(int)CompanionSlot.Food] = cell = new DXItemCell
            {
                Parent = CompanionTab,
                Location = new Point(24, 17),
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Food,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 102);

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary library);

            HealthBar = new DXControl
            {
                Parent = CompanionTab,
                Location = new Point(60, 123),
                Size = library.GetSize(4375),
                Visible = false
            };
            HealthBar.BeforeDraw += (o, e) =>
            {
                if (library == null) return;

                if (Info == null) return;

                float percent = Math.Min(1, Math.Max(0, GameScene.Game.Companion != null ? 1 : 0));

                if (percent == 0) return;

                MirImage image = library.CreateImage(4375, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(HealthBar.DisplayArea.X, HealthBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, HealthBar);
            };

            HealthLabel = new DXLabel
            {
                Parent = CompanionTab,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                AutoSize = false,
                Size = new Size(128, 20),
                Location = new Point(60, 117),
                Visible = false
            };

            NameLabel = new DXLabel
            {
                Parent = CompanionTab,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                AutoSize = false,
                Size = new Size(152, 17),
                Location = new Point(73, 156)
            };

            NameLabelTitle = new DXLabel
            {
                Parent = CompanionTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = CEnvir.Language.CompanionDialogCompanionTabNameLabel,
                AutoSize = false,
                Size = new Size(60, 17),
                Location = new Point(10, 156)
            };

            LevelLabel = new DXLabel
            {
                Parent = CompanionTab,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                AutoSize = false,
                Size = new Size(152, 17),
                Location = new Point(73, 178)
            };
            
            LevelLabelTitle = new DXLabel
            {
                Parent = CompanionTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = CEnvir.Language.CompanionDialogCompanionTabLevelLabel,
                AutoSize = false,
                Size = new Size(60, 17),
                Location = new Point(10, 178)
            };

            ExperienceBar = new DXControl
            {
                Parent = CompanionTab,
                Location = new Point(73, 202),
                Size = library.GetSize(4310),
            };
            ExperienceBar.BeforeDraw += (o, e) =>
            {
                if (library == null) return;

                if (Info == null) return;

                float percent = Math.Min(1, Math.Max(0, GameScene.Game.Companion.Experience / (float)Info.MaxExperience));

                if (percent == 0) return;

                MirImage image = library.CreateImage(4310, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(ExperienceBar.DisplayArea.X, ExperienceBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, ExperienceBar);
            };

            ExperienceLabel = new DXLabel
            {
                Parent = CompanionTab,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                AutoSize = false,
                Size = new Size(152, 17),
                Location = new Point(73, 200)
            };

            ExperienceLabelTitle = new DXLabel
            {
                Parent = CompanionTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = CEnvir.Language.CompanionDialogCompanionTabExpLabel,
                AutoSize = false,
                Size = new Size(60, 17),
                Location = new Point(10, 200)
            };

            HungerBar = new DXControl
            {
                Parent = CompanionTab,
                Location = new Point(73, 224),
                Size = library.GetSize(4311),
            };
            HungerBar.BeforeDraw += (o, e) =>
            {
                if (library == null) return;

                if (Info == null) return;

                float percent = Math.Min(1, Math.Max(0, GameScene.Game.Companion.Hunger / (float)Info.MaxHunger));

                if (percent == 0) return;

                MirImage image = library.CreateImage(4311, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(HungerBar.DisplayArea.X, HungerBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, HungerBar);
            };

            HungerLabel = new DXLabel
            {
                Parent = CompanionTab,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                AutoSize = false,
                Size = new Size(152, 17),
                Location = new Point(73, 222)
            };

            HungerLabelTitle = new DXLabel
            {
                Parent = CompanionTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = CEnvir.Language.CompanionDialogCompanionTabHungerLabel,
                AutoSize = false,
                Size = new Size(60, 17),
                Location = new Point(10, 222)
            };

            #endregion

            #region Bonus Panel

            BonusControl = new DXControl
            {
                Parent = CompanionTab,
                Visible = false,
                Location = new Point(252, 0),
                Size = new Size(208, 300),
            };

            BonusScrollBar = new DXVScrollBar
            {
                Parent = BonusControl,
                Size = new Size(14, BonusControl.Size.Height - 2),
                Location = new Point(BonusControl.Size.Width - 14, 1),
                VisibleSize = BonusControl.Size.Height,
                Visible = false,
                Change = 57
            };
            BonusControl.MouseWheel += BonusScrollBar.DoMouseWheel;

            int i = 0;
            CompanionBonusStat bonusStat;

            BonusStats.Add(bonusStat = new()
            {
                Parent = BonusControl,
                Location = new Point(0, 5 + (i * 57)),
                Index = i,
                Level = 3
            });
            bonusStat.MouseWheel += BonusScrollBar.DoMouseWheel;

            i++;
            BonusStats.Add(bonusStat = new()
            {
                Parent = BonusControl,
                Location = new Point(0, 5 + (i * 57)),
                Index = i,
                Level = 5
            });
            bonusStat.MouseWheel += BonusScrollBar.DoMouseWheel;

            i++;
            BonusStats.Add(bonusStat = new()
            {
                Parent = BonusControl,
                Location = new Point(0, 5 + (i * 57)),
                Index = i,
                Level = 7
            });
            bonusStat.MouseWheel += BonusScrollBar.DoMouseWheel;

            i++;
            BonusStats.Add(bonusStat = new()
            {
                Parent = BonusControl,
                Location = new Point(0, 5 + (i * 57)),
                Index = i,
                Level = 10
            });
            bonusStat.MouseWheel += BonusScrollBar.DoMouseWheel;

            i++;
            BonusStats.Add(bonusStat = new()
            {
                Parent = BonusControl,
                Location = new Point(0, 5 + (i * 57)),
                Index = i,
                Level = 11
            });
            bonusStat.MouseWheel += BonusScrollBar.DoMouseWheel;

            i++;
            BonusStats.Add(bonusStat = new()
            {
                Parent = BonusControl,
                Location = new Point(0, 5 + (i * 57)),
                Index = i,
                Level = 13
            });
            bonusStat.MouseWheel += BonusScrollBar.DoMouseWheel;

            i++;
            BonusStats.Add(bonusStat = new()
            {
                Parent = BonusControl,
                Location = new Point(0, 5 + (i * 57)),
                Index = i,
                Level = 15
            });
            bonusStat.MouseWheel += BonusScrollBar.DoMouseWheel;

            BonusScrollBar.MaxValue = (BonusStats.Count * 57) + 15;
            BonusScrollBar.ValueChanged += (o, e) => UpdateBonusLocations();

            #endregion

            #region Filter Panel

            DXLabel label;

            FilterControl = new DXImageControl
            {
                Index = 143,
                LibraryFile = LibraryFile.Interface,
                Parent = CompanionTab,
                Visible = false,
                Location = new Point(252, 0),
                Size = new Size(208, 300)
            };

            label = new DXLabel
            {
                Parent = FilterControl,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Class",
            };
            label.Location = new Point(10, 10);

            DrawClassFilter();

            label = new DXLabel
            {
                Parent = FilterControl,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Rarity Type",
            };
            label.Location = new Point(10, 70);

            DrawRarityFilter();

            label = new DXLabel
            {
                Parent = FilterControl,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Item Type",
            };
            label.Location = new Point(10, 130);

            DrawItemTypeFilter();

            SaveFilterButton = new DXButton
            {
                Parent = this,
                Label = { Text = "Save settings", },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            SaveFilterButton.Location = new Point(370, 40);
            SaveFilterButton.MouseClick += (o, e) =>
            {
                List<MirClass> fClass = GetCheckedItemsClass();
                List<Rarity> fRarity = GetCheckedItemsRarity();
                List<ItemType> fType = GetCheckedItemsType();

                GameScene.Game.User.FiltersClass = String.Join(",", fClass);
                GameScene.Game.User.FiltersRarity = String.Join(",", fRarity);
                GameScene.Game.User.FiltersItemType = String.Join(",", fType);
                CEnvir.Enqueue(new C.SendCompanionFilters { FilterClass = GetCheckedItemsClass(), FilterRarity = GetCheckedItemsRarity(), FilterItemType = GetCheckedItemsType() });
            };

            #endregion

            #region BagPanel

            BagControl = new DXImageControl
            {
                Index = 142,
                LibraryFile = LibraryFile.Interface,
                Parent = CompanionTab,
                Visible = false,
                Location = new Point(252, 0),
                Size = new Size(208, 300),
            };

            InventoryGrid = new DXItemGrid
            {
                GridSize = new Size(5, 6),
                Parent = BagControl,
                GridType = GridType.CompanionInventory,
                Location = new Point(10, 14),
                GridPadding = 1,
                BackColour = Color.Empty,
                Border = false
            };

            WeightBar = new DXControl
            {
                Parent = BagControl,
                Location = new Point(8, 266),
                Size = library.GetSize(4312),
            };
            WeightBar.BeforeDraw += (o, e) =>
            {
                if (library == null) return;

                if (Info == null) return;

                float percent = Math.Min(1, Math.Max(0, BagWeight / (float)MaxBagWeight));

                if (percent == 0) return;

                MirImage image = library.CreateImage(4312, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(WeightBar.DisplayArea.X, WeightBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, WeightBar);
            };

            WeightLabel = new DXLabel
            {
                Parent = BagControl,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(7, 264),
                AutoSize = false,
                Size = new Size(80, 15)
            };

            #endregion
        }

        #region Methods

        public void UpdateBonusLocations()
        {
            int y = -BonusScrollBar.Value;

            foreach (CompanionBonusStat row in BonusStats)
                row.Location = new Point(0, 5 + (57 * row.Index) + y);
        }

        public void UpdateBonus(int level, Stats stats)
        {
            var bonus = BonusStats.FirstOrDefault(x => x.Level == level);

            if (bonus == null) return;

            bonus.LevelLabel.Text = $"Lv. {level} Bonus";
            bonus.StatLabel.Text = stats == null ? "Not Gained" : stats.GetDisplay(stats.Values.Keys.First());
            bonus.StatLabel.ForeColour = stats == null ? Color.Red : Color.LimeGreen;
        }

        private void ChangeView(string view)
        {
            BonusButton.Enabled = false;
            FilterButton.Enabled = false;
            BagButton.Enabled = false;

            SaveFilterButton.Visible = false;

            BonusControl.Visible = false;
            FilterControl.Visible = false;
            BagControl.Visible = false;

            switch (view)
            {
                case "Bonus":
                    BonusControl.Visible = true;
                    BonusButton.Enabled = false;
                    FilterButton.Enabled = true;
                    BagButton.Enabled = true;
                    break;
                case "Filter":
                    FilterControl.Visible = true;
                    BonusButton.Enabled = true;
                    FilterButton.Enabled = false;
                    BagButton.Enabled = true;
                    SaveFilterButton.Visible = true;
                    break;
                case "Bag":
                    BagControl.Visible = true;
                    BonusButton.Enabled = true;
                    FilterButton.Enabled = true;
                    BagButton.Enabled = false;
                    break;
            }
        }

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
            CompanionDisplay.CompanionObject = new ClientCompanionObject
            {
                HeadShape = EquipmentGrid[(int)CompanionSlot.Head].Item?.Info.Shape ?? 0,
                BackShape = EquipmentGrid[(int)CompanionSlot.Back].Item?.Info.Shape ?? 0,
                Name = GameScene.Game.Companion.Name
            };

            NameLabel.Text = GameScene.Game.Companion.Name;

            Refresh();
        }

        public void CompanionEquipmentChanged()
        {
            if (CompanionDisplay == null || CompanionDisplay.CompanionObject == null)
                return;

            CompanionDisplay.CompanionObject.HeadShape = EquipmentGrid[(int)CompanionSlot.Head].Item?.Info.Shape ?? 0;
            CompanionDisplay.CompanionObject.BackShape = EquipmentGrid[(int)CompanionSlot.Back].Item?.Info.Shape ?? 0;
            CompanionDisplay.CompanionObject.Name = GameScene.Game.Companion.Name;
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
            LevelLabel.Text = "Lv. " + GameScene.Game.Companion.Level.ToString();

            Info = Globals.CompanionLevelInfoList.Binding.First(x => x.Level == GameScene.Game.Companion.Level);

            HealthLabel.Text = GameScene.Game.Companion != null ? "100%" : "0%";

            ExperienceLabel.Text = Info.MaxExperience > 0 ? $"{GameScene.Game.Companion.Experience / (decimal)Info.MaxExperience:p2}" : "100%";

            HungerLabel.Text = $"{GameScene.Game.Companion.Hunger} of {Info.MaxHunger}";

            WeightLabel.Text = $"{BagWeight} / {MaxBagWeight}";
            WeightLabel.ForeColour = BagWeight >= MaxBagWeight ? Color.Red : Color.White;

            UpdateBonus(3, GameScene.Game.Companion.Level3);
            UpdateBonus(5, GameScene.Game.Companion.Level5);
            UpdateBonus(7, GameScene.Game.Companion.Level7);
            UpdateBonus(10, GameScene.Game.Companion.Level10);
            UpdateBonus(11, GameScene.Game.Companion.Level11);
            UpdateBonus(13, GameScene.Game.Companion.Level13);
            UpdateBonus(15, GameScene.Game.Companion.Level15);

            for (int i = 0; i < InventoryGrid.Grid.Length; i++)
                InventoryGrid.Grid[i].Enabled = i < InventorySize;
        }

        #endregion

        #region Filter Methods

        private List<MirClass> GetCheckedItemsClass()
        {
            List<MirClass> selected = new List<MirClass>();
            foreach (KeyValuePair<MirClass, DXCheckBox> pair in FilterClass)
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
                if (item == "Nothing" || item == "Consumable" || item == "Torch" || item == "Poison" || item == "Amulet" || item == "Meat" || item == "Ore" || item == "Currency"
                || item == "DarkStone" || item == "RefineSpecial" || item == "HorseArmour" || item == "CompanionFood" || item == "System" || item == "ItemPart" || item.Contains("Companion")
                || item == "Hook" || item == "Float" || item == "Bait" || item == "Finder" || item == "Reel")
                {
                    continue;
                }

                FilterType[itemType] = new DXCheckBox
                {
                    Parent = FilterControl,
                    Hint = "Pick " + item.ToLower() + " items",
                };
                FilterType[itemType].Location = new Point(10 + (110 * index), 150 + (18 * row));

                DXLabel label = new DXLabel
                {
                    Parent = FilterControl,
                    Outline = true,
                    ForeColour = Color.AntiqueWhite,
                    OutlineColour = Color.Black,
                    IsControl = false,
                    Text = char.ToUpper(item[0]) + item.Substring(1)
                };
                label.Location = new Point(25 + (110 * index++), 150 + (18 * row));
                if (index % 2 == 0)
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
            int row = 0;

            foreach (MirClass mirClass in classes)
            {
                FilterClass[mirClass] = new DXCheckBox
                {
                    Parent = FilterControl,
                    Hint = "Pick " + mirClass.ToString().ToLower() + " items",
                };
                FilterClass[mirClass].Location = new Point(10 + (110 * index), 30 + (18 * row));

                DXLabel label = new DXLabel
                {
                    Parent = FilterControl,
                    Outline = true,
                    ForeColour = Color.AntiqueWhite,
                    OutlineColour = Color.Black,
                    IsControl = false,
                    Text = char.ToUpper(mirClass.ToString()[0]) + mirClass.ToString().Substring(1)
                };
                label.Location = new Point(25 + (110 * index++), 30 + (18 * row));

                if (index % 2 == 0)
                {
                    row++;
                    index = 0;
                }
            }
        }
        private void DrawRarityFilter()
        {
            Array rarities = Enum.GetValues(typeof(Rarity));
            int index = 0;
            int row = 0;

            foreach (Rarity rarity in rarities)
            {
                FilterRarity[rarity] = new DXCheckBox
                {
                    Parent = FilterControl,
                    Hint = "Pick " + rarity.ToString().ToLower() + " items",
                };
                FilterRarity[rarity].Location = new Point(10 + (110 * index), 90 + (18 * row));

                Color rarityLabelColor = Color.AntiqueWhite;
                switch (rarity)
                {
                    case Rarity.Elite:
                        rarityLabelColor = Color.MediumPurple;
                        break;
                    case Rarity.Superior:
                        rarityLabelColor = Color.PaleGreen;
                        break;
                }
                DXLabel label = new DXLabel
                {
                    Parent = FilterControl,
                    Outline = true,
                    ForeColour = rarityLabelColor,
                    OutlineColour = Color.Black,
                    IsControl = false,
                    Text = char.ToUpper(rarity.ToString()[0]) + rarity.ToString().Substring(1)
                };
                label.Location = new Point(25 + (110 * index++), 90 + (18 * row));

                if (index % 2 == 0)
                {
                    row++;
                    index = 0;
                }
            }
        }

        public void RefreshFilter()
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

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                EquipmentGrid = null;
                Info = null;
                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (BonusButton != null)
                {
                    if (!BonusButton.IsDisposed)
                        BonusButton.Dispose();

                    BonusButton = null;
                }

                if (FilterButton != null)
                {
                    if (!FilterButton.IsDisposed)
                        FilterButton.Dispose();

                    FilterButton = null;
                }

                if (BagButton != null)
                {
                    if (!BagButton.IsDisposed)
                        BagButton.Dispose();

                    BagButton = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (HealthBar != null)
                {
                    if (!HealthBar.IsDisposed)
                        HealthBar.Dispose();

                    HealthBar = null;
                }

                if (ExperienceBar != null)
                {
                    if (!ExperienceBar.IsDisposed)
                        ExperienceBar.Dispose();

                    ExperienceBar = null;
                }

                if (HungerBar != null)
                {
                    if (!HungerBar.IsDisposed)
                        HungerBar.Dispose();

                    HungerBar = null;
                }

                if (NameLabelTitle != null)
                {
                    if (!NameLabelTitle.IsDisposed)
                        NameLabelTitle.Dispose();

                    NameLabelTitle = null;
                }
                if (LevelLabelTitle != null)
                {
                    if (!LevelLabelTitle.IsDisposed)
                        LevelLabelTitle.Dispose();

                    LevelLabelTitle = null;
                }

                if (ExperienceLabelTitle != null)
                {
                    if (!ExperienceLabelTitle.IsDisposed)
                        ExperienceLabelTitle.Dispose();

                    ExperienceLabelTitle = null;
                }

                if (HungerLabelTitle != null)
                {
                    if (!HungerLabelTitle.IsDisposed)
                        HungerLabelTitle.Dispose();

                    HungerLabelTitle = null;
                }

                if (HungerLabel != null)
                {
                    if (!HungerLabel.IsDisposed)
                        HungerLabel.Dispose();

                    HungerLabel = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
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

                if (HealthBar != null)
                {
                    if (!HealthBar.IsDisposed)
                        HealthBar.Dispose();

                    HealthBar = null;
                }

                if (BonusScrollBar != null)
                {
                    if (!BonusScrollBar.IsDisposed)
                        BonusScrollBar.Dispose();

                    BonusScrollBar = null;
                }

                if (BonusControl != null)
                {
                    if (!BonusControl.IsDisposed)
                        BonusControl.Dispose();

                    BonusControl = null;
                }

                BonusStats = null;

                if (FilterControl != null)
                {
                    if (!FilterControl.IsDisposed)
                        FilterControl.Dispose();

                    FilterControl = null;
                }

                if (SaveFilterButton != null)
                {
                    if (!SaveFilterButton.IsDisposed)
                        SaveFilterButton.Dispose();

                    SaveFilterButton = null;
                }

                foreach (KeyValuePair<MirClass, DXCheckBox> pair in FilterClass)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                FilterClass.Clear();
                FilterClass = null;

                foreach (KeyValuePair<Rarity, DXCheckBox> pair in FilterRarity)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                FilterRarity.Clear();
                FilterRarity = null;

                foreach (KeyValuePair<ItemType, DXCheckBox> pair in FilterType)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                FilterType.Clear();
                FilterType = null;

                if (FilterTypeCommon != null)
                {
                    if (!FilterTypeCommon.IsDisposed)
                        FilterTypeCommon.Dispose();

                    FilterTypeCommon = null;
                }

                if (FilterTypeElite != null)
                {
                    if (!FilterTypeElite.IsDisposed)
                        FilterTypeElite.Dispose();

                    FilterTypeElite = null;
                }

                if (FilterTypeSuperior != null)
                {
                    if (!FilterTypeSuperior.IsDisposed)
                        FilterTypeSuperior.Dispose();

                    FilterTypeSuperior = null;
                }

                if (BagControl != null)
                {
                    if (!BagControl.IsDisposed)
                        BagControl.Dispose();

                    BagControl = null;
                }

                if (WeightLabel != null)
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }

                if (WeightBar != null)
                {
                    if (!WeightBar.IsDisposed)
                        WeightBar.Dispose();

                    WeightBar = null;
                }

                BagWeight = 0;
                MaxBagWeight = 0;
                InventorySize = 0;

                if (InventoryGrid != null)
                {
                    if (!InventoryGrid.IsDisposed)
                        InventoryGrid.Dispose();

                    InventoryGrid = null;
                }
            }
        }

        #endregion
    }

    public sealed class CompanionBonusStat : DXControl
    {
        #region Index

        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value) return;

                int oldValue = _Index;
                _Index = value;

                OnIndexChanged(oldValue, value);
            }
        }
        private int _Index;
        public event EventHandler<EventArgs> IndexChanged;
        public void OnIndexChanged(int oValue, int nValue)
        {
            IndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public int Level;

        public DXLabel LevelLabel, StatLabel;

        public CompanionBonusStat()
        {
            Size = new Size(215, 57);

            Random r = new Random();

            var i = r.Next(10);

            LevelLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                AutoSize = false,
                Size = new Size(this.Size.Width - 38, 17),
                Location = new Point(20, 8)
            };

            StatLabel = new DXLabel
            {
                Parent = this,
                Outline = true,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                OutlineColour = Color.Black,
                IsControl = false,
                AutoSize = false,
                Size = new Size(this.Size.Width - 38, 20),
                Location = new Point(20, 30)
            };
        }

        public void Update(int level, Stats stat)
        {
            LevelLabel.Text = $"Lv. {level} Skill";
            StatLabel.Text = stat.GetDisplay(stat.Values.Keys.FirstOrDefault());
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (StatLabel != null)
                {
                    if (!StatLabel.IsDisposed)
                        StatLabel.Dispose();

                    StatLabel = null;
                }
            }
        }

        #endregion
    }
}
