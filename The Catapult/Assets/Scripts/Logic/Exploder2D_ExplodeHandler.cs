using Destructible2D;
using Exploder2D;
using Exploder2D.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
	public class Exploder2D_ExplodeHandler : MonoBehaviour
	{
		private Exploder2DObject _exploder;

		private Queue<GameObject> listToDestroy;

		private bool destroyingOne;

		private void Start()
		{
			listToDestroy = new Queue<GameObject>();
			destroyingOne = false;
			_exploder = Exploder2DSingleton.Exploder2DInstance;
		}

		public void ExplodeObject(GameObject obj)
		{
			listToDestroy.Enqueue(obj);
		}

		public void ExplodeOneObject(GameObject obj)
		{
			if (obj.GetComponent<D2dDestructible>() != null)
			{
				D2dDestructible component = obj.GetComponent<D2dDestructible>();
				component.OnEndSplit.AddListener(OnEndSplit);
				D2dQuadFracturer.Fracture(component, 5, 0.5f);
				component.OnEndSplit.RemoveListener(OnEndSplit);
				return;
			}
			Exploder2DUtils.SetActive(_exploder.gameObject, status: true);
			if (obj != null)
			{
				_exploder.transform.position = Exploder2DUtils.GetCentroid(obj);
				_exploder.Radius = 0.1f;
				if (obj != null)
				{
					_exploder.Explode(OnExplosion, obj);
				}
				else
				{
					destroyingOne = false;
				}
			}
			else
			{
				destroyingOne = false;
			}
		}

		private void OnEndSplit(List<D2dDestructible> clones)
		{
			for (int num = clones.Count - 1; num >= 0; num--)
			{
				D2dDestructible d2dDestructible = clones[num];
				d2dDestructible.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				if (d2dDestructible.GetComponent<FixedJoint2D>() != null)
				{
					UnityEngine.Object.Destroy(d2dDestructible.GetComponent<FixedJoint2D>());
				}
				if (d2dDestructible.GetComponent<SpringJoint2D>() != null)
				{
					UnityEngine.Object.Destroy(d2dDestructible.GetComponent<SpringJoint2D>());
				}
				if (d2dDestructible.GetComponent<HingeJoint2D>() != null)
				{
					UnityEngine.Object.Destroy(d2dDestructible.GetComponent<HingeJoint2D>());
				}
			}
		}

		private void OnExplosion(float time, Exploder2DObject.ExplosionState state)
		{
			if (state == Exploder2DObject.ExplosionState.ExplosionFinished)
			{
				List<Fragment2D> activeFragments = FragmentPool2D.Instance.GetActiveFragments();
				for (int i = 0; i < activeFragments.Count; i++)
				{
					if (activeFragments[i] == null)
					{
						UnityEngine.Object.Destroy(activeFragments[i].gameObject);
						continue;
					}
					activeFragments[i].gameObject.tag = "ExploderFragment";
					activeFragments[i].gameObject.layer = UnityEngine.Random.Range(14, 16);
					activeFragments[i].GetComponent<Rigidbody2D>().gravityScale = 3f;
				}
			}
			destroyingOne = false;
		}

		public void ClearList()
		{
			listToDestroy.Clear();
		}

		private void FixedUpdate()
		{
			if (listToDestroy.Count <= 0 || destroyingOne)
			{
				return;
			}
			destroyingOne = true;
			GameObject gameObject = listToDestroy.Dequeue();
			if (gameObject != null)
			{
				ExplodeOneObject(gameObject);
				if (gameObject != null)
				{
					listToDestroy.Enqueue(gameObject);
				}
			}
			else
			{
				destroyingOne = false;
			}
		}
	}
}
