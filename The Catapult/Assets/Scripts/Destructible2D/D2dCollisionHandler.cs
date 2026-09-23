using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Collision Handler")]
	public class D2dCollisionHandler : MonoBehaviour
	{
		[Tooltip("Output debug information about collisions?")]
		public bool DebugCollisions;

		[Tooltip("The layers that must hit this collider for damage to get inflicted")]
		public LayerMask ImpactMask = -1;

		[Tooltip("This amount of damage required to register an impact")]
		public float ImpactThreshold = 1f;

		[Tooltip("The minimum amount of seconds between each impact")]
		public float ImpactDelay;

		[Tooltip("Should this collider inflict damage on the Destructible when it takes impact?")]
		public bool DamageOnImpact;

		[Tooltip("The destructible that takes the damage")]
		public D2dDestructible DamageDestructible;

		[Tooltip("The damage will be scaled by this value after it passes the ImpactThreshold")]
		public float DamageScale = 1f;

		[Tooltip("If an impact passes the impact threshold, ignore any other impacts? (can happen when complex shapes hit this)")]
		public bool UseFirstOnly = true;

		public D2dVector2Event OnImpact;

		private float cooldownTime;

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			if (DebugCollisions)
			{
				UnityEngine.Debug.Log(base.name + " hit " + collision.collider.name + " for " + collision.relativeVelocity.magnitude);
			}
			if (ImpactDelay > 0f)
			{
				if (!(Time.time >= cooldownTime))
				{
					return;
				}
				cooldownTime = Time.time + ImpactDelay;
			}
			int num = 1 << collision.collider.gameObject.layer;
			if ((num & (int)ImpactMask) == 0)
			{
				return;
			}
			ContactPoint2D[] contacts = collision.contacts;
			for (int num2 = contacts.Length - 1; num2 >= 0; num2--)
			{
				ContactPoint2D contactPoint2D = contacts[num2];
				float magnitude = collision.relativeVelocity.magnitude;
				if (magnitude >= ImpactThreshold)
				{
					if (DamageOnImpact)
					{
						if (DamageDestructible == null)
						{
							DamageDestructible = GetComponentInChildren<D2dDestructible>();
						}
						if (DamageDestructible != null)
						{
							DamageDestructible.Damage += magnitude * DamageScale;
						}
					}
					if (OnImpact != null)
					{
						OnImpact.Invoke(contactPoint2D.point);
					}
					if (UseFirstOnly)
					{
						break;
					}
				}
			}
		}
	}
}
