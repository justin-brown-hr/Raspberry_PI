using System;
using UnityEngine;

namespace Exploder2D.Utils
{
	internal class Hull2D
	{
		public static void SortPoints(Vector2[] array)
		{
			Array.Sort(array, delegate(Vector2 value0, Vector2 value1)
			{
				int num = value0.x.CompareTo(value1.x);
				return (num != 0) ? num : value0.y.CompareTo(value1.y);
			});
		}

		public static void DumpArray(Vector2[] array)
		{
			foreach (Vector2 vector in array)
			{
				UnityEngine.Debug.Log("V: " + vector);
			}
		}

		public static Vector2[] ChainHull2D(Vector2[] Pnts)
		{
			int num = Pnts.Length;
			int num2 = 0;
			SortPoints(Pnts);
			Vector2[] array = new Vector2[2 * num];
			for (int i = 0; i < num; i++)
			{
				while (num2 >= 2 && Hull2DCross(ref array[num2 - 2], ref array[num2 - 1], ref Pnts[i]) <= 0f)
				{
					num2--;
				}
				array[num2++] = Pnts[i];
			}
			int num4 = num - 2;
			int num5 = num2 + 1;
			while (num4 >= 0)
			{
				while (num2 >= num5 && Hull2DCross(ref array[num2 - 2], ref array[num2 - 1], ref Pnts[num4]) <= 0f)
				{
					num2--;
				}
				array[num2++] = Pnts[num4];
				num4--;
			}
			Vector2[] array2 = new Vector2[num2];
			for (int j = 0; j < num2; j++)
			{
				array2[j] = array[j];
			}
			return array2;
		}

		private static float Hull2DCross(ref Vector2 O, ref Vector2 A, ref Vector2 B)
		{
			return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
		}
	}
}
