using System;
using UnityEngine;

namespace Destructible2D
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Destructible 2D/D2D Waypoints")]
	public class D2dWaypoints : MonoBehaviour
	{
		[Tooltip("The rate at which this GameObject accelerates toward its current target")]
		public float Acceleration = 5f;

		[Tooltip("The maximum speed at which this GameObject can move toward its current target")]
		public float MaximumSpeed = 2f;

		[Tooltip("The extra acceleration given to stop this gameObject orbiting its target")]
		public float SpeedBoost = 2f;

		[Tooltip("If this gameObject gets within this distance of its current target then it will switch target")]
		public float MinimumDistance = 1f;

		[Tooltip("The  points this GameObject will randomly move between")]
		public Vector2[] Points;

		[SerializeField]
		private Vector2 targetPoint;

		[NonSerialized]
		private Rigidbody2D body;

		protected virtual void Awake()
		{
			ChangeTargetPoint();
		}

		protected virtual void FixedUpdate()
		{
			Vector2 b = base.transform.position;
			Vector2 a = targetPoint - b;
			if (a.magnitude <= MinimumDistance)
			{
				ChangeTargetPoint();
				a = targetPoint - b;
			}
			if (a.magnitude > MaximumSpeed)
			{
				a = a.normalized * MaximumSpeed;
			}
			if (body == null)
			{
				body = GetComponent<Rigidbody2D>();
			}
			body.velocity = D2dHelper.Dampen2(body.velocity, a * SpeedBoost, Acceleration, Time.deltaTime);
		}

		private void ChangeTargetPoint()
		{
			if (Points != null && Points.Length > 0)
			{
				int num = UnityEngine.Random.Range(0, Points.Length);
				targetPoint = Points[num];
			}
		}
	}
}
