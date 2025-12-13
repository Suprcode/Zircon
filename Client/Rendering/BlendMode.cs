namespace Client.Rendering;

public enum BlendMode : sbyte
{
    NONE = -1,
    NORMAL = 0,
    LIGHT = 1,
    LIGHTINV = 2,
    INVNORMAL = 3,
    INVLIGHT = 4,
    INVLIGHTINV = 5,
    INVCOLOR = 6,
    INVBACKGROUND = 7,
    COLORFY = 8,
    MASK = 9,
    HIGHLIGHT = 10,
    EFFECTMASK = 11,
    LIGHTMAP = 12
}
