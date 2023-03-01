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
            this.ItemStatsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.StatImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.DropsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMonster = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MonsterLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.colChance = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colIndex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.colRequiredClass = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RequiredClassImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.colRequiredGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RequiredGenderImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RequiredTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colShape = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemEffect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colExteriorEffect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colWeight = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDurability = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStackSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSellRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStartItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanRepair = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanSell = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanStore = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanTrade = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanDrop = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColCanDeathDrop = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRarity = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SetLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)(this.ItemStatsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemTypeImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredClassImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredGenderImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredTypeImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SetLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemStatsGridView
            // 
            this.ItemStatsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colStat,
            this.colAmount});
            this.ItemStatsGridView.GridControl = this.ItemInfoGridControl;
            this.ItemStatsGridView.Name = "ItemStatsGridView";
            this.ItemStatsGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.ItemStatsGridView.OptionsView.EnableAppearanceOddRow = true;
            this.ItemStatsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.ItemStatsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.ItemStatsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colStat
            // 
            this.colStat.ColumnEdit = this.StatImageComboBox;
            this.colStat.FieldName = "Stat";
            this.colStat.Name = "colStat";
            this.colStat.Visible = true;
            this.colStat.VisibleIndex = 0;
            // 
            // StatImageComboBox
            // 
            this.StatImageComboBox.AutoHeight = false;
            this.StatImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.StatImageComboBox.Name = "StatImageComboBox";
            // 
            // colAmount
            // 
            this.colAmount.FieldName = "Amount";
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 1;
            // 
            // ItemInfoGridControl
            // 
            this.ItemInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = this.ItemStatsGridView;
            gridLevelNode1.RelationName = "ItemStats";
            gridLevelNode2.LevelTemplate = this.DropsGridView;
            gridLevelNode2.RelationName = "Drops";
            this.ItemInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1,
            gridLevelNode2});
            this.ItemInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.ItemInfoGridControl.MainView = this.ItemInfoGridView;
            this.ItemInfoGridControl.MenuManager = this.ribbon;
            this.ItemInfoGridControl.Name = "ItemInfoGridControl";
            this.ItemInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.ItemTypeImageComboBox,
            this.RequiredClassImageComboBox,
            this.RequiredGenderImageComboBox,
            this.StatImageComboBox,
            this.RequiredTypeImageComboBox,
            this.MonsterLookUpEdit,
            this.SetLookUpEdit});
            this.ItemInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.ItemInfoGridControl.Size = new System.Drawing.Size(803, 397);
            this.ItemInfoGridControl.TabIndex = 2;
            this.ItemInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.DropsGridView,
            this.ItemInfoGridView,
            this.ItemStatsGridView});
            // 
            // DropsGridView
            // 
            this.DropsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMonster,
            this.colChance,
            this.colDAmount,
            this.gridColumn3,
            this.gridColumn8});
            this.DropsGridView.GridControl = this.ItemInfoGridControl;
            this.DropsGridView.Name = "DropsGridView";
            this.DropsGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.DropsGridView.OptionsView.EnableAppearanceOddRow = true;
            this.DropsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.DropsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.DropsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colMonster
            // 
            this.colMonster.ColumnEdit = this.MonsterLookUpEdit;
            this.colMonster.FieldName = "Monster";
            this.colMonster.Name = "colMonster";
            this.colMonster.Visible = true;
            this.colMonster.VisibleIndex = 0;
            // 
            // MonsterLookUpEdit
            // 
            this.MonsterLookUpEdit.AutoHeight = false;
            this.MonsterLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.MonsterLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MonsterLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AI", "AI"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Level", "Level"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Experience", "Experience"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "Is Boss")});
            this.MonsterLookUpEdit.DisplayMember = "MonsterName";
            this.MonsterLookUpEdit.Name = "MonsterLookUpEdit";
            this.MonsterLookUpEdit.NullText = "[Monster is null]";
            // 
            // colChance
            // 
            this.colChance.FieldName = "Chance";
            this.colChance.Name = "colChance";
            this.colChance.Visible = true;
            this.colChance.VisibleIndex = 1;
            // 
            // colDAmount
            // 
            this.colDAmount.FieldName = "Amount";
            this.colDAmount.Name = "colDAmount";
            this.colDAmount.Visible = true;
            this.colDAmount.VisibleIndex = 2;
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "DropSet";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            // 
            // gridColumn8
            // 
            this.gridColumn8.FieldName = "PartOnly";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 4;
            // 
            // ItemInfoGridView
            // 
            this.ItemInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colIndex,
            this.colItemName,
            this.colItemType,
            this.colRequiredClass,
            this.colRequiredGender,
            this.gridColumn1,
            this.gridColumn2,
            this.colShape,
            this.colItemEffect,
            this.colExteriorEffect,
            this.colImage,
            this.colWeight,
            this.colDurability,
            this.colPrice,
            this.colStackSize,
            this.colSellRate,
            this.colStartItem,
            this.colCanRepair,
            this.colCanSell,
            this.colCanStore,
            this.colCanTrade,
            this.colCanDrop,
            this.ColCanDeathDrop,
            this.gridColumn4,
            this.colRarity,
            this.colDescription,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7});
            this.ItemInfoGridView.GridControl = this.ItemInfoGridControl;
            this.ItemInfoGridView.Name = "ItemInfoGridView";
            this.ItemInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.ItemInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.ItemInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.ItemInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.ItemInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.ItemInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colIndex
            // 
            this.colIndex.FieldName = "Index";
            this.colIndex.Name = "colIndex";
            this.colIndex.Width = 48;
            // 
            // colItemName
            // 
            this.colItemName.FieldName = "ItemName";
            this.colItemName.Name = "colItemName";
            this.colItemName.Visible = true;
            this.colItemName.VisibleIndex = 0;
            this.colItemName.Width = 56;
            // 
            // colItemType
            // 
            this.colItemType.ColumnEdit = this.ItemTypeImageComboBox;
            this.colItemType.FieldName = "ItemType";
            this.colItemType.Name = "colItemType";
            this.colItemType.Visible = true;
            this.colItemType.VisibleIndex = 1;
            this.colItemType.Width = 24;
            // 
            // ItemTypeImageComboBox
            // 
            this.ItemTypeImageComboBox.AutoHeight = false;
            this.ItemTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ItemTypeImageComboBox.Name = "ItemTypeImageComboBox";
            // 
            // colRequiredClass
            // 
            this.colRequiredClass.ColumnEdit = this.RequiredClassImageComboBox;
            this.colRequiredClass.FieldName = "RequiredClass";
            this.colRequiredClass.Name = "colRequiredClass";
            this.colRequiredClass.Visible = true;
            this.colRequiredClass.VisibleIndex = 2;
            this.colRequiredClass.Width = 24;
            // 
            // RequiredClassImageComboBox
            // 
            this.RequiredClassImageComboBox.AutoHeight = false;
            this.RequiredClassImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RequiredClassImageComboBox.Name = "RequiredClassImageComboBox";
            // 
            // colRequiredGender
            // 
            this.colRequiredGender.ColumnEdit = this.RequiredGenderImageComboBox;
            this.colRequiredGender.FieldName = "RequiredGender";
            this.colRequiredGender.Name = "colRequiredGender";
            this.colRequiredGender.Visible = true;
            this.colRequiredGender.VisibleIndex = 3;
            this.colRequiredGender.Width = 24;
            // 
            // RequiredGenderImageComboBox
            // 
            this.RequiredGenderImageComboBox.AutoHeight = false;
            this.RequiredGenderImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RequiredGenderImageComboBox.Name = "RequiredGenderImageComboBox";
            // 
            // gridColumn1
            // 
            this.gridColumn1.ColumnEdit = this.RequiredTypeImageComboBox;
            this.gridColumn1.FieldName = "RequiredType";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 4;
            this.gridColumn1.Width = 24;
            // 
            // RequiredTypeImageComboBox
            // 
            this.RequiredTypeImageComboBox.AutoHeight = false;
            this.RequiredTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RequiredTypeImageComboBox.Name = "RequiredTypeImageComboBox";
            // 
            // gridColumn2
            // 
            this.gridColumn2.FieldName = "RequiredAmount";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 5;
            this.gridColumn2.Width = 24;
            // 
            // colShape
            // 
            this.colShape.FieldName = "Shape";
            this.colShape.Name = "colShape";
            this.colShape.Visible = true;
            this.colShape.VisibleIndex = 6;
            this.colShape.Width = 24;
            // 
            // colItemEffect
            // 
            this.colItemEffect.FieldName = "ItemEffect";
            this.colItemEffect.Name = "colItemEffect";
            this.colItemEffect.Visible = true;
            this.colItemEffect.VisibleIndex = 7;
            this.colItemEffect.Width = 24;
            // 
            // colExteriorEffect
            // 
            this.colExteriorEffect.FieldName = "ExteriorEffect";
            this.colExteriorEffect.Name = "colExteriorEffect";
            this.colExteriorEffect.Visible = true;
            this.colExteriorEffect.VisibleIndex = 9;
            this.colExteriorEffect.Width = 24;
            // 
            // colImage
            // 
            this.colImage.FieldName = "Image";
            this.colImage.Name = "colImage";
            this.colImage.Visible = true;
            this.colImage.VisibleIndex = 8;
            this.colImage.Width = 24;
            // 
            // colWeight
            // 
            this.colWeight.FieldName = "Weight";
            this.colWeight.Name = "colWeight";
            this.colWeight.Visible = true;
            this.colWeight.VisibleIndex = 10;
            this.colWeight.Width = 24;
            // 
            // colDurability
            // 
            this.colDurability.FieldName = "Durability";
            this.colDurability.Name = "colDurability";
            this.colDurability.Visible = true;
            this.colDurability.VisibleIndex = 11;
            this.colDurability.Width = 24;
            // 
            // colPrice
            // 
            this.colPrice.FieldName = "Price";
            this.colPrice.Name = "colPrice";
            this.colPrice.Visible = true;
            this.colPrice.VisibleIndex = 12;
            this.colPrice.Width = 24;
            // 
            // colStackSize
            // 
            this.colStackSize.FieldName = "StackSize";
            this.colStackSize.Name = "colStackSize";
            this.colStackSize.Visible = true;
            this.colStackSize.VisibleIndex = 13;
            this.colStackSize.Width = 24;
            // 
            // colSellRate
            // 
            this.colSellRate.FieldName = "SellRate";
            this.colSellRate.Name = "colSellRate";
            this.colSellRate.Visible = true;
            this.colSellRate.VisibleIndex = 14;
            this.colSellRate.Width = 24;
            // 
            // colStartItem
            // 
            this.colStartItem.FieldName = "StartItem";
            this.colStartItem.Name = "colStartItem";
            this.colStartItem.Visible = true;
            this.colStartItem.VisibleIndex = 15;
            this.colStartItem.Width = 24;
            // 
            // colCanRepair
            // 
            this.colCanRepair.FieldName = "CanRepair";
            this.colCanRepair.Name = "colCanRepair";
            this.colCanRepair.Visible = true;
            this.colCanRepair.VisibleIndex = 16;
            this.colCanRepair.Width = 24;
            // 
            // colCanSell
            // 
            this.colCanSell.FieldName = "CanSell";
            this.colCanSell.Name = "colCanSell";
            this.colCanSell.Visible = true;
            this.colCanSell.VisibleIndex = 17;
            this.colCanSell.Width = 24;
            // 
            // colCanStore
            // 
            this.colCanStore.FieldName = "CanStore";
            this.colCanStore.Name = "colCanStore";
            this.colCanStore.Visible = true;
            this.colCanStore.VisibleIndex = 18;
            this.colCanStore.Width = 24;
            // 
            // colCanTrade
            // 
            this.colCanTrade.FieldName = "CanTrade";
            this.colCanTrade.Name = "colCanTrade";
            this.colCanTrade.Visible = true;
            this.colCanTrade.VisibleIndex = 19;
            this.colCanTrade.Width = 24;
            // 
            // colCanDrop
            // 
            this.colCanDrop.FieldName = "CanDrop";
            this.colCanDrop.Name = "colCanDrop";
            this.colCanDrop.Visible = true;
            this.colCanDrop.VisibleIndex = 20;
            this.colCanDrop.Width = 24;
            // 
            // ColCanDeathDrop
            // 
            this.ColCanDeathDrop.FieldName = "CanDeathDrop";
            this.ColCanDeathDrop.Name = "ColCanDeathDrop";
            this.ColCanDeathDrop.Visible = true;
            this.ColCanDeathDrop.VisibleIndex = 21;
            this.ColCanDeathDrop.Width = 24;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "CanAutoPot";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 22;
            this.gridColumn4.Width = 24;
            // 
            // colRarity
            // 
            this.colRarity.FieldName = "Rarity";
            this.colRarity.Name = "colRarity";
            this.colRarity.Visible = true;
            this.colRarity.VisibleIndex = 23;
            this.colRarity.Width = 24;
            // 
            // colDescription
            // 
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 24;
            this.colDescription.Width = 24;
            // 
            // gridColumn5
            // 
            this.gridColumn5.ColumnEdit = this.SetLookUpEdit;
            this.gridColumn5.FieldName = "Set";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 25;
            this.gridColumn5.Width = 24;
            // 
            // SetLookUpEdit
            // 
            this.SetLookUpEdit.AutoHeight = false;
            this.SetLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.SetLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.SetLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SetName", "Set Name")});
            this.SetLookUpEdit.DisplayMember = "SetName";
            this.SetLookUpEdit.Name = "SetLookUpEdit";
            this.SetLookUpEdit.NullText = "";
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Buff Icon";
            this.gridColumn6.FieldName = "BuffIcon";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 26;
            this.gridColumn6.Width = 24;
            // 
            // gridColumn7
            // 
            this.gridColumn7.FieldName = "PartCount";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 27;
            this.gridColumn7.Width = 87;
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton,
            this.ribbon.SearchEditItem});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 3;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(803, 144);
            // 
            // SaveButton
            // 
            this.SaveButton.Caption = "Save Database";
            this.SaveButton.Id = 1;
            this.SaveButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.Image")));
            this.SaveButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.LargeImage")));
            this.SaveButton.LargeWidth = 60;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SaveButton_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonPageGroup1.ItemLinks.Add(this.SaveButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Saving";
            // 
            // ItemInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 541);
            this.Controls.Add(this.ItemInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "ItemInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Item Info";
            ((System.ComponentModel.ISupportInitialize)(this.ItemStatsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemTypeImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredClassImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredGenderImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredTypeImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SetLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl ItemInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView ItemInfoGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView ItemStatsGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colStat;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colItemName;
        private DevExpress.XtraGrid.Columns.GridColumn colItemType;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox ItemTypeImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn colRequiredClass;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequiredClassImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn colRequiredGender;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequiredGenderImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn colShape;
        private DevExpress.XtraGrid.Columns.GridColumn colImage;
        private DevExpress.XtraGrid.Columns.GridColumn colDurability;
        private DevExpress.XtraGrid.Columns.GridColumn colPrice;
        private DevExpress.XtraGrid.Columns.GridColumn colStackSize;
        private DevExpress.XtraGrid.Columns.GridColumn colSellRate;
        private DevExpress.XtraGrid.Columns.GridColumn colStartItem;
        private DevExpress.XtraGrid.Columns.GridColumn colCanRepair;
        private DevExpress.XtraGrid.Columns.GridColumn colCanSell;
        private DevExpress.XtraGrid.Columns.GridColumn colCanStore;
        private DevExpress.XtraGrid.Columns.GridColumn colCanTrade;
        private DevExpress.XtraGrid.Columns.GridColumn colCanDrop;
        private DevExpress.XtraGrid.Columns.GridColumn ColCanDeathDrop;
        private DevExpress.XtraGrid.Columns.GridColumn colRarity;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequiredTypeImageComboBox;
        private DevExpress.XtraGrid.Views.Grid.GridView DropsGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colMonster;
        private DevExpress.XtraGrid.Columns.GridColumn colChance;
        private DevExpress.XtraGrid.Columns.GridColumn colDAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MonsterLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colItemEffect;
        private DevExpress.XtraGrid.Columns.GridColumn colExteriorEffect;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit SetLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colWeight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn colIndex;
    }
}