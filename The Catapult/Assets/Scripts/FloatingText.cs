using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
	public Text damageText;

	public float life_time;

	private void OnEnable()
	{
		UnityEngine.Object.Destroy(base.gameObject, life_time);
	}

	public void SetText(string text)
	{
		damageText.text = text;
	}
}
