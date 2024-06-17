using UnityEngine;

public class HealerUnitAI : UnitAI
{
	protected override void Start()
	{
		base.Start();
		target = GameObject.FindWithTag("Player").transform;
	}

	protected override void FixedUpdate()
    {
        if (PlayerStats.IsDeath)
		 	return;
        
        animator.SetFloat("Speed", rb2D.velocity.sqrMagnitude);

		if (animator.GetFloat("Speed") < .04f)
			_standingStillTimeout -= Time.deltaTime;
		else
			_standingStillTimeout = standingStillTimeout;

		RequestNewPath(PlayerMovement.Position);
	}
}