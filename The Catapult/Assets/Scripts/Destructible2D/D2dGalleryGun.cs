using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Gun")]
	public class D2dGalleryGun : MonoBehaviour
	{
		[Tooltip("How much the mouse position relates to the gun position")]
		public float MoveScale = 0.25f;

		[Tooltip("How quickly the gun moves to its atrget position")]
		public float MoveSpeed = 5f;

		[Tooltip("The prefab spawned at the muzzle of the gun when shooting")]
		public GameObject MuzzlePrefab;

		[Tooltip("The prefab spawned at the mouse position when shooting")]
		public GameObject BulletPrefab;

		protected virtual void Update()
		{
			Vector3 localPosition = base.transform.localPosition;
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			float target = (mousePosition.x - (float)(Screen.width / 2)) * MoveScale;
			Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
			float target2 = (mousePosition2.y - (float)(Screen.height / 2)) * MoveScale;
			localPosition.x = D2dHelper.Dampen(localPosition.x, target, MoveSpeed, Time.deltaTime);
			localPosition.y = D2dHelper.Dampen(localPosition.y, target2, MoveSpeed, Time.deltaTime);
			base.transform.localPosition = localPosition;
			if (Input.GetMouseButtonDown(0))
			{
				Camera main = Camera.main;
				if (MuzzlePrefab != null)
				{
					Object.Instantiate(MuzzlePrefab, base.transform.position, Quaternion.identity);
				}
				if (BulletPrefab != null && main != null)
				{
					Vector3 position = main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
					Object.Instantiate(BulletPrefab, position, Quaternion.identity);
				}
			}
		}
	}
}
