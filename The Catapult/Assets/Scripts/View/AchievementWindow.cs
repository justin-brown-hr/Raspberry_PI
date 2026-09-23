using UnityEngine;

namespace View
{
	public class AchievementWindow : MonoBehaviour
	{
		public AchievementWindowSlot[] achievementSlots;

		public void LoadAchievementData()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
