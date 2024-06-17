using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References"), Space]
	[SerializeField] private Stats stats;
    [SerializeField] private Rigidbody2D rb2D;
	[SerializeField] private Animator animator;
    
    [Header("Mobility Settings"), Space]
	[SerializeField] private float acceleration;
	[SerializeField] private float deceleration;

	// Properties.
	public static Vector2 Position { get; private set; }

	// Private fields.
	private Vector2 _movementDirection;
	private Vector2 _previousDirection;
	private float _currentSpeed;

	private void Update()
	{
		_movementDirection.x = InputManager.Instance.GetAxisRaw("Horizontal");
		_movementDirection.y = InputManager.Instance.GetAxisRaw("Vertical");
		_movementDirection.Normalize();

		if (_movementDirection.sqrMagnitude > .01f)
			_previousDirection = _movementDirection;
	}
	
	private void FixedUpdate()
	{
		if (GameManager.Instance.GameFinished)
			return;
		
		UpdateVelocity();
	}

	private void UpdateVelocity()
	{
		if (_movementDirection.sqrMagnitude > .01f)
		{
			_currentSpeed += acceleration * Time.deltaTime;
			_currentSpeed = Mathf.Min(stats.GetDynamicStat(Stat.MoveSpeed), _currentSpeed);
			
			rb2D.velocity = _movementDirection * _currentSpeed;
		}

		else if (_currentSpeed > 0f)
		{
			_currentSpeed -= deceleration * Time.deltaTime;
			_currentSpeed = Mathf.Max(0f, _currentSpeed);

			rb2D.velocity = _previousDirection * _currentSpeed;
		}

		animator.SetFloat("Speed", rb2D.velocity.sqrMagnitude);
		Position = rb2D.position;
	}
}
