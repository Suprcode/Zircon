using System.Collections.Generic;
using System.Drawing;
using Library;
using MemoryPack;

namespace LibraryCore;

public sealed class StatsFormatter : MemoryPackFormatter<Stats>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Stats value)
    {
        writer.WriteCollectionHeader(value.Values.Count);
        foreach (var pair in value.Values)
        {
            writer.WriteVarInt((int)pair.Key);
            writer.WriteVarInt(pair.Value);
        }
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Stats value)
    {
        reader.TryReadCollectionHeader(out var length);

        var dict = new SortedDictionary<Stat, int>();
        for (int i = 0; i < length; i++)
        {
            dict[(Stat)reader.ReadVarIntInt32()] = reader.ReadVarIntInt32();
        }
        value = new Stats() { Values = dict };
    }
}

public sealed class ColorFormatter : MemoryPackFormatter<Color>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color value)
    {
        var argb = value.ToArgb();
        var rgba = argb << 8 | argb >> 24;
        writer.WriteVarInt(rgba);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Color value)
    {
        var rgba = reader.ReadVarIntInt32();
        var argb = rgba >> 8 | rgba << 24;
        value = Color.FromArgb(argb);
    }
}
