using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Destructible 2D/D2D Player Camera")]
	public class D2dPlayerCamera : MonoBehaviour
	{
		[Tooltip("How quickly the camera can move per second")]
		public float Speed = 1f;

		[Tooltip("How quickly the camera moves to its target location")]
		public float Acceleration = 2f;

		[SerializeField]
		private Vector2 velocity;

		protected virtual void Update()
		{
			float axisRaw = UnityEngine.Input.GetAxisRaw("Horizontal");
			float axisRaw2 = UnityEngine.Input.GetAxisRaw("Vertical");
			velocity.x += axisRaw * Speed * Time.deltaTime;
			velocity.y += axisRaw2 * Speed * Time.deltaTime;
			velocity = D2dHelper.Dampen2(velocity, Vector2.zero, Acceleration, Time.deltaTime);
			base.transform.Translate(velocity);
		}
	}
}
