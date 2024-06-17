using UnityEngine;

public abstract class UpgradeBase : ScriptableObject
{
    [Header("Basic Info"), Space]
    public Sprite icon;
    public string upgradeName;
    public string description;
    public int goldCost;

	// Properties.
	public bool IsApplied { get; set; }
    
    public abstract void DoUpgrade();
	public abstract void RemoveUpgrade();
}