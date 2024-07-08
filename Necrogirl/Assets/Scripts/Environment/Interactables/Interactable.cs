using System;
using UnityEngine;

/// <summary>
/// Base class for all interactable objects.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Interactable : MonoBehaviour
{
	public enum InputSource { Mouse, Keyboard, Joystick, None }

	public enum InteractableType
	{
		/// <summary>
		/// Can only be controlled by other mechanisms.
		/// </summary>
		Passive,

		/// <summary>
		/// Can either be controlled by other mechanisms or interacted by the player.
		/// </summary>
		Active,

		/// <summary>
		/// Can only be interacted manually by the player.
		/// </summary>
		Manual
	}

	[Header("Type"), Space]
	public InteractableType type;
	public InputSource inputSource;

	[Header("Reference"), Space]
	public Transform player;
	[SerializeField] protected GameObject popupLabelPrefab;

	[Header("Interaction Radius"), Space]	
	[SerializeField, Tooltip("The distance required for the player to interact with this object.")]
	protected float interactDistance;
	
	[SerializeField, ReadOnly] protected bool hasInteracted;

	//[Header("Dialogue (Optional)")]
	//[ReadOnly] public DialogueTrigger dialogueTrigger;
	//[ReadOnly] public bool oneTimeDialogueTriggered;

	//public bool HasDialogue => dialogueTrigger != null;
	
	// Protected fields.
	protected Transform _worldCanvas;
	protected SpriteRenderer _spriteRenderer;
	protected Material _mat;
	protected InteractionPopupLabel _popupLabel;

	protected virtual void Awake()
	{
		if (player == null)
			player = GameObject.FindWithTag("Player").transform;

		_worldCanvas = GameObject.FindWithTag("WorldCanvas").transform;
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_mat = _spriteRenderer.material;
	}

	protected void Update()
	{
		if (type == InteractableType.Passive)
			return;

		float playerDistance = Vector2.Distance(player.position, transform.position);

		CheckForInteraction(0f, playerDistance);
	}

	#region Dialogue Triggering and Remote Controlling.
	/// <summary>
	/// Bind this function to an Ink story for external function execution.
	/// </summary>
	public virtual void InkExternalFunction() { }

	/// <summary>
	/// This method is responsible for being executed by other <c>Interactable</c> objects.
	/// </summary>
	public virtual void ExecuteRemoteLogic(bool state)
	{
		Debug.Log($"Execute logic of {transform.name} remotely.");
	}
	#endregion

	#region Interaction Methods.
	public virtual void Interact()
	{
		Debug.Log($"Interacting with {transform.name}.");

		//if (HasDialogue)
		//	dialogueTrigger.TriggerDialogue();
	}

	protected virtual void CheckForInteraction(float mouseDistance, float playerDistance)
	{
		if (playerDistance <= interactDistance)
		{
			TriggerInteraction(playerDistance);
		}
		else
		{
			CancelInteraction(playerDistance);
		}
	}

	protected virtual void TriggerInteraction(float playerDistance)
	{
		if (_popupLabel == null)
			CreatePopupLabel();
		else
			_popupLabel.transform.position = transform.position;

		_mat.SetFloat("_Thickness", 1f);

		// TODO - derived classes implement their own way to trigger interaction.
	}

	protected virtual void CancelInteraction(float playerDistance)
	{
		if (_popupLabel != null)
			Destroy(_popupLabel.gameObject);

		_mat.SetFloat("_Thickness", 0f);

		// TODO - derived classes implement their own way to cancel interaction.
	}
	#endregion

	protected virtual void CreatePopupLabel()
	{
		GameObject label = Instantiate(popupLabelPrefab);
		label.name = popupLabelPrefab.name;

		_popupLabel = label.GetComponent<InteractionPopupLabel>();

		_popupLabel.SetupLabel(this.transform, inputSource);
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, interactDistance);
	}
}
