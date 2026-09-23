using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Click To Explode")]
	public class D2dClickToExplode : MonoBehaviour
	{
		[Tooltip("The key you must hold down to spawn")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the prefab should spawn at")]
		public float Intercept;

		[Tooltip("The prefab that gets spawned under the mouse when clicking")]
		public GameObject ExplosionPrefab;

		[Tooltip("The amount of times you want the clicked object to fracture")]
		public int FractureCount = 5;

		[Tooltip("The amount of outward force added to each fractured part")]
		public float Force;

		private Vector2 explosionPosition;

		protected virtual void Update()
		{
			if (FractureCount > 0)
			{
			}
		}

		private void OnEndSplit(List<D2dDestructible> clones)
		{
			for (int num = clones.Count - 1; num >= 0; num--)
			{
				D2dDestructible d2dDestructible = clones[num];
				Rigidbody2D component = d2dDestructible.GetComponent<Rigidbody2D>();
				if (component != null)
				{
					Vector2 b = d2dDestructible.transform.InverseTransformPoint(explosionPosition);
					Vector2 a = d2dDestructible.AlphaRect.center - b;
					component.AddRelativeForce(a * Force, ForceMode2D.Impulse);
				}
			}
		}
	}
}
