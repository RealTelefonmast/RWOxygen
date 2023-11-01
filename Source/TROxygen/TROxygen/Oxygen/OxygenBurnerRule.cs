using RimWorld;
using TAC.Atmosphere;
using Verse;

namespace TROxygen.Oxygen;

public class OxygenBurnerRule : AtmosphericConverterFromThingRule
{
    public override AtmosphereConverterBase ConverterFor(Thing thing)
    {
        switch (thing)
        {
            case ThingWithComps twc:
            {
                if (OxygenUtility.IsFuelBurner(twc))
                {
                    return new OxygenBurne_Fuel(twc);
                }
                break;
            }
            case Fire fire:
                return new OxygenBurner_Fire(fire);
        }
        return null;
    }
}