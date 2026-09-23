using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[AddComponentMenu("Destructible 2D/D2D Background")]
	public class D2dBackground : MonoBehaviour
	{
		public enum Axes
		{
			Horizontal,
			Vertical,
			HorizontalAndVertical
		}

		[Tooltip("The prefab used to render the background tiles")]
		public D2dTile Prefab;

		[Tooltip("The axes the background will tile across")]
		public Axes TileAxis = Axes.HorizontalAndVertical;

		[Tooltip("Scroll the background?")]
		public Vector2 OffsetPerSecond;

		[Tooltip("The current scrolling position")]
		public Vector2 Offset;

		[Tooltip("Override the sorting of all background renderers?")]
		public bool OverrideSorting;

		[Tooltip("The new sorting order")]
		public int SortingOrder;

		[SerializeField]
		private List<D2dTile> tiles;

		protected virtual void Update()
		{
			Offset += OffsetPerSecond * Time.deltaTime;
			UpdateTiles();
		}

		private void UpdateTiles()
		{
			int num = 0;
			if (Prefab != null && Prefab.Size.x > 0f && Prefab.Size.y > 0f)
			{
				Camera main = Camera.main;
				if (main != null && main.orthographic)
				{
					int num2 = Mathf.CeilToInt(main.orthographicSize * main.aspect / Prefab.Size.x);
					int num3 = Mathf.CeilToInt(main.orthographicSize / Prefab.Size.y);
					if (TileAxis == Axes.Horizontal)
					{
						num3 = 0;
					}
					if (TileAxis == Axes.Vertical)
					{
						num2 = 0;
					}
					for (int i = -num3; i <= num3; i++)
					{
						for (int j = -num2; j <= num2; j++)
						{
							if (num == tiles.Count)
							{
								tiles.Add(null);
							}
							D2dTile d2dTile = tiles[num];
							if (d2dTile == null)
							{
								d2dTile = UnityEngine.Object.Instantiate(Prefab);
								d2dTile.enabled = false;
								d2dTile.transform.SetParent(base.transform, worldPositionStays: false);
								tiles[num] = d2dTile;
							}
							if (OverrideSorting)
							{
								d2dTile.UpdateRenderer(SortingOrder);
							}
							d2dTile.Offset.X = j;
							d2dTile.Offset.Y = i;
							d2dTile.UpdatePosition(Offset);
							num++;
						}
					}
				}
			}
			for (int num4 = tiles.Count - 1; num4 >= num; num4--)
			{
				D2dTile d2dTile2 = tiles[num4];
				if (d2dTile2 != null)
				{
					D2dHelper.Destroy(d2dTile2.gameObject);
				}
				tiles.RemoveAt(num4);
			}
		}
	}
}
