using UnityEngine;

public class Map_MuveGroundController : MonoBehaviour
{
	private bool stat;

	private float x;

	private float x1;

	public float speed = 40f;

	public GameObject Oll;

	private void Start()
	{
	}

	private void Update()
	{
		Vector2 vector = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
		if (Input.GetMouseButtonDown(0))
		{
			x1 = vector.x;
			stat = true;
			Vector3 localPosition = Oll.transform.localPosition;
			x = localPosition.x;
		}
		if (Input.GetMouseButtonUp(0))
		{
			stat = false;
		}
		if (stat)
		{
			UnityEngine.Debug.Log("------- Mause Position -------> " + vector.x);
			if (vector.x < x1 - 0.0001f)
			{
			}
			float num = vector.x - (vector.x + 3f);
			if (vector.x > x1)
			{
				float num2 = vector.x - x1;
				float num3 = x + num2;
				Oll.transform.localPosition = new Vector3(num3, -6.99f, 0f);
			}
			if (vector.x < x1)
			{
				float num4 = vector.x - x1;
				float num5 = x + x + x + num4;
				Oll.transform.localPosition = new Vector3(num5, -6.99f, 0f);
			}
		}
	}
}
