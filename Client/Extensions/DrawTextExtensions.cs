using System.Collections.Generic;
using Client.Envir;
using Shared.Rendering;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Client.Extensions
{
    public class DXButtonIndex
    {
        public CharacterRange Range;
        public DXButtonType Type;
    };

    public class ButtonInfo
    {
        public Rectangle Region;
        public int Index;
        public int Length;
    }

    public enum DXButtonType
    {
        Button,
        Label
    };

    public partial class DrawTextExtensions
    {
        [GeneratedRegex("\\<(?<Text>.*?):(?<Default>.+?)\\>", RegexOptions.Compiled)]
        public static partial Regex ValueRegex();

        [GeneratedRegex("\\{(?<Text>.*?):(?<Colour>.+?)\\}", RegexOptions.Compiled)]
        public static partial Regex ColourRegex();

        [GeneratedRegex("\\[(?<Text>.*?):(?<ID>.+?)\\]", RegexOptions.Compiled)]
        public static partial Regex ButtonRegex();

        public static List<ButtonInfo> GetWordRegionsNew(string text, Font font, TextFormatFlags flags, int width, int index, int length)
        {
            List<ButtonInfo> regions = new List<ButtonInfo>();

            float rasterScale = Math.Max(1F, CEnvir.Target?.TextRasterScale ?? 1F);
            using Font rasterFont = RenderingPipelineManager.CreatePixelFont(font, rasterScale);
            Size measureBounds = new Size(
                Math.Max(1, (int)Math.Ceiling(width * rasterScale)),
                Math.Max(1, (int)Math.Ceiling(9999 * rasterScale)));

            Size Measure(string value) => RenderingPipelineManager.MeasureText(value, rasterFont, measureBounds, flags);
            int ToLogicalPosition(int value) => (int)Math.Round(value / rasterScale, MidpointRounding.AwayFromZero);
            int ToLogicalLength(int value) => (int)Math.Ceiling(value / rasterScale);

            Size tSize = Measure("A");
            int physicalLineHeight = tSize.Height;
            int h = ToLogicalLength(physicalLineHeight);
            int leading = tSize.Width - (Measure("AA").Width - tSize.Width);

            int lineStart = 0;
            int lastHeight = physicalLineHeight;

            Regex regex = new Regex(@"(?<Words>\S+)", RegexOptions.Compiled);

            MatchCollection matches = regex.Matches(text);

            List<CharacterRange> ranges = new List<CharacterRange>();

            foreach (Match match in matches)
                ranges.Add(new CharacterRange(match.Index, match.Length));

            ButtonInfo currentInfo = null;

            //If Word Wrap enabled.
            foreach (CharacterRange range in ranges)
            {
                int height = Measure(text.Substring(0, range.First + range.Length)).Height;

                if (range.First >= index + length) break;

                if (height > lastHeight)
                {
                    lineStart = range.First; // New Line was formed record from start.
                    lastHeight = height;

                    //This Word is on a new line and therefore must start at 0.
                    //We do NOT know its length on this new line but since its on a new line it will be easy to measure.

                    if (range.First >= index)
                    {
                        //We need to capture this word
                        //It needs to be a new Rectangle.
                        Rectangle region = new Rectangle
                        {
                            X = 0,
                            Y = ToLogicalPosition(height - physicalLineHeight),
                            Width = ToLogicalLength(Measure(text.Substring(range.First, range.Length)).Width),
                            Height = h,
                        };
                        currentInfo = new ButtonInfo { Region = region, Index = range.First, Length = range.Length };
                        regions.Add(currentInfo);
                    }
                }
                else
                {
                    //it is on the same Line IT Must be able to contain ALL of the letters. (Word Wrap)
                    //just need to know the length of the word and the Length of the start of the line to the start of the word

                    if (range.First >= index)
                    {
                        if (currentInfo == null)
                        {
                            int physicalX = Measure(text.Substring(lineStart, range.First - lineStart)).Width;
                            if (physicalX > 0)
                                physicalX -= leading;

                            Rectangle region = new Rectangle
                            {
                                X = ToLogicalPosition(physicalX),
                                Y = ToLogicalPosition(height - physicalLineHeight),
                                Width = ToLogicalLength(Measure(text.Substring(range.First, range.Length)).Width),
                                Height = h,
                            };

                            currentInfo = new ButtonInfo { Region = region, Index = range.First, Length = range.Length };
                            regions.Add(currentInfo);
                        }
                        else
                        {
                            //Measure Current.Index to range.First + Length
                            currentInfo.Length = range.First + range.Length - currentInfo.Index;
                            currentInfo.Region.Width = ToLogicalLength(Measure(text.Substring(currentInfo.Index, currentInfo.Length)).Width);
                        }
                        //We need to capture this word.
                        //ADD to any previous rects otherwise create new ?
                    }
                }
            }

            return regions;
        }

    }
}
