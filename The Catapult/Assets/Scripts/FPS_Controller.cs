using UnityEngine;

public class FPS_Controller : MonoBehaviour
{
	public bool stat_FPS = true;

	private float deltaTime;

	private void Awake()
	{
		if (stat_FPS)
		{
		}
		Application.targetFrameRate = 60;
	}

	private void Update()
	{
		if (stat_FPS)
		{
			deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		}
	}

	private void OnGUI()
	{
		if (stat_FPS)
		{
			int width = Screen.width;
			int height = Screen.height;
			GUIStyle gUIStyle = new GUIStyle();
			Rect position = new Rect(0f, 0f, width, height * 2 / 100);
			gUIStyle.alignment = TextAnchor.UpperLeft;
			gUIStyle.fontSize = height * 6 / 100;
			gUIStyle.normal.textColor = new Color(0f, 0f, 0.5f, 1f);
			float num = deltaTime * 1000f;
			float num2 = 1f / deltaTime;
			string text = $"{num:0.0} ms ({num2:0.} fps)";
			GUI.Label(position, text, gUIStyle);
		}
	}
}
