using UnityEngine;
using System;

namespace CSTGames.DataPersistence
{
	[Serializable]
	public struct ItemSaveData
	{
		public string id;
		public string itemName;
		public ItemCategory category;

		public int slotIndex;
		public int quantity;
		//public bool isFavorite;

		public ItemSaveData(Item itemToSave)
		{
			this.id = itemToSave.id;
			this.itemName = itemToSave.displayName;
			this.category = itemToSave.category;

			this.slotIndex = itemToSave.slotIndex;
			this.quantity = itemToSave.quantity;
			//this.isFavorite = itemToSave.isFavorite;
		}
	}

	[Serializable]
	public class GameData
	{
		public long lastUpdated;

		/// <summary>
		/// Initialize with default values of the data.
		/// </summary>
		public GameData()
		{
			this.lastUpdated = DateTime.Now.ToBinary();
		}
	}
}
