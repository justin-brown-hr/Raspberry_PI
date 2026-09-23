using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Destructible 2D/D2D Follow")]
	public class D2dFollow : MonoBehaviour
	{
		[Tooltip("The target object you want this GameObject to follow")]
		public Transform Target;

		public void UpdatePosition()
		{
			if (Target != null)
			{
				Vector3 position = base.transform.position;
				Vector3 position2 = Target.position;
				position.x = position2.x;
				Vector3 position3 = Target.position;
				position.y = position3.y;
				base.transform.position = position;
			}
		}

		protected virtual void Update()
		{
			UpdatePosition();
		}
	}
}
