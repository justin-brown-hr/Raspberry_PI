using UnityEngine;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Destructible 2D/D2D Compound Part")]
	public class D2dCompoundPart : MonoBehaviour
	{
		[Tooltip("The maximum force magnitude applied when this part is detached")]
		public float MaxForce = 1f;

		[SerializeField]
		private Rigidbody2D thisRigidbody;

		[SerializeField]
		private Vector2 acceleration;

		[ContextMenu("Detach")]
		public void Detach()
		{
			base.transform.parent = null;
			thisRigidbody = base.gameObject.AddComponent<Rigidbody2D>();
			thisRigidbody.gravityScale = 0f;
			acceleration = UnityEngine.Random.insideUnitCircle * MaxForce;
		}

		protected virtual void FixedUpdate()
		{
			if (thisRigidbody != null)
			{
				thisRigidbody.AddRelativeForce(acceleration, ForceMode2D.Force);
			}
		}
	}
}
