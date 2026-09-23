using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;

	[HideInInspector]
	public bool jump;

	public float moveForce = 365f;

	public float maxSpeed = 5f;

	public AudioClip[] jumpClips;

	public float jumpForce = 1000f;

	public AudioClip[] taunts;

	public float tauntProbability = 50f;

	public float tauntDelay = 1f;

	private int tauntIndex;

	private Transform groundCheck;

	private bool grounded;

	private Animator anim;

	private void Awake()
	{
		Physics2D.gravity = new Vector2(0f, -30f);
		groundCheck = base.transform.Find("groundCheck");
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		RaycastHit2D[] array = Physics2D.LinecastAll(base.transform.position, groundCheck.position);
		grounded = false;
		if (array != null)
		{
			RaycastHit2D[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D = array2[i];
				if (FakeTag.IsTag(raycastHit2D.collider, "ground"))
				{
					grounded = true;
				}
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			jump = true;
		}
	}

	private void FixedUpdate()
	{
		float axis = UnityEngine.Input.GetAxis("Horizontal");
		anim.SetFloat("Speed", Mathf.Abs(axis));
		float num = axis;
		Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
		if (num * velocity.x < maxSpeed)
		{
			GetComponent<Rigidbody2D>().AddForce(Vector2.right * axis * moveForce);
		}
		Vector2 velocity2 = GetComponent<Rigidbody2D>().velocity;
		if (Mathf.Abs(velocity2.x) > maxSpeed)
		{
			Rigidbody2D component = GetComponent<Rigidbody2D>();
			Vector2 velocity3 = GetComponent<Rigidbody2D>().velocity;
			float x = Mathf.Sign(velocity3.x) * maxSpeed;
			Vector2 velocity4 = GetComponent<Rigidbody2D>().velocity;
			component.velocity = new Vector2(x, velocity4.y);
		}
		if (axis > 0f && !facingRight)
		{
			Flip();
		}
		else if (axis < 0f && facingRight)
		{
			Flip();
		}
		if (jump)
		{
			anim.SetTrigger("Jump");
			int num2 = UnityEngine.Random.Range(0, jumpClips.Length);
			AudioSource.PlayClipAtPoint(jumpClips[num2], base.transform.position);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
			jump = false;
		}
	}

	private void Flip()
	{
		facingRight = !facingRight;
		Vector3 localScale = base.transform.localScale;
		localScale.x *= -1f;
		base.transform.localScale = localScale;
	}

	public IEnumerator Taunt()
	{
		float tauntChance = UnityEngine.Random.Range(0f, 100f);
		if (tauntChance > tauntProbability)
		{
			yield return new WaitForSeconds(tauntDelay);
			if (!GetComponent<AudioSource>().isPlaying)
			{
				tauntIndex = TauntRandom();
				GetComponent<AudioSource>().clip = taunts[tauntIndex];
				GetComponent<AudioSource>().Play();
			}
		}
	}

	private int TauntRandom()
	{
		int num = UnityEngine.Random.Range(0, taunts.Length);
		if (num == tauntIndex)
		{
			return TauntRandom();
		}
		return num;
	}
}
