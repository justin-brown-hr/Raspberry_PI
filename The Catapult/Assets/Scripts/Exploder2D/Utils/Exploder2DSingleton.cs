using UnityEngine;

namespace Exploder2D.Utils
{
	public class Exploder2DSingleton : MonoBehaviour
	{
		public static Exploder2DObject Exploder2DInstance;

		private void Awake()
		{
			Exploder2DInstance = base.gameObject.GetComponent<Exploder2DObject>();
		}
	}
}
