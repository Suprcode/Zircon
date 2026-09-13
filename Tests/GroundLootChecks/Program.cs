using Client.Models;
using System.Diagnostics;
using System.Drawing;

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

var random = new Random(431);
var buckets = new RenderRows<(int ID, int Row)>();
// Compare the complete ordered output with the previous renderer across moving viewports.
for (int frame = 0; frame < 200; frame++)
{
    int first = random.Next(-20, 100), last = first + random.Next(0, 80);
    var objects = Enumerable.Range(0, 2000).Select(id => (ID: id, Row: random.Next(-30, 200))).ToArray();
    buckets.Reset(first, last);
    foreach (var ob in objects) buckets.Add(ob.Row, ob);
    var expected = new List<(int, int)>();
    var actual = new List<(int, int)>();
    for (int row = first; row <= last; row++)
    {
        foreach (var ob in objects) if (ob.Row == row) expected.Add(ob);
        actual.AddRange(buckets[row]);
    }
    Require(expected.SequenceEqual(actual), "Row ordering or viewport selection changed.");
}
buckets.Reset(0, -1);
buckets.Add(0, (1, 0));
buckets.Reset(0, 0);
Require(buckets[0].Count == 0, "Empty reset retained a previous scene.");
buckets.Add(0, (1, 0));
buckets.Clear();
buckets.Add(0, (2, 0));
buckets.Reset(0, 0);
Require(buckets[0].Count == 0, "Cleared buckets accepted or retained scene objects.");

var piles = new GroundLootPiles();
var drops = Enumerable.Range(1, 2000).Select(id => (ID: (uint)id, Tile: new Point(id % 11, id % 7), Priority: id % 5)).ToArray();
for (int pass = 0; pass < 20; pass++)
{
    // Vary order and remove entries to cover representative pickup and changing piles.
    var remaining = drops.Where(d => d.ID % (pass + 2) != 0).OrderBy(_ => random.Next()).ToArray();
    piles.Clear();
    foreach (var drop in remaining) piles.Add(drop.Tile, drop.ID, drop.Priority);
    foreach (var group in remaining.GroupBy(d => d.Tile))
    {
        var expected = group.OrderByDescending(d => d.Priority).ThenBy(d => d.ID).First();
        var actual = piles[group.Key];
        Require(actual == expected.ID, "Pile representative changed with insertion order or pickup.");
    }
}
piles.Clear();
piles.Add(Point.Empty, 42, 0);
Require(piles[Point.Empty] == 42, "Pile state retained across maps.");
Console.WriteLine("PASS: ordered row equivalence (200 scenes), empty/reset boundaries, deterministic piles and representative removal (20 shuffles).");

Client.Envir.Config.HighlightedItems = " Red Sword,BLUE ring";
Require(ItemHighlights.Contains("red sword") && ItemHighlights.Contains("BlueRing"), "Highlight normalization failed.");
Client.Envir.Config.HighlightedItems = "different";
Require(!ItemHighlights.Contains("red sword"), "Highlight retained after settings changed.");
Client.Envir.Config.HighlightedItems = null;
Require(!ItemHighlights.Contains("anything"), "Null highlight settings failed.");
for (int cycle = 0; cycle < 10000; cycle++)
{
    var first = GroundItemLabels.Acquire("Sword", Color.White, Color.Empty);
    var second = GroundItemLabels.Acquire("Sword", Color.White, Color.Empty);
    var rare = GroundItemLabels.Acquire("Sword", Color.PaleGreen, Color.Empty);
    var focus = GroundItemLabels.Acquire("Sword", Color.White, Color.Black);
    Require(ReferenceEquals(first, second) && !ReferenceEquals(first, rare) && !ReferenceEquals(first, focus), "Label style sharing is incorrect.");
    GroundItemLabels.Release(first);
    Require(!second.IsDisposed, "A shared label was disposed while still in use.");
    GroundItemLabels.Release(second);
    GroundItemLabels.Release(rare);
    GroundItemLabels.Release(focus);
    Require(Client.Controls.DXLabel.LiveCount == 0, "Label cache retained unused labels.");
}
Console.WriteLine("PASS: highlight refresh, shared-label ownership, colour separation and 10,000 drop/pickup cache cycles (headless labels).");

var denseDrops = Enumerable.Range(1, 8000)
    .Select(id => (ID: (uint)id, Tile: new Point(id % 100, 0), Priority: id % 5)).ToArray();
void RebuildPiles()
{
    piles.Clear();
    foreach (var drop in denseDrops)
        piles.Add(drop.Tile, drop.ID, drop.Priority);
}
RebuildPiles();
long pileBytes = GC.GetAllocatedBytesForCurrentThread();
for (int pass = 0; pass < 100; pass++) RebuildPiles();
Require(GC.GetAllocatedBytesForCurrentThread() == pileBytes, "Warmed compact selection allocated memory.");
foreach (var group in denseDrops.GroupBy(drop => drop.Tile))
    Require(piles[group.Key] == group.OrderByDescending(drop => drop.Priority).ThenBy(drop => drop.ID).First().ID,
        "Dense selection did not preserve priority and tie-breaking.");
Console.WriteLine("PASS: 8,000-item compact selection, deterministic winners and zero warmed allocations.");

var effectLibrary = new Library.MirLibrary();
Client.Envir.CEnvir.LibraryList.Add(Library.LibraryFile.ProgUse, effectLibrary);
DateTime effectStart = Client.Envir.CEnvir.Now;
var normal = new MirEffect(10, 2, TimeSpan.FromMilliseconds(100), Library.LibraryFile.ProgUse, 10, 20, Color.White);
var otherNormal = new MirEffect(10, 2, TimeSpan.FromMilliseconds(100), Library.LibraryFile.ProgUse, 10, 20, Color.White);
Require(!ReferenceEquals(normal.Delays, otherNormal.Delays) && !ReferenceEquals(normal.LightColours, otherNormal.LightColours),
    "Ordinary effects must own their animation arrays.");
Require(normal.Delays.All(delay => delay == TimeSpan.FromMilliseconds(100)) && normal.LightColours.All(colour => colour == Color.White),
    "Ordinary effect constructor did not initialize its animation data.");
var lootTarget = new ItemObject { DrawX = 100, DrawY = 100 };
var loot = new LootEffect(lootTarget, 100, Color.PaleGreen);
var sameLoot = new LootEffect(lootTarget, 100, Color.PaleGreen);
var differentLoot = new LootEffect(lootTarget, 120, Color.MediumPurple);
Require(ReferenceEquals(loot.Delays, sameLoot.Delays) && ReferenceEquals(loot.LightColours, sameLoot.LightColours) &&
    !ReferenceEquals(loot.LightColours, differentLoot.LightColours), "Loot animation sharing is incorrect.");
Client.Envir.CEnvir.Now = effectStart.AddMilliseconds(450);
loot.Process();
Require(loot.FrameIndex == 4 && loot.DrawFrame == 104 && loot.FrameLight == 60, "Loot animation frame/light mismatch.");
int boundReads = effectLibrary.SizeReads;
loot.Process();
Require(effectLibrary.SizeReads == boundReads, "Loot recomputed cached animation bounds.");
lootTarget.DrawX = 2000;
loot.Process();
Client.Envir.CEnvir.Now = effectStart.AddMilliseconds(2750);
lootTarget.DrawX = 100;
loot.Process();
Require(loot.FrameIndex == 7, "Loot animation did not catch up after returning to view.");
effectLibrary.Images = new Library.MirImage[130];
loot.Process();
Require(effectLibrary.SizeReads > boundReads, "Loot bounds did not refresh after resource replacement.");
lootTarget.IsPileRepresentative = false;
Require(loot.FrameLight == 0, "Suppressed pile item retained its light.");
bool effectCompleted = false;
normal.CompleteAction = () => effectCompleted = true;
normal.Process();
Require(effectCompleted && !Client.Scenes.GameScene.Game.MapControl.Effects.Contains(normal), "Ordinary effect completion changed.");
Console.WriteLine("PASS: effect constructor ownership, shared loot data, animation resume, cached bounds/reset and normal completion.");

Console.WriteLine("items,rows,old_checks,new_group_and_visits,old_ms,new_ms,new_allocated_bytes_per_iteration");
foreach (int count in new[] { 100, 500, 1000, 2000 })
{
    const int rows = 50, iterations = 1000;
    var objects = Enumerable.Range(0, count).Select(id => (ID: id, Row: id % rows)).ToList();
    long oldSum = 0, newSum = 0;
    void Old()
    {
        for (int row = 0; row < rows; row++)
            foreach (var ob in objects) if (ob.Row == row) oldSum += ob.ID;
    }
    void New()
    {
        buckets.Reset(0, rows - 1);
        foreach (var ob in objects) buckets.Add(ob.Row, ob);
        for (int row = 0; row < rows; row++)
            foreach (var ob in buckets[row]) newSum += ob.ID;
    }
    for (int i = 0; i < 200; i++) { Old(); New(); }
    var timer = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++) Old();
    double oldMs = timer.Elapsed.TotalMilliseconds / iterations;
    long allocated = GC.GetAllocatedBytesForCurrentThread();
    timer.Restart();
    for (int i = 0; i < iterations; i++) New();
    double newMs = timer.Elapsed.TotalMilliseconds / iterations;
    allocated = GC.GetAllocatedBytesForCurrentThread() - allocated;
    Require(oldSum == newSum, "Benchmark outputs differ.");
    Require(allocated == 0, "Warmed row grouping allocated memory.");
    Console.WriteLine(FormattableString.Invariant($"{count},{rows},{count * rows},{count * 2},{oldMs:F6},{newMs:F6},{allocated / iterations}"));
}
