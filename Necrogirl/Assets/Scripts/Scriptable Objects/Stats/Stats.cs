using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using CSTGames.Utility;

[CreateAssetMenu(menuName = "Unit Stats", fileName = "New Blank Stats")]
public class Stats : ScriptableObject
{
	[Header("Stats"), Space]
	public SerializedDictionary<Stat, float> staticStats = new SerializedDictionary<Stat, float>();
	public SerializedDictionary<Stat, float> dynamicStats = new SerializedDictionary<Stat, float>();

	// Private fields.
	private readonly List<StatsUpgrade> appliedUpgrades = new List<StatsUpgrade>();
	private readonly HashSet<Stat> toStringIgnoreStats = new HashSet<Stat>()
	{
		Stat.ManaCost,
		Stat.LifeStealRatio,
		Stat.HealEfficiencyLossRatio,
		Stat.ProjectileSpeed,
		Stat.ProjectileLifeTime,
		Stat.ProjectileTrackingRigidity,
	};

	public void AddUpgrade(StatsUpgrade upgrade)
	{
		if (!appliedUpgrades.Contains(upgrade))
			appliedUpgrades.Add(upgrade);
	}

	public void ClearUpgrades()
	{
		appliedUpgrades.Clear();
	}

	public float GetStaticStat(Stat statName)
	{
		if (staticStats.TryGetValue(statName, out float value))
			return value;
		else
		{
			Debug.LogWarning($"No STATIC stat value found for {statName} on {this.name}");
			return -1f;
		}
	}
	
	public float GetDynamicStat(Stat statName)
	{
		if (dynamicStats.TryGetValue(statName, out float baseValue))
			return GetUpgradedValue(statName, baseValue);
		else
		{
			Debug.LogWarning($"No DYNAMIC stat value found for {statName} on {this.name}");
			return -1f;
		}
	}

	public void ModifyStat(Stat statName, float delta)
	{
		if (dynamicStats.TryGetValue(statName, out float _))
		{
			dynamicStats[statName] += delta;
		}
		else
		{
			Debug.LogError($"No DYNAMIC stat value found for {statName} on {this.name}");
		}
	}

	private float GetUpgradedValue(Stat stat, float baseValue)
	{
		foreach (StatsUpgrade upgrade in appliedUpgrades)
		{
			if (!upgrade.affectedStats.TryGetValue(stat, out float upgradeValue))
				continue;
			
			if (upgrade.isPercentageUpgrade)
				baseValue *= 1f + (upgradeValue / 100f);
			else
				baseValue += upgradeValue;
		}

		return baseValue;
	}

    public override string ToString()
    {
        string result = "";
		foreach (KeyValuePair<Stat, float> stat in dynamicStats)
		{
			if (!toStringIgnoreStats.Contains(stat.Key))
				result += $"{stat.Key.ToString().AddWhitespaceBeforeCapital()}: {stat.Value}\n";
		}
		result += "\n";
		foreach (KeyValuePair<Stat, float> stat in staticStats)
		{
			if (!toStringIgnoreStats.Contains(stat.Key))
				result += $"{stat.Key.ToString().AddWhitespaceBeforeCapital()}: {stat.Value}\n";
		}
		return result.TrimEnd('\r', '\n');
    }
}

public enum Stat
{
	// Dynamic.
	MaxHealth,
	MaxMana,
	Damage,
	AttackSpeed,
	MoveSpeed,

	// Static.
	KnockBackStrength,
	KnockBackRes,
	ManaCost,
	LifeStealRatio,
	HealEfficiencyLossRatio,
	ProjectileSpeed,
	ProjectileTrackingRigidity,
	ProjectileLifeTime
}