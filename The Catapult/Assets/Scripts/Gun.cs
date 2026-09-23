using UnityEngine;

public class Gun : MonoBehaviour
{
	public Rigidbody2D rocket;

	public float speed = 20f;

	private PlayerControl playerCtrl;

	private Animator anim;

	private void Awake()
	{
		anim = base.transform.root.gameObject.GetComponent<Animator>();
		playerCtrl = base.transform.root.GetComponent<PlayerControl>();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			anim.SetTrigger("Shoot");
			GetComponent<AudioSource>().Play();
			if (playerCtrl.facingRight)
			{
				Rigidbody2D rigidbody2D = UnityEngine.Object.Instantiate(rocket, base.transform.position, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
				rigidbody2D.velocity = new Vector2(speed, 0f);
			}
			else
			{
				Rigidbody2D rigidbody2D2 = UnityEngine.Object.Instantiate(rocket, base.transform.position, Quaternion.Euler(new Vector3(0f, 0f, 180f)));
				rigidbody2D2.velocity = new Vector2(0f - speed, 0f);
			}
		}
	}
}
