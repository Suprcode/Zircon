using MirDB;
using System;

namespace Library.SystemModels
{
    public class CraftingLevelInfo : DBObject
    {
        [IsIdentity]
        public int Level
        {
            get => _Level;
            set { if (_Level == value) return; int oldValue = _Level; _Level = value; OnChanged(oldValue, value, "Level"); }
        }
        private int _Level;

        public long RequiredExperience
        {
            get => _RequiredExperience;
            set { if (_RequiredExperience == value) return; long oldValue = _RequiredExperience; _RequiredExperience = value; OnChanged(oldValue, value, "RequiredExperience"); }
        }
        private long _RequiredExperience;
    }

    public class CraftingRecipeInfo : DBObject
    {
        [IsIdentity]
        public string Description
        {
            get => _Description;
            set { if (_Description == value) return; string oldValue = _Description; _Description = value; OnChanged(oldValue, value, "Description"); }
        }
        private string _Description;

        public CraftingCategory Category
        {
            get => _Category;
            set { if (_Category == value) return; CraftingCategory oldValue = _Category; _Category = value; OnChanged(oldValue, value, "Category"); }
        }
        private CraftingCategory _Category;

        public ItemInfo Item
        {
            get => _Item;
            set { if (_Item == value) return; ItemInfo oldValue = _Item; _Item = value; OnChanged(oldValue, value, "Item"); }
        }
        private ItemInfo _Item;

        public int Amount
        {
            get => _Amount;
            set { if (_Amount == value) return; int oldValue = _Amount; _Amount = value; OnChanged(oldValue, value, "Amount"); }
        }
        private int _Amount = 1;

        public int RequiredLevel
        {
            get => _RequiredLevel;
            set { if (_RequiredLevel == value) return; int oldValue = _RequiredLevel; _RequiredLevel = value; OnChanged(oldValue, value, "RequiredLevel"); }
        }
        private int _RequiredLevel = 1;

        public long RequiredGold
        {
            get => _RequiredGold;
            set { if (_RequiredGold == value) return; long oldValue = _RequiredGold; _RequiredGold = value; OnChanged(oldValue, value, "RequiredGold"); }
        }
        private long _RequiredGold;

        public long Experience
        {
            get => _Experience;
            set { if (_Experience == value) return; long oldValue = _Experience; _Experience = value; OnChanged(oldValue, value, "Experience"); }
        }
        private long _Experience;

        public TimeSpan Duration
        {
            get => _Duration;
            set { if (_Duration == value) return; TimeSpan oldValue = _Duration; _Duration = value; OnChanged(oldValue, value, "Duration"); }
        }
        private TimeSpan _Duration = TimeSpan.FromSeconds(1);

        public int SuccessRate
        {
            get => _SuccessRate;
            set { if (_SuccessRate == value) return; int oldValue = _SuccessRate; _SuccessRate = value; OnChanged(oldValue, value, "SuccessRate"); }
        }
        private int _SuccessRate = 100;

        [Association("Design1Ingredients", true)]
        public DBBindingList<CraftingIngredientInfo> Design1Ingredients { get; set; }

        [Association("Design2Ingredients", true)]
        public DBBindingList<CraftingIngredientInfo> Design2Ingredients { get; set; }

        [Association("Design3Ingredients", true)]
        public DBBindingList<CraftingIngredientInfo> Design3Ingredients { get; set; }

        protected internal override void OnDeleted()
        {
            for (int i = Design1Ingredients.Count - 1; i >= 0; i--)
                Design1Ingredients[i].Delete();

            for (int i = Design2Ingredients.Count - 1; i >= 0; i--)
                Design2Ingredients[i].Delete();

            for (int i = Design3Ingredients.Count - 1; i >= 0; i--)
                Design3Ingredients[i].Delete();

            Item = null;
            base.OnDeleted();
        }
    }

    public class CraftingIngredientInfo : DBObject
    {
        [Association("Design1Ingredients")]
        public CraftingRecipeInfo Design1Recipe
        {
            get => _Design1Recipe;
            set { if (_Design1Recipe == value) return; CraftingRecipeInfo oldValue = _Design1Recipe; _Design1Recipe = value; OnChanged(oldValue, value, "Design1Recipe"); }
        }
        private CraftingRecipeInfo _Design1Recipe;

        [Association("Design2Ingredients")]
        public CraftingRecipeInfo Design2Recipe
        {
            get => _Design2Recipe;
            set { if (_Design2Recipe == value) return; CraftingRecipeInfo oldValue = _Design2Recipe; _Design2Recipe = value; OnChanged(oldValue, value, "Design2Recipe"); }
        }
        private CraftingRecipeInfo _Design2Recipe;

        [Association("Design3Ingredients")]
        public CraftingRecipeInfo Design3Recipe
        {
            get => _Design3Recipe;
            set { if (_Design3Recipe == value) return; CraftingRecipeInfo oldValue = _Design3Recipe; _Design3Recipe = value; OnChanged(oldValue, value, "Design3Recipe"); }
        }
        private CraftingRecipeInfo _Design3Recipe;

        public ItemInfo Item
        {
            get => _Item;
            set { if (_Item == value) return; ItemInfo oldValue = _Item; _Item = value; OnChanged(oldValue, value, "Item"); }
        }
        private ItemInfo _Item;

        public long Amount
        {
            get => _Amount;
            set { if (_Amount == value) return; long oldValue = _Amount; _Amount = value; OnChanged(oldValue, value, "Amount"); }
        }
        private long _Amount = 1;

        protected internal override void OnDeleted()
        {
            Design1Recipe = null;
            Design2Recipe = null;
            Design3Recipe = null;
            Item = null;
            base.OnDeleted();
        }
    }
}
