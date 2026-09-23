using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Quad Fracturer")]
	public class D2dQuadFracturer : D2dFracturer
	{
		[Tooltip("This changes how random the fractured shapes will be")]
		[Range(0f, 0.5f)]
		public float Irregularity = 0.25f;

		private static List<D2dQuad> quads = new List<D2dQuad>();

		private static List<D2dVector2> points = new List<D2dVector2>();

		private static int quadCount;

		private static int pointCount;

		private static int xMin;

		private static int xMax;

		private static int yMin;

		private static int yMax;

		[ContextMenu("Fracture")]
		public override void Fracture()
		{
			base.Fracture();
			Fracture(destructible, FractureCount, Irregularity);
		}

		public static void Fracture(D2dDestructible destructible, int count, float irregularity)
		{
			if (destructible != null && count > 0)
			{
				D2dSplitGroup.ClearAll();
				int alphaWidth = destructible.AlphaWidth;
				int alphaHeight = destructible.AlphaHeight;
				D2dQuad d2dQuad = default(D2dQuad);
				quadCount = 1;
				pointCount = 0;
				xMin = 0;
				xMax = alphaWidth - 1;
				yMin = 0;
				yMax = alphaHeight - 1;
				d2dQuad.BL = new D2dVector2(xMin, yMin);
				d2dQuad.BR = new D2dVector2(xMax, yMin);
				d2dQuad.TL = new D2dVector2(xMin, yMax);
				d2dQuad.TR = new D2dVector2(xMax, yMax);
				d2dQuad.Calculate();
				if (quads.Count > 0)
				{
					quads[0] = d2dQuad;
				}
				else
				{
					quads.Add(d2dQuad);
				}
				for (int i = 0; i < count; i++)
				{
					SplitLargest();
				}
				if (irregularity > 0f)
				{
					FindPoints();
					ShiftPoints(irregularity);
				}
				for (int j = 0; j < quadCount; j++)
				{
					D2dQuad d2dQuad2 = quads[j];
					D2dSplitGroup splitGroup = D2dSplitGroup.GetSplitGroup();
					splitGroup.AddTriangle(d2dQuad2.BL, d2dQuad2.BR, d2dQuad2.TL);
					splitGroup.AddTriangle(d2dQuad2.TR, d2dQuad2.TL, d2dQuad2.BR);
				}
				destructible.Split(null, D2dSplitGroup.SplitGroups);
				D2dSplitGroup.ClearAll();
			}
		}

		private static void FindPoints()
		{
			for (int i = 0; i < quadCount; i++)
			{
				D2dQuad d2dQuad = quads[i];
				TryAddPoint(d2dQuad.BL);
				TryAddPoint(d2dQuad.BR);
				TryAddPoint(d2dQuad.TL);
				TryAddPoint(d2dQuad.TR);
			}
		}

		private static void ShiftPoints(float irregularity)
		{
			for (int i = 0; i < pointCount; i++)
			{
				D2dVector2 oldPoint = points[i];
				Vector2 vector = Random.insideUnitCircle.normalized * FindMaxMovement(oldPoint.X, oldPoint.Y) * irregularity;
				int num = Mathf.RoundToInt(vector.x);
				int num2 = Mathf.RoundToInt(vector.y);
				if (oldPoint.X <= xMin || oldPoint.X >= xMax)
				{
					num = 0;
				}
				if (oldPoint.Y <= yMin || oldPoint.Y >= yMax)
				{
					num2 = 0;
				}
				if (num != 0 || num2 != 0)
				{
					D2dVector2 d2dVector = new D2dVector2(oldPoint.X + num, oldPoint.Y + num2);
					MovePoints(oldPoint, d2dVector);
					points[i] = d2dVector;
				}
			}
		}

		private static void MovePoints(D2dVector2 oldPoint, D2dVector2 newPoint)
		{
			for (int i = 0; i < quadCount; i++)
			{
				D2dQuad value = quads[i];
				TryMovePoint(ref value.BL, oldPoint, newPoint);
				TryMovePoint(ref value.BR, oldPoint, newPoint);
				TryMovePoint(ref value.TL, oldPoint, newPoint);
				TryMovePoint(ref value.TR, oldPoint, newPoint);
				quads[i] = value;
			}
		}

		private static void TryMovePoint(ref D2dVector2 point, D2dVector2 oldPoint, D2dVector2 newPoint)
		{
			if (point.X == oldPoint.X && point.Y == oldPoint.Y)
			{
				point.X = newPoint.X;
				point.Y = newPoint.Y;
			}
		}

		private static void TryAddPoint(D2dVector2 newPoint)
		{
			for (int i = 0; i < pointCount; i++)
			{
				D2dVector2 d2dVector = points[i];
				if (d2dVector.X == newPoint.X && d2dVector.Y == newPoint.Y)
				{
					return;
				}
			}
			if (points.Count > pointCount)
			{
				points[pointCount] = newPoint;
			}
			else
			{
				points.Add(newPoint);
			}
			pointCount++;
		}

		private static int FindMaxMovement(int x, int y)
		{
			int num = int.MaxValue;
			for (int i = 0; i < pointCount; i++)
			{
				D2dVector2 d2dVector = points[i];
				int num2 = d2dVector.X - x;
				int num3 = d2dVector.Y - y;
				int num4 = num2 * num2 + num3 * num3;
				if (num4 > 0)
				{
					num = Mathf.Min(num, num4);
				}
			}
			return Mathf.FloorToInt(Mathf.Sqrt(num));
		}

		private static void SplitLargest()
		{
			int index = 0;
			int num = 0;
			for (int i = 0; i < quadCount; i++)
			{
				D2dQuad d2dQuad = quads[i];
				if (d2dQuad.Area > num)
				{
					index = i;
					num = d2dQuad.Area;
				}
			}
			D2dQuad first = default(D2dQuad);
			D2dQuad second = default(D2dQuad);
			quads[index].Split(ref first, ref second);
			quads[index] = first;
			if (quads.Count > quadCount)
			{
				quads[quadCount] = second;
			}
			else
			{
				quads.Add(second);
			}
			quadCount++;
		}
	}
}
