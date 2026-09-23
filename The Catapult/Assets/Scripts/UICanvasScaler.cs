using UnityEngine;
using UnityEngine.UI;

public class UICanvasScaler : MonoBehaviour
{
	public enum ScaleTypr
	{
		Horazontal,
		Vertical
	}

	public ScaleTypr scaleType = ScaleTypr.Vertical;

	public float windowScale;

	public float maxWindowSizeX = 1080f;

	public float maxWindowSizeY = 1270f;

	private void Awake()
	{
		RefreshCanvasScaleFactor();
	}

	public void LateUpdate()
	{
		RefreshCanvasScaleFactor();
	}

	private float GetCnavasScaleFactor()
	{
		if (scaleType == ScaleTypr.Vertical)
		{
			return (float)Screen.height / maxWindowSizeY;
		}
		if (scaleType == ScaleTypr.Horazontal)
		{
			return (float)Screen.width / maxWindowSizeX;
		}
		return (float)Screen.height / maxWindowSizeY;
	}

	private void RefreshCanvasScaleFactor()
	{
		GetComponent<CanvasScaler>().scaleFactor = windowScale * GetCnavasScaleFactor();
	}
}
