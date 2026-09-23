using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[AddComponentMenu("Destructible 2D/D2D Bullet")]
	public class D2dBullet : MonoBehaviour
	{
		[Tooltip("The tag this bullet cannot hit")]
		public string IgnoreTag;

		[Tooltip("The layers this bullet can hit")]
		public LayerMask RaycastMask = -1;

		[Tooltip("The prefab that gets spawned when this bullet hits something")]
		public GameObject ExplosionPrefab;

		[Tooltip("The distance this bullet moves each second")]
		public float Speed;

		[Tooltip("The maximum length of the bullet trail")]
		public float MaxLength;

		[Tooltip("The scale of the bullet after it's scaled up")]
		public Vector3 MaxScale;

		private Vector3 oldPosition;

		protected virtual void Start()
		{
			oldPosition = base.transform.position;
		}

		protected virtual void FixedUpdate()
		{
			Vector3 position = base.transform.position;
			float num = (position - oldPosition).magnitude;
			Vector3 normalized = (position - oldPosition).normalized;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(oldPosition, normalized, num, RaycastMask);
			if (num > MaxLength)
			{
				num = MaxLength;
				oldPosition = position - normalized * num;
			}
			base.transform.localScale = MaxScale * D2dHelper.Divide(num, MaxLength);
			if (raycastHit2D.collider != null && (string.IsNullOrEmpty(IgnoreTag) || raycastHit2D.collider.tag != IgnoreTag))
			{
				if (ExplosionPrefab != null)
				{
					Object.Instantiate(ExplosionPrefab, raycastHit2D.point, Quaternion.identity);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		protected virtual void Update()
		{
			base.transform.Translate(0f, Speed * Time.deltaTime, 0f);
		}
	}
}
