using UnityEngine;

public class Black_hole : MonoBehaviour
{
	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, 0.3f);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag != "Player" || other.tag != "p_arms")
		{
			UnityEngine.Object.Destroy(other.gameObject);
		}
	}
}
