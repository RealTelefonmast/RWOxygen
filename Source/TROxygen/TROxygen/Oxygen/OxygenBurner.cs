using System;
using JetBrains.Annotations;
using RimWorld;
using TAC.Atmosphere;
using TeleCore.Static;
using Verse;

namespace TROxygen.Oxygen;

//One mole of methane (about 16g) consumes two moles of oxygen to produce approximately 890.4 kilojoules (kJ) of heat energy.
//In terms of volume, 1 mole of any ideal gas at standard temperature and pressure (STP) occupies about 22.4 liters. Therefore,
//2 moles of O2 are roughly 44.8 liters.
//(100 liters / 44.8 liters) * 890.4 kJ = 1988.57 kJ
public abstract class OxygenBurner : AtmosphereConverterBase
{
    protected const double Jouls_Per_Oxygen_Liter = 20000; // 20kJ
    
    public virtual double BurningRate => 1;
    public sealed override bool IsActive => Atmosphere.Volume.StoredValueOf(OxygenDefOf.Atmosphere_Oxygen) > 0;
 
    public OxygenBurner([NotNull] Thing thing) : base(thing)
    {
    }
    
    public override void Tick()
    {
        
    }

}

//The generator provides 1000W power constantly for 6.6 days.
//1 watt (W) = 1 joule per second (J/s)
//so,
//1000 joules/second * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 6.6 days =
//1000 * 60 * 60 * 24 * 6.6 = 570,240,000 joules
//This is the total energy generated for 6.6 days using 30 units of fuel.
//Given these numbers, you could say that each fuel unit provides 570,240,000 joules / 30 = 19,008,000 joules
public class OxygenBurne_Fuel : OxygenBurner
{
    //NOTE: Dont forget burners generally run at 20-30% efficiency
    private const double Burner_Efficiency = 0.25d;
    
    //A chemfuel burner runs for 6.6d with 30 units of fuel producing 1000W or J/s
    private const double JoulPerRun = 570240000; //1000 joules/second * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 6.6 days
    private const double Jouls_Per_Day = 86400000;
    public const double Jouls_Per_Hour = 3600000;
    public const double Jouls_Per_Tick = 1440;
    private double JoulPerUnit = 19008000; //The amount of energy generated per unit of fuel
    
    public const double Ticks_Per_Liter_Burned = Jouls_Per_Oxygen_Liter / (((JoulPerRun / 6.6d) / 24d) / 2500);
    
    private CompRefuelable _fuelSource;
    private bool previouslyHadAtmosphere = false;

    public override double BurningRate => Math.Round((TickRate / Ticks_Per_Liter_Burned) * (100d / Burner_Efficiency), 2);
    public double BurningRate_CO => BurningRate * 0.05d;
    public double BurningRate_CO2 => BurningRate * 0.20d;
    public int TickRate => 90;
    
    public OxygenBurne_Fuel(ThingWithComps thing) : base(thing)
    {
        _fuelSource = thing.GetComp<CompRefuelable>();
    }

    public override void Tick()
    {
        if (GenTicks.TicksAbs % TickRate != 0) return;
        if (Atmosphere == null)
        {
            Log.Warning($"Tried to tick oxygen burner with thing without a room: {_sourceThing}");
            return;
        }

        if (!_fuelSource.HasFuel) return;

        var result = Atmosphere.Volume.TryRemove(OxygenDefOf.Atmosphere_Oxygen, BurningRate);
        if (result)
        {
            previouslyHadAtmosphere = true;
            Atmosphere.Volume.TryAdd(OxygenDefOf.Atmosphere_CarbonMonoxide, BurningRate_CO);
            Atmosphere.Volume.TryAdd(OxygenDefOf.Atmosphere_CarbonDioxide, BurningRate_CO2);
        }
        else if (previouslyHadAtmosphere)
        {
            previouslyHadAtmosphere = false;
            ((ThingWithComps)_sourceThing).BroadcastCompSignal(KnownCompSignals.RanOutOfFuel);
        }
    }
}