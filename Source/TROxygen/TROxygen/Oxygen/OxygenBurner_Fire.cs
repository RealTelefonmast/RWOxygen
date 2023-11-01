using System;
using System.Collections.Generic;
using RimWorld;
using TAC;
using TAC.Atmosphere;
using TeleCore;
using UnityEngine;
using Verse;

namespace TROxygen.Oxygen;

public class OxygenBurner_Fire : OxygenBurner
{
    private Fire _sourceFire;
    
    public override double BurningRate => 0.25f;
    
    protected IEnumerable<RoomComponent_Atmosphere> Atmospheres
    {
        get
        {
            foreach (var room in _sourceThing.GetRoomsAround())
            {
                yield return room.GetRoomComp<RoomComponent_Atmosphere>();
            }
        }
    }
    
    public OxygenBurner_Fire(Fire fire) : base(fire)
    {
        _sourceFire = fire;
    }
    
    public override void Tick()
    {
        if (GenTicks.TicksAbs % 2 != 0) return;
        
        //If a wall, use atmospheres surrounding it
        if (_sourceFire.parent != null && _sourceFire.parent.def.IsRoomDivider())
        {
            foreach (var atmos in Atmospheres)
            {
                BurnOxygen(atmos);
            }
            return;   
        }

        if (Atmosphere == null)
        {
            Log.WarningOnce($"Tried to tick oxygen burner with thing without a room: {_sourceThing}", _sourceFire.GetHashCode());
            return;
        }
        
        //Otherwise burn oxygen in the direct room
        BurnOxygen(Atmosphere);
    }

    private void BurnOxygen(RoomComponent_Atmosphere atmos)
    {
        var saturation = atmos.Volume.StoredPercentOf(OxygenDefOf.Atmosphere_Oxygen);
        var result = atmos.Volume.TryRemove(OxygenDefOf.Atmosphere_Oxygen, BurningRate);
        if(result.Actual.Value > 0) //[NMODefOf.Atmosphere_Oxygen]
        {
            atmos.Volume.TryAdd(OxygenDefOf.Atmosphere_CarbonMonoxide, 0.125f);
        }
        
        var exp = Fire.MaxFireSize * ((saturation * saturation) * 10);
        _sourceFire.fireSize = Mathf.Clamp((float)Math.Round(_sourceFire.fireSize, 2), 0, exp);
    }
}