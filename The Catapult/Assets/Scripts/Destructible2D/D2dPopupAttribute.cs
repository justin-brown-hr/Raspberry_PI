using System.Linq;
using UnityEngine;

namespace Destructible2D
{
	public class D2dPopupAttribute : PropertyAttribute
	{
		public GUIContent[] Names;

		public int[] Values;

		public D2dPopupAttribute(params int[] newValues)
		{
			Names = (from v in newValues
				select new GUIContent(v.ToString())).ToArray();
			Values = (from v in newValues
				select (v)).ToArray();
		}
	}
}
