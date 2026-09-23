using System;
using UnityEngine;

namespace Destructible2D
{
	public class D2dThruster : MonoBehaviour
	{
		[Tooltip("The current thottle amount")]
		public float Throttle;

		[Tooltip("The scale of this thruster when throttle is 1")]
		public Vector3 MaxScale = Vector3.one;

		[Tooltip("How quickly the throttle scales to the desired value")]
		public float Dampening = 10f;

		[Tooltip("The amount of force applied to the rigidbody2D when throttle is 1")]
		public float MaxForce = 1f;

		[Tooltip("The amount the thruster effect can flicker")]
		public float Flicker = 0.1f;

		[NonSerialized]
		private Rigidbody2D body;

		[SerializeField]
		private float currentThrottle;

		protected virtual void FixedUpdate()
		{
			if (body == null)
			{
				body = GetComponentInParent<Rigidbody2D>();
			}
			if (body != null)
			{
				body.AddForceAtPosition(base.transform.up * MaxForce * (0f - Throttle), base.transform.position, ForceMode2D.Force);
			}
		}

		protected virtual void Update()
		{
			currentThrottle = D2dHelper.Dampen(currentThrottle, Throttle, Dampening, Time.deltaTime);
			base.transform.localScale = MaxScale * UnityEngine.Random.Range(1f - Flicker, 1f + Flicker) * currentThrottle;
		}
	}
}
