using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Drag To Slice")]
	public class D2dDragToStamp : MonoBehaviour
	{
		[Tooltip("The key you must hold down to do slicing")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the indicator should spawn at")]
		public float Intercept;

		[Tooltip("The GameObject layers this can stamp")]
		public LayerMask Layers = -1;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the stamp")]
		public Vector2 Size = Vector2.one;

		[Tooltip("The stretch factorwhen connecting stamps")]
		public float Stretch = 1f;

		[Tooltip("How hard the stamp should be")]
		public float Hardness = 1f;

		[Tooltip("The delay between each repeat stamp")]
		public float Delay = 0.25f;

		private float cooldown;

		[SerializeField]
		private bool down;

		[SerializeField]
		private Vector3 lastMousePosition;

		[SerializeField]
		private float lastAngle;

		protected virtual void Update()
		{
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			if (UnityEngine.Input.GetKey(Requires))
			{
				if (down)
				{
					cooldown -= Time.deltaTime;
					if (cooldown <= 0f)
					{
						cooldown = Delay;
						Stamp(lastMousePosition, mousePosition);
						lastMousePosition = mousePosition;
					}
				}
				else
				{
					down = true;
					cooldown = Delay;
					Stamp(mousePosition, mousePosition);
					lastMousePosition = mousePosition;
				}
			}
			else if (down)
			{
				down = false;
				if (mousePosition != lastMousePosition)
				{
					Stamp(lastMousePosition, mousePosition);
				}
			}
		}

		private void Stamp(Vector2 from, Vector2 to)
		{
			Camera main = Camera.main;
			if (main != null)
			{
				if (from != to)
				{
					Vector2 vector = to - from;
					lastAngle = (0f - Mathf.Atan2(vector.x, vector.y)) * 57.29578f;
				}
				Vector3 a = D2dHelper.ScreenToWorldPosition(from, Intercept, main);
				Vector3 b = D2dHelper.ScreenToWorldPosition(to, Intercept, main);
				Vector3 v = (a + b) * 0.5f;
				float num = Vector3.Distance(a, b) * Stretch;
				if (num < Size.y)
				{
					num = Size.y;
				}
				D2dDestructible.StampAll(size: new Vector2(Size.x, num), position: v, angle: lastAngle, stampTex: StampTex, hardness: Hardness, layerMask: Layers);
			}
		}
	}
}
