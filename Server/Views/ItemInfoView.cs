using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using Server.Envir;

namespace Server.Views
{
    public partial class ItemInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ItemInfoView()
        {
            InitializeComponent();

            ItemInfoGridControl.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            SetLookUpEdit.DataSource = SMain.Session.GetCollection<SetInfo>().Binding;

            ItemTypeImageComboBox.Items.AddEnum<ItemType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            RequiredGenderImageComboBox.Items.AddEnum<RequiredGender>();
            StatImageComboBox.Items.AddEnum<Stat>();
            RequiredTypeImageComboBox.Items.AddEnum<RequiredType>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(ItemInfoGridView);
            SMain.SetUpView(ItemStatsGridView);
            SMain.SetUpView(DropsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<string> itemData = new List<string>();

            itemData.Add(GetLine(null));
            foreach (ItemInfo item in SMain.Session.GetCollection<ItemInfo>().Binding)
                itemData.Add(GetLine(item));

            string path = @"C:\Zircon Server\Data Works\Exports\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllLines(path + "Items.csv", itemData);
        }

        public string GetLine(ItemInfo info)
        {
            StringBuilder builder = new StringBuilder();

            
            builder.Append((info?.ItemName ?? "Name") + ", ");
            builder.Append((info?.ItemType.ToString() ?? "Type") + ", ");

            if (info == null)
            {
                builder.Append("Required Class");
            }
            else
            {
                Type type = info.RequiredClass.GetType();

                MemberInfo[] infos = type.GetMember(info.RequiredClass.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                builder.Append(description?.Description.Replace(",", "") ?? info.RequiredClass.ToString());
            }

            builder.Append(", ");

            builder.Append((info?.RequiredType.ToString() ?? "Required Type") + ", ");
            builder.Append((info?.RequiredAmount.ToString() ?? "Required Amount") + ", ");
            builder.Append((info?.Image.ToString("00000") ?? "Image") + ", ");
            builder.Append((info?.Weight.ToString() ?? "Weight") + ", ");
            builder.Append((info?.Durability.ToString() ?? "Durability") + ", ");
            builder.Append((info?.Rarity.ToString() ?? "Rarity") + ", ");

            builder.Append((info == null ? "AC" : string.Format("{0}-{1}", info.Stats[Stat.MinAC], info.Stats[Stat.MaxAC])) + ", ");
            builder.Append((info == null ? "MR" : string.Format("{0}-{1}", info.Stats[Stat.MinMR], info.Stats[Stat.MaxMR])) + ", ");
            builder.Append((info == null ? "DC" : string.Format("{0}-{1}", info.Stats[Stat.MinDC], info.Stats[Stat.MaxDC])) + ", ");
            builder.Append((info == null ? "MC" : string.Format("{0}-{1}", info.Stats[Stat.MinMC], info.Stats[Stat.MaxMC])) + ", ");
            builder.Append((info == null ? "SC" : string.Format("{0}-{1}", info.Stats[Stat.MinSC], info.Stats[Stat.MaxSC])) + ", ");
            builder.Append((info == null ? "Accuracy" : string.Format("+{0}", info.Stats[Stat.Accuracy])) + ", ");
            builder.Append((info == null ? "Agility" : string.Format("+{0}", info.Stats[Stat.Agility])) + ", ");
            builder.Append((info == null ? "Attack Speed" : string.Format("+{0}", info.Stats[Stat.AttackSpeed])) + ", ");
            builder.Append((info == null ? "Health" : string.Format("+{0}", info.Stats[Stat.Health])) + ", ");
            builder.Append((info == null ? "Mana" : string.Format("+{0}", info.Stats[Stat.Mana])) + ", ");
            builder.Append((info == null ? "Luck" : string.Format("+{0}", info.Stats[Stat.Luck])) + ", ");
            builder.Append((info == null ? "Experienc eRate" : string.Format("+{0}", info.Stats[Stat.ExperienceRate])) + ", ");
            builder.Append((info == null ? "Drop Rate" : string.Format("+{0}", info.Stats[Stat.DropRate])) + ", ");
            builder.Append((info == null ? "Gold Rate" : string.Format("+{0}", info.Stats[Stat.GoldRate])) + ", ");
            builder.Append((info == null ? "Skill Rate" : string.Format("+{0}", info.Stats[Stat.SkillRate])) + ", ");
            builder.Append((info == null ? "Duration" : string.Format("+{0}", info.Stats[Stat.Duration])) + ", ");
            builder.Append((info == null ? "Inventory Weight" : string.Format("+{0}", info.Stats[Stat.BagWeight])) + ", ");
            builder.Append((info == null ? "Wear Weight" : string.Format("+{0}", info.Stats[Stat.WearWeight])) + ", ");
            builder.Append((info == null ? "Hand Weight" : string.Format("+{0}", info.Stats[Stat.HandWeight])) + ", ");
            builder.Append((info == null ? "Life Steal" : string.Format("+{0}", info.Stats[Stat.LifeSteal])) + ", ");
            builder.Append((info == null ? "Fire Attack" : string.Format("+{0}", info.Stats[Stat.FireAttack])) + ", ");
            builder.Append((info == null ? "Ice Attack" : string.Format("+{0}", info.Stats[Stat.IceAttack])) + ", ");
            builder.Append((info == null ? "Lightning Attack" : string.Format("+{0}", info.Stats[Stat.LightningAttack])) + ", ");
            builder.Append((info == null ? "Wind Attack" : string.Format("+{0}", info.Stats[Stat.WindAttack])) + ", ");
            builder.Append((info == null ? "Holy Attack" : string.Format("+{0}", info.Stats[Stat.HolyAttack])) + ", ");
            builder.Append((info == null ? "Dark Attack" : string.Format("+{0}", info.Stats[Stat.DarkAttack])) + ", ");
            builder.Append((info == null ? "Phantom Attack" : string.Format("+{0}", info.Stats[Stat.PhantomAttack])) + ", ");


            return builder.ToString();
        }

    }
}