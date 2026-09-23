using UnityEngine;

namespace Exploder2D
{
	public class Exploder2DSlowMotion : MonoBehaviour
	{
		public float slowMotionTime = 1f;

		public Exploder2DObject Exploder2D;

		private float slowMotionSpeed = 1f;

		private bool slowmo;

		public void EnableSlowMotion(bool status)
		{
			slowmo = status;
			if (slowmo)
			{
				slowMotionSpeed = 0.05f;
			}
			else
			{
				slowMotionSpeed = 1f;
			}
			slowMotionTime = slowMotionSpeed;
		}

		public void Update()
		{
			slowMotionSpeed = slowMotionTime;
			Time.timeScale = slowMotionSpeed;
			Time.fixedDeltaTime = slowMotionSpeed * 0.02f;
			if (UnityEngine.Input.GetKeyDown(KeyCode.T))
			{
				slowmo = !slowmo;
				EnableSlowMotion(slowmo);
			}
		}
	}
}
