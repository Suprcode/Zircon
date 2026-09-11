using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class CraftingDesignTab : DXTab
    {
        private DXItemCell[] IngredientCells = new DXItemCell[5];
        private DXLabel[] IngredientLabels = new DXLabel[5];

        public readonly int Design;
        public CraftingRecipeInfo Recipe;

        public CraftingDesignTab(int design, int minimumTabWidth, Point cellLocation, int rowSpacing, Size cellSize, Size labelSize)
        {
            Design = design;
            MinimumTabWidth = minimumTabWidth;
            BackColour = Color.Empty;
            Location = new Point(0, 22);
            TabButton.Label.Text = design switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                _ => design.ToString(),
            };
            TabButton.Hint = string.Format(CEnvir.Language.CraftingDesignLabel, design);

            for (int i = 0; i < IngredientCells.Length; i++)
            {
                IngredientCells[i] = new DXItemCell
                {
                    Parent = this,
                    Location = new Point(cellLocation.X, cellLocation.Y + i * rowSpacing),
                    Size = cellSize,
                    Border = false,
                    FixedBorder = true,
                    FixedBorderColour = true,
                    BorderColour = Color.Empty,
                    ReadOnly = true,
                    ItemGrid = new ClientUserItem[1],
                    Slot = 0,
                    ShowCountLabel = true,
                };

                IngredientLabels[i] = new DXLabel
                {
                    Parent = this,
                    AutoSize = false,
                    Location = new Point(cellLocation.X + cellSize.Width + 7, cellLocation.Y + i * rowSpacing),
                    Size = labelSize,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
                };
            }
        }

        public void Refresh()
        {
            List<CraftingIngredientInfo> ingredients = CraftingRecipeDialog.GetDesignIngredients(Recipe, Design);

            for (int i = 0; i < IngredientCells.Length; i++)
            {
                CraftingIngredientInfo ingredient = i < ingredients.Count ? ingredients[i] : null;
                long available = CraftingRecipeDialog.Available(ingredient?.Item);

                IngredientCells[i].Item = CraftingRecipeDialog.DisplayItem(ingredient?.Item, ingredient?.Amount ?? 0);
                IngredientCells[i].Visible = ingredient != null;
                IngredientLabels[i].Visible = ingredient != null;
                IngredientLabels[i].Text = ingredient == null ? string.Empty : $"{ingredient.Item.ItemName}  {available:#,##0} / {ingredient.Amount:#,##0}";
                IngredientLabels[i].ForeColour = available >= (ingredient?.Amount ?? 0) ? Color.White : Color.Red;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Recipe = null;

            if (IngredientCells != null)
            {
                for (int i = 0; i < IngredientCells.Length; i++)
                {
                    if (IngredientCells[i] != null)
                    {
                        if (!IngredientCells[i].IsDisposed)
                            IngredientCells[i].Dispose();

                        IngredientCells[i] = null;
                    }
                }

                IngredientCells = null;
            }

            if (IngredientLabels != null)
            {
                for (int i = 0; i < IngredientLabels.Length; i++)
                {
                    if (IngredientLabels[i] != null)
                    {
                        if (!IngredientLabels[i].IsDisposed)
                            IngredientLabels[i].Dispose();

                        IngredientLabels[i] = null;
                    }
                }

                IngredientLabels = null;
            }
        }
    }

    public sealed class CharacterCraftingControl : DXControl
    {
        private DXLabel LevelTitleLabel, ExperienceTitleLabel, FavouriteTitleLabel, ResultTitleLabel, MaterialsTitleLabel;
        private DXLabel LevelLabel, ExperienceLabel, ItemNameLabel, SuccessLabel, DurationLabel;
        private DXControl ExperienceBarClip;
        private DXImageControl ExperienceBar;
        private DXItemCell ResultCell;
        private DXButton CraftButton, RecipesButton;
        private DXTabControl DesignTabControl;
        private CraftingDesignTab[] DesignTabs = new CraftingDesignTab[3];
        private CraftingRecipeInfo Recipe;

        public CharacterCraftingControl()
        {
            Size = new Size(331, 443);

            LevelTitleLabel = CreateCaption(CEnvir.Language.CraftingLevelLabel, new Point(10, 57), new Size(86, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            ExperienceTitleLabel = CreateCaption(CEnvir.Language.CraftingExperienceLabel, new Point(99, 57), new Size(217, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            FavouriteTitleLabel = CreateCaption(CEnvir.Language.CraftingFavouriteLabel, new Point(90, 107), new Size(151, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            ResultTitleLabel = CreateCaption(CEnvir.Language.CraftingResultLabel, new Point(17, 131), new Size(90, 18), TextFormatFlags.VerticalCenter);
            MaterialsTitleLabel = CreateCaption(CEnvir.Language.CharacterCraftingMaterialsLabel, new Point(16, 201), new Size(64, 20), TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
            ApplySectionTitleStyle(FavouriteTitleLabel);
            ApplySectionTitleStyle(ResultTitleLabel);
            ApplySectionTitleStyle(MaterialsTitleLabel);

            LevelLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(14, 80),
                Size = new Size(78, 18),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            ExperienceLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(222, 78),
                Size = new Size(84, 18),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            ExperienceBarClip = new DXControl
            {
                Parent = this,
                Location = new Point(109, 88),
                Size = new Size(0, 6),
                IsControl = false,
            };

            ExperienceBar = new DXImageControl
            {
                Parent = ExperienceBarClip,
                LibraryFile = LibraryFile.GameInter,
                Index = 6260,
                IsControl = false,
            };

            ResultCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(34, 153),
                Size = new Size(36, 36),
                Border = false,
                FixedBorder = true,
                FixedBorderColour = true,
                BorderColour = Color.Empty,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = true,
            };

            ItemNameLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(78, 148),
                Size = new Size(222, 15),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };

            SuccessLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(78, 163),
                Size = new Size(222, 15),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };

            DurationLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(78, 178),
                Size = new Size(222, 15),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };

            DesignTabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(48, 197),
                Size = new Size(266, 193),
                MarginLeft = 37,
                Border = false,
            };

            for (int i = 0; i < DesignTabs.Length; i++)
            {
                DesignTabs[i] = new CraftingDesignTab(i + 1, 40, new Point(16, 5), 32, new Size(32, 32), new Size(198, 32))
                {
                    Parent = DesignTabControl,
                };
            }

            DesignTabControl.SelectedTabChanged += (o, e) => RefreshCraftButton();

            CraftButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Location = new Point(29, 401),
                Size = new Size(120, DefaultHeight),
                Label = { Text = CEnvir.Language.CraftingCraftButtonLabel },
            };
            CraftButton.MouseClick += (o, e) => CraftingRecipeDialog.StartCraft(Recipe, SelectedDesign);

            RecipesButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Location = new Point(182, 401),
                Size = new Size(120, DefaultHeight),
                Label = { Text = CEnvir.Language.CraftingOpenRecipesButtonLabel },
            };
            RecipesButton.MouseClick += (o, e) => GameScene.Game.CraftingRecipeBox.Visible = true;
        }

        private DXLabel CreateCaption(string text, Point location, Size size, TextFormatFlags drawFormat)
        {
            return new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Text = text,
                Location = location,
                Size = size,
                ForeColour = Constants.ActiveTabColour,
                DrawFormat = drawFormat,
            };
        }

        private static void ApplySectionTitleStyle(DXLabel label)
        {
            label.Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold);
            label.LabelStyle = DXLabelStyle.GoldTitle;
            label.IsControl = false;
        }

        private int SelectedDesign => DesignTabControl?.SelectedTab is CraftingDesignTab tab ? tab.Design : 0;

        public void Refresh()
        {
            UserObject user = GameScene.Game?.User;
            if (user == null) return;

            CraftingLevelInfo level = Globals.CraftingLevelInfoList?.Binding.FirstOrDefault(x => x.Level == user.CraftingLevel);
            bool maximum = level == null;
            LevelLabel.Text = maximum ? CEnvir.Language.CraftingMaxLevelLabel : user.CraftingLevel.ToString();
            ExperienceLabel.Text = maximum ? "0 / 0" : $"{user.CraftingExperience:#,##0} / {level.RequiredExperience:#,##0}";
            double experienceProgress = maximum ? 0D : level.RequiredExperience <= 0 ? 0D : Math.Clamp(user.CraftingExperience / (double)level.RequiredExperience, 0D, 1D);
            ExperienceBarClip.Size = new Size((int)(ExperienceBar.Size.Width * experienceProgress), ExperienceBar.Size.Height);

            Recipe = Globals.CraftingRecipeInfoList?.Binding.FirstOrDefault(x => x.Index == user.FavouriteCraftingRecipeIndex);
            ResultCell.Item = DisplayFavouriteItem();
            ItemNameLabel.Text = Recipe?.Item == null ? string.Empty : $"{Recipe.Item.ItemName} x{Recipe.Amount}";
            SuccessLabel.Text = Recipe == null ? string.Empty : CraftingRecipeDialog.SuccessRateText(Recipe);
            DurationLabel.Text = Recipe == null ? string.Empty : string.Format(CEnvir.Language.CraftingDurationLabel, Functions.ToString(Recipe.Duration, true, true));

            RefreshDesignTabs();
        }

        private ClientUserItem DisplayFavouriteItem()
        {
            return CraftingRecipeDialog.DisplayItem(Recipe?.Item, Recipe?.Amount ?? 0);
        }

        private void RefreshDesignTabs()
        {
            for (int i = 0; i < DesignTabs.Length; i++)
            {
                DesignTabs[i].Recipe = Recipe;
                DesignTabs[i].TabButton.Visible = CraftingRecipeDialog.HasDesign(Recipe, DesignTabs[i].Design);
                DesignTabs[i].Refresh();
            }

            DesignTabControl.TabsChanged();
            SelectBestDesign(DesignTabControl, Recipe);
            RefreshCraftButton();
        }

        private void RefreshCraftButton()
        {
            CraftButton.Enabled = CraftingRecipeDialog.IsDesignCraftable(Recipe, SelectedDesign);
        }

        internal static void SelectBestDesign(DXTabControl tabControl, CraftingRecipeInfo recipe)
        {
            if (tabControl?.SelectedTab is CraftingDesignTab selected && selected.TabButton.Visible) return;

            CraftingDesignTab first = null;
            foreach (DXControl control in tabControl.Controls)
            {
                if (control is not CraftingDesignTab tab || !tab.TabButton.Visible) continue;
                first ??= tab;
                if (!CraftingRecipeDialog.IsDesignCraftable(recipe, tab.Design)) continue;

                tabControl.SelectedTab = tab;
                return;
            }

            tabControl.SelectedTab = first;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Recipe = null;

            if (DesignTabs != null)
            {
                for (int i = 0; i < DesignTabs.Length; i++)
                {
                    if (DesignTabs[i] != null)
                    {
                        if (!DesignTabs[i].IsDisposed)
                            DesignTabs[i].Dispose();

                        DesignTabs[i] = null;
                    }
                }

                DesignTabs = null;
            }

            if (DesignTabControl != null)
            {
                if (!DesignTabControl.IsDisposed)
                    DesignTabControl.Dispose();

                DesignTabControl = null;
            }

            if (LevelLabel != null)
            {
                if (!LevelLabel.IsDisposed)
                    LevelLabel.Dispose();

                LevelLabel = null;
            }

            if (LevelTitleLabel != null)
            {
                if (!LevelTitleLabel.IsDisposed)
                    LevelTitleLabel.Dispose();

                LevelTitleLabel = null;
            }

            if (ExperienceTitleLabel != null)
            {
                if (!ExperienceTitleLabel.IsDisposed)
                    ExperienceTitleLabel.Dispose();

                ExperienceTitleLabel = null;
            }

            if (FavouriteTitleLabel != null)
            {
                if (!FavouriteTitleLabel.IsDisposed)
                    FavouriteTitleLabel.Dispose();

                FavouriteTitleLabel = null;
            }

            if (ResultTitleLabel != null)
            {
                if (!ResultTitleLabel.IsDisposed)
                    ResultTitleLabel.Dispose();

                ResultTitleLabel = null;
            }

            if (MaterialsTitleLabel != null)
            {
                if (!MaterialsTitleLabel.IsDisposed)
                    MaterialsTitleLabel.Dispose();

                MaterialsTitleLabel = null;
            }

            if (ExperienceLabel != null)
            {
                if (!ExperienceLabel.IsDisposed)
                    ExperienceLabel.Dispose();

                ExperienceLabel = null;
            }

            if (ExperienceBar != null)
            {
                if (!ExperienceBar.IsDisposed)
                    ExperienceBar.Dispose();

                ExperienceBar = null;
            }

            if (ExperienceBarClip != null)
            {
                if (!ExperienceBarClip.IsDisposed)
                    ExperienceBarClip.Dispose();

                ExperienceBarClip = null;
            }

            if (ItemNameLabel != null)
            {
                if (!ItemNameLabel.IsDisposed)
                    ItemNameLabel.Dispose();

                ItemNameLabel = null;
            }

            if (SuccessLabel != null)
            {
                if (!SuccessLabel.IsDisposed)
                    SuccessLabel.Dispose();

                SuccessLabel = null;
            }

            if (DurationLabel != null)
            {
                if (!DurationLabel.IsDisposed)
                    DurationLabel.Dispose();

                DurationLabel = null;
            }

            if (ResultCell != null)
            {
                if (!ResultCell.IsDisposed)
                    ResultCell.Dispose();

                ResultCell = null;
            }

            if (CraftButton != null)
            {
                if (!CraftButton.IsDisposed)
                    CraftButton.Dispose();

                CraftButton = null;
            }

            if (RecipesButton != null)
            {
                if (!RecipesButton.IsDisposed)
                    RecipesButton.Dispose();

                RecipesButton = null;
            }
        }
    }

    public sealed class CraftingRecipeDialog : DXImageControl
    {
        private const int PageSize = 12;

        private DXTabControl CategoryTabControl, DesignTabControl;
        private DXTab[] CategoryTabs = new DXTab[5];
        private CraftingDesignTab[] DesignTabs = new CraftingDesignTab[3];
        private DXButton[] RecipeButtons = new DXButton[PageSize];
        private DXButton CloseButton, PreviousPageButton, NextPageButton, FavouriteButton, CraftButton;
        private DXCheckBox CraftableCheckBox;
        private DXLabel TitleLabel, RecipeListTitleLabel, ResultTitleLabel, LevelTitleLabel, GoldTitleLabel, ExperienceTitleLabel, MaterialsTitleLabel;
        private DXLabel PageLabel, ItemNameLabel, LevelLabel, GoldLabel, ExperienceLabel, SuccessLabel, DurationLabel;
        private DXItemCell ResultCell;
        private Dictionary<int, long> MaterialTotals = new Dictionary<int, long>();
        private CraftingCategory Category;
        private CraftingRecipeInfo SelectedRecipe;
        private int Page;

        public WindowSetting Settings;
        public WindowType Type => WindowType.CraftingBox;

        public CraftingRecipeDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 330;
            Movable = true;
            Sort = true;
            DropShadow = true;

            CloseButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = 15,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft,
            };
            CloseButton.Location = new Point(Size.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXWindowTitleLabel
            {
                Parent = this,
                Text = CEnvir.Language.CraftingRecipesTitle,
            };

            RecipeListTitleLabel = CreateSectionTitleLabel(CEnvir.Language.CraftingRecipeListLabel, new Point(14, 67), new Size(194, 18));
            ResultTitleLabel = CreateSectionTitleLabel(CEnvir.Language.CraftingResultLabel, new Point(232, 67), new Size(120, 17));

            CategoryTabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(8, 38),
                Size = new Size(488, 26),
                Border = false,
            };

            string[] categoryNames =
            {
                CEnvir.Language.CraftingWeaponCategory,
                CEnvir.Language.CraftingArmourCategory,
                CEnvir.Language.CraftingAccessoriesCategory,
                CEnvir.Language.CraftingConsumableCategory,
                CEnvir.Language.CraftingIngredientsCategory,
            };

            for (int i = 0; i < CategoryTabs.Length; i++)
            {
                CategoryTabs[i] = new DXTab
                {
                    Parent = CategoryTabControl,
                    BackColour = Color.Empty,
                    MinimumTabWidth = 68,
                    Location = new Point(0, 22),
                    TabButton = { Label = { Text = categoryNames[i] } },
                };
            }
            CategoryTabControl.SelectedTabChanged += CategoryTabControl_SelectedTabChanged;

            CraftableCheckBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(14, 92),
                Label = { Text = CEnvir.Language.CraftingOnlyCraftableLabel },
            };
            CraftableCheckBox.CheckedChanged += (o, e) =>
            {
                Page = 0;
                RefreshList();
            };

            for (int i = 0; i < RecipeButtons.Length; i++)
            {
                RecipeButtons[i] = new DXButton
                {
                    Parent = this,
                    LibraryFile = LibraryFile.GameInter,
                    Index = 3740,
                    HoverIndex = 3741,
                    PressedIndex = 3741,
                    Location = new Point(16, 116 + i * 28),
                    Size = new Size(192, 24),
                    Label = { ForeColour = Constants.InactiveTabColour },
                    Visible = false,
                };
                RecipeButtons[i].MouseClick += RecipeButton_MouseClick;
            }

            PreviousPageButton = new DXButton
            {
                Parent = this,
                Location = new Point(32, 470),
                Size = new Size(16, 16),
                Index = 3730,
                PressedIndex = 3731,
                LibraryFile = LibraryFile.GameInter,
            };
            PreviousPageButton.MouseClick += (o, e) =>
            {
                if (Page <= 0) return;
                Page--;
                RefreshList();
            };

            NextPageButton = new DXButton
            {
                Parent = this,
                Location = new Point(175, 470),
                Size = new Size(16, 16),
                Index = 3732,
                PressedIndex = 3733,
                LibraryFile = LibraryFile.GameInter,
            };
            NextPageButton.MouseClick += (o, e) =>
            {
                Page++;
                RefreshList();
            };

            PageLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(50, 466),
                Size = new Size(120, 20),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            ResultCell = CreateCell(this, new Point(229, 91), new Size(38, 38));
            ItemNameLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(273, 88),
                Size = new Size(214, 15),
                ForeColour = Constants.TextColour,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };
            SuccessLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(273, 103),
                Size = new Size(214, 15),
                ForeColour = Constants.TextColour,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };
            DurationLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(273, 118),
                Size = new Size(214, 15),
                ForeColour = Constants.TextColour,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };

            LevelLabel = CreateValueLabel(new Point(359, 137));
            GoldLabel = CreateValueLabel(new Point(359, 159));
            ExperienceLabel = CreateValueLabel(new Point(359, 181));
            LevelTitleLabel = CreateRequirementTitleLabel(CEnvir.Language.CraftingRequiredLevelTitleLabel, new Point(229, 137), new Size(126, 19));
            GoldTitleLabel = CreateRequirementTitleLabel(CEnvir.Language.CraftingRequiredGoldTitleLabel, new Point(229, 159), new Size(126, 19));
            ExperienceTitleLabel = CreateRequirementTitleLabel(CEnvir.Language.CraftingExperienceRewardTitleLabel, new Point(229, 181), new Size(126, 19));
            MaterialsTitleLabel = CreateSectionTitleLabel(CEnvir.Language.CraftingRequiredMaterialsLabel, new Point(229, 202), new Size(150, 18));

            DesignTabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(229, 218),
                Size = new Size(263, 244),
                MarginLeft = 0,
                Border = false,
            };

            for (int i = 0; i < DesignTabs.Length; i++)
            {
                DesignTabs[i] = new CraftingDesignTab(i + 1, 40, new Point(0, 6), 43, new Size(38, 38), new Size(216, 38))
                {
                    Parent = DesignTabControl,
                };
            }
            DesignTabControl.SelectedTabChanged += (o, e) => RefreshCraftButton();

            FavouriteButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Location = new Point(229, 466),
                Size = new Size(128, DefaultHeight),
                Label = { Text = CEnvir.Language.CraftingFavouriteButtonLabel },
            };
            FavouriteButton.MouseClick += (o, e) =>
            {
                if (SelectedRecipe == null) return;
                CEnvir.Enqueue(new C.CraftingSetFavourite { RecipeIndex = SelectedRecipe.Index });
            };

            CraftButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Location = new Point(364, 466),
                Size = new Size(128, DefaultHeight),
                Label = { Text = CEnvir.Language.CraftingCraftButtonLabel },
            };
            CraftButton.MouseClick += (o, e) => StartCraft(SelectedRecipe, SelectedDesign);

            Category = CraftingCategory.Weapon;
            RefreshList();
        }

        private int SelectedDesign => DesignTabControl?.SelectedTab is CraftingDesignTab tab ? tab.Design : 0;

        private DXLabel CreateValueLabel(Point location)
        {
            return new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = location,
                Size = new Size(132, 19),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
        }

        private DXLabel CreateSectionTitleLabel(string text, Point location, Size size)
        {
            return new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Text = text,
                Location = location,
                Size = size,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold),
                LabelStyle = DXLabelStyle.GoldTitle,
                DrawFormat = TextFormatFlags.VerticalCenter,
                IsControl = false,
            };
        }

        private DXLabel CreateRequirementTitleLabel(string text, Point location, Size size)
        {
            return new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Text = text,
                Location = location,
                Size = size,
                LabelStyle = DXLabelStyle.BlueTitle,
                DrawFormat = TextFormatFlags.VerticalCenter,
                IsControl = false,
            };
        }

        private static void SetRequirementState(DXLabel valueLabel, bool met)
        {
            valueLabel.ForeColour = met ? Constants.TextColour : Color.Red;
        }

        private static DXItemCell CreateCell(DXControl parent, Point location, Size size)
        {
            return new DXItemCell
            {
                Parent = parent,
                Location = location,
                Size = size,
                Border = false,
                FixedBorder = true,
                FixedBorderColour = true,
                BorderColour = Color.Empty,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = true,
            };
        }

        public static ClientUserItem DisplayItem(ItemInfo info, long count)
        {
            return info == null ? null : new ClientUserItem(info, count) { InfoIndex = info.Index };
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                BringToFront();
                RefreshAll();
            }

            if (Settings != null)
                Settings.Visible = nValue;
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
            if (e.KeyCode != Keys.Escape) return;

            Visible = false;
            if (!Config.EscapeCloseAll)
                e.Handled = true;
        }

        public void LoadSettings()
        {
            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);
            if (Settings != null)
            {
                Location = Settings.Location;
                Visible = Settings.Visible;
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Location = Location;
            Settings.Visible = Visible;
        }

        public void RefreshAll(bool refreshMaterials = true)
        {
            if (refreshMaterials)
                RefreshMaterialTotals();

            RefreshList();
            GameScene.Game?.CharacterBox?.RefreshCrafting();
        }

        private void RefreshMaterialTotals()
        {
            MaterialTotals.Clear();
            AddMaterialTotals(GameScene.Game?.Inventory);
            AddMaterialTotals(CEnvir.Storage);
        }

        private void AddMaterialTotals(IEnumerable<ClientUserItem> items)
        {
            if (items == null) return;

            foreach (ClientUserItem item in items)
            {
                if (item?.Info == null) continue;
                MaterialTotals[item.Info.Index] = MaterialTotals.TryGetValue(item.Info.Index, out long count) ? count + item.Count : item.Count;
            }
        }

        private void CategoryTabControl_SelectedTabChanged(object sender, EventArgs e)
        {
            int index = Array.IndexOf(CategoryTabs, CategoryTabControl.SelectedTab);
            if (index < 0) return;

            Category = (CraftingCategory)index;
            Page = 0;
            SetSelectedRecipe(null);
            RefreshList();
        }

        private List<CraftingRecipeInfo> CurrentRecipes()
        {
            IEnumerable<CraftingRecipeInfo> query = Globals.CraftingRecipeInfoList?.Binding.Where(x => x.Category == Category && x.Item != null) ?? Enumerable.Empty<CraftingRecipeInfo>();
            if (CraftableCheckBox.Checked)
                query = query.Where(IsRecipeCraftable);

            return query.OrderBy(x => x.Item.ItemName).ThenBy(x => x.Index).ToList();
        }

        private void RefreshList()
        {
            List<CraftingRecipeInfo> recipes = CurrentRecipes();
            int pages = Math.Max(1, (recipes.Count + PageSize - 1) / PageSize);
            Page = Math.Clamp(Page, 0, pages - 1);
            PageLabel.Text = $"{Page + 1} / {pages}";
            PreviousPageButton.Enabled = Page > 0;
            NextPageButton.Enabled = Page + 1 < pages;

            for (int i = 0; i < RecipeButtons.Length; i++)
            {
                int index = Page * PageSize + i;
                CraftingRecipeInfo recipe = index < recipes.Count ? recipes[index] : null;
                DXButton button = RecipeButtons[i];

                button.Tag = recipe;
                button.Visible = recipe != null;
                button.Label.Text = recipe == null ? string.Empty : $"{recipe.Item.ItemName} x{recipe.Amount}";
                SetRecipeButtonSelected(button, recipe == SelectedRecipe);
            }

            if (SelectedRecipe == null || !recipes.Contains(SelectedRecipe))
                SetSelectedRecipe(recipes.FirstOrDefault());
            else
                RefreshSelectedRecipe();
        }

        private void RecipeButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender is DXButton button)
                SetSelectedRecipe(button.Tag as CraftingRecipeInfo);
        }

        private void SetSelectedRecipe(CraftingRecipeInfo recipe)
        {
            if (SelectedRecipe == recipe)
            {
                RefreshSelectedRecipe();
                return;
            }

            SelectedRecipe = recipe;
            foreach (DXButton button in RecipeButtons)
                SetRecipeButtonSelected(button, button.Tag == SelectedRecipe);

            RefreshSelectedRecipe();
        }

        private static void SetRecipeButtonSelected(DXButton button, bool selected)
        {
            button.Index = selected ? 3741 : 3740;
            button.HoverIndex = 3741;
            button.Label.ForeColour = selected ? Constants.ActiveTabColour : Constants.InactiveTabColour;
        }

        private void RefreshSelectedRecipe()
        {
            CraftingRecipeInfo recipe = SelectedRecipe;
            ResultCell.Item = DisplayItem(recipe?.Item, recipe?.Amount ?? 0);
            ItemNameLabel.Text = recipe?.Item == null ? string.Empty : $"{recipe.Item.ItemName} x{recipe.Amount}";
            SuccessLabel.Text = recipe == null ? string.Empty : SuccessRateText(recipe);
            DurationLabel.Text = recipe == null ? string.Empty : string.Format(CEnvir.Language.CraftingDurationLabel, Functions.ToString(recipe.Duration, true, true));
            LevelLabel.Text = recipe?.RequiredLevel.ToString() ?? string.Empty;
            GoldLabel.Text = recipe == null ? string.Empty : recipe.RequiredGold.ToString("#,##0");
            ExperienceLabel.Text = recipe == null ? string.Empty : recipe.Experience.ToString("#,##0");

            UserObject user = GameScene.Game?.User;
            SetRequirementState(LevelLabel, recipe == null || user?.CraftingLevel >= recipe.RequiredLevel);
            SetRequirementState(GoldLabel, recipe == null || user?.Gold.Amount >= recipe.RequiredGold);

            for (int i = 0; i < DesignTabs.Length; i++)
            {
                DesignTabs[i].Recipe = recipe;
                DesignTabs[i].TabButton.Visible = HasDesign(recipe, DesignTabs[i].Design);
                DesignTabs[i].Refresh();
            }

            DesignTabControl.TabsChanged();
            CharacterCraftingControl.SelectBestDesign(DesignTabControl, recipe);
            FavouriteButton.Enabled = recipe != null && user != null && user.FavouriteCraftingRecipeIndex != recipe.Index;
            RefreshCraftButton();
        }

        private void RefreshCraftButton()
        {
            CraftButton.Enabled = IsDesignCraftable(SelectedRecipe, SelectedDesign);
        }

        public static int EffectiveSuccessRate(CraftingRecipeInfo recipe)
        {
            return Math.Clamp(recipe.SuccessRate + (GameScene.Game?.User?.Stats[Stat.CraftingSuccess] ?? 0), 0, 100);
        }

        public static string SuccessRateText(CraftingRecipeInfo recipe)
        {
            int rate = EffectiveSuccessRate(recipe);
            int bonus = Math.Max(0, rate - Math.Clamp(recipe.SuccessRate, 0, 100));

            return bonus > 0
                ? string.Format(CEnvir.Language.CraftingSuccessRateBonusLabel, rate, bonus)
                : string.Format(CEnvir.Language.CraftingSuccessRateLabel, rate);
        }

        public static long Available(ItemInfo item)
        {
            CraftingRecipeDialog dialog = GameScene.Game?.CraftingRecipeBox;
            return item != null && dialog?.MaterialTotals?.TryGetValue(item.Index, out long count) == true ? count : 0;
        }

        private static bool IsRecipeCraftable(CraftingRecipeInfo recipe)
        {
            return IsDesignCraftable(recipe, 1) || IsDesignCraftable(recipe, 2) || IsDesignCraftable(recipe, 3);
        }

        public static bool HasDesign(CraftingRecipeInfo recipe, int design)
        {
            return GetDesignIngredients(recipe, design).Count > 0;
        }

        public static List<CraftingIngredientInfo> GetDesignIngredients(CraftingRecipeInfo recipe, int design)
        {
            if (recipe == null) return new List<CraftingIngredientInfo>();

            return design switch
            {
                1 => recipe.Design1Ingredients.OrderBy(x => x.Item?.ItemName).ThenBy(x => x.Index).ToList(),
                2 => recipe.Design2Ingredients.OrderBy(x => x.Item?.ItemName).ThenBy(x => x.Index).ToList(),
                3 => recipe.Design3Ingredients.OrderBy(x => x.Item?.ItemName).ThenBy(x => x.Index).ToList(),
                _ => new List<CraftingIngredientInfo>(),
            };
        }

        public static bool IsDesignCraftable(CraftingRecipeInfo recipe, int design)
        {
            UserObject user = GameScene.Game?.User;
            List<CraftingIngredientInfo> ingredients = GetDesignIngredients(recipe, design);
            if (recipe?.Item == null || user == null || ingredients.Count is < 1 or > 5 || user.CraftingLevel < recipe.RequiredLevel || user.Gold.Amount < recipe.RequiredGold) return false;

            return ingredients.All(x => x.Item != null && x.Amount > 0 && Available(x.Item) >= x.Amount) && ingredients.GroupBy(x => x.Item).All(x => x.Count() == 1);
        }

        public static void StartCraft(CraftingRecipeInfo recipe, int design)
        {
            if (recipe == null || !IsDesignCraftable(recipe, design)) return;

            CEnvir.Enqueue(new C.CraftingStart { RecipeIndex = recipe.Index, Design = design });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            SelectedRecipe = null;
            Settings = null;

            if (MaterialTotals != null)
            {
                MaterialTotals.Clear();
                MaterialTotals = null;
            }

            if (CategoryTabs != null)
            {
                for (int i = 0; i < CategoryTabs.Length; i++)
                {
                    if (CategoryTabs[i] != null)
                    {
                        if (!CategoryTabs[i].IsDisposed)
                            CategoryTabs[i].Dispose();

                        CategoryTabs[i] = null;
                    }
                }

                CategoryTabs = null;
            }

            if (DesignTabs != null)
            {
                for (int i = 0; i < DesignTabs.Length; i++)
                {
                    if (DesignTabs[i] != null)
                    {
                        if (!DesignTabs[i].IsDisposed)
                            DesignTabs[i].Dispose();

                        DesignTabs[i] = null;
                    }
                }

                DesignTabs = null;
            }

            if (RecipeButtons != null)
            {
                for (int i = 0; i < RecipeButtons.Length; i++)
                {
                    if (RecipeButtons[i] != null)
                    {
                        if (!RecipeButtons[i].IsDisposed)
                            RecipeButtons[i].Dispose();

                        RecipeButtons[i] = null;
                    }
                }

                RecipeButtons = null;
            }

            if (CategoryTabControl != null)
            {
                if (!CategoryTabControl.IsDisposed)
                    CategoryTabControl.Dispose();

                CategoryTabControl = null;
            }

            if (DesignTabControl != null)
            {
                if (!DesignTabControl.IsDisposed)
                    DesignTabControl.Dispose();

                DesignTabControl = null;
            }

            if (CloseButton != null)
            {
                if (!CloseButton.IsDisposed)
                    CloseButton.Dispose();

                CloseButton = null;
            }

            if (PreviousPageButton != null)
            {
                if (!PreviousPageButton.IsDisposed)
                    PreviousPageButton.Dispose();

                PreviousPageButton = null;
            }

            if (NextPageButton != null)
            {
                if (!NextPageButton.IsDisposed)
                    NextPageButton.Dispose();

                NextPageButton = null;
            }

            if (FavouriteButton != null)
            {
                if (!FavouriteButton.IsDisposed)
                    FavouriteButton.Dispose();

                FavouriteButton = null;
            }

            if (CraftButton != null)
            {
                if (!CraftButton.IsDisposed)
                    CraftButton.Dispose();

                CraftButton = null;
            }

            if (CraftableCheckBox != null)
            {
                if (!CraftableCheckBox.IsDisposed)
                    CraftableCheckBox.Dispose();

                CraftableCheckBox = null;
            }

            if (TitleLabel != null)
            {
                if (!TitleLabel.IsDisposed)
                    TitleLabel.Dispose();

                TitleLabel = null;
            }

            if (RecipeListTitleLabel != null)
            {
                if (!RecipeListTitleLabel.IsDisposed)
                    RecipeListTitleLabel.Dispose();

                RecipeListTitleLabel = null;
            }

            if (ResultTitleLabel != null)
            {
                if (!ResultTitleLabel.IsDisposed)
                    ResultTitleLabel.Dispose();

                ResultTitleLabel = null;
            }

            if (LevelTitleLabel != null)
            {
                if (!LevelTitleLabel.IsDisposed)
                    LevelTitleLabel.Dispose();

                LevelTitleLabel = null;
            }

            if (GoldTitleLabel != null)
            {
                if (!GoldTitleLabel.IsDisposed)
                    GoldTitleLabel.Dispose();

                GoldTitleLabel = null;
            }

            if (ExperienceTitleLabel != null)
            {
                if (!ExperienceTitleLabel.IsDisposed)
                    ExperienceTitleLabel.Dispose();

                ExperienceTitleLabel = null;
            }

            if (MaterialsTitleLabel != null)
            {
                if (!MaterialsTitleLabel.IsDisposed)
                    MaterialsTitleLabel.Dispose();

                MaterialsTitleLabel = null;
            }

            if (PageLabel != null)
            {
                if (!PageLabel.IsDisposed)
                    PageLabel.Dispose();

                PageLabel = null;
            }

            if (ItemNameLabel != null)
            {
                if (!ItemNameLabel.IsDisposed)
                    ItemNameLabel.Dispose();

                ItemNameLabel = null;
            }

            if (LevelLabel != null)
            {
                if (!LevelLabel.IsDisposed)
                    LevelLabel.Dispose();

                LevelLabel = null;
            }

            if (GoldLabel != null)
            {
                if (!GoldLabel.IsDisposed)
                    GoldLabel.Dispose();

                GoldLabel = null;
            }

            if (ExperienceLabel != null)
            {
                if (!ExperienceLabel.IsDisposed)
                    ExperienceLabel.Dispose();

                ExperienceLabel = null;
            }

            if (SuccessLabel != null)
            {
                if (!SuccessLabel.IsDisposed)
                    SuccessLabel.Dispose();

                SuccessLabel = null;
            }

            if (DurationLabel != null)
            {
                if (!DurationLabel.IsDisposed)
                    DurationLabel.Dispose();

                DurationLabel = null;
            }

            if (ResultCell != null)
            {
                if (!ResultCell.IsDisposed)
                    ResultCell.Dispose();

                ResultCell = null;
            }
        }
    }

    public sealed class CraftingProgressDialog : DXImageControl
    {
        private const int ActionButtonY = 156;

        private DXItemCell ItemCell;
        private DXLabel TitleLabel, NameLabel, StateLabel;
        private DXControl BarClip;
        private DXImageControl Bar;
        private DXButton ActionButton, RetryButton;
        private CraftingRecipeInfo Recipe;
        private CraftingResult? Result;
        private int Design;
        private DateTime StartTime;
        private TimeSpan Duration;
        private bool Active;

        public CraftingProgressDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 331;
            Sort = true;
            DropShadow = true;

            TitleLabel = new DXWindowTitleLabel
            {
                Parent = this,
                Text = CEnvir.Language.CraftingProgressTitle,
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(30, 59),
                Size = new Size(36, 36),
                Border = false,
                FixedBorder = true,
                FixedBorderColour = true,
                BorderColour = Color.Empty,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = true,
            };
            NameLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(80, 55),
                Size = new Size(188, 20),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };
            StateLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(80, 78),
                Size = new Size(188, 18),
                ForeColour = Constants.TextColour,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
            };
            BarClip = new DXControl
            {
                Parent = this,
                Location = new Point(26, 116),
                Size = new Size(0, 10),
            };
            Bar = new DXImageControl
            {
                Parent = BarClip,
                LibraryFile = LibraryFile.GameInter,
                Index = 3780,
                IsControl = false,
            };
            ActionButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Location = new Point((Size.Width - 90) / 2, ActionButtonY),
                Size = new Size(90, DefaultHeight),
            };
            ActionButton.MouseClick += ActionButton_MouseClick;
            RetryButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Location = new Point(154, ActionButtonY),
                Size = new Size(90, DefaultHeight),
                Label = { Text = CEnvir.Language.CraftingRetryButtonLabel },
                Visible = false,
            };
            RetryButton.MouseClick += RetryButton_MouseClick;
            Visible = false;
        }

        public void Start(CraftingRecipeInfo recipe, int design, TimeSpan duration)
        {
            Recipe = recipe;
            Design = design;
            Result = null;
            ItemCell.Item = CraftingRecipeDialog.DisplayItem(recipe?.Item, recipe?.Amount ?? 0);
            NameLabel.Text = recipe?.Item == null ? string.Empty : $"{recipe.Item.ItemName} x{recipe.Amount}";
            StateLabel.Text = CEnvir.Language.CraftingInProgressLabel;
            Bar.Index = 3780;
            Duration = duration > TimeSpan.Zero ? duration : TimeSpan.FromMilliseconds(100);
            StartTime = CEnvir.Now;
            Active = true;
            BarClip.Size = new Size(0, 10);
            ActionButton.Label.Text = CEnvir.Language.CommonControlCancel;
            ActionButton.Enabled = true;
            ActionButton.Visible = true;
            ActionButton.Location = new Point((Size.Width - ActionButton.Size.Width) / 2, ActionButtonY);
            RetryButton.Visible = false;
            Visible = true;
            BringToFront();
        }

        public void End(CraftingResult result, bool interrupted)
        {
            Active = false;
            Result = result;
            BarClip.Size = new Size(244, 10);

            switch (result)
            {
                case CraftingResult.Success:
                    Bar.Index = 3780;
                    StateLabel.Text = CEnvir.Language.CraftingSuccessLabel;
                    break;
                case CraftingResult.Failed:
                    Bar.Index = 3782;
                    StateLabel.Text = CEnvir.Language.CraftingFailedLabel;
                    break;
                default:
                    Bar.Index = 3784;
                    StateLabel.Text = CEnvir.Language.CraftingCancelledLabel;
                    break;
            }

            if (interrupted)
            {
                ActionButton.Visible = false;
                RetryButton.Visible = false;
                Visible = false;
                return;
            }

            ActionButton.Visible = true;
            ActionButton.Label.Text = CEnvir.Language.CommonControlClose;
            ActionButton.Enabled = true;
            ActionButton.Location = new Point((Size.Width - ActionButton.Size.Width) / 2, ActionButtonY);
            RetryButton.Visible = false;
            Visible = true;
            BringToFront();
        }

        public void RefreshActionButton()
        {
            if (ActionButton == null || RetryButton == null || Active) return;

            bool canRetry = Result == CraftingResult.Failed && CraftingRecipeDialog.IsDesignCraftable(Recipe, Design);
            RetryButton.Visible = canRetry;
            RetryButton.Enabled = canRetry;
            ActionButton.Location = canRetry ? new Point(52, ActionButtonY) : new Point((Size.Width - ActionButton.Size.Width) / 2, ActionButtonY);
        }

        private void ActionButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                ActionButton.Enabled = false;
                CEnvir.Enqueue(new C.CraftingCancel());
                return;
            }

            Visible = false;
        }

        private void RetryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (Active || Result != CraftingResult.Failed || !CraftingRecipeDialog.IsDesignCraftable(Recipe, Design)) return;

            ActionButton.Enabled = false;
            RetryButton.Enabled = false;
            CraftingRecipeDialog.StartCraft(Recipe, Design);
        }

        public override void Process()
        {
            base.Process();
            if (!Active) return;

            double progress = Math.Clamp((CEnvir.Now - StartTime).TotalMilliseconds / Duration.TotalMilliseconds, 0D, 1D);
            BarClip.Size = new Size((int)(244 * progress), 10);
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode != Keys.Escape || Active) return;

            Visible = false;
            if (!Config.EscapeCloseAll)
                e.Handled = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Recipe = null;
            Result = null;
            Design = 0;
            Active = false;

            if (TitleLabel != null)
            {
                if (!TitleLabel.IsDisposed)
                    TitleLabel.Dispose();

                TitleLabel = null;
            }

            if (ItemCell != null)
            {
                if (!ItemCell.IsDisposed)
                    ItemCell.Dispose();

                ItemCell = null;
            }

            if (NameLabel != null)
            {
                if (!NameLabel.IsDisposed)
                    NameLabel.Dispose();

                NameLabel = null;
            }

            if (StateLabel != null)
            {
                if (!StateLabel.IsDisposed)
                    StateLabel.Dispose();

                StateLabel = null;
            }

            if (Bar != null)
            {
                if (!Bar.IsDisposed)
                    Bar.Dispose();

                Bar = null;
            }

            if (BarClip != null)
            {
                if (!BarClip.IsDisposed)
                    BarClip.Dispose();

                BarClip = null;
            }

            if (ActionButton != null)
            {
                ActionButton.MouseClick -= ActionButton_MouseClick;

                if (!ActionButton.IsDisposed)
                    ActionButton.Dispose();

                ActionButton = null;
            }


            if (RetryButton != null)
            {
                RetryButton.MouseClick -= RetryButton_MouseClick;

                if (!RetryButton.IsDisposed)
                    RetryButton.Dispose();

                RetryButton = null;
            }
        }
    }
}
