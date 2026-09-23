using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	public float bombRadius = 10f;

	public float bombForce = 100f;

	public AudioClip boom;

	public AudioClip fuse;

	public float fuseTime = 1.5f;

	public GameObject explosion;

	private LayBombs layBombs;

	private PickupSpawner pickupSpawner;

	private ParticleSystem explosionFX;

	private void Awake()
	{
		explosionFX = GameObject.FindGameObjectWithTag("ExplosionFX").GetComponent<ParticleSystem>();
		pickupSpawner = GameObject.Find("pickupManager").GetComponent<PickupSpawner>();
		if ((bool)GameObject.FindGameObjectWithTag("Player"))
		{
			layBombs = GameObject.FindGameObjectWithTag("Player").GetComponent<LayBombs>();
		}
	}

	private void Start()
	{
		if (base.transform.root == base.transform)
		{
			StartCoroutine(BombDetonation());
		}
	}

	private IEnumerator BombDetonation()
	{
		AudioSource.PlayClipAtPoint(fuse, base.transform.position);
		yield return new WaitForSeconds(fuseTime);
		Explode();
	}

	public void Explode()
	{
		layBombs.bombLaid = false;
		pickupSpawner.StartCoroutine(pickupSpawner.DeliverPickup());
		Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, bombRadius, 1 << LayerMask.NameToLayer("Enemies"));
		Collider2D[] array2 = array;
		foreach (Collider2D collider2D in array2)
		{
			Rigidbody2D component = collider2D.GetComponent<Rigidbody2D>();
			if (component != null && FakeTag.IsTag(collider2D, "Enemy"))
			{
				component.gameObject.GetComponent<Enemy>().HP = 0;
				Vector3 v = (component.transform.position - base.transform.position).normalized * bombForce;
				component.AddForce(v);
			}
		}
		explosionFX.transform.position = base.transform.position;
		explosionFX.Play();
		Object.Instantiate(explosion, base.transform.position, Quaternion.identity);
		AudioSource.PlayClipAtPoint(boom, base.transform.position);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
