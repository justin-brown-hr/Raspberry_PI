using Exploder2D;
using Exploder2D.Utils;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public float moveSpeed = 2f;

	public int HP = 2;

	public Sprite deadEnemy;

	public Sprite damagedEnemy;

	public AudioClip[] deathClips;

	public GameObject hundredPointsUI;

	public float deathSpinMin = -100f;

	public float deathSpinMax = 100f;

	private SpriteRenderer ren;

	private Transform frontCheck;

	private bool dead;

	private Score score;

	private void Awake()
	{
		ren = base.transform.Find("body").GetComponent<SpriteRenderer>();
		frontCheck = base.transform.Find("frontCheck").transform;
		score = GameObject.Find("Score").GetComponent<Score>();
	}

	private void FixedUpdate()
	{
		Collider2D[] array = Physics2D.OverlapPointAll(frontCheck.position, 1);
		Collider2D[] array2 = array;
		foreach (Collider2D c in array2)
		{
			if (FakeTag.IsTag(c, "Obstacle"))
			{
				Flip();
				break;
			}
		}
		Rigidbody2D component = GetComponent<Rigidbody2D>();
		Vector3 localScale = base.transform.localScale;
		float x = localScale.x * moveSpeed;
		Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
		component.velocity = new Vector2(x, velocity.y);
		if (HP == 1 && damagedEnemy != null)
		{
			ren.sprite = damagedEnemy;
		}
		if (HP <= 0 && !dead)
		{
			Death();
		}
	}

	public void Hurt()
	{
		HP--;
	}

	private void Death()
	{
		dead = true;
		Exploder2DObject exploder2DInstance = Exploder2DSingleton.Exploder2DInstance;
		if ((bool)exploder2DInstance)
		{
			exploder2DInstance.gameObject.transform.position = Exploder2DUtils.GetCentroid(base.gameObject);
			exploder2DInstance.DontUseTag = true;
			exploder2DInstance.Radius = 1f;
			exploder2DInstance.Explode();
		}
		score.score++;
	}

	public void Flip()
	{
		Vector3 localScale = base.transform.localScale;
		localScale.x *= -1f;
		base.transform.localScale = localScale;
	}
}
