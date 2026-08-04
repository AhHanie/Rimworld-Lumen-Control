using RimWorld;
using UnityEngine;
using Verse;

namespace Lumen_Control
{
    public static class LightPowerConsumptionUtility
    {
        public static void UpdatePowerConsumption(CompGlower glower)
        {
            if (!ModSettings.ScalePowerConsumptionWithRadius)
            {
                return;
            }

            CompPowerTrader powerTrader = glower.parent.GetComp<CompPowerTrader>();
            if (powerTrader == null)
            {
                return;
            }

            CompProperties_Power powerProps = powerTrader.Props;
            if (powerProps.PowerConsumption <= 0f)
            {
                return;
            }

            bool usesIdleDraw = !powerTrader.PowerOn && !Mathf.Approximately(powerProps.idlePowerDraw, -1f);
            if (usesIdleDraw)
            {
                return;
            }

            float extraRadius = Mathf.Max(0f, glower.GlowRadius - glower.Props.glowRadius);
            float adjustedWatts = powerProps.PowerConsumption * (1f + (ModSettings.PowerCostPercentPerRadius / 100f) * extraRadius);
            powerTrader.PowerOutput = -adjustedWatts;
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
                    if (glower == null)
                    {
                        continue;
                    }

                    CompPowerTrader powerTrader = thing.TryGetComp<CompPowerTrader>();
                    if (powerTrader == null)
                    {
                        continue;
                    }

                    powerTrader.SetUpPowerVars();
                    UpdatePowerConsumption(glower);
                }
            }
        }
    }
}
