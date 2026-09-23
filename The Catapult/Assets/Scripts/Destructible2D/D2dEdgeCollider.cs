using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Edge Collider")]
	public class D2dEdgeCollider : D2dCollider
	{
		[Tooltip("The size of each collider cell")]
		[D2dPopup(new int[]
		{
			8,
			16,
			32,
			64,
			128,
			256
		})]
		public int CellSize = 64;

		[Tooltip("How many vertices should remain in the collider shapes")]
		[Range(0.5f, 1f)]
		public float Detail = 0.9f;

		[Tooltip("The minimum distance between vertices")]
		[Range(0.001f, 1f)]
		public float Weld = 0.01f;

		[SerializeField]
		private int expectedCellSize;

		[SerializeField]
		private int expectedWidth;

		[SerializeField]
		private int expectedHeight;

		[SerializeField]
		private int cellWidth;

		[SerializeField]
		private int cellHeight;

		[SerializeField]
		private D2dEdgeColliderCell[] cells;

		private static Stack<EdgeCollider2D> tempColliders = new Stack<EdgeCollider2D>();

		public override void UpdateColliderSettings()
		{
			if (cells != null)
			{
				for (int num = cells.Length - 1; num >= 0; num--)
				{
					cells[num]?.UpdateColliderSettings(IsTrigger, Material);
				}
			}
		}

		protected override void OnAlphaDataReplaced()
		{
			base.OnAlphaDataReplaced();
			Rebuild();
		}

		protected override void OnAlphaDataModified(D2dRect rect)
		{
			base.OnAlphaDataModified(rect);
			if (CellSize <= 0)
			{
				Mark();
				Sweep();
				return;
			}
			if (destructible.AlphaWidth != expectedWidth || destructible.AlphaHeight != expectedHeight || cells == null || cells.Length != cellWidth * cellHeight || CellSize != expectedCellSize)
			{
				Rebuild();
				return;
			}
			int value = rect.MinX / CellSize;
			int value2 = rect.MinY / CellSize;
			int value3 = (rect.MaxX + 1) / CellSize;
			int value4 = (rect.MaxY + 1) / CellSize;
			value = Mathf.Clamp(value, 0, cellWidth - 1);
			value3 = Mathf.Clamp(value3, 0, cellWidth - 1);
			value2 = Mathf.Clamp(value2, 0, cellHeight - 1);
			value4 = Mathf.Clamp(value4, 0, cellHeight - 1);
			for (int i = value2; i <= value4; i++)
			{
				int num = i * cellWidth;
				for (int j = value; j <= value3; j++)
				{
					int num2 = j + num;
					RebuildCell(ref cells[num2], j, i);
				}
			}
			Sweep();
		}

		protected override void OnAlphaDataSubset(D2dRect rect)
		{
			base.OnAlphaDataSubset(rect);
			Rebuild();
		}

		protected override void OnStartSplit()
		{
			base.OnStartSplit();
			Mark();
			Sweep();
		}

		private void Mark()
		{
			tempColliders.Clear();
			if (cells == null)
			{
				return;
			}
			for (int num = cells.Length - 1; num >= 0; num--)
			{
				D2dEdgeColliderCell d2dEdgeColliderCell = cells[num];
				if (d2dEdgeColliderCell != null)
				{
					d2dEdgeColliderCell.Clear(tempColliders);
					cells[num] = D2dEdgeColliderCell.Add(d2dEdgeColliderCell);
				}
			}
		}

		private void Sweep()
		{
			while (tempColliders.Count > 0)
			{
				D2dHelper.Destroy(tempColliders.Pop());
			}
		}

		private void Rebuild()
		{
			Mark();
			if (CellSize > 0)
			{
				expectedCellSize = CellSize;
				expectedWidth = destructible.AlphaWidth;
				expectedHeight = destructible.AlphaHeight;
				cellWidth = (expectedWidth + CellSize - 1) / CellSize;
				cellHeight = (expectedHeight + CellSize - 1) / CellSize;
				cells = new D2dEdgeColliderCell[cellWidth * cellHeight];
				for (int i = 0; i < cellHeight; i++)
				{
					int num = i * cellWidth;
					for (int j = 0; j < cellWidth; j++)
					{
						RebuildCell(ref cells[j + num], j, i);
					}
				}
			}
			Sweep();
		}

		private void RebuildCell(ref D2dEdgeColliderCell cell, int cellX, int cellY)
		{
			int num = CellSize * cellX;
			int num2 = CellSize * cellY;
			int num3 = Mathf.Min(CellSize + num, destructible.AlphaWidth);
			int num4 = Mathf.Min(CellSize + num2, destructible.AlphaHeight);
			if (num > 0)
			{
				num++;
			}
			if (num2 > 0)
			{
				num2++;
			}
			if (num3 < destructible.AlphaWidth)
			{
				num3--;
			}
			if (num4 < destructible.AlphaHeight)
			{
				num4--;
			}
			D2dColliderBuilder.AlphaData = destructible.AlphaData;
			D2dColliderBuilder.AlphaWidth = destructible.AlphaWidth;
			D2dColliderBuilder.AlphaHeight = destructible.AlphaHeight;
			D2dColliderBuilder.MinX = num;
			D2dColliderBuilder.MinY = num2;
			D2dColliderBuilder.MaxX = num3;
			D2dColliderBuilder.MaxY = num4;
			D2dColliderBuilder.CalculateEdgeCells();
			if (cell == null)
			{
				cell = D2dEdgeColliderCell.Get();
			}
			cell.Clear(tempColliders);
			D2dColliderBuilder.BuildEdge(cell, tempColliders, child, Weld, Detail);
			cell.UpdateColliderSettings(IsTrigger, Material);
		}
	}
}
