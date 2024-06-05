using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("Enemy List"), Space]
    [SerializeField] private Transform container;
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

	[Header("Spawning Settings"), Space]
    [SerializeField] private Vector2Int spawnCount;
    public float range;
	[HideInInspector] public Vector2 position;

    private void Awake()
    {
		position = transform.position;
        int count = Random.Range(spawnCount.x, spawnCount.y);

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * range;

            GameObject prefab = enemies[Random.Range(0, enemies.Count)];
            GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
            enemy.name = prefab.name;
            enemy.transform.SetParent(container);
			enemy.GetComponent<MeleeEnemyAI>().SetSpawnArea(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}