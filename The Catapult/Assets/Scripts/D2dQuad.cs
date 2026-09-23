using System;
using UnityEngine;

namespace Destructible2D
{
	[Serializable]
	public struct D2dQuad
	{
		public D2dVector2 BL;

		public D2dVector2 BR;

		public D2dVector2 TL;

		public D2dVector2 TR;

		public D2dVector2 Size;

		public int Area;

		public void Calculate()
		{
			int num = Mathf.Min(Mathf.Min(BL.X, BR.X), Mathf.Min(TL.X, TR.X));
			int num2 = Mathf.Min(Mathf.Min(BL.Y, BR.Y), Mathf.Min(TL.Y, TR.Y));
			int num3 = Mathf.Max(Mathf.Max(BL.X, BR.X), Mathf.Max(TL.X, TR.X));
			int num4 = Mathf.Max(Mathf.Max(BL.Y, BR.Y), Mathf.Max(TL.Y, TR.Y));
			Size.X = num3 - num;
			Size.Y = num4 - num2;
			Area = Size.X * Size.Y;
		}

		public void Split(ref D2dQuad first, ref D2dQuad second)
		{
			if (Size.X > Size.Y)
			{
				D2dVector2 d2dVector = TL + (TR - TL) / 2;
				D2dVector2 d2dVector2 = BL + (BR - BL) / 2;
				first.BL = BL;
				first.BR = d2dVector2;
				first.TL = TL;
				first.TR = d2dVector;
				second.BL = d2dVector2;
				second.BR = BR;
				second.TL = d2dVector;
				second.TR = TR;
			}
			else
			{
				D2dVector2 d2dVector3 = BL + (TL - BL) / 2;
				D2dVector2 d2dVector4 = BR + (TR - BR) / 2;
				first.BL = d2dVector3;
				first.BR = d2dVector4;
				first.TL = TL;
				first.TR = TR;
				second.BL = BL;
				second.BR = BR;
				second.TL = d2dVector3;
				second.TR = d2dVector4;
			}
			first.Calculate();
			second.Calculate();
		}
	}
}
