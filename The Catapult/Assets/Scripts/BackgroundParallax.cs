using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
	public Transform[] backgrounds;

	public float parallaxScale;

	public float parallaxReductionFactor;

	public float smoothing;

	private Transform cam;

	private Vector3 previousCamPos;

	private void Awake()
	{
		cam = Camera.main.transform;
	}

	private void Start()
	{
		previousCamPos = cam.position;
	}

	private void Update()
	{
		float x = previousCamPos.x;
		Vector3 position = cam.position;
		float num = (x - position.x) * parallaxScale;
		Vector3 b = default(Vector3);
		for (int i = 0; i < backgrounds.Length; i++)
		{
			Vector3 position2 = backgrounds[i].position;
			float num2 = position2.x + num * ((float)i * parallaxReductionFactor + 1f);
			float x2 = num2;
			Vector3 position3 = backgrounds[i].position;
			float y = position3.y;
			Vector3 position4 = backgrounds[i].position;
			b = new Vector3(x2, y, position4.z);
			backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, b, smoothing * Time.deltaTime);
		}
		previousCamPos = cam.position;
	}
}
