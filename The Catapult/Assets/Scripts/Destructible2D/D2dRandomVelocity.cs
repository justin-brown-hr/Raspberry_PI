using System;
using UnityEngine;

namespace Destructible2D
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Destructible 2D/D2D UnityEngine.Random Velocity")]
	public class D2dRandomVelocity : MonoBehaviour
	{
		[Tooltip("The maximum speed applied")]
		public float MaxLinearSpeed = 1f;

		[Tooltip("The maximum speed applied")]
		public float MaxAngularSpeed = 1f;

		[Tooltip("Should the new velocity be added to the existing one?")]
		public bool Additive;

		[NonSerialized]
		private Rigidbody2D body;

		public void RandomizeVelocity()
		{
			if (body == null)
			{
				body = GetComponent<Rigidbody2D>();
			}
			if (Additive)
			{
				body.velocity += UnityEngine.Random.insideUnitCircle * MaxLinearSpeed;
				body.angularVelocity += UnityEngine.Random.value * MaxAngularSpeed;
			}
			else
			{
				body.velocity = UnityEngine.Random.insideUnitCircle * MaxLinearSpeed;
				body.angularVelocity = UnityEngine.Random.value * MaxAngularSpeed;
			}
		}
	}
}
