using System;
using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Wheel")]
	public class D2dWheel : MonoBehaviour
	{
		[Tooltip("The current rotational speed of the wheel")]
		public float Speed;

		[Tooltip("How quickly the wheel matches the ground speed")]
		public float GripDampening = 1f;

		[Tooltip("How quickly the wheels slow down")]
		public float Friction = 0.1f;

		[SerializeField]
		private bool oldPositionSet;

		[SerializeField]
		private Vector2 oldPosition;

		[NonSerialized]
		private Rigidbody2D body;

		public void AddTorque(float amount)
		{
			Speed += amount;
		}

		protected virtual void FixedUpdate()
		{
			if (body == null)
			{
				body = GetComponentInParent<Rigidbody2D>();
			}
			if (body != null)
			{
				if (!oldPositionSet)
				{
					oldPositionSet = true;
					oldPosition = base.transform.position;
				}
				Vector2 a = base.transform.position;
				Vector2 b = a - oldPosition;
				float num = b.magnitude / Time.fixedDeltaTime;
				float target = num * Vector2.Dot(base.transform.up, body.transform.up);
				oldPosition = a;
				Speed = D2dHelper.Dampen(Speed, target, GripDampening, Time.fixedDeltaTime);
				Vector2 a2 = (Vector2)base.transform.up * Speed * Time.fixedDeltaTime;
				body.AddForceAtPosition(a2 - b, base.transform.position, ForceMode2D.Impulse);
				Speed = D2dHelper.Dampen(Speed, 0f, Friction, Time.fixedDeltaTime);
			}
		}
	}
}
