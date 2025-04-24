using System.Collections.Generic;
using UnityEngine;

public class SpeedPortal : InteractiveObject
{
    private enum SpeedPortalType
    {
        Slow, // x0.8
        Normal, // x1 
        Fast, // x1.25
        VeryFast, // x1.5
        UltraFast  // 1.85
    }
    
    private Dictionary<SpeedPortalType, float> speedMultipliers = new Dictionary<SpeedPortalType, float>
    {
        { SpeedPortalType.Slow, 0.8f },
        { SpeedPortalType.Normal, 1f },
        { SpeedPortalType.Fast, 1.25f },
        { SpeedPortalType.VeryFast, 1.5f },
        { SpeedPortalType.UltraFast, 1.85f }
    };

    [SerializeField] private SpeedPortalType speedPortalType;

    public override void Interact(PlayerController player)
    {
        if (player == null) return;
        
        player.speedModifier = speedMultipliers[speedPortalType];
    }
}