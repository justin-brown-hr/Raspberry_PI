using UnityEngine;
using UnityEngine.UI;
namespace Exploder2D
{
	public class HUDFPS : MonoBehaviour
	{
		public float updateInterval = 0.5f;

		private float accum;

		private int frames;

		private float timeleft;

		private void Start()
		{
			if (!GetComponent<Text>())
			{
				UnityEngine.Debug.Log("UtilityFramesPerSecond needs a Text component!");
				base.enabled = false;
			}
			else
			{
				timeleft = updateInterval;
			}
		}

		private void Update()
		{
			timeleft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			frames++;
			if ((double)timeleft <= 0.0)
			{
				float num = accum / (float)frames;
				string text = $"{num:F2} FPS";
				GetComponent<Text>().text = text;
				if (num < 30f)
				{
					GetComponent<Text>().material.color = Color.yellow;
				}
				else if (num < 10f)
				{
					GetComponent<Text>().material.color = Color.red;
				}
				else
				{
					GetComponent<Text>().material.color = Color.black;
				}
				timeleft = updateInterval;
				accum = 0f;
				frames = 0;
			}
		}
	}
}
