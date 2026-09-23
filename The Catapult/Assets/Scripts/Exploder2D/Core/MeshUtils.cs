using Exploder2D.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Exploder2D.Core
{
	public static class MeshUtils
	{
		public static Vector3 ComputeBarycentricCoordinates(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
		{
			float num = b.x - a.x;
			float num2 = b.y - a.y;
			float num3 = b.z - a.z;
			float num4 = c.x - a.x;
			float num5 = c.y - a.y;
			float num6 = c.z - a.z;
			float num7 = p.x - a.x;
			float num8 = p.y - a.y;
			float num9 = p.z - a.z;
			float num10 = num * num + num2 * num2 + num3 * num3;
			float num11 = num * num4 + num2 * num5 + num3 * num6;
			float num12 = num4 * num4 + num5 * num5 + num6 * num6;
			float num13 = num7 * num + num8 * num2 + num9 * num3;
			float num14 = num7 * num4 + num8 * num5 + num9 * num6;
			float num15 = num10 * num12 - num11 * num11;
			float num16 = (num12 * num13 - num11 * num14) / num15;
			float num17 = (num10 * num14 - num11 * num13) / num15;
			float x = 1f - num16 - num17;
			return new Vector3(x, num16, num17);
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T val = a;
			a = b;
			b = val;
		}

		public static void CenterPivot(Vector3[] vertices, Vector3 centroid)
		{
			int num = vertices.Length;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = vertices[i];
				vector.x -= centroid.x;
				vector.y -= centroid.y;
				vector.z -= centroid.z;
				vertices[i] = vector;
			}
		}

		public static List<CutterMesh> IsolateMeshIslands(SpriteMesh mesh)
		{
			int[] triangles = mesh.triangles;
			int allocSize = mesh.vertices.Length;
			int num = mesh.triangles.Length;
			Vector2[] vertices = mesh.vertices;
			if (num <= 3)
			{
				return null;
			}
			LSHash lSHash = new LSHash(0.1f, allocSize);
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = lSHash.Hash(vertices[triangles[i]]);
			}
			List<HashSet<int>> list = new List<HashSet<int>>();
			list.Add(new HashSet<int>
			{
				array[0],
				array[1],
				array[2]
			});
			List<HashSet<int>> list2 = list;
			List<List<int>> list3 = new List<List<int>>();
			list3.Add(new List<int>(num)
			{
				0,
				1,
				2
			});
			List<List<int>> list4 = list3;
			bool[] array2 = new bool[num];
			array2[0] = true;
			array2[1] = true;
			array2[2] = true;
			HashSet<int> hashSet = list2[0];
			List<int> list5 = list4[0];
			int num2 = 3;
			int num3 = -1;
			int num4 = 0;
			do
			{
				bool flag = false;
				for (int j = 3; j < num; j += 3)
				{
					if (!array2[j])
					{
						if (hashSet.Contains(array[j]) || hashSet.Contains(array[j + 1]) || hashSet.Contains(array[j + 2]))
						{
							hashSet.Add(array[j]);
							hashSet.Add(array[j + 1]);
							hashSet.Add(array[j + 2]);
							list5.Add(j);
							list5.Add(j + 1);
							list5.Add(j + 2);
							array2[j] = true;
							array2[j + 1] = true;
							array2[j + 2] = true;
							num2 += 3;
							flag = true;
						}
						else
						{
							num3 = j;
						}
					}
				}
				if (num2 == num)
				{
					break;
				}
				if (!flag)
				{
					HashSet<int> hashSet2 = new HashSet<int>();
					hashSet2.Add(array[num3]);
					hashSet2.Add(array[num3 + 1]);
					hashSet2.Add(array[num3 + 2]);
					hashSet = hashSet2;
					List<int> list6 = new List<int>(num / 2);
					list6.Add(num3);
					list6.Add(num3 + 1);
					list6.Add(num3 + 2);
					list5 = list6;
					list2.Add(hashSet);
					list4.Add(list5);
				}
				num4++;
			}
			while (num4 <= 100);
			int count = list2.Count;
			if (count == 1)
			{
				return null;
			}
			List<CutterMesh> list7 = new List<CutterMesh>(list2.Count);
			foreach (List<int> item2 in list4)
			{
				CutterMesh cutterMesh = default(CutterMesh);
				cutterMesh.mesh = new SpriteMesh();
				CutterMesh item = cutterMesh;
				int count2 = item2.Count;
				SpriteMesh mesh2 = item.mesh;
				List<int> list8 = new List<int>(count2);
				List<Vector2> list9 = new List<Vector2>(count2);
				Dictionary<int, int> dictionary = new Dictionary<int, int>(num);
				Vector2 a = Vector2.zero;
				int num5 = 0;
				int num6 = 0;
				foreach (int item3 in item2)
				{
					int num7 = triangles[item3];
					int value = 0;
					if (dictionary.TryGetValue(num7, out value))
					{
						list8.Add(value);
					}
					else
					{
						list8.Add(num6);
						dictionary.Add(num7, num6);
						num6++;
						a += vertices[num7];
						num5++;
						list9.Add(vertices[num7]);
					}
				}
				mesh2.vertices = list9.ToArray();
				mesh2.triangles = list8.ToArray();
				item.centroidLocal = a / num5;
				list7.Add(item);
			}
			return list7;
		}

		public static void GeneratePolygonCollider(PolygonCollider2D collider, SpriteMesh mesh)
		{
			if (mesh != null && (bool)collider)
			{
				Vector2[] vertices = mesh.vertices;
				Vector2[] array = new Vector2[vertices.Length];
				for (int i = 0; i < vertices.Length; i++)
				{
					array[i] = vertices[i];
				}
				Vector2[] points = Hull2D.ChainHull2D(array);
				collider.SetPath(0, points);
			}
		}

		public static float Signed2DTriArea(Vector2 a, Vector2 b, Vector2 c)
		{
			return (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
		}

		public static bool Test2DSegmentSegment(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 p)
		{
			float num = Signed2DTriArea(a, b, d);
			float num2 = Signed2DTriArea(a, b, c);
			if (num * num2 < 0f)
			{
				float num3 = Signed2DTriArea(c, d, a);
				float num4 = num3 + num2 - num;
				if (num3 * num4 < 0f)
				{
					float d2 = num3 / (num3 - num4);
					p = a + d2 * (b - a);
					return true;
				}
			}
			p = Vector2.zero;
			return false;
		}
	}
}
