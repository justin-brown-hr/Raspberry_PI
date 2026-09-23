using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[AddComponentMenu("Destructible 2D/D2D Gallery Target")]
	public class D2dGalleryTarget : MonoBehaviour
	{
		[Tooltip("Is the target facing forward?")]
		public bool FrontShowing;

		[Tooltip("How fast the target can flip sides")]
		public float FlipSpeed = 10f;

		[Tooltip("The minimum time the target can face forward in seconds")]
		public float FrontTimeMin = 1f;

		[Tooltip("The maximum time the target can face forward in seconds")]
		public float FrontTimeMax = 2f;

		[Tooltip("The minimum time the target can be hidden in seconds")]
		public float BackTimeMin = 1f;

		[Tooltip("The maximum time the target can be hidden in seconds")]
		public float BackTimeMax = 10f;

		[Tooltip("The start position of the target in local space")]
		public Vector3 StartPosition;

		[Tooltip("The end position of the target in local space")]
		public Vector3 EndPosition;

		[Tooltip("The current movement progress in local space")]
		public float MoveProgress;

		[Tooltip("The maximum speed this target can move in local space")]
		public float MoveSpeed;

		[Tooltip("The destructible of this target")]
		public D2dDestructible Destructible;

		private float cooldown;

		private float angle;

		protected virtual void Awake()
		{
			ResetCooldown();
		}

		protected virtual void Update()
		{
			if (Application.isPlaying)
			{
				cooldown -= Time.deltaTime;
				if (cooldown <= 0f)
				{
					FrontShowing = !FrontShowing;
					ResetCooldown();
				}
			}
			float num = (!FrontShowing) ? 180f : 0f;
			if (Application.isPlaying)
			{
				angle = D2dHelper.Dampen(angle, num, FlipSpeed, Time.deltaTime);
			}
			else
			{
				angle = num;
			}
			base.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
			if (Destructible != null)
			{
				Destructible.Indestructible = (num >= 90f);
			}
			MoveProgress += MoveSpeed * Time.deltaTime;
			float magnitude = (EndPosition - StartPosition).magnitude;
			if (magnitude > 0f)
			{
				float t = Mathf.PingPong(MoveProgress / magnitude, 1f);
				base.transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, Mathf.SmoothStep(0f, 1f, t));
			}
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (base.transform.parent != null)
			{
				Gizmos.matrix = base.transform.parent.localToWorldMatrix;
			}
			Gizmos.DrawLine(StartPosition, EndPosition);
		}

		private void ResetCooldown()
		{
			if (FrontShowing)
			{
				cooldown = UnityEngine.Random.Range(FrontTimeMin, FrontTimeMax);
			}
			else
			{
				cooldown = UnityEngine.Random.Range(BackTimeMin, BackTimeMax);
			}
		}
	}
}
