using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Library.SystemModels;
using Library;

namespace Server.Views
{
    public partial class CurrencyInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CurrencyInfoView()
        {
            InitializeComponent();

            CurrencyInfoGridControl.DataSource = SMain.Session.GetCollection<CurrencyInfo>().Binding;

            CurrencyTypeImageComboBox.Items.AddEnum<CurrencyType>();
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CurrencyInfoGridView);
        }

        public static void AddDefaultCurrencies()
        {
            bool needSave = false;

            var gold = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.Gold);

            if (gold == null)
            {
                var goldItem = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemName == "Gold");

                gold = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                gold.Name = "Gold";
                gold.Type = CurrencyType.Gold;
                needSave = true;

                if (goldItem != null)
                {
                    gold.DropItem = goldItem;
                    goldItem.Effect = ItemEffect.None;
                }
            }

            var gameGold = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.GameGold);

            if (gameGold == null)
            {
                gameGold = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                gameGold.Name = "Game Gold";
                gameGold.Type = CurrencyType.GameGold;
                needSave = true;
            }

            var huntGold = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.HuntGold);

            if (huntGold == null)
            {
                huntGold = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                huntGold.Name = "Hunt Gold";
                huntGold.Type = CurrencyType.HuntGold;
                needSave = true;
            }

            if (needSave)
            {
                SMain.Session.Save(true);
            }
        }

        private void SavingButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}