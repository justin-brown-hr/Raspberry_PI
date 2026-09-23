using UnityEngine;
using UnityEngine.Serialization;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Spawner")]
	public class D2dSpawner : MonoBehaviour
	{
		[Tooltip("The source GameObject you want to spawn")]
		[FormerlySerializedAs("Source")]
		public GameObject Prefab;

		[Tooltip("Should the source get spawned in Start?")]
		public bool SpawnInStart;

		public void SpawnAt(Collision2D collision)
		{
			if (Prefab != null && base.isActiveAndEnabled)
			{
				ContactPoint2D[] contacts = collision.contacts;
				for (int num = contacts.Length - 1; num >= 0; num--)
				{
					Object.Instantiate(Prefab, contacts[num].point, base.transform.rotation);
				}
			}
		}

		public void SpawnAt(Vector2 position)
		{
			if (Prefab != null && base.isActiveAndEnabled)
			{
				Object.Instantiate(Prefab, position, base.transform.rotation);
			}
		}

		public void SpawnAt(Vector3 position)
		{
			if (Prefab != null && base.isActiveAndEnabled)
			{
				Object.Instantiate(Prefab, position, base.transform.rotation);
			}
		}

		public void Spawn()
		{
			if (Prefab != null && base.isActiveAndEnabled)
			{
				Object.Instantiate(Prefab, base.transform.position, base.transform.rotation);
			}
		}

		protected virtual void Start()
		{
			if (SpawnInStart)
			{
				Spawn();
			}
		}
	}
}
