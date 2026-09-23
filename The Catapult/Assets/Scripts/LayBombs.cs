using UnityEngine;

public class LayBombs : MonoBehaviour
{
	[HideInInspector]
	public bool bombLaid;

	public int bombCount;

	public AudioClip bombsAway;

	public GameObject bomb;

	private SpriteRenderer bombHUD;

	private void Awake()
	{
		bombHUD = GameObject.Find("ui_bombHUD").GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire2") && !bombLaid && bombCount > 0)
		{
			bombCount--;
			bombLaid = true;
			AudioSource.PlayClipAtPoint(bombsAway, base.transform.position);
			Object.Instantiate(bomb, base.transform.position, base.transform.rotation);
		}
		bombHUD.enabled = (bombCount > 0);
	}
}
