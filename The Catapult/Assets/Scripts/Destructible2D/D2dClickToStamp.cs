using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Click To Stamp")]
	public class D2dClickToStamp : MonoBehaviour
	{
		[Tooltip("The key you must hold down to stamp")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the stamps should spawn at")]
		public float Intercept;

		[Tooltip("The GameObject layers this can stamp")]
		public LayerMask Layers = -1;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the stamp")]
		public Vector2 Size = Vector2.one;

		[Tooltip("The angle of the stamp")]
		public float Angle;

		[Tooltip("The hardness of the stamp")]
		public float Hardness = 1f;

		protected virtual void Update()
		{
			if (UnityEngine.Input.GetKeyDown(Requires))
			{
				Camera main = Camera.main;
				if (main != null)
				{
					Vector3 v = D2dHelper.ScreenToWorldPosition(UnityEngine.Input.mousePosition, Intercept, main);
					D2dDestructible.StampAll(v, Size, Angle, StampTex, Hardness, Layers);
				}
			}
		}
	}
}
