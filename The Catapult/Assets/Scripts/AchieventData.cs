using System;
using UnityEngine;

namespace Logic
{
	[Serializable]
	public struct AchieventData
	{
		public AchieventType type;

		public AchievementRegion region;

		[Header("Global info")]
		public string name;

		public string description;

		public Sprite image;

		[Header("Ingame part")]
		public int currentProgress;

		public int targetProgress;

		public int reward;

		public bool isAchieved;

		[Header("Increment data")]
		public bool isIncrement;

		[Header("Repeatable achievement data")]
		public bool isRepeatable;

		public int currentRepeatCount;

		public int maxRepeatCount;

		[Header("Play Games Section")]
		public string achievementID;
	}
}
