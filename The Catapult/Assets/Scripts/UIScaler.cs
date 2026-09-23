using UnityEngine;

public class UIScaler : MonoBehaviour
{
	public float widht = 970f;

	public float height = 600f;

	public RectTransform[] additionals;

	private void Awake()
	{
		float num = (float)Screen.height / height;
		float num2 = (float)Screen.width / widht;
		if (num < num2)
		{
			GetComponent<RectTransform>().localScale = new Vector3(num, num, num);
			for (int i = 0; i < additionals.Length; i++)
			{
				additionals[i].localScale = new Vector3(num, num, num);
			}
		}
		else
		{
			GetComponent<RectTransform>().localScale = new Vector3(num2, num2, num2);
			for (int j = 0; j < additionals.Length; j++)
			{
				additionals[j].localScale = new Vector3(num2, num2, num2);
			}
		}
	}
}
