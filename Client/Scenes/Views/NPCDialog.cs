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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using Font = System.Drawing.Font;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    public class ButtonInfo
    {
        public Rectangle Region;
        public int Index;
        public int Length;
    }

    public sealed partial class NPCDialog : DXControl
    {
        #region Properties

        private readonly Regex B = ButtonRegex();
        private readonly Regex C = ColourRegex();
        private readonly Regex V = ValueRegex();

        public NPCPage Page;
        private readonly DXControl PageTextContainer;
        private DXLabel PageText;

        private List<DXLabel> Buttons = new ();
        private bool Opened;

        private string CurrentPageSay;
        private bool Rolling = true;

        public DXButton CloseButton;
        private DXImageControl HeaderImage, FooterImage;
        private DXImageControl[] RowImages = new DXImageControl[6];
        private DXVScrollBar ScrollBar;

        private const int _HeaderHeight = 140;
        private const int _FooterHeight = 64;
        private const int _RowHeight = 20;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.NPCGoodsBox != null && !IsVisible)
                GameScene.Game.NPCGoodsBox.Visible = false;

            if (GameScene.Game.NPCRepairBox != null && !IsVisible)
                GameScene.Game.NPCRepairBox.Visible = false;

            if (GameScene.Game.NPCRefinementStoneBox != null && !IsVisible)
                GameScene.Game.NPCRefinementStoneBox.Visible = false;

            if (GameScene.Game.NPCRefineBox != null && !IsVisible)
                GameScene.Game.NPCRefineBox.Visible = false;

            if (GameScene.Game.NPCRefineRetrieveBox != null && !IsVisible)
                GameScene.Game.NPCRefineRetrieveBox.Visible = false;

            if (GameScene.Game.NPCQuestBox != null && !IsVisible)
                GameScene.Game.NPCQuestBox.Visible = false;

            if (GameScene.Game.NPCAdoptCompanionBox != null && !IsVisible)
                GameScene.Game.NPCAdoptCompanionBox.Visible = false;

            if (GameScene.Game.NPCCompanionStorageBox != null && !IsVisible)
                GameScene.Game.NPCCompanionStorageBox.Visible = false;

            if (GameScene.Game.NPCWeddingRingBox != null && !IsVisible)
                GameScene.Game.NPCWeddingRingBox.Visible = false;

            if (GameScene.Game.NPCMasterRefineBox != null && !IsVisible)
                GameScene.Game.NPCMasterRefineBox.Visible = false;

            if (GameScene.Game.NPCItemFragmentBox != null && !IsVisible)
                GameScene.Game.NPCItemFragmentBox.Visible = false;

            if (GameScene.Game.NPCAccessoryUpgradeBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryUpgradeBox.Visible = false;

            if (GameScene.Game.NPCAccessoryLevelBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryLevelBox.Visible = false;

            if (GameScene.Game.NPCAccessoryResetBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryResetBox.Visible = false;

            if (GameScene.Game.NPCWeaponCraftBox != null && !IsVisible)
                GameScene.Game.NPCWeaponCraftBox.Visible = false;

            if (GameScene.Game.NPCAccessoryRefineBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryRefineBox.Visible = false;

            if (GameScene.Game.NPCRollBox != null && !IsVisible)
                GameScene.Game.NPCRollBox.Visible = false;

            if (GameScene.Game.NPCQuestListBox != null && !IsVisible)
                GameScene.Game.NPCQuestListBox.Visible = false;

            GameScene.Game.InventoryBox.NormalMode();

            if (Opened)
            {
                GameScene.Game.NPCID = 0;
                Opened = false;
                CEnvir.Enqueue(new C.NPCClose());
            }

            if (IsVisible)
            {
                if (GameScene.Game.CharacterBox.Location.X < Size.Width)
                    GameScene.Game.CharacterBox.Location = new Point(Size.Width, 0);

                GameScene.Game.StorageBox.Location = new Point(Math.Max(0, GameScene.Game.InventoryBox.Location.X - GameScene.Game.StorageBox.Size.Width), GameScene.Game.InventoryBox.Location.Y);
                
                BringToFront();
            }
            else if (GameScene.Game.CharacterBox.Location.X == Size.Width)
            {
                GameScene.Game.CharacterBox.ApplySettings();
                GameScene.Game.StorageBox.ApplySettings();
            }
        }

        #endregion

        public NPCDialog()
        {
            Movable = false;
            Sort = true;

            HeaderImage = new DXImageControl
            {
                Parent = this,
                Index = 380,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(0, 0),
                IsControl = false
            };

            for (int i = 0; i < RowImages.Length; i++)
            {
                RowImages[i] = new DXImageControl
                {
                    Parent = this,
                    Index = 381,
                    LibraryFile = LibraryFile.GameInter,
                    Location = new Point(0, _HeaderHeight + i * _RowHeight),
                    IsControl = false,
                    Visible = false
                };
            }

            FooterImage = new DXImageControl
            {
                Parent = this,
                Index = 382,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(0, _HeaderHeight),
                IsControl = false
            };

            PageTextContainer = new DXControl
            {
                Parent = this,
                Location = new Point(15, 45),
                Size = new Size(350, 10)
            };

            PageText = new DXLabel
            {
                AutoSize = false,
                Outline = false,
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,
                Parent = PageTextContainer,
                Location = new Point(0, 0),
                Size = new Size(350, 10),
                ForeColour = Color.White
            };

            ScrollBar = new DXVScrollBar
            {
                Visible = true,
                Parent = this,
                Location = new Point(350, 45),
                Size = new Size(14, 349),
                VisibleSize = 10,
                Change = 1,
                MinValue = 0,
                MaxValue = 100,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 387, LibraryFile = LibraryFile.GameInter },
                DownButton = { Index = 385, LibraryFile = LibraryFile.GameInter },
                PositionBar = { Index = -1, LibraryFile = LibraryFile.None }
                //HideWhenNoScroll = true
            };
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;
            PageText.MouseWheel += ScrollBar.DoMouseWheel;

            SetSize(0);

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(380 - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            int y = -ScrollBar.Value;

            PageText.Location = new Point(0, 0 + y);
        }

        #region Methods

        private void SetSize(int pageTextHeight)
        {
            var overflow = pageTextHeight - _HeaderHeight - _FooterHeight + 35 + 45;
            var additionalRowCount = 0;

            if (overflow > 0)
            {
                additionalRowCount = Math.Min(overflow / _RowHeight, RowImages.Length);
            }

            for (int i = 0; i < RowImages.Length; i++)
            {
                RowImages[i].Visible = additionalRowCount > i;
            }

            FooterImage.Location = new Point(0, _HeaderHeight + additionalRowCount * _RowHeight);

            Size = new Size(380, _HeaderHeight + _FooterHeight + additionalRowCount * _RowHeight);

            PageText.Size = new Size(350, pageTextHeight);
            PageTextContainer.Size = new Size(350, Size.Height - 45 - 14);
            ScrollBar.Size = new Size(14, Size.Height - 45 - 14);

            ScrollBar.MaxValue = PageText.Size.Height - PageTextContainer.Size.Height + 14;
        }

        public void Response(S.NPCResponse info)
        {
            GameScene.Game.NPCID = info.ObjectID;
            GameScene.Game.NPCBox.Visible = true;

            Page = info.Page;

            CurrentPageSay = Page.Say;

            var text = CurrentPageSay = V.Replace(CurrentPageSay, match =>
            {
                string text = match.Groups["Text"].Value;
                string defaultText = match.Groups["Default"].Value;
                ClientNPCValues valueItem = info.Values.Find(item => item.ID.ToString() == text);
                return valueItem != null ? valueItem.Value : defaultText;
            });

            text = B.Replace(text, @"${Text}");
            text = C.Replace(text, @"${Text}");
            PageText.Text = text;

            int height = DXLabel.GetHeight(PageText, PageText.Size.Width).Height;
            SetSize(height);
            ProcessText(CurrentPageSay);

            Opened = true;

            GameScene.Game.NPCGoodsBox.Visible = false;
            GameScene.Game.NPCRepairBox.Visible = false;
            GameScene.Game.NPCRefineBox.Visible = false;
            GameScene.Game.NPCRefinementStoneBox.Visible = false;
            GameScene.Game.NPCRefineRetrieveBox.Visible = false;
            GameScene.Game.NPCAdoptCompanionBox.Visible = false;
            GameScene.Game.NPCCompanionStorageBox.Visible = false;
            GameScene.Game.NPCWeddingRingBox.Visible = false;
            GameScene.Game.NPCItemFragmentBox.Visible = false;
            GameScene.Game.NPCAccessoryUpgradeBox.Visible = false;
            GameScene.Game.NPCAccessoryLevelBox.Visible = false;
            GameScene.Game.NPCMasterRefineBox.Visible = false;
            GameScene.Game.NPCAccessoryResetBox.Visible = false;
            GameScene.Game.NPCWeaponCraftBox.Visible = false;
            GameScene.Game.NPCQuestListBox.Visible = false;

            if (Rolling)
                Rolling = false;
            else
                GameScene.Game.NPCRollBox.Visible = false;

            GameScene.Game.InventoryBox.NormalMode();

            switch (info.Page.DialogType)
            {
                case NPCDialogType.None:
                    {
                        ShowQuestList();
                    }
                    break;
                case NPCDialogType.BuySell:
                    GameScene.Game.NPCGoodsBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCGoodsBox.Visible = Page.Goods.Count > 0;
                    GameScene.Game.NPCGoodsBox.NewGoods(Page.Goods, Page.Currency);

                    if (Page.Types.Count > 0)
                    {
                        GameScene.Game.InventoryBox.SellMode(Page.Currency, Page.Types.Select(x => x.ItemType).ToList());
                        GameScene.Game.InventoryBox.Visible = true;
                    }

                    //exclusion to show quest list for sellmode as no dialog beneath
                    if (!GameScene.Game.NPCGoodsBox.Visible)
                    {
                        ShowQuestList();
                    }
                break;
                case NPCDialogType.Repair:
                    GameScene.Game.NPCRepairBox.Visible = true;
                    GameScene.Game.NPCRepairBox.Location = new Point(0, Size.Height);
                    break;
                case NPCDialogType.RefinementStone:
                    GameScene.Game.NPCRefinementStoneBox.Visible = true;
                    GameScene.Game.NPCRefinementStoneBox.Location = new Point(Size.Width - GameScene.Game.NPCRefinementStoneBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.Refine:
                    GameScene.Game.NPCRefineBox.Visible = true;
                    GameScene.Game.NPCRefineBox.Location = new Point(Size.Width - GameScene.Game.NPCRefineBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.MasterRefine:
                    GameScene.Game.NPCMasterRefineBox.Visible = true;
                    GameScene.Game.NPCMasterRefineBox.Location = new Point(Size.Width - GameScene.Game.NPCRefineBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.RefineRetrieve:
                    GameScene.Game.NPCRefineRetrieveBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCRefineRetrieveBox.Visible = true;
                    GameScene.Game.NPCRefineRetrieveBox.RefreshList();
                    break;
                case NPCDialogType.CompanionManage:
                    GameScene.Game.NPCCompanionStorageBox.Visible = true;
                    GameScene.Game.NPCCompanionStorageBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCAdoptCompanionBox.Visible = true;
                    GameScene.Game.NPCAdoptCompanionBox.Location = new Point(GameScene.Game.NPCCompanionStorageBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.WeddingRing:
                    GameScene.Game.NPCWeddingRingBox.Visible = true;
                    GameScene.Game.NPCWeddingRingBox.Location = new Point(Size.Width - GameScene.Game.NPCWeddingRingBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.ItemFragment:
                    GameScene.Game.NPCItemFragmentBox.Visible = true;
                    GameScene.Game.NPCItemFragmentBox.Location = new Point(Size.Width - GameScene.Game.NPCItemFragmentBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.AccessoryRefineUpgrade:
                    GameScene.Game.NPCAccessoryUpgradeBox.Visible = true;
                    GameScene.Game.NPCAccessoryUpgradeBox.Location = new Point(0, Size.Height);
                    break; 
                case NPCDialogType.AccessoryRefineLevel:
                    GameScene.Game.NPCAccessoryLevelBox.Visible = true;
                    GameScene.Game.NPCAccessoryLevelBox.Location = new Point(Size.Width - GameScene.Game.NPCAccessoryLevelBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.AccessoryReset:
                    GameScene.Game.NPCAccessoryResetBox.Visible = true;
                    GameScene.Game.NPCAccessoryResetBox.Location = new Point(Size.Width - GameScene.Game.NPCAccessoryResetBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.WeaponCraft:
                    GameScene.Game.NPCWeaponCraftBox.Visible = true;
                    GameScene.Game.NPCWeaponCraftBox.Location = new Point(Size.Width - GameScene.Game.NPCWeaponCraftBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.AccessoryRefine:
                    GameScene.Game.NPCAccessoryRefineBox.Visible = true;
                    GameScene.Game.NPCAccessoryRefineBox.Location = new Point(Size.Width - GameScene.Game.NPCAccessoryRefineBox.Size.Width, Size.Height);
                    break;
                case NPCDialogType.RollDie:
                    Rolling = true;
                    CEnvir.Enqueue(new C.NPCRoll { Type = 0 });
                    break;
                case NPCDialogType.RollYut:
                    Rolling = true;
                    CEnvir.Enqueue(new C.NPCRoll { Type = 1 });
                    break;
            }
        }

        private void ShowQuestList()
        {
            GameScene.Game.NPCQuestListBox.NPCInfo = null;
            GameScene.Game.NPCQuestListBox.Location = new Point(0, GameScene.Game.NPCBox.Size.Height);

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.NPC || ob.ObjectID != GameScene.Game.NPCID) continue;

                GameScene.Game.NPCQuestListBox.NPCInfo = ((NPCObject)ob).NPCInfo;

                break;
            }
        }
        
        private void ProcessText(string page)
        {
            foreach (DXLabel label in Buttons)
                label.Dispose();

            Buttons.Clear();

            List<ButtonIndex> buttonRanges = new();

            List<Match> matchList = new();
            matchList.AddRange(B.Matches(page).Cast<Match>());
            matchList.AddRange(C.Matches(page).Cast<Match>());

            matchList = matchList.OrderBy(x => x.Groups["Text"].Index).ToList();

            int offset = 1;
            foreach (Match match in matchList)
            {
                ButtonIndex index = new()
                {
                    Range = new CharacterRange(match.Groups["Text"].Index - offset, match.Groups["Text"].Length)
                };

                buttonRanges.Add(index);

                if (!string.IsNullOrEmpty(match.Groups["ID"].Value))
                {
                    index.Type = ButtonType.Button;
                    offset += 3 + match.Groups["ID"].Length;
                }
                else if (!string.IsNullOrEmpty(match.Groups["Colour"].Value))
                {
                    index.Type = ButtonType.Label;
                    offset += 3 + match.Groups["Colour"].Length;
                }
            }

            for (int i = 0; i < buttonRanges.Count; i++)
            {
                var buttonIndex = buttonRanges[i];

                List<ButtonInfo> buttons = GetWordRegionsNew(DXManager.Graphics, PageText.Text, PageText.Font, PageText.DrawFormat, PageText.Size.Width, buttonIndex.Range.First, buttonIndex.Range.Length);

                List<DXLabel> labels = new();

                foreach (ButtonInfo info in buttons)
                {
                    labels.Add(new DXLabel
                    {
                        AutoSize = false,
                        Parent = PageText,
                        Location = info.Region.Location,
                        DrawFormat = PageText.DrawFormat,
                        Text = PageText.Text.Substring(info.Index, info.Length),
                        Font = PageText.Font,
                        Size = info.Region.Size,
                        Outline = false
                    });
                }

                int index = i;
                DateTime NextButtonTime = DateTime.MinValue;
                foreach (DXLabel label in labels)
                {
                    switch (buttonIndex.Type)
                    {
                        case ButtonType.Button:
                            {
                                label.ForeColour = Color.Yellow;
                                label.Sound = SoundIndex.ButtonC;

                                label.MouseEnter += (o, e) =>
                                {
                                    if (GameScene.Game.Observer) return;
                                    foreach (DXLabel l in labels)
                                        l.ForeColour = Color.Red;
                                };
                                label.MouseLeave += (o, e) =>
                                {
                                    if (GameScene.Game.Observer) return;
                                    foreach (DXLabel l in labels)
                                        l.ForeColour = Color.Yellow;
                                };
                                label.MouseClick += (o, e) =>
                                {
                                    if (GameScene.Game.Observer) return;

                                    if (matchList[index].Groups["ID"].Value == "0")
                                    {
                                        Visible = false;
                                        return;
                                    }

                                    if (CEnvir.Now < NextButtonTime) return;

                                    int idx = int.Parse(matchList[index].Groups["ID"].Value);

                                    NextButtonTime = CEnvir.Now.AddSeconds(1);

                                    CEnvir.Enqueue(new C.NPCButton { ButtonID = idx });
                                };
                            }
                            break;
                        case ButtonType.Label:
                            {
                                label.ForeColour = Color.FromName(matchList[index].Groups["Colour"].Value);
                            }
                            break;
                    }

                    Buttons.Add(label);
                }
            }
        }

        public static List<ButtonInfo> GetWordRegionsNew(Graphics graphics, string text, Font font, TextFormatFlags flags, int width, int index, int length)
        {
            List<ButtonInfo> regions = new List<ButtonInfo>();

            Size tSize = TextRenderer.MeasureText(graphics, "A", font, new Size(width, 2000), flags);
            int h = tSize.Height;
            int leading = tSize.Width - (TextRenderer.MeasureText(graphics, "AA", font, new Size(width, 2000), flags).Width - tSize.Width);

            int lineStart = 0;
            int lastHeight = h;

            Regex regex = new Regex(@"(?<Words>\S+)", RegexOptions.Compiled);

            MatchCollection matches = regex.Matches(text);

            List<CharacterRange> ranges = new List<CharacterRange>();

            foreach (Match match in matches)
                ranges.Add(new CharacterRange(match.Index, match.Length));

            ButtonInfo currentInfo = null;

            //If Word Wrap enabled.
            foreach (CharacterRange range in ranges)
            {
                int height = TextRenderer.MeasureText(graphics, text.Substring(0, range.First + range.Length), font, new Size(width, 9999), flags).Height;

                if (range.First >= index + length) break;

                if (height > lastHeight)
                {
                    lineStart = range.First; // New Line was formed record from start.
                    lastHeight = height;

                    //This Word is on a new line and therefore must start at 0.
                    //We do NOT know its length on this new line but since its on a new line it will be easy to measure.

                    if (range.First >= index)
                    {
                        //We need to capture this word
                        //It needs to be a new Rectangle.
                        Rectangle region = new Rectangle
                        {
                            X = 0,
                            Y = height - h,
                            Width = TextRenderer.MeasureText(graphics, text.Substring(range.First, range.Length), font, new Size(width, 9999), flags).Width,
                            Height = h,
                        };
                        currentInfo = new ButtonInfo { Region = region, Index = range.First, Length = range.Length };
                        regions.Add(currentInfo);
                    }
                }
                else
                {
                    //it is on the same Line IT Must be able to contain ALL of the letters. (Word Wrap)
                    //just need to know the length of the word and the Length of the start of the line to the start of the word

                    if (range.First >= index)
                    {
                        if (currentInfo == null)
                        {
                            Rectangle region = new Rectangle
                            {
                                X = TextRenderer.MeasureText(graphics, text.Substring(lineStart, range.First - lineStart), font, new Size(width, 9999), flags).Width,
                                Y = height - h,
                                Width = TextRenderer.MeasureText(graphics, text.Substring(range.First, range.Length), font, new Size(width, 9999), flags).Width,
                                Height = h,
                            };

                            if (region.X > 0)
                                region.X -= leading;
                            currentInfo = new ButtonInfo { Region = region, Index = range.First, Length = range.Length };
                            regions.Add(currentInfo);
                        }
                        else
                        {
                            //Measure Current.Index to range.First + Length
                            currentInfo.Length = range.First + range.Length - currentInfo.Index;
                            currentInfo.Region.Width = TextRenderer.MeasureText(graphics, text.Substring(currentInfo.Index, currentInfo.Length), font, new Size(width, 9999), flags).Width;
                        }
                        //We need to capture this word.
                        //ADD to any previous rects otherwise create new ?
                    }
                }
            }

            return regions;
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Visible = false;
                    break;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Page = null;

                if (PageText != null)
                {
                    if (!PageText.IsDisposed)
                        PageText.Dispose();

                    PageText = null;
                }

                if (HeaderImage != null)
                {
                    if (!HeaderImage.IsDisposed)
                        HeaderImage.Dispose();

                    HeaderImage = null;
                }

                if (FooterImage != null)
                {
                    if (!FooterImage.IsDisposed)
                        FooterImage.Dispose();

                    FooterImage = null;
                }

                if (RowImages != null)
                {
                    for (int i = 0; i < RowImages.Length; i++)
                    {
                        if (RowImages[i] != null)
                        {
                            if (!RowImages[i].IsDisposed)
                                RowImages[i].Dispose();

                            RowImages[i] = null;
                        }
                    }

                    RowImages = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (PageText != null)
                {
                    if (!PageText.IsDisposed)
                        PageText.Dispose();

                    PageText = null;
                }

                if (Buttons != null)
                {
                    for (int i = 0; i < Buttons.Count; i++)
                    {
                        if (Buttons[i] != null)
                        {
                            if (!Buttons[i].IsDisposed)
                                Buttons[i].Dispose();

                            Buttons[i] = null;
                        }
                    }

                    Buttons.Clear();
                    Buttons = null;
                }

                Opened = false;
            }
        }

        #endregion

        public class ButtonIndex
        {
            public CharacterRange Range;
            public ButtonType Type;
        };

        public enum ButtonType
        {
            Button,
            Label
        };

        [GeneratedRegex("\\<(?<Text>.*?):(?<Default>.+?)\\>", RegexOptions.Compiled)]
        private static partial Regex ValueRegex();

        [GeneratedRegex("\\{(?<Text>.*?):(?<Colour>.+?)\\}", RegexOptions.Compiled)]
        private static partial Regex ColourRegex();

        [GeneratedRegex("\\[(?<Text>.*?):(?<ID>.+?)\\]", RegexOptions.Compiled)]
        private static partial Regex ButtonRegex();
    }

    public sealed class NPCGoodsDialog : DXWindow
    {
        #region Properties

        #region SelectedCell

        public NPCGoodsCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                if (_SelectedCell == value) return;

                NPCGoodsCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private NPCGoodsCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(NPCGoodsCell oValue, NPCGoodsCell nValue)
        {
            if (oValue != null) oValue.Selected = false;
            if (nValue != null) nValue.Selected = true;

            BuyButton.Enabled = nValue != null;

            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        private DXVScrollBar ScrollBar;
        public DXCheckBox GuildCheckBox;

        private CurrencyInfo Currency;

        public List<NPCGoodsCell> Cells = new List<NPCGoodsCell>();
        private DXButton BuyButton;
        public DXControl ClientPanel;

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (ClientPanel == null) return;

            ClientPanel.Size = ClientArea.Size;
            ClientPanel.Location = ClientArea.Location;

        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCGoodsDialog()
        {
            TitleLabel.Text = "Goods";

            HasFooter = true;
            Movable = false;


            SetClientSize(new Size(227, 7*43 + 1));

            ClientPanel = new DXControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
                PassThrough = true,
            };

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(14, ClientArea.Height - 1),
            };
            ScrollBar.Location = new Point(ClientArea.Right - ScrollBar.Size.Width - 2, ClientArea.Y + 1);
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            MouseWheel += ScrollBar.DoMouseWheel;

            BuyButton = new DXButton
            {
                Location = new Point(40, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Buy" },
                Enabled = false,
            };
            BuyButton.MouseClick += (o, e) => Buy();

            GuildCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Use Guild Funds:" },
                Enabled = false,
            };
            GuildCheckBox.Location = new Point( 200, BuyButton.Location.Y + (BuyButton.Size.Height - GuildCheckBox.Size.Height) /2);
        }

        #region Methods

        public void NewGoods(IList<NPCGood> goods, CurrencyInfo currency)
        {
            Currency = currency ?? Globals.CurrencyInfoList.Binding.First(x => x.Type == CurrencyType.Gold);

            foreach (NPCGoodsCell cell in Cells)
                cell.Dispose();

            Cells.Clear();

            foreach (NPCGood good in goods)
            {
                NPCGoodsCell cell;
                Cells.Add(cell = new NPCGoodsCell
                {
                    Parent = ClientPanel,
                    Good = good,
                    Currency = Currency
                });
                cell.MouseClick += (o, e) => SelectedCell = cell;
                cell.MouseWheel += ScrollBar.DoMouseWheel;
                cell.MouseDoubleClick += (o, e) => Buy();
            }

            ScrollBar.MaxValue = goods.Count*43 - 2;
            SetClientSize(new Size(ClientArea.Width, Math.Min(ScrollBar.MaxValue, 7*43 - 3) + 1));
            ScrollBar.VisibleSize = ClientArea.Height;
            ScrollBar.Size = new Size(ScrollBar.Size.Width, ClientArea.Height - 2);

            BuyButton.Location = new Point(30, Size.Height - 43);

            GuildCheckBox.Checked = false;
            GuildCheckBox.Location = new Point(120, BuyButton.Location.Y + (BuyButton.Size.Height - GuildCheckBox.Size.Height) / 2);
            GuildCheckBox.Visible = Currency.Type == CurrencyType.Gold;

            ScrollBar.Value = 0;
            UpdateLocations();
        }
        private void UpdateLocations()
        {
            int y = -ScrollBar.Value + 1;

            foreach (NPCGoodsCell cell in Cells)
            {
                cell.Location = new Point(1, y);

                y += cell.Size.Height + 3;
            }
        }

        public void Buy()
        {
            if (GameScene.Game.Observer) return;

            if (SelectedCell == null) return;

            var userCurrency = GameScene.Game.User.GetCurrency(Currency);

            long gold = userCurrency.Amount;

            if (GuildCheckBox.Checked && GameScene.Game.GuildBox.GuildInfo != null)
                gold = GameScene.Game.GuildBox.GuildInfo.GuildFunds;

            var cost = (int)Math.Max(1, SelectedCell.Good.Cost * Currency.ExchangeRate);

            if (SelectedCell.Good.Item.StackSize > 1)
            {
                long maxCount = SelectedCell.Good.Item.StackSize;

                maxCount = Math.Min(maxCount, gold / cost);
                if (SelectedCell.Good.Item.Weight > 0)
                {
                    switch (SelectedCell.Good.Item.ItemType)
                    {
                        case ItemType.Amulet:
                        case ItemType.Poison:
                            if (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight < SelectedCell.Good.Item.Weight)
                            {
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.BuySellOverweight, SelectedCell.Good.Item.ItemName), MessageType.System);
                                return;
                            }
                            break;
                        default:
                            maxCount = Math.Min(maxCount, (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight) / SelectedCell.Good.Item.Weight);
                            break;
                    }
                }

                if (maxCount < 0)
                {
                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.BuySellOverweight, SelectedCell.Good.Item.ItemName), MessageType.System);
                    return;
                }

                ClientUserItem item = new ClientUserItem(SelectedCell.Good.Item, (int) Math.Min(int.MaxValue, maxCount));

                DXItemAmountWindow window = new DXItemAmountWindow("Buy Item", item);
                window.ConfirmButton.MouseClick += (o, e) =>
                {
                    CEnvir.Enqueue(new C.NPCBuy { Index = SelectedCell.Good.Index, Amount = window.Amount, GuildFunds = GuildCheckBox.Checked });
                    GuildCheckBox.Checked = false;
                };
            }
            else
            {
                if (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight < SelectedCell.Good.Item.Weight)
                {
                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.BuySellOverweight, SelectedCell.Good.Item.ItemName), MessageType.System);
                    return;
                }

                if (cost > gold)
                {
                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.BuySellNeedGold, SelectedCell.Good.Item.ItemName), MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.NPCBuy { Index = SelectedCell.Good.Index, Amount = 1, GuildFunds = GuildCheckBox.Checked });
                GuildCheckBox.Checked = false;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedCell = null;
                SelectedCellChanged = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }

                if (BuyButton != null)
                {
                    if (!BuyButton.IsDisposed)
                        BuyButton.Dispose();

                    BuyButton = null;
                }

                if (ClientPanel != null)
                {
                    if (!ClientPanel.IsDisposed)
                        ClientPanel.Dispose();

                    ClientPanel = null;
                }

                if (Cells != null)
                {
                    for (int i = 0; i < Cells.Count; i++)
                    {
                        if (Cells[i] != null)
                        {
                            if (!Cells[i].IsDisposed)
                                Cells[i].Dispose();

                            Cells[i] = null;
                        }
                    }

                    Cells.Clear();
                    Cells = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCGoodsCell : DXControl
    {
        #region Properties

        #region Good

        public NPCGood Good
        {
            get => _Good;
            set
            {
                if (_Good == value) return;

                NPCGood oldValue = _Good;
                _Good = value;

                OnGoodChanged(oldValue, value);
            }
        }
        private NPCGood _Good;
        public event EventHandler<EventArgs> GoodChanged;
        public void OnGoodChanged(NPCGood oValue, NPCGood nValue)
        {
            ItemCell.Item = new ClientUserItem(Good.Item, 1) { Flags = UserItemFlags.Locked  };
            
            switch (Good.Item.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Book:
                    ItemCell.Item.Flags |= UserItemFlags.NonRefinable;
                    break;
            }
            ItemNameLabel.Text = Good.Item.ItemName;

            switch (Good.Item.ItemType)
            {
                case ItemType.Nothing:
                    RequirementLabel.Text = string.Empty;
                    break;
                case ItemType.Meat:
                    RequirementLabel.Text = $"Quality: {Good.Item.Durability/1000}";
                    RequirementLabel.ForeColour = Color.Wheat;
                    break;
                case ItemType.Ore:
                    RequirementLabel.Text = $"Purity: {Good.Item.Durability/1000}";
                    RequirementLabel.ForeColour = Color.Wheat;
                    break;
                case ItemType.Consumable:
                case ItemType.Scroll:
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Torch:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.DarkStone:

                    if (GameScene.Game.CanUseItem(ItemCell.Item))
                    {
                        RequirementLabel.Text = "Can use Item";
                        RequirementLabel.ForeColour = Color.Aquamarine;
                    }
                    else
                    {
                        RequirementLabel.Text = "Cannot use Item";
                        RequirementLabel.ForeColour = Color.Red;
                    }
                    break;
            }

            UpdateCosts();

            GoodChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Currency

        public CurrencyInfo Currency
        {
            get => _Currency;
            set
            {
                if (_Currency == value) return;

                CurrencyInfo oldValue = _Currency;
                _Currency = value;

                OnCurrencyChanged(oldValue, value);
            }
        }
        private CurrencyInfo _Currency;

        public event EventHandler<EventArgs> CurrencyChanged;
        public void OnCurrencyChanged(CurrencyInfo oValue, CurrencyInfo nValue)
        {
            if (Currency == null || Currency.DropItem == null)
            {
                CurrencyIcon.LibraryFile = LibraryFile.Inventory;
                CurrencyIcon.Index = 121;
            }
            else
            {
                CurrencyIcon.LibraryFile = LibraryFile.Ground;
                CurrencyIcon.Index = Currency.DropItem.Image;
            }

            UpdateCosts();

            CurrencyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell ItemCell;

        public DXImageControl CurrencyIcon;
        public DXLabel ItemNameLabel, RequirementLabel, CostLabel;

        #endregion

        public NPCGoodsCell()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);
            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(219, 40);

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight)/2, (Size.Height - DXItemCell.CellHeight)/2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };
            ItemNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X*2 + ItemCell.Size.Width, ItemCell.Location.Y),
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            RequirementLabel = new DXLabel
            {
                Parent = this,
                Text = "Requirement",
                IsControl = false,
            };
            RequirementLabel.Location = new Point(ItemCell.Location.X*2 + ItemCell.Size.Width, ItemCell.Location.Y + ItemCell.Size.Height - RequirementLabel.Size.Height);

            CurrencyIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.Inventory,
                Index = 121,
                Parent = this,
                IsControl = false,
            };

            CostLabel = new DXLabel
            {
                Parent = this,
                IsControl = false,
            };
        }

        #region Methods

        public void UpdateCosts()
        {
            if (Good == null || Currency == null) return;

            var userCurrency = GameScene.Game.User.GetCurrency(Currency);

            var totalCost = (int)Math.Max(1, Good.Cost * Currency.ExchangeRate);

            CostLabel.Text = totalCost.ToString("##,##0");
            CostLabel.ForeColour = totalCost > userCurrency.Amount ? Color.Red : Color.Yellow;
            CostLabel.Location = new Point(Size.Width - CurrencyIcon.Size.Width - ItemCell.Location.X - 10 - CostLabel.Size.Width, ItemCell.Location.Y + ItemCell.Size.Height - RequirementLabel.Size.Height);

            CurrencyIcon.Location = new Point(Size.Width - CurrencyIcon.Size.Width - ItemCell.Location.X - 10, Size.Height - CurrencyIcon.Size.Height - ItemCell.Location.X);
        }

        public void UpdateColours()
        {
            switch (Good.Item.ItemType)
            {
                case ItemType.Consumable:
                case ItemType.Scroll:
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Torch:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.DarkStone:
                    RequirementLabel.ForeColour = GameScene.Game.CanUseItem(ItemCell.Item) ? Color.Aquamarine : Color.Red;
                    break;
            }
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            GameScene.Game.MouseItem = ItemCell.Item;
        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            GameScene.Game.MouseItem = null;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Good = null;
                GoodChanged = null;

                _Selected = false;
                SelectedChanged = null;

                _Currency = null;
                CurrencyChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (CurrencyIcon != null)
                {
                    if (!CurrencyIcon.IsDisposed)
                        CurrencyIcon.Dispose();

                    CurrencyIcon = null;
                }

                if (ItemNameLabel != null)
                {
                    if (!ItemNameLabel.IsDisposed)
                        ItemNameLabel.Dispose();

                    ItemNameLabel = null;
                }

                if (RequirementLabel != null)
                {
                    if (!RequirementLabel.IsDisposed)
                        RequirementLabel.Dispose();

                    RequirementLabel = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCRepairDialog : DXWindow
    {
        #region Properties
        public DXItemGrid Grid;

        public DXLabel GoldLabel;
        public DXButton RepairButton, GuildStorageButton;
        public DXCheckBox SpecialCheckBox;
        public DXCheckBox GuildCheckBox;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
            {
                GameScene.Game.InventoryBox.Visible = true;
                GameScene.Game.CharacterBox.Visible = true;
                GameScene.Game.StorageBox.Visible = true;
            }

            if (!IsVisible)
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCRepairDialog()
        {
            TitleLabel.Text = "Repair Items";
            Movable = false;

            Grid = new DXItemGrid
            {
                GridSize = new Size(11, 5),
                Parent = this,
                GridType = GridType.Repair,
                Linked = true
            };

            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 70));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();


            GoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 65),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 65),
                Text = "Repair Cost:",
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton inventory = new DXButton
            {
                Label = { Text = "Inventory" },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            inventory.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            DXButton equipment = new DXButton
            {
                Label = { Text = "Equipment" },
                Location = new Point(ClientArea.X + 5 + inventory.Size.Width, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            equipment.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.CharacterBox.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };


            DXButton storage = new DXButton
            {
                Label = { Text = "Storage" },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + inventory.Size.Height + 5 + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
            };
            storage.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.StorageBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            GuildStorageButton = new DXButton
            {
                Label = { Text = "Guild Storage" },
                Location = new Point(ClientArea.X + inventory.Size.Width + 5, GoldLabel.Location.Y + GoldLabel.Size.Height + inventory.Size.Height + 5 + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            GuildStorageButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.GuildBox.GuildInfo == null) return;

                foreach (DXItemCell cell in GameScene.Game.GuildBox.StorageGrid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };


            SpecialCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Special Repair" },
                Checked = Config.SpecialRepair,
            };
            SpecialCheckBox.Location = new Point(ClientArea.Right - 80 - SpecialCheckBox.Size.Width - 5, GoldLabel.Location.Y + GoldLabel.Size.Height + 7);
            SpecialCheckBox.CheckedChanged += (o, e) =>
            {
                Config.SpecialRepair = SpecialCheckBox.Checked;

                if (SpecialCheckBox.Checked)
                    foreach (DXItemCell cell in Grid.Grid)
                    {
                        if (cell.Item == null) continue;
                        if (CEnvir.Now > cell.Item.NextSpecialRepair) continue;


                        cell.Link = null;
                    }

                CalculateCost();
            };


            GuildCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Use Guild Funds" },
                Enabled = false,
            };
            GuildCheckBox.Location = new Point(ClientArea.Right - 80 - GuildCheckBox.Size.Width - 5, GoldLabel.Location.Y + GoldLabel.Size.Height + SpecialCheckBox.Size.Height + 5 + 7);
            GuildCheckBox.CheckedChanged += (o, e) => CalculateCost();


            RepairButton = new DXButton
            {
                Label = { Text = "Repair" },
                Location = new Point(ClientArea.Right - 80, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            RepairButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCRepair { Links = links, Special = SpecialCheckBox.Checked, GuildFunds = GuildCheckBox.Checked });

                GuildCheckBox.Checked = false;
            };
        }

        #region Methods
        private void CalculateCost()
        {
            int sum = 0;

            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                sum += cell.Link.Item.RepairCost(SpecialCheckBox.Checked);
                count++;
            }

            if (GuildCheckBox.Checked)
            {
                GoldLabel.ForeColour = sum > GameScene.Game.GuildBox.GuildInfo.GuildFunds ? Color.Red : Color.White;
            }
            else
            {
                GoldLabel.ForeColour = sum > MapObject.User.Gold.Amount ? Color.Red : Color.White;
            }

            GoldLabel.Text = sum.ToString("#,##0");

            RepairButton.Enabled = sum <= MapObject.User.Gold.Amount && count > 0;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (RepairButton != null)
                {
                    if (!RepairButton.IsDisposed)
                        RepairButton.Dispose();

                    RepairButton = null;
                }

                if (GuildStorageButton != null)
                {
                    if (!GuildStorageButton.IsDisposed)
                        GuildStorageButton.Dispose();

                    GuildStorageButton = null;
                }
                
                if (SpecialCheckBox != null)
                {
                    if (!SpecialCheckBox.IsDisposed)
                        SpecialCheckBox.Dispose();

                    SpecialCheckBox = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }

            }

        }

        #endregion
    }

    public sealed class NPCRefineDialog : DXWindow
    {
        #region Properties

        #region RefineType

        public RefineType RefineType
        {
            get => _RefineType;
            set
            {
                if (_RefineType == value) return;

                RefineType oldValue = _RefineType;
                _RefineType = value;

                OnRefineTypeChanged(oldValue, value);
            }
        }
        private RefineType _RefineType;
        public event EventHandler<EventArgs> RefineTypeChanged;
        public void OnRefineTypeChanged(RefineType oValue, RefineType nValue)
        {
            switch (oValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = true;
                    break;
                case RefineType.Durability:
                    DurabilityCheckBox.Checked = false;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = false;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = false;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = false;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = false;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = false;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = false;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = false;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = false;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = false;
                    break;
            }

            switch (nValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = false;
                    break;
                case RefineType.Durability:
                    DurabilityCheckBox.Checked = true;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = true;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = true;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = true;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = true;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = true;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = true;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = true;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = true;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = true;
                    break;
            }

            RefineTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region RefineQuality

        public RefineQuality RefineQuality
        {
            get => _RefineQuality;
            set
            {
                if (_RefineQuality == value) return;

                RefineQuality oldValue = _RefineQuality;
                _RefineQuality = value;

                OnRefineQualityChanged(oldValue, value);
            }
        }
        private RefineQuality _RefineQuality;
        public event EventHandler<EventArgs> RefineQualityChanged;
        public void OnRefineQualityChanged(RefineQuality oValue, RefineQuality nValue)
        {
            switch (nValue)
            {
                case RefineQuality.Rush:
                    DurationLabel.Text = "";
                    break;
                default:
                    DurationLabel.Text = Functions.ToString(Globals.RefineTimes[nValue], false);
                    break;
            }

            RefineQualityChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        public DXItemGrid BlackIronGrid, AccessoryGrid, SpecialGrid;

        public DXCheckBox DurabilityCheckBox, DCCheckBox, SPCheckBox, FireCheckBox, IceCheckBox, LightningCheckBox, WindCheckBox, HolyCheckBox, DarkCheckBox, PhantomCheckBox;
        public DXButton SubmitButton;

        public DXComboBox RefineQualityBox;
        public DXLabel DurationLabel;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                BlackIronGrid.ClearLinks();
                AccessoryGrid.ClearLinks();
                SpecialGrid.ClearLinks();
            }
        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCRefineDialog()
        {
            TitleLabel.Text = "Refine";



            SetClientSize(new Size(491, 130));

            DXLabel label = new DXLabel
            {
                Text = "Black Iron Ore",
                Location = ClientArea.Location,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            BlackIronGrid = new DXItemGrid
            {
                GridSize = new Size(5, 1),
                Parent = this,
                GridType = GridType.RefineBlackIronOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Accessories",
                Location = new Point(label.Location.X, BlackIronGrid.Location.Y + BlackIronGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            AccessoryGrid =  new DXItemGrid
            {
                GridSize = new Size(3, 1),
                Parent = this,
                GridType = GridType.RefineAccessory,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Special",
                Location = new Point(AccessoryGrid.Location.X + AccessoryGrid.Size.Width + DXItemCell.CellWidth - 7, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            SpecialGrid =  new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.RefineSpecial,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };


            SetClientSize(new Size(491, SpecialGrid.Location.Y + SpecialGrid.Size.Height - ClientArea.Y + 2));
            
            DCCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "DC" },
                ReadOnly = true,
            };
            DCCheckBox.MouseClick += (o, e) => RefineType = RefineType.DC;
            SPCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Spell Power" },
                ReadOnly = true,
            };
            SPCheckBox.MouseClick += (o, e) => RefineType = RefineType.SpellPower;

            FireCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Fire" },
                ReadOnly = true,
            };
            FireCheckBox.MouseClick += (o, e) => RefineType = RefineType.Fire;

            IceCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Ice" },
                ReadOnly = true,
            };
            IceCheckBox.MouseClick += (o, e) => RefineType = RefineType.Ice;

            LightningCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Lightning" },
                ReadOnly = true,
            };
            LightningCheckBox.MouseClick += (o, e) => RefineType = RefineType.Lightning;

            WindCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Wind" },
                ReadOnly = true,
            };
            WindCheckBox.MouseClick += (o, e) => RefineType = RefineType.Wind;

            HolyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Holy" },
                ReadOnly = true,
            };
            HolyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Holy;

            DarkCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Dark" },
                ReadOnly = true,
            };
            DarkCheckBox.MouseClick += (o, e) => RefineType = RefineType.Dark;


            PhantomCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Phantom" },
                ReadOnly = true,
            };
            PhantomCheckBox.MouseClick += (o, e) => RefineType = RefineType.Phantom;


            DCCheckBox.Location = new Point(ClientArea.Right - DCCheckBox.Size.Width - 240, ClientArea.Y + 50);
            SPCheckBox.Location = new Point(ClientArea.Right - SPCheckBox.Size.Width - 156, ClientArea.Y + 50);

            FireCheckBox.Location = new Point(ClientArea.Right - FireCheckBox.Size.Width - 240, ClientArea.Y + 73);
            IceCheckBox.Location = new Point(ClientArea.Right - IceCheckBox.Size.Width - 156, ClientArea.Y + 73);
            LightningCheckBox.Location = new Point(ClientArea.Right - LightningCheckBox.Size.Width - 81, ClientArea.Y + 73);
            WindCheckBox.Location = new Point(ClientArea.Right - WindCheckBox.Size.Width - 5, ClientArea.Y + 73);
            HolyCheckBox.Location = new Point(ClientArea.Right - HolyCheckBox.Size.Width - 240, ClientArea.Y + 90);
            DarkCheckBox.Location = new Point(ClientArea.Right - DarkCheckBox.Size.Width - 156, ClientArea.Y + 90);
            PhantomCheckBox.Location = new Point(ClientArea.Right - PhantomCheckBox.Size.Width - 240, ClientArea.Y + 107);

            SubmitButton = new DXButton
            {
                Label = { Text = "Submit" },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            SubmitButton.Location = new Point(ClientArea.Right - SubmitButton.Size.Width, ClientArea.Bottom - SubmitButton.Size.Height);
            SubmitButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> ores = new List<CellLinkInfo>();
                List<CellLinkInfo> items = new List<CellLinkInfo>();
                List<CellLinkInfo> specials = new List<CellLinkInfo>();

                foreach (DXItemCell cell in BlackIronGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    ores.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in AccessoryGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    items.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in SpecialGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    specials.Add(new CellLinkInfo { Count = 1, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCRefine { RefineType = RefineType, RefineQuality = RefineQuality, Ores = ores, Items = items, Specials = specials });

            };

            RefineQualityBox = new DXComboBox
            {
                Parent = this,
                Size = new Size(80, DXComboBox.DefaultNormalHeight),
            };
            RefineQualityBox.SelectedItemChanged += (o, e) => RefineQuality = (RefineQuality?) RefineQualityBox.SelectedItem ?? RefineQuality.Quick;
            RefineQualityBox.Location = new Point(ClientArea.Right - RefineQualityBox.Size.Width - 160, BlackIronGrid.Location.Y);


            foreach (KeyValuePair<RefineQuality, TimeSpan> pair in Globals.RefineTimes)
                new DXListBoxItem
                {
                    Parent = RefineQualityBox.ListBox,
                    Label = { Text = $"{pair.Key}" },
                    Item = pair.Key
                };

            label = new DXLabel
            {
                Parent = this,
                Text = "Quality:",
            };
            label.Location = new Point(RefineQualityBox.Location.X - label.Size.Width - 5, RefineQualityBox.Location.Y + (RefineQualityBox.Size.Height - label.Size.Height)/2);


            DurationLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(RefineQualityBox.Location.X  + RefineQualityBox.Size.Width  + 5, RefineQualityBox.Location.Y + (RefineQualityBox.Size.Height - label.Size.Height) / 2)
            };

            RefineQualityBox.ListBox.SelectItem(RefineQuality.Quick);
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _RefineType = 0;
                RefineTypeChanged = null;

                _RefineQuality = 0;
                RefineQualityChanged = null;

                if (BlackIronGrid != null)
                {
                    if (!BlackIronGrid.IsDisposed)
                        BlackIronGrid.Dispose();

                    BlackIronGrid = null;
                }

                if (AccessoryGrid != null)
                {
                    if (!AccessoryGrid.IsDisposed)
                        AccessoryGrid.Dispose();

                    AccessoryGrid = null;
                }

                if (SpecialGrid != null)
                {
                    if (!SpecialGrid.IsDisposed)
                        SpecialGrid.Dispose();

                    SpecialGrid = null;
                }

                if (DurabilityCheckBox != null)
                {
                    if (!DurabilityCheckBox.IsDisposed)
                        DurabilityCheckBox.Dispose();

                    DurabilityCheckBox = null;
                }

                if (DCCheckBox != null)
                {
                    if (!DCCheckBox.IsDisposed)
                        DCCheckBox.Dispose();

                    DCCheckBox = null;
                }

                if (SPCheckBox != null)
                {
                    if (!SPCheckBox.IsDisposed)
                        SPCheckBox.Dispose();

                    SPCheckBox = null;
                }

                if (FireCheckBox != null)
                {
                    if (!FireCheckBox.IsDisposed)
                        FireCheckBox.Dispose();

                    FireCheckBox = null;
                }

                if (IceCheckBox != null)
                {
                    if (!IceCheckBox.IsDisposed)
                        IceCheckBox.Dispose();

                    IceCheckBox = null;
                }

                if (LightningCheckBox != null)
                {
                    if (!LightningCheckBox.IsDisposed)
                        LightningCheckBox.Dispose();

                    LightningCheckBox = null;
                }

                if (WindCheckBox != null)
                {
                    if (!WindCheckBox.IsDisposed)
                        WindCheckBox.Dispose();

                    WindCheckBox = null;
                }

                if (HolyCheckBox != null)
                {
                    if (!HolyCheckBox.IsDisposed)
                        HolyCheckBox.Dispose();

                    HolyCheckBox = null;
                }

                if (DarkCheckBox != null)
                {
                    if (!DarkCheckBox.IsDisposed)
                        DarkCheckBox.Dispose();

                    DarkCheckBox = null;
                }

                if (PhantomCheckBox != null)
                {
                    if (!PhantomCheckBox.IsDisposed)
                        PhantomCheckBox.Dispose();

                    PhantomCheckBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }

                if (RefineQualityBox != null)
                {
                    if (!RefineQualityBox.IsDisposed)
                        RefineQualityBox.Dispose();

                    RefineQualityBox = null;
                }

                if (DurationLabel != null)
                {
                    if (!DurationLabel.IsDisposed)
                        DurationLabel.Dispose();

                    DurationLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCRefineRetrieveDialog : DXWindow
    {
        #region Properties

        #region SelectedCell

        public NPCRefineCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                if (_SelectedCell == value) return;

                NPCRefineCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private NPCRefineCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(NPCRefineCell oValue, NPCRefineCell nValue)
        {
            if (oValue != null) oValue.Selected = false;
            if (nValue != null) nValue.Selected = true;

            RetrieveButton.Enabled = nValue != null;

            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public List<ClientRefineInfo> Refines = new List<ClientRefineInfo>();
        private DXVScrollBar ScrollBar;

        public List<NPCRefineCell> Cells = new List<NPCRefineCell>();
        private DXButton RetrieveButton;
        public DXControl ClientPanel;

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (ClientPanel == null) return;

            ClientPanel.Size = ClientArea.Size;
            ClientPanel.Location = ClientArea.Location;

        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCRefineRetrieveDialog()
        {
            TitleLabel.Text = "Refines";

            HasFooter = true;
            Movable = false;


            SetClientSize(new Size(491, 302));

            ClientPanel = new DXControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
                PassThrough = true,
            };

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(14, ClientArea.Height - 1),
            };
            ScrollBar.Location = new Point(ClientArea.Right - ScrollBar.Size.Width - 2, ClientArea.Y + 1);
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            MouseWheel += ScrollBar.DoMouseWheel;

            RetrieveButton = new DXButton
            {
                Location = new Point((Size.Width - 80)/2, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Retrieve" },
                Enabled = false,
            };
            RetrieveButton.MouseClick += (o, e) => Retrieve();
        }

        #region Methods
        public void RefreshList()
        {
            foreach (NPCRefineCell cell in Cells)
                cell.Dispose();

            Cells.Clear();

            foreach (ClientRefineInfo refine in Refines)
            {
                NPCRefineCell cell;
                Cells.Add(cell = new NPCRefineCell
                {
                    Parent = ClientPanel,
                    Refine = refine
                });
                cell.MouseClick += (o, e) => SelectedCell = cell;
                cell.MouseWheel += ScrollBar.DoMouseWheel;
                cell.MouseDoubleClick += (o, e) => Retrieve();
            }


            ScrollBar.MaxValue = Refines.Count*43 - 2;
            SetClientSize(new Size(ClientArea.Width, Math.Min(Math.Max(3 * 43 - 2, ScrollBar.MaxValue), 7*43 - 3) + 1));
            ScrollBar.VisibleSize = ClientArea.Height;
            ScrollBar.Size = new Size(ScrollBar.Size.Width, ClientArea.Height - 2);

            RetrieveButton.Location = new Point((Size.Width - 80)/2, Size.Height - 43);
            ScrollBar.Value = 0;
            UpdateLocations();
        }
        private void UpdateLocations()
        {
            int y = -ScrollBar.Value + 1;

            foreach (NPCRefineCell cell in Cells)
            {
                cell.Location = new Point(1, y);

                y += cell.Size.Height + 3;
            }
        }

        public void Retrieve()
        {
            if (GameScene.Game.Observer) return;
            if (SelectedCell == null) return;

            CEnvir.Enqueue(new C.NPCRefineRetrieve { Index = SelectedCell.Refine.Index });

        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedCell = null;
                SelectedCellChanged = null;

                Refines.Clear();
                Refines = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (Cells != null)
                {
                    for (int i = 0; i < Cells.Count; i++)
                    {
                        if (Cells[i] != null)
                        {
                            if (!Cells[i].IsDisposed)
                                Cells[i].Dispose();

                            Cells[i] = null;
                        }

                    }
                    Cells.Clear();
                    Cells = null;
                }

                if (RetrieveButton != null)
                {
                    if (!RetrieveButton.IsDisposed)
                        RetrieveButton.Dispose();

                    RetrieveButton = null;
                }

                if (ClientPanel != null)
                {
                    if (!ClientPanel.IsDisposed)
                        ClientPanel.Dispose();

                    ClientPanel = null;
                }

            }

        }

        #endregion
    }

    public sealed class NPCRefineCell : DXControl
    {
        #region Properties

        #region Refine

        public ClientRefineInfo Refine
        {
            get => _Refine;
            set
            {
                if (_Refine == value) return;

                ClientRefineInfo oldValue = _Refine;
                _Refine = value;

                OnRefineChanged(oldValue, value);
            }
        }
        private ClientRefineInfo _Refine;
        public event EventHandler<EventArgs> RefineChanged;
        public void OnRefineChanged(ClientRefineInfo oValue, ClientRefineInfo nValue)
        {
            ItemCell.Item = Refine.Weapon;
            ItemNameLabel.Text = Refine.Weapon.Info.ItemName;

            switch (Refine.Type)
            {
                case RefineType.Durability:
                    RefineTypeLabel.Text = "Durability";
                    break;
                case RefineType.DC:
                    RefineTypeLabel.Text = "DC";
                    break;
                case RefineType.SpellPower:
                    RefineTypeLabel.Text = "Spell Power";
                    break;
                case RefineType.Fire:
                    RefineTypeLabel.Text = "Fire Element";
                    break;
                case RefineType.Ice:
                    RefineTypeLabel.Text = "Ice Element";
                    break;
                case RefineType.Lightning:
                    RefineTypeLabel.Text = "Lightning Element";
                    break;
                case RefineType.Wind:
                    RefineTypeLabel.Text = "Wind Element";
                    break;
                case RefineType.Holy:
                    RefineTypeLabel.Text = "Holy Element";
                    break;
                case RefineType.Dark:
                    RefineTypeLabel.Text = "Dark Element";
                    break;
                case RefineType.Phantom:
                    RefineTypeLabel.Text = "Phantom Element";
                    break;
                case RefineType.Reset:
                    RefineTypeLabel.Text = "Reset";
                    break;
            }

            MaxChanceLabel.Text = $"{Refine.MaxChance}%";
            ChanceLabel.Text = $"{Refine.Chance}%";

            if (CEnvir.Now > Refine.RetrieveTime)
            {
                RetrieveTimeLabel.Text = "Complete";
                RetrieveTimeLabel.ForeColour = Color.LightSeaGreen;
            }
            else
            {
                RetrieveTimeLabel.Text = Functions.ToString(Refine.RetrieveTime - CEnvir.Now, true, true);

                RetrieveTimeLabel.ProcessAction = () =>
                {
                    if (Refine == null || CEnvir.Now > Refine.RetrieveTime)
                    {
                        RetrieveTimeLabel.Text = "Complete";
                        RetrieveTimeLabel.ForeColour = Color.LightSeaGreen;
                        RetrieveTimeLabel.ProcessAction = null;
                        return;
                    }

                    RetrieveTimeLabel.Text = Functions.ToString(Refine.RetrieveTime - CEnvir.Now, true, true);
                    RetrieveTimeLabel.ForeColour = Color.White;
                };
            }

            RefineChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell ItemCell;
        
        public DXLabel ItemNameLabel, RefineTypeLabel, ChanceLabel, MaxChanceLabel, RetrieveTimeLabel;

        #endregion

        public NPCRefineCell()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(483, 40);


            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, (Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };
            ItemNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X * 2 + ItemCell.Size.Width, ItemCell.Location.Y),
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            RefineTypeLabel = new DXLabel
            {
                Parent = this,
                Text = "Refine Type:",
                IsControl = false,
            };
            RefineTypeLabel.Location = new Point(ItemCell.Location.X * 2 + ItemCell.Size.Width, ItemCell.Location.Y + ItemCell.Size.Height - RefineTypeLabel.Size.Height);


            RefineTypeLabel = new DXLabel
            {
                Parent = this,
                Text = "None",
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(RefineTypeLabel.Location.X  + RefineTypeLabel.Size.Width, RefineTypeLabel.Location.Y)
            };

            ChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "Success Chance:",
                IsControl = false,
            };
            ChanceLabel.Location = new Point(300 - ChanceLabel.Size.Width, ItemNameLabel.Location.Y );

            ChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "0%",
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(ChanceLabel.Location.X + ChanceLabel.Size.Width, ChanceLabel.Location.Y)
            };

            MaxChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "Maximum Chance:",
                IsControl = false,
            };
            MaxChanceLabel.Location = new Point(300 - MaxChanceLabel.Size.Width, RefineTypeLabel.Location.Y);

            MaxChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "0%",
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(MaxChanceLabel.Location.X + MaxChanceLabel.Size.Width, MaxChanceLabel.Location.Y)
            };
            

            RetrieveTimeLabel = new DXLabel
            {
                Parent = this,
                Text = "Time Left:",
                IsControl = false,
            };
            RetrieveTimeLabel.Location = new Point(390 - RetrieveTimeLabel.Size.Width, RefineTypeLabel.Location.Y);

            RetrieveTimeLabel = new DXLabel
            {
                Parent = this,
                Text = "0 Seconds",
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(RetrieveTimeLabel.Location.X + RetrieveTimeLabel.Size.Width, RetrieveTimeLabel.Location.Y)
            };

        }

        #region Methods
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            GameScene.Game.MouseItem = ItemCell.Item;
        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            GameScene.Game.MouseItem = null;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Refine = null;
                RefineChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }
                
                if (ItemNameLabel != null)
                {
                    if (!ItemNameLabel.IsDisposed)
                        ItemNameLabel.Dispose();

                    ItemNameLabel = null;
                }

                if (RefineTypeLabel != null)
                {
                    if (!RefineTypeLabel.IsDisposed)
                        RefineTypeLabel.Dispose();

                    RefineTypeLabel = null;
                }

                if (ChanceLabel != null)
                {
                    if (!ChanceLabel.IsDisposed)
                        ChanceLabel.Dispose();

                    ChanceLabel = null;
                }

                if (MaxChanceLabel != null)
                {
                    if (!MaxChanceLabel.IsDisposed)
                        MaxChanceLabel.Dispose();

                    MaxChanceLabel = null;
                }

                if (RetrieveTimeLabel != null)
                {
                    if (!RetrieveTimeLabel.IsDisposed)
                        RetrieveTimeLabel.Dispose();

                    RetrieveTimeLabel = null;
                }

            }

        }

        #endregion
    }

    public sealed class NPCQuestListDialog : DXImageControl
    {
        public NPCQuestRow[] Rows;

        public DXLabel TitleLabel, AcceptableLabel, CountLabel;

        public List<QuestInfo> Quests = new List<QuestInfo>();

        public DXVScrollBar ScrollBar;

        #region NPCInfo

        public NPCInfo NPCInfo
        {
            get => _NPCInfo;
            set
            {
                if (_NPCInfo == value) return;

                NPCInfo oldValue = _NPCInfo;
                _NPCInfo = value;

                OnNPCInfoChanged(oldValue, value);
            }
        }
        private NPCInfo _NPCInfo;
        public event EventHandler<EventArgs> NPCInfoChanged;
        public void OnNPCInfoChanged(NPCInfo oValue, NPCInfo nValue)
        {
            NPCInfoChanged?.Invoke(this, EventArgs.Empty);

            if (nValue != null && oValue != null && oValue != nValue)
            {
                SelectedQuest = null;
            }

            UpdateQuestDisplay();
        }

        #endregion

        #region SelectedQuest

        public NPCQuestRow SelectedQuest
        {
            get => _SelectedQuest;
            set
            {
                if (_SelectedQuest == value) return;

                NPCQuestRow oldValue = _SelectedQuest;
                _SelectedQuest = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private NPCQuestRow _SelectedQuest;
        public event EventHandler<EventArgs> SelectedQuestChanged;
        public void OnSelectedQuestChanged(NPCQuestRow oValue, NPCQuestRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (SelectedQuest != null)
            {
                SelectedQuest.Selected = true;
            }

            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);

            GameScene.Game.NPCQuestBox.NPCInfo = NPCInfo;
            GameScene.Game.NPCQuestBox.SelectedQuest = SelectedQuest;
            GameScene.Game.NPCQuestBox.Visible = GameScene.Game.NPCQuestBox.SelectedQuest != null;
        }

        #endregion

        #region IsVisible
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.NPCQuestBox != null && !IsVisible)
            {
                GameScene.Game.NPCQuestBox.Visible = false;
                NPCInfo = null;
            }

            if (IsVisible)
                BringToFront();
        }
        #endregion

        public NPCQuestListDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 209;
            Movable = false;
            Sort = true;
            Location = new Point(0, GameScene.Game.NPCBox.Size.Height);

            TitleLabel = new DXLabel
            {
                Text = "Quest List",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            AcceptableLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(170, 15),
                Parent = this,
                Outline = true,
                IsControl = false,
                DrawFormat = TextFormatFlags.HorizontalCenter
            };
            AcceptableLabel.Location = new Point(15, 185);

            CountLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(50, 15),
                Parent = this,
                Outline = true,
                IsControl = false,
                DrawFormat = TextFormatFlags.HorizontalCenter
            };
            CountLabel.Location = new Point(205, 185);

            Rows = new NPCQuestRow[6];

            DXControl panel = new DXControl
            {
                Size = new Size(365, 2 + Rows.Length * 22),
                Location = new Point(8, 37),
                Parent = this,
                DrawTexture = true
            };

            ScrollBar = new DXVScrollBar
            {
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                Parent = panel,
                Location = new Point(panel.Size.Width - 20, 0),
                Size = new Size(22, Rows.Length * 22 + 5),
                VisibleSize = Rows.Length,
                Change = 1,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new NPCQuestRow
                {
                    Parent = panel,
                    Location = new Point(2, 2 + i * 22),
                    Size = new Size(340, 20)
                };
                int index = i;
                Rows[index].MouseClick += (o, e) =>
                {
                    if (Rows[index].QuestInfo == null) return;

                    SelectedQuest = Rows[index];
                };
                Rows[index].MouseWheel += ScrollBar.DoMouseWheel;
            }
        }

        public void UpdateQuestDisplay()
        {
            if (NPCInfo == null)
            {
                Visible = false;
                return;
            }

            Quests.Clear();

            List<QuestInfo> availableQuests = new List<QuestInfo>(), currentQuests = new List<QuestInfo>(), completeQuests = new List<QuestInfo>();

            foreach (QuestInfo quest in NPCInfo.StartQuests)
            {
                if (!GameScene.Game.CanAccept(quest)) continue;

                availableQuests.Add(quest);
            }

            foreach (QuestInfo quest in NPCInfo.FinishQuests)
            {
                ClientUserQuest userQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == quest);

                if (userQuest == null || userQuest.Completed) continue;

                if (!userQuest.IsComplete)
                    currentQuests.Add(quest);
                else
                    completeQuests.Add(quest);
            }

            completeQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));
            availableQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));
            currentQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));

            Quests.AddRange(completeQuests);
            Quests.AddRange(availableQuests);
            Quests.AddRange(currentQuests);

            Visible = Quests.Count > 0;

            if (Quests.Count == 0) return;

            QuestInfo previousQuest = SelectedQuest?.QuestInfo;

            SelectedQuest = null;

            UpdateScrollBar();

            if (previousQuest != null)
            {
                foreach (NPCQuestRow row in Rows)
                {
                    if (row.QuestInfo != previousQuest) continue;

                    SelectedQuest = row;
                    break;
                }
            }

            AcceptableLabel.Text = "Acceptable Quests";
            CountLabel.Text = Quests.Count.ToString();

            //if (SelectedQuest == null)
            //    SelectedQuest = Rows[0];
        }

        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Quests.Count;

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i].QuestInfo = i + ScrollBar.Value >= Quests.Count ? null : Quests[i + ScrollBar.Value];
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _NPCInfo = null;
                NPCInfoChanged = null;

                Quests.Clear();
                Quests = null;

                _SelectedQuest = null;
                SelectedQuestChanged = null;

                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }

                    }

                    Rows = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }
            }
        }

        #endregion
    }

    public sealed class NPCQuestDialog : DXImageControl
    {
        #region Properties

        #region NPCInfo

        public NPCInfo NPCInfo
        {
            get => _NPCInfo;
            set
            {
                if (_NPCInfo == value) return;

                NPCInfo oldValue = _NPCInfo;
                _NPCInfo = value;

                OnNPCInfoChanged(oldValue, value);
            }
        }
        private NPCInfo _NPCInfo;
        public event EventHandler<EventArgs> NPCInfoChanged;
        public void OnNPCInfoChanged(NPCInfo oValue, NPCInfo nValue)
        {
            NPCInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedQuest

        public NPCQuestRow SelectedQuest
        {
            get => _SelectedQuest;
            set
            {
                if (_SelectedQuest == value) return;

                NPCQuestRow oldValue = _SelectedQuest;
                _SelectedQuest = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private NPCQuestRow _SelectedQuest;
        public event EventHandler<EventArgs> SelectedQuestChanged;
        public void OnSelectedQuestChanged(NPCQuestRow oValue, NPCQuestRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            foreach (DXItemCell cell in RewardGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            SelectedCell = null;

            if (SelectedQuest?.QuestInfo == null)
            {
                QuestLabel.Text = string.Empty;
                TasksLabel.Text = string.Empty;
                DescriptionLabel.Text = string.Empty;

                AcceptButton.Visible = false;
                CompleteButton.Visible = false;
                StartLabel.Text = string.Empty;
                EndLabel.Text = string.Empty;
                return;
            }

            SelectedQuest.Selected = true;
            
            int standard = 0, choice = 0;
            HasChoice = false;

            foreach (QuestReward reward in SelectedQuest.QuestInfo.Rewards)
            {
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                UserItemFlags flags = UserItemFlags.None;
                TimeSpan duration = TimeSpan.FromSeconds(reward.Duration);

                if (reward.Bound)
                    flags |= UserItemFlags.Bound;

                if (duration != TimeSpan.Zero)
                    flags |= UserItemFlags.Expirable;

                ClientUserItem item = new ClientUserItem(reward.Item, reward.Amount)
                {
                    Flags = flags,
                    ExpireTime = duration
                };

                if (reward.Choice)
                {
                    if (choice >= ChoiceGrid.Grid.Length) continue;
                    
                    HasChoice = true;

                    ChoiceGrid.Grid[choice].Item = item;
                    ChoiceGrid.Grid[choice].Tag = reward;
                    choice++;
                }
                else
                {
                    if (standard >= RewardGrid.Grid.Length) continue;

                    RewardGrid.Grid[standard].Item = item;
                    RewardGrid.Grid[standard].Tag = reward;
                    standard++;
                }
            }

            if (HasChoice)
                SelectedCell = null;

            QuestLabel.Text = SelectedQuest.QuestInfo.QuestName;
            DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, false);
            TasksLabel.Text = GameScene.Game.GetTaskText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            int height = DXLabel.GetHeight(DescriptionLabel, DescriptionLabel.Size.Width).Height;

            DescriptionLabel.Size = new Size(DescriptionContainer.Size.Width, height);
            DescriptionScrollBar.MaxValue = DescriptionLabel.Size.Height - DescriptionContainer.Size.Height + 14;

            EndLabel.Text = SelectedQuest.QuestInfo.FinishNPC.RegionName;
            StartLabel.Text = SelectedQuest.QuestInfo.StartNPC.RegionName;

            AcceptButton.Visible = SelectedQuest.UserQuest == null;
            CompleteButton.Visible = SelectedQuest.UserQuest != null && SelectedQuest.UserQuest.IsComplete;

            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedCell

        public DXItemCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                DXItemCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private DXItemCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(DXItemCell oValue, DXItemCell nValue)
        {
            if (oValue != null)
            {
                oValue.FixedBorder = false;
                oValue.Border = false;
                oValue.FixedBorderColour = false;
                oValue.BorderColour = Color.Lime;
            }

            if (nValue != null)
            {
                nValue.Border = true;
                nValue.FixedBorder = true;
                nValue.FixedBorderColour = true;
                nValue.BorderColour = Color.Lime;
            }
            
            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region IsVisible
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
                BringToFront();
        }
        #endregion

        public List<QuestInfo> Quests = new List<QuestInfo>();

        public DXButton CloseButton;
        public DXLabel TitleLabel;

        public DXControl DescriptionContainer;
        public DXLabel QuestLabel, TasksLabel, DescriptionLabel, EndLabel, StartLabel;
        public DXVScrollBar DescriptionScrollBar;

        public DXItemGrid RewardGrid, ChoiceGrid;

        public DXButton AcceptButton, CompleteButton;

        public ClientUserItem[] RewardArray, ChoiceArray;

        public bool HasChoice;

        #endregion

        public NPCQuestDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 212;
            Movable = false;
            Sort = true;
            Location = new Point(GameScene.Game.NPCBox.Size.Width, 0);

            TitleLabel = new DXLabel
            {
                Text = "Quest",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(360 - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            int width = 10;

            QuestLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(width, 40)
            };

            var label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabDetailsLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width, 65)
            };

            DescriptionContainer = new DXControl
            {
                Parent = this,
                Location = new Point(width + 3, label.Location.Y + label.Size.Height + 5),
                Size = new Size(313, 81),
            };

            DescriptionLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(313, 81),
                Border = false,
                ForeColour = Color.White,
                Location = new Point(0, 0),
                Parent = DescriptionContainer,
                DrawFormat = TextFormatFlags.WordBreak
            };

            DescriptionScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(20, 115),
                Location = new Point(Size.Width - 29, 62),
                VisibleSize = 6,
                Change = 1,
                Border = false,
                BackColour = Color.Empty,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
            };
            DescriptionScrollBar.ValueChanged += DescriptionScrollBar_ValueChanged;
            DescriptionLabel.MouseWheel += DescriptionScrollBar.DoMouseWheel;

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabTasksLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, DescriptionContainer.Location.Y + DescriptionContainer.Size.Height + 9),
            };

            TasksLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(334, 61),
                ForeColour = Color.White,
                Location = new Point(width + 3, label.Location.Y + label.Size.Height + 5),
                Parent = this,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabRewardsLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, TasksLabel.Location.Y + TasksLabel.Size.Height + 24),
            };

            RewardArray = new ClientUserItem[5];
            RewardGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(width + 2, label.Location.Y + label.Size.Height + 5),
                GridSize = new Size(RewardArray.Length, 1),
                ItemGrid = RewardArray,
                ReadOnly = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabChoiceLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(RewardGrid.Location.X + 13 + RewardGrid.Size.Width, TasksLabel.Location.Y + TasksLabel.Size.Height + 24),
            };

            ChoiceArray = new ClientUserItem[4];
            ChoiceGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(RewardGrid.Location.X + 16 + RewardGrid.Size.Width, label.Location.Y + label.Size.Height + 5),
                GridSize = new Size(ChoiceArray.Length, 1),
                ItemGrid = ChoiceArray,
                ReadOnly = true,
            };
            for (int i = 0; i < 4; i++)
            {
                ChoiceGrid.Grid[i].MouseClick += (o, e) => SelectedCell = HasChoice ? (DXItemCell)o : null;
            }

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabStartLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            label.Location = new Point(width + 50 - label.Size.Width, ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 10);

            StartLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(label.Location.X + label.Size.Width - 8, label.Location.Y + (label.Size.Height - 12) / 2),
            };
            StartLabel.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo?.StartNPC?.Region?.Map == null) return;

                GameScene.Game.BigMapBox.Visible = true;
                GameScene.Game.BigMapBox.Opacity = 1F;
                GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo.StartNPC.Region.Map;
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabEndLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, label.Location.Y + label.Size.Height),
            };
            label.Location = new Point(width + 50 - label.Size.Width, ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 10 + label.Size.Height);

            EndLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(label.Location.X + label.Size.Width - 8, label.Location.Y + (label.Size.Height - 12) / 2),
            };
            EndLabel.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo?.FinishNPC?.Region?.Map == null) return;

                GameScene.Game.BigMapBox.Visible = true;
                GameScene.Game.BigMapBox.Opacity = 1F;
                GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo.FinishNPC.Region.Map;
            };

            AcceptButton = new DXButton
            {
                Label = { Text = "Accept" },
                Parent = this,
                Location = new Point(250, label.Location.Y + label.Size.Height + 40),
                Size = new Size(100, DefaultHeight),
                ButtonType = ButtonType.Default,
                Visible = false,
            };
            AcceptButton.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo == null) return;

                CEnvir.Enqueue(new C.QuestAccept { Index = SelectedQuest.QuestInfo.Index });
            };

            CompleteButton = new DXButton
            {
                Label = { Text = "Complete" },
                Parent = this,
                Location = new Point(250, label.Location.Y + label.Size.Height + 40),
                Size = new Size(100, DefaultHeight),
                ButtonType = ButtonType.Default,
                Visible = false,
            };
            CompleteButton.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo == null) return;

                if (HasChoice && SelectedCell == null)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.QuestSelectReward, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.QuestComplete { Index = SelectedQuest.QuestInfo.Index, ChoiceIndex = ((QuestReward) SelectedCell?.Tag)?.Index ?? 0 });
            };
        }

        #region Methods

        private void DescriptionScrollBar_ValueChanged(object sender, EventArgs e)
        {
            int y = -DescriptionScrollBar.Value;

            DescriptionLabel.Location = new Point(0, 0 + y);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _NPCInfo = null;
                NPCInfoChanged = null;

                Quests.Clear();
                Quests = null;

                HasChoice = false;

                _SelectedQuest = null;
                SelectedQuestChanged = null;

                _SelectedCell = null;
                SelectedCellChanged = null;

                if (QuestLabel != null)
                {
                    if (!QuestLabel.IsDisposed)
                        QuestLabel.Dispose();

                    QuestLabel = null;
                }

                if (TasksLabel != null)
                {
                    if (!TasksLabel.IsDisposed)
                        TasksLabel.Dispose();

                    TasksLabel = null;
                }
                
                if (DescriptionContainer != null)
                {
                    if (!DescriptionContainer.IsDisposed)
                        DescriptionContainer.Dispose();

                    DescriptionContainer = null;
                }

                if (DescriptionLabel != null)
                {
                    if (!DescriptionLabel.IsDisposed)
                        DescriptionLabel.Dispose();

                    DescriptionLabel = null;
                }

                if (DescriptionScrollBar != null)
                {
                    if (!DescriptionScrollBar.IsDisposed)
                        DescriptionScrollBar.Dispose();

                    DescriptionScrollBar = null;
                }

                if (EndLabel != null)
                {
                    if (!EndLabel.IsDisposed)
                        EndLabel.Dispose();

                    EndLabel = null;
                }

                if (RewardGrid != null)
                {
                    if (!RewardGrid.IsDisposed)
                        RewardGrid.Dispose();

                    RewardGrid = null;
                }

                if (ChoiceGrid != null)
                {
                    if (!ChoiceGrid.IsDisposed)
                        ChoiceGrid.Dispose();

                    ChoiceGrid = null;
                }

                if (AcceptButton != null)
                {
                    if (!AcceptButton.IsDisposed)
                        AcceptButton.Dispose();

                    AcceptButton = null;
                }

                if (CompleteButton != null)
                {
                    if (!CompleteButton.IsDisposed)
                        CompleteButton.Dispose();

                    CompleteButton = null;
                }

                RewardArray = null;
                ChoiceArray = null;
            }

        }

        #endregion
    }

    public sealed class NPCQuestRow : DXControl
    {
        #region Properties

        #region QuestInfo

        public QuestInfo QuestInfo
        {
            get => _QuestInfo;
            set
            {
                QuestInfo oldValue = _QuestInfo;
                _QuestInfo = value;

                OnQuestInfoChanged(oldValue, value);
            }
        }
        private QuestInfo _QuestInfo;
        public event EventHandler<EventArgs> QuestInfoChanged;
        public void OnQuestInfoChanged(QuestInfo oValue, QuestInfo nValue)
        {
            if (QuestInfo == null)
            {
                Selected = false;
                UserQuest = null;
                QuestNameLabel.Text = string.Empty;
                QuestIcon.Visible = false;
            }
            else
            {
                UserQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == QuestInfo);
                QuestNameLabel.Text = $"{QuestInfo.QuestName}";
                QuestIcon.Visible = true;
            }

            Color colour = Color.White;

            QuestIcon icon = Library.QuestIcon.None;
            QuestType type = QuestType.General;

            if (UserQuest != null)
            {
                type = UserQuest.Quest.QuestType;
                icon = UserQuest.IsComplete ? Library.QuestIcon.Complete : Library.QuestIcon.Incomplete;
            }
            else if (QuestInfo != null)
            {
                type = QuestInfo.QuestType;

                icon = Library.QuestIcon.New;
            }

            int startIndex = 0;

            switch (type)
            {
                case QuestType.General:
                    startIndex = 16;
                    break;
                case QuestType.Daily:
                    startIndex = 76;
                    break;
                case QuestType.Weekly:
                    startIndex = 76;
                    break;
                case QuestType.Repeatable:
                    startIndex = 16;
                    break;
                case QuestType.Story:
                    startIndex = 56;
                    break;
                case QuestType.Account:
                    startIndex = 36;
                    break;
            }

            switch (icon)
            {
                case Library.QuestIcon.New:
                    startIndex += 0;
                    break;
                case Library.QuestIcon.Incomplete:
                    startIndex = 2;
                    break;
                case Library.QuestIcon.Complete:
                    startIndex += 2;
                    break;
            }

            QuestIcon.BaseIndex = startIndex;

            QuestInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region UserQuest

        public ClientUserQuest UserQuest
        {
            get => _UserQuest;
            set
            {
                ClientUserQuest oldValue = _UserQuest;
                _UserQuest = value;

                OnUserQuestChanged(oldValue, value);
            }
        }
        private ClientUserQuest _UserQuest;
        public event EventHandler<EventArgs> UserQuestChanged;
        public void OnUserQuestChanged(ClientUserQuest oValue, ClientUserQuest nValue)
        {
            UserQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXAnimatedControl QuestIcon;
        public DXLabel QuestNameLabel;

        #endregion
        
        public NPCQuestRow()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(280, 20);

            QuestIcon = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(2,2),
                Loop = true,
                LibraryFile = LibraryFile.QuestIcon,
                BaseIndex = 2,
                FrameCount = 2,
                AnimationDelay = TimeSpan.FromSeconds(1),
                Visible = false,
                IsControl = false,
            };

            QuestNameLabel = new DXLabel
            {
                Location = new Point(20, 2),
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _QuestInfo = null;
                QuestInfoChanged = null;

                _UserQuest = null;
                UserQuestChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (QuestIcon != null)
                {
                    if (!QuestIcon.IsDisposed)
                        QuestIcon.Dispose();

                    QuestIcon = null;
                }

                if (QuestNameLabel != null)
                {
                    if (!QuestNameLabel.IsDisposed)
                        QuestNameLabel.Dispose();

                    QuestNameLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCAdoptCompanionDialog : DXWindow
    {
        #region Properties

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;

        public DXLabel NameLabel, IndexLabel, PriceLabel;
        public DXButton LeftButton, RightButton, AdoptButton, UnlockButton;

        public DXTextBox CompanionNameTextBox;

        public List<CompanionInfo> AvailableCompanions = new List<CompanionInfo>();

        #region SelectedCompanionInfo

        public CompanionInfo SelectedCompanionInfo
        {
            get => _SelectedCompanionInfo;
            set
            {
                if (_SelectedCompanionInfo == value) return;

                CompanionInfo oldValue = _SelectedCompanionInfo;
                _SelectedCompanionInfo = value;

                OnSelectedCompanionInfoChanged(oldValue, value);
            }
        }
        private CompanionInfo _SelectedCompanionInfo;
        public event EventHandler<EventArgs> SelectedCompanionInfoChanged;
        public void OnSelectedCompanionInfoChanged(CompanionInfo oValue, CompanionInfo nValue)
        {
            CompanionDisplay = null;

            if (SelectedCompanionInfo?.MonsterInfo == null) return;

            CompanionDisplay = new MonsterObject(SelectedCompanionInfo);

            RefreshUnlockButton();

            PriceLabel.Text = SelectedCompanionInfo.Price.ToString("#,##0");
            NameLabel.Text = SelectedCompanionInfo.MonsterInfo.MonsterName;
            SelectedCompanionInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        
        

        #endregion

        #region SelectedIndex

        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                int oldValue = _SelectedIndex;
                _SelectedIndex = value;

                OnSelectedIndexChanged(oldValue, value);
            }
        }
        private int _SelectedIndex;
        public event EventHandler<EventArgs> SelectedIndexChanged;
        public void OnSelectedIndexChanged(int oValue, int nValue)
        {
            if (SelectedIndex >= Globals.CompanionInfoList.Count) return;

            SelectedCompanionInfo = Globals.CompanionInfoList[SelectedIndex];

            IndexLabel.Text = $"{SelectedIndex + 1} of {Globals.CompanionInfoList.Count}";

            LeftButton.Enabled = SelectedIndex > 0;

            RightButton.Enabled = SelectedIndex < Globals.CompanionInfoList.Count - 1;

            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AdoptAttempted

        public bool AdoptAttempted
        {
            get => _AdoptAttempted;
            set
            {
                if (_AdoptAttempted == value) return;

                bool oldValue = _AdoptAttempted;
                _AdoptAttempted = value;

                OnAdoptAttemptedChanged(oldValue, value);
            }
        }
        private bool _AdoptAttempted;
        public event EventHandler<EventArgs> AdoptAttemptedChanged;
        public void OnAdoptAttemptedChanged(bool oValue, bool nValue)
        {
            RefreshUnlockButton();
            AdoptAttemptedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region CompanionNameValid

        public bool CompanionNameValid
        {
            get => _CompanionNameValid;
            set
            {
                if (_CompanionNameValid == value) return;

                bool oldValue = _CompanionNameValid;
                _CompanionNameValid = value;

                OnCompanionNameValidChanged(oldValue, value);
            }
        }
        private bool _CompanionNameValid;
        public event EventHandler<EventArgs> CompanionNameValidChanged;
        public  void OnCompanionNameValidChanged(bool oValue, bool nValue)
        {
            RefreshUnlockButton();
            CompanionNameValidChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public bool CanAdopt => GameScene.Game.User != null && SelectedCompanionInfo != null && SelectedCompanionInfo.Price <= GameScene.Game.User.Gold.Amount && !AdoptAttempted && !UnlockButton.Visible && CompanionNameValid;


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCAdoptCompanionDialog()
        {
            TitleLabel.Text = "Adopt Companion";
            
            Movable = false;

            SetClientSize(new Size(275, 130));
            CompanionDisplayPoint = new Point(40, 95);

            NameLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            NameLabel.SizeChanged += (o, e) =>
            {
                NameLabel.Location = new Point(CompanionDisplayPoint.X  + 25 - NameLabel.Size.Width / 2, CompanionDisplayPoint.Y + 30);
            };

            IndexLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(CompanionDisplayPoint.X , 200),
            };
            IndexLabel.SizeChanged += (o, e) =>
            {
                IndexLabel.Location = new Point(CompanionDisplayPoint.X  + 25 - IndexLabel.Size.Width / 2, CompanionDisplayPoint.Y + 55);
            };
            LeftButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 32,
                Location = new Point(CompanionDisplayPoint.X - 20, CompanionDisplayPoint.Y + 55)
            };
            LeftButton.MouseClick += (o, e) => SelectedIndex--;
            RightButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 37,
                Location = new Point(CompanionDisplayPoint.X + 60, CompanionDisplayPoint.Y + 55)
            };
            RightButton.MouseClick += (o, e) => SelectedIndex++;

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "Price:"
            };
            label.Location = new Point(160 - label.Size.Width, CompanionDisplayPoint.Y);

            PriceLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(160 , CompanionDisplayPoint.Y),
                ForeColour = Color.White,
            };

            CompanionNameTextBox = new DXTextBox
            {
                Parent = this,
                Location = new Point(160, CompanionDisplayPoint.Y + 25),
                Size = new Size(120, 20)
            };
            CompanionNameTextBox.TextBox.TextChanged += TextBox_TextChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = "Name:"
            };
            label.Location = new Point(CompanionNameTextBox.Location.X - label.Size.Width, CompanionNameTextBox.Location.Y + (CompanionNameTextBox.Size.Height - label.Size.Height)/2);

            AdoptButton = new DXButton
            {
                Parent = this,
                Location = new Point(CompanionNameTextBox.Location.X, CompanionNameTextBox.Location.Y + 27),
                Size = new Size(120, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Adopt" }
            };
            AdoptButton.MouseClick += AdoptButton_MouseClick;

                UnlockButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.Right - 80, ClientArea.Y),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Unlock" }
            };

            UnlockButton.MouseClick += UnlockButton_MouseClick;

            SelectedIndex = 0;
        }

        #region Methods
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            CompanionNameValid = Globals.CharacterReg.IsMatch(CompanionNameTextBox.TextBox.Text);

            if (string.IsNullOrEmpty(CompanionNameTextBox.TextBox.Text))
                CompanionNameTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                CompanionNameTextBox.BorderColour = CompanionNameValid ? Color.Green : Color.Red;
        }

        private void AdoptButton_MouseClick(object sender, MouseEventArgs e)
        {
            AdoptAttempted = true;

            CEnvir.Enqueue(new C.CompanionAdopt { Index = SelectedCompanionInfo.Index, Name = CompanionNameTextBox.TextBox.Text });
        }
        private void UnlockButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Inventory.All(x => x == null || x.Info.ItemEffect != ItemEffect.CompanionTicket))
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.CompanionNeedTicket, MessageType.System);
                return;
            }

            DXMessageBox box = new DXMessageBox($"Are you sure you want to use a Companion Ticket?\n\n" + $"" + $"This will unlock the {SelectedCompanionInfo.MonsterInfo.MonsterName} appearance for new companions", "Unlock Appearance", DXMessageBoxButtons.YesNo);


            box.YesButton.MouseClick += (o1, e1) =>
            {
                CEnvir.Enqueue(new C.CompanionUnlock { Index = SelectedCompanionInfo.Index });

                UnlockButton.Enabled = false;
            };
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

        public void RefreshUnlockButton()
        {

            UnlockButton.Visible = !SelectedCompanionInfo.Available && !AvailableCompanions.Contains(SelectedCompanionInfo);

            if (GameScene.Game.User == null || SelectedCompanionInfo == null || SelectedCompanionInfo.Price <= GameScene.Game.User.Gold.Amount)
                PriceLabel.ForeColour = Color.FromArgb(198, 166, 99);
            else
                PriceLabel.ForeColour = Color.Red;


            AdoptButton.Enabled = CanAdopt;
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

                _SelectedCompanionInfo = null;
                SelectedCompanionInfoChanged = null;

                _SelectedIndex = 0;
                SelectedIndexChanged = null;

                _AdoptAttempted = false;
                AdoptAttemptedChanged = null;

                _CompanionNameValid = false;
                CompanionNameValidChanged = null;
                    
                
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (IndexLabel != null)
                {
                    if (!IndexLabel.IsDisposed)
                        IndexLabel.Dispose();

                    IndexLabel = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }

                if (LeftButton != null)
                {
                    if (!LeftButton.IsDisposed)
                        LeftButton.Dispose();

                    LeftButton = null;
                }

                if (RightButton != null)
                {
                    if (!RightButton.IsDisposed)
                        RightButton.Dispose();

                    RightButton = null;
                }
                
                if (AdoptButton != null)
                {
                    if (!AdoptButton.IsDisposed)
                        AdoptButton.Dispose();

                    AdoptButton = null;
                }

                if (UnlockButton != null)
                {
                    if (!UnlockButton.IsDisposed)
                        UnlockButton.Dispose();

                    UnlockButton = null;
                }

                if (CompanionNameTextBox != null)
                {
                    if (!CompanionNameTextBox.IsDisposed)
                        CompanionNameTextBox.Dispose();

                    CompanionNameTextBox = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCCompanionStorageDialog : DXWindow
    {
        #region Properties

        private DXVScrollBar ScrollBar;

        public NPCCompanionStorageRow[] Rows;

        public List<ClientUserCompanion> Companions = new List<ClientUserCompanion>();


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCCompanionStorageDialog()
        {
            TitleLabel.Text = "Storage";

            Movable = false;

            SetClientSize(new Size(198, 349));

            Rows = new NPCCompanionStorageRow[4];

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new NPCCompanionStorageRow
                {
                    Parent = this,
                    Location = new Point(ClientArea.X, ClientArea.Y + i*88),
                };
            }

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(ClientArea.Right - 15, ClientArea.Y + 1),
                Size = new Size(14, Rows.Length * 87 -1),
                VisibleSize = Rows.Length,
                Change = 1,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();
        }

        #region Methods

        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Companions.Count;

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i].UserCompanion = i + ScrollBar.Value >= Companions.Count ? null : Companions[i + ScrollBar.Value];
            }


        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Companions.Clear();
                Companions = null;
                
                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }

                    }

                    Rows = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCCompanionStorageRow : DXControl
    {
        #region Properties
        #region UserCompanion

        public ClientUserCompanion UserCompanion
        {
            get => _UserCompanion;
            set
            {
                ClientUserCompanion oldValue = _UserCompanion;
                _UserCompanion = value;

                OnUserCompanionChanged(oldValue, value);
            }
        }
        private ClientUserCompanion _UserCompanion;
        public event EventHandler<EventArgs> UserCompanionChanged;
        public void OnUserCompanionChanged(ClientUserCompanion oValue, ClientUserCompanion nValue)
        {
            UserCompanionChanged?.Invoke(this, EventArgs.Empty);

            if (UserCompanion == null)
            {
                Visible = false;
                return;
            }

            Visible = true;

            CompanionDisplay = new MonsterObject(UserCompanion.CompanionInfo);

            NameLabel.Text = UserCompanion.Name;
            LevelLabel.Text = $"Level {UserCompanion.Level}";

            if (UserCompanion == GameScene.Game.Companion)
                Selected = true;
            else
            {
                Selected = false;

                if (!string.IsNullOrEmpty(UserCompanion.CharacterName))
                {
                    RetrieveButton.Enabled = false;
                    RetrieveButton.Hint = $"The Companion is currently with {UserCompanion.CharacterName}.";
                }
                else
                {
                    RetrieveButton.Enabled = true;
                    RetrieveButton.Hint = null;
                }

            }
        }

        #endregion
        
        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            RetrieveButton.Visible = !Selected;
            StoreButton.Visible = Selected;


            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;
        public DXLabel NameLabel, LevelLabel;
        public DXButton StoreButton, RetrieveButton;

        #endregion

        public NPCCompanionStorageRow()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(180, 85);
            CompanionDisplayPoint = new Point(10, 45);

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(85,5)

            };

            LevelLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(85, 30)
            };

            StoreButton = new DXButton
            {
                Parent = this,
                Location = new Point(85, 60),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Store" },
                Visible = false
            };
            StoreButton.MouseClick += StoreButton_MouseClick;


            RetrieveButton = new DXButton
            {
                Parent = this,
                Location = new Point(85, 60),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Retrieve" }
            };
            RetrieveButton.MouseClick += RetrieveButton_MouseClick;


        }

        #region Methods

        private void StoreButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.CompanionStore { Index = UserCompanion.Index });
        }

        private void RetrieveButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.CompanionRetrieve { Index = UserCompanion.Index });
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

        
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _UserCompanion = null;
                UserCompanionChanged = null;
                
                _Selected = false;
                SelectedChanged = null;

                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

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

                if (StoreButton != null)
                {
                    if (!StoreButton.IsDisposed)
                        StoreButton.Dispose();

                    StoreButton = null;
                }

                if (RetrieveButton != null)
                {
                    if (!RetrieveButton.IsDisposed)
                        RetrieveButton.Dispose();

                    RetrieveButton = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCWeddingRingDialog : DXWindow
    {
        #region Properties

        public DXItemGrid RingGrid;
        public DXButton BindButton;


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCWeddingRingDialog()
        {
            HasTitle = false;
            SetClientSize(new Size(60, 85));
            CloseButton.Visible = false;

            DXLabel label = new DXLabel
            {
                Text = "Ring",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = ClientArea.Location,
                AutoSize = false,
                Size = new Size(ClientArea.Width, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };
            RingGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Width - 36)/2, label.Size.Height + label.Location.Y + 5),
                GridSize = new Size(1, 1),
                Linked = true,
                GridType = GridType.WeddingRing,
            };

            RingGrid.Grid[0].LinkChanged += (o, e) => BindButton.Enabled = RingGrid.Grid[0].Item != null;
            RingGrid.Grid[0].BeforeDraw += (o, e) => Draw(RingGrid.Grid[0], 31);

            BindButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point((ClientArea.Width - 50)/2 + ClientArea.X, ClientArea.Bottom - SmallButtonHeight),
                Label = { Text = "Bind" },
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled =  false,
            };
            BindButton.MouseClick += (o, e) =>
            {
                if (RingGrid.Grid[0].Item == null || RingGrid.Grid[0].Item.Info.ItemType != ItemType.Ring) return;

                CEnvir.Enqueue(new C.MarriageMakeRing {  Slot = RingGrid.Grid[0].Link.Slot });

                RingGrid.Grid[0].Link = null;
            };
        }
        
        #region Methods

        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }
        
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (RingGrid != null)
                {
                    if (!RingGrid.IsDisposed)
                        RingGrid.Dispose();

                    RingGrid = null;
                }

                if (BindButton != null)
                {
                    if (!BindButton.IsDisposed)
                        BindButton.Dispose();

                    BindButton = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCRefinementStoneDialog : DXWindow
    {
        #region Properties
        
        public DXItemGrid IronOreGrid, GoldOreGrid, DiamondGrid, SilverOreGrid, CrystalGrid;
        public DXNumberTextBox GoldBox;
        
        public DXButton SubmitButton;
        
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                GoldOreGrid.ClearLinks();
                DiamondGrid.ClearLinks();
                SilverOreGrid.ClearLinks();
                IronOreGrid.ClearLinks();
                CrystalGrid.ClearLinks();

                GoldBox.Value = 0;
            }
        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCRefinementStoneDialog()
        {
            TitleLabel.Text = "Refinement Stone";


            SetClientSize(new Size(491, 130));

            DXLabel label = new DXLabel
            {
                Text = "Iron Ore",
                Location = new Point(ClientArea.X + 21, ClientArea.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            IronOreGrid = new DXItemGrid
            {
                GridSize = new Size(4, 1),
                Parent = this,
                GridType = GridType.RefinementStoneIronOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Silver Ore",
                Location = new Point(IronOreGrid.Size.Width + 5 + IronOreGrid.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            SilverOreGrid = new DXItemGrid
            {
                GridSize = new Size(4, 1),
                Parent = this,
                GridType = GridType.RefinementStoneSilverOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Diamond",
                Location = new Point(SilverOreGrid.Size.Width + 5 + SilverOreGrid.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            DiamondGrid = new DXItemGrid
            {
                GridSize = new Size(4, 1),
                Parent = this,
                GridType = GridType.RefinementStoneDiamond,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Gold Ore",
                Location = new Point(ClientArea.X + 21, IronOreGrid.Location.Y + IronOreGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            GoldOreGrid = new DXItemGrid
            {
                GridSize = new Size(2, 1),
                Parent = this,
                GridType = GridType.RefinementStoneGoldOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };


            label = new DXLabel
            {
                Text = "Crystal",
                Location = new Point(IronOreGrid.Size.Width + 5 + IronOreGrid.Location.X, IronOreGrid.Location.Y + IronOreGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            CrystalGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.RefinementStoneCrystal,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };


            label = new DXLabel
            {
                Text = "Gold",
                Location = new Point(SilverOreGrid.Size.Width + 5 + SilverOreGrid.Location.X, SilverOreGrid.Location.Y + SilverOreGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            GoldBox = new DXNumberTextBox
            {
                Location = new Point(label.Location.X + 6, label.Location.Y + label.Size.Height + 5),
                Parent = this,
                MaxValue = 2000000000,
                Size = new Size(36 * 4 - 5, 16)
            };


            foreach (DXItemCell cell in IronOreGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in SilverOreGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in DiamondGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in GoldOreGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in CrystalGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }

            GoldBox.ValueChanged += (o, e) => UpdateButton();
            
            SubmitButton = new DXButton
            {
                Label = { Text = "Submit" },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                Enabled =false,
                ButtonType = ButtonType.SmallButton,
                Location = new Point(GoldBox.Location.X + GoldBox.Size.Width - 78, GoldBox.Location.Y + GoldBox.Size.Height + 5),
            };
            SubmitButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> iron = new List<CellLinkInfo>();
                foreach (DXItemCell cell in IronOreGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    iron.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (iron.Count < 4)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedIronOre, MessageType.System);
                    return;
                }

                List<CellLinkInfo> silver = new List<CellLinkInfo>();
                foreach (DXItemCell cell in SilverOreGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    silver.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (silver.Count < 4)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedSilverOre, MessageType.System);
                    return;
                }

                List<CellLinkInfo> diamond = new List<CellLinkInfo>();
                foreach (DXItemCell cell in DiamondGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    diamond.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (diamond.Count < 4)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedDiamond, MessageType.System);
                    return;
                }

                List<CellLinkInfo> gold = new List<CellLinkInfo>();
                foreach (DXItemCell cell in GoldOreGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    gold.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (gold.Count < 2)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedGoldOre, MessageType.System);
                    return;
                }

                List<CellLinkInfo> crystal = new List<CellLinkInfo>();
                foreach (DXItemCell cell in CrystalGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    crystal.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (crystal.Count < 1)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedCrystal, MessageType.System);
                    return;
                }

                if (GoldBox.Value > GameScene.Game.User.Gold.Amount)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedGold, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.NPCRefinementStone { IronOres = iron, SilverOres = silver, DiamondOres = diamond, GoldOres = gold, Crystal = crystal, Gold = GoldBox.Value });

                GoldBox.Value = 0;
            };
        }

        #region Methods
        public void UpdateButton()
        {
            SubmitButton.Enabled = false;

            if (GoldBox.Value > GameScene.Game.User.Gold.Amount)
            {
                GoldBox.BorderColour = Color.Red;
                return;
            }
            GoldBox.BorderColour = Color.FromArgb(198, 166, 99);

            foreach (DXItemCell cell in IronOreGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in SilverOreGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in DiamondGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in GoldOreGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in CrystalGrid.Grid)
            {
                if (cell.Link == null) return;
            }

            SubmitButton.Enabled = true;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (IronOreGrid != null)
                {
                    if (!IronOreGrid.IsDisposed)
                        IronOreGrid.Dispose();

                    IronOreGrid = null;
                }

                if (SilverOreGrid != null)
                {
                    if (!SilverOreGrid.IsDisposed)
                        SilverOreGrid.Dispose();

                    SilverOreGrid = null;
                }

                if (DiamondGrid != null)
                {
                    if (!DiamondGrid.IsDisposed)
                        DiamondGrid.Dispose();

                    DiamondGrid = null;
                }

                if (GoldOreGrid != null)
                {
                    if (!GoldOreGrid.IsDisposed)
                        GoldOreGrid.Dispose();

                    GoldOreGrid = null;
                }

                if (CrystalGrid != null)
                {
                    if (!CrystalGrid.IsDisposed)
                        CrystalGrid.Dispose();

                    CrystalGrid = null;
                }

                if (GoldBox != null)
                {
                    if (!GoldBox.IsDisposed)
                        GoldBox.Dispose();

                    GoldBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCItemFragmentDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;
        public DXButton FragmentButton;
        public DXLabel CostLabel;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                Grid.ClearLinks();
        }
        
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCItemFragmentDialog()
        {
            TitleLabel.Text = "Fragment Items";

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 3),
                Parent = this,
                GridType = GridType.ItemFragment,
                Linked = true
            };

            Movable = false;
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 50));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();


            CostLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "Fragment Cost:",
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "Select All" },
                Location = new Point(ClientArea.X, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            FragmentButton = new DXButton
            {
                Label = { Text = "Fragment" },
                Location = new Point(ClientArea.Right - 80, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            FragmentButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCFragment { Links = links });
            };
        }

        #region Methods
        private void CalculateCost()
        {
            int sum = 0;

            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                sum += cell.Link.Item.FragmentCost();
                count++;
            }

            CostLabel.ForeColour = sum > MapObject.User.Gold.Amount ? Color.Red : Color.White;

            CostLabel.Text = sum.ToString("#,##0");

            FragmentButton.Enabled = sum <= MapObject.User.Gold.Amount && count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (FragmentButton != null)
                {
                    if (!FragmentButton.IsDisposed)
                        FragmentButton.Dispose();

                    FragmentButton = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCMasterRefineDialog : DXWindow
    {
        #region Properties

        #region RefineType

        public RefineType RefineType
        {
            get => _RefineType;
            set
            {
                if (_RefineType == value) return;

                RefineType oldValue = _RefineType;
                _RefineType = value;

                OnRefineTypeChanged(oldValue, value);
            }
        }
        private RefineType _RefineType;
        public event EventHandler<EventArgs> RefineTypeChanged;
        public void OnRefineTypeChanged(RefineType oValue, RefineType nValue)
        {
            switch (oValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = true;
                    EvaluateButton.Enabled = true;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = false;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = false;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = false;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = false;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = false;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = false;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = false;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = false;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = false;
                    break;
            }

            switch (nValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = false;
                    EvaluateButton.Enabled = false;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = true;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = true;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = true;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = true;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = true;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = true;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = true;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = true;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = true;
                    break;
            }

            RefineTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        public DXItemGrid Fragment1Grid, Fragment2Grid, Fragment3Grid, RefinementStoneGrid, SpecialGrid;

        public DXCheckBox DCCheckBox, SPCheckBox, FireCheckBox, IceCheckBox, LightningCheckBox, WindCheckBox, HolyCheckBox, DarkCheckBox, PhantomCheckBox;
        public DXButton SubmitButton, EvaluateButton;
        
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                Fragment1Grid.ClearLinks();
                Fragment2Grid.ClearLinks();
                Fragment3Grid.ClearLinks();
                RefinementStoneGrid.ClearLinks();
                SpecialGrid.ClearLinks();
            }
        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCMasterRefineDialog()
        {
            TitleLabel.Text = "Master Refine";
            

            SetClientSize(new Size(491, 130));

            DXLabel label = new DXLabel
            {
                Text = "Fragement I",
                Location = ClientArea.Location,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            Fragment1Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment1,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Fragement II",
                Location = new Point(label.Size.Width + 5 + label.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            Fragment2Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment2,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Fragement III",
                Location = new Point(label.Size.Width + 5 + label.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            Fragment3Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment3,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };


            label = new DXLabel
            {
                Text = "Refinement Stone",
                Location = new Point(ClientArea.Location.X, Fragment3Grid.Location.Y + Fragment3Grid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            RefinementStoneGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineStone,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "Special",
                Location = new Point(Fragment3Grid.Location.X - 5, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            SpecialGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineSpecial,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };


            SetClientSize(new Size(491, SpecialGrid.Location.Y + SpecialGrid.Size.Height - ClientArea.Y + 2));

            DCCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "DC" },
                ReadOnly = true,
            };
            DCCheckBox.MouseClick += (o, e) => RefineType = RefineType.DC;
            SPCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Spell Power" },
                ReadOnly = true,
            };
            SPCheckBox.MouseClick += (o, e) => RefineType = RefineType.SpellPower;

            FireCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Fire" },
                ReadOnly = true,
            };
            FireCheckBox.MouseClick += (o, e) => RefineType = RefineType.Fire;

            IceCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Ice" },
                ReadOnly = true,
            };
            IceCheckBox.MouseClick += (o, e) => RefineType = RefineType.Ice;

            LightningCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Lightning" },
                ReadOnly = true,
            };
            LightningCheckBox.MouseClick += (o, e) => RefineType = RefineType.Lightning;

            WindCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Wind" },
                ReadOnly = true,
            };
            WindCheckBox.MouseClick += (o, e) => RefineType = RefineType.Wind;

            HolyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Holy" },
                ReadOnly = true,
            };
            HolyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Holy;

            DarkCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Dark" },
                ReadOnly = true,
            };
            DarkCheckBox.MouseClick += (o, e) => RefineType = RefineType.Dark;


            PhantomCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Phantom" },
                ReadOnly = true,
            };
            PhantomCheckBox.MouseClick += (o, e) => RefineType = RefineType.Phantom;


            DCCheckBox.Location = new Point(ClientArea.Right - DCCheckBox.Size.Width - 240, ClientArea.Y + 50);
            SPCheckBox.Location = new Point(ClientArea.Right - SPCheckBox.Size.Width - 156, ClientArea.Y + 50);

            FireCheckBox.Location = new Point(ClientArea.Right - FireCheckBox.Size.Width - 240, ClientArea.Y + 73);
            IceCheckBox.Location = new Point(ClientArea.Right - IceCheckBox.Size.Width - 156, ClientArea.Y + 73);
            LightningCheckBox.Location = new Point(ClientArea.Right - LightningCheckBox.Size.Width - 81, ClientArea.Y + 73);
            WindCheckBox.Location = new Point(ClientArea.Right - WindCheckBox.Size.Width - 5, ClientArea.Y + 73);
            HolyCheckBox.Location = new Point(ClientArea.Right - HolyCheckBox.Size.Width - 240, ClientArea.Y + 90);
            DarkCheckBox.Location = new Point(ClientArea.Right - DarkCheckBox.Size.Width - 156, ClientArea.Y + 90);
            PhantomCheckBox.Location = new Point(ClientArea.Right - PhantomCheckBox.Size.Width - 240, ClientArea.Y + 107);

            EvaluateButton = new DXButton
            {
                Label = { Text = "Evaluate" },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            EvaluateButton.Location = new Point(ClientArea.Right - EvaluateButton.Size.Width, ClientArea.Top + EvaluateButton.Size.Height);
            EvaluateButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> frag1 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag2 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag3 = new List<CellLinkInfo>();
                List<CellLinkInfo> stone = new List<CellLinkInfo>();
                List<CellLinkInfo> special = new List<CellLinkInfo>();



                foreach (DXItemCell cell in Fragment1Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag1.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in Fragment2Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag2.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in Fragment3Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag3.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in RefinementStoneGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    stone.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in SpecialGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    special.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }

                if (frag1.Count < 1 || frag1[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedFragmentI, MessageType.System);
                    return;
                }

                if (frag2.Count < 1 || frag2[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedFragmentII, MessageType.System);
                    return;
                }

                if (frag3.Count < 1)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedFragmentIII, MessageType.System);
                    return;
                }

                if (stone.Count < 1)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedRefinementStone, MessageType.System);
                    return;
                }
                
               DXMessageBox box = new DXMessageBox("Are you sure you want to pay for an evaluation?", "Evaluation", DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) => CEnvir.Enqueue(new C.NPCMasterRefineEvaluate { RefineType = RefineType, Fragment1s = frag1, Fragment2s = frag2, Fragment3s = frag3, Stones = stone, Specials = special });
            };
            
            label = new DXLabel
            {
                Text = $"Cost: {Globals.MasterRefineEvaluateCost:#,##0}",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.Right - label.Size.Width, EvaluateButton.Location.Y + EvaluateButton.Size.Height + 5);

            SubmitButton = new DXButton
            {
                Label = { Text = "Submit" },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            SubmitButton.Location = new Point(ClientArea.Right - SubmitButton.Size.Width, ClientArea.Bottom - SubmitButton.Size.Height);
            SubmitButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> frag1 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag2 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag3 = new List<CellLinkInfo>();
                List<CellLinkInfo> stone = new List<CellLinkInfo>();
                List<CellLinkInfo> special = new List<CellLinkInfo>();



                foreach (DXItemCell cell in Fragment1Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag1.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in Fragment2Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag2.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in Fragment3Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag3.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in RefinementStoneGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    stone.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in SpecialGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    special.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                if (frag1.Count < 1 || frag1[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedFragmentI, MessageType.System);
                    return;
                }

                if (frag2.Count < 1 || frag2[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedFragmentII, MessageType.System);
                    return;
                }

                if (frag3.Count < 1)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedFragmentIII, MessageType.System);
                    return;
                }

                if (stone.Count < 1)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.RefineNeedRefinementStone, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.NPCMasterRefine { RefineType = RefineType, Fragment1s = frag1, Fragment2s = frag2, Fragment3s = frag3, Stones = stone, Specials = special });
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _RefineType = 0;
                RefineTypeChanged = null;
                
                if (Fragment1Grid != null)
                {
                    if (!Fragment1Grid.IsDisposed)
                        Fragment1Grid.Dispose();

                    Fragment1Grid = null;
                }

                if (Fragment2Grid != null)
                {
                    if (!Fragment2Grid.IsDisposed)
                        Fragment2Grid.Dispose();

                    Fragment2Grid = null;
                }

                if (Fragment3Grid != null)
                {
                    if (!Fragment3Grid.IsDisposed)
                        Fragment3Grid.Dispose();

                    Fragment3Grid = null;
                }

                if (RefinementStoneGrid != null)
                {
                    if (!RefinementStoneGrid.IsDisposed)
                        RefinementStoneGrid.Dispose();

                    RefinementStoneGrid = null;
                }

                if (SpecialGrid != null)
                {
                    if (!SpecialGrid.IsDisposed)
                        SpecialGrid.Dispose();

                    SpecialGrid = null;
                }


                if (DCCheckBox != null)
                {
                    if (!DCCheckBox.IsDisposed)
                        DCCheckBox.Dispose();

                    DCCheckBox = null;
                }

                if (SPCheckBox != null)
                {
                    if (!SPCheckBox.IsDisposed)
                        SPCheckBox.Dispose();

                    SPCheckBox = null;
                }

                if (FireCheckBox != null)
                {
                    if (!FireCheckBox.IsDisposed)
                        FireCheckBox.Dispose();

                    FireCheckBox = null;
                }

                if (IceCheckBox != null)
                {
                    if (!IceCheckBox.IsDisposed)
                        IceCheckBox.Dispose();

                    IceCheckBox = null;
                }

                if (LightningCheckBox != null)
                {
                    if (!LightningCheckBox.IsDisposed)
                        LightningCheckBox.Dispose();

                    LightningCheckBox = null;
                }

                if (WindCheckBox != null)
                {
                    if (!WindCheckBox.IsDisposed)
                        WindCheckBox.Dispose();

                    WindCheckBox = null;
                }

                if (HolyCheckBox != null)
                {
                    if (!HolyCheckBox.IsDisposed)
                        HolyCheckBox.Dispose();

                    HolyCheckBox = null;
                }

                if (DarkCheckBox != null)
                {
                    if (!DarkCheckBox.IsDisposed)
                        DarkCheckBox.Dispose();

                    DarkCheckBox = null;
                }

                if (PhantomCheckBox != null)
                {
                    if (!PhantomCheckBox.IsDisposed)
                        PhantomCheckBox.Dispose();

                    PhantomCheckBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCAccessoryUpgradeDialog : DXWindow
    {

        #region Properties

        #region RefineType

        public RefineType RefineType
        {
            get => _RefineType;
            set
            {
                if (_RefineType == value) return;

                RefineType oldValue = _RefineType;
                _RefineType = value;

                OnRefineTypeChanged(oldValue, value);
            }
        }
        private RefineType _RefineType;
        public event EventHandler<EventArgs> RefineTypeChanged;
        public void OnRefineTypeChanged(RefineType oValue, RefineType nValue)
        {
            switch (oValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = true;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = false;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = false;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = false;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = false;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = false;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = false;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = false;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = false;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = false;
                    break;
                case RefineType.Health:
                    HealthCheckBox.Checked = false;
                    break;
                case RefineType.Mana:
                    ManaCheckBox.Checked = false;
                    break;
                case RefineType.AC:
                    ACCheckBox.Checked = false;
                    break;
                case RefineType.MR:
                    MRCheckBox.Checked = false;
                    break;
                case RefineType.Accuracy:
                    AccuracyCheckBox.Checked = false;
                    break;
                case RefineType.Agility:
                    AgilityCheckBox.Checked = false;
                    break;
                case RefineType.HealthPercent:
                    HealthPercentCheckBox.Checked = false;
                    break;
                case RefineType.ManaPercent:
                    ManaPercentCheckBox.Checked = false;
                    break;
                case RefineType.DCPercent:
                    DCPercentCheckBox.Checked = false;
                    break;
                case RefineType.SPPercent:
                    SPPercentCheckBox.Checked = false;
                    break;
            }

            switch (nValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = false;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = true;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = true;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = true;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = true;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = true;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = true;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = true;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = true;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = true;
                    break;
                case RefineType.Health:
                    HealthCheckBox.Checked = true;
                    break;
                case RefineType.Mana:
                    ManaCheckBox.Checked = true;
                    break;
                case RefineType.AC:
                    ACCheckBox.Checked = true;
                    break;
                case RefineType.MR:
                    MRCheckBox.Checked = true;
                    break;
                case RefineType.Accuracy:
                    AccuracyCheckBox.Checked = true;
                    break;
                case RefineType.Agility:
                    AgilityCheckBox.Checked = true;
                    break;
                case RefineType.HealthPercent:
                    HealthPercentCheckBox.Checked = true;
                    break;
                case RefineType.ManaPercent:
                    ManaPercentCheckBox.Checked = true;
                    break;
                case RefineType.DCPercent:
                    DCPercentCheckBox.Checked = true;
                    break;
                case RefineType.SPPercent:
                    SPPercentCheckBox.Checked = true;
                    break;
            }

            RefineTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemGrid TargetCell;

        public DXCheckBox DCPercentCheckBox, SPPercentCheckBox, HealthPercentCheckBox, ManaPercentCheckBox,
                          FireCheckBox, IceCheckBox, LightningCheckBox, WindCheckBox, HolyCheckBox, DarkCheckBox, PhantomCheckBox,
                          DCCheckBox, SPCheckBox, HealthCheckBox, ManaCheckBox,
                          ACCheckBox, MRCheckBox, AccuracyCheckBox, AgilityCheckBox;

        public DXButton SubmitButton;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)            
                TargetCell.ClearLinks();
            
        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCAccessoryUpgradeDialog()
        {
            TitleLabel.Text = "Accessory Upgrade";

            SetClientSize(new Size(491, 130));
            Movable = false;

            DXLabel label = new DXLabel
            {
                Text = "Item",
                Location = new Point(ClientArea.X + 65, ClientArea.Y + 15),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            TargetCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryRefineUpgradeTarget,
                Linked = true,
                Location = new Point(label.Location.X - 3, label.Location.Y + label.Size.Height + 5)
            };

            DCPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "DC 1%" },
                ReadOnly = true,
            };
            DCPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.DCPercent;

            SPPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Spell Power 1%" },
                ReadOnly = true,
            };
            SPPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.SPPercent;

            HealthPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Health 1%" },
                ReadOnly = true,
            };
            HealthPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.HealthPercent;

            ManaPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Mana 1%" },
                ReadOnly = true,
            };
            ManaPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.ManaPercent;

            DCCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "DC 0-1" },
                ReadOnly = true,
            };
            DCCheckBox.MouseClick += (o, e) => RefineType = RefineType.DC;

            SPCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Spell Power 0-1" },
                ReadOnly = true,
            };
            SPCheckBox.MouseClick += (o, e) => RefineType = RefineType.SpellPower;

            FireCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Fire +1" },
                ReadOnly = true,
            };
            FireCheckBox.MouseClick += (o, e) => RefineType = RefineType.Fire;

            IceCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Ice +1" },
                ReadOnly = true,
            };
            IceCheckBox.MouseClick += (o, e) => RefineType = RefineType.Ice;

            LightningCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Lightning +1" },
                ReadOnly = true,
            };
            LightningCheckBox.MouseClick += (o, e) => RefineType = RefineType.Lightning;

            WindCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Wind +1" },
                ReadOnly = true,
            };
            WindCheckBox.MouseClick += (o, e) => RefineType = RefineType.Wind;

            HolyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Holy +1" },
                ReadOnly = true,
            };
            HolyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Holy;

            DarkCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Dark +1" },
                ReadOnly = true,
            };
            DarkCheckBox.MouseClick += (o, e) => RefineType = RefineType.Dark;

            PhantomCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Phantom +1" },
                ReadOnly = true,
            };
            PhantomCheckBox.MouseClick += (o, e) => RefineType = RefineType.Phantom;

            HealthCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Health +10" },
                ReadOnly = true,
            };
            HealthCheckBox.MouseClick += (o, e) => RefineType = RefineType.Health;

            ManaCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Mana +10" },
                ReadOnly = true,
            };
            ManaCheckBox.MouseClick += (o, e) => RefineType = RefineType.Mana;

            ACCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "AC 1-1" },
                ReadOnly = true,
            };
            ACCheckBox.MouseClick += (o, e) => RefineType = RefineType.AC;

            MRCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "MR 1-1" },
                ReadOnly = true,
            };
            MRCheckBox.MouseClick += (o, e) => RefineType = RefineType.MR;

            AccuracyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Accuracy +1" },
                ReadOnly = true,
            };
            AccuracyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Accuracy;

            AgilityCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Agility +1" },
                ReadOnly = true,
            };
            AgilityCheckBox.MouseClick += (o, e) => RefineType = RefineType.Agility;

            DCPercentCheckBox.Location = new Point(ClientArea.Right - DCPercentCheckBox.Size.Width - 280, ClientArea.Y + 5);
            SPPercentCheckBox.Location = new Point(ClientArea.Right - SPPercentCheckBox.Size.Width - 186, ClientArea.Y + 5);
            HealthPercentCheckBox.Location = new Point(ClientArea.Right - HealthPercentCheckBox.Size.Width - 101, ClientArea.Y + 5);
            ManaPercentCheckBox.Location = new Point(ClientArea.Right - ManaPercentCheckBox.Size.Width - 15, ClientArea.Y + 5);


            DCCheckBox.Location = new Point(ClientArea.Right - DCCheckBox.Size.Width - 280, ClientArea.Y + 22);
            SPCheckBox.Location = new Point(ClientArea.Right - SPCheckBox.Size.Width - 186, ClientArea.Y + 22);
            HealthCheckBox.Location = new Point(ClientArea.Right - HealthCheckBox.Size.Width - 101, ClientArea.Y + 22);
            ManaCheckBox.Location = new Point(ClientArea.Right - ManaCheckBox.Size.Width - 15, ClientArea.Y + 22);

            ACCheckBox.Location = new Point(ClientArea.Right - ACCheckBox.Size.Width - 280, ClientArea.Y + 39);
            MRCheckBox.Location = new Point(ClientArea.Right - MRCheckBox.Size.Width - 186, ClientArea.Y + 39);
            AccuracyCheckBox.Location = new Point(ClientArea.Right - AccuracyCheckBox.Size.Width - 101, ClientArea.Y + 39);
            AgilityCheckBox.Location = new Point(ClientArea.Right - AgilityCheckBox.Size.Width - 15, ClientArea.Y + 39);


            new DXLabel
            {
                Text = "Attack Element",
                Location = new Point(ClientArea.Right - HealthCheckBox.Size.Width - 150, ClientArea.Y + 73),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };


            FireCheckBox.Location = new Point(ClientArea.Right - FireCheckBox.Size.Width - 280, ClientArea.Y + 90);
            IceCheckBox.Location = new Point(ClientArea.Right - IceCheckBox.Size.Width - 186, ClientArea.Y + 90);
            LightningCheckBox.Location = new Point(ClientArea.Right - LightningCheckBox.Size.Width - 101, ClientArea.Y + 90);
            WindCheckBox.Location = new Point(ClientArea.Right - WindCheckBox.Size.Width - 15, ClientArea.Y + 90);
            HolyCheckBox.Location = new Point(ClientArea.Right - HolyCheckBox.Size.Width - 280, ClientArea.Y + 105);
            DarkCheckBox.Location = new Point(ClientArea.Right - DarkCheckBox.Size.Width - 186, ClientArea.Y + 105);
            PhantomCheckBox.Location = new Point(ClientArea.Right - PhantomCheckBox.Size.Width - 101, ClientArea.Y + 105);


            SubmitButton = new DXButton
            {
                Label = { Text = "Submit" },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            SubmitButton.Location = new Point(ClientArea.Left + 40, ClientArea.Bottom - SubmitButton.Size.Height- 5);
            SubmitButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                ;

                DXItemCell cell = TargetCell.Grid[0];

                if (cell.Link == null) return;

                CellLinkInfo targetLink = new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot };
                cell.Link.Locked = true;
                cell.Link = null;
                
                CEnvir.Enqueue(new C.NPCAccessoryUpgrade { Target = targetLink, RefineType = RefineType });

            };


        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _RefineType = 0;
                RefineTypeChanged = null;

                if (TargetCell != null)
                {
                    if (!TargetCell.IsDisposed)
                        TargetCell.Dispose();

                    TargetCell = null;
                }
                

                if (DCCheckBox != null)
                {
                    if (!DCCheckBox.IsDisposed)
                        DCCheckBox.Dispose();

                    DCCheckBox = null;
                }

                if (SPCheckBox != null)
                {
                    if (!SPCheckBox.IsDisposed)
                        SPCheckBox.Dispose();

                    SPCheckBox = null;
                }

                if (FireCheckBox != null)
                {
                    if (!FireCheckBox.IsDisposed)
                        FireCheckBox.Dispose();

                    FireCheckBox = null;
                }

                if (IceCheckBox != null)
                {
                    if (!IceCheckBox.IsDisposed)
                        IceCheckBox.Dispose();

                    IceCheckBox = null;
                }

                if (LightningCheckBox != null)
                {
                    if (!LightningCheckBox.IsDisposed)
                        LightningCheckBox.Dispose();

                    LightningCheckBox = null;
                }

                if (WindCheckBox != null)
                {
                    if (!WindCheckBox.IsDisposed)
                        WindCheckBox.Dispose();

                    WindCheckBox = null;
                }

                if (HolyCheckBox != null)
                {
                    if (!HolyCheckBox.IsDisposed)
                        HolyCheckBox.Dispose();

                    HolyCheckBox = null;
                }

                if (DarkCheckBox != null)
                {
                    if (!DarkCheckBox.IsDisposed)
                        DarkCheckBox.Dispose();

                    DarkCheckBox = null;
                }

                if (PhantomCheckBox != null)
                {
                    if (!PhantomCheckBox.IsDisposed)
                        PhantomCheckBox.Dispose();

                    PhantomCheckBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }

                if (HealthCheckBox != null)
                {
                    if (!HealthCheckBox.IsDisposed)
                        HealthCheckBox.Dispose();

                    HealthCheckBox = null;
                }

                if (ManaCheckBox != null)
                {
                    if (!ManaCheckBox.IsDisposed)
                        ManaCheckBox.Dispose();

                    ManaCheckBox = null;
                }

                if (ACCheckBox != null)
                {
                    if (!ACCheckBox.IsDisposed)
                        ACCheckBox.Dispose();

                    ACCheckBox = null;
                }

                if (MRCheckBox != null)
                {
                    if (!MRCheckBox.IsDisposed)
                        MRCheckBox.Dispose();

                    MRCheckBox = null;
                }

                if (AccuracyCheckBox != null)
                {
                    if (!AccuracyCheckBox.IsDisposed)
                        AccuracyCheckBox.Dispose();

                    AccuracyCheckBox = null;
                }

                if (AgilityCheckBox != null)
                {
                    if (!AgilityCheckBox.IsDisposed)
                        AgilityCheckBox.Dispose();

                    AgilityCheckBox = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCAccessoryLevelDialog : DXWindow
    {
        #region Properties

        public DXItemGrid TargetCell;
        public DXItemGrid Grid;
        public DXButton LevelUpButton;
        public DXLabel CostLabel;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                TargetCell.ClearLinks();
                Grid.ClearLinks();
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCAccessoryLevelDialog()
        {
            TitleLabel.Text = "Accessory Leveling";

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 3),
                Parent = this,
                GridType = GridType.AccessoryRefineLevelItems,
                Linked = true
            };

            Movable = false;
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 110));
            Grid.Location = new Point(ClientArea.X, ClientArea.Y + 60);

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();

            DXLabel label = new DXLabel
            {
                Text = "Accessory",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y);

            TargetCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryRefineLevelTarget,
                Linked = true,
            };
            TargetCell.Location = new Point(label.Location.X + (label.Size.Width - TargetCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            CostLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "Leveling Cost:",
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "Select All" },
                Location = new Point(ClientArea.X, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            LevelUpButton = new DXButton
            {
                Label = { Text = "Level Up" },
                Location = new Point(ClientArea.Right - 80, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            LevelUpButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                ;
                List<CellLinkInfo> links = new List<CellLinkInfo>();
                
                DXItemCell target = TargetCell.Grid[0];

                if (target.Link == null) return;

                CellLinkInfo targetLink = new CellLinkInfo { Count = target.LinkedCount, GridType = target.Link.GridType, Slot = target.Link.Slot };
                target.Link.Locked = true;
                target.Link = null;

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCAccessoryLevelUp { Target = targetLink, Links = links });
            };
        }

        #region Methods
        private void CalculateCost()
        {
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;
                
                count++;
            }

            CostLabel.ForeColour = count > MapObject.User.Gold.Amount ? Color.Red : Color.White;

            //CostLabel.Text = count.ToString("#,##0");

            LevelUpButton.Enabled = count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (LevelUpButton != null)
                {
                    if (!LevelUpButton.IsDisposed)
                        LevelUpButton.Dispose();

                    LevelUpButton = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class NPCAccessoryResetDialog : DXWindow
    {
        #region Properties

        public DXItemGrid AccessoryGrid;
        public DXButton ResetButton;
        
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCAccessoryResetDialog()
        {
            HasTitle = false;
            SetClientSize(new Size(100, 105));
            CloseButton.Visible = false;

            DXLabel label = new DXLabel
            {
                Text = "Accessory",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = ClientArea.Location,
                AutoSize = false,
                Size = new Size(ClientArea.Width, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };
            AccessoryGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Width - 36) / 2, label.Size.Height + label.Location.Y + 5),
                GridSize = new Size(1, 1),
                Linked = true,
                GridType = GridType.AccessoryReset,
            };

            AccessoryGrid.Grid[0].LinkChanged += (o, e) => ResetButton.Enabled = AccessoryGrid.Grid[0].Item != null;
            AccessoryGrid.Grid[0].BeforeDraw += (o, e) => Draw(AccessoryGrid.Grid[0], 31);

            ResetButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point((ClientArea.Width - 50) / 2 + ClientArea.X, ClientArea.Bottom - SmallButtonHeight),
                Label = { Text = "Reset" },
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };

            label = new DXLabel
            {
                Text = $"Cost: {Globals.AccessoryResetCost:#,##0}",
                Parent = this,
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, ResetButton.Location.Y - 25),
                AutoSize = false,
                Size = new Size(ClientArea.Width, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };

            ResetButton.MouseClick += (o, e) =>
            {
                if (AccessoryGrid.Grid[0].Item == null) return;

                if (GameScene.Game.Observer) return;

                switch (AccessoryGrid.Grid[0].Item.Info.ItemType)
                {
                    case ItemType.Ring:
                    case ItemType.Bracelet:
                    case ItemType.Necklace:
                        break;
                    default:
                        return;
                }
                

                CellLinkInfo targetLink = new CellLinkInfo { Count = AccessoryGrid.Grid[0].LinkedCount, GridType = AccessoryGrid.Grid[0].Link.GridType, Slot = AccessoryGrid.Grid[0].Link.Slot };

                AccessoryGrid.Grid[0].Link.Locked = true;
                AccessoryGrid.Grid[0].Link = null;

                CEnvir.Enqueue(new C.NPCAccessoryReset { Cell = targetLink });
            };
        }

        #region Methods

        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;    }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (AccessoryGrid != null)
                {
                    if (!AccessoryGrid.IsDisposed)
                        AccessoryGrid.Dispose();

                    AccessoryGrid = null;
                }

                if (ResetButton != null)
                {
                    if (!ResetButton.IsDisposed)
                        ResetButton.Dispose();

                    ResetButton = null;
                }
            }

        }

        #endregion
    }

    public class NPCWeaponCraftWindow : DXWindow
    {
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        private DXComboBox ClassComboBox;

        private DXImageControl PreviewImageBox;


        public DXItemGrid TemplateCell;

        public DXItemGrid YellowCell;
        public DXItemGrid BlueCell;
        public DXItemGrid RedCell;
        public DXItemGrid PurpleCell;
        public DXItemGrid GreenCell;
        public DXItemGrid GreyCell;

        private DXLabel ClassLabel;

        private DXButton AttemptButton;
        
        
        #region RequiredClass

        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                RequiredClass oldValue = _RequiredClass;
                _RequiredClass = value;

                OnRequiredClassChanged(oldValue, value);
            }
        }
        private RequiredClass _RequiredClass;
        public event EventHandler<EventArgs> RequiredClassChanged;
        public virtual void OnRequiredClassChanged(RequiredClass oValue, RequiredClass nValue)
        {

            if (TemplateCell.Grid[0].Item == null || TemplateCell.Grid[0].Item.Info.ItemEffect == ItemEffect.WeaponTemplate)
            {
                switch (RequiredClass)
                {
                    case RequiredClass.None:
                        PreviewImageBox.Index = 1110;
                        break;
                    case RequiredClass.Warrior:
                        PreviewImageBox.Index = 1111;
                        break;
                    case RequiredClass.Wizard:
                        PreviewImageBox.Index = 1112;
                        break;
                    case RequiredClass.Taoist:
                        PreviewImageBox.Index = 1113;
                        break;
                    case RequiredClass.Assassin:
                        PreviewImageBox.Index = 1114;
                        break;

                }
            }
            else
            {
                PreviewImageBox.Index = TemplateCell.Grid[0].Item.Info.Image;
            }

            AttemptButton.Enabled = CanCraft;

            RequiredClassChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        public long Cost
        {
            get
            {

                long cost = Globals.CraftWeaponPercentCost;

                if (TemplateCell.Grid[0].Item != null && TemplateCell.Grid[0].Item.Info.ItemEffect != ItemEffect.WeaponTemplate)
                {
                    switch (TemplateCell.Grid[0].Item.Info.Rarity)
                    {
                        case Rarity.Common:
                            cost = Globals.CommonCraftWeaponPercentCost;
                            break;
                        case Rarity.Superior:
                            cost = Globals.SuperiorCraftWeaponPercentCost;
                            break;
                        case Rarity.Elite:
                            cost = Globals.EliteCraftWeaponPercentCost;
                            break;
                    }
                }

                return cost;
            }
        }

        public bool CanCraft => Cost <= GameScene.Game.User.Gold.Amount && TemplateCell.Grid[0].Link != null && RequiredClass != RequiredClass.None;

        public NPCWeaponCraftWindow()
        {
            TitleLabel.Text = "Weapon Craft";

            HasFooter = false;

            SetClientSize(new Size(250, 280));

            DXLabel label = new DXLabel
            {
                Text = "Template / Weapon",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y);

            TemplateCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftTemplate,
                Linked = true,
            };
            TemplateCell.Location = new Point(label.Location.X + (label.Size.Width - TemplateCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);
            TemplateCell.Grid[0].LinkChanged += (o, e) =>
            {
                if (TemplateCell.Grid[0].Item == null || TemplateCell.Grid[0].Item.Info.ItemEffect == ItemEffect.WeaponTemplate)
                {
                    ClassLabel.Text = "Class:";
                    switch (RequiredClass)
                    {
                        case RequiredClass.None:
                            PreviewImageBox.Index = 1110;
                            break;
                        case RequiredClass.Warrior:
                            PreviewImageBox.Index = 1111;
                            break;
                        case RequiredClass.Wizard:
                            PreviewImageBox.Index = 1112;
                            break;
                        case RequiredClass.Taoist:
                            PreviewImageBox.Index = 1113;
                            break;
                        case RequiredClass.Assassin:
                            PreviewImageBox.Index = 1114;
                            break;

                    }
                }
                else
                {
                    ClassLabel.Text = "Stats:";
                    PreviewImageBox.Index = TemplateCell.Grid[0].Item.Info.Image;
                }

                ClassLabel.Location = new Point(ClientArea.X + (ClientArea.Width - ClassLabel.Size.Width) / 2, ClientArea.Y + 185);

                AttemptButton.Enabled = CanCraft;
            };


            label = new DXLabel
            {
                Text = "Yellow",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y + 60);
            YellowCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftYellow,
                Linked = true,
            };
            YellowCell.Location = new Point(label.Location.X + (label.Size.Width - YellowCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Blue",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y + 60);
            BlueCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftBlue,
                Linked = true,
            };
            BlueCell.Location = new Point(label.Location.X + (label.Size.Width - BlueCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Red",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 100, ClientArea.Y + 60);
            RedCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftRed,
                Linked = true,
            };
            RedCell.Location = new Point(label.Location.X + (label.Size.Width - RedCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Purple",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y + 120);

            PurpleCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftPurple,
                Linked = true,
            };
            PurpleCell.Location = new Point(label.Location.X + (label.Size.Width - PurpleCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Green",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y + 120);

            GreenCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftGreen,
                Linked = true,
            };
            GreenCell.Location = new Point(label.Location.X + (label.Size.Width - GreenCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "Grey",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 100, ClientArea.Y + 120);

            GreyCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftGrey,
                Linked = true,
            };
            GreyCell.Location = new Point(label.Location.X + (label.Size.Width - GreyCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);


            ClassLabel = new DXLabel
            {
                Text = "Class:",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            ClassLabel.Location = new Point(ClientArea.X + (ClientArea.Width - ClassLabel.Size.Width) / 2, ClientArea.Y + 185);
            #region Class
            ClassComboBox = new DXComboBox
            {
                Parent = this,
                Size = new Size(GreenCell.Size.Width + 48, DXComboBox.DefaultNormalHeight),
            };
            ClassComboBox.Location = new Point(GreenCell.Location.X + 1, ClientArea.Y + 185);
            ClassComboBox.SelectedItemChanged += (o, e) =>
            {
                RequiredClass = (RequiredClass?)ClassComboBox.SelectedItem ?? RequiredClass.None;
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.None}" },
                Item = RequiredClass.None
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Warrior}" },
                Item = RequiredClass.Warrior
            };
            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Wizard}" },
                Item = RequiredClass.Wizard
            };
            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Taoist}" },
                Item = RequiredClass.Taoist
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"{RequiredClass.Assassin}" },
                Item = RequiredClass.Assassin
            };

            ClassComboBox.ListBox.SelectItem(RequiredClass.None);
            #endregion

            #region Preview

            PreviewImageBox = new DXImageControl
            {
                Parent = this,
                Location = new Point(ClientArea.X + 20, ClientArea.Y + ClientArea.Height / 2 - 76),
                LibraryFile = LibraryFile.Equip,
                Index = 1110,
                Border = true,
            };

            #endregion

           

            AttemptButton = new DXButton
            {
                Parent = this,
                Location = new Point(YellowCell.Location.X, ClientArea.Y + 260),
                Size = new Size(YellowCell.Size.Width + 99, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Craft" }
            };
            AttemptButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;


                if (TemplateCell.Grid[0].Link == null) return;

                C.NPCWeaponCraft packet = new C.NPCWeaponCraft
                {
                    Class = RequiredClass,

                    Template = new CellLinkInfo { Count = TemplateCell.Grid[0].LinkedCount, GridType = TemplateCell.Grid[0].Link.GridType, Slot = TemplateCell.Grid[0].Link.Slot }
                };

                TemplateCell.Grid[0].Link.Locked = true;
                TemplateCell.Grid[0].Link = null; 

                if (YellowCell.Grid[0].Link != null)
                {
                    packet.Yellow = new CellLinkInfo { Count = YellowCell.Grid[0].LinkedCount, GridType = YellowCell.Grid[0].Link.GridType, Slot = YellowCell.Grid[0].Link.Slot };
                    YellowCell.Grid[0].Link.Locked = true;
                    YellowCell.Grid[0].Link = null;
                }

                if (BlueCell.Grid[0].Link != null)
                {
                    packet.Blue = new CellLinkInfo { Count = BlueCell.Grid[0].LinkedCount, GridType = BlueCell.Grid[0].Link.GridType, Slot = BlueCell.Grid[0].Link.Slot };
                    BlueCell.Grid[0].Link.Locked = true;
                    BlueCell.Grid[0].Link = null;
                }

                if (RedCell.Grid[0].Link != null)
                {
                    packet.Red = new CellLinkInfo { Count = RedCell.Grid[0].LinkedCount, GridType = RedCell.Grid[0].Link.GridType, Slot = RedCell.Grid[0].Link.Slot };
                    RedCell.Grid[0].Link.Locked = true;
                    RedCell.Grid[0].Link = null;
                }

                if (PurpleCell.Grid[0].Link != null)
                {
                    packet.Purple = new CellLinkInfo { Count = PurpleCell.Grid[0].LinkedCount, GridType = PurpleCell.Grid[0].Link.GridType, Slot = PurpleCell.Grid[0].Link.Slot };
                    PurpleCell.Grid[0].Link.Locked = true;
                    PurpleCell.Grid[0].Link = null;
                }

                if (GreenCell.Grid[0].Link != null)
                {
                    packet.Green = new CellLinkInfo { Count = GreenCell.Grid[0].LinkedCount, GridType = GreenCell.Grid[0].Link.GridType, Slot = GreenCell.Grid[0].Link.Slot };
                    GreenCell.Grid[0].Link.Locked = true;
                    GreenCell.Grid[0].Link = null;
                }

                if (GreyCell.Grid[0].Link != null)
                {
                    packet.Grey = new CellLinkInfo { Count = GreyCell.Grid[0].LinkedCount, GridType = GreyCell.Grid[0].Link.GridType, Slot = GreyCell.Grid[0].Link.Slot };
                    GreyCell.Grid[0].Link.Locked = true;
                    GreyCell.Grid[0].Link = null;
                }

                CEnvir.Enqueue(packet);
                AttemptButton.Enabled = CanCraft;
            };
        }

    }

    public sealed class NPCAccessoryRefineDialog : DXWindow
    {
        #region Properties
        #region RefineType

        public RefineType RefineType
        {
            get => _RefineType;
            set
            {
                if (_RefineType == value) return;

                RefineType oldValue = _RefineType;
                _RefineType = value;

                OnRefineTypeChanged(oldValue, value);
            }
        }
        private RefineType _RefineType;
        public event EventHandler<EventArgs> RefineTypeChanged;

        public void OnRefineTypeChanged(RefineType oValue, RefineType nValue)
        {

            switch (oValue)
            {
                case RefineType.None:
                    RefineButton.Enabled = true;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = false;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = false;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = false;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = false;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = false;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = false;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = false;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = false;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = false;
                    break;
                case RefineType.Health:
                    HealthCheckBox.Checked = false;
                    break;
                case RefineType.Mana:
                    ManaCheckBox.Checked = false;
                    break;
                case RefineType.AC:
                    ACCheckBox.Checked = false;
                    break;
                case RefineType.MR:
                    MRCheckBox.Checked = false;
                    break;
                case RefineType.Accuracy:
                    AccuracyCheckBox.Checked = false;
                    break;
                case RefineType.Agility:
                    AgilityCheckBox.Checked = false;
                    break;
            }

            switch (nValue)
            {
                case RefineType.None:
                    RefineButton.Enabled = false;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = true;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = true;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = true;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = true;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = true;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = true;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = true;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = true;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = true;
                    break;
                case RefineType.Health:
                    HealthCheckBox.Checked = true;
                    break;
                case RefineType.Mana:
                    ManaCheckBox.Checked = true;
                    break;
                case RefineType.AC:
                    ACCheckBox.Checked = true;
                    break;
                case RefineType.MR:
                    MRCheckBox.Checked = true;
                    break;
                case RefineType.Accuracy:
                    AccuracyCheckBox.Checked = true;
                    break;
                case RefineType.Agility:
                    AgilityCheckBox.Checked = true;
                    break;

            }

            SetRefine();

            RefineTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        public DXItemGrid TargetCell;
        public DXItemGrid OreTargetCell;
        public DXItemGrid Grid;
        public DXButton RefineButton;
        public DXLabel CostLabel;


        public DXCheckBox FireCheckBox, IceCheckBox, LightningCheckBox, WindCheckBox, HolyCheckBox, DarkCheckBox, PhantomCheckBox,
                          DCCheckBox, SPCheckBox, HealthCheckBox, ManaCheckBox,
                          ACCheckBox, MRCheckBox, AccuracyCheckBox, AgilityCheckBox;
        

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                TargetCell.ClearLinks();
                Grid.ClearLinks();
                OreTargetCell.ClearLinks();
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public NPCAccessoryRefineDialog()
        {
            TitleLabel.Text = "Accessory Refine";

            SetClientSize(new Size(491, 200));
            Movable = false;

            DXLabel label = new DXLabel
            {
                Text = "Accessory",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + 10, ClientArea.Y + 18);

            TargetCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryRefineCombTarget,
                Linked = true,
            };
            TargetCell.Location = new Point(label.Location.X + (label.Size.Width - TargetCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            DXLabel Orelabel = new DXLabel
            {
                Text = "Ore",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            Orelabel.Location = new Point(TargetCell.Location.X + TargetCell.Size.Width * 2, label.Location.Y);

            OreTargetCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.RefineCorundumOre,
                Linked = true,
            };
            OreTargetCell.Location = new Point(TargetCell.Location.X + ((TargetCell.Size.Width - 1) * 2), Orelabel.Location.Y + Orelabel.Size.Height + 5);

            DXLabel Materialslabel = new DXLabel
            {
                Text = "Copies Of Accessory",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            Materialslabel.Location = new Point(TargetCell.Location.X, TargetCell.Location.Y + TargetCell.Size.Height + 5);

            Grid = new DXItemGrid
            {
                GridSize = new Size(2, 1),
                Parent = this,
                GridType = GridType.AccessoryRefineCombItems,
                Linked = true
            };
            Grid.Location = new Point(Materialslabel.Location.X, Materialslabel.Location.Y + Materialslabel.Size.Height + 5);


            foreach (DXItemCell cell in TargetCell.Grid)
            {
                cell.LinkChanged += (o, e) => SetRefine();
            }
            foreach (DXItemCell cell in OreTargetCell.Grid)
            {
                cell.LinkChanged += (o, e) => SetRefine();
            }
            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.LinkChanged += (o, e) => CalculateCost();
            }
            #region RefineOptions


            DCCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "DC" },
                ReadOnly = true,
            };
            DCCheckBox.MouseClick += (o, e) => RefineType = RefineType.DC;

            SPCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Spell Power" },
                ReadOnly = true,
            };
            SPCheckBox.MouseClick += (o, e) => RefineType = RefineType.SpellPower;

            FireCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Fire" },
                ReadOnly = true,
            };
            FireCheckBox.MouseClick += (o, e) => RefineType = RefineType.Fire;

            IceCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Ice" },
                ReadOnly = true,
            };
            IceCheckBox.MouseClick += (o, e) => RefineType = RefineType.Ice;

            LightningCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Lightning" },
                ReadOnly = true,
            };
            LightningCheckBox.MouseClick += (o, e) => RefineType = RefineType.Lightning;

            WindCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Wind" },
                ReadOnly = true,
            };
            WindCheckBox.MouseClick += (o, e) => RefineType = RefineType.Wind;

            HolyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Holy" },
                ReadOnly = true,
            };
            HolyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Holy;

            DarkCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Dark" },
                ReadOnly = true,
            };
            DarkCheckBox.MouseClick += (o, e) => RefineType = RefineType.Dark;

            PhantomCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Phantom" },
                ReadOnly = true,
            };
            PhantomCheckBox.MouseClick += (o, e) => RefineType = RefineType.Phantom;

            HealthCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Health" },
                ReadOnly = true,
            };
            HealthCheckBox.MouseClick += (o, e) => RefineType = RefineType.Health;

            ManaCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Mana" },
                ReadOnly = true,
            };
            ManaCheckBox.MouseClick += (o, e) => RefineType = RefineType.Mana;

            ACCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "AC" },
                ReadOnly = true,
            };
            ACCheckBox.MouseClick += (o, e) => RefineType = RefineType.AC;

            MRCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "MR" },
                ReadOnly = true,
            };
            MRCheckBox.MouseClick += (o, e) => RefineType = RefineType.MR;

            AccuracyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Accuracy" },
                ReadOnly = true,
            };
            AccuracyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Accuracy;

            AgilityCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Agility" },
                ReadOnly = true,
            };
            AgilityCheckBox.MouseClick += (o, e) => RefineType = RefineType.Agility;


            DCCheckBox.Location = new Point(ClientArea.Right - DCCheckBox.Size.Width - 280, ClientArea.Y + 22);
            SPCheckBox.Location = new Point(ClientArea.Right - SPCheckBox.Size.Width - 186, ClientArea.Y + 22);
            HealthCheckBox.Location = new Point(ClientArea.Right - HealthCheckBox.Size.Width - 101, ClientArea.Y + 22);
            ManaCheckBox.Location = new Point(ClientArea.Right - ManaCheckBox.Size.Width - 15, ClientArea.Y + 22);

            ACCheckBox.Location = new Point(ClientArea.Right - ACCheckBox.Size.Width - 280, ClientArea.Y + 39);
            MRCheckBox.Location = new Point(ClientArea.Right - MRCheckBox.Size.Width - 186, ClientArea.Y + 39);
            AccuracyCheckBox.Location = new Point(ClientArea.Right - AccuracyCheckBox.Size.Width - 101, ClientArea.Y + 39);
            AgilityCheckBox.Location = new Point(ClientArea.Right - AgilityCheckBox.Size.Width - 15, ClientArea.Y + 39);


            new DXLabel
            {
                Text = "Attack Element",
                Location = new Point(ClientArea.Right - HealthCheckBox.Size.Width - 150, ClientArea.Y + 73),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            new DXLabel
            {
                Text = "Refine Options",
                Location = new Point(ClientArea.Right - HealthCheckBox.Size.Width - 150, ClientArea.Y + 5),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };


            FireCheckBox.Location = new Point(ClientArea.Right - FireCheckBox.Size.Width - 280, ClientArea.Y + 90);
            IceCheckBox.Location = new Point(ClientArea.Right - IceCheckBox.Size.Width - 186, ClientArea.Y + 90);
            LightningCheckBox.Location = new Point(ClientArea.Right - LightningCheckBox.Size.Width - 101, ClientArea.Y + 90);
            WindCheckBox.Location = new Point(ClientArea.Right - WindCheckBox.Size.Width - 15, ClientArea.Y + 90);
            HolyCheckBox.Location = new Point(ClientArea.Right - HolyCheckBox.Size.Width - 280, ClientArea.Y + 105);
            DarkCheckBox.Location = new Point(ClientArea.Right - DarkCheckBox.Size.Width - 186, ClientArea.Y + 105);
            PhantomCheckBox.Location = new Point(ClientArea.Right - PhantomCheckBox.Size.Width - 101, ClientArea.Y + 105);


            #endregion


            CostLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "50000",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "Refine Cost:",
                Size = new Size(79, 20),
                IsControl = false,
            };
            RefineButton = new DXButton
            {
                Label = { Text = "Refine" },
                Location = new Point(ClientArea.Right - 80, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            RefineButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                ;
                int count = 0;
                List<CellLinkInfo> links = new List<CellLinkInfo>();

                DXItemCell target = TargetCell.Grid[0];
                DXItemCell oretarget = OreTargetCell.Grid[0];

                if (target.Link == null) return;

                CellLinkInfo targetLink = new CellLinkInfo { Count = target.LinkedCount, GridType = target.Link.GridType, Slot = target.Link.Slot };
                target.Link.Locked = true;

                if (oretarget.Link == null)
                {
                    target.Link.Locked = false;
                    return;
                }

                CellLinkInfo oretargetLink = new CellLinkInfo { Count = oretarget.LinkedCount, GridType = oretarget.Link.GridType, Slot = oretarget.Link.Slot };
                oretarget.Link.Locked = true;

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                    count++;
                }
                if (count != 2)
                {
                    oretarget.Link.Locked = false;
                    target.Link.Locked = false;
                    foreach (DXItemCell cell in Grid.Grid)
                    {
                        if (cell.Link == null) continue;
                        cell.Link.Locked = false;
                    }
                    return;
                }

                target.Link = null;

                oretarget.Link = null;

                CEnvir.Enqueue(new C.NPCAccessoryRefine { Target = targetLink, OreTarget = oretargetLink, Links = links, RefineType = RefineType });
            };
        }

        #region Methods
        public void SetRefine()
        {

            RefineButton.Enabled = false;
            if (RefineType.ToString() == "None")
            {
                RefineButton.Enabled = false;
                return;
            }
            foreach (DXItemCell cell in TargetCell.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in OreTargetCell.Grid)
            {
                if (cell.Link == null) return;
            }
            int count = 0;

            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link == null) continue;

                count++;
            }
            DXItemCell target = TargetCell.Grid[0];
            DXItemCell oretarget = OreTargetCell.Grid[0];

            if (count != 2)
            {
                RefineButton.Enabled = false;
                return;
            }

            if (MapObject.User.Gold.Amount < 50000)
            {
                RefineButton.Enabled = false;
                return;
            }

            RefineButton.Enabled = true;
        }
        private void CalculateCost()
        {

            int amount = 50000;

            CostLabel.ForeColour = amount > MapObject.User.Gold.Amount ? Color.Red : Color.White;

            CostLabel.Text = amount.ToString("#,##0");

            SetRefine();
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TargetCell != null)
                {
                    if (!TargetCell.IsDisposed)
                        TargetCell.Dispose();

                    TargetCell = null;
                }
                if (OreTargetCell != null)
                {
                    if (!OreTargetCell.IsDisposed)
                        OreTargetCell.Dispose();

                    OreTargetCell = null;
                }
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (RefineButton != null)
                {
                    if (!RefineButton.IsDisposed)
                        RefineButton.Dispose();

                    RefineButton = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }

        }

        #endregion
    }

    public class NPCRollDialog : DXControl
    {
        private readonly DXAnimatedControl _animation;
        private readonly DXImageControl _image;

        private int _currentLoop;
        private int _type;
        private int _result;

        private bool _rolled;
        private bool _rolling;

        public NPCRollDialog()
        {
            Movable = false;
            Sort = true;

            _animation = new DXAnimatedControl
            {
                Parent = this,
                Index = 0,
                LibraryFile = LibraryFile.MiniGames,
                UseOffSet = true,
                Location = new Point(0, 0),
                Visible = true
            };
            _animation.AfterAnimationLoop += (o, e) =>
            {
                switch (_type)
                {
                    case 0: //Die
                        {
                            if (_currentLoop < 5)
                            {
                                _currentLoop++;
                                return;
                            }

                            _image.Visible = true;
                            _animation.Visible = false;
                            _animation.Animated = false;
                            ReturnResult();
                        }
                        break;
                    case 1: //Yut
                        {
                            _image.Visible = true;
                            _animation.Visible = false;
                            _animation.Animated = false;
                            ReturnResult();
                        }
                        break;
                }

            };

            _image = new DXImageControl
            {
                Parent = this,
                Index = 0,
                LibraryFile = LibraryFile.MiniGames,
                UseOffSet = true,
                Location = new Point(0, 0),
                Visible = false
            };
            _image.MouseClick += _image_Click;
        }

        public void Setup(int type, int result, bool autoRoll)
        {
            _type = type;
            _result = result;

            _rolled = false;

            _currentLoop = 0;
            Visible = true;

            switch (type)
            {
                case 0: //Die
                    {
                        Size = new Size(65, 65);
                        Location = new Point((GameScene.ActiveScene.Size.Width / 2) - 38, (GameScene.ActiveScene.Size.Height / 2) - 40);

                        _image.Index = 12;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = true;

                        _animation.Loop = true;
                        _animation.Visible = false;
                        _animation.Animated = false;
                    }
                    break;
                case 1: //Yut
                    {
                        Size = new Size(180, 210);
                        Location = new Point((GameScene.ActiveScene.Size.Width / 2) - 90, (GameScene.ActiveScene.Size.Height / 2) - 65);

                        _image.Index = 100;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = true;

                        _animation.Loop = false;
                        _animation.Visible = false;
                        _animation.Animated = false;
                    }
                    break;
            }
        }

        private void Roll()
        {
            Visible = true;

            _rolling = true;

            switch (_type)
            {
                case 0: //Die
                    {
                        _image.Index = 11 + _result;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = false;

                        _animation.BaseIndex = 20;
                        _animation.LibraryFile = LibraryFile.MiniGames;
                        _animation.FrameCount = 4;
                        _animation.AnimationDelay = TimeSpan.FromMilliseconds(400);
                        _animation.AnimationStart = DateTime.MinValue;
                        _animation.Loop = true;
                        _animation.Visible = true;
                        _animation.Animated = true;

                        DXSoundManager.Play(SoundIndex.RollDice);
                    }
                    break;
                case 1: //Yut
                    {
                        _image.Index = 106 + _result;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = false;

                        _animation.BaseIndex = 100;
                        _animation.LibraryFile = LibraryFile.MiniGames;
                        _animation.FrameCount = 6;
                        _animation.AnimationDelay = TimeSpan.FromMilliseconds(600);
                        _animation.AnimationStart = DateTime.MinValue;
                        _animation.Loop = false;
                        _animation.Visible = true;
                        _animation.Animated = true;

                        DXSoundManager.Play(SoundIndex.RollYut);
                    }
                    break;
            }
        }

        private void _image_Click(object sender, EventArgs e)
        {
            if (_rolling) return;

            if (_rolled)
            {
                Hide();
                return;
            }

            Roll();
        }

        private void Hide()
        {
            Visible = false;
        }

        private void ReturnResult()
        {
            _rolling = false;
            _rolled = true;

            CEnvir.Enqueue(new C.NPCRollResult());
        }
    }
}