using System;
using UnityEngine;
using UnityEngine.Events;

namespace Destructible2D
{
	[Serializable]
	public class D2dCollision2DEvent : UnityEvent<Collision2D>
	{
	}
}
