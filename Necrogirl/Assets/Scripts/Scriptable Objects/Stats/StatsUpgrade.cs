using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "Unit Stats/Upgrades", fileName = "New Blank Upgrade")]
public class StatsUpgrade : UpgradeBase
{
	[Header("Detail"), Space]
	public List<Stats> unitsToApply = new List<Stats>();
	public SerializedDictionary<Stat, float> affectedStats = new SerializedDictionary<Stat, float>();
	public bool isPercentageUpgrade;

	public override void DoUpgrade()
	{
		if (!IsApplied)
		{
			Debug.Log($"Applying \"{this.upgradeName}\" upgrade...");
			unitsToApply.ForEach(unit => unit.AddUpgrade(this));
			IsApplied = true;
		}
	}

	public override void RemoveUpgrade()
	{
		if (IsApplied)
		{
			Debug.Log($"Removing \"{this.upgradeName}\" upgrade...");
			unitsToApply.ForEach(unit => unit.RemoveUpgrade(this));
			IsApplied = false;
		}
	}
}