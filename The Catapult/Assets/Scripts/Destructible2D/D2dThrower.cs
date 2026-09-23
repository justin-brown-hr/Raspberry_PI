using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Thrower")]
	public class D2dThrower : MonoBehaviour
	{
		[Tooltip("The minimum delay between throws in seconds")]
		public float DelayMin = 0.5f;

		[Tooltip("The maximum delay between throws in seconds")]
		public float DelayMax = 2f;

		[Tooltip("The minimum speed of the thrown object")]
		public float SpeedMin = 10f;

		[Tooltip("The maximum speed of the thrown object")]
		public float SpeedMax = 20f;

		[Tooltip("Maximum degrees spread when throwing")]
		public float Spread = 10f;

		[Tooltip("The prefabs that can be thrown")]
		public GameObject[] ThrowPrefabs;

		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
			if (!(cooldown <= 0f))
			{
				return;
			}
			cooldown = UnityEngine.Random.Range(DelayMin, DelayMax);
			if (ThrowPrefabs != null && ThrowPrefabs.Length > 0)
			{
				int num = UnityEngine.Random.Range(0, ThrowPrefabs.Length);
				GameObject original = ThrowPrefabs[num];
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				gameObject.transform.position = base.transform.position;
				if (component != null)
				{
					float f = UnityEngine.Random.Range(-0.5f, 0.5f) * Spread * 0.0174532924f;
					float num2 = UnityEngine.Random.Range(SpeedMin, SpeedMax);
					component.velocity = new Vector2(Mathf.Sin(f) * num2, Mathf.Cos(f) * num2);
					component.angularVelocity = UnityEngine.Random.Range(-180f, 180f);
				}
			}
		}
	}
}
