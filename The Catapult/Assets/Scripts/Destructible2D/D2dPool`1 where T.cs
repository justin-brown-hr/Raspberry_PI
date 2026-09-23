using System;
using System.Collections.Generic;

namespace Destructible2D
{
	public static class D2dPool<T> where T : class
	{
		private static List<T> cache = new List<T>();

		public static T Spawn()
		{
			return Spawn(null, null);
		}

		public static T Spawn(Action<T> onSpawn)
		{
			return Spawn(null, onSpawn);
		}

		public static T Spawn(Predicate<T> match)
		{
			return Spawn(match, null);
		}

		public static T Spawn(Predicate<T> match, Action<T> onSpawn)
		{
			int num = (match == null) ? (cache.Count - 1) : cache.FindIndex(match);
			if (num >= 0)
			{
				T val = cache[num];
				cache.RemoveAt(num);
				onSpawn?.Invoke(val);
				return val;
			}
			return (T)null;
		}

		public static void Despawn(T instance)
		{
			Despawn(instance, null);
		}

		public static void Despawn(T instance, Action<T> onDespawn)
		{
			if (instance != null)
			{
				onDespawn?.Invoke(instance);
				cache.Add(instance);
			}
		}
	}
}
