using System;
using UnityEngine;

public enum CursorTextureType { Default, Clicked }

public class CursorManager : Singleton<CursorManager>
{
	[Serializable]
	public struct CustomCursor
	{
		public CursorTextureType type;

		public Texture2D texture;

		[Tooltip("The normalized position of the texture to be used as a hotspot, as (0, 0) will be at the top-left corner of the texture.")]
		public Vector2 relativeHotSpot;

		public Vector2 TextureHotSpot => new Vector2(texture.width * relativeHotSpot.x, texture.height * relativeHotSpot.y);
	}

	[Header("Custom Cursors"), Space]
	[SerializeField] private CustomCursor defaultCursor;
	[SerializeField] private CustomCursor onClickedCursor;
	[SerializeField] private bool showInEditor;

	private void Update()
	{
		if (Application.isEditor && !showInEditor)
			return;

		if (LegacyInputManager.Instance.GetKey(KeybindingActions.PrimaryAttack))
			SwitchCursorTexture(CursorTextureType.Clicked);
		else
			SwitchCursorTexture(CursorTextureType.Default);
	}

	public void SwitchCursorTexture(CursorTextureType type, CursorMode mode = CursorMode.ForceSoftware)
	{
		switch (type)
		{
			case CursorTextureType.Default:
				Cursor.SetCursor(defaultCursor.texture, defaultCursor.TextureHotSpot, mode);
				break;

			case CursorTextureType.Clicked:
				Cursor.SetCursor(onClickedCursor.texture, onClickedCursor.TextureHotSpot, mode);
				break;
		}
	}

	public void SetVisible(bool visible)
	{
		Cursor.visible = visible;
	}
}
