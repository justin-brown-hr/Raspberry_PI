using System;
using UnityEngine;

namespace Exploder2D.Core.Math
{
	public class Line2D
	{
		[Flags]
		public enum PointClass
		{
			Coplanar = 0x0,
			Front = 0x1,
			Back = 0x2,
			Intersection = 0x3
		}

		private const float epsylon = 0.0001f;

		public Vector2 Normal;

		public float Distance;

		public Vector2 Pnt
		{
			get;
			private set;
		}

		public Line2D()
		{
		}

		public Line2D(Line2D instance)
		{
			Normal = instance.Normal;
			Distance = instance.Distance;
			Pnt = instance.Pnt;
		}

		public static Line2D CratePointPoint(Vector2 a, Vector2 b)
		{
			Line2D line2D = new Line2D();
			line2D.Normal = (b - a).normalized;
			Line2D line2D2 = line2D;
			line2D2.Distance = Vector2.Dot(line2D2.Normal, a);
			line2D2.Pnt = a;
			return line2D2;
		}

		public static Line2D CreateNormalPoint(Vector2 normal, Vector2 p)
		{
			Line2D line2D = new Line2D();
			line2D.Normal = normal.normalized;
			Line2D line2D2 = line2D;
			line2D2.Distance = Vector2.Dot(line2D2.Normal, p);
			line2D2.Pnt = p;
			return line2D2;
		}

		public PointClass ClassifyPoint(Vector2 p)
		{
			float num = Vector2.Dot(p, Normal) - Distance;
			return (num < -0.0001f) ? PointClass.Back : ((num > 0.0001f) ? PointClass.Front : PointClass.Coplanar);
		}

		public bool GetSide(Vector2 n)
		{
			return Vector2.Dot(n, Normal) - Distance > 0.0001f;
		}

		public void Flip()
		{
			Normal = -Normal;
			Distance = 0f - Distance;
		}

		public bool GetSideFix(ref Vector2 n)
		{
			float num = n.x * Normal.x + n.y * Normal.y - Distance;
			float num2 = 1f;
			float num3 = num;
			if (num < 0f)
			{
				num2 = -1f;
				num3 = 0f - num;
			}
			if (num3 < 0.0011f)
			{
				n.x += Normal.x * 0.001f * num2;
				n.y += Normal.y * 0.001f * num2;
				num = n.x * Normal.x + n.y * Normal.y - Distance;
			}
			return num > 0.0001f;
		}

		public bool SameSide(Vector2 a, Vector2 b)
		{
			throw new NotImplementedException();
		}

		public bool IntersectSegment(Vector2 a, Vector2 b, out float t, ref Vector2 q)
		{
			float num = b.x - a.x;
			float num2 = b.y - a.y;
			float num3 = Normal.x * a.x + Normal.y * a.y;
			float num4 = Normal.x * num + Normal.y * num2;
			t = (Distance - num3) / num4;
			if (t >= -0.0001f && t <= 1.0001f)
			{
				q.x = a.x + t * num;
				q.y = a.y + t * num2;
				return true;
			}
			q = Vector2.zero;
			return false;
		}

		public void InverseTransform(Transform transform)
		{
			Vector3 v = transform.InverseTransformDirection(Normal);
			Vector3 v2 = transform.InverseTransformPoint(Pnt);
			Normal = v;
			Distance = Vector2.Dot(v, v2);
		}
	}
}
