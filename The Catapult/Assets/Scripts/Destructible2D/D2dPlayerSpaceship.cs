using System;
using UnityEngine;

namespace Destructible2D
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Destructible 2D/D2D Player Spaceship")]
	public class D2dPlayerSpaceship : MonoBehaviour
	{
		[Tooltip("Minimum time between each shot in seconds")]
		public float ShootDelay = 0.1f;

		[Tooltip("The left gun")]
		public D2dGun LeftGun;

		[Tooltip("The right gun")]
		public D2dGun RightGun;

		[Tooltip("The left thruster")]
		public D2dThruster LeftThruster;

		[Tooltip("The right thruster")]
		public D2dThruster RightThruster;

		[NonSerialized]
		private Rigidbody2D body;

		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
			if (Input.GetButton("Jump") && cooldown <= 0f)
			{
				cooldown = ShootDelay;
				if (LeftGun != null && LeftGun.CanShoot)
				{
					LeftGun.Shoot();
				}
				else if (RightGun != null && RightGun.CanShoot)
				{
					RightGun.Shoot();
				}
			}
			if (LeftThruster != null)
			{
				LeftThruster.Throttle = UnityEngine.Input.GetAxisRaw("Vertical") + UnityEngine.Input.GetAxisRaw("Horizontal") * 0.5f;
			}
			if (RightThruster != null)
			{
				RightThruster.Throttle = UnityEngine.Input.GetAxisRaw("Vertical") - UnityEngine.Input.GetAxisRaw("Horizontal") * 0.5f;
			}
		}
	}
}
