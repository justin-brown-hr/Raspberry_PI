using Exploder2D.Utils;
using UnityEngine;

namespace Exploder2D.Demo
{
	public class DemoClickExplode2D : MonoBehaviour
	{
		private Exploder2DObject exploder;

		public Camera Camera;

		private void Start()
		{
			exploder = Exploder2DSingleton.Exploder2DInstance;
		}

		private bool IsExplodable(GameObject obj)
		{
			if (exploder.DontUseTag)
			{
				return obj.GetComponent<Explodable2D>() != null;
			}
			return obj.CompareTag(Exploder2DObject.Tag);
		}

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
			{
				return;
			}
			RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.ScreenPointToRay(UnityEngine.Input.mousePosition));
			if (!rayIntersection)
			{
				return;
			}
			GameObject gameObject = rayIntersection.collider.gameObject;
			if (IsExplodable(gameObject))
			{
				if (Input.GetMouseButtonDown(0))
				{
					ExplodeObject(gameObject);
				}
				else
				{
					ExplodeAfterCrack();
				}
			}
		}

		private void ExplodeObject(GameObject obj)
		{
			Exploder2DUtils.SetActive(exploder.gameObject, status: true);
			exploder.transform.position = Exploder2DUtils.GetCentroid(obj);
			exploder.Radius = 0.1f;
			exploder.Explode(OnExplosion);
		}

		private void OnExplosion(float time, Exploder2DObject.ExplosionState state)
		{
			if (state != Exploder2DObject.ExplosionState.ExplosionFinished)
			{
			}
		}

		private void OnCracked()
		{
		}

		private void ExplodeAfterCrack()
		{
		}

		private void OnGUI()
		{
		}
	}
}
