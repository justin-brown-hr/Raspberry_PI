using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Drag To Shoot")]
	public class D2dDragToShoot : MonoBehaviour
	{
		[Tooltip("The key you must hold down to do slicing")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the indicator should spawn at")]
		public float Intercept;

		[Tooltip("The prefab used to show what the slice will look like")]
		public GameObject IndicatorPrefab;

		[Tooltip("The prefab spawned at the impact point of the ray")]
		public GameObject ImpactPrefab;

		[Tooltip("The thickness of the indicator")]
		public float Thickness = 1f;

		[Tooltip("The max range the shot")]
		public float Range = 10f;

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
				if (ImpactPrefab != null && main != null)
				{
					Vector3 startPos = D2dHelper.ScreenToWorldPosition(startMousePosition, Intercept, main);
					Vector3 endPos = D2dHelper.ScreenToWorldPosition(UnityEngine.Input.mousePosition, Intercept, main);
					CalculateEndPos(startPos, ref endPos);
					Object.Instantiate(ImpactPrefab, endPos, Quaternion.identity);
				}
			}
			if (down && main != null && IndicatorPrefab != null)
			{
				if (indicatorInstance == null)
				{
					indicatorInstance = UnityEngine.Object.Instantiate(IndicatorPrefab);
				}
				Vector3 vector = D2dHelper.ScreenToWorldPosition(startMousePosition, Intercept, main);
				Vector3 endPos2 = D2dHelper.ScreenToWorldPosition(UnityEngine.Input.mousePosition, Intercept, main);
				CalculateEndPos(vector, ref endPos2);
				float num = Vector3.Distance(endPos2, vector);
				float num2 = D2dHelper.Atan2(endPos2 - vector) * 57.29578f;
				indicatorInstance.transform.position = vector;
				indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, 0f - num2);
				indicatorInstance.transform.localScale = new Vector3(Thickness, num, num);
			}
			else if (indicatorInstance != null)
			{
				UnityEngine.Object.Destroy(indicatorInstance.gameObject);
			}
		}

		private void CalculateEndPos(Vector3 startPos, ref Vector3 endPos)
		{
			Vector3 vector = endPos - startPos;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(startPos, vector.normalized, Range);
			float d = Range;
			if (raycastHit2D.collider != null)
			{
				d = raycastHit2D.distance;
			}
			endPos = startPos + vector.normalized * d;
		}
	}
}
