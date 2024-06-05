using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class StatsUpgrade : UpgradeBase
{
	[Header("Detail"), Space]
	public List<Stats> unitsToApply = new List<Stats>();
	public SerializedDictionary<Stat, float> affectedStats = new SerializedDictionary<Stat, float>();
	public bool isPercentageUpgrade;

	public override void DoUpgrade()
	{
		foreach (Stats unit in unitsToApply)
		{
			unit.AddUpgrade(this);
		}
	}
}