using UnityEngine;

namespace Exploder2D.Core
{
	public class SpriteMesh
	{
		public int[] triangles;

		public Vector2[] vertices;

		public Vector2 min;

		public Vector2 max;

		public ushort[] uTriangles
		{
			get
			{
				ushort[] array = new ushort[triangles.Length];
				for (int i = 0; i < triangles.Length; i++)
				{
					array[i] = (ushort)triangles[i];
				}
				return array;
			}
		}

		public SpriteMesh()
		{
		}

		public SpriteMesh(Sprite sprite)
		{
			min.x = float.MaxValue;
			min.y = float.MaxValue;
			max.x = float.MinValue;
			max.y = float.MinValue;
			ushort[] array = sprite.triangles;
			triangles = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				triangles[i] = array[i];
			}
			vertices = sprite.vertices;
			for (int j = 0; j < vertices.Length; j++)
			{
				if (vertices[j].x > max.x)
				{
					max.x = vertices[j].x;
				}
				if (vertices[j].y > max.y)
				{
					max.y = vertices[j].y;
				}
				if (vertices[j].x < min.x)
				{
					min.x = vertices[j].x;
				}
				if (vertices[j].y < min.y)
				{
					min.y = vertices[j].y;
				}
			}
		}

		public Vector2 GetCentroidLocal()
		{
			Vector2 a = Vector2.zero;
			for (int i = 0; i < vertices.Length; i++)
			{
				a += vertices[i];
			}
			return a / vertices.Length;
		}
	}
}
