using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[Serializable]
	public class D2dEdgeColliderCell
	{
		public List<EdgeCollider2D> Colliders;

		public static Stack<D2dEdgeColliderCell> pool = new Stack<D2dEdgeColliderCell>();

		public static D2dEdgeColliderCell Add(D2dEdgeColliderCell cell)
		{
			pool.Push(cell);
			return null;
		}

		public static D2dEdgeColliderCell Get()
		{
			if (pool.Count > 0)
			{
				return pool.Pop();
			}
			return new D2dEdgeColliderCell();
		}

		public EdgeCollider2D AddPath(Stack<EdgeCollider2D> tempColliders, GameObject child, Vector2[] points)
		{
			EdgeCollider2D edgeCollider2D = null;
			edgeCollider2D = ((tempColliders.Count <= 0) ? child.AddComponent<EdgeCollider2D>() : tempColliders.Pop());
			edgeCollider2D.points = points;
			if (Colliders == null)
			{
				Colliders = new List<EdgeCollider2D>();
			}
			Colliders.Add(edgeCollider2D);
			return edgeCollider2D;
		}

		public void Clear(Stack<EdgeCollider2D> tempColliders)
		{
			if (Colliders == null)
			{
				return;
			}
			for (int num = Colliders.Count - 1; num >= 0; num--)
			{
				EdgeCollider2D edgeCollider2D = Colliders[num];
				if (edgeCollider2D != null)
				{
					tempColliders.Push(edgeCollider2D);
				}
			}
			Colliders.Clear();
		}

		public void UpdateColliderSettings(bool isTrigger, PhysicsMaterial2D material)
		{
			if (Colliders == null)
			{
				return;
			}
			for (int num = Colliders.Count - 1; num >= 0; num--)
			{
				EdgeCollider2D edgeCollider2D = Colliders[num];
				if (edgeCollider2D != null)
				{
					edgeCollider2D.isTrigger = isTrigger;
					edgeCollider2D.sharedMaterial = material;
				}
			}
		}
	}
}
