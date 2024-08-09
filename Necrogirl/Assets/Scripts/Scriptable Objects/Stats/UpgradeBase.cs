using UnityEngine;

public abstract class UpgradeBase : IdentifiableSO
{
    [Header("Gold Cost"), Space]
    public int goldCost;

	// Properties.
	public bool IsApplied { get; set; }
    
    public abstract void DoUpgrade();
	public abstract void RemoveUpgrade();
}