using UnityEngine;

public class Pauser : MonoBehaviour
{
	private bool paused;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.P))
		{
			paused = !paused;
		}
		if (paused)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}
}
