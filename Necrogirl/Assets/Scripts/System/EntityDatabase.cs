using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

public class EntityDatabase : Singleton<EntityDatabase>
{
	[Header("Entity Collections"), Space]
	[SerializeField] private SerializedDictionary<EntityName, GameObject> entities = new SerializedDictionary<EntityName, GameObject>();
	public List<Stats> enemyStats = new List<Stats>();
	public List<Stats> unitStats = new List<Stats>();
	public List<UpgradeBase> upgrades = new List<UpgradeBase>(); 

	private void Start()
    {
        enemyStats.ForEach(stat => stat.ClearUpgrades());
		unitStats.ForEach(stat => stat.ClearUpgrades());
		upgrades.ForEach(upgrade => upgrade.IsApplied = false);
    }

	public bool TryGetEntity(EntityName name, out GameObject value)
	{
		return entities.TryGetValue(name, out value);
	}

	public bool TryGetEntityByIndex(EntityType type, int index, out GameObject value)
	{
		List<EntityName> entityNames = new List<EntityName>(entities.Keys);
		int enemyCount = enemyStats.Count;

		switch (type)
		{
			case EntityType.Unit:
				return entities.TryGetValue(entityNames[index + enemyCount], out value);
			case EntityType.Enemy:
			case EntityType.Any:
			default:
				return entities.TryGetValue(entityNames[index], out value);
		}
	}
}

public enum EntityType { Unit, Enemy, Any }
public enum EntityName
{
	// Enemies.
	GoblinAssassin,
	GoblinArcher,
	GoblinShaman,
	Demon,
	Succubus,

	// Units.
	BlackKnight,
	WhiteKnight,
	Dwarf,
	Elf,
	Priest
}