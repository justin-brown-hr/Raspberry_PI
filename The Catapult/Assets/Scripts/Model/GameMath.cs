using Logic;
using UnityEngine;

namespace Model
{
	internal class GameMath
	{
		internal const float MAX_DISTANCE = 12f;

		internal const float MIN_DISTANCE = 1f;

		private const float MIN_TIME = 0.2f;

		private const float MAX_TIME = 0.8f;

		private const float MIN_POWER = 5f;

		private const float MAX_POWER = 25f;

		public static float DistanceToAnimation(float distance)
		{
			float num = 0f;
			if (distance < 1f)
			{
				return 0.8f;
			}
			if (distance > 12f)
			{
				return 0.2f;
			}
			float num2 = distance / 12f * 100f;
			num = 0.006f * num2;
			return 0.8f - num;
		}

		public static Vector2 GetShootVector(GameSides side, float aimDistance, bool isAdditional = false, int aiType = -1)
		{
			int currentCatapultIndex = NewDataController.instance.GetCurrentCatapultIndex();
			Vector2 a;
			switch (side)
			{
			case GameSides.Player1:
				if (NewDataController.instance.GetGameMode() == GameMode.Single)
				{
					switch (currentCatapultIndex)
					{
					case 1:
						a = new Vector2(16f, 16f);
						break;
					case 2:
						a = (isAdditional ? new Vector2(16f, 14f) : new Vector2(16f, 16f));
						break;
					default:
						a = new Vector2(20f, 12.8f);
						break;
					}
				}
				else
				{
					a = new Vector2(20f, 12.8f);
				}
				break;
			case GameSides.Player2:
				a = new Vector2(-20f, 12.8f);
				break;
			case GameSides.AI:
				a = ((aiType != 3 && aiType != 4 && aiType != 5) ? new Vector2(-14f, 10f) : new Vector2(-21f, 15f));
				break;
			default:
				a = new Vector2(-14f, 10f);
				break;
			}
			float d = (aimDistance <= 1f) ? 5f : ((!(aimDistance > 12f)) ? (5f + (aimDistance - 1f) * 1.82f) : 25f);
			return a * d;
		}

		public static Vector2 GetBossShootVector(float aimDistance)
		{
			Vector2 zero = Vector2.zero;
			float num = 0f;
			zero = new Vector2(-20f, 12.8f);
			num = ((aimDistance <= 1f) ? 5f : ((!(aimDistance > 12f)) ? (5f + (aimDistance - 1f) * 1.82f) : 25f));
			return zero * num;
		}

		public static float CalculateAngle(float distance)
		{
			float result = 0f;
			switch (NewDataController.instance.GetCurrentCatapultIndex())
			{
			case 0:
				result = (75f - distance * 4f) * -1f;
				break;
			case 1:
				result = (75f - distance * 6f) * -1f;
				break;
			case 2:
				result = (75f - distance * 4f) * -1f;
				break;
			case 3:
				result = (75f - distance * 6f) * -1f;
				break;
			case 4:
				result = (75f - distance * 6f) * -1f;
				break;
			case 5:
				result = (75f - distance * 4f) * -1f;
				break;
			}
			return result;
		}

		public static float CalculateAIAngle(float distance, int type)
		{
			float result = 0f;
			switch (type)
			{
			case 0:
				result = 70f - distance * 4f;
				break;
			case 1:
				result = (15f + distance * 4f) * -1f;
				break;
			case 2:
				result = (5f + distance * 4f) * -1f;
				break;
			case 3:
				result = 70f - distance * 4f;
				break;
			case 4:
				result = (15f + distance * 4f) * -1f;
				break;
			case 5:
				result = 70f - distance * 4f;
				break;
			}
			return result;
		}
	}
}
