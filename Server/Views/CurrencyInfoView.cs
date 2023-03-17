using System;
using System.Linq;
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
            SMain.SetUpView(CurrencyInfoImageGridView);
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
                    goldItem.ItemEffect = ItemEffect.None;
                }
            }

            if (string.IsNullOrEmpty(gold.Abbreviation))
            {
                gold.Abbreviation = "Gold";
                needSave = true;
            }

            if (gold.Images.Count == 0)
            {
                var image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 120;
                image.Amount = 0;
                gold.Images.Add(image);

                image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 121;
                image.Amount = 100;
                gold.Images.Add(image);

                image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 122;
                image.Amount = 200;
                gold.Images.Add(image);

                image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 123;
                image.Amount = 500;
                gold.Images.Add(image);

                image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 124;
                image.Amount = 1000;
                gold.Images.Add(image);

                image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 125;
                image.Amount = 1000000;
                gold.Images.Add(image);

                image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 126;
                image.Amount = 5000000;
                gold.Images.Add(image);

                image = SMain.Session.GetCollection<CurrencyInfoImage>().CreateNewObject();
                image.Image = 127;
                image.Amount = 10000000;
                gold.Images.Add(image);

                needSave = true;
            }

            var gameGold = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.GameGold);

            if (gameGold == null)
            {
                gameGold = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                gameGold.Name = "Game Gold";
                gameGold.Type = CurrencyType.GameGold;
                needSave = true;
            }

            if (string.IsNullOrEmpty(gameGold.Abbreviation))
            {
                gameGold.Abbreviation = "GG";
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

            if (string.IsNullOrEmpty(huntGold.Abbreviation))
            {
                huntGold.Abbreviation = "HG";
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

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<CurrencyInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<CurrencyInfo>(CurrencyInfoGridView);
        }
    }
}