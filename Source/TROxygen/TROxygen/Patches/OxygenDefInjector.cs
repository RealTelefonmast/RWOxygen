using System.Collections.Generic;
using RimWorld;
using TeleCore;
using TeleCore.Data.Logging;
using TROxygen.Oxygen;
using TRR;
using Verse;

namespace TROxygen.Patches;

public class OxygenDefInjection : DefInjectBase
{
    public override bool AcceptsSpecial(Def def)
    {
        return def is PawnKindDef;
    }

    public override void OnDefSpecialInjected(Def def)
    {
        if (def is PawnKindDef kindDef)
        {
            if (kindDef.race.race is { IsMechanoid: false } && !kindDef.HasModExtension<RespirationExtension>())
            {
                kindDef.modExtensions ??= new List<DefModExtension>();
                kindDef.modExtensions.Add(new RespirationExtension
                {
                    inhaledGas = OxygenDefOf.Atmosphere_Oxygen,
                    exhaledGas = OxygenDefOf.Atmosphere_CarbonDioxide,
                    worker = new RespirationWorker_Breathing
                    {
                        hypo = OxygenDefOf.Hypoxia,
                        hyper = OxygenDefOf.Hyperoxia,
                        recoverySicknessHypo = OxygenDefOf.HypoxiaSickness,
                        recoverySicknessHyper = null,
                    },
                });
            }
        }
    }
}