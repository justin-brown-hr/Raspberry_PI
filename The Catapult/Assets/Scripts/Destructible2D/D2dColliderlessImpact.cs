using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[AddComponentMenu("Destructible 2D/D2D Colliderless Impact")]
	public class D2dColliderlessImpact : MonoBehaviour
	{
		[Tooltip("The prefab that gets spawned once this GameObject hits a destructible")]
		public GameObject ImpactPrefab;

		[SerializeField]
		private Vector3 oldPosition;

		protected virtual void OnEnable()
		{
			oldPosition = base.transform.position;
		}

		protected virtual void Start()
		{
			oldPosition = base.transform.position;
		}

		protected virtual void FixedUpdate()
		{
			Vector3 position = base.transform.position;
			D2dHit d2dHit = D2dDestructible.RaycastAlphaFirst(oldPosition, position);
			if (d2dHit != null)
			{
				if (ImpactPrefab != null)
				{
					Object.Instantiate(ImpactPrefab, d2dHit.Position, base.transform.rotation);
					UnityEngine.Debug.Log(" ---- Spawn in D2dColliderlessImpact ---");
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
			oldPosition = position;
		}
	}
}
