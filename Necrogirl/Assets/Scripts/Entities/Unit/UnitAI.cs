using UnityEngine;

public abstract class UnitAI : EntityAI
{
    [Header("Force Follow Distance"), Space]
	[SerializeField] private float forceFollowPlayerLimit;

	// Properties.
	public bool TargetIsPlayer => target == _player;

	// Protected fields.
	protected Transform _player = null;

	protected override void Start()
	{
		base.Start();
		_nearbyEntities.Add(this.rb2D);

		animator.Play("Unit Summoned");
	}

    protected override bool TrySelectTarget()
    {
        // Force the unit to follow the player if she goes far than a certain distance.
		if (Vector3.Distance(transform.position, PlayerMovement.Position) >= forceFollowPlayerLimit)
		{
			target = _player;
			return true;
		}

		int hitColliders = Physics2D.OverlapCircle(transform.position, aggroRange, _contactFilter, _hitTargets);

		if (hitColliders > 0)
		{
			_inAggroTargets.Clear();
			for (int i = 0; i < hitColliders; i++)
			{
				EntityStats entity = _hitTargets[i].GetComponentInParent<EntityStats>();

				if (entity != null)
					_inAggroTargets.Add(entity);
			}
			
			// Sort by priority if the list is not empty.
			if (_inAggroTargets.Count > 0)
			{
				_inAggroTargets.Sort();

				// Only assign new target if it's different from the previous one or the previous is null.
				if (target == null || target != _inAggroTargets[0].transform)
				{
					target = _inAggroTargets[0].transform;
					Debug.Log(target.name);

					if (_player == null && target.name.Equals("Player"))
						_player = target;
				}
			}

			return _inAggroTargets.Count > 0;
		} 

		return false;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, forceFollowPlayerLimit);
    }
}