using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Destructible2D
{
	[Serializable]
	public class D2dDestructibleListEvent : UnityEvent<List<D2dDestructible>>
	{
	}
}
