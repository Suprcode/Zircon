using Client.Rendering;
using Client.Scenes.Views;
using System.Collections.Generic;
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

    public enum DXButtonType
    {
        Button,
        Label
    };

    public partial class DXTextExtensions
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

            Size tSize = RenderingPipelineManager.MeasureText("A", font, new Size(width, 2000), flags);
            int h = tSize.Height;
            int leading = tSize.Width - (RenderingPipelineManager.MeasureText("AA", font, new Size(width, 2000), flags).Width - tSize.Width);

            int lineStart = 0;
            int lastHeight = h;

            Regex regex = new Regex(@"(?<Words>\S+)", RegexOptions.Compiled);

            MatchCollection matches = regex.Matches(text);

            List<CharacterRange> ranges = new List<CharacterRange>();

            foreach (Match match in matches)
                ranges.Add(new CharacterRange(match.Index, match.Length));

            ButtonInfo currentInfo = null;

            //If Word Wrap enabled.
            foreach (CharacterRange range in ranges)
            {
                int height = RenderingPipelineManager.MeasureText(text.Substring(0, range.First + range.Length), font, new Size(width, 9999), flags).Height;

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
                            Y = height - h,
                            Width = RenderingPipelineManager.MeasureText(text.Substring(range.First, range.Length), font, new Size(width, 9999), flags).Width,
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
                            Rectangle region = new Rectangle
                            {
                                X = RenderingPipelineManager.MeasureText(text.Substring(lineStart, range.First - lineStart), font, new Size(width, 9999), flags).Width,
                                Y = height - h,
                                Width = RenderingPipelineManager.MeasureText(text.Substring(range.First, range.Length), font, new Size(width, 9999), flags).Width,
                                Height = h,
                            };

                            if (region.X > 0)
                                region.X -= leading;
                            currentInfo = new ButtonInfo { Region = region, Index = range.First, Length = range.Length };
                            regions.Add(currentInfo);
                        }
                        else
                        {
                            //Measure Current.Index to range.First + Length
                            currentInfo.Length = range.First + range.Length - currentInfo.Index;
                            currentInfo.Region.Width = RenderingPipelineManager.MeasureText(text.Substring(currentInfo.Index, currentInfo.Length), font, new Size(width, 9999), flags).Width;
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
