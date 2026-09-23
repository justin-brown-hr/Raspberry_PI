using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Drag To Throw")]
	public class D2dDragToThrow : MonoBehaviour
	{
		[Tooltip("The key you must hold down to do slicing")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the indicator should spawn at")]
		public float Intercept;

		[Tooltip("The prefab used to show what the slice will look like")]
		public GameObject IndicatorPrefab;

		[Tooltip("The scale of the throw indicator")]
		public float Scale = 1f;

		[Tooltip("The prefab that gets thrown")]
		public GameObject ProjectilePrefab;

		[Tooltip("How fast the projectile will be launched")]
		public float ProjectileSpeed = 10f;

		[Tooltip("How much spread is added to the project when fired")]
		public float ProjectileSpread;

		[SerializeField]
		private bool down;

		[SerializeField]
		private Vector3 startMousePosition;

		[SerializeField]
		private GameObject indicatorInstance;

		protected virtual void Update()
		{
			Camera main = Camera.main;
			if (UnityEngine.Input.touchCount <= 0)
			{
				return;
			}
			if (UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
			{
				down = true;
				startMousePosition = UnityEngine.Input.GetTouch(0).position;
			}
			if (UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				down = false;
				if (main != null && ProjectilePrefab != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(ProjectilePrefab);
					Vector3 vector = D2dHelper.ScreenToWorldPosition(startMousePosition, Intercept, main);
					Vector3 a = D2dHelper.ScreenToWorldPosition(UnityEngine.Input.GetTouch(0).position, Intercept, main);
					float num = D2dHelper.Atan2(a - vector) * 57.29578f;
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					if (component != null)
					{
						component.velocity = (a - vector) * ProjectileSpeed;
					}
					num += UnityEngine.Random.Range(0f - ProjectileSpread, ProjectileSpread);
					gameObject.transform.position = vector;
					gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f - num);
				}
			}
			if (down && main != null && IndicatorPrefab != null)
			{
				if (indicatorInstance == null)
				{
					indicatorInstance = UnityEngine.Object.Instantiate(IndicatorPrefab);
				}
				Vector3 vector2 = D2dHelper.ScreenToWorldPosition(startMousePosition, Intercept, main);
				Vector3 a2 = D2dHelper.ScreenToWorldPosition(UnityEngine.Input.GetTouch(0).position, Intercept, main);
				float num2 = Vector3.Distance(a2, vector2) * Scale;
				float num3 = D2dHelper.Atan2(a2 - vector2) * 57.29578f;
				indicatorInstance.transform.position = vector2;
				indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, 0f - num3);
				indicatorInstance.transform.localScale = new Vector3(num2, num2, num2);
			}
			else if (indicatorInstance != null)
			{
				UnityEngine.Object.Destroy(indicatorInstance.gameObject);
			}
		}
	}
}
