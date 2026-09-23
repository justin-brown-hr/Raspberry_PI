using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Destructible 2D/D2D Fixture")]
	public class D2dFixture : MonoBehaviour
	{
		[Tooltip("The fixture offset position")]
		public Vector3 Offset;

		[NonSerialized]
		private D2dDestructible destructible;

		protected virtual void OnEnable()
		{
			if (destructible == null)
			{
				destructible = GetComponentInParent<D2dDestructible>();
			}
			if (destructible.OnStartSplit == null)
			{
				destructible.OnStartSplit = new D2dEvent();
			}
			if (destructible.OnEndSplit == null)
			{
				destructible.OnEndSplit = new D2dDestructibleListEvent();
			}
			Hook();
		}

		protected virtual void OnDisable()
		{
			Unhook();
		}

		protected virtual void Update()
		{
			UpdateFixture();
		}

		private void UpdateFixture()
		{
			if (destructible == null)
			{
				destructible = GetComponentInParent<D2dDestructible>();
			}
			if (destructible == null)
			{
				DestroyFixture();
				return;
			}
			Vector3 worldPosition = base.transform.TransformPoint(Offset);
			if (destructible.SampleAlpha(worldPosition) < 0.5f)
			{
				DestroyFixture();
			}
		}

		private void DestroyFixture()
		{
			D2dHelper.Destroy(base.gameObject);
		}

		private void OnStartSplit()
		{
			base.transform.SetParent(null, worldPositionStays: false);
		}

		private void OnEndSplit(List<D2dDestructible> clones)
		{
			for (int num = clones.Count - 1; num >= 0; num--)
			{
				D2dDestructible newDestructible = clones[num];
				if (TryFixTo(newDestructible))
				{
					return;
				}
			}
			DestroyFixture();
		}

		private bool TryFixTo(D2dDestructible newDestructible)
		{
			bool flag = destructible != newDestructible;
			base.transform.SetParent(newDestructible.transform, worldPositionStays: false);
			Vector3 worldPosition = base.transform.TransformPoint(Offset);
			if (newDestructible.SampleAlpha(worldPosition) > 0.5f)
			{
				if (flag)
				{
					Unhook();
					destructible = newDestructible;
					Hook();
				}
				return true;
			}
			base.transform.SetParent(destructible.transform, worldPositionStays: false);
			return false;
		}

		private void Hook()
		{
			destructible.OnStartSplit.AddListener(OnStartSplit);
			destructible.OnEndSplit.AddListener(OnEndSplit);
		}

		private void Unhook()
		{
			destructible.OnStartSplit.RemoveListener(OnStartSplit);
			destructible.OnEndSplit.RemoveListener(OnEndSplit);
		}
	}
}
