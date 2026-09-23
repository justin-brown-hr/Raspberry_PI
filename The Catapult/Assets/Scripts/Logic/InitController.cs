using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
	public class InitController : MonoBehaviour
	{
		public static InitController instance;

		private UpgradeShopData upgradesData;

		public bool guiPressed;

		public float guiPressedFingerId;

		public int lastOpenedShopWindow;

		private int qar;

		private int qar_k;

		private int b_qar;

		private int b_qar_k;

		public Image logoImage;

		public int LaunchCount
		{
			get;
			private set;
		}

		public bool IsAppRated
		{
			get;
			private set;
		}

		private void Awake()
		{
			if (!instance)
			{
				instance = this;
				Object.DontDestroyOnLoad(this);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Start()
		{
			Screen.sleepTimeout = -1;
			CheckLeaderBoard();
			GameEntered();
			upgradesData = UpgradeShopData.instance;
			guiPressedFingerId = -1f;
			guiPressed = false;
			lastOpenedShopWindow = -1;
			StartCoroutine(SceneEffect());
		}

		private void GameEntered()
		{
			if (!PlayerPrefs.HasKey("IsAppRated"))
			{
				IsAppRated = false;
				PlayerPrefs.SetInt("IsAppRated", IsAppRated ? 1 : 0);
			}
			else
			{
				int @int = PlayerPrefs.GetInt("IsAppRated");
				IsAppRated = (@int == 1);
			}
			if (!IsAppRated)
			{
				if (!PlayerPrefs.HasKey("LaunchCount"))
				{
					LaunchCount = 1;
				}
				else
				{
					LaunchCount = PlayerPrefs.GetInt("LaunchCount");
					LaunchCount++;
				}
				PlayerPrefs.SetInt("LaunchCount", LaunchCount);
			}
		}

		public void AppRated()
		{
			IsAppRated = true;
			PlayerPrefs.SetInt("IsAppRated", 1);
		}

		private void CheckLeaderBoard()
		{
			qar = UnityEngine.Random.Range(-100, -10);
			qar_k = -qar;
			if (!PlayerPrefs.HasKey("b_qar"))
			{
				if (PlayerPrefs.HasKey("Best"))
				{
					b_qar = PlayerPrefs.GetInt("Best");
					b_qar_k = 0;
					PlayerPrefs.SetInt("b_qar", b_qar);
					PlayerPrefs.SetInt("b_qar_k", 0);
					PlayerPrefs.DeleteKey("Best");
					Leaderboard.Instance.IncludeScoreInLeaderboard(b_qar, b_qar_k);
				}
				else
				{
					PlayerPrefs.SetInt("b_qar", 0);
					PlayerPrefs.SetInt("b_qar_k", 0);
				}
			}
			b_qar = PlayerPrefs.GetInt("b_qar");
			b_qar_k = PlayerPrefs.GetInt("b_qar_k");
		}

		public void AddPoint()
		{
			qar++;
		}

		public void ResetPoint()
		{
			qar = UnityEngine.Random.Range(-100, -10);
			qar_k = -qar;
		}

		public int GetPoint()
		{
			return qar + qar_k;
		}

		public void SetBestScore()
		{
			b_qar = qar;
			b_qar_k = qar_k;
			PlayerPrefs.SetInt("b_qar", b_qar);
			PlayerPrefs.SetInt("b_qar_k", b_qar_k);
			Leaderboard.Instance.IncludeScoreInLeaderboard(b_qar, b_qar_k);
		}

		public int GetBestScore()
		{
			return b_qar + b_qar_k;
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				Screen.sleepTimeout = -2;
			}
			else
			{
				Screen.sleepTimeout = -1;
			}
		}

		private void OnApplicationQuit()
		{
			Screen.sleepTimeout = -2;
		}

		public float GetCurrentCost()
		{
			return upgradesData.AskCatapultCost(NewDataController.instance.GetCurrentCatapultIndex(), NewDataController.instance.GetCurrentCatapultUpgrade());
		}

		public float GetCatapultHPIncrease()
		{
			return upgradesData.AskCatapultHPIncrease(NewDataController.instance.GetCurrentCatapultIndex(), NewDataController.instance.GetCurrentCatapultUpgrade());
		}

		public Sprite AskCatapultSprite(int index, bool isPvp = false)
		{
			if (!isPvp)
			{
				return upgradesData.AskSprite(NewDataController.instance.GetCurrentCatapultIndex(), NewDataController.instance.GetCurrentCatapultUpgrade(), index);
			}
			if (NewDataController.instance.GetCurrentCatapultIndex() == 0)
			{
				return upgradesData.AskSprite(0, NewDataController.instance.GetCurrentCatapultUpgrade(), index);
			}
			return upgradesData.AskSprite(0, 10, index);
		}

		public Sprite AskAICatapultSprite(int catapult, int index, int level)
		{
			return upgradesData.AskAiSprite(catapult, level, index);
		}

		public Sprite AskDestroyerParticle(int catapult, int index, int level)
		{
			return upgradesData.AskAIDestroyedSprite(catapult, level, index);
		}

		public Sprite AskPlayerDestroyerParticle(int catapult, int index, int level)
		{
			return upgradesData.AskPlayerDestroyedSprite(catapult, level, index);
		}

		public float GetAICatapultHPIncrease(int type, int level)
		{
			return upgradesData.AskAiHealthIncrease(type, level);
		}

		public float GetMaxLevel(int catapult)
		{
			return upgradesData.GetUpgradeLength(catapult);
		}

		private void LoadNextLevel()
		{
			BYV_ScenesLoader.Instance.Load_Scene("MainScene", statusShowInter: false);
		}

		private IEnumerator SceneEffect()
		{
			logoImage.color = new Color(1f, 1f, 1f, 0f);
			while (true)
			{
				Color color = logoImage.color;
				if (!(color.a < 0.95f))
				{
					break;
				}
				Image image = logoImage;
				Color color2 = logoImage.color;
				image.color = new Color(1f, 1f, 1f, color2.a + 1f * Time.fixedDeltaTime);
				yield return null;
			}
			logoImage.color = new Color(1f, 1f, 1f, 1f);
			yield return new WaitForSeconds(1f);
			while (true)
			{
				Color color3 = logoImage.color;
				if (!(color3.a > 0.05f))
				{
					break;
				}
				Image image2 = logoImage;
				Color color4 = logoImage.color;
				image2.color = new Color(1f, 1f, 1f, color4.a - 1f * Time.fixedDeltaTime);
				yield return null;
			}
			LoadNextLevel();
		}
	}
}
