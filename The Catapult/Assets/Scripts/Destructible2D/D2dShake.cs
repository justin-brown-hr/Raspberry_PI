using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Shake")]
	public class D2dShake : MonoBehaviour
	{
		[Tooltip("The amount of shake this applies to the D2dCameraShake component")]
		public float Shake;

		protected virtual void Awake()
		{
			for (int num = D2dCameraShake.AllCameraShakes.Count - 1; num >= 0; num--)
			{
				D2dCameraShake.AllCameraShakes[num].Shake += Shake;
			}
		}
	}
}
