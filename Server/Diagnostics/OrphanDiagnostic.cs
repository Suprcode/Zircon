using MirDB;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Server.Diagnostics
{
    public static class OrphanDiagnostic
    {
        private static readonly PropertyInfo IsDeletedProperty = typeof(DBObject).GetProperty(
            "IsDeleted",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        public sealed class ScanResult
        {
            public List<OrphanTypeResult> Results { get; } = new List<OrphanTypeResult>();
            public int TotalRowsScanned => Results.Sum(x => x.TotalRows);
            public int TotalCleanableOrphans => Results.Sum(x => x.CleanableOrphans);
            public int TotalMarkedTemporary => Results.Sum(x => x.MarkedTemporary);
        }

        public sealed class OrphanTypeResult
        {
            public string ObjectType { get; set; }
            public string ParentType { get; set; }
            public string ParentProperty { get; set; }
            public string ParentList { get; set; }
            public int TotalRows { get; set; }
            public int LinkedRows { get; set; }
            public int CleanableOrphans { get; set; }
            public int ExistingTemporaryOrphans { get; set; }
            public int MissingParent { get; set; }
            public int DeletedParent { get; set; }
            public int MissingParentListLink { get; set; }
            public int MarkedTemporary { get; set; }
            public string SampleIndices { get; set; }

            internal readonly List<int> Samples = new List<int>();
        }

        private sealed class CollectionEntry
        {
            public Type ObjectType;
            public IEnumerable Binding;
        }

        private sealed class OwnedAssociation
        {
            public Type ChildType;
            public Type ParentType;
            public PropertyInfo ParentProperty;
            public PropertyInfo ParentListProperty;
            public PropertyInfo ParentReferenceProperty;
            public CollectionEntry ChildCollection;
        }

        public static ScanResult Scan(int maxSamplesPerType = 40)
        {
            return ScanInternal(markTemporary: false, maxSamplesPerType);
        }

        public static ScanResult MarkTemporaryOnCleanableOrphans(int maxSamplesPerType = 40)
        {
            return ScanInternal(markTemporary: true, maxSamplesPerType);
        }

        public static string FormatLog(ScanResult result, bool cleanRun)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(cleanRun ? "=== MirDB orphan clean ===" : "=== MirDB orphan scan ===");
            builder.AppendLine("Scope: configured dependent user-data tables from currently loaded SEnvir DB collections.");
            builder.AppendLine($"Total dependent rows scanned: {result.TotalRowsScanned:N0}");
            builder.AppendLine($"Total cleanable orphan rows: {result.TotalCleanableOrphans:N0}");

            if (cleanRun)
                builder.AppendLine($"Rows marked IsTemporary: {result.TotalMarkedTemporary:N0}");

            foreach (OrphanTypeResult row in result.Results.Where(x => x.CleanableOrphans > 0 || x.ExistingTemporaryOrphans > 0 || x.MarkedTemporary > 0))
            {
                builder.AppendLine();
                builder.AppendLine($"{row.ObjectType} via {row.ParentType}.{row.ParentList}");
                builder.AppendLine($"  Total rows: {row.TotalRows:N0}");
                builder.AppendLine($"  Linked rows: {row.LinkedRows:N0}");
                builder.AppendLine($"  Cleanable orphans: {row.CleanableOrphans:N0}");
                builder.AppendLine($"  Already temporary orphans: {row.ExistingTemporaryOrphans:N0}");
                builder.AppendLine($"  Missing parent: {row.MissingParent:N0}");
                builder.AppendLine($"  Deleted parent: {row.DeletedParent:N0}");
                builder.AppendLine($"  Missing parent list link: {row.MissingParentListLink:N0}");

                if (cleanRun)
                    builder.AppendLine($"  Marked IsTemporary: {row.MarkedTemporary:N0}");

                if (!string.IsNullOrWhiteSpace(row.SampleIndices))
                    builder.AppendLine($"  Sample indices: {row.SampleIndices}");
            }

            builder.AppendLine(cleanRun ? "=== End MirDB orphan clean ===" : "=== End MirDB orphan scan ===");

            return builder.ToString();
        }

        private static ScanResult ScanInternal(bool markTemporary, int maxSamplesPerType)
        {
            Dictionary<Type, CollectionEntry> collections = GetLoadedCollections();
            List<OwnedAssociation> associations = GetOwnedAssociations(collections);
            ScanResult scan = new ScanResult();

            foreach (IGrouping<Type, OwnedAssociation> group in associations.GroupBy(x => x.ChildType).OrderBy(x => x.Key.Name))
            {
                List<OwnedAssociation> childAssociations = group.OrderBy(x => x.ParentType.Name).ThenBy(GetParentLinkName).ToList();
                OwnedAssociation firstAssociation = childAssociations[0];

                OrphanTypeResult result = new OrphanTypeResult
                {
                    ObjectType = group.Key.Name,
                    ParentType = string.Join(", ", childAssociations.Select(x => x.ParentType.Name).Distinct()),
                    ParentProperty = string.Join(", ", childAssociations.Select(x => x.ParentProperty.Name).Distinct()),
                    ParentList = string.Join(", ", childAssociations.Select(GetParentLinkName).Distinct())
                };

                foreach (object value in firstAssociation.ChildCollection.Binding)
                {
                    result.TotalRows++;

                    DBObject child = value as DBObject;
                    if (child == null || IsMirDbDeleted(child))
                        continue;

                    List<OrphanReason> reasons = childAssociations.Select(x => GetOrphanReason(child, x)).ToList();

                    if (reasons.Any(x => x == OrphanReason.None))
                    {
                        result.LinkedRows++;
                        continue;
                    }

                    OrphanReason reason = GetBestReason(reasons);
                    AddReason(result, reason);

                    if (child.IsTemporary)
                    {
                        result.ExistingTemporaryOrphans++;
                        continue;
                    }

                    result.CleanableOrphans++;

                    if (result.Samples.Count < maxSamplesPerType)
                        result.Samples.Add(child.Index);

                    if (markTemporary)
                    {
                        child.IsTemporary = true;
                        result.MarkedTemporary++;
                    }
                }

                result.SampleIndices = string.Join(", ", result.Samples);
                scan.Results.Add(result);
            }

            return scan;
        }

        private static Dictionary<Type, CollectionEntry> GetLoadedCollections()
        {
            Dictionary<Type, CollectionEntry> result = new Dictionary<Type, CollectionEntry>();
            FieldInfo[] fields = typeof(SEnvir).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsGenericType || field.FieldType.GetGenericTypeDefinition() != typeof(DBCollection<>))
                    continue;

                object collection = field.GetValue(null);
                if (collection == null)
                    continue;

                Type objectType = field.FieldType.GetGenericArguments()[0];
                FieldInfo bindingField = field.FieldType.GetField("Binding");
                IEnumerable binding = bindingField?.GetValue(collection) as IEnumerable;

                if (binding == null || result.ContainsKey(objectType))
                    continue;

                result[objectType] = new CollectionEntry { ObjectType = objectType, Binding = binding };
            }

            return result;
        }

        private static List<OwnedAssociation> GetOwnedAssociations(Dictionary<Type, CollectionEntry> collections)
        {
            List<OwnedAssociation> result = new List<OwnedAssociation>();

            AddOwnership<CharacterInfo, AccountInfo>(result, collections, nameof(CharacterInfo.Account), nameof(AccountInfo.Characters));
            AddOwnership<UserCurrency, AccountInfo>(result, collections, nameof(UserCurrency.Account), nameof(AccountInfo.Currencies));
            AddOwnership<AuctionInfo, AccountInfo>(result, collections, nameof(AuctionInfo.Account), nameof(AccountInfo.Auctions));
            AddOwnership<MailInfo, AccountInfo>(result, collections, nameof(MailInfo.Account), nameof(AccountInfo.Mail));
            AddOwnership<UserDrop, AccountInfo>(result, collections, nameof(UserDrop.Account), nameof(AccountInfo.UserDrops));
            AddOwnership<UserCompanion, AccountInfo>(result, collections, nameof(UserCompanion.Account), nameof(AccountInfo.Companions));
            AddOwnership<UserCompanionUnlock, AccountInfo>(result, collections, nameof(UserCompanionUnlock.Account), nameof(AccountInfo.CompanionUnlocks));
            AddOwnership<BlockInfo, AccountInfo>(result, collections, nameof(BlockInfo.Account), nameof(AccountInfo.BlockingList));
            AddOwnership<BlockInfo, AccountInfo>(result, collections, nameof(BlockInfo.BlockedAccount), nameof(AccountInfo.BlockedByList));
            AddOwnership<UserFortuneInfo, AccountInfo>(result, collections, nameof(UserFortuneInfo.Account), nameof(AccountInfo.Fortunes));

            AddOwnership<CharacterBeltLink, CharacterInfo>(result, collections, nameof(CharacterBeltLink.Character), nameof(CharacterInfo.BeltLinks));
            AddOwnership<AutoPotionLink, CharacterInfo>(result, collections, nameof(AutoPotionLink.Character), nameof(CharacterInfo.AutoPotionLinks));
            AddOwnership<UserMagic, CharacterInfo>(result, collections, nameof(UserMagic.Character), nameof(CharacterInfo.Magics));
            AddOwnership<UserMagic, UserDiscipline>(result, collections, nameof(UserMagic.Discipline), nameof(UserDiscipline.Magics));
            AddOwnership<UserDiscipline, CharacterInfo>(result, collections, nameof(UserDiscipline.Character), nameof(CharacterInfo.Discipline));
            AddOwnership<UserCompanion, CharacterInfo>(result, collections, nameof(UserCompanion.Character), nameof(CharacterInfo.Companion));
            AddOwnership<BuffInfo, CharacterInfo>(result, collections, nameof(BuffInfo.Character), nameof(CharacterInfo.Buffs));
            AddOwnership<BuffInfo, AccountInfo>(result, collections, nameof(BuffInfo.Account), nameof(AccountInfo.Buffs));
            AddOwnership<RefineInfo, CharacterInfo>(result, collections, nameof(RefineInfo.Character), nameof(CharacterInfo.Refines));
            AddOwnership<UserQuest, CharacterInfo>(result, collections, nameof(UserQuest.Character), nameof(CharacterInfo.Quests));
            AddOwnership<UserQuest, AccountInfo>(result, collections, nameof(UserQuest.Account), nameof(AccountInfo.Quests));
            AddOwnership<FriendInfo, CharacterInfo>(result, collections, nameof(FriendInfo.Character), nameof(CharacterInfo.Friends));
            AddOwnership<FriendInfo, CharacterInfo>(result, collections, nameof(FriendInfo.FriendedCharacter), nameof(CharacterInfo.FriendedBy));

            AddOwnership<UserItemStat, UserItem>(result, collections, nameof(UserItemStat.Item), nameof(UserItem.AddedStats));
            AddOwnership<UserQuestTask, UserQuest>(result, collections, nameof(UserQuestTask.Quest), nameof(UserQuest.Tasks));
            AddOwnership<UserMilestoneLog, CharacterInfo>(result, collections, nameof(UserMilestoneLog.Character), nameof(CharacterInfo.MilestoneLogs));
            AddOwnership<UserMilestone, CharacterInfo>(result, collections, nameof(UserMilestone.Character), nameof(CharacterInfo.Milestones));
            AddOwnership<GuildMemberInfo, GuildInfo>(result, collections, nameof(GuildMemberInfo.Guild), nameof(GuildInfo.Members));
            AddOwnership<GuildMemberInfo, AccountInfo>(result, collections, nameof(GuildMemberInfo.Account), nameof(AccountInfo.GuildMember));
            AddOwnership<UserConquest, GuildInfo>(result, collections, nameof(UserConquest.Guild), nameof(GuildInfo.Conquest));

            return result;
        }

        private static void AddOwnership<TChild, TParent>(List<OwnedAssociation> result, Dictionary<Type, CollectionEntry> collections, string parentPropertyName, string parentLinkPropertyName)
            where TChild : DBObject, new()
            where TParent : DBObject, new()
        {
            Type childType = typeof(TChild);
            Type parentType = typeof(TParent);

            if (!collections.TryGetValue(childType, out CollectionEntry childCollection) || !collections.ContainsKey(parentType))
                return;

            PropertyInfo parentProperty = childType.GetProperty(parentPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (parentProperty == null || parentProperty.PropertyType != parentType)
                return;

            PropertyInfo parentLinkProperty = parentType.GetProperty(parentLinkPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (parentLinkProperty == null)
                return;

            PropertyInfo parentListProperty = null;
            PropertyInfo parentReferenceProperty = null;

            if (parentLinkProperty.PropertyType.IsGenericType &&
                parentLinkProperty.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                parentLinkProperty.PropertyType.GetGenericArguments()[0] == childType)
            {
                parentListProperty = parentLinkProperty;
            }
            else if (parentLinkProperty.PropertyType == childType)
            {
                parentReferenceProperty = parentLinkProperty;
            }
            else
                return;

            result.Add(new OwnedAssociation
            {
                ChildType = childType,
                ParentType = parentType,
                ParentProperty = parentProperty,
                ParentListProperty = parentListProperty,
                ParentReferenceProperty = parentReferenceProperty,
                ChildCollection = childCollection
            });
        }

        private static OrphanReason GetOrphanReason(DBObject child, OwnedAssociation association)
        {
            DBObject parent = association.ParentProperty.GetValue(child) as DBObject;

            if (parent == null)
                return OrphanReason.MissingParent;

            if (IsMirDbDeleted(parent))
                return OrphanReason.DeletedParent;

            if (association.ParentListProperty != null)
            {
                IList parentList = association.ParentListProperty.GetValue(parent) as IList;

                if (parentList == null || !parentList.Contains(child))
                    return OrphanReason.MissingParentListLink;
            }
            else if (!ReferenceEquals(association.ParentReferenceProperty.GetValue(parent), child))
            {
                return OrphanReason.MissingParentListLink;
            }

            return OrphanReason.None;
        }

        private static string GetParentLinkName(OwnedAssociation association)
        {
            return association.ParentListProperty?.Name ?? association.ParentReferenceProperty.Name;
        }

        private static OrphanReason GetBestReason(List<OrphanReason> reasons)
        {
            if (reasons.Any(x => x == OrphanReason.MissingParentListLink))
                return OrphanReason.MissingParentListLink;

            if (reasons.Any(x => x == OrphanReason.DeletedParent))
                return OrphanReason.DeletedParent;

            return OrphanReason.MissingParent;
        }

        private static void AddReason(OrphanTypeResult result, OrphanReason reason)
        {
            switch (reason)
            {
                case OrphanReason.MissingParent:
                    result.MissingParent++;
                    break;
                case OrphanReason.DeletedParent:
                    result.DeletedParent++;
                    break;
                case OrphanReason.MissingParentListLink:
                    result.MissingParentListLink++;
                    break;
            }
        }

        private static bool IsMirDbDeleted(DBObject ob)
        {
            if (ob == null || IsDeletedProperty == null)
                return false;

            try
            {
                return IsDeletedProperty.GetValue(ob) is bool b && b;
            }
            catch
            {
                return false;
            }
        }

        private enum OrphanReason
        {
            None,
            MissingParent,
            DeletedParent,
            MissingParentListLink
        }
    }
}
