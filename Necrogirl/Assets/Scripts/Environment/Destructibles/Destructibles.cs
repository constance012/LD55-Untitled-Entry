using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Destructibles : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private List<GameObject> brokenPieces;
	[SerializeField] private GameObject hpBottle;
	[SerializeField] private GameObject manaBottle;
	[SerializeField] private GameObject coin;

	[Header("Drop Settings"), Space]
	[SerializeField] private Vector2Int coinCount;
	[SerializeField, Range(0f, 1f)] private float manaDropChance;
	[SerializeField, Range(0f, 1f)] private float healthDropChance;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Our Projectile"))
		{
			if(Random.value < healthDropChance)
			{
				GameObject healthPotion = Instantiate(hpBottle, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
				healthPotion.name = hpBottle.name;
			}

			if (Random.value < manaDropChance)
			{
				GameObject manaPotion = Instantiate(manaBottle, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
				manaPotion.name = manaBottle.name;
			}
			
			int coinQuantity = Random.Range(coinCount.x, coinCount.y);
			GameObject coins = Instantiate(coin, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
			coins.name = coin.name;
			coins.GetComponent<ItemPickup>().ItemQuantity = coinQuantity;

			int pieceCount = Random.Range(5, 11);
			for(int i = 0; i < pieceCount; i++)
			{
				GameObject piecePrefab = brokenPieces[Random.Range(0, brokenPieces.Count)];
				Instantiate(piecePrefab, transform.position, Quaternion.identity);
			}
			
			Destroy(gameObject);
		}
	}
}
