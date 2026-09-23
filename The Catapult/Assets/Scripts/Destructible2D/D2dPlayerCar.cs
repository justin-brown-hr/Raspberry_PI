using UnityEngine;

namespace Destructible2D
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Destructible 2D/D2D Player Car")]
	public class D2dPlayerCar : MonoBehaviour
	{
		[Tooltip("The wheels used to steer this car")]
		public D2dWheel[] SteerWheels;

		[Tooltip("The maximum +- angle of turning")]
		public float SteerAngleMax = 20f;

		[Tooltip("How quickly the steering wheels turn to their target angle")]
		public float SteerAngleDampening = 5f;

		[Tooltip("The wheels used to move this car")]
		public D2dWheel[] DriveWheels;

		[Tooltip("The maximum torque that can be applied to each drive wheel")]
		public float DriveTorque = 1f;

		[SerializeField]
		private float currentAngle;

		protected virtual void Update()
		{
			float target = UnityEngine.Input.GetAxisRaw("Horizontal") * SteerAngleMax;
			currentAngle = D2dHelper.Dampen(currentAngle, target, SteerAngleDampening, Time.deltaTime);
			for (int i = 0; i < SteerWheels.Length; i++)
			{
				SteerWheels[i].transform.localRotation = Quaternion.Euler(0f, 0f, 0f - currentAngle);
			}
		}

		protected virtual void FixedUpdate()
		{
			for (int i = 0; i < DriveWheels.Length; i++)
			{
				DriveWheels[i].AddTorque(UnityEngine.Input.GetAxisRaw("Vertical") * DriveTorque * Time.fixedDeltaTime);
			}
		}
	}
}
