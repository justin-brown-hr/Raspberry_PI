using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	public static class D2dFloodfill
	{
		private class Line
		{
			public int y;

			public int min;

			public int max;

			public bool used;

			public List<Line> ups = new List<Line>();

			public List<Line> dns = new List<Line>();
		}

		public class Island
		{
			public List<D2dFloodfillPixel> Pixels = new List<D2dFloodfillPixel>();

			public int MinX;

			public int MinY;

			public int MaxX;

			public int MaxY;

			public void AddPixel(int x, int y)
			{
				D2dFloodfillPixel item = default(D2dFloodfillPixel);
				item.X = x;
				item.Y = y;
				Pixels.Add(item);
			}

			public void Clear()
			{
				Pixels.Clear();
			}

			public D2dSplitGroup CreateSplitGroup(bool feather, bool allowExpand)
			{
				if (feather)
				{
				}
				D2dSplitGroup splitGroup = D2dSplitGroup.GetSplitGroup();
				for (int num = Pixels.Count - 1; num >= 0; num--)
				{
					D2dFloodfillPixel d2dFloodfillPixel = Pixels[num];
					splitGroup.AddPixel(d2dFloodfillPixel.X, d2dFloodfillPixel.Y);
				}
				if (allowExpand && !feather)
				{
					splitGroup.Rect.MinX--;
					splitGroup.Rect.MaxX++;
					splitGroup.Rect.MinY--;
					splitGroup.Rect.MaxY++;
				}
				return splitGroup;
			}
		}

		public static List<Island> Islands = new List<Island>();

		public static List<Island> BorderIslands = new List<Island>();

		private static List<Line> lines = new List<Line>();

		private static List<Line> scan = new List<Line>();

		private static int lineCount;

		public static Island GetIsland()
		{
			Island island = D2dPool<Island>.Spawn() ?? new Island();
			Islands.Add(island);
			return island;
		}

		public static void Clear()
		{
			for (int num = Islands.Count - 1; num >= 0; num--)
			{
				Island island = Islands[num];
				island.Clear();
				D2dPool<Island>.Despawn(island);
			}
			for (int num2 = BorderIslands.Count - 1; num2 >= 0; num2--)
			{
				Island island2 = BorderIslands[num2];
				island2.Clear();
				D2dPool<Island>.Despawn(island2);
			}
			Islands.Clear();
			BorderIslands.Clear();
		}

		public static void FastFind(byte[] alphaData, int alphaWidth, int alphaHeight)
		{
			FastFind(alphaData, alphaWidth, alphaHeight, new D2dRect(0, alphaWidth, 0, alphaHeight));
		}

		public static void FastFindLocal(byte[] alphaData, int alphaWidth, int alphaHeight, D2dRect rect)
		{
			FastFind(alphaData, alphaWidth, alphaHeight, rect);
			for (int num = Islands.Count - 1; num >= 0; num--)
			{
				Island island = Islands[num];
				if (island.MinX == rect.MinX || island.MaxX == rect.MaxX || island.MinY == rect.MinY || island.MaxY == rect.MaxY)
				{
					Islands.RemoveAt(num);
					BorderIslands.Add(island);
				}
			}
		}

		private static void FastFind(byte[] alphaData, int alphaWidth, int alphaHeight, D2dRect rect)
		{
			Clear();
			lineCount = 0;
			int num = 0;
			for (int i = rect.MinY; i < rect.MaxY; i++)
			{
				int num2 = FastFindLines(alphaData, alphaWidth, i, rect.MinX, rect.MaxX);
				FastLinkLines(lineCount - num2 - num, lineCount - num2, lineCount);
				num = num2;
			}
			for (int j = 0; j < lineCount; j++)
			{
				Line line = lines[j];
				if (line.used)
				{
					continue;
				}
				Island island = GetIsland();
				island.MinX = line.min;
				island.MaxX = line.max;
				island.MinY = line.y;
				island.MaxY = line.y + 1;
				scan.Clear();
				scan.Add(line);
				line.used = true;
				for (int k = 0; k < scan.Count; k++)
				{
					Line line2 = scan[k];
					island.MinX = Mathf.Min(island.MinX, line.min);
					island.MaxX = Mathf.Max(island.MaxX, line.max);
					island.MinY = Mathf.Min(island.MinY, line.y);
					island.MaxY = Mathf.Max(island.MaxY, line.y + 1);
					AddToScan(line2.ups);
					AddToScan(line2.dns);
					for (int l = line2.min; l < line2.max; l++)
					{
						island.AddPixel(l, line2.y);
					}
				}
			}
		}

		private static void AddToScan(List<Line> lines)
		{
			for (int num = lines.Count - 1; num >= 0; num--)
			{
				Line line = lines[num];
				if (!line.used)
				{
					scan.Add(line);
					line.used = true;
				}
			}
		}

		private static void FastLinkLines(int min, int mid, int max)
		{
			for (int i = min; i < mid; i++)
			{
				Line line = lines[i];
				for (int j = mid; j < max; j++)
				{
					Line line2 = lines[j];
					if (line2.min < line.max && line2.max > line.min)
					{
						line.ups.Add(line2);
						line2.dns.Add(line);
					}
				}
				line.min--;
				line.max++;
			}
		}

		private static int FastFindLines(byte[] alphaData, int alphaWidth, int y, int minX, int maxX)
		{
			Line line = null;
			int num = 0;
			int num2 = alphaWidth * y;
			for (int i = minX; i < maxX; i++)
			{
				if (alphaData[num2 + i] > 127)
				{
					if (line == null)
					{
						line = GetLine();
						num++;
						line.min = (line.max = i);
						line.y = y;
					}
					line.max++;
				}
				else if (line != null)
				{
					line = null;
				}
			}
			return num;
		}

		private static Line GetLine()
		{
			Line line = null;
			if (lineCount >= lines.Count)
			{
				line = new Line();
				lines.Add(line);
			}
			else
			{
				line = lines[lineCount];
				line.used = false;
				line.ups.Clear();
				line.dns.Clear();
			}
			lineCount++;
			return line;
		}
	}
}
