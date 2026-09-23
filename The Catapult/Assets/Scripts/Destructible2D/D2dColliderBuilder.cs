using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	public static class D2dColliderBuilder
	{
		private enum Edge
		{
			Left,
			Right,
			Bottom,
			Top
		}

		private class Point
		{
			public bool Used;

			public Vector2 Position;

			public Point Other;

			public Cell OppositeCell;

			public int OppositeIndex;
		}

		private class Cell
		{
			public Point[] Points = new Point[4];
		}

		private static List<Cell> cells = new List<Cell>();

		private static int cellCount;

		private static int cellsWidth;

		private static int cellsHeight;

		public static byte[] AlphaData;

		public static int AlphaWidth;

		public static int AlphaHeight;

		public static int MinX;

		public static int MaxX;

		public static int MinY;

		public static int MaxY;

		private static List<Point> points = new List<Point>();

		private static int pointCount;

		private static Cell cell;

		private static D2dLinkedList<Point> lines = new D2dLinkedList<Point>();

		private static readonly int[] opposite = new int[4]
		{
			1,
			0,
			3,
			2
		};

		private static byte GetPolyAlpha(int x, int y)
		{
			if (x >= MinX && x < MaxX && y >= MinY && y < MaxY)
			{
				return AlphaData[x + y * AlphaWidth];
			}
			return 0;
		}

		private static byte GetEdgeAlpha(int x, int y)
		{
			if (x >= 0 && x < AlphaWidth && y >= 0 && y < AlphaHeight)
			{
				return AlphaData[x + y * AlphaWidth];
			}
			return 0;
		}

		public static void CalculatePolyCells()
		{
			cellsWidth = MaxX - MinX + 2;
			cellsHeight = MaxY - MinY + 2;
			pointCount = 0;
			for (int num = cellsWidth * cellsHeight - cells.Count; num > 0; num--)
			{
				cells.Add(new Cell());
			}
			for (int i = MinY - 1; i <= MaxY; i++)
			{
				int num2 = (i - MinY + 1) * cellsWidth - MinX + 1;
				for (int j = MinX - 1; j <= MaxX; j++)
				{
					byte polyAlpha = GetPolyAlpha(j, i);
					bool flag = polyAlpha >= 128;
					byte polyAlpha2 = GetPolyAlpha(j + 1, i);
					bool flag2 = polyAlpha2 >= 128;
					byte polyAlpha3 = GetPolyAlpha(j, i + 1);
					bool flag3 = polyAlpha3 >= 128;
					byte polyAlpha4 = GetPolyAlpha(j + 1, i + 1);
					bool flag4 = polyAlpha4 >= 128;
					int num3 = (flag ? 1 : 0) + (flag2 ? 2 : 0) + (flag3 ? 4 : 0) + (flag4 ? 8 : 0);
					int num4 = num2 + j;
					cell = cells[num4];
					if (num3 > 0 && num3 < 15)
					{
						float num5 = (float)j + 0.5f;
						float num6 = (float)i + 0.5f;
						if (flag ^ flag2)
						{
							Build(0, num5 + (float)(polyAlpha - 128) / (float)(polyAlpha - polyAlpha2), num6);
						}
						if (flag3 ^ flag4)
						{
							Build(1, num5 + (float)(polyAlpha3 - 128) / (float)(polyAlpha3 - polyAlpha4), num6 + 1f);
						}
						if (flag ^ flag3)
						{
							Build(2, num5, num6 + (float)(polyAlpha - 128) / (float)(polyAlpha - polyAlpha3));
						}
						if (flag2 ^ flag4)
						{
							Build(3, num5 + 1f, num6 + (float)(polyAlpha2 - 128) / (float)(polyAlpha2 - polyAlpha4));
						}
						switch (num3)
						{
						case 1:
						case 14:
							Link(0, 2, num4 - cellsWidth, num4 - 1);
							break;
						case 2:
						case 13:
							Link(0, 3, num4 - cellsWidth, num4 + 1);
							break;
						case 4:
						case 11:
							Link(1, 2, num4 + cellsWidth, num4 - 1);
							break;
						case 7:
						case 8:
							Link(1, 3, num4 + cellsWidth, num4 + 1);
							break;
						case 3:
						case 12:
							Link(2, 3, num4 - 1, num4 + 1);
							break;
						case 5:
						case 10:
							Link(0, 1, num4 - cellsWidth, num4 + cellsWidth);
							break;
						case 6:
							Link(1, 2, num4 + cellsWidth, num4 - 1);
							Link(0, 3, num4 - cellsWidth, num4 + 1);
							break;
						case 9:
							Link(0, 2, num4 - cellsWidth, num4 - 1);
							Link(1, 3, num4 + cellsWidth, num4 + 1);
							break;
						}
					}
				}
			}
		}

		public static void CalculateEdgeCells()
		{
			cellsWidth = MaxX - MinX + 2;
			cellsHeight = MaxY - MinY + 2;
			pointCount = 0;
			for (int num = cellsWidth * cellsHeight - cells.Count; num > 0; num--)
			{
				cells.Add(new Cell());
			}
			for (int i = MinY - 1; i <= MaxY; i++)
			{
				int num2 = (i - MinY + 1) * cellsWidth - MinX + 1;
				for (int j = MinX - 1; j <= MaxX; j++)
				{
					byte edgeAlpha = GetEdgeAlpha(j, i);
					bool flag = edgeAlpha >= 128;
					byte edgeAlpha2 = GetEdgeAlpha(j + 1, i);
					bool flag2 = edgeAlpha2 >= 128;
					byte edgeAlpha3 = GetEdgeAlpha(j, i + 1);
					bool flag3 = edgeAlpha3 >= 128;
					byte edgeAlpha4 = GetEdgeAlpha(j + 1, i + 1);
					bool flag4 = edgeAlpha4 >= 128;
					int num3 = (flag ? 1 : 0) + (flag2 ? 2 : 0) + (flag3 ? 4 : 0) + (flag4 ? 8 : 0);
					int index = num2 + j;
					cell = cells[index];
					if (num3 > 0 && num3 < 15)
					{
						float num4 = (float)j + 0.5f;
						float num5 = (float)i + 0.5f;
						if (flag ^ flag2)
						{
							Build(0, num4 + (float)(edgeAlpha - 128) / (float)(edgeAlpha - edgeAlpha2), num5);
						}
						if (flag3 ^ flag4)
						{
							Build(1, num4 + (float)(edgeAlpha3 - 128) / (float)(edgeAlpha3 - edgeAlpha4), num5 + 1f);
						}
						if (flag ^ flag3)
						{
							Build(2, num4, num5 + (float)(edgeAlpha - 128) / (float)(edgeAlpha - edgeAlpha3));
						}
						if (flag2 ^ flag4)
						{
							Build(3, num4 + 1f, num5 + (float)(edgeAlpha2 - 128) / (float)(edgeAlpha2 - edgeAlpha4));
						}
						int num6 = j - MinX + 1;
						int num7 = i - MinY + 1;
						switch (num3)
						{
						case 1:
						case 14:
							Link(0, 2, num6, num7 - 1, num6 - 1, num7);
							break;
						case 2:
						case 13:
							Link(0, 3, num6, num7 - 1, num6 + 1, num7);
							break;
						case 4:
						case 11:
							Link(1, 2, num6, num7 + 1, num6 - 1, num7);
							break;
						case 7:
						case 8:
							Link(1, 3, num6, num7 + 1, num6 + 1, num7);
							break;
						case 3:
						case 12:
							Link(2, 3, num6 - 1, num7, num6 + 1, num7);
							break;
						case 5:
						case 10:
							Link(0, 1, num6, num7 - 1, num6, num7 + 1);
							break;
						case 6:
							Link(1, 2, num6, num7 + 1, num6 - 1, num7);
							Link(0, 3, num6, num7 - 1, num6 + 1, num7);
							break;
						case 9:
							Link(0, 2, num6, num7 - 1, num6 - 1, num7);
							Link(1, 3, num6, num7 + 1, num6 + 1, num7);
							break;
						}
					}
				}
			}
		}

		private static void Build(int index, float x, float y)
		{
			Point point = cell.Points[index] = GetNextPoint();
			point.Position.x = x;
			point.Position.y = y;
			point.OppositeCell = null;
		}

		private static void Link(int edgeA, int edgeB, int xa, int ya, int xb, int yb)
		{
			Point point = cell.Points[edgeA];
			Point point2 = point.Other = cell.Points[edgeB];
			point2.Other = point;
			if (xa >= 0 && xa < cellsWidth && ya >= 0 && ya < cellsHeight)
			{
				point.OppositeCell = cells[xa + ya * cellsWidth];
				point.OppositeIndex = opposite[edgeA];
			}
			if (xb >= 0 && xb < cellsWidth && yb >= 0 && yb < cellsHeight)
			{
				point2.OppositeCell = cells[xb + yb * cellsWidth];
				point2.OppositeIndex = opposite[edgeB];
			}
		}

		private static void Link(int edgeA, int edgeB, int indexA, int indexB)
		{
			Point point = cell.Points[edgeA];
			Point point2 = point.Other = cell.Points[edgeB];
			point2.Other = point;
			point.OppositeCell = cells[indexA];
			point2.OppositeCell = cells[indexB];
			point.OppositeIndex = opposite[edgeA];
			point2.OppositeIndex = opposite[edgeB];
		}

		public static void BuildPoly(D2dPolygonColliderCell cell, Stack<PolygonCollider2D> tempColliders, GameObject child, float weld, float detail)
		{
			for (int i = 0; i < pointCount; i++)
			{
				Point point = points[i];
				if (!point.Used)
				{
					Trace(point);
					WeldLines(weld);
					OptimizeEdges(detail);
					cell.AddPolygon(tempColliders, child, ExtractPoints(1));
				}
			}
		}

		public static void BuildEdge(D2dEdgeColliderCell cell, Stack<EdgeCollider2D> tempColliders, GameObject child, float weld, float detail)
		{
			for (int i = 0; i < pointCount; i++)
			{
				Point point = points[i];
				if (!point.Used)
				{
					Trace(point);
					WeldLines(weld);
					OptimizeEdges(detail);
					cell.AddPath(tempColliders, child, ExtractPoints(0));
				}
			}
		}

		private static void WeldLines(float threshold)
		{
			if (lines.Count <= 2)
			{
				return;
			}
			D2dLinkedList<Point>.Node first = lines.First;
			D2dLinkedList<Point>.Node node = first.Next;
			D2dLinkedList<Point>.Node next = node.Next;
			Vector2 b = node.Value.Position - first.Value.Position;
			while (next != null)
			{
				Vector2 vector = next.Value.Position - node.Value.Position;
				if ((vector - b).sqrMagnitude < threshold)
				{
					lines.Remove(node);
				}
				else
				{
					b = vector;
				}
				node = next;
				next = node.Next;
			}
		}

		private static void OptimizeEdges(float detail)
		{
			if (!(detail < 1f) || lines.Count <= 2)
			{
				return;
			}
			D2dLinkedList<Point>.Node node = lines.First;
			D2dLinkedList<Point>.Node node2 = node.Next;
			D2dLinkedList<Point>.Node next = node2.Next;
			while (next != lines.Last)
			{
				Vector2 position = node.Value.Position;
				Vector2 position2 = node2.Value.Position;
				Vector2 position3 = next.Value.Position;
				Vector3 lhs = Vector3.Normalize(position2 - position);
				Vector3 rhs = Vector3.Normalize(position3 - position2);
				float num = Vector3.Dot(lhs, rhs);
				if (num > detail)
				{
					lines.Remove(node2);
					node2 = next;
					next = next.Next;
				}
				else
				{
					node = node2;
					node2 = next;
					next = next.Next;
				}
			}
		}

		public static Vector2[] ExtractPoints(int skip)
		{
			int num = lines.Count - skip;
			if (num > 1)
			{
				Vector2[] array = new Vector2[num];
				D2dLinkedList<Point>.Node node = lines.First;
				for (int i = 0; i < num; i++)
				{
					array[i] = node.Value.Position;
					node = node.Next;
				}
				return array;
			}
			return null;
		}

		private static void Trace(Point point)
		{
			lines.Clear();
			lines.AddFirst(point);
			while (!point.Used)
			{
				Point other = point.Other;
				lines.AddLast(other);
				point.Used = true;
				other.Used = true;
				if (other.OppositeCell == null)
				{
					break;
				}
				point = other.OppositeCell.Points[other.OppositeIndex];
			}
		}

		private static Point GetNextPoint()
		{
			Point point = null;
			if (pointCount >= points.Count)
			{
				point = new Point();
				points.Add(point);
			}
			else
			{
				point = points[pointCount];
				point.Used = false;
			}
			pointCount++;
			return point;
		}
	}
}
