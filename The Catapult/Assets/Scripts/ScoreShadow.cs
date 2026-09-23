using UnityEngine;
using UnityEngine.UI;

public class ScoreShadow : MonoBehaviour
{
	public GameObject guiCopy;

	private void Awake()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = guiCopy.transform.position;
		float x = position2.x;
		Vector3 position3 = guiCopy.transform.position;
		float y = position3.y - 0.005f;
		Vector3 position4 = guiCopy.transform.position;
		position = new Vector3(x, y, position4.z - 1f);
		base.transform.position = position;
	}

	private void Update()
	{
		GetComponent<Text>().text = guiCopy.GetComponent<Text>().text;
	}
}
