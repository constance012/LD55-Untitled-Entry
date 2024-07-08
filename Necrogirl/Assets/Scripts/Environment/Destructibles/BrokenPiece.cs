using UnityEngine;
using DG.Tweening;

public class BrokenPiece : MonoBehaviour
{
	[Header("References"), Space]
    [SerializeField] private Rigidbody2D rb2D;

	[Header("Settings"), Space]
	[SerializeField] private Vector2 torqueRange;
	[SerializeField] private Vector2 forceRange;

    private void Start()
    {
        rb2D.AddForce(Random.insideUnitCircle.normalized * Random.Range(forceRange.x, forceRange.y), ForceMode2D.Impulse);
        rb2D.AddTorque(Random.Range(torqueRange.x, torqueRange.y), ForceMode2D.Impulse);

        transform.DOScale(0f, .3f).SetDelay(Random.Range(.7f, 1.2f)).OnComplete(() => Destroy(gameObject));
    }
}
