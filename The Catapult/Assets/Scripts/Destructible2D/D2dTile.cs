using System;
using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[AddComponentMenu("Destructible 2D/D2D Tile")]
	public class D2dTile : MonoBehaviour
	{
		[Tooltip("The size of this tile in local coordinates")]
		public Vector2 Size;

		[Tooltip("The position offset of this tile in local coordinates")]
		public D2dVector2 Offset;

		[NonSerialized]
		private Renderer mainRenderer;

		public void UpdatePosition(Vector2 offset)
		{
			Camera main = Camera.main;
			if (main != null && Size.x > 0f && Size.y > 0f)
			{
				Vector3 localPosition = base.transform.localPosition;
				Vector3 vector = main.transform.position - (Vector3)offset;
				localPosition.x = (float)Mathf.RoundToInt(vector.x / Size.x + (float)Offset.X) * Size.x + offset.x;
				localPosition.y = (float)Mathf.RoundToInt(vector.y / Size.y + (float)Offset.Y) * Size.y + offset.y;
				base.transform.localPosition = localPosition;
			}
		}

		public void UpdateRenderer(int sortingOrder)
		{
			if (mainRenderer == null)
			{
				mainRenderer = GetComponent<Renderer>();
			}
			if (mainRenderer != null)
			{
				mainRenderer.sortingOrder = sortingOrder;
			}
		}

		protected virtual void Update()
		{
			UpdatePosition(Vector3.zero);
		}
	}
}
