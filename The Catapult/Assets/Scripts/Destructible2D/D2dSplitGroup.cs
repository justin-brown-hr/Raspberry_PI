using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	public class D2dSplitGroup
	{
		public static List<D2dSplitGroup> SplitGroups = new List<D2dSplitGroup>();

		public List<D2dSplitPixel> Pixels = new List<D2dSplitPixel>();

		public byte[] Data;

		public D2dRect Rect;

		public static D2dSplitGroup GetSplitGroup()
		{
			D2dSplitGroup d2dSplitGroup = D2dPool<D2dSplitGroup>.Spawn() ?? new D2dSplitGroup();
			SplitGroups.Add(d2dSplitGroup);
			return d2dSplitGroup;
		}

		public static void ClearAll()
		{
			for (int num = SplitGroups.Count - 1; num >= 0; num--)
			{
				D2dSplitGroup d2dSplitGroup = SplitGroups[num];
				d2dSplitGroup.Clear();
				D2dPool<D2dSplitGroup>.Despawn(d2dSplitGroup);
			}
			SplitGroups.Clear();
		}

		public void GenerateData()
		{
			int sizeX = Rect.SizeX;
			int minX = Rect.MinX;
			int minY = Rect.MinY;
			Data = new byte[Rect.Area];
			for (int num = Pixels.Count - 1; num >= 0; num--)
			{
				D2dSplitPixel d2dSplitPixel = Pixels[num];
				int num2 = d2dSplitPixel.X - minX;
				int num3 = d2dSplitPixel.Y - minY;
				Data[num2 + num3 * sizeX] = d2dSplitPixel.Alpha;
			}
		}

		public void CombineData(byte[] prevData, int prevWidth, int prevHeight)
		{
			int minX = Rect.MinX;
			int minY = Rect.MinY;
			int sizeX = Rect.SizeX;
			int sizeY = Rect.SizeY;
			if (Data == null || Data.Length < sizeX * sizeY || prevData == null || prevData.Length < prevWidth * prevHeight)
			{
				return;
			}
			for (int i = 0; i < sizeY; i++)
			{
				for (int j = 0; j < sizeX; j++)
				{
					int num = j + minX;
					int num2 = i + minY;
					int num3 = j + i * sizeX;
					if (num >= 0 && num2 >= 0 && num < prevWidth && num2 < prevHeight)
					{
						int num4 = num + num2 * prevWidth;
						float num5 = D2dHelper.ConvertAlpha(Data[num3]);
						float num6 = D2dHelper.ConvertAlpha(prevData[num4]);
						Data[num3] = D2dHelper.ConvertAlpha(num5 * num6);
					}
					else
					{
						Data[num3] = 0;
					}
				}
			}
		}

		public void AddPixel(int x, int y)
		{
			D2dSplitPixel d2dSplitPixel = D2dPool<D2dSplitPixel>.Spawn() ?? new D2dSplitPixel();
			d2dSplitPixel.Alpha = byte.MaxValue;
			d2dSplitPixel.X = x;
			d2dSplitPixel.Y = y;
			Rect.Add(x, y);
			Pixels.Add(d2dSplitPixel);
		}

		public void AddIsland(D2dFloodfill.Island island)
		{
			for (int num = island.Pixels.Count - 1; num >= 0; num--)
			{
				D2dFloodfillPixel d2dFloodfillPixel = island.Pixels[num];
				AddPixel(d2dFloodfillPixel.X, d2dFloodfillPixel.Y);
			}
		}

		public void AddTriangle(D2dVector2 a, D2dVector2 b, D2dVector2 c)
		{
			if (a.Y != b.Y || a.Y != c.Y)
			{
				if (b.Y > a.Y)
				{
					D2dHelper.Swap(ref a, ref b);
				}
				if (c.Y > a.Y)
				{
					D2dHelper.Swap(ref c, ref a);
				}
				if (c.Y > b.Y)
				{
					D2dHelper.Swap(ref b, ref c);
				}
				int num = a.Y - c.Y;
				int num2 = a.Y - b.Y;
				int num3 = b.Y - c.Y;
				float num4 = (float)c.X + (float)(a.X - c.X) * D2dHelper.Divide(num3, num);
				D2dVector2 d2dVector = new D2dVector2((int)num4, b.Y);
				float ls = D2dHelper.Divide(a.X - b.X, num2);
				float rs = D2dHelper.Divide(a.X - d2dVector.X, num2);
				AddTriangle(b.X, d2dVector.X, ls, rs, b.Y, 1, num2);
				float ls2 = D2dHelper.Divide(c.X - b.X, num3);
				float rs2 = D2dHelper.Divide(c.X - d2dVector.X, num3);
				AddTriangle(b.X, d2dVector.X, ls2, rs2, b.Y, -1, num3);
			}
		}

		public void AddTriangle(float l, float r, float ls, float rs, int y, int s, int c)
		{
			if (l > r)
			{
				D2dHelper.Swap(ref l, ref r);
				D2dHelper.Swap(ref ls, ref rs);
			}
			for (int i = 0; i < c; i++)
			{
				int num = Mathf.FloorToInt(l);
				int num2 = Mathf.CeilToInt(r);
				for (int j = num; j < num2; j++)
				{
					AddPixel(j, y);
				}
				y += s;
				l += ls;
				r += rs;
			}
		}

		public void Clear()
		{
			for (int num = Pixels.Count - 1; num >= 0; num--)
			{
				D2dPool<D2dSplitPixel>.Despawn(Pixels[num]);
			}
			Data = null;
			Rect.Clear();
			Pixels.Clear();
		}
	}
}
