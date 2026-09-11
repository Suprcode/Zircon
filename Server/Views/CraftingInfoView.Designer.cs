namespace Server.Views
{
    partial class CraftingInfoView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode3 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CraftingInfoView));
            Design1IngredientGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            Design1IndexColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            Design1ItemColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            ItemLookUpGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ItemLookUpIndexColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpNameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpTypeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpStackSizeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            Design1AmountColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            CraftingGridControl = new DevExpress.XtraGrid.GridControl();
            Design2IngredientGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            Design2IndexColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            Design2ItemColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            Design2AmountColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            Design3IngredientGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            Design3IndexColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            Design3ItemColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            Design3AmountColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            RecipeIndexColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeDescriptionColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeCategoryColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeItemColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeAmountColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeLevelColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeGoldColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeExperienceColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeDurationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RecipeSuccessRateColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            HomePage = new DevExpress.XtraBars.Ribbon.RibbonPage();
            SaveGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            CraftingTabPane = new DevExpress.XtraBars.Navigation.TabPane();
            RecipesTabPage = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            LevelsTabPage = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            LevelGridControl = new DevExpress.XtraGrid.GridControl();
            LevelGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            LevelIndexColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            LevelColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RequiredExperienceColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)Design1IngredientGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CraftingGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Design2IngredientGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Design3IngredientGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RecipeGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbonControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CraftingTabPane).BeginInit();
            CraftingTabPane.SuspendLayout();
            RecipesTabPage.SuspendLayout();
            LevelsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LevelGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LevelGridView).BeginInit();
            SuspendLayout();
            // 
            // Design1IngredientGridView
            // 
            Design1IngredientGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { Design1IndexColumn, Design1ItemColumn, Design1AmountColumn });
            Design1IngredientGridView.DetailHeight = 512;
            Design1IngredientGridView.GridControl = CraftingGridControl;
            Design1IngredientGridView.Name = "Design1IngredientGridView";
            Design1IngredientGridView.OptionsEditForm.PopupEditFormWidth = 1200;
            Design1IngredientGridView.OptionsView.EnableAppearanceEvenRow = true;
            Design1IngredientGridView.OptionsView.EnableAppearanceOddRow = true;
            Design1IngredientGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            Design1IngredientGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            Design1IngredientGridView.OptionsView.ShowGroupPanel = false;
            // 
            // Design1IndexColumn
            // 
            Design1IndexColumn.FieldName = "Index";
            Design1IndexColumn.MinWidth = 30;
            Design1IndexColumn.Name = "Design1IndexColumn";
            Design1IndexColumn.Width = 112;
            // 
            // Design1ItemColumn
            // 
            Design1ItemColumn.ColumnEdit = ItemLookUpEdit;
            Design1ItemColumn.FieldName = "Item";
            Design1ItemColumn.MinWidth = 30;
            Design1ItemColumn.Name = "Design1ItemColumn";
            Design1ItemColumn.Visible = true;
            Design1ItemColumn.VisibleIndex = 0;
            Design1ItemColumn.Width = 112;
            // 
            // ItemLookUpEdit
            // 
            ItemLookUpEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            ItemLookUpEdit.AutoHeight = false;
            ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ItemLookUpEdit.DisplayMember = "ItemName";
            ItemLookUpEdit.Name = "ItemLookUpEdit";
            ItemLookUpEdit.NullText = "[Item is null]";
            ItemLookUpEdit.PopupView = ItemLookUpGridView;
            // 
            // ItemLookUpGridView
            // 
            ItemLookUpGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { ItemLookUpIndexColumn, ItemLookUpNameColumn, ItemLookUpTypeColumn, ItemLookUpStackSizeColumn });
            ItemLookUpGridView.DetailHeight = 512;
            ItemLookUpGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            ItemLookUpGridView.Name = "ItemLookUpGridView";
            ItemLookUpGridView.OptionsEditForm.PopupEditFormWidth = 1200;
            ItemLookUpGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            ItemLookUpGridView.OptionsView.ShowAutoFilterRow = true;
            ItemLookUpGridView.OptionsView.ShowGroupPanel = false;
            // 
            // ItemLookUpIndexColumn
            // 
            ItemLookUpIndexColumn.Caption = "Index";
            ItemLookUpIndexColumn.FieldName = "Index";
            ItemLookUpIndexColumn.MinWidth = 30;
            ItemLookUpIndexColumn.Name = "ItemLookUpIndexColumn";
            ItemLookUpIndexColumn.Visible = true;
            ItemLookUpIndexColumn.VisibleIndex = 0;
            ItemLookUpIndexColumn.Width = 112;
            // 
            // ItemLookUpNameColumn
            // 
            ItemLookUpNameColumn.Caption = "Item Name";
            ItemLookUpNameColumn.FieldName = "ItemName";
            ItemLookUpNameColumn.MinWidth = 30;
            ItemLookUpNameColumn.Name = "ItemLookUpNameColumn";
            ItemLookUpNameColumn.Visible = true;
            ItemLookUpNameColumn.VisibleIndex = 1;
            ItemLookUpNameColumn.Width = 112;
            // 
            // ItemLookUpTypeColumn
            // 
            ItemLookUpTypeColumn.Caption = "Item Type";
            ItemLookUpTypeColumn.FieldName = "ItemType";
            ItemLookUpTypeColumn.MinWidth = 30;
            ItemLookUpTypeColumn.Name = "ItemLookUpTypeColumn";
            ItemLookUpTypeColumn.Visible = true;
            ItemLookUpTypeColumn.VisibleIndex = 2;
            ItemLookUpTypeColumn.Width = 112;
            // 
            // ItemLookUpStackSizeColumn
            // 
            ItemLookUpStackSizeColumn.Caption = "Stack Size";
            ItemLookUpStackSizeColumn.FieldName = "StackSize";
            ItemLookUpStackSizeColumn.MinWidth = 30;
            ItemLookUpStackSizeColumn.Name = "ItemLookUpStackSizeColumn";
            ItemLookUpStackSizeColumn.Visible = true;
            ItemLookUpStackSizeColumn.VisibleIndex = 3;
            ItemLookUpStackSizeColumn.Width = 112;
            // 
            // Design1AmountColumn
            // 
            Design1AmountColumn.FieldName = "Amount";
            Design1AmountColumn.MinWidth = 30;
            Design1AmountColumn.Name = "Design1AmountColumn";
            Design1AmountColumn.Visible = true;
            Design1AmountColumn.VisibleIndex = 1;
            Design1AmountColumn.Width = 112;
            // 
            // CraftingGridControl
            // 
            CraftingGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            CraftingGridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            gridLevelNode1.LevelTemplate = Design1IngredientGridView;
            gridLevelNode1.RelationName = "Design1Ingredients";
            gridLevelNode2.LevelTemplate = Design2IngredientGridView;
            gridLevelNode2.RelationName = "Design2Ingredients";
            gridLevelNode3.LevelTemplate = Design3IngredientGridView;
            gridLevelNode3.RelationName = "Design3Ingredients";
            CraftingGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1, gridLevelNode2, gridLevelNode3 });
            CraftingGridControl.Location = new System.Drawing.Point(0, 0);
            CraftingGridControl.MainView = RecipeGridView;
            CraftingGridControl.Margin = new System.Windows.Forms.Padding(4);
            CraftingGridControl.MenuManager = ribbonControl;
            CraftingGridControl.Name = "CraftingGridControl";
            CraftingGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { ItemLookUpEdit });
            CraftingGridControl.ShowOnlyPredefinedDetails = true;
            CraftingGridControl.Size = new System.Drawing.Size(1547, 684);
            CraftingGridControl.TabIndex = 1;
            CraftingGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { Design2IngredientGridView, Design3IngredientGridView, RecipeGridView, Design1IngredientGridView });
            // 
            // Design2IngredientGridView
            // 
            Design2IngredientGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { Design2IndexColumn, Design2ItemColumn, Design2AmountColumn });
            Design2IngredientGridView.DetailHeight = 512;
            Design2IngredientGridView.GridControl = CraftingGridControl;
            Design2IngredientGridView.Name = "Design2IngredientGridView";
            Design2IngredientGridView.OptionsEditForm.PopupEditFormWidth = 1200;
            Design2IngredientGridView.OptionsView.EnableAppearanceEvenRow = true;
            Design2IngredientGridView.OptionsView.EnableAppearanceOddRow = true;
            Design2IngredientGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            Design2IngredientGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            Design2IngredientGridView.OptionsView.ShowGroupPanel = false;
            // 
            // Design2IndexColumn
            // 
            Design2IndexColumn.FieldName = "Index";
            Design2IndexColumn.MinWidth = 30;
            Design2IndexColumn.Name = "Design2IndexColumn";
            Design2IndexColumn.Width = 112;
            // 
            // Design2ItemColumn
            // 
            Design2ItemColumn.ColumnEdit = ItemLookUpEdit;
            Design2ItemColumn.FieldName = "Item";
            Design2ItemColumn.MinWidth = 30;
            Design2ItemColumn.Name = "Design2ItemColumn";
            Design2ItemColumn.Visible = true;
            Design2ItemColumn.VisibleIndex = 0;
            Design2ItemColumn.Width = 112;
            // 
            // Design2AmountColumn
            // 
            Design2AmountColumn.FieldName = "Amount";
            Design2AmountColumn.MinWidth = 30;
            Design2AmountColumn.Name = "Design2AmountColumn";
            Design2AmountColumn.Visible = true;
            Design2AmountColumn.VisibleIndex = 1;
            Design2AmountColumn.Width = 112;
            // 
            // Design3IngredientGridView
            // 
            Design3IngredientGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { Design3IndexColumn, Design3ItemColumn, Design3AmountColumn });
            Design3IngredientGridView.DetailHeight = 512;
            Design3IngredientGridView.GridControl = CraftingGridControl;
            Design3IngredientGridView.Name = "Design3IngredientGridView";
            Design3IngredientGridView.OptionsEditForm.PopupEditFormWidth = 1200;
            Design3IngredientGridView.OptionsView.EnableAppearanceEvenRow = true;
            Design3IngredientGridView.OptionsView.EnableAppearanceOddRow = true;
            Design3IngredientGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            Design3IngredientGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            Design3IngredientGridView.OptionsView.ShowGroupPanel = false;
            // 
            // Design3IndexColumn
            // 
            Design3IndexColumn.FieldName = "Index";
            Design3IndexColumn.MinWidth = 30;
            Design3IndexColumn.Name = "Design3IndexColumn";
            Design3IndexColumn.Width = 112;
            // 
            // Design3ItemColumn
            // 
            Design3ItemColumn.ColumnEdit = ItemLookUpEdit;
            Design3ItemColumn.FieldName = "Item";
            Design3ItemColumn.MinWidth = 30;
            Design3ItemColumn.Name = "Design3ItemColumn";
            Design3ItemColumn.Visible = true;
            Design3ItemColumn.VisibleIndex = 0;
            Design3ItemColumn.Width = 112;
            // 
            // Design3AmountColumn
            // 
            Design3AmountColumn.FieldName = "Amount";
            Design3AmountColumn.MinWidth = 30;
            Design3AmountColumn.Name = "Design3AmountColumn";
            Design3AmountColumn.Visible = true;
            Design3AmountColumn.VisibleIndex = 1;
            Design3AmountColumn.Width = 112;
            // 
            // RecipeGridView
            // 
            RecipeGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { RecipeIndexColumn, RecipeDescriptionColumn, RecipeCategoryColumn, RecipeItemColumn, RecipeAmountColumn, RecipeLevelColumn, RecipeGoldColumn, RecipeExperienceColumn, RecipeDurationColumn, RecipeSuccessRateColumn });
            RecipeGridView.DetailHeight = 512;
            RecipeGridView.GridControl = CraftingGridControl;
            RecipeGridView.Name = "RecipeGridView";
            RecipeGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            RecipeGridView.OptionsEditForm.PopupEditFormWidth = 1200;
            RecipeGridView.OptionsView.EnableAppearanceEvenRow = true;
            RecipeGridView.OptionsView.EnableAppearanceOddRow = true;
            RecipeGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            RecipeGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            RecipeGridView.OptionsView.ShowGroupPanel = false;
            RecipeGridView.MasterRowGetRelationDisplayCaption += RecipeGridView_MasterRowGetRelationDisplayCaption;
            // 
            // RecipeIndexColumn
            // 
            RecipeIndexColumn.FieldName = "Index";
            RecipeIndexColumn.MinWidth = 30;
            RecipeIndexColumn.Name = "RecipeIndexColumn";
            RecipeIndexColumn.Width = 112;
            // 
            // RecipeDescriptionColumn
            // 
            RecipeDescriptionColumn.FieldName = "Description";
            RecipeDescriptionColumn.MinWidth = 30;
            RecipeDescriptionColumn.Name = "RecipeDescriptionColumn";
            RecipeDescriptionColumn.Visible = true;
            RecipeDescriptionColumn.VisibleIndex = 0;
            RecipeDescriptionColumn.Width = 112;
            // 
            // RecipeCategoryColumn
            // 
            RecipeCategoryColumn.FieldName = "Category";
            RecipeCategoryColumn.MinWidth = 30;
            RecipeCategoryColumn.Name = "RecipeCategoryColumn";
            RecipeCategoryColumn.Visible = true;
            RecipeCategoryColumn.VisibleIndex = 1;
            RecipeCategoryColumn.Width = 112;
            // 
            // RecipeItemColumn
            // 
            RecipeItemColumn.ColumnEdit = ItemLookUpEdit;
            RecipeItemColumn.FieldName = "Item";
            RecipeItemColumn.MinWidth = 30;
            RecipeItemColumn.Name = "RecipeItemColumn";
            RecipeItemColumn.Visible = true;
            RecipeItemColumn.VisibleIndex = 2;
            RecipeItemColumn.Width = 112;
            // 
            // RecipeAmountColumn
            // 
            RecipeAmountColumn.FieldName = "Amount";
            RecipeAmountColumn.MinWidth = 30;
            RecipeAmountColumn.Name = "RecipeAmountColumn";
            RecipeAmountColumn.Visible = true;
            RecipeAmountColumn.VisibleIndex = 3;
            RecipeAmountColumn.Width = 112;
            // 
            // RecipeLevelColumn
            // 
            RecipeLevelColumn.FieldName = "RequiredLevel";
            RecipeLevelColumn.MinWidth = 30;
            RecipeLevelColumn.Name = "RecipeLevelColumn";
            RecipeLevelColumn.Visible = true;
            RecipeLevelColumn.VisibleIndex = 4;
            RecipeLevelColumn.Width = 112;
            // 
            // RecipeGoldColumn
            // 
            RecipeGoldColumn.FieldName = "RequiredGold";
            RecipeGoldColumn.MinWidth = 30;
            RecipeGoldColumn.Name = "RecipeGoldColumn";
            RecipeGoldColumn.Visible = true;
            RecipeGoldColumn.VisibleIndex = 5;
            RecipeGoldColumn.Width = 112;
            // 
            // RecipeExperienceColumn
            // 
            RecipeExperienceColumn.FieldName = "Experience";
            RecipeExperienceColumn.MinWidth = 30;
            RecipeExperienceColumn.Name = "RecipeExperienceColumn";
            RecipeExperienceColumn.Visible = true;
            RecipeExperienceColumn.VisibleIndex = 6;
            RecipeExperienceColumn.Width = 112;
            // 
            // RecipeDurationColumn
            // 
            RecipeDurationColumn.FieldName = "Duration";
            RecipeDurationColumn.MinWidth = 30;
            RecipeDurationColumn.Name = "RecipeDurationColumn";
            RecipeDurationColumn.Visible = true;
            RecipeDurationColumn.VisibleIndex = 7;
            RecipeDurationColumn.Width = 112;
            // 
            // RecipeSuccessRateColumn
            // 
            RecipeSuccessRateColumn.FieldName = "SuccessRate";
            RecipeSuccessRateColumn.MinWidth = 30;
            RecipeSuccessRateColumn.Name = "RecipeSuccessRateColumn";
            RecipeSuccessRateColumn.Visible = true;
            RecipeSuccessRateColumn.VisibleIndex = 8;
            RecipeSuccessRateColumn.Width = 112;
            // 
            // ribbonControl
            // 
            ribbonControl.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(45, 44, 45, 44);
            ribbonControl.ExpandCollapseItem.Id = 0;
            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl.ExpandCollapseItem, SaveButton, ImportButton, ExportButton });
            ribbonControl.Location = new System.Drawing.Point(0, 0);
            ribbonControl.Margin = new System.Windows.Forms.Padding(4);
            ribbonControl.MaxItemId = 4;
            ribbonControl.Name = "ribbonControl";
            ribbonControl.OptionsMenuMinWidth = 495;
            ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { HomePage });
            ribbonControl.Size = new System.Drawing.Size(1547, 209);
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
            // ImportButton
            // 
            ImportButton.Caption = "Import";
            ImportButton.Id = 2;
            ImportButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ImportButton.ImageOptions.Image");
            ImportButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ImportButton.ImageOptions.LargeImage");
            ImportButton.Name = "ImportButton";
            ImportButton.ItemClick += ImportButton_ItemClick;
            // 
            // ExportButton
            // 
            ExportButton.Caption = "Export";
            ExportButton.Id = 3;
            ExportButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ExportButton.ImageOptions.Image");
            ExportButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ExportButton.ImageOptions.LargeImage");
            ExportButton.Name = "ExportButton";
            ExportButton.ItemClick += ExportButton_ItemClick;
            // 
            // HomePage
            // 
            HomePage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { SaveGroup, JsonGroup });
            HomePage.Name = "HomePage";
            HomePage.Text = "Home";
            // 
            // SaveGroup
            // 
            SaveGroup.AllowTextClipping = false;
            SaveGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            SaveGroup.ItemLinks.Add(SaveButton);
            SaveGroup.Name = "SaveGroup";
            SaveGroup.Text = "Saving";
            // 
            // JsonGroup
            // 
            JsonGroup.ItemLinks.Add(ImportButton);
            JsonGroup.ItemLinks.Add(ExportButton);
            JsonGroup.Name = "JsonGroup";
            JsonGroup.Text = "Json";
            // 
            // CraftingTabPane
            // 
            CraftingTabPane.Controls.Add(RecipesTabPage);
            CraftingTabPane.Controls.Add(LevelsTabPage);
            CraftingTabPane.Dock = System.Windows.Forms.DockStyle.Fill;
            CraftingTabPane.Location = new System.Drawing.Point(0, 209);
            CraftingTabPane.Margin = new System.Windows.Forms.Padding(4);
            CraftingTabPane.Name = "CraftingTabPane";
            CraftingTabPane.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] { RecipesTabPage, LevelsTabPage });
            CraftingTabPane.RegularSize = new System.Drawing.Size(1547, 727);
            CraftingTabPane.SelectedPage = RecipesTabPage;
            CraftingTabPane.Size = new System.Drawing.Size(1547, 727);
            CraftingTabPane.TabIndex = 2;
            // 
            // RecipesTabPage
            // 
            RecipesTabPage.Caption = "Recipes";
            RecipesTabPage.Controls.Add(CraftingGridControl);
            RecipesTabPage.Margin = new System.Windows.Forms.Padding(4);
            RecipesTabPage.Name = "RecipesTabPage";
            RecipesTabPage.Size = new System.Drawing.Size(1547, 684);
            // 
            // LevelsTabPage
            // 
            LevelsTabPage.Caption = "Levels";
            LevelsTabPage.Controls.Add(LevelGridControl);
            LevelsTabPage.Margin = new System.Windows.Forms.Padding(4);
            LevelsTabPage.Name = "LevelsTabPage";
            LevelsTabPage.Size = new System.Drawing.Size(1575, 697);
            // 
            // LevelGridControl
            // 
            LevelGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            LevelGridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            LevelGridControl.Location = new System.Drawing.Point(0, 0);
            LevelGridControl.MainView = LevelGridView;
            LevelGridControl.Margin = new System.Windows.Forms.Padding(4);
            LevelGridControl.MenuManager = ribbonControl;
            LevelGridControl.Name = "LevelGridControl";
            LevelGridControl.Size = new System.Drawing.Size(1575, 697);
            LevelGridControl.TabIndex = 1;
            LevelGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { LevelGridView });
            // 
            // LevelGridView
            // 
            LevelGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { LevelIndexColumn, LevelColumn, RequiredExperienceColumn });
            LevelGridView.DetailHeight = 512;
            LevelGridView.GridControl = LevelGridControl;
            LevelGridView.Name = "LevelGridView";
            LevelGridView.OptionsEditForm.PopupEditFormWidth = 1200;
            LevelGridView.OptionsView.EnableAppearanceEvenRow = true;
            LevelGridView.OptionsView.EnableAppearanceOddRow = true;
            LevelGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            LevelGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            LevelGridView.OptionsView.ShowGroupPanel = false;
            // 
            // LevelIndexColumn
            // 
            LevelIndexColumn.FieldName = "Index";
            LevelIndexColumn.MinWidth = 30;
            LevelIndexColumn.Name = "LevelIndexColumn";
            LevelIndexColumn.Width = 112;
            // 
            // LevelColumn
            // 
            LevelColumn.FieldName = "Level";
            LevelColumn.MinWidth = 30;
            LevelColumn.Name = "LevelColumn";
            LevelColumn.Visible = true;
            LevelColumn.VisibleIndex = 0;
            LevelColumn.Width = 112;
            // 
            // RequiredExperienceColumn
            // 
            RequiredExperienceColumn.FieldName = "RequiredExperience";
            RequiredExperienceColumn.MinWidth = 30;
            RequiredExperienceColumn.Name = "RequiredExperienceColumn";
            RequiredExperienceColumn.Visible = true;
            RequiredExperienceColumn.VisibleIndex = 1;
            RequiredExperienceColumn.Width = 112;
            // 
            // CraftingInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1547, 936);
            Controls.Add(CraftingTabPane);
            Controls.Add(ribbonControl);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "CraftingInfoView";
            Ribbon = ribbonControl;
            Text = "Crafting";
            ((System.ComponentModel.ISupportInitialize)Design1IngredientGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)CraftingGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)Design2IngredientGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)Design3IngredientGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)RecipeGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbonControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)CraftingTabPane).EndInit();
            CraftingTabPane.ResumeLayout(false);
            RecipesTabPage.ResumeLayout(false);
            LevelsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)LevelGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)LevelGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPage HomePage;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup SaveGroup;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonGroup;
        private DevExpress.XtraGrid.GridControl CraftingGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView RecipeGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView Design1IngredientGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView Design2IngredientGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView Design3IngredientGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Views.Grid.GridView ItemLookUpGridView;
        private DevExpress.XtraGrid.Columns.GridColumn ItemLookUpIndexColumn;
        private DevExpress.XtraGrid.Columns.GridColumn ItemLookUpNameColumn;
        private DevExpress.XtraGrid.Columns.GridColumn ItemLookUpTypeColumn;
        private DevExpress.XtraGrid.Columns.GridColumn ItemLookUpStackSizeColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeIndexColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeDescriptionColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeCategoryColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeItemColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeAmountColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeLevelColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeGoldColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeExperienceColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeDurationColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RecipeSuccessRateColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design1IndexColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design1ItemColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design1AmountColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design2IndexColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design2ItemColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design2AmountColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design3IndexColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design3ItemColumn;
        private DevExpress.XtraGrid.Columns.GridColumn Design3AmountColumn;
        private DevExpress.XtraBars.Navigation.TabPane CraftingTabPane;
        private DevExpress.XtraBars.Navigation.TabNavigationPage RecipesTabPage;
        private DevExpress.XtraBars.Navigation.TabNavigationPage LevelsTabPage;
        private DevExpress.XtraGrid.GridControl LevelGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView LevelGridView;
        private DevExpress.XtraGrid.Columns.GridColumn LevelIndexColumn;
        private DevExpress.XtraGrid.Columns.GridColumn LevelColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RequiredExperienceColumn;
    }
}
