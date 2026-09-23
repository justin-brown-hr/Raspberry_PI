using System;
using UnityEngine;

namespace Logic
{
	internal class ProjectileControl : MonoBehaviour
	{
		public static ProjectileControl instance;

		public Sprite[] projectileSpriteByIndex;

		public GameObject[] projectilePrefabByIndex;

		public float[] projectileRechargeByIndex;

		public ProjectileType[] projectileTypeByIndex;

		public int[] projectileCostByIndex;

		private int[] projectileCountByIndex;

		private ProjectileType[] projectileInSlots;

		private string bombPre5 = "Bomb";

		private string tripplePre5 = "Tripple";

		private string projectileCountPost5 = "projectile_";

		private string projectileEquipPost5 = "weaponSlot_";

		private string projectileCountSaveString = "proj_c_";

		private string equipedProjectileSaveString = "proj_e_";

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void Start()
		{
			projectileCountByIndex = new int[projectileSpriteByIndex.Length];
			projectileInSlots = new ProjectileType[2];
			LoadFromOldSaves();
		}

		private void LoadDataFromPlayerPrefs()
		{
			LoadProjectileCountFromPlayerPrefs();
			LoadSelectedProjectilesFromPlayerPrefs();
		}

		private void LoadProjectileCountFromPlayerPrefs()
		{
			for (int i = 0; i < projectileCountByIndex.Length; i++)
			{
				try
				{
					if (!PlayerPrefs.HasKey(projectileCountSaveString + i))
					{
						SetProjectileCount(i, 0);
						SaveProjectileCount(i);
					}
					else
					{
						SetProjectileCount(i, PlayerPrefs.GetInt(projectileCountSaveString + i));
					}
				}
				catch
				{
					SetProjectileCount(i, 0);
				}
			}
		}

		private bool CheckProjectileIndex(int index)
		{
			if (index < 0 || index >= projectileTypeByIndex.Length)
			{
				return false;
			}
			return true;
		}

		private void SetProjectileCount(int index, int value)
		{
			if (CheckProjectileIndex(index) && value >= 0)
			{
				projectileCountByIndex[index] = value;
			}
		}

		private void SaveProjectileCount(int index)
		{
			if (CheckProjectileIndex(index))
			{
				PlayerPrefs.SetInt(projectileCountSaveString + index, projectileCountByIndex[index]);
			}
		}

		public int GetProjectileCount(int index)
		{
			if (!CheckProjectileIndex(index))
			{
				return -1;
			}
			return projectileCountByIndex[index];
		}

		public int[] GetAllProjectileCounts()
		{
			int[] array = new int[projectileCountByIndex.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = projectileCountByIndex[i];
			}
			return array;
		}

		private void LoadSelectedProjectilesFromPlayerPrefs()
		{
			for (int i = 0; i < projectileInSlots.Length; i++)
			{
				try
				{
					if (!PlayerPrefs.HasKey(equipedProjectileSaveString + i))
					{
						SetEquipedProjectile(i, 0);
						SaveEquipedProjectile(i);
					}
					else
					{
						SetEquipedProjectile(i, PlayerPrefs.GetInt(equipedProjectileSaveString + i));
					}
				}
				catch
				{
					SetEquipedProjectile(i, 0);
				}
			}
		}

		private ProjectileType IndexToType(int index)
		{
			if (!CheckProjectileIndex(index))
			{
				return ProjectileType.Stone;
			}
			return projectileTypeByIndex[index];
		}

		private int TypeToIndex(ProjectileType type)
		{
			for (int i = 0; i < projectileTypeByIndex.Length; i++)
			{
				if (projectileTypeByIndex[i] == type)
				{
					return i;
				}
			}
			return -1;
		}

		private void SetEquipedProjectile(int slotIndex, int value)
		{
			if ((slotIndex == 0 || slotIndex == 1) && CheckProjectileIndex(value))
			{
				projectileInSlots[slotIndex] = IndexToType(value);
			}
		}

		public void SaveEquipedProjectile(int slotIndex)
		{
			if (slotIndex == 0 || slotIndex == 1)
			{
				int num = TypeToIndex(projectileInSlots[slotIndex]);
				if (num != -1)
				{
					PlayerPrefs.SetInt(equipedProjectileSaveString + slotIndex, num);
				}
			}
		}

		public int GetEquipedProjectileIndex(int slotIndex)
		{
			if (slotIndex != 0 && slotIndex != 1)
			{
				return -1;
			}
			int num = TypeToIndex(projectileInSlots[slotIndex]);
			if (num != -1)
			{
				return num;
			}
			return -1;
		}

		public int[] GetAllEquipedProjectiles()
		{
			int[] array = new int[projectileInSlots.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = TypeToIndex(projectileInSlots[i]);
			}
			return array;
		}

		public ProjectileType CheckSlotProjectile(int slotIndex)
		{
			switch (slotIndex)
			{
			case -1:
				return ProjectileType.Stone;
			case 0:
			case 1:
				return projectileInSlots[slotIndex];
			default:
				return ProjectileType.Stone;
			}
		}

		public GameObject GetPrefabByType(ProjectileType type)
		{
			int num = TypeToIndex(type);
			if (num != -1)
			{
				return projectilePrefabByIndex[num];
			}
			return null;
		}

		public GameObject GetPrefabByIndex(int index)
		{
			if (!CheckProjectileIndex(index))
			{
				return null;
			}
			return projectilePrefabByIndex[index];
		}

		public float GetProjectileRechargeByType(ProjectileType type)
		{
			int num = TypeToIndex(type);
			if (num != -1)
			{
				return projectileRechargeByIndex[num];
			}
			return -1f;
		}

		public int BuyProjectileIngame(int slotIndex)
		{
			if (slotIndex != 0 && slotIndex != 1)
			{
				return -1;
			}
			int num = TypeToIndex(projectileInSlots[slotIndex]);
			if (num != -1)
			{
				projectileCountByIndex[num] += 10;
				SaveProjectileCount(num);
				return projectileCountByIndex[num];
			}
			return -1;
		}

		public int BuyProjectile(int index)
		{
			if (!CheckProjectileIndex(index))
			{
				return -1;
			}
			projectileCountByIndex[index] += 10;
			SaveProjectileCount(index);
			return projectileCountByIndex[index];
		}

		public Sprite GetSpriteByType(ProjectileType type)
		{
			int num = TypeToIndex(type);
			if (num != -1)
			{
				return projectileSpriteByIndex[num];
			}
			return null;
		}

		public Sprite GetSpriteByIndex(int index)
		{
			if (!CheckProjectileIndex(index))
			{
				return null;
			}
			return projectileSpriteByIndex[index];
		}

		public int GetCostByType(ProjectileType type)
		{
			int num = TypeToIndex(type);
			if (num != -1)
			{
				return projectileCostByIndex[num];
			}
			return -1;
		}

		public int GetProjectileCountByType(ProjectileType type)
		{
			int num = TypeToIndex(type);
			if (num != -1)
			{
				return projectileCountByIndex[num];
			}
			return -1;
		}

		public void UseProjectileInSlot(int slotIndex)
		{
			int num = TypeToIndex(projectileInSlots[slotIndex]);
			if (num != -1)
			{
				projectileCountByIndex[num]--;
				SaveProjectileCount(num);
			}
		}

		public int GetProjectileCountInSlot(int slotIndex)
		{
			int num = TypeToIndex(projectileInSlots[slotIndex]);
			if (num != -1)
			{
				return projectileCountByIndex[num];
			}
			return -1;
		}

		public int EquipProjectile(int index)
		{
			ProjectileType projectileType = IndexToType(index);
			if (projectileType == ProjectileType.Stone)
			{
				return -1;
			}
			for (int i = 0; i < projectileInSlots.Length; i++)
			{
				if (projectileInSlots[i] == projectileType)
				{
					return -1;
				}
			}
			for (int j = 0; j < projectileInSlots.Length; j++)
			{
				if (projectileInSlots[j] == ProjectileType.Stone)
				{
					projectileInSlots[j] = projectileType;
					SaveEquipedProjectile(j);
					return j;
				}
			}
			return -1;
		}

		public void ClearSlot(int slotIndex)
		{
			projectileInSlots[slotIndex] = ProjectileType.Stone;
			SaveEquipedProjectile(slotIndex);
		}

		public void LoadFromOldSaves()
		{
			if (PlayerPrefs.HasKey(projectileCountSaveString + 0))
			{
				LoadDataFromPlayerPrefs();
				return;
			}
			if (!PlayerPrefs.HasKey(projectileCountPost5 + 0))
			{
				LoadDataFromPlayerPrefs();
				try
				{
					if (PlayerPrefs.HasKey(tripplePre5))
					{
						SetProjectileCount(1, PlayerPrefs.GetInt(tripplePre5));
						SaveProjectileCount(1);
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex.ToString());
				}
				try
				{
					if (PlayerPrefs.HasKey(bombPre5))
					{
						SetProjectileCount(2, PlayerPrefs.GetInt(bombPre5));
						SaveProjectileCount(2);
					}
				}
				catch (Exception ex2)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex2.ToString());
				}
				return;
			}
			for (int i = 0; i < projectileCountByIndex.Length; i++)
			{
				try
				{
					if (PlayerPrefs.HasKey(projectileCountPost5 + i))
					{
						SetProjectileCount(i, PlayerPrefs.GetInt(projectileCountPost5 + i));
						SaveProjectileCount(i);
					}
				}
				catch (Exception ex3)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex3.ToString());
				}
			}
			for (int j = 0; j < projectileInSlots.Length; j++)
			{
				try
				{
					if (PlayerPrefs.HasKey(projectileEquipPost5 + j))
					{
						SetEquipedProjectile(j, PlayerPrefs.GetInt(projectileEquipPost5 + j));
						SaveEquipedProjectile(j);
					}
				}
				catch (Exception ex4)
				{
					UnityEngine.Debug.LogError("(BYV)" + ex4.ToString());
				}
			}
		}

		public void LoadProjectilesSavedData(int[] selected, int[] count)
		{
			for (int i = 0; i < projectileCountByIndex.Length; i++)
			{
				if (i < count.Length)
				{
					SetProjectileCount(i, count[i]);
					SaveProjectileCount(i);
				}
				else
				{
					SetProjectileCount(i, 0);
					SaveProjectileCount(i);
				}
			}
			for (int j = 0; j < projectileInSlots.Length; j++)
			{
				if (j < selected.Length)
				{
					SetEquipedProjectile(j, selected[j]);
					SaveEquipedProjectile(j);
				}
				else
				{
					SetEquipedProjectile(j, 0);
					SaveEquipedProjectile(j);
				}
			}
		}

		public void LoadOldProjectilesSavedData(int[] selected, int[] count)
		{
			for (int i = 0; i < projectileCountByIndex.Length; i++)
			{
				if (i < count.Length && count[i] >= 0)
				{
					SetProjectileCount(i, count[i]);
					SaveProjectileCount(i);
				}
				else
				{
					SetProjectileCount(i, 0);
					SaveProjectileCount(i);
				}
			}
			for (int j = 0; j < projectileInSlots.Length; j++)
			{
				if (j < selected.Length && CheckProjectileIndex(selected[j]))
				{
					SetEquipedProjectile(j, selected[j]);
					SaveEquipedProjectile(j);
				}
				else
				{
					SetEquipedProjectile(j, 0);
					SaveEquipedProjectile(j);
				}
			}
		}
	}
}
