using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	public float healthBonus;

	public AudioClip collect;

	private PickupSpawner pickupSpawner;

	private Animator anim;

	private bool landed;

	private void Awake()
	{
		pickupSpawner = GameObject.Find("pickupManager").GetComponent<PickupSpawner>();
		anim = base.transform.root.GetComponent<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (FakeTag.IsTag(other, "Player"))
		{
			PlayerHealth component = other.GetComponent<PlayerHealth>();
			component.health += healthBonus;
			component.health = Mathf.Clamp(component.health, 0f, 100f);
			component.UpdateHealthBar();
			pickupSpawner.StartCoroutine(pickupSpawner.DeliverPickup());
			AudioSource.PlayClipAtPoint(collect, base.transform.position);
			UnityEngine.Object.Destroy(base.transform.root.gameObject);
		}
		else if (FakeTag.IsTag(other, "ground") && !landed)
		{
			anim.SetTrigger("Land");
			base.transform.parent = null;
			base.gameObject.AddComponent<Rigidbody2D>();
			landed = true;
		}
	}
}
