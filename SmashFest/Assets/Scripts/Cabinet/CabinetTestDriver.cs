using System;
using System.Collections;
using System.IO;
using Gameplay.Objects;
using UnityEngine;

namespace Cabinet
{
	// Unattended playtest, only when launched with -cabinetTest [outDir]:
	// waits in attract mode, presses start, aims at real objects and fires, and saves
	// a screenshot every couple of seconds so layout and flow can be reviewed offline.
	public class CabinetTestDriver : MonoBehaviour
	{
		private const float ShotInterval = 2.2f;
		private const float ScreenshotInterval = 2f;
		private const float RunDuration = 170f;

		private string _outDir;
		private int _shotIndex;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Create()
		{
			string[] args = Environment.GetCommandLineArgs();
			int i = Array.IndexOf(args, "-cabinetTest");
			if (i < 0)
			{
				return;
			}
			Application.runInBackground = true;
			GameObject go = new GameObject("CabinetTestDriver");
			DontDestroyOnLoad(go);
			CabinetTestDriver driver = go.AddComponent<CabinetTestDriver>();
			driver._outDir = i + 1 < args.Length && !args[i + 1].StartsWith("-") ? args[i + 1] : "cabinet-test";
			Directory.CreateDirectory(driver._outDir);
		}

		private IEnumerator Start()
		{
			StartCoroutine(ScreenshotRoutine());
			Debug.Log("CABINETTEST: started, attract mode for 7s");
			yield return new WaitForSecondsRealtime(7f);
			CabinetInput.SetTestInput(null, true);
			Debug.Log("CABINETTEST: pressed start");

			float end = Time.realtimeSinceStartup + RunDuration;
			while (Time.realtimeSinceStartup < end)
			{
				yield return new WaitForSecondsRealtime(ShotInterval * 0.6f);
				if (!CabinetDirector.IsPlaying)
				{
					CabinetInput.SetTestInput(null, false);
					continue;
				}
				Vector2? target = PickTarget();
				CabinetInput.SetTestInput(target, false);
				yield return new WaitForSecondsRealtime(ShotInterval * 0.4f);
				if (CabinetDirector.IsPlaying)
				{
					CabinetInput.SetTestInput(target, true);
					Debug.Log("CABINETTEST: fire at " + target);
				}
			}
			Debug.Log("CABINETTEST: done");
			Application.Quit();
		}

		private static Vector2? PickTarget()
		{
			Camera cam = Camera.main;
			if (cam == null)
			{
				return null;
			}
			BaseObject[] objects = FindObjectsOfType<BaseObject>();
			BaseObject best = null;
			float bestY = float.MaxValue;
			foreach (BaseObject o in objects)
			{
				if (o == null || o.BeingDestroyed || !o.gameObject.activeInHierarchy)
				{
					continue;
				}
				Vector3 vp = cam.WorldToViewportPoint(o.transform.position);
				if (vp.z <= 0f || vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f)
				{
					continue;
				}
				// lowest object on screen first, like a kid aiming at the base of a tower
				if (vp.y < bestY)
				{
					bestY = vp.y;
					best = o;
				}
			}
			if (best == null)
			{
				return null;
			}
			Vector3 p = cam.WorldToViewportPoint(best.transform.position);
			return new Vector2(p.x, p.y);
		}

		private IEnumerator ScreenshotRoutine()
		{
			while (true)
			{
				yield return new WaitForSecondsRealtime(ScreenshotInterval);
				yield return new WaitForEndOfFrame();
				string path = Path.Combine(_outDir, string.Format("shot_{0:D3}.png", _shotIndex++));
				ScreenCapture.CaptureScreenshot(path);
			}
		}
	}
}
