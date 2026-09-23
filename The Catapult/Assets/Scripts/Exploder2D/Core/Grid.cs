using System.Collections.Generic;
using UnityEngine;

namespace Exploder2D.Core
{
	internal class Grid
	{
		private enum CellType
		{
			Out,
			Edge,
			In
		}

		private struct Cell
		{
			public Vector2 pos;

			public Vector2 center;

			public CellType type;

			public float size;

			public List<Vector2> pnts;

			public void AddPnt(Vector2 pnt)
			{
				if (pnts == null)
				{
					pnts = new List<Vector2>(2);
				}
				pnts.Add(pnt);
			}

			public void LineIntersection(Vector2 p0, Vector2 p1, short v, short h, IDictionary<long, Vector2> intersections)
			{
				int num = (v << 16) | h;
				Vector2 a = pos;
				Vector2 vector = pos + Vector2.up * size;
				if (MeshUtils.Test2DSegmentSegment(a, vector, p0, p1, out Vector2 p2))
				{
					int num2 = (v - 1 << 16) | h;
					long key = (num >= num2) ? (((long)num2 << 32) | (uint)num) : (((long)num << 32) | (uint)num2);
					intersections[key] = p2;
				}
				a = vector;
				vector += Vector2.right * size;
				if (MeshUtils.Test2DSegmentSegment(a, vector, p0, p1, out p2))
				{
					int num3 = (v << 16) | (h + 1);
					long key2 = (num >= num3) ? (((long)num3 << 32) | (uint)num) : (((long)num << 32) | (uint)num3);
					intersections[key2] = p2;
				}
				a = vector;
				vector = pos + Vector2.right * size;
				if (MeshUtils.Test2DSegmentSegment(a, vector, p0, p1, out p2))
				{
					int num4 = (v + 1 << 16) | h;
					long key3 = (num >= num4) ? (((long)num4 << 32) | (uint)num) : (((long)num << 32) | (uint)num4);
					intersections[key3] = p2;
				}
				a = pos;
				if (MeshUtils.Test2DSegmentSegment(a, vector, p0, p1, out p2))
				{
					int num5 = (v << 16) | (h - 1);
					long key4 = (num >= num5) ? (((long)num5 << 32) | (uint)num) : (((long)num << 32) | (uint)num5);
					intersections[key4] = p2;
				}
			}

			public void PointIntersection(Vector2 p, short v, short h, IDictionary<long, Vector2> intersections)
			{
				int num = (v << 16) | h;
				Vector2 vector = pos;
				if (Mathf.Abs(p.x - vector.x) < 0.01f)
				{
					int num2 = (v - 1 << 16) | h;
					long key = (num >= num2) ? (((long)num2 << 32) | (uint)num) : (((long)num << 32) | (uint)num2);
					intersections[key] = p;
				}
				vector = pos + Vector2.up * size;
				if (Mathf.Abs(p.y - vector.y) < 0.01f)
				{
					int num3 = (v << 16) | (h + 1);
					long key2 = (num >= num3) ? (((long)num3 << 32) | (uint)num) : (((long)num << 32) | (uint)num3);
					intersections[key2] = p;
				}
				vector = pos + Vector2.right * size;
				if (Mathf.Abs(p.x - vector.x) < 0.01f)
				{
					int num4 = (v + 1 << 16) | h;
					long key3 = (num >= num4) ? (((long)num4 << 32) | (uint)num) : (((long)num << 32) | (uint)num4);
					intersections[key3] = p;
				}
				vector = pos;
				if (Mathf.Abs(p.y - vector.x) < 0.01f)
				{
					int num5 = (v << 16) | (h - 1);
					long key4 = (num >= num5) ? (((long)num5 << 32) | (uint)num) : (((long)num << 32) | (uint)num5);
					intersections[key4] = p;
				}
			}
		}

		private readonly Vector2 min;

		private readonly float resolution;

		private readonly short hCount;

		private readonly short vCount;

		private readonly Dictionary<long, Vector2> intersections;

		private readonly Cell[][] grid;

		public Grid(Vector2 min, Vector2 max, float resolution)
		{
			this.min = min;
			this.resolution = resolution;
			Vector2 vector = max - min;
			if (resolution > vector.x)
			{
				resolution = vector.x;
			}
			if (resolution > vector.y)
			{
				resolution = vector.y;
			}
			hCount = (short)Mathf.Clamp(vector.x / resolution + 4f, 0f, 32767f);
			vCount = (short)Mathf.Clamp(vector.y / resolution + 4f, 0f, 32767f);
			intersections = new Dictionary<long, Vector2>(vCount * hCount);
			grid = new Cell[vCount][];
			for (int i = 0; i < vCount; i++)
			{
				grid[i] = new Cell[hCount];
				for (int j = 0; j < hCount; j++)
				{
					grid[i][j] = new Cell
					{
						pos = min + new Vector2((float)j * resolution - resolution * 1.5f, (float)i * resolution - resolution * 1.5f),
						type = CellType.Out,
						size = resolution
					};
				}
			}
		}

		public void Intersect(Vector2[] path)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < path.Length - 1; i++)
			{
				Vector2 a = path[i] - min;
				Vector2 vector = path[i + 1] - min;
				int num3 = (int)((double)(a - vector).sqrMagnitude / ((double)resolution * 0.3 * (double)resolution * 0.30000001192092896)) + 1;
				for (int j = 0; j <= num3; j++)
				{
					float num4 = (float)j / (float)num3;
					Vector2 a2 = a * (1f - num4) + vector * num4;
					short num5 = (short)((a2.x + resolution * 1.5f) / resolution);
					short num6 = (short)((a2.y + resolution * 1.5f) / resolution);
					grid[num6][num5].type = CellType.Edge;
					if (j == 0 || j == num3)
					{
						grid[num6][num5].LineIntersection(a + min, vector + min, num6, num5, intersections);
						grid[num6][num5].PointIntersection(a2 + min, num6, num5, intersections);
						grid[num6][num5].AddPnt(a2 + min);
					}
					else if (num6 != num || num5 != num2)
					{
						grid[num6][num5].LineIntersection(a + min, vector + min, num6, num5, intersections);
					}
					num = num6;
					num2 = num5;
				}
			}
			for (int k = 0; k < vCount; k++)
			{
				int num7 = -1;
				int num8 = -1;
				for (int l = 0; l < hCount; l++)
				{
					if (num7 == -1 && grid[k][l].type == CellType.Edge)
					{
						num7 = l;
					}
					if (num8 == -1 && grid[k][hCount - 1 - l].type == CellType.Edge)
					{
						num8 = hCount - 1 - l;
					}
					if (num7 != -1 && num8 != -1)
					{
						break;
					}
				}
				if (num7 == -1 || num8 == -1)
				{
					continue;
				}
				for (int m = num7 + 1; m < num8; m++)
				{
					if (grid[k][m].type != CellType.Edge)
					{
						grid[k][m].type = CellType.In;
					}
				}
			}
			foreach (KeyValuePair<long, Vector2> intersection in intersections)
			{
				int num9 = (int)intersection.Key;
				int num10 = (int)(intersection.Key >> 32);
				short num11 = (short)num9;
				short num12 = (short)(num9 >> 16);
				short num13 = (short)num10;
				short num14 = (short)(num10 >> 16);
				grid[num12][num11].AddPnt(intersection.Value);
				grid[num14][num13].AddPnt(intersection.Value);
			}
		}

		public void Triangulate()
		{
			for (int i = 1; i < vCount - 1; i++)
			{
				for (int j = 1; j < hCount - 1; j++)
				{
					if (grid[i][j].type == CellType.Edge)
					{
						Vector2 pos = grid[i][j].pos;
						int num = (int)((int)grid[i][j - 1].type + (int)grid[i - 1][j - 1].type + grid[i - 1][j].type);
						int num2 = (int)((int)grid[i][j - 1].type + (int)grid[i + 1][j - 1].type + grid[i + 1][j].type);
						if (num2 > num)
						{
							pos = grid[i + 1][j].pos;
							num = num2;
						}
						num2 = (int)((int)grid[i + 1][j].type + (int)grid[i + 1][j + 1].type + grid[i][j + 1].type);
						if (num2 > num)
						{
							pos = grid[i + 1][j + 1].pos;
							num = num2;
						}
						num2 = (int)((int)grid[i][j + 1].type + (int)grid[i - 1][j - 1].type + grid[i - 1][j].type);
						if (num2 > num)
						{
							pos = grid[i][j + 1].pos;
						}
						grid[i][j].center = pos;
					}
				}
			}
		}

		public void DebugDraw(Transform transform)
		{
			for (int i = 0; i < grid.Length; i++)
			{
				for (int j = 0; j < grid[i].Length; j++)
				{
					Vector2 v = transform.TransformPoint(grid[i][j].center);
					Vector2 vector = transform.TransformPoint(grid[i][j].pos);
					Vector2 b = transform.TransformDirection(Vector2.right * resolution * 1f);
					Vector2 b2 = transform.TransformDirection(Vector2.up * resolution * 1f);
					Color color;
					switch (grid[i][j].type)
					{
					case CellType.Edge:
						color = Color.red;
						break;
					case CellType.In:
						color = Color.yellow;
						break;
					default:
						color = Color.white;
						break;
					}
					UnityEngine.Debug.DrawLine(vector, vector + b, color, 10f);
					UnityEngine.Debug.DrawLine(vector, vector + b2, color, 10f);
					if (grid[i][j].type == CellType.Edge && grid[i][j].pnts != null)
					{
						foreach (Vector2 pnt in grid[i][j].pnts)
						{
							Vector2 v2 = transform.TransformPoint(pnt);
							UnityEngine.Debug.DrawLine(v, v2, Color.magenta, 10f);
						}
					}
				}
			}
		}
	}
}
