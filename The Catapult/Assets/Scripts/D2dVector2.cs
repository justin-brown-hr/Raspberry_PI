using System;
using UnityEngine;

namespace Destructible2D
{
	[Serializable]
	public struct D2dVector2
	{
		public int X;

		public int Y;

		public Vector3 V => new Vector3(X, Y, 0f);

		public float Magnitude => (float)Math.Sqrt(X * X + Y * Y);

		public D2dVector2(int newX, int newY)
		{
			X = newX;
			Y = newY;
		}

		public static int DistanceSq(D2dVector2 a, D2dVector2 b)
		{
			int num = b.X - a.X;
			int num2 = b.Y - a.Y;
			return num * num + num2 * num2;
		}

		public static D2dVector2 operator +(D2dVector2 a, D2dVector2 b)
		{
			a.X += b.X;
			a.Y += b.Y;
			return a;
		}

		public static D2dVector2 operator -(D2dVector2 a, D2dVector2 b)
		{
			a.X -= b.X;
			a.Y -= b.Y;
			return a;
		}

		public static D2dVector2 operator *(D2dVector2 a, float b)
		{
			a.X = (int)((float)a.X * b);
			a.Y = (int)((float)a.Y * b);
			return a;
		}

		public static D2dVector2 operator /(D2dVector2 a, int b)
		{
			a.X /= b;
			a.Y /= b;
			return a;
		}

		public override bool Equals(object o)
		{
			if (o is D2dVector2)
			{
				D2dVector2 d2dVector = (D2dVector2)o;
				return X == d2dVector.X && Y == d2dVector.Y;
			}
			return false;
		}

		public static bool operator ==(D2dVector2 a, D2dVector2 b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(D2dVector2 a, D2dVector2 b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public override string ToString()
		{
			return " (" + X + ", " + Y + ") ";
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}
