using DevExpress.XtraGrid.Columns;

namespace Server.Views
{
    partial class ItemInfoView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemInfoView));
            ItemStatsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colStat = new GridColumn();
            StatImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            colAmount = new GridColumn();
            ItemInfoGridControl = new DevExpress.XtraGrid.GridControl();
            DropsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colMonster = new GridColumn();
            MonsterLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            colChance = new GridColumn();
            colDAmount = new GridColumn();
            gridColumn3 = new GridColumn();
            gridColumn8 = new GridColumn();
            ItemInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colIndex = new GridColumn();
            colItemName = new GridColumn();
            colItemType = new GridColumn();
            ItemTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            colRequiredClass = new GridColumn();
            RequiredClassImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            colRequiredGender = new GridColumn();
            RequiredGenderImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn1 = new GridColumn();
            RequiredTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn2 = new GridColumn();
            colShape = new GridColumn();
            colItemEffect = new GridColumn();
            colExteriorEffect = new GridColumn();
            colImage = new GridColumn();
            colWeight = new GridColumn();
            colDurability = new GridColumn();
            colPrice = new GridColumn();
            colStackSize = new GridColumn();
            colSellRate = new GridColumn();
            colStartItem = new GridColumn();
            colCanRepair = new GridColumn();
            colCanSell = new GridColumn();
            colCanStore = new GridColumn();
            colCanTrade = new GridColumn();
            colCanDrop = new GridColumn();
            ColCanDeathDrop = new GridColumn();
            gridColumn4 = new GridColumn();
            colRarity = new GridColumn();
            colDescription = new GridColumn();
            gridColumn5 = new GridColumn();
            SetLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn6 = new GridColumn();
            gridColumn7 = new GridColumn();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)ItemStatsGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StatImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DropsGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MonsterLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemTypeImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RequiredClassImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RequiredGenderImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RequiredTypeImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SetLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            SuspendLayout();
            // 
            // ItemStatsGridView
            // 
            ItemStatsGridView.Columns.AddRange(new GridColumn[] { colStat, colAmount });
            ItemStatsGridView.GridControl = ItemInfoGridControl;
            ItemStatsGridView.Name = "ItemStatsGridView";
            ItemStatsGridView.OptionsView.EnableAppearanceEvenRow = true;
            ItemStatsGridView.OptionsView.EnableAppearanceOddRow = true;
            ItemStatsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            ItemStatsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            ItemStatsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colStat
            // 
            colStat.ColumnEdit = StatImageComboBox;
            colStat.FieldName = "Stat";
            colStat.Name = "colStat";
            colStat.Visible = true;
            colStat.VisibleIndex = 0;
            // 
            // StatImageComboBox
            // 
            StatImageComboBox.AutoHeight = false;
            StatImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            StatImageComboBox.Name = "StatImageComboBox";
            // 
            // colAmount
            // 
            colAmount.FieldName = "Amount";
            colAmount.Name = "colAmount";
            colAmount.Visible = true;
            colAmount.VisibleIndex = 1;
            // 
            // ItemInfoGridControl
            // 
            ItemInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = ItemStatsGridView;
            gridLevelNode1.RelationName = "ItemStats";
            gridLevelNode2.LevelTemplate = DropsGridView;
            gridLevelNode2.RelationName = "Drops";
            ItemInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1, gridLevelNode2 });
            ItemInfoGridControl.Location = new System.Drawing.Point(0, 144);
            ItemInfoGridControl.MainView = ItemInfoGridView;
            ItemInfoGridControl.MenuManager = ribbon;
            ItemInfoGridControl.Name = "ItemInfoGridControl";
            ItemInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { ItemTypeImageComboBox, RequiredClassImageComboBox, RequiredGenderImageComboBox, StatImageComboBox, RequiredTypeImageComboBox, MonsterLookUpEdit, SetLookUpEdit });
            ItemInfoGridControl.ShowOnlyPredefinedDetails = true;
            ItemInfoGridControl.Size = new System.Drawing.Size(747, 369);
            ItemInfoGridControl.TabIndex = 2;
            ItemInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { DropsGridView, ItemInfoGridView, ItemStatsGridView });
            // 
            // DropsGridView
            // 
            DropsGridView.Columns.AddRange(new GridColumn[] { colMonster, colChance, colDAmount, gridColumn3, gridColumn8 });
            DropsGridView.GridControl = ItemInfoGridControl;
            DropsGridView.Name = "DropsGridView";
            DropsGridView.OptionsView.EnableAppearanceEvenRow = true;
            DropsGridView.OptionsView.EnableAppearanceOddRow = true;
            DropsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            DropsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            DropsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colMonster
            // 
            colMonster.ColumnEdit = MonsterLookUpEdit;
            colMonster.FieldName = "Monster";
            colMonster.Name = "colMonster";
            colMonster.Visible = true;
            colMonster.VisibleIndex = 0;
            // 
            // MonsterLookUpEdit
            // 
            MonsterLookUpEdit.AutoHeight = false;
            MonsterLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            MonsterLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            MonsterLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AI", "AI"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Level", "Level"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Experience", "Experience"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "Is Boss") });
            MonsterLookUpEdit.DisplayMember = "MonsterName";
            MonsterLookUpEdit.Name = "MonsterLookUpEdit";
            MonsterLookUpEdit.NullText = "[Monster is null]";
            // 
            // colChance
            // 
            colChance.FieldName = "Chance";
            colChance.Name = "colChance";
            colChance.Visible = true;
            colChance.VisibleIndex = 1;
            // 
            // colDAmount
            // 
            colDAmount.FieldName = "Amount";
            colDAmount.Name = "colDAmount";
            colDAmount.Visible = true;
            colDAmount.VisibleIndex = 2;
            // 
            // gridColumn3
            // 
            gridColumn3.FieldName = "DropSet";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 3;
            // 
            // gridColumn8
            // 
            gridColumn8.FieldName = "PartOnly";
            gridColumn8.Name = "gridColumn8";
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 4;
            // 
            // ItemInfoGridView
            // 
            ItemInfoGridView.Columns.AddRange(new GridColumn[] { colIndex, colItemName, colItemType, colRequiredClass, colRequiredGender, gridColumn1, gridColumn2, colShape, colItemEffect, colExteriorEffect, colImage, colWeight, colDurability, colPrice, colStackSize, colSellRate, colStartItem, colCanRepair, colCanSell, colCanStore, colCanTrade, colCanDrop, ColCanDeathDrop, gridColumn4, colRarity, colDescription, gridColumn5, gridColumn6, gridColumn7 });
            ItemInfoGridView.GridControl = ItemInfoGridControl;
            ItemInfoGridView.Name = "ItemInfoGridView";
            ItemInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            ItemInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            ItemInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            ItemInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            ItemInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            ItemInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colIndex
            // 
            colIndex.FieldName = "Index";
            colIndex.Name = "colIndex";
            colIndex.Width = 48;
            // 
            // colItemName
            // 
            colItemName.FieldName = "ItemName";
            colItemName.Name = "colItemName";
            colItemName.Visible = true;
            colItemName.VisibleIndex = 0;
            colItemName.Width = 56;
            // 
            // colItemType
            // 
            colItemType.ColumnEdit = ItemTypeImageComboBox;
            colItemType.FieldName = "ItemType";
            colItemType.Name = "colItemType";
            colItemType.Visible = true;
            colItemType.VisibleIndex = 1;
            colItemType.Width = 24;
            // 
            // ItemTypeImageComboBox
            // 
            ItemTypeImageComboBox.AutoHeight = false;
            ItemTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ItemTypeImageComboBox.Name = "ItemTypeImageComboBox";
            // 
            // colRequiredClass
            // 
            colRequiredClass.ColumnEdit = RequiredClassImageComboBox;
            colRequiredClass.FieldName = "RequiredClass";
            colRequiredClass.Name = "colRequiredClass";
            colRequiredClass.Visible = true;
            colRequiredClass.VisibleIndex = 2;
            colRequiredClass.Width = 24;
            // 
            // RequiredClassImageComboBox
            // 
            RequiredClassImageComboBox.AutoHeight = false;
            RequiredClassImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RequiredClassImageComboBox.Name = "RequiredClassImageComboBox";
            // 
            // colRequiredGender
            // 
            colRequiredGender.ColumnEdit = RequiredGenderImageComboBox;
            colRequiredGender.FieldName = "RequiredGender";
            colRequiredGender.Name = "colRequiredGender";
            colRequiredGender.Visible = true;
            colRequiredGender.VisibleIndex = 3;
            colRequiredGender.Width = 24;
            // 
            // RequiredGenderImageComboBox
            // 
            RequiredGenderImageComboBox.AutoHeight = false;
            RequiredGenderImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RequiredGenderImageComboBox.Name = "RequiredGenderImageComboBox";
            // 
            // gridColumn1
            // 
            gridColumn1.ColumnEdit = RequiredTypeImageComboBox;
            gridColumn1.FieldName = "RequiredType";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 4;
            gridColumn1.Width = 24;
            // 
            // RequiredTypeImageComboBox
            // 
            RequiredTypeImageComboBox.AutoHeight = false;
            RequiredTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RequiredTypeImageComboBox.Name = "RequiredTypeImageComboBox";
            // 
            // gridColumn2
            // 
            gridColumn2.FieldName = "RequiredAmount";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 5;
            gridColumn2.Width = 24;
            // 
            // colShape
            // 
            colShape.FieldName = "Shape";
            colShape.Name = "colShape";
            colShape.Visible = true;
            colShape.VisibleIndex = 6;
            colShape.Width = 24;
            // 
            // colItemEffect
            // 
            colItemEffect.FieldName = "ItemEffect";
            colItemEffect.Name = "colItemEffect";
            colItemEffect.Visible = true;
            colItemEffect.VisibleIndex = 7;
            colItemEffect.Width = 24;
            // 
            // colExteriorEffect
            // 
            colExteriorEffect.FieldName = "ExteriorEffect";
            colExteriorEffect.Name = "colExteriorEffect";
            colExteriorEffect.Visible = true;
            colExteriorEffect.VisibleIndex = 9;
            colExteriorEffect.Width = 24;
            // 
            // colImage
            // 
            colImage.FieldName = "Image";
            colImage.Name = "colImage";
            colImage.Visible = true;
            colImage.VisibleIndex = 8;
            colImage.Width = 24;
            // 
            // colWeight
            // 
            colWeight.FieldName = "Weight";
            colWeight.Name = "colWeight";
            colWeight.Visible = true;
            colWeight.VisibleIndex = 10;
            colWeight.Width = 24;
            // 
            // colDurability
            // 
            colDurability.FieldName = "Durability";
            colDurability.Name = "colDurability";
            colDurability.Visible = true;
            colDurability.VisibleIndex = 11;
            colDurability.Width = 24;
            // 
            // colPrice
            // 
            colPrice.FieldName = "Price";
            colPrice.Name = "colPrice";
            colPrice.Visible = true;
            colPrice.VisibleIndex = 12;
            colPrice.Width = 24;
            // 
            // colStackSize
            // 
            colStackSize.FieldName = "StackSize";
            colStackSize.Name = "colStackSize";
            colStackSize.Visible = true;
            colStackSize.VisibleIndex = 13;
            colStackSize.Width = 24;
            // 
            // colSellRate
            // 
            colSellRate.FieldName = "SellRate";
            colSellRate.Name = "colSellRate";
            colSellRate.Visible = true;
            colSellRate.VisibleIndex = 14;
            colSellRate.Width = 24;
            // 
            // colStartItem
            // 
            colStartItem.FieldName = "StartItem";
            colStartItem.Name = "colStartItem";
            colStartItem.Visible = true;
            colStartItem.VisibleIndex = 15;
            colStartItem.Width = 24;
            // 
            // colCanRepair
            // 
            colCanRepair.FieldName = "CanRepair";
            colCanRepair.Name = "colCanRepair";
            colCanRepair.Visible = true;
            colCanRepair.VisibleIndex = 16;
            colCanRepair.Width = 24;
            // 
            // colCanSell
            // 
            colCanSell.FieldName = "CanSell";
            colCanSell.Name = "colCanSell";
            colCanSell.Visible = true;
            colCanSell.VisibleIndex = 17;
            colCanSell.Width = 24;
            // 
            // colCanStore
            // 
            colCanStore.FieldName = "CanStore";
            colCanStore.Name = "colCanStore";
            colCanStore.Visible = true;
            colCanStore.VisibleIndex = 18;
            colCanStore.Width = 24;
            // 
            // colCanTrade
            // 
            colCanTrade.FieldName = "CanTrade";
            colCanTrade.Name = "colCanTrade";
            colCanTrade.Visible = true;
            colCanTrade.VisibleIndex = 19;
            colCanTrade.Width = 24;
            // 
            // colCanDrop
            // 
            colCanDrop.FieldName = "CanDrop";
            colCanDrop.Name = "colCanDrop";
            colCanDrop.Visible = true;
            colCanDrop.VisibleIndex = 20;
            colCanDrop.Width = 24;
            // 
            // ColCanDeathDrop
            // 
            ColCanDeathDrop.FieldName = "CanDeathDrop";
            ColCanDeathDrop.Name = "ColCanDeathDrop";
            ColCanDeathDrop.Visible = true;
            ColCanDeathDrop.VisibleIndex = 21;
            ColCanDeathDrop.Width = 24;
            // 
            // gridColumn4
            // 
            gridColumn4.FieldName = "CanAutoPot";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 22;
            gridColumn4.Width = 24;
            // 
            // colRarity
            // 
            colRarity.FieldName = "Rarity";
            colRarity.Name = "colRarity";
            colRarity.Visible = true;
            colRarity.VisibleIndex = 23;
            colRarity.Width = 24;
            // 
            // colDescription
            // 
            colDescription.FieldName = "Description";
            colDescription.Name = "colDescription";
            colDescription.Visible = true;
            colDescription.VisibleIndex = 24;
            colDescription.Width = 24;
            // 
            // gridColumn5
            // 
            gridColumn5.ColumnEdit = SetLookUpEdit;
            gridColumn5.FieldName = "Set";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 25;
            gridColumn5.Width = 24;
            // 
            // SetLookUpEdit
            // 
            SetLookUpEdit.AutoHeight = false;
            SetLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            SetLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            SetLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SetName", "Set Name") });
            SetLookUpEdit.DisplayMember = "SetName";
            SetLookUpEdit.Name = "SetLookUpEdit";
            SetLookUpEdit.NullText = "";
            // 
            // gridColumn6
            // 
            gridColumn6.Caption = "Buff Icon";
            gridColumn6.FieldName = "BuffIcon";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 26;
            gridColumn6.Width = 24;
            // 
            // gridColumn7
            // 
            gridColumn7.FieldName = "PartCount";
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 27;
            gridColumn7.Width = 87;
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveButton, ExportButton, ImportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 5;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(747, 144);
            // 
            // SaveButton
            // 
            SaveButton.Caption = "Save Database";
            SaveButton.Id = 1;
            SaveButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.Image");
            SaveButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.LargeImage");
            SaveButton.LargeWidth = 60;
            SaveButton.Name = "SaveButton";
            SaveButton.ItemClick += SaveButton_ItemClick;
            // 
            // ExportButton
            // 
            ExportButton.Caption = "Export";
            ExportButton.Id = 3;
            ExportButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ExportButton.ImageOptions.Image");
            ExportButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ExportButton.ImageOptions.LargeImage");
            ExportButton.Name = "ExportButton";
            ExportButton.ItemClick += ExportButton_Click;
            // 
            // ImportButton
            // 
            ImportButton.Caption = "Import";
            ImportButton.Id = 4;
            ImportButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ImportButton.ImageOptions.Image");
            ImportButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ImportButton.ImageOptions.LargeImage");
            ImportButton.Name = "ImportButton";
            ImportButton.ItemClick += ImportButton_Click;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, JsonImportExport });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(SaveButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Saving";
            // 
            // JsonImportExport
            // 
            JsonImportExport.AllowTextClipping = false;
            JsonImportExport.ItemLinks.Add(ImportButton);
            JsonImportExport.ItemLinks.Add(ExportButton);
            JsonImportExport.Name = "JsonImportExport";
            JsonImportExport.Text = "Json";
            // 
            // ItemInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(747, 513);
            Controls.Add(ItemInfoGridControl);
            Controls.Add(ribbon);
            Name = "ItemInfoView";
            Ribbon = ribbon;
            Text = "Item Info";
            ((System.ComponentModel.ISupportInitialize)ItemStatsGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)StatImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)DropsGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)MonsterLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemTypeImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)RequiredClassImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)RequiredGenderImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)RequiredTypeImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)SetLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl ItemInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView ItemInfoGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView ItemStatsGridView;
        private GridColumn colStat;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatImageComboBox;
        private GridColumn colAmount;
        private GridColumn colItemName;
        private GridColumn colItemType;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox ItemTypeImageComboBox;
        private GridColumn colRequiredClass;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequiredClassImageComboBox;
        private GridColumn colRequiredGender;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequiredGenderImageComboBox;
        private GridColumn colShape;
        private GridColumn colImage;
        private GridColumn colDurability;
        private GridColumn colPrice;
        private GridColumn colStackSize;
        private GridColumn colSellRate;
        private GridColumn colStartItem;
        private GridColumn colCanRepair;
        private GridColumn colCanSell;
        private GridColumn colCanStore;
        private GridColumn colCanTrade;
        private GridColumn colCanDrop;
        private GridColumn ColCanDeathDrop;
        private GridColumn colRarity;
        private GridColumn colDescription;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequiredTypeImageComboBox;
        private DevExpress.XtraGrid.Views.Grid.GridView DropsGridView;
        private GridColumn colMonster;
        private GridColumn colChance;
        private GridColumn colDAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MonsterLookUpEdit;
        private GridColumn colItemEffect;
        private GridColumn colExteriorEffect;
        private GridColumn gridColumn1;
        private GridColumn gridColumn2;
        private GridColumn gridColumn3;
        private GridColumn gridColumn4;
        private GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit SetLookUpEdit;
        private GridColumn colWeight;
        private GridColumn gridColumn6;
        private GridColumn gridColumn7;
        private GridColumn gridColumn8;
        private GridColumn colIndex;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
    }
}