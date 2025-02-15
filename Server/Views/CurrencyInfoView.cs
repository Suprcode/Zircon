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
            CurrencyCategoryImageComboBox.Items.AddEnum<CurrencyCategory>();
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding.Where(x => x.ItemType == ItemType.Currency);
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

            #region Gold
            var gold = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.Gold);
            var goldItem = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemName == "Gold");

            if (goldItem == null)
            {
                goldItem = SMain.Session.GetCollection<ItemInfo>().CreateNewObject();
                goldItem.ItemName = "Gold";
                goldItem.ItemType = ItemType.Currency;
                goldItem.StackSize = 25000;
                goldItem.Image = 121;
                goldItem.SellRate = 0;
                goldItem.CanDrop = true;
            }

            if (gold == null)
            {
                gold = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                gold.Name = "Gold";
                gold.Type = CurrencyType.Gold;
                gold.Category = CurrencyCategory.Basic;
                needSave = true;

                if (goldItem != null)
                {
                    gold.DropItem = goldItem;
                    goldItem.ItemEffect = ItemEffect.None;
                    goldItem.ItemType = ItemType.Currency;
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
            #endregion

            #region GameGold
            var gameGold = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.GameGold);

            if (gameGold == null)
            {
                gameGold = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                gameGold.Name = "Game Gold";
                gameGold.Type = CurrencyType.GameGold;
                gameGold.Category = CurrencyCategory.Other;
                needSave = true;
            }

            if (string.IsNullOrEmpty(gameGold.Abbreviation))
            {
                gameGold.Abbreviation = "GG";
                needSave = true;
            }
            #endregion

            #region HuntGold
            var huntGold = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.HuntGold);

            if (huntGold == null)
            {
                huntGold = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                huntGold.Name = "Hunt Gold";
                huntGold.Type = CurrencyType.HuntGold;
                huntGold.Category = CurrencyCategory.Other;
                needSave = true;
            }

            if (string.IsNullOrEmpty(huntGold.Abbreviation))
            {
                huntGold.Abbreviation = "HG";
                needSave = true;
            }
            #endregion

            #region FamePoint
            var fp = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.FP);
            var fpItem = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemName == "Fame Point");

            if (fpItem == null)
            {
                fpItem = SMain.Session.GetCollection<ItemInfo>().CreateNewObject();
                fpItem.ItemName = "Fame Point";
                fpItem.ItemType = ItemType.Currency;
                fpItem.StackSize = 25000;
                fpItem.Image = 4010;
                fpItem.SellRate = 0;
                fpItem.CanDrop = false;
            }

            if (fp == null)
            {
                fp = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                fp.Name = "Fame Point";
                fp.Abbreviation = "FP";
                fp.Type = CurrencyType.FP;
                fp.Category = CurrencyCategory.Player;
                fp.DropItem = fpItem;
                needSave = true;
            }
            #endregion

            #region ContributionPoint
            var cp = SMain.Session.GetCollection<CurrencyInfo>().Binding.FirstOrDefault(x => x.Type == CurrencyType.CP);
            var cpItem = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemName == "Contribution Point");

            if (cpItem == null)
            {
                cpItem = SMain.Session.GetCollection<ItemInfo>().CreateNewObject();
                cpItem.ItemName = "Contribution Point";
                cpItem.ItemType = ItemType.Currency;
                cpItem.StackSize = 25000;
                cpItem.Image = 4012;
                cpItem.CanDrop = false;
            }

            if (cp == null)
            {
                cp = SMain.Session.GetCollection<CurrencyInfo>().CreateNewObject();
                cp.Name = "Contribution Point";
                cp.Abbreviation = "CP";
                cp.Type = CurrencyType.CP;
                cp.Category = CurrencyCategory.Player;
                cp.DropItem = cpItem;
                needSave = true;
            }
            #endregion

            //Make sure all the items used are currency items
            foreach (var currency in SMain.Session.GetCollection<CurrencyInfo>().Binding)
            {
                if (currency.DropItem != null)
                {
                    currency.DropItem.ItemType = ItemType.Currency;
                }
            }

            if (needSave)
            {
                SMain.Session.Save(true);
            }
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
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