using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
	internal class PvPIngameShop : MonoBehaviour
	{
		public static PvPIngameShop instance;

		private int[] currentIndexes;

		private float[] rechargeTimers;

		private bool[] rechargeStatus;

		private int projByCrow = 5;

		private int[] projectilesAllowed;

		private GameObject[] projectilePrefabs;

		private List<GameObject> stonePool;

		private List<GameObject> firstPlayerPool;

		private List<GameObject> secondPlayerPool;

		private float maxRechargeTime;

		private float firstRechargeTime;

		private bool isFirstRecharged;

		private float secondRechargeTime;

		private bool isSecondRecharged;

		private void Awake()
		{
			instance = this;
			isFirstRecharged = true;
			firstRechargeTime = 0f;
			isSecondRecharged = true;
			secondRechargeTime = 0f;
			maxRechargeTime = 0.5f;
		}

		private void Start()
		{
			projectilePrefabs = new GameObject[ProjectileControl.instance.projectilePrefabByIndex.Length];
			stonePool = new List<GameObject>();
			firstPlayerPool = new List<GameObject>();
			secondPlayerPool = new List<GameObject>();
			currentIndexes = new int[2];
			rechargeTimers = new float[2];
			rechargeStatus = new bool[2]
			{
				true,
				true
			};
			projectilesAllowed = new int[2];
			for (int i = 0; i < projectilePrefabs.Length; i++)
			{
				projectilePrefabs[i] = ProjectileControl.instance.GetPrefabByIndex(i);
			}
			for (int j = 0; j < 15; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(projectilePrefabs[0]);
				gameObject.SetActive(value: false);
				gameObject.transform.SetParent(base.transform);
				stonePool.Add(gameObject);
			}
		}

		public void ResetPoints(GameSides side)
		{
			projectilesAllowed[(int)side] = 0;
			PvPGameControl.instance.SetProjectileType(side, ProjectileControl.instance.GetSpriteByIndex(0));
			PvPGameControl.instance.SetAvailableCount(side, 0f);
		}

		public void BirdKilled(GameSides side)
		{
			UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			int num = UnityEngine.Random.Range(1, projectilePrefabs.Length);
			projectilesAllowed[(int)side] = projByCrow;
			currentIndexes[(int)side] = num;
			ClearPool(side);
			for (int i = 0; i < projByCrow; i++)
			{
				if (side == GameSides.Player1)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(projectilePrefabs[num]);
					gameObject.SetActive(value: false);
					gameObject.transform.SetParent(base.transform);
					firstPlayerPool.Add(gameObject);
				}
				else
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(projectilePrefabs[num]);
					gameObject2.SetActive(value: false);
					gameObject2.transform.SetParent(base.transform);
					secondPlayerPool.Add(gameObject2);
				}
			}
			PvPGameControl.instance.SetProjectileType(side, ProjectileControl.instance.GetSpriteByIndex(num));
			PvPGameControl.instance.SetAvailableCount(side, projByCrow);
		}

		public void ClearPool(GameSides side)
		{
			if (side == GameSides.Player1)
			{
				for (int num = firstPlayerPool.Count - 1; num > -1; num--)
				{
					UnityEngine.Object.Destroy(firstPlayerPool[num].gameObject);
				}
				firstPlayerPool.Clear();
			}
			else
			{
				for (int num2 = secondPlayerPool.Count - 1; num2 > -1; num2--)
				{
					UnityEngine.Object.Destroy(secondPlayerPool[num2].gameObject);
				}
				secondPlayerPool.Clear();
			}
		}

		public GameObject BorrowProjectile(GameSides side)
		{
			GameObject gameObject = null;
			if (side == GameSides.Player1)
			{
				if (firstPlayerPool.Count > 0)
				{
					for (int i = 0; i < firstPlayerPool.Count; i++)
					{
						if (firstPlayerPool[i] == null)
						{
							firstPlayerPool.RemoveAt(i);
						}
						else if (!firstPlayerPool[i].activeSelf && rechargeStatus[0])
						{
							gameObject = firstPlayerPool[i];
							projectilesAllowed[(int)side]--;
							PvPGameControl.instance.SetAvailableCount(side, projectilesAllowed[(int)side]);
							break;
						}
					}
				}
			}
			else if (secondPlayerPool.Count > 0)
			{
				for (int j = 0; j < secondPlayerPool.Count; j++)
				{
					if (secondPlayerPool[j] == null)
					{
						secondPlayerPool.RemoveAt(j);
					}
					else if (!secondPlayerPool[j].activeSelf && rechargeStatus[0])
					{
						gameObject = secondPlayerPool[j];
						projectilesAllowed[(int)side]--;
						PvPGameControl.instance.SetAvailableCount(side, projectilesAllowed[(int)side]);
						break;
					}
				}
			}
			if (gameObject == null && projectilesAllowed[(int)side] == 0)
			{
				for (int k = 0; k < stonePool.Count; k++)
				{
					if (stonePool[k] == null)
					{
						stonePool.RemoveAt(k);
					}
					else if (!stonePool[k].activeSelf && rechargeStatus[(int)side])
					{
						gameObject = stonePool[k];
						break;
					}
				}
			}
			return gameObject;
		}

		public void Shoot(GameSides side)
		{
			if (projectilesAllowed[(int)side] == 0)
			{
				PvPGameControl.instance.SetProjectileType(side, ProjectileControl.instance.GetSpriteByIndex(0));
				PvPGameControl.instance.SetAvailableCount(side, 0f);
			}
			rechargeStatus[(int)side] = false;
			rechargeTimers[(int)side] = 0f;
			PvPGameControl.instance.Recharge(maxRechargeTime, side);
		}

		public void ReturnToPool(GameObject projectile, ProjectileType type, bool needNew = false)
		{
			if (needNew && type == ProjectileType.Stone)
			{
				stonePool.Remove(projectile);
				projectile = null;
				projectile = UnityEngine.Object.Instantiate(projectilePrefabs[0]);
				stonePool.Add(projectile);
			}
			if (type == ProjectileType.Stone)
			{
				projectile.SetActive(value: false);
				projectile.transform.SetParent(base.transform);
				projectile.transform.position = base.transform.position;
			}
			else
			{
				UnityEngine.Object.Destroy(projectile);
			}
		}

		private void Update()
		{
			if (!rechargeStatus[0])
			{
				if (rechargeTimers[0] < maxRechargeTime)
				{
					rechargeTimers[0] += Time.deltaTime;
				}
				else
				{
					rechargeStatus[0] = true;
				}
			}
			if (!rechargeStatus[1])
			{
				if (rechargeTimers[1] < maxRechargeTime)
				{
					rechargeTimers[1] += Time.deltaTime;
				}
				else
				{
					rechargeStatus[1] = true;
				}
			}
		}
	}
}
