using Client.Envir;
using Client.Models;
using Library;
using Shared.Rendering;
using System.Drawing;

namespace Client.Scenes.Views
{
    public sealed partial class MapControl
    {
        private RenderTargetResource worldNamesTarget;
        private UICacheKey worldNamesRenderKey;
        private Point worldNamesTile;
        private (bool Players, bool NPCs, bool Monsters, bool CompactLoot) worldNamesSettings;
        private bool worldNamesValid;

        private void DrawWorldNames()
        {
            // Names formerly lived in the map texture. Keep the same refresh cadence,
            // but use physical pixels so window/UI scaling does not blur the text.
            if (!HasGroundItems || !Config.ShowItemNames || !RenderingPipelineManager.CanCacheUI)
            {
                ReleaseWorldNames();
                DrawWorldNamesDirect();
                return;
            }

            RenderingPipelineManager.SetUIScaleOrigin(PointF.Empty);
            UICacheKey renderKey = RenderingPipelineManager.GetUICacheKey(
                CEnvir.Target.WindowScale, CEnvir.Target.TextRasterScale);
            // World names have individual scale origins; the cache covers the viewport.
            Rectangle bounds = new Rectangle(Point.Empty, renderKey.BackBuffer);
            var settings = (Config.ShowPlayerNames, Config.ShowNPCNames, Config.ShowMonsterNames, Config.DenseLoot);
            if (renderKey != worldNamesRenderKey)
                ReleaseWorldNames();

            if (!worldNamesTarget.IsValid)
            {
                worldNamesTarget = RenderingPipelineManager.RentUICacheTarget(bounds.Size);
                worldNamesValid = false;
            }

            if (!worldNamesValid || worldNamesTile != MapLocation || worldNamesSettings != settings)
            {
                worldNamesValid = false;
                using (RenderingPipelineManager.PushUICacheTarget(worldNamesTarget.Surface, bounds))
                {
                    RenderingPipelineManager.Clear(RenderClearFlags.Target, Color.FromArgb(0), 0, 0);
                    DrawWorldNamesDirect();
                }
                worldNamesRenderKey = renderKey;
                worldNamesTile = MapLocation;
                worldNamesSettings = settings;
                worldNamesValid = true;
            }

            RenderingPipelineManager.PresentUICache(worldNamesTarget.Texture, bounds, bounds);
        }

        private void ReleaseWorldNames()
        {
            if (worldNamesTarget.IsValid)
                RenderingPipelineManager.ReturnUICacheTarget(worldNamesTarget);
            worldNamesTarget = default;
            worldNamesValid = false;
        }

        private void DrawWorldNamesDirect()
        {
            foreach (MapObject ob in HasGroundItems ? nameOverlayObjects : Objects)
            {
                if (ob.Dead) continue;
                switch (ob.Race)
                {
                    case ObjectType.Player:
                        if (!Config.ShowPlayerNames) continue;
                        break;
                    case ObjectType.Item:
                        if (!Config.ShowItemNames || ob.CurrentLocation == MapLocation) continue;
                        break;
                    case ObjectType.NPC:
                        if (!Config.ShowNPCNames) continue;
                        break;
                    case ObjectType.Monster:
                        if (!Config.ShowMonsterNames) continue;
                        break;
                }
                SetWorldOverlayScaleOrigin(ob);
                ob.DrawName();
            }
        }
    }
}
