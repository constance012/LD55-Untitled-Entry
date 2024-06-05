using UnityEngine;

public abstract class UpgradeBase : ScriptableObject
{
    [Header("Basic Info"), Space]
    public Sprite icon;
    public string upgradeName;
    public string description;
    public int goldCost;
    
    public abstract void DoUpgrade();
}