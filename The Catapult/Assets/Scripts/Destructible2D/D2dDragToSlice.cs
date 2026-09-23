using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Drag To Slice")]
	public class D2dDragToSlice : MonoBehaviour
	{
		[Tooltip("The key you must hold down to do slicing")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the indicator should spawn at")]
		public float Intercept;

		[Tooltip("The prefab used to show what the slice will look like")]
		public GameObject IndicatorPrefab;

		[Tooltip("The shape of the slice when it stamps the Destructibles in the scene")]
		public Texture2D StampTex;

		[Tooltip("How hard the stamp should be")]
		public float Hardness = 1f;

		[Tooltip("The thickness of the slice line")]
		public float Thickness = 1f;

		[SerializeField]
		private bool down;

		[SerializeField]
		private Vector3 startMousePosition;

		[SerializeField]
		private GameObject indicatorInstance;

		protected virtual void Update()
		{
			Camera main = Camera.main;
			if (UnityEngine.Input.GetKey(Requires) && !down)
			{
				down = true;
				startMousePosition = UnityEngine.Input.mousePosition;
			}
			if (!Input.GetKey(Requires) && down)
			{
				down = false;
				if (main != null)
				{
					Vector3 mousePosition = UnityEngine.Input.mousePosition;
					Vector3 v = D2dHelper.ScreenToWorldPosition(startMousePosition, Intercept, main);
					Vector3 v2 = D2dHelper.ScreenToWorldPosition(mousePosition, Intercept, main);
					D2dDestructible.SliceAll(v, v2, Thickness, StampTex, Hardness);
				}
			}
			if (down && main != null && IndicatorPrefab != null)
			{
				if (indicatorInstance == null)
				{
					indicatorInstance = UnityEngine.Object.Instantiate(IndicatorPrefab);
				}
				Vector3 vector = D2dHelper.ScreenToWorldPosition(startMousePosition, Intercept, main);
				Vector3 a = D2dHelper.ScreenToWorldPosition(UnityEngine.Input.mousePosition, Intercept, main);
				float num = Vector3.Distance(a, vector);
				float num2 = D2dHelper.Atan2(a - vector) * 57.29578f;
				indicatorInstance.transform.position = vector;
				indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, 0f - num2);
				indicatorInstance.transform.localScale = new Vector3(Thickness, num, num);
			}
			else if (indicatorInstance != null)
			{
				UnityEngine.Object.Destroy(indicatorInstance.gameObject);
			}
		}
	}
}
