using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Explosion")]
	public class D2dExplosion : MonoBehaviour
	{
		[Tooltip("The layers the explosion should work on")]
		public LayerMask Mask = -1;

		[Tooltip("Sould the explosion stamp a shape?")]
		public bool Stamp = true;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the explosion stamp in world space")]
		public Vector2 StampSize = new Vector2(1f, 1f);

		[Tooltip("How hard the stamp is")]
		public float StampHardness = 1f;

		[Tooltip("Randomly rotate the stamp?")]
		public bool StampRandomDirection = true;

		[Tooltip("Should the explosion cast rays?")]
		public bool Raycast = true;

		[Tooltip("The size of the explosion raycast sphere")]
		public float RaycastRadius = 1f;

		[Tooltip("The amount of raycasts sent out")]
		public int RaycastCount = 32;

		[Tooltip("The amount of force added to objects that the raycasts hit")]
		public float ForcePerRay = 1f;

		[Tooltip("The amount of damage added to objects that the raycasts hit")]
		public float DamagePerRay = 1f;

		protected virtual void Start()
		{
			if (Stamp)
			{
				Vector3 position = base.transform.position;
				float angle = (!StampRandomDirection) ? 0f : UnityEngine.Random.Range(-180f, 180f);
				D2dDestructible.StampAll(position, StampSize, angle, StampTex, StampHardness, Mask);
			}
			if (!Raycast || RaycastCount <= 0)
			{
				return;
			}
			float num = 360f / (float)RaycastCount;
			if (DamagePerRay != 0f)
			{
				for (int i = 0; i < RaycastCount; i++)
				{
					float f = (float)i * num;
					RaycastHit2D raycastHit2D = Physics2D.Raycast(direction: new Vector2(Mathf.Sin(f), Mathf.Cos(f)), origin: base.transform.position, distance: RaycastRadius, layerMask: Mask);
					Collider2D collider = raycastHit2D.collider;
					if (collider != null && !collider.isTrigger)
					{
						float num2 = 1f - raycastHit2D.fraction;
						D2dDestructible componentInParent = collider.GetComponentInParent<D2dDestructible>();
						if (componentInParent != null)
						{
							componentInParent.Damage += DamagePerRay * num2;
						}
					}
				}
			}
			if (ForcePerRay == 0f)
			{
				return;
			}
			for (int j = 0; j < RaycastCount; j++)
			{
				float f2 = (float)j * num;
				Vector2 vector = new Vector2(Mathf.Sin(f2), Mathf.Cos(f2));
				RaycastHit2D raycastHit2D2 = Physics2D.Raycast(base.transform.position, vector, RaycastRadius, Mask);
				Collider2D collider2 = raycastHit2D2.collider;
				if (collider2 != null && !collider2.isTrigger)
				{
					float d = 1f - raycastHit2D2.fraction;
					Rigidbody2D attachedRigidbody = collider2.attachedRigidbody;
					if (attachedRigidbody != null)
					{
						Vector2 force = vector * ForcePerRay * d;
						attachedRigidbody.AddForceAtPosition(force, raycastHit2D2.point);
					}
				}
			}
		}
	}
}
