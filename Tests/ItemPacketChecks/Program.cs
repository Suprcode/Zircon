using System.Collections.Concurrent;
using System.Reflection;
using System.Numerics;
using System.Runtime.CompilerServices;
using Library;
using Library.Network;
using Library.SystemModels;
using MirDB;
using Server.DBModels;
using Server.Envir;
using Server.Envir.Translations;
using Server.Models;
using C = Library.Network.ClientPackets;

// Exercise the real handlers against disposable MirDB objects, without sockets or a live world.
string root = Path.Combine(Path.GetTempPath(), "ZirconItemChecks-" + Guid.NewGuid().ToString("N"));
var session = new Session(SessionMode.System | SessionMode.Users, root + Path.DirectorySeparatorChar);
session.Initialize(typeof(ItemInfo).Assembly, typeof(UserItem).Assembly);
foreach (FieldInfo field in typeof(SEnvir).GetFields(BindingFlags.Public | BindingFlags.Static))
    if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(DBCollection<>))
        field.SetValue(null, session.GetCollection(field.FieldType.GetGenericArguments()[0]));
SEnvir.Session = session;
SEnvir.Random = new Random(20261101);
Globals.CurrencyInfoList = SEnvir.CurrencyInfoList;

var goldItem = Definition(ItemType.Nothing, 1);
var goldInfo = SEnvir.CurrencyInfoList.CreateNewObject();
goldInfo.Type = CurrencyType.Gold;
goldInfo.DropItem = goldItem;
foreach (CurrencyType type in new[] { CurrencyType.GameGold, CurrencyType.HuntGold })
{
    var currency = SEnvir.CurrencyInfoList.CreateNewObject();
    currency.Type = type;
}
var ring = Definition(ItemType.Ring, 1);
var ore = Definition(ItemType.Ore, 1);
ore.ItemEffect = ItemEffect.Corundum;
var stack = Definition(ItemType.Consumable, 100);
int checks = 0;

PlayerObject Player()
{
    var account = SEnvir.AccountInfoList.CreateNewObject();
    account.Password = Array.Empty<byte>();
    var character = SEnvir.CharacterInfoList.CreateNewObject();
    character.Account = account;
    character.CharacterName = "PacketCheck";
    var con = (SConnection)RuntimeHelpers.GetUninitializedObject(typeof(SConnection));
    con.Observers = new();
    con.SendList = new ConcurrentQueue<Packet>();
    con.Language = new EnglishMessages();
    con.Stage = GameStage.Game;
    con.Connected = true;
    var player = (PlayerObject)RuntimeHelpers.GetUninitializedObject(typeof(CheckPlayer));
    player.Character = character;
    player.Connection = con;
    player.Inventory = new UserItem[Globals.InventorySize];
    player.Equipment = new UserItem[Globals.EquipmentSize];
    player.Storage = new UserItem[1000];
    player.PartsStorage = new UserItem[1000];
    player.TradeItems = new();
    player.NPCVals = new();
    player.Stats = new Stats();
    con.Player = player;
    return player;
}

ItemInfo Definition(ItemType type, int size)
{
    var info = SEnvir.ItemInfoList.CreateNewObject();
    info.ItemType = type;
    info.StackSize = size;
    return info;
}

UserItem Put(PlayerObject player, int slot, ItemInfo info, long count = 1)
{
    var item = SEnvir.UserItemList.CreateNewObject();
    item.Info = info;
    item.Count = count;
    item.Character = player.Character;
    item.Slot = slot;
    player.Inventory[slot] = item;
    return item;
}

CellLinkInfo Link(int slot, long count = 1) => new() { GridType = GridType.Inventory, Slot = slot, Count = count };
void Check(bool success, string name)
{
    if (!success) throw new Exception(name);
    checks++;
}

var player = Player();
Check(!player.ParseLinks(new List<CellLinkInfo> { Link(0, long.MaxValue), Link(0, 1) }, 0, 5), "Duplicate quantity overflow rejected");
var validLinks = new List<CellLinkInfo> { Link(0, 2), Link(0, 3) };
Check(player.ParseLinks(validLinks, 0, 5) && validLinks.Count == 1 && validLinks[0].Count == 5, "Valid duplicate quantities combined");
Check(!player.ParseLinks(new List<CellLinkInfo> { null }, 0, 5), "Null link rejected");

for (int i = 0; i < player.Inventory.Length; i++) Put(player, i, ring);
player.Inventory[0] = null;
Put(player, 0, stack, 99);
Check(!player.CanGainItems(false, new ItemCheck(stack, 1, 0, TimeSpan.Zero), new ItemCheck(ring, 1, 0, TimeSpan.Zero)), "Later reward checked after first reward stacks");
Check(!player.CanGainItems(false, new ItemCheck(stack, 1, 0, TimeSpan.Zero), new ItemCheck(stack, 1, 0, TimeSpan.Zero)), "Stack space cannot be reused");
Check(player.CanGainItems(false, new ItemCheck(stack, 1, 0, TimeSpan.Zero)), "Available stack capacity accepted");
player.Gold.Amount = long.MaxValue - 1;
Check(!player.CanGainItems(false, new ItemCheck(goldItem, 2, 0, TimeSpan.Zero)), "Currency balance overflow rejected");
Check(!player.CanGainItems(false, new ItemCheck(goldItem, 1, 0, TimeSpan.Zero), new ItemCheck(goldItem, 1, 0, TimeSpan.Zero)), "Combined currency overflow rejected");

player = Player();
var template = Definition(ItemType.Nothing, 1);
template.ItemEffect = ItemEffect.WeaponTemplate;
var material = Put(player, 0, template);
player.Connection.Process(new C.NPCWeaponCraft { Template = Link(0), Class = RequiredClass.Warrior });
Check(player.Inventory[0] == material && player.Gold.Amount == 0, "Craft requires NPC authorization");
player.NPC = new NPCObject();
player.NPCPage = session.GetCollection<NPCPage>().CreateNewObject();
player.NPCPage.DialogType = NPCDialogType.WeaponCraft;
player.Connection.Process(new C.NPCWeaponCraft { Template = Link(0), Class = RequiredClass.Warrior });
Check(player.Inventory[0] == material && player.Gold.Amount == 0, "Craft cannot spend missing gold");

player = Player();
player.NPC = new NPCObject();
player.NPCPage = session.GetCollection<NPCPage>().CreateNewObject();
player.NPCPage.DialogType = NPCDialogType.AccessoryRefine;
player.Gold.Amount = 50000;
var target = Put(player, 0, ring);
Put(player, 1, ore);
Put(player, 2, ring);
Put(player, 3, ring);
void RejectRefine(C.NPCAccessoryRefine packet, string name)
{
    player.Connection.Process(packet);
    Check(player.Gold.Amount == 50000 && player.Inventory[0] == target && player.Inventory[1]?.Info == ore &&
        player.Inventory[2]?.Count == 1 && player.Inventory[3]?.Count == 1 && target.AddedStats.Count == 0, name);
}
RejectRefine(new() { Target = Link(0), OreTarget = Link(1), Links = new(), RefineType = RefineType.DC }, "Refine needs two materials");
RejectRefine(new() { Target = Link(0), OreTarget = null, Links = new() { Link(2), Link(3) }, RefineType = RefineType.DC }, "Null ore rejected");
RejectRefine(new() { Target = Link(0), OreTarget = Link(1), Links = new() { Link(0), Link(3) }, RefineType = RefineType.DC }, "Self consumption rejected");
RejectRefine(new() { Target = Link(0), OreTarget = Link(2), Links = new() { Link(1), Link(3) }, RefineType = RefineType.DC }, "Incorrect ore rejected");
RejectRefine(new() { Target = Link(0), OreTarget = Link(1), Links = new() { Link(2, long.MaxValue), Link(3) }, RefineType = RefineType.DC }, "Oversized materials rejected");
player.Inventory[2].Flags = UserItemFlags.Locked;
RejectRefine(new() { Target = Link(0), OreTarget = Link(1), Links = new() { Link(2), Link(3) }, RefineType = RefineType.DC }, "Invalid material cannot be skipped");

var other = Player();
player.TradePartner = other;
other.TradePartner = player;
other.TradeConfirmed = true;
player.Connection.Process(new C.TradeAddItem { Cell = Link(3) });
player.Connection.Process(new C.TradeAddGold { Gold = 100 });
Check(player.TradeItems.Count == 0 && player.TradeGold == 0, "Confirmed offers cannot be amended");

player = Player();
player.InSafeZone = true;
var moved = Put(player, 0, ring);
player.Connection.Process(new C.ItemMove { FromGrid = GridType.Inventory, FromSlot = 0, ToGrid = GridType.Storage, ToSlot = 0 });
Check(player.Inventory[0] == null && player.Storage[0] == moved && moved.Character == null && moved.Account == player.Character.Account &&
    !player.Character.Items.Contains(moved) && player.Character.Account.Items.Contains(moved), "Storage move clears old persistent owner");
var swapped = Put(player, 1, stack, 2);
player.Connection.Process(new C.ItemMove { FromGrid = GridType.Storage, FromSlot = 0, ToGrid = GridType.Inventory, ToSlot = 1 });
Check(player.Inventory[1] == moved && player.Storage[0] == swapped && moved.Account == null && moved.Character == player.Character &&
    swapped.Character == null && swapped.Account == player.Character.Account, "Swap clears both old persistent owners");
int movedIndex = moved.Index;
int swappedIndex = swapped.Index;
int characterIndex = player.Character.Index;
int accountIndex = player.Character.Account.Index;

var buyer = Player();
var listing = SEnvir.AuctionInfoList.CreateNewObject();
listing.Account = player.Character.Account;
listing.Price = int.MaxValue;
var listedItem = SEnvir.UserItemList.CreateNewObject();
listedItem.Info = ring;
listedItem.Count = long.MaxValue;
listing.Item = listedItem;
buyer.Connection.MPSearchResults = new() { listing };
buyer.Connection.Process(new C.MarketPlaceBuy { Index = listing.Index, Count = long.MaxValue });
Check(listing.Item == listedItem && listedItem.Count == long.MaxValue && buyer.Gold.Amount == 0, "Marketplace total overflow rejected before payment or transfer");

var store = SEnvir.StoreInfoList.CreateNewObject();
store.Item = stack;
store.Available = true;
store.Price = 1500000000;
buyer.GameGold.Amount = 4000000000;
buyer.Connection.Process(new C.MarketPlaceStoreBuy { Index = store.Index, Count = 2 });
Check(buyer.GameGold.Amount == 1000000000 && buyer.Inventory.Where(x => x?.Info == stack).Sum(x => x.Count) == 2,
    "Cash shop charges full 64 bit price without truncation");

player.NPC = new NPCObject();
player.NPCPage = session.GetCollection<NPCPage>().CreateNewObject();
player.NPCPage.DialogType = NPCDialogType.RollDie;
player.NPCPage.SuccessPage = session.GetCollection<NPCPage>().CreateNewObject();
var rollPage = player.NPCPage;
var rewardPage = rollPage.SuccessPage;
rewardPage.Say = "Reward";
rewardPage.SuccessPage = rewardPage;
var rewardAction = session.GetCollection<NPCAction>().CreateNewObject();
rewardAction.Page = rewardPage;
rewardAction.ActionType = NPCActionType.GiveGold;
rewardAction.IntParameter1 = 10;
player.Connection.Process(new C.NPCRollResult());
Check(player.NPCPage == rollPage && player.Gold.Amount == 0, "NPC reward continuation requires a roll");
player.Connection.Process(new C.NPCRoll { Type = 0 });
int roll = (int)player.NPCVals["ROLLRESULT"];
for (int i = 0; i < 100; i++) player.Connection.Process(new C.NPCRoll { Type = 0 });
Check((int)player.NPCVals["ROLLRESULT"] == roll, "Pending NPC roll cannot be rerolled");
player.Connection.Process(new C.NPCRollResult());
Check(player.NPCPage == rewardPage && player.Gold.Amount == 10, "Pending NPC roll pays its reward once");
player.Connection.Process(new C.NPCRollResult());
Check(player.Gold.Amount == 10, "NPC reward continuation cannot be replayed");

var random = new Random(20261101);
long[] extremes = { long.MinValue, -1, 0, 1, 2, long.MaxValue - 1, long.MaxValue };
for (int iteration = 0; iteration < 5000; iteration++)
{
    var links = Enumerable.Range(0, random.Next(6)).Select(_ => Link(random.Next(4), extremes[random.Next(extremes.Length)])).ToList();
    var expected = links.GroupBy(x => x.Slot).ToDictionary(x => x.Key, x => x.Aggregate(BigInteger.Zero, (sum, link) => sum + link.Count));
    bool valid = links.All(x => x.Count > 0) && expected.Values.All(x => x <= long.MaxValue);
    Check(player.ParseLinks(links, 0, 5) == valid, "Random quantity validation agrees with unlimited precision arithmetic");
    if (valid) Check(links.All(x => x.Count == expected[x.Slot]) && links.Count == expected.Count, "Random quantities conserved");
    player.Connection.Process(new C.ItemMove { FromGrid = GridType.Inventory, FromSlot = random.Next(2) == 0 ? int.MinValue : int.MaxValue, ToGrid = (GridType)random.Next(256), ToSlot = random.Next() });
    player.Connection.Process(new C.ItemSplit { Grid = GridType.Inventory, Slot = 1, Count = extremes[random.Next(3)] });
    player.Connection.SendList.Clear();
}
Check(player.Inventory[1] == moved && moved.Count == 1 && swapped.Count == 2, "Random rejected packets conserve items");

session.Save(true);
var reload = new Session(SessionMode.System | SessionMode.Users, root + Path.DirectorySeparatorChar);
reload.Initialize(typeof(ItemInfo).Assembly, typeof(UserItem).Assembly);
var reloadedMove = reload.GetCollection<UserItem>().Binding.Single(x => x.Index == movedIndex);
var reloadedSwap = reload.GetCollection<UserItem>().Binding.Single(x => x.Index == swappedIndex);
Check(reloadedMove.Character?.Index == characterIndex && reloadedMove.Account == null && reloadedSwap.Character == null &&
    reloadedSwap.Account?.Index == accountIndex, "Exclusive ownership survives save and reload");

Console.WriteLine($"Passed {checks} item packet regression checks. Disposable database: {root}");

// Stats and world broadcasts are outside these transaction checks; all item and MirDB code is production code.
sealed class CheckPlayer : PlayerObject
{
    private CheckPlayer() : base(null, null) { }
    public override void RefreshStats() { }
}
