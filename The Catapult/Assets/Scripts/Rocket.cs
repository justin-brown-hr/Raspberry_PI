using UnityEngine;

public class Rocket : MonoBehaviour
{
	public GameObject explosion;

	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, 2f);
	}

	private void OnExplode()
	{
		Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		Object.Instantiate(explosion, base.transform.position, rotation);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (FakeTag.IsTag(col, "Enemy"))
		{
			col.gameObject.GetComponent<Enemy>().Hurt();
			OnExplode();
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (FakeTag.IsTag(col, "BombPickup"))
		{
			col.gameObject.GetComponent<Bomb>().Explode();
			UnityEngine.Object.Destroy(col.transform.root.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (!FakeTag.IsTag(col, "Player"))
		{
			OnExplode();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
