using UnityEngine;

[RequireComponent(typeof(Explodable))]
public class ExplodeOnClick : MonoBehaviour
{
	private Explodable _explodable;

	private void Start()
	{
		_explodable = GetComponent<Explodable>();
	}

	private void OnMouseDown()
	{
		_explodable.explode(null);
		ExplosionForce explosionForce = UnityEngine.Object.FindObjectOfType<ExplosionForce>();
		explosionForce.doExplosion(base.transform.position);
	}
}
