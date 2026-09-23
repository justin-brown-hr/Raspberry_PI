using UnityEngine;

public class Destroyer : MonoBehaviour
{
	public bool destroyOnAwake;

	public float awakeDestroyDelay;

	public bool findChild;

	public string namedChild;

	private void Awake()
	{
		if (destroyOnAwake)
		{
			if (findChild)
			{
				UnityEngine.Object.Destroy(base.transform.Find(namedChild).gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject, awakeDestroyDelay);
			}
		}
	}

	private void DestroyChildGameObject()
	{
		if (base.transform.Find(namedChild).gameObject != null)
		{
			UnityEngine.Object.Destroy(base.transform.Find(namedChild).gameObject);
		}
	}

	private void DisableChildGameObject()
	{
		if (base.transform.Find(namedChild).gameObject.activeSelf)
		{
			base.transform.Find(namedChild).gameObject.SetActive(value: false);
		}
	}

	private void DestroyGameObject()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
