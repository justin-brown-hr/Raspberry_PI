using Exploder2D;
using Model;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Logic
{
	internal class IngameShop : MonoBehaviour
	{
		public static IngameShop instance;

		private ProjectileControl _mainControl;

		public IngameShopControls[] _controls;

		public IngameShieldControl _shieldControl;

		public GameObject enemySpecialPrefab;

		public GameObject bossProjectilePrefab;

		private GameObject[] projectileInSlotPrefabs;

		private List<GameObject>[] slotPools;

		private float[] poolSizes;

		private float[] poolRechargeTime;

		private float[] poolRechargeTimers;

		private bool[] poolIsRecharged;

		private float[] poolCost;

		private GameObject shieldPrefab;

		private ShieldTypes _currentShieldType;

		public GameObject fragmentPoolParent;

		public GameObject[] destroyedFragments;

		private GameObject[] simpleStoneFragmentPool;

		private GameObject[] spikeStoneFragmentPool;

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			_mainControl = ProjectileControl.instance;
			InitFragmentPool();
			PrepareShield();
			PrepareSlots();
			SetProjectilesInSlots();
			GeneratePool();
			_controls[0].Activate();
		}

		public void InitFragmentPool()
		{
			simpleStoneFragmentPool = new GameObject[24];
			for (int i = 0; i < 24; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(destroyedFragments[0], fragmentPoolParent.transform);
				gameObject.SetActive(value: false);
				gameObject.GetComponent<DestroyedProjectilePart>().InitPart();
				simpleStoneFragmentPool[i] = gameObject;
			}
			spikeStoneFragmentPool = new GameObject[24];
			for (int j = 0; j < 24; j++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(destroyedFragments[1], fragmentPoolParent.transform);
				gameObject2.SetActive(value: false);
				gameObject2.GetComponent<DestroyedProjectilePart>().InitPart();
				spikeStoneFragmentPool[j] = gameObject2;
			}
		}

		private void PrepareShield()
		{
			shieldPrefab = UpgradeShopData.instance.GetCurrentShieldPrebab();
			if (shieldPrefab != null)
			{
				_shieldControl.InitSlot(UpgradeShopData.instance.GetCurrentShieldIcon(), UpgradeShopData.instance.GetCurrentShieldActiveTime(), UpgradeShopData.instance.GetCurrentShieldRechargeTime());
				_currentShieldType = shieldPrefab.GetComponent<PvEShield>()._shieldType;
				GlobalLogic.instance.playerCatapult.InitShield(shieldPrefab);
			}
			else
			{
				_shieldControl.gameObject.SetActive(value: false);
			}
		}

		private void PrepareSlots()
		{
			projectileInSlotPrefabs = new GameObject[3];
			slotPools = new List<GameObject>[5];
			poolSizes = new float[5];
			poolRechargeTime = new float[3];
			poolRechargeTimers = new float[3];
			poolIsRecharged = new bool[3];
			poolCost = new float[3];
		}

		private void SetProjectilesInSlots()
		{
			ProjectileType projectileType = _mainControl.CheckSlotProjectile(-1);
			if (projectileType != 0)
			{
				UnityEngine.Debug.LogError("Something goes wrong with projectile pool generation");
				return;
			}
			projectileInSlotPrefabs[0] = _mainControl.GetPrefabByType(projectileType);
			if (projectileInSlotPrefabs[0] != null)
			{
				poolRechargeTime[0] = _mainControl.GetProjectileRechargeByType(projectileType);
				poolSizes[0] = 18f;
				slotPools[0] = new List<GameObject>();
				poolIsRecharged[0] = true;
				_controls[0].Initialize(projectileType, poolRechargeTime[0]);
			}
			else
			{
				_controls[0].gameObject.SetActive(value: false);
				poolSizes[0] = 0f;
			}
			for (int i = 0; i < 2; i++)
			{
				ProjectileType projectileType2 = _mainControl.CheckSlotProjectile(i);
				if (projectileType2 == ProjectileType.Stone)
				{
					_controls[i + 1].gameObject.SetActive(value: false);
					continue;
				}
				projectileInSlotPrefabs[i + 1] = _mainControl.GetPrefabByType(projectileType2);
				if (projectileInSlotPrefabs[i + 1] != null)
				{
					poolRechargeTime[i + 1] = _mainControl.GetProjectileRechargeByType(projectileType2);
					poolSizes[i + 1] = 15f;
					slotPools[i + 1] = new List<GameObject>();
					poolIsRecharged[i + 1] = true;
					_controls[i + 1].Initialize(projectileType2, poolRechargeTime[i + 1]);
				}
				else
				{
					_controls[i + 1].gameObject.SetActive(value: false);
					poolSizes[i + 1] = 0f;
				}
			}
			if (enemySpecialPrefab != null)
			{
				poolSizes[3] = 8f;
				slotPools[3] = new List<GameObject>();
			}
			if (bossProjectilePrefab != null)
			{
				poolSizes[4] = 18f;
				slotPools[4] = new List<GameObject>();
			}
		}

		public GameObject GetFragmentFromPool(ProjectileType type)
		{
			GameObject result = null;
			switch (type)
			{
			case ProjectileType.Stone:
				for (int j = 0; j < simpleStoneFragmentPool.Length; j++)
				{
					if (simpleStoneFragmentPool[j] == null)
					{
						simpleStoneFragmentPool[j] = UnityEngine.Object.Instantiate(destroyedFragments[0], fragmentPoolParent.transform);
						simpleStoneFragmentPool[j].SetActive(value: false);
						simpleStoneFragmentPool[j].GetComponent<DestroyedProjectilePart>().InitPart();
					}
					if (!simpleStoneFragmentPool[j].activeSelf)
					{
						simpleStoneFragmentPool[j].SetActive(value: true);
						return simpleStoneFragmentPool[j];
					}
				}
				break;
			case ProjectileType.StoneThorns:
				for (int i = 0; i < spikeStoneFragmentPool.Length; i++)
				{
					if (spikeStoneFragmentPool[i] == null)
					{
						spikeStoneFragmentPool[i] = UnityEngine.Object.Instantiate(destroyedFragments[1], fragmentPoolParent.transform);
						spikeStoneFragmentPool[i].SetActive(value: false);
						spikeStoneFragmentPool[i].GetComponent<DestroyedProjectilePart>().InitPart();
					}
					if (!spikeStoneFragmentPool[i].activeSelf)
					{
						spikeStoneFragmentPool[i].SetActive(value: true);
						return spikeStoneFragmentPool[i];
					}
				}
				break;
			}
			return result;
		}

		public void ReInitShield()
		{
			shieldPrefab = UpgradeShopData.instance.GetCurrentShieldPrebab();
			if (shieldPrefab != null)
			{
				GlobalLogic.instance.playerCatapult.InitShield(shieldPrefab);
			}
		}

		public void EnableShield()
		{
			if (_shieldControl.ShieldSlotCkick())
			{
				GlobalLogic.instance.playerCatapult.ActivateShield();
			}
		}

		public void DisableShield()
		{
			GlobalLogic.instance.playerCatapult.DeactivateShield();
		}

		private void GeneratePool()
		{
			for (int i = 0; (float)i < poolSizes[0]; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(projectileInSlotPrefabs[0]);
				gameObject.SetActive(value: false);
				gameObject.transform.SetParent(base.transform);
				gameObject.GetComponent<Projectile>().InitInPool(0);
				slotPools[0].Add(gameObject);
			}
			for (int j = 0; (float)j < poolSizes[1]; j++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(projectileInSlotPrefabs[1]);
				gameObject2.SetActive(value: false);
				gameObject2.transform.SetParent(base.transform);
				gameObject2.GetComponent<Projectile>().InitInPool(1);
				slotPools[1].Add(gameObject2);
			}
			for (int k = 0; (float)k < poolSizes[2]; k++)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate(projectileInSlotPrefabs[2]);
				gameObject3.SetActive(value: false);
				gameObject3.transform.SetParent(base.transform);
				gameObject3.GetComponent<Projectile>().InitInPool(2);
				slotPools[2].Add(gameObject3);
			}
			for (int l = 0; (float)l < poolSizes[3]; l++)
			{
				GameObject gameObject4 = UnityEngine.Object.Instantiate(enemySpecialPrefab);
				gameObject4.SetActive(value: false);
				gameObject4.transform.SetParent(base.transform);
				gameObject4.GetComponent<Projectile>().InitInPool(3);
				slotPools[3].Add(gameObject4);
			}
			for (int m = 0; (float)m < poolSizes[4]; m++)
			{
				GameObject gameObject5 = UnityEngine.Object.Instantiate(bossProjectilePrefab);
				gameObject5.SetActive(value: false);
				gameObject5.transform.SetParent(base.transform);
				gameObject5.GetComponent<Projectile>().InitInPool(4);
				slotPools[4].Add(gameObject5);
			}
		}

		public void StartRecharge(int slot)
		{
			if (slot > 0)
			{
				_mainControl.UseProjectileInSlot(slot - 1);
				RefreshCount(slot);
			}
			_controls[slot].StartRecharge();
			poolRechargeTimers[slot] = 0f;
			poolIsRecharged[slot] = false;
		}

		public void RefreshCount(int slot)
		{
			_controls[slot].RefreshCount(_mainControl.GetProjectileCountInSlot(slot - 1));
		}

		private void RechargeTick()
		{
			if (poolSizes[0] > 0f && !poolIsRecharged[0])
			{
				if (poolRechargeTimers[0] < poolRechargeTime[0])
				{
					poolRechargeTimers[0] += Time.deltaTime;
				}
				else
				{
					poolIsRecharged[0] = true;
				}
			}
			if (poolSizes[1] > 0f && !poolIsRecharged[1])
			{
				if (poolRechargeTimers[1] < poolRechargeTime[1])
				{
					poolRechargeTimers[1] += Time.deltaTime;
				}
				else
				{
					poolIsRecharged[1] = true;
				}
			}
			if (poolSizes[2] > 0f && !poolIsRecharged[2])
			{
				if (poolRechargeTimers[2] < poolRechargeTime[2])
				{
					poolRechargeTimers[2] += Time.deltaTime;
				}
				else
				{
					poolIsRecharged[2] = true;
				}
			}
		}

		private void Update()
		{
			RechargeTick();
		}

		public GameObject BorrowProjectile(int fromSlot, bool requestFromPlayer = false)
		{
			GameObject gameObject = null;
			switch (fromSlot)
			{
			case -1:
				for (int m = 0; m < slotPools[0].Count; m++)
				{
					if (slotPools[0][m] == null)
					{
						slotPools[0].RemoveAt(m);
					}
					else if (!slotPools[0][m].activeSelf)
					{
						if (!requestFromPlayer)
						{
							gameObject = slotPools[0][m];
							break;
						}
						if (poolIsRecharged[0])
						{
							gameObject = slotPools[0][m];
							break;
						}
					}
				}
				break;
			case 0:
				if (_mainControl.GetProjectileCountInSlot(0) > 0)
				{
					for (int k = 0; k < slotPools[1].Count; k++)
					{
						if (slotPools[1][k] == null)
						{
							slotPools[1].RemoveAt(k);
						}
						else if (!slotPools[1][k].activeSelf)
						{
							if (!requestFromPlayer)
							{
								gameObject = slotPools[1][k];
								break;
							}
							if (poolIsRecharged[1])
							{
								gameObject = slotPools[1][k];
								break;
							}
						}
					}
				}
				else
				{
					GlobalLogic.instance.ChangeProjectile(-1);
					_controls[0].Activate();
					gameObject = BorrowProjectile(-1, requestFromPlayer);
				}
				break;
			case 1:
				if (_mainControl.GetProjectileCountInSlot(1) > 0)
				{
					for (int j = 0; j < slotPools[2].Count; j++)
					{
						if (slotPools[2][j] == null)
						{
							slotPools[2].RemoveAt(j);
						}
						else if (!slotPools[2][j].activeSelf)
						{
							if (!requestFromPlayer)
							{
								gameObject = slotPools[2][j];
								break;
							}
							if (poolIsRecharged[2])
							{
								gameObject = slotPools[2][j];
								break;
							}
						}
					}
				}
				else
				{
					GlobalLogic.instance.ChangeProjectile(-1);
					_controls[0].Activate();
					gameObject = BorrowProjectile(-1, requestFromPlayer);
				}
				break;
			case 3:
				if (requestFromPlayer)
				{
					break;
				}
				for (int l = 0; l < slotPools[3].Count; l++)
				{
					if (slotPools[3][l] == null)
					{
						slotPools[3].RemoveAt(l);
					}
					else if (!slotPools[3][l].activeSelf)
					{
						gameObject = slotPools[3][l];
						break;
					}
				}
				if (gameObject == null)
				{
					gameObject = BorrowProjectile(-1);
				}
				break;
			case 4:
				if (requestFromPlayer)
				{
					break;
				}
				for (int i = 0; i < slotPools[4].Count; i++)
				{
					if (slotPools[4][i] == null)
					{
						slotPools[4].RemoveAt(i);
						GameObject gameObject2 = UnityEngine.Object.Instantiate(bossProjectilePrefab);
						gameObject2.SetActive(value: false);
						gameObject2.transform.SetParent(base.transform);
						gameObject2.GetComponent<Projectile>().InitInPool(4);
						slotPools[4].Add(gameObject2);
						gameObject = gameObject2;
						break;
					}
					if (!slotPools[4][i].activeSelf)
					{
						gameObject = slotPools[4][i];
						slotPools[4].RemoveAt(i);
						slotPools[4].Add(gameObject);
						break;
					}
				}
				break;
			}
			return gameObject;
		}

		public void ReturnToPool(int fromSlot, GameObject obj, bool needNew = false)
		{
			switch (fromSlot)
			{
			case 0:
				if (needNew)
				{
					if (obj != null)
					{
						slotPools[0].Remove(obj);
					}
					obj = null;
					obj = UnityEngine.Object.Instantiate(projectileInSlotPrefabs[0]);
					obj.GetComponent<Projectile>().InitInPool(0);
					slotPools[0].Add(obj);
				}
				break;
			case 1:
				if (needNew)
				{
					if (obj != null)
					{
						slotPools[1].Remove(obj);
					}
					obj = null;
					obj = UnityEngine.Object.Instantiate(projectileInSlotPrefabs[1]);
					obj.GetComponent<Projectile>().InitInPool(1);
					slotPools[1].Add(obj);
				}
				break;
			case 2:
				if (needNew)
				{
					if (obj != null)
					{
						slotPools[2].Remove(obj);
					}
					obj = null;
					obj = UnityEngine.Object.Instantiate(projectileInSlotPrefabs[2]);
					obj.GetComponent<Projectile>().InitInPool(2);
					slotPools[2].Add(obj);
				}
				break;
			}
			obj.SetActive(value: false);
			obj.transform.SetParent(base.transform);
			obj.transform.position = base.transform.position;
		}

		public void OnComponentDestroy()
		{
			List<Fragment2D> activeFragments = FragmentPool2D.Instance.GetActiveFragments();
			for (int i = 0; i < activeFragments.Count; i++)
			{
				if (activeFragments[i] != null)
				{
					activeFragments[i].gameObject.transform.parent = null;
					activeFragments[i].gameObject.layer = UnityEngine.Random.Range(14, 16);
					activeFragments[i].gameObject.GetComponent<Rigidbody2D>().useAutoMass = true;
					activeFragments[i].gameObject.GetComponent<Rigidbody2D>().gravityScale = 4f;
				}
				else
				{
					UnityEngine.Object.Destroy(activeFragments[i]);
				}
			}
		}
	}
}
