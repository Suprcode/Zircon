using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Client.Controls
{
    public sealed class DXCheckedComboBox : DXComboBox
    {
        public readonly List<DXCheckedListBoxItem> Items = new();
        public readonly List<DXCheckedComboBoxSection> Sections = new();

        public string EmptyText { get; set; } = string.Empty;
        public string SelectedCountFormat { get; set; } = "{0}";
        public int HintMaximumWidth { get; set; } = 450;
        public bool ReadOnly { get; set; }

        public IEnumerable<object> CheckedItems => Items.Where(x => x.Checked).Select(x => x.Item);

        public event EventHandler<EventArgs> CheckedItemsChanged;

        public DXCheckedComboBox()
        {
            SelectedLabel.MouseClick += ToggleDropDown;
        }

        public DXCheckedListBoxItem AddItem(object item, string text, bool isChecked = false)
        {
            return AddItem(null, item, text, isChecked);
        }

        public DXCheckedListBoxItem AddEnumItem(Enum item, bool isChecked = false)
        {
            return AddItem(item, GetEnumDisplayText(item), isChecked);
        }

        public static string GetEnumDisplayText(Enum value)
        {
            if (value == null) return string.Empty;

            MemberInfo member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }

        public DXCheckedComboBoxSection AddSection(string text)
        {
            DXCheckedComboBoxSection section = new DXCheckedComboBoxSection(this, text);
            Sections.Add(section);
            return section;
        }

        internal DXCheckedListBoxItem AddItem(DXCheckedComboBoxSection section, object item, string text, bool isChecked)
        {
            DXCheckedListBoxItem listItem = new DXCheckedListBoxItem
            {
                Owner = this,
                Section = section,
                Item = item,
                Text = text,
                Checked = isChecked,
                Parent = ListBox,
            };

            listItem.CheckedChanged += ListItem_CheckedChanged;
            Items.Add(listItem);
            section?.Items.Add(listItem);
            RefreshSelectionDisplay();
            return listItem;
        }

        public void SetItemChecked(object item, bool isChecked)
        {
            DXCheckedListBoxItem listItem = Items.FirstOrDefault(x => Equals(x.Item, item));
            if (listItem != null)
                listItem.Checked = isChecked;
        }

        public bool IsItemChecked(object item)
        {
            return Items.Any(x => Equals(x.Item, item) && x.Checked);
        }

        private void ToggleDropDown(object sender, MouseEventArgs e)
        {
            Showing = !Showing;
        }

        private void ListItem_CheckedChanged(object sender, EventArgs e)
        {
            RefreshSelectionDisplay();
            CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshSelectionDisplay()
        {
            List<string> names = Items.Where(x => x.Checked).Select(x => x.Text).ToList();

            SelectedLabel.Text = names.Count switch
            {
                0 => EmptyText,
                1 => names[0],
                _ => string.Format(SelectedCountFormat, names.Count),
            };

            List<string> hintSections = new List<string>();
            foreach (DXCheckedComboBoxSection section in Sections)
            {
                List<string> sectionNames = section.Items.Where(x => x.Checked).Select(x => x.Text).ToList();
                if (sectionNames.Count > 0)
                    hintSections.Add(BuildHintSection(section.Text, sectionNames, HintMaximumWidth));
            }

            List<string> unsectionedNames = Items.Where(x => x.Section == null && x.Checked).Select(x => x.Text).ToList();
            if (unsectionedNames.Count > 0)
                hintSections.Add(BuildHintSection(null, unsectionedNames, HintMaximumWidth));

            string hint = hintSections.Count == 0 ? EmptyText : string.Join(Environment.NewLine, hintSections);
            Hint = null;
            SelectedLabel.Hint = hint;
            DownArrow.Hint = null;
        }

        public static string BuildHintSection(string heading, IEnumerable<string> names, int hintMaximumWidth = 450)
        {
            string firstPrefix = string.IsNullOrEmpty(heading) ? string.Empty : $"{heading}: ";
            string continuationPrefix = string.IsNullOrEmpty(heading) ? string.Empty : "  ";
            string line = firstPrefix;
            int itemsOnLine = 0;
            List<string> lines = new();

            foreach (string name in names)
            {
                string candidate = line + (itemsOnLine == 0 ? string.Empty : ", ") + name;
                bool exceedsWidth = hintMaximumWidth > 0 &&
                                    DXLabel.GetSize(candidate, HintLabel.Font, HintLabel.Outline).Width > hintMaximumWidth;

                if (itemsOnLine > 0 && exceedsWidth)
                {
                    lines.Add(line);
                    line = continuationPrefix + name;
                    itemsOnLine = 1;
                    continue;
                }

                line = candidate;
                itemsOnLine++;
            }

            if (itemsOnLine > 0)
                lines.Add(line);

            return string.Join(Environment.NewLine, lines);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            if (SelectedLabel != null)
                SelectedLabel.MouseClick -= ToggleDropDown;

            foreach (DXCheckedListBoxItem item in Items)
            {
                if (item == null) continue;

                item.CheckedChanged -= ListItem_CheckedChanged;
                if (!item.IsDisposed)
                    item.Dispose();
            }

            Items.Clear();

            foreach (DXCheckedComboBoxSection section in Sections)
                section.Dispose();

            Sections.Clear();
            CheckedItemsChanged = null;
        }
    }

    public sealed class DXCheckedComboBoxSection : IDisposable
    {
        private DXCheckedComboBox _owner;

        public string Text { get; }
        public DXCheckedListBoxSection Header { get; private set; }
        public readonly List<DXCheckedListBoxItem> Items = new();

        internal DXCheckedComboBoxSection(DXCheckedComboBox owner, string text)
        {
            _owner = owner;
            Text = text;
            Header = new DXCheckedListBoxSection { Text = text, Parent = owner.ListBox };
        }

        public DXCheckedListBoxItem AddItem(object item, string text, bool isChecked = false)
        {
            return _owner.AddItem(this, item, text, isChecked);
        }

        public DXCheckedListBoxItem AddEnumItem(Enum item, bool isChecked = false)
        {
            return _owner.AddItem(this, item, DXCheckedComboBox.GetEnumDisplayText(item), isChecked);
        }

        public void Dispose()
        {
            _owner = null;
            Items.Clear();

            if (Header != null)
            {
                if (!Header.IsDisposed)
                    Header.Dispose();

                Header = null;
            }
        }
    }

    public sealed class DXCheckedListBoxSection : DXListBoxItem
    {
        public override void OnTextChanged(string oValue, string nValue)
        {
            base.OnTextChanged(oValue, nValue);

            if (Label != null)
                Label.Text = nValue;
        }

        public DXCheckedListBoxSection()
        {
            Label.Location = new Point(5, 1);
            Label.ForeColour = Color.Goldenrod;
            Font oldFont = Label.Font;
            Label.Font = new Font(Label.Font.FontFamily, Label.Font.Size, FontStyle.Bold);
            oldFont.Dispose();
            BackColour = Color.FromArgb(32, 16, 16);
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
        }

        public override void UpdateColours()
        {
            Label.ForeColour = Color.Goldenrod;
            BackColour = Color.FromArgb(32, 16, 16);
        }
    }

    public sealed class DXCheckedListBoxItem : DXListBoxItem
    {
        public DXCheckedComboBox Owner;
        public DXCheckedComboBoxSection Section;
        public DXImageControl CheckBox;

        public override void OnTextChanged(string oValue, string nValue)
        {
            base.OnTextChanged(oValue, nValue);

            if (Label != null)
                Label.Text = nValue;
        }

        public bool Checked
        {
            get => _Checked;
            set
            {
                if (_Checked == value) return;

                _Checked = value;
                CheckBox.Index = Checked ? 162 : 161;
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool _Checked;

        public event EventHandler<EventArgs> CheckedChanged;

        public DXCheckedListBoxItem()
        {
            Label.Location = new Point(20, 0);

            CheckBox = new DXImageControl
            {
                Parent = this,
                Location = new Point(2, 1),
                LibraryFile = LibraryFile.GameInter,
                Index = 161,
                IsControl = false,
            };
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (Owner == null || Owner.ReadOnly) return;

            Checked = !Checked;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Owner = null;
            Section = null;
            CheckedChanged = null;
            _Checked = false;

            if (CheckBox != null)
            {
                if (!CheckBox.IsDisposed)
                    CheckBox.Dispose();

                CheckBox = null;
            }
        }
    }
}
