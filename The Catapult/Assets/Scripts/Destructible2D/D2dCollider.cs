using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(D2dDestructible))]
	public abstract class D2dCollider : MonoBehaviour
	{
		[Tooltip("Should these colliders be marked as triggers?")]
		public bool IsTrigger;

		[Tooltip("The physics material applied to these colliders")]
		public PhysicsMaterial2D Material;

		[SerializeField]
		protected GameObject child;

		[SerializeField]
		protected bool awoken;

		[NonSerialized]
		protected D2dDestructible destructible;

		[NonSerialized]
		private GameObject tempChild;

		public abstract void UpdateColliderSettings();

		[ContextMenu("Regenerate")]
		public void Regenerate()
		{
			OnAlphaDataReplaced();
		}

		public void DestroyChild()
		{
			if (child != null)
			{
				child = D2dHelper.Destroy(child);
			}
		}

		protected virtual void OnEnable()
		{
			if (destructible == null)
			{
				destructible = GetComponent<D2dDestructible>();
			}
			if (destructible.OnAlphaDataReplaced == null)
			{
				destructible.OnAlphaDataReplaced = new D2dEvent();
			}
			if (destructible.OnAlphaDataModified == null)
			{
				destructible.OnAlphaDataModified = new D2dD2dRectEvent();
			}
			if (destructible.OnAlphaDataSubset == null)
			{
				destructible.OnAlphaDataSubset = new D2dD2dRectEvent();
			}
			if (destructible.OnStartSplit == null)
			{
				destructible.OnStartSplit = new D2dEvent();
			}
			if (destructible.OnEndSplit == null)
			{
				destructible.OnEndSplit = new D2dDestructibleListEvent();
			}
			destructible.OnAlphaDataReplaced.AddListener(OnAlphaDataReplaced);
			destructible.OnAlphaDataModified.AddListener(OnAlphaDataModified);
			destructible.OnAlphaDataSubset.AddListener(OnAlphaDataSubset);
			destructible.OnStartSplit.AddListener(OnStartSplit);
			destructible.OnEndSplit.AddListener(OnEndSplit);
			if (child != null)
			{
				child.SetActive(value: true);
			}
		}

		protected virtual void OnDisable()
		{
			destructible.OnAlphaDataReplaced.RemoveListener(OnAlphaDataReplaced);
			destructible.OnAlphaDataModified.RemoveListener(OnAlphaDataModified);
			destructible.OnAlphaDataSubset.RemoveListener(OnAlphaDataSubset);
			destructible.OnStartSplit.RemoveListener(OnStartSplit);
			destructible.OnEndSplit.RemoveListener(OnEndSplit);
			if (child != null)
			{
				child.SetActive(value: false);
			}
			if (destructible.IsOnStartSplit)
			{
				if (child != null)
				{
					child.transform.SetParent(null, worldPositionStays: false);
					child = D2dHelper.Destroy(child);
				}
				if (tempChild != null)
				{
					tempChild = D2dHelper.Destroy(tempChild);
				}
			}
		}

		protected virtual void Awake()
		{
			if (GetComponent<Collider2D>() != null)
			{
				Collider2D[] components = GetComponents<Collider2D>();
				for (int num = components.Length - 1; num >= 0; num--)
				{
					D2dHelper.Destroy(components[num]);
				}
			}
		}

		protected virtual void Start()
		{
			if (!awoken)
			{
				awoken = true;
				OnAlphaDataReplaced();
			}
		}

		protected virtual void Update()
		{
			if (child == null)
			{
				OnAlphaDataReplaced();
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				Regenerate();
				stopwatch.Stop();
				UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
			}
		}

		protected virtual void OnDestroy()
		{
			DestroyChild();
		}

		protected virtual void OnAlphaDataReplaced()
		{
			UpdateBeforeBuild();
		}

		protected virtual void OnAlphaDataModified(D2dRect rect)
		{
			UpdateBeforeBuild();
		}

		protected virtual void OnAlphaDataSubset(D2dRect rect)
		{
			UpdateBeforeBuild();
		}

		protected virtual void OnStartSplit()
		{
			if (child != null)
			{
				child.transform.SetParent(null, worldPositionStays: false);
				tempChild = child;
				child = null;
			}
		}

		protected virtual void OnEndSplit(List<D2dDestructible> clones)
		{
			ReconnectChild();
		}

		private void UpdateBeforeBuild()
		{
			if (destructible == null)
			{
				destructible = GetComponent<D2dDestructible>();
			}
			if (child == null)
			{
				ReconnectChild();
				if (child == null)
				{
					child = new GameObject("Collider");
					child.layer = base.transform.gameObject.layer;
					child.transform.SetParent(base.transform, worldPositionStays: false);
				}
			}
			if (destructible.AlphaIsValid)
			{
				float x = destructible.AlphaRect.x;
				float y = destructible.AlphaRect.y;
				float x2 = destructible.AlphaRect.width / (float)destructible.AlphaWidth;
				float y2 = destructible.AlphaRect.height / (float)destructible.AlphaHeight;
				child.transform.localPosition = new Vector3(x, y, 0f);
				child.transform.localScale = new Vector3(x2, y2, 0f);
			}
		}

		private void ReconnectChild()
		{
			if (tempChild != null)
			{
				child = tempChild;
				child.transform.SetParent(base.transform, worldPositionStays: false);
				tempChild = null;
			}
		}
	}
}
