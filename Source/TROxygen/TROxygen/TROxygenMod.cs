using HarmonyLib;
using Verse;

namespace TROxygen;

//TODO: Still planning much more, ThingComp for breathing, like an EVA backpack, or other oxygen-providing equipment.

public class TROxygenMod : Mod
{
    private static Harmony _oxygen;
        
    public TROxygenMod(ModContentPack content) : base(content)
    {
        _oxygen = new Harmony("telefonmast.realism.oxygen");
        _oxygen.PatchAll();
    }
}