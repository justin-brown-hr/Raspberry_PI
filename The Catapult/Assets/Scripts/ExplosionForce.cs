using System.Collections;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
	public float force = 50f;

	public float radius = 5f;

	public float upliftModifer = 5f;

	public void doExplosion(Vector3 position)
	{
		base.transform.localPosition = position;
		StartCoroutine(waitAndExplode());
	}

	private IEnumerator waitAndExplode()
	{
		yield return new WaitForFixedUpdate();
		Collider2D[] colliders = Physics2D.OverlapCircleAll(base.transform.position, radius);
		Collider2D[] array = colliders;
		foreach (Collider2D collider2D in array)
		{
			if ((bool)collider2D.GetComponent<Rigidbody2D>() && collider2D.name != "hero")
			{
				AddExplosionForce(collider2D.GetComponent<Rigidbody2D>(), force, base.transform.position, radius, upliftModifer);
			}
		}
	}

	private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0f)
	{
		Vector3 vector = body.transform.position - explosionPosition;
		float d = 1f - vector.magnitude / explosionRadius;
		Vector3 v = vector.normalized * explosionForce * d;
		v.z = 0f;
		body.AddForce(v);
		if (upliftModifer != 0f)
		{
			float d2 = 1f - upliftModifier / explosionRadius;
			Vector3 v2 = Vector2.up * explosionForce * d2;
			v2.z = 0f;
			body.AddForce(v2);
		}
	}
}
