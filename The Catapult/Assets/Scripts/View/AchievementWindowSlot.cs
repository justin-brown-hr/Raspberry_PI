using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	public class AchievementWindowSlot : MonoBehaviour
	{
		public Text caption;

		public Text descripion;

		public Text reward;

		public Image[] starsList;

		public Image progressBar;

		public Image achievementImage;

		public Image сompletionImage;

		public Sprite achievementUnlocked;

		public Sprite stepAchieved;

		public void InitSlot(AchieventData data)
		{
			caption.text = data.name;
			descripion.text = data.description;
			reward.text = data.reward.ToString();
			achievementImage.sprite = data.image;
			achievementImage.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
			achievementImage.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(140f, 140f);
			progressBar.fillAmount = (float)data.currentProgress / (float)data.targetProgress;
			if (data.isAchieved)
			{
				сompletionImage.sprite = achievementUnlocked;
			}
			for (int i = 0; i < starsList.Length; i++)
			{
				if (i == 0)
				{
					starsList[i].gameObject.SetActive(value: true);
					if (data.isAchieved)
					{
						starsList[i].sprite = stepAchieved;
						starsList[i].color = new Color(1f, 1f, 1f, 1f);
					}
				}
				else
				{
					starsList[i].gameObject.SetActive(value: false);
				}
			}
		}

		public void DeactivateSlot()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
