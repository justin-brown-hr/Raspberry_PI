using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Click To Spawn")]
	public class D2dClickToSpawn : MonoBehaviour
	{
		[Tooltip("The key you must hold down to spawn")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the prefab should spawn at")]
		public float Intercept;

		[Tooltip("The prefab that gets spawned under the mouse when clicking")]
		public GameObject Prefab;

		protected virtual void Update()
		{
			if (UnityEngine.Input.GetKeyDown(Requires) && Prefab != null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					Vector3 position = D2dHelper.ScreenToWorldPosition(UnityEngine.Input.mousePosition, Intercept, main);
					Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
					Object.Instantiate(Prefab, position, rotation);
					UnityEngine.Debug.Log(" ---- Spawn in D2dClickToSpawn ---");
				}
			}
		}
	}
}
