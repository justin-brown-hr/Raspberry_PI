using UnityEngine;

public class BombPickup : MonoBehaviour
{
	public AudioClip pickupClip;

	private Animator anim;

	private bool landed;

	private void Awake()
	{
		anim = base.transform.root.GetComponent<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (FakeTag.IsTag(other, "Player"))
		{
			AudioSource.PlayClipAtPoint(pickupClip, base.transform.position);
			other.GetComponent<LayBombs>().bombCount++;
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
