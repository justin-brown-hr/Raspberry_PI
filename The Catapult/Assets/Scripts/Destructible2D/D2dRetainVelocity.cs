using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Destructible 2D/D2D Retain Velocity")]
	public class D2dRetainVelocity : MonoBehaviour
	{
		[NonSerialized]
		private Rigidbody2D body;

		[NonSerialized]
		private D2dDestructible destructible;

		[NonSerialized]
		private float angularVelocity;

		[NonSerialized]
		private Vector2 velocity;

		protected virtual void OnEnable()
		{
			if (destructible == null)
			{
				destructible = GetComponent<D2dDestructible>();
			}
			if (destructible.OnStartSplit == null)
			{
				destructible.OnStartSplit = new D2dEvent();
			}
			if (destructible.OnEndSplit == null)
			{
				destructible.OnEndSplit = new D2dDestructibleListEvent();
			}
			destructible.OnStartSplit.AddListener(StartSplit);
			destructible.OnEndSplit.AddListener(EndSplit);
		}

		protected virtual void OnDisable()
		{
			destructible.OnStartSplit.RemoveListener(StartSplit);
			destructible.OnEndSplit.RemoveListener(EndSplit);
		}

		protected virtual void StartSplit()
		{
			if (body == null)
			{
				body = GetComponent<Rigidbody2D>();
			}
			velocity = body.velocity;
			angularVelocity = body.angularVelocity;
		}

		protected virtual void EndSplit(List<D2dDestructible> clones)
		{
			for (int num = clones.Count - 1; num >= 0; num--)
			{
				D2dDestructible d2dDestructible = clones[num];
				if (d2dDestructible.gameObject != base.gameObject)
				{
					Rigidbody2D component = d2dDestructible.GetComponent<Rigidbody2D>();
					if (component != null)
					{
						component.velocity = velocity;
						component.angularVelocity = angularVelocity;
					}
				}
			}
		}
	}
}
