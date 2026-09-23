using UnityEngine;

namespace Exploder2D
{
	public class Example : MonoBehaviour
	{
		public Exploder2DObject Exploder2D;

		public void ExplodeObject(GameObject obj)
		{
			Exploder2D.Explode(OnExplosion);
		}

		private void OnExplosion(float time, Exploder2DObject.ExplosionState state)
		{
			if (state != Exploder2DObject.ExplosionState.ExplosionFinished)
			{
			}
		}

		private void CrackAndExplodeObject(GameObject obj)
		{
			Exploder2D.Crack(OnCracked);
		}

		private void OnCracked()
		{
			Exploder2D.ExplodeCracked(OnExplosion);
		}
	}
}
