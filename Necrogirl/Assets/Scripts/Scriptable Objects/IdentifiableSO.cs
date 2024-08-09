using System;
using UnityEngine;

public class IdentifiableSO : ScriptableObject
{
	[ContextMenu("Generate ID")]
	private void GenerateID()
	{
		id = Guid.NewGuid().ToString();
	}

	[ContextMenu("Clear ID")]
	private void ClearID()
	{
		id = "";
	}

	[Header("ID"), Space, HideInInspector] public string id;

	[Header("Basic Info"), Space]
	public string displayName;
	[TextArea(5, 10)] public string description;
	public Sprite icon;
	public Rarity rarity;
}

[Serializable]
public struct Rarity
{
	public string title;
	public Sprite icon;
	public Color color;
}