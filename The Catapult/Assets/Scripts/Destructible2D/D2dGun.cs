using UnityEngine;

namespace Destructible2D
{
	public class D2dGun : MonoBehaviour
	{
		[Tooltip("Minimum time between each shot in seconds")]
		public float ShootDelay = 0.1f;

		[Tooltip("The bullet prefab spawned when shooting")]
		public GameObject BulletPrefab;

		[Tooltip("The muzzle prefab spawned on the gun when shooting")]
		public GameObject MuzzleFlashPrefab;

		[SerializeField]
		private float cooldown;

		public bool CanShoot => cooldown <= 0f;

		public void Shoot()
		{
			if (cooldown <= 0f)
			{
				cooldown = ShootDelay;
				if (BulletPrefab != null)
				{
					Object.Instantiate(BulletPrefab, base.transform.position, base.transform.rotation);
				}
				if (MuzzleFlashPrefab != null)
				{
					Object.Instantiate(MuzzleFlashPrefab, base.transform.position, base.transform.rotation);
				}
			}
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}
	}
}
