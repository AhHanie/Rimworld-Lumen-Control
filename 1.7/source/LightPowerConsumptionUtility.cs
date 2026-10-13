using RimWorld;
using UnityEngine;
using Verse;

namespace Lumen_Control
{
    public static class LightPowerConsumptionUtility
    {
        public static float ScaleConsumption(CompPowerTrader powerTrader, float consumption)
        {
            if (!ModSettings.ScalePowerConsumptionWithRadius)
            {
                return consumption;
            }

            CompGlower glower = powerTrader.parent.GetComp<CompGlower>();
            if (glower == null)
            {
                return consumption;
            }

            float extraRadius = Mathf.Max(0f, glower.GlowRadius - glower.Props.glowRadius);
            return consumption * (1f + (ModSettings.PowerCostPercentPerRadius / 100f) * extraRadius);
        }

        public static void UpdatePowerConsumption(CompGlower glower)
        {
            glower.parent.GetComp<CompPowerTrader>()?.SetUpPowerVars();
        }

        public static void RefreshAllSpawnedLights()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            foreach (Map map in Find.Maps)
            {
                foreach (Thing thing in map.listerThings.ThingsInGroup(ThingRequestGroup.PowerTrader))
                {
                    CompGlower glower = thing.TryGetComp<CompGlower>();
                    if (glower != null)
                    {
                        UpdatePowerConsumption(glower);
                    }
                }
            }
        }
    }
}
