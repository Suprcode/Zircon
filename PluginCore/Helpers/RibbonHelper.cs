using System;
using System.Drawing;
using System.Text;

using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace PluginCore.Helpers
{
    public static class RibbonHelper
    {
        public static RibbonPageGroup AddGroup(IPluginStart start, RibbonPage page, string title)
        {
            var group = new RibbonPageGroup
            {
                AllowTextClipping = false,
                Name = $"{start.Name}Group",
                ShowCaptionButton = false,
                Text = title
            };

            page.Groups.Add(group);

            return group;
        }

        public static void AddButton(IPluginStart start, RibbonPageGroup group, string caption, Image largeIcon, ItemClickEventHandler itemClickEventHandler)
        {
            var button = new BarButtonItem { Caption = caption };

            button.ImageOptions.Image = largeIcon;
            button.ImageOptions.LargeImage = largeIcon;
            button.Name = $"{start.Name}{caption.Replace(" ", "")}";
            button.ItemClick += itemClickEventHandler;

            group.ItemLinks.Add(button);
        }
    }
}
