using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Linq;

namespace Server.Views
{
    public partial class CraftingInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CraftingInfoView()
        {
            InitializeComponent();
            CraftingGridControl.DataSource = SMain.Session.GetCollection<CraftingRecipeInfo>().Binding;
            LevelGridControl.DataSource = SMain.Session.GetCollection<CraftingLevelInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            Design1IngredientGridView.ValidateRow += IngredientGridView_ValidateRow;
            Design2IngredientGridView.ValidateRow += IngredientGridView_ValidateRow;
            Design3IngredientGridView.ValidateRow += IngredientGridView_ValidateRow;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SMain.SetUpView(RecipeGridView);
            SMain.SetUpView(Design1IngredientGridView);
            SMain.SetUpView(Design2IngredientGridView);
            SMain.SetUpView(Design3IngredientGridView);
            SMain.SetUpView(LevelGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e) => SMain.Session.Save(true);
        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (CraftingTabPane.SelectedPage == RecipesTabPage)
                JsonImporter.Import<CraftingRecipeInfo>();
            else if (CraftingTabPane.SelectedPage == LevelsTabPage)
                JsonImporter.Import<CraftingLevelInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (CraftingTabPane.SelectedPage == RecipesTabPage)
                JsonExporter.Export<CraftingRecipeInfo>(RecipeGridView);
            else if (CraftingTabPane.SelectedPage == LevelsTabPage)
                JsonExporter.Export<CraftingLevelInfo>(LevelGridView);
        }

        private void IngredientGridView_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            if (e.Row is not CraftingIngredientInfo ingredient) return;

            var ingredients = ingredient.Design1Recipe?.Design1Ingredients ?? ingredient.Design2Recipe?.Design2Ingredients ?? ingredient.Design3Recipe?.Design3Ingredients;
            if (ingredients == null) return;

            bool tooMany = ingredients.Count > 5;
            bool duplicate = ingredient.Item != null && ingredients.Any(x => x != ingredient && x.Item == ingredient.Item);
            if (!tooMany && !duplicate) return;

            e.Valid = false;
            e.ErrorText = tooMany
                ? "A crafting design can contain no more than five ingredients."
                : "Each ingredient item can only appear once in a crafting design.";
        }

        private void RecipeGridView_MasterRowGetRelationDisplayCaption(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = e.RelationIndex switch
            {
                0 => "Design 1",
                1 => "Design 2",
                _ => "Design 3",
            };
        }
    }
}
