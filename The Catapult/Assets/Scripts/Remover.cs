using UnityEngine;

public class Remover : MonoBehaviour
{
	public GameObject splash;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (FakeTag.IsTag(col, "Player"))
		{
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().enabled = false;
			if (GameObject.FindGameObjectWithTag("HealthBar").activeSelf)
			{
				GameObject.FindGameObjectWithTag("HealthBar").SetActive(value: false);
			}
			Object.Instantiate(splash, col.transform.position, base.transform.rotation);
			UnityEngine.Object.Destroy(col.gameObject);
			StartCoroutine("ReloadGame");
		}
	}
}
