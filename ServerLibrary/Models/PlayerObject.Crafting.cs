using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject
    {
        public CraftingRecipeInfo ActiveCraftingRecipe;
        public int ActiveCraftingDesign;
        public DateTime CraftingCompleteTime;
        public bool Crafting => ActiveCraftingRecipe != null;

        public void SendCraftingState()
        {
            Enqueue(new S.CraftingState
            {
                Level = Math.Max(1, Character.CraftingLevel),
                Experience = Character.CraftingExperience,
                FavouriteRecipeIndex = Character.FavouriteCraftingRecipe?.Index ?? 0,
            });
        }

        public void SetCraftingFavourite(C.CraftingSetFavourite packet)
        {
            CraftingRecipeInfo recipe = SEnvir.CraftingRecipeInfoList.Binding.FirstOrDefault(x => x.Index == packet.RecipeIndex);
            if (recipe == null) return;
            if (Character.FavouriteCraftingRecipe == recipe) return;

            Character.FavouriteCraftingRecipe = recipe;
            SendCraftingState();
        }

        public void StartCrafting(C.CraftingStart packet)
        {
            if (Crafting || Dead || Node == null || Fishing)
            {
                SendCraftingRejected();
                return;
            }

            CraftingRecipeInfo recipe = SEnvir.CraftingRecipeInfoList.Binding.FirstOrDefault(x => x.Index == packet.RecipeIndex);
            if (!CanCraft(recipe, packet.Design))
            {
                SendCraftingRejected();
                return;
            }

            if (AutoPath != null)
                CancelAutoPath();

            ActiveCraftingRecipe = recipe;
            ActiveCraftingDesign = packet.Design;
            TimeSpan duration = recipe.Duration > TimeSpan.Zero ? recipe.Duration : TimeSpan.FromMilliseconds(100);
            CraftingCompleteTime = SEnvir.Now + duration;

            Enqueue(new S.CraftingStarted { RecipeIndex = recipe.Index, Design = packet.Design, Duration = duration });
        }

        public void ProcessCrafting()
        {
            if (!Crafting) return;

            if (Dead || Node == null)
            {
                CancelCrafting();
                return;
            }

            if (SEnvir.Now < CraftingCompleteTime) return;

            CraftingRecipeInfo recipe = ActiveCraftingRecipe;
            int design = ActiveCraftingDesign;

            if (!CanCraft(recipe, design))
            {
                CancelCrafting();
                return;
            }

            Dictionary<ItemInfo, long> requirements = GetCraftingRequirements(recipe, design);
            foreach (KeyValuePair<ItemInfo, long> pair in requirements)
                TakeCraftingItem(pair.Key, pair.Value);

            if (recipe.RequiredGold > 0)
            {
                Gold.Amount -= recipe.RequiredGold;
                GoldChanged();
            }

            ActiveCraftingRecipe = null;
            ActiveCraftingDesign = 0;
            CraftingCompleteTime = DateTime.MinValue;

            bool success = SEnvir.Random.Next(100) < Math.Clamp(recipe.SuccessRate + Stats[Stat.CraftingSuccess], 0, 100);
            if (success)
            {
                long remaining = recipe.Amount;
                while (remaining > 0)
                {
                    long count = Math.Min(remaining, Math.Max(1, recipe.Item.StackSize));
                    GainItem(SEnvir.CreateFreshItem(new ItemCheck(recipe.Item, count, UserItemFlags.None, TimeSpan.Zero)));
                    remaining -= count;
                }

                GainCraftingExperience(recipe.Experience);
            }

            RefreshWeight();
            Enqueue(new S.CraftingEnded
            {
                Result = success ? CraftingResult.Success : CraftingResult.Failed,
                Level = Character.CraftingLevel,
                Experience = Character.CraftingExperience,
            });
        }

        public void CancelCrafting(bool notify = true, bool interrupted = false)
        {
            if (!Crafting) return;

            ActiveCraftingRecipe = null;
            ActiveCraftingDesign = 0;
            CraftingCompleteTime = DateTime.MinValue;

            if (notify && Connection != null)
            {
                Enqueue(new S.CraftingEnded
                {
                    Result = CraftingResult.Cancelled,
                    Interrupted = interrupted,
                    Level = Character.CraftingLevel,
                    Experience = Character.CraftingExperience,
                });

                if (interrupted)
                    Connection.ReceiveChatWithObservers(con => con.Language.CraftingInterrupted, MessageType.System);
            }
        }

        private bool CanCraft(CraftingRecipeInfo recipe, int design)
        {
            if (!ValidCraftingDesign(recipe, design) || Character.CraftingLevel < recipe.RequiredLevel || Gold.Amount < recipe.RequiredGold) return false;

            Dictionary<ItemInfo, long> totals = GetCraftingMaterialTotals();
            foreach (KeyValuePair<ItemInfo, long> pair in GetCraftingRequirements(recipe, design))
                if (!totals.TryGetValue(pair.Key, out long amount) || amount < pair.Value) return false;

            return CanGainCraftingResult(recipe, design);
        }

        private static bool ValidCraftingDesign(CraftingRecipeInfo recipe, int design)
        {
            List<CraftingIngredientInfo> ingredients = GetCraftingIngredients(recipe, design);
            if (recipe?.Item == null || recipe.Amount <= 0 || ingredients.Count is < 1 or > 5) return false;
            return ingredients.All(x => x.Item != null && x.Amount > 0) && ingredients.GroupBy(x => x.Item).All(x => x.Count() == 1);
        }

        private static List<CraftingIngredientInfo> GetCraftingIngredients(CraftingRecipeInfo recipe, int design)
        {
            if (recipe == null) return new List<CraftingIngredientInfo>();

            return design switch
            {
                1 => recipe.Design1Ingredients.ToList(),
                2 => recipe.Design2Ingredients.ToList(),
                3 => recipe.Design3Ingredients.ToList(),
                _ => new List<CraftingIngredientInfo>(),
            };
        }

        private static Dictionary<ItemInfo, long> GetCraftingRequirements(CraftingRecipeInfo recipe, int design)
        {
            return GetCraftingIngredients(recipe, design).GroupBy(x => x.Item).ToDictionary(x => x.Key, x => x.Sum(y => y.Amount));
        }

        private Dictionary<ItemInfo, long> GetCraftingMaterialTotals()
        {
            Dictionary<ItemInfo, long> result = new Dictionary<ItemInfo, long>();
            foreach (UserItem item in Inventory.Concat(Storage))
            {
                if (item?.Info == null) continue;
                result[item.Info] = result.TryGetValue(item.Info, out long count) ? count + item.Count : item.Count;
            }
            return result;
        }

        private bool CanGainCraftingResult(CraftingRecipeInfo recipe, int design)
        {
            if (SEnvir.IsCurrencyItem(recipe.Item) || recipe.Item.ItemEffect == ItemEffect.Experience) return true;

            List<(ItemInfo Info, long Count)> projected = Inventory.Select(x => (x?.Info, x?.Count ?? 0)).ToList();
            foreach (KeyValuePair<ItemInfo, long> requirement in GetCraftingRequirements(recipe, design))
            {
                long remaining = requirement.Value;
                for (int i = 0; i < projected.Count && remaining > 0; i++)
                {
                    if (projected[i].Info != requirement.Key) continue;
                    long take = Math.Min(projected[i].Count, remaining);
                    projected[i] = (projected[i].Info, projected[i].Count - take);
                    remaining -= take;
                }
            }

            long capacity = projected.Count(x => x.Count == 0) * Math.Max(1, recipe.Item.StackSize);
            if (recipe.Item.StackSize > 1)
                capacity += projected.Where(x => x.Info == recipe.Item && x.Count > 0).Sum(x => recipe.Item.StackSize - x.Count);

            return capacity >= recipe.Amount;
        }

        private void TakeCraftingItem(ItemInfo info, long count)
        {
            count = TakeCraftingItem(Inventory, GridType.Inventory, info, count);
            if (count > 0)
                TakeCraftingItem(Storage, GridType.Storage, info, count);
        }

        private long TakeCraftingItem(UserItem[] items, GridType grid, ItemInfo info, long count)
        {
            for (int i = 0; i < items.Length && count > 0; i++)
            {
                UserItem item = items[i];
                if (item?.Info != info) continue;

                long take = Math.Min(item.Count, count);
                item.Count -= take;
                count -= take;

                if (item.Count > 0)
                {
                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = grid, Slot = i, Count = item.Count }, Success = true });
                    continue;
                }

                RemoveItem(item);
                items[i] = null;
                item.Delete();
                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = grid, Slot = i }, Success = true });
            }
            return count;
        }

        private void GainCraftingExperience(long amount)
        {
            if (amount <= 0) return;

            CraftingLevelInfo level = SEnvir.CraftingLevelInfoList.Binding.FirstOrDefault(x => x.Level == Character.CraftingLevel);
            if (level == null)
            {
                Character.CraftingExperience = 0;
                return;
            }

            Character.CraftingExperience += amount;
            while (level.RequiredExperience > 0 && Character.CraftingExperience >= level.RequiredExperience)
            {
                Character.CraftingExperience -= level.RequiredExperience;
                Character.CraftingLevel++;

                level = SEnvir.CraftingLevelInfoList.Binding.FirstOrDefault(x => x.Level == Character.CraftingLevel);
                if (level != null) continue;

                Character.CraftingExperience = 0;
                break;
            }
        }

        public bool LevelCrafting()
        {
            int level = Math.Max(1, Character.CraftingLevel);
            if (SEnvir.CraftingLevelInfoList.Binding.All(x => x.Level != level))
            {
                bool changed = Character.CraftingLevel != level || Character.CraftingExperience != 0;
                Character.CraftingLevel = level;
                Character.CraftingExperience = 0;
                if (changed)
                    SendCraftingState();
                return false;
            }

            Character.CraftingLevel = level + 1;
            Character.CraftingExperience = 0;
            SendCraftingState();
            return true;
        }

        private void SendCraftingRejected()
        {
            Connection?.ReceiveChatWithObservers(con => con.Language.CraftingCannotStart, MessageType.System);
        }
    }
}
