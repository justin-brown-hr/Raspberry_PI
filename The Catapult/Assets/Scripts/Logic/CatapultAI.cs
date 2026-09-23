using System;
using UnityEngine;

namespace Logic
{
	internal class CatapultAI : MonoBehaviour
	{
		public int catapultType;

		internal int catapultLevel;

		internal bool levelInitialized;

		private float MIN_SHOOT_TIME = 1f;

		private float MAX_SHOOT_TIME = 4f;

		private float maxPower;

		private float minPower;

		internal float enemyBonusChance = 0.25f;

		private void Awake()
		{
			maxPower = 12f;
			minPower = 1f;
			levelInitialized = false;
			catapultLevel = 0;
		}

		public void Init(int minLevel, int maxLevel)
		{
			switch (catapultType)
			{
			case 0:
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				catapultLevel = UnityEngine.Random.Range(minLevel, maxLevel);
				break;
			case 1:
				if (minLevel == maxLevel)
				{
					catapultLevel = 0;
					break;
				}
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				catapultLevel = UnityEngine.Random.Range(0, 6);
				break;
			case 2:
				if (minLevel == maxLevel)
				{
					catapultLevel = 0;
					break;
				}
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				catapultLevel = UnityEngine.Random.Range(0, 6);
				break;
			case 3:
				if (minLevel == maxLevel)
				{
					catapultLevel = 0;
					break;
				}
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				catapultLevel = UnityEngine.Random.Range(1, 5);
				break;
			case 4:
				if (minLevel == maxLevel)
				{
					catapultLevel = 0;
					break;
				}
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				catapultLevel = UnityEngine.Random.Range(1, 5);
				break;
			case 5:
				if (minLevel == maxLevel)
				{
					catapultLevel = 0;
					break;
				}
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				catapultLevel = UnityEngine.Random.Range(1, 5);
				break;
			}
			levelInitialized = true;
		}

		public float GetShootTime()
		{
			float num = 0f;
			return UnityEngine.Random.Range(MIN_SHOOT_TIME, MAX_SHOOT_TIME);
		}

		public float GetShootDistance()
		{
			float num = 0f;
			return UnityEngine.Random.Range(minPower, maxPower);
		}
	}
}
