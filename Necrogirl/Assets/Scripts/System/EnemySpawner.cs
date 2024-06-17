using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
	[Header("Enemy List"), Space]
    [SerializeField] private Transform container;
	[SerializeField] private SerializedDictionary<EntityName, Vector2Int> enemiesToSpawn = new SerializedDictionary<EntityName, Vector2Int>();

	[Header("Spawning Settings"), Space]
	[SerializeField] private bool spawnAllRandomly;
    [SerializeField] private Vector2Int randomSpawnCount;
    public float range;
	[HideInInspector] public Vector2 position;

    private void Start()
    {
		position = transform.position;
        SpawnEnemies();
    }

	private void SpawnEnemies()
	{
		if (spawnAllRandomly)
		{
			int count = Random.Range(randomSpawnCount.x, randomSpawnCount.y + 1);

			for (int i = 0; i < count; i++)
			{
				Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * range;

				List<EntityName> enemyNames = new List<EntityName>(enemiesToSpawn.Keys);
				EntityName enemyName = enemyNames[Random.Range(0, enemyNames.Count)];
				EntityDatabase.Instance.TryGetEntity(enemyName, out GameObject prefab);

				GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
				enemy.name = prefab.name;
				enemy.transform.SetParent(container);
				enemy.GetComponent<MeleeEnemyAI>().SetSpawnArea(this);
			}
		}
		else
		{
			foreach (var pair in enemiesToSpawn)
			{
				int enemyCount = Random.Range(pair.Value.x, pair.Value.y + 1);
				for (int i = 0; i < enemyCount; i++)
				{
					Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * range;
					EntityDatabase.Instance.TryGetEntity(pair.Key, out GameObject prefab);

					GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
					enemy.name = prefab.name;
					enemy.transform.SetParent(container);
					enemy.GetComponent<MeleeEnemyAI>().SetSpawnArea(this);
				}
			}
		}
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}