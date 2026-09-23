using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Destructible 2D/D2D Camera Shake")]
	public class D2dCameraShake : MonoBehaviour
	{
		public static List<D2dCameraShake> AllCameraShakes = new List<D2dCameraShake>();

		[Tooltip("The current shake strength. This gets reduced automatically")]
		public float Shake;

		[Tooltip("The speed at which the Shake value gets reduced")]
		public float ShakeDampening = 10f;

		[Tooltip("The amount this camera shakes relative to the Shake value")]
		public float ShakeScale = 1f;

		[Tooltip("The freqncy of the camera shake")]
		public float ShakeSpeed = 10f;

		[SerializeField]
		private float offsetX;

		[SerializeField]
		private float offsetY;

		protected virtual void Awake()
		{
			offsetX = UnityEngine.Random.Range(-1000f, 1000f);
			offsetY = UnityEngine.Random.Range(-1000f, 1000f);
		}

		protected virtual void OnEnable()
		{
			AllCameraShakes.Add(this);
		}

		protected virtual void OnDisable()
		{
			AllCameraShakes.Remove(this);
		}

		protected virtual void LateUpdate()
		{
			Shake = D2dHelper.Dampen(Shake, 0f, ShakeDampening, Time.deltaTime, 0.1f);
			float num = Shake * ShakeScale;
			float y = Time.time * ShakeSpeed;
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = Mathf.PerlinNoise(offsetX, y) * num;
			localPosition.y = Mathf.PerlinNoise(offsetY, y) * num;
			base.transform.localPosition = localPosition;
		}
	}
}
