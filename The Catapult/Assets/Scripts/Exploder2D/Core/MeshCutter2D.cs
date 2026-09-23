using Exploder2D.Core.Math;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Exploder2D.Core
{
	public class MeshCutter2D
	{
		private struct Triangle
		{
			public int[] ids;

			public Vector2[] pos;
		}

		private List<int>[] triangles;

		private List<Vector2>[] vertices;

		private List<int> cutTris;

		private int[] triCache;

		private Vector2[] centroid;

		private int[] triCounter;

		private Dictionary<long, int>[] cutVertCache;

		private Dictionary<int, int>[] cornerVertCache;

		private int contourBufferSize;

		public void Init(int trianglesNum, int verticesNum)
		{
			AllocateBuffers(trianglesNum, verticesNum);
		}

		private void AllocateBuffers(int trianglesNum, int verticesNum)
		{
			if (triangles == null || triangles[0].Capacity < trianglesNum)
			{
				triangles = new List<int>[2]
				{
					new List<int>(trianglesNum),
					new List<int>(trianglesNum)
				};
			}
			else
			{
				triangles[0].Clear();
				triangles[1].Clear();
			}
			if (vertices == null || vertices[0].Capacity < verticesNum || triCache.Length < verticesNum)
			{
				vertices = new List<Vector2>[2]
				{
					new List<Vector2>(verticesNum),
					new List<Vector2>(verticesNum)
				};
				centroid = new Vector2[2];
				triCache = new int[verticesNum + 1];
				triCounter = new int[2];
				cutTris = new List<int>(verticesNum / 3);
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				vertices[i].Clear();
				centroid[i] = Vector2.zero;
				triCounter[i] = 0;
			}
			cutTris.Clear();
			for (int j = 0; j < triCache.Length; j++)
			{
				triCache[j] = 0;
			}
		}

		private void AllocateContours(int cutTrianglesNum)
		{
			if (cutVertCache == null || contourBufferSize < cutTrianglesNum)
			{
				cutVertCache = new Dictionary<long, int>[2]
				{
					new Dictionary<long, int>(cutTrianglesNum * 2),
					new Dictionary<long, int>(cutTrianglesNum * 2)
				};
				cornerVertCache = new Dictionary<int, int>[2]
				{
					new Dictionary<int, int>(cutTrianglesNum),
					new Dictionary<int, int>(cutTrianglesNum)
				};
				contourBufferSize = cutTrianglesNum;
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				cutVertCache[i].Clear();
				cornerVertCache[i].Clear();
			}
		}

		public float Cut(SpriteMesh spriteMesh, Transform meshTransform, Line2D line2D, ref List<CutterMesh> meshes)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int num = spriteMesh.triangles.Length;
			int verticesNum = spriteMesh.vertices.Length;
			int[] array = spriteMesh.triangles;
			Vector2[] array2 = spriteMesh.vertices;
			AllocateBuffers(num, verticesNum);
			for (int i = 0; i < num; i += 3)
			{
				Vector2 n = array2[array[i]];
				Vector2 n2 = array2[array[i + 1]];
				Vector2 n3 = array2[array[i + 2]];
				bool sideFix = line2D.GetSideFix(ref n);
				bool sideFix2 = line2D.GetSideFix(ref n2);
				bool sideFix3 = line2D.GetSideFix(ref n3);
				array2[array[i]] = n;
				array2[array[i + 1]] = n2;
				array2[array[i + 2]] = n3;
				if (sideFix == sideFix2 && sideFix2 == sideFix3)
				{
					int num2 = (!sideFix) ? 1 : 0;
					if (array[i] >= triCache.Length)
					{
					}
					if (triCache[array[i]] == 0)
					{
						triangles[num2].Add(triCounter[num2]);
						vertices[num2].Add(array2[array[i]]);
						centroid[num2] += array2[array[i]];
						triCache[array[i]] = triCounter[num2] + 1;
						triCounter[num2]++;
					}
					else
					{
						triangles[num2].Add(triCache[array[i]] - 1);
					}
					if (triCache[array[i + 1]] == 0)
					{
						triangles[num2].Add(triCounter[num2]);
						vertices[num2].Add(array2[array[i + 1]]);
						centroid[num2] += array2[array[i + 1]];
						triCache[array[i + 1]] = triCounter[num2] + 1;
						triCounter[num2]++;
					}
					else
					{
						triangles[num2].Add(triCache[array[i + 1]] - 1);
					}
					if (triCache[array[i + 2]] == 0)
					{
						triangles[num2].Add(triCounter[num2]);
						vertices[num2].Add(array2[array[i + 2]]);
						centroid[num2] += array2[array[i + 2]];
						triCache[array[i + 2]] = triCounter[num2] + 1;
						triCounter[num2]++;
					}
					else
					{
						triangles[num2].Add(triCache[array[i + 2]] - 1);
					}
				}
				else
				{
					cutTris.Add(i);
				}
			}
			if (vertices[0].Count == 0)
			{
				centroid[0] = array2[0];
			}
			else
			{
				centroid[0] /= (float)vertices[0].Count;
			}
			if (vertices[1].Count == 0)
			{
				centroid[1] = array2[1];
			}
			else
			{
				centroid[1] /= (float)vertices[1].Count;
			}
			CutterMesh item = default(CutterMesh);
			item.centroidLocal = centroid[0];
			CutterMesh item2 = default(CutterMesh);
			item2.centroidLocal = centroid[1];
			item.mesh = null;
			item2.mesh = null;
			if (cutTris.Count < 1)
			{
				stopwatch.Stop();
				return stopwatch.ElapsedMilliseconds;
			}
			AllocateContours(cutTris.Count);
			foreach (int cutTri in cutTris)
			{
				Triangle triangle = default(Triangle);
				triangle.ids = new int[3]
				{
					array[cutTri],
					array[cutTri + 1],
					array[cutTri + 2]
				};
				triangle.pos = new Vector2[3]
				{
					array2[array[cutTri]],
					array2[array[cutTri + 1]],
					array2[array[cutTri + 2]]
				};
				Triangle triangle2 = triangle;
				bool side = line2D.GetSide(triangle2.pos[0]);
				bool side2 = line2D.GetSide(triangle2.pos[1]);
				bool side3 = line2D.GetSide(triangle2.pos[2]);
				Vector2 q = Vector2.zero;
				Vector2 q2 = Vector2.zero;
				int num3 = (!side) ? 1 : 0;
				int num4 = 1 - num3;
				float t;
				float t2;
				if (side == side2)
				{
					bool flag = line2D.IntersectSegment(triangle2.pos[2], triangle2.pos[0], out t, ref q);
					bool flag2 = line2D.IntersectSegment(triangle2.pos[2], triangle2.pos[1], out t2, ref q2);
					int item3 = AddIntersectionPoint(q, triangle2.ids[2], triangle2.ids[0], cutVertCache[num3], vertices[num3]);
					int item4 = AddIntersectionPoint(q2, triangle2.ids[2], triangle2.ids[1], cutVertCache[num3], vertices[num3]);
					int item5 = AddTrianglePoint(triangle2.pos[0], triangle2.ids[0], triCache, cornerVertCache[num3], vertices[num3]);
					int item6 = AddTrianglePoint(triangle2.pos[1], triangle2.ids[1], triCache, cornerVertCache[num3], vertices[num3]);
					triangles[num3].Add(item3);
					triangles[num3].Add(item5);
					triangles[num3].Add(item4);
					triangles[num3].Add(item4);
					triangles[num3].Add(item5);
					triangles[num3].Add(item6);
					int item7 = AddIntersectionPoint(q, triangle2.ids[2], triangle2.ids[0], cutVertCache[num4], vertices[num4]);
					int item8 = AddIntersectionPoint(q2, triangle2.ids[2], triangle2.ids[1], cutVertCache[num4], vertices[num4]);
					int item9 = AddTrianglePoint(triangle2.pos[2], triangle2.ids[2], triCache, cornerVertCache[num4], vertices[num4]);
					triangles[num4].Add(item9);
					triangles[num4].Add(item7);
					triangles[num4].Add(item8);
				}
				else if (side == side3)
				{
					bool flag3 = line2D.IntersectSegment(triangle2.pos[1], triangle2.pos[0], out t, ref q2);
					bool flag4 = line2D.IntersectSegment(triangle2.pos[1], triangle2.pos[2], out t2, ref q);
					int item10 = AddIntersectionPoint(q, triangle2.ids[1], triangle2.ids[2], cutVertCache[num3], vertices[num3]);
					int item11 = AddIntersectionPoint(q2, triangle2.ids[1], triangle2.ids[0], cutVertCache[num3], vertices[num3]);
					int item12 = AddTrianglePoint(triangle2.pos[0], triangle2.ids[0], triCache, cornerVertCache[num3], vertices[num3]);
					int item13 = AddTrianglePoint(triangle2.pos[2], triangle2.ids[2], triCache, cornerVertCache[num3], vertices[num3]);
					triangles[num3].Add(item13);
					triangles[num3].Add(item11);
					triangles[num3].Add(item10);
					triangles[num3].Add(item13);
					triangles[num3].Add(item12);
					triangles[num3].Add(item11);
					int item14 = AddIntersectionPoint(q, triangle2.ids[1], triangle2.ids[2], cutVertCache[num4], vertices[num4]);
					int item15 = AddIntersectionPoint(q2, triangle2.ids[1], triangle2.ids[0], cutVertCache[num4], vertices[num4]);
					int item16 = AddTrianglePoint(triangle2.pos[1], triangle2.ids[1], triCache, cornerVertCache[num4], vertices[num4]);
					triangles[num4].Add(item14);
					triangles[num4].Add(item15);
					triangles[num4].Add(item16);
				}
				else
				{
					bool flag5 = line2D.IntersectSegment(triangle2.pos[0], triangle2.pos[1], out t, ref q);
					bool flag6 = line2D.IntersectSegment(triangle2.pos[0], triangle2.pos[2], out t2, ref q2);
					int item17 = AddIntersectionPoint(q, triangle2.ids[0], triangle2.ids[1], cutVertCache[num4], vertices[num4]);
					int item18 = AddIntersectionPoint(q2, triangle2.ids[0], triangle2.ids[2], cutVertCache[num4], vertices[num4]);
					int item19 = AddTrianglePoint(triangle2.pos[1], triangle2.ids[1], triCache, cornerVertCache[num4], vertices[num4]);
					int item20 = AddTrianglePoint(triangle2.pos[2], triangle2.ids[2], triCache, cornerVertCache[num4], vertices[num4]);
					triangles[num4].Add(item20);
					triangles[num4].Add(item18);
					triangles[num4].Add(item19);
					triangles[num4].Add(item18);
					triangles[num4].Add(item17);
					triangles[num4].Add(item19);
					int item21 = AddIntersectionPoint(q, triangle2.ids[0], triangle2.ids[1], cutVertCache[num3], vertices[num3]);
					int item22 = AddIntersectionPoint(q2, triangle2.ids[0], triangle2.ids[2], cutVertCache[num3], vertices[num3]);
					int item23 = AddTrianglePoint(triangle2.pos[0], triangle2.ids[0], triCache, cornerVertCache[num3], vertices[num3]);
					triangles[num3].Add(item22);
					triangles[num3].Add(item23);
					triangles[num3].Add(item21);
				}
			}
			List<int>[] array3 = null;
			if (vertices[0].Count > 3 && vertices[1].Count > 3)
			{
				item.mesh = new SpriteMesh();
				item2.mesh = new SpriteMesh();
				Vector2[] array4 = vertices[0].ToArray();
				Vector2[] array5 = vertices[1].ToArray();
				item.mesh.vertices = array4;
				item2.mesh.vertices = array5;
				if (array3 != null && array3[0].Count > 3)
				{
					triangles[0].AddRange(array3[0]);
					triangles[1].AddRange(array3[1]);
				}
				item.mesh.triangles = triangles[0].ToArray();
				item2.mesh.triangles = triangles[1].ToArray();
				item.centroidLocal = Vector2.zero;
				item2.centroidLocal = Vector2.zero;
				foreach (Vector2 item24 in vertices[0])
				{
					item.centroidLocal += item24;
				}
				item.centroidLocal /= (float)vertices[0].Count;
				foreach (Vector2 item25 in vertices[1])
				{
					item2.centroidLocal += item25;
				}
				item2.centroidLocal /= (float)vertices[1].Count;
				meshes = new List<CutterMesh>
				{
					item,
					item2
				};
				stopwatch.Stop();
				return stopwatch.ElapsedMilliseconds;
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		private int AddIntersectionPoint(Vector2 pos, int edge0, int edge1, Dictionary<long, int> cache, List<Vector2> vertices)
		{
			int num = (edge0 >= edge1) ? ((edge1 << 16) + edge0) : ((edge0 << 16) + edge1);
			if (cache.TryGetValue(num, out int value))
			{
				return value;
			}
			vertices.Add(pos);
			int num2 = vertices.Count - 1;
			cache.Add(num, num2);
			return num2;
		}

		private int AddTrianglePoint(Vector2 pos, int idx, int[] triCache, Dictionary<int, int> cache, List<Vector2> vertices)
		{
			if (triCache[idx] != 0)
			{
				return triCache[idx] - 1;
			}
			if (cache.TryGetValue(idx, out int value))
			{
				return value;
			}
			vertices.Add(pos);
			int num = vertices.Count - 1;
			cache.Add(idx, num);
			return num;
		}
	}
}
