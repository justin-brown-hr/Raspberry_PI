using System;
using UnityEngine;

namespace Destructible2D
{
	[Serializable]
	public class D2dSnapshot
	{
		public Rect AlphaRect;

		public byte[] AlphaData;

		public int AlphaWidth;

		public int AlphaHeight;
	}
}
