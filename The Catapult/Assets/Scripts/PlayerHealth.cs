using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public float health = 100f;

	public float repeatDamagePeriod = 2f;

	public AudioClip[] ouchClips;

	public float hurtForce = 10f;

	public float damageAmount = 10f;

	private SpriteRenderer healthBar;

	private float lastHitTime;

	private Vector3 healthScale;

	private PlayerControl playerControl;

	private Animator anim;

	private void Awake()
	{
		playerControl = GetComponent<PlayerControl>();
		healthBar = GameObject.Find("HealthBar").GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		healthScale = healthBar.transform.localScale;
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (!FakeTag.IsTag(col.collider, "Enemy") || !(Time.time > lastHitTime + repeatDamagePeriod))
		{
			return;
		}
		if (health > 0f)
		{
			TakeDamage(col.transform);
			lastHitTime = Time.time;
			return;
		}
		Collider2D[] components = GetComponents<Collider2D>();
		Collider2D[] array = components;
		foreach (Collider2D collider2D in array)
		{
			collider2D.isTrigger = true;
		}
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
		SpriteRenderer[] array2 = componentsInChildren;
		foreach (SpriteRenderer spriteRenderer in array2)
		{
			spriteRenderer.sortingLayerName = "UI";
		}
		GetComponent<PlayerControl>().enabled = false;
		GetComponentInChildren<Gun>().enabled = false;
		anim.SetTrigger("Die");
	}

	private void TakeDamage(Transform enemy)
	{
		playerControl.jump = false;
		Vector3 a = base.transform.position - enemy.position + Vector3.up * 5f;
		GetComponent<Rigidbody2D>().AddForce(a * hurtForce);
		health -= damageAmount;
		UpdateHealthBar();
		int num = UnityEngine.Random.Range(0, ouchClips.Length);
		AudioSource.PlayClipAtPoint(ouchClips[num], base.transform.position);
	}

	public void UpdateHealthBar()
	{
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1f - health * 0.01f);
		healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.01f, 1f, 1f);
	}
}
