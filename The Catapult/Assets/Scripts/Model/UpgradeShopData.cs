using Logic;
using UnityEngine;

namespace Model
{
	internal class UpgradeShopData : MonoBehaviour
	{
		public static UpgradeShopData instance;

		[Header("First catapult data")]
		public string[] catapult_1_byLevel_new;

		public float[] catapult_1_byHP;

		public int[] catapult_1_byCost;

		[Header("Second catapult data")]
		public string[] catapult_2_byLevel_new;

		public float[] catapult_2_byHP;

		public int[] catapult_2_byCost;

		[Header("Third catapult data")]
		public string[] catapult_3_byLevel_new;

		public float[] catapult_3_byHP;

		public int[] catapult_3_byCost;

		[Header("Helmets data")]
		public Sprite[] helmetSprites;

		public float[] playerHPIncrease;

		[Header("Shields data")]
		public GameObject[] shieldPrefab;

		public Sprite[] shieldIcon;

		public float[] shieldRechargeTime;

		public float[] shieldActiveTime;

		[Header("Enemy catapults data")]
		public string[] enemy_big_byLevel_name;

		public float[] enemy_big_byHP;

		public string[] enemy_ground_0_byLevel_name;

		public float[] enemy_ground_0_byHP;

		public string[] enemy_ground_1_byLevel_name;

		public float[] enemy_ground_1_byHP;

		public string[] enemy_ground_2_byLevel_name;

		public float[] enemy_ground_2_byHP;

		public string[] enemy_ground_3_byLevel_name;

		public float[] enemy_ground_3_byHP;

		[Header("Destroyed catapults data")]
		public string[] enemy_0_destr_name;

		public string[] enemy_1_destr_name;

		public string[] enemy_2g_destr_name;

		public string[] enemy_3g_destr_name;

		public string[] enemy_4g_destr_name;

		public string[] enemy_5g_destr_name;

		public string[] player_1_destr_name;

		public string[] player_2_destr_name;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public Sprite AskSprite(int catapultType, int catapultLevel, int index)
		{
			Sprite result = null;
			switch (catapultType)
			{
			case 0:
				if (catapultLevel != 0 || index != 7)
				{
					Sprite[] array = Resources.LoadAll<Sprite>("CatapultSprites/" + catapult_1_byLevel_new[catapultLevel]);
					if (array.Length > index)
					{
						result = array[index];
					}
				}
				break;
			case 1:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("CatapultSprites/" + catapult_2_byLevel_new[catapultLevel]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 2:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("CatapultSprites/" + catapult_3_byLevel_new[catapultLevel]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			}
			return result;
		}

		public int GetUpgradeLength(int catapultType)
		{
			int result = -1;
			switch (catapultType)
			{
			case 0:
				result = catapult_1_byCost.Length;
				break;
			case 1:
				result = catapult_2_byCost.Length;
				break;
			case 2:
				result = catapult_3_byCost.Length;
				break;
			default:
				UnityEngine.Debug.LogError("Wrong catapult index at UpgradeShop -> " + catapultType);
				break;
			}
			return result;
		}

		public int AskCurrentCatapultCost()
		{
			int result = -2;
			switch (NewDataController.instance.GetCurrentCatapultIndex())
			{
			case 0:
				result = catapult_1_byCost[NewDataController.instance.GetCurrentCatapultUpgrade()];
				break;
			case 1:
				result = catapult_2_byCost[NewDataController.instance.GetCurrentCatapultUpgrade()];
				break;
			case 2:
				result = catapult_3_byCost[NewDataController.instance.GetCurrentCatapultUpgrade()];
				break;
			}
			return result;
		}

		public float AskCatapultCost(int catapultType, int catapultLevel)
		{
			float result = -2f;
			switch (catapultType)
			{
			case 0:
				result = catapult_1_byCost[catapultLevel];
				break;
			case 1:
				result = catapult_2_byCost[catapultLevel];
				break;
			case 2:
				result = catapult_3_byCost[catapultLevel];
				break;
			}
			return result;
		}

		public float AskCatapultHPIncrease(int catapultType, int catapultLevel)
		{
			float result = -1f;
			switch (catapultType)
			{
			case 0:
				result = catapult_1_byHP[catapultLevel];
				break;
			case 1:
				result = catapult_2_byHP[catapultLevel];
				break;
			case 2:
				result = catapult_3_byHP[catapultLevel];
				break;
			}
			return result;
		}

		public int GetHelmetCount()
		{
			return helmetSprites.Length;
		}

		public Sprite GetCurrentHelmetSprite()
		{
			int equipedHelmet = NewDataController.instance.GetEquipedHelmet();
			if (equipedHelmet < 0 || equipedHelmet >= helmetSprites.Length)
			{
				return null;
			}
			Sprite sprite = null;
			return helmetSprites[equipedHelmet];
		}

		public float GetCurrentHelmetHPIncrease()
		{
			int equipedHelmet = NewDataController.instance.GetEquipedHelmet();
			if (equipedHelmet < 0 || equipedHelmet >= helmetSprites.Length)
			{
				return 1f;
			}
			return playerHPIncrease[equipedHelmet];
		}

		public int GetShieldCount()
		{
			return shieldPrefab.Length;
		}

		public GameObject GetCurrentShieldPrebab()
		{
			int equipedShield = NewDataController.instance.GetEquipedShield();
			if (equipedShield == 999)
			{
				return null;
			}
			GameObject gameObject = null;
			return shieldPrefab[equipedShield];
		}

		private GameObject GetShieldPrefab(int index)
		{
			if (index < 0 || index > shieldPrefab.Length)
			{
				return null;
			}
			return shieldPrefab[index];
		}

		public Sprite GetCurrentShieldIcon()
		{
			int equipedShield = NewDataController.instance.GetEquipedShield();
			if (equipedShield == 999)
			{
				return null;
			}
			return shieldIcon[equipedShield];
		}

		public float GetCurrentShieldActiveTime()
		{
			int equipedShield = NewDataController.instance.GetEquipedShield();
			if (equipedShield == 999)
			{
				return -1f;
			}
			return shieldActiveTime[equipedShield];
		}

		public float GetCurrentShieldRechargeTime()
		{
			int equipedShield = NewDataController.instance.GetEquipedShield();
			if (equipedShield == 999)
			{
				return -1f;
			}
			return shieldRechargeTime[equipedShield];
		}

		public ShieldTypes GetShieldType(int index)
		{
			GameObject gameObject = GetShieldPrefab(index);
			if (gameObject != null)
			{
				return gameObject.GetComponent<PvEShield>()._shieldType;
			}
			return ShieldTypes.None;
		}

		public Sprite AskAiSprite(int catapult, int level, int index)
		{
			Sprite result = null;
			switch (catapult)
			{
			case 0:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("CatapultSprites/" + catapult_1_byLevel_new[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 1:
			{
				if (level == 0 && index == 4)
				{
					result = AskAiSprite(1, 1, 4);
					break;
				}
				Sprite[] array = Resources.LoadAll<Sprite>("EnemyCatapultSprites/" + enemy_big_byLevel_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 2:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("EnemyCatapultSprites/" + enemy_ground_0_byLevel_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 3:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("EnemyCatapultSprites/" + enemy_ground_1_byLevel_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 4:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("EnemyCatapultSprites/" + enemy_ground_2_byLevel_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 5:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("EnemyCatapultSprites/" + enemy_ground_3_byLevel_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			}
			return result;
		}

		public float AskAiHealthIncrease(int catapult, int level)
		{
			float result = -1f;
			switch (catapult)
			{
			case 0:
				result = catapult_1_byHP[level];
				break;
			case 1:
				result = enemy_big_byHP[level];
				break;
			case 2:
				result = enemy_ground_0_byHP[level];
				break;
			case 3:
				result = enemy_ground_1_byHP[level];
				break;
			case 4:
				result = enemy_ground_2_byHP[level];
				break;
			case 5:
				result = enemy_ground_3_byHP[level];
				break;
			}
			return result;
		}

		public Sprite AskAIDestroyedSprite(int catapult, int level, int index)
		{
			Sprite result = null;
			switch (catapult)
			{
			case 0:
				if (enemy_0_destr_name.Length >= level)
				{
					Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + enemy_0_destr_name[level]);
					if (array.Length > index)
					{
						result = array[index];
					}
				}
				break;
			case 1:
				if (enemy_1_destr_name.Length >= level)
				{
					Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + enemy_1_destr_name[level]);
					if (array.Length > index)
					{
						result = array[index];
					}
				}
				break;
			case 2:
				if (enemy_2g_destr_name.Length >= level)
				{
					Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + enemy_2g_destr_name[level]);
					if (array.Length > index)
					{
						result = array[index];
					}
				}
				break;
			case 3:
				if (enemy_3g_destr_name.Length >= level)
				{
					Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + enemy_3g_destr_name[level]);
					if (array.Length > index)
					{
						result = array[index];
					}
				}
				break;
			case 4:
				if (enemy_4g_destr_name.Length >= level)
				{
					Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + enemy_4g_destr_name[level]);
					if (array.Length > index)
					{
						result = array[index];
					}
				}
				break;
			case 5:
				if (enemy_5g_destr_name.Length >= level)
				{
					Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + enemy_5g_destr_name[level]);
					if (array.Length > index)
					{
						result = array[index];
					}
				}
				break;
			}
			return result;
		}

		public Sprite AskPlayerDestroyedSprite(int catapult, int level, int index)
		{
			Sprite result = null;
			switch (catapult)
			{
			case 0:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + enemy_0_destr_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 1:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + player_1_destr_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			case 2:
			{
				Sprite[] array = Resources.LoadAll<Sprite>("DestroyerSprites/" + player_2_destr_name[level]);
				if (array.Length > index)
				{
					result = array[index];
				}
				break;
			}
			}
			return result;
		}
	}
}
