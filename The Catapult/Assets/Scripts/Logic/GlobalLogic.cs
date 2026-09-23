using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
	public class GlobalLogic : MonoBehaviour
	{
		public static GlobalLogic instance;

		public SpriteRenderer background;

		public Leaderboard _leaderboard;

		public GameMenuControl menuControl;

		internal CatapultLogic playerCatapult;

		private List<CatapultLogic> enemyCatapults;

		public Exploder2D_ExplodeHandler _ExplodeHandler;

		public string[] playersPlatformNames;

		public string[] enemysPlatformNames;

		public string[] groundEnemysPlatformNames;

		public GameObject birdPrefab;

		public int[] bossRewards;

		public int bossSpawnScore;

		internal bool guiPressed;

		internal int _currentProjectileSlot;

		private bool newGame;

		internal bool isPaused;

		private float respawnTime;

		private float respawnTick;

		private bool nextWave;

		private int oneGameCoins;

		private float birdSpawnTime;

		private bool birdSpawnAllowed;

		internal bool isGameEnded;

		private int enemyCount;

		private int spawnAttempt;

		private int enemySpawnTypes;

		private int enemyGroundSpawnTypes;

		private bool canSpawnGround;

		private int minAiLevel;

		private int maxAiLevel;

		private int minGroundAiLevel;

		private int maxGroundAiLevel;

		private float minYCoord;

		private float maxYCoord;

		private int updatedCatapultCount;

		private int gameDifficulty;

		private float checkRadius;

		private int upgradePerRound;

		private bool spawnWavePlayer;

		private int spawnedCatapultCount;

		private int spawnedGroundCatapultCount;

		private bool canUpgradeBigOne;

		private bool canUpgradeGroundOne;

		private bool nextSpawnBoss;

		private bool bossSpawned;

		private int bossStage;

		private float bossRewardSumm;

		public Transform catapultPoolParent;

		private GameObject[] catapultSmallStone;

		private GameObject[] catapultBigStone;

		private GameObject[] catapultSmallGround;

		private GameObject[] catapultBigGround;

		private GameObject[] catapultGround0;

		private GameObject[] catapultGround1;

		private GameObject[] catapultGround2;

		private GameObject[] catapultGround3;

		public BossLogic _currentBoss
		{
			get;
			private set;
		}

		public int _oneGameCoins
		{
			get
			{
				return oneGameCoins;
			}
			set
			{
				oneGameCoins = value;
				GameMenuControl.instance.SetOneGameCoins(oneGameCoins);
			}
		}

		public bool gameEndedByBoss
		{
			get;
			set;
		}

		public void BlockDonationPanel()
		{
			GameMenuControl.instance.donations.BlockPanel();
		}

		private void InitCatapultPools()
		{
			catapultSmallStone = new GameObject[3];
			catapultBigStone = new GameObject[3];
			catapultSmallGround = new GameObject[3];
			catapultBigGround = new GameObject[3];
			catapultGround0 = new GameObject[1];
			catapultGround1 = new GameObject[1];
			catapultGround2 = new GameObject[1];
			catapultGround3 = new GameObject[1];
		}

		private void FillCatapultPools()
		{
			GameObject original = Resources.Load<GameObject>("Prefabs/Enemy/" + enemysPlatformNames[0]);
			for (int i = 0; i < catapultSmallStone.Length; i++)
			{
				catapultSmallStone[i] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultSmallStone[i].GetComponentInChildren<CatapultLogic>().InitCatapultInPool();
				catapultSmallStone[i].SetActive(value: false);
			}
			original = Resources.Load<GameObject>("Prefabs/Enemy/" + enemysPlatformNames[1]);
			for (int j = 0; j < catapultSmallGround.Length; j++)
			{
				catapultSmallGround[j] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultSmallGround[j].GetComponentInChildren<CatapultLogic>().InitCatapultInPool();
				catapultSmallGround[j].SetActive(value: false);
			}
			original = Resources.Load<GameObject>("Prefabs/Enemy/" + enemysPlatformNames[2]);
			for (int k = 0; k < catapultBigStone.Length; k++)
			{
				catapultBigStone[k] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultBigStone[k].GetComponentInChildren<CatapultLogic>().InitCatapultInPool();
				catapultBigStone[k].SetActive(value: false);
			}
			original = Resources.Load<GameObject>("Prefabs/Enemy/" + enemysPlatformNames[3]);
			for (int l = 0; l < catapultBigGround.Length; l++)
			{
				catapultBigGround[l] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultBigGround[l].GetComponentInChildren<CatapultLogic>().InitCatapultInPool();
				catapultBigGround[l].SetActive(value: false);
			}
			original = Resources.Load<GameObject>("Prefabs/Enemy/" + groundEnemysPlatformNames[0]);
			for (int m = 0; m < catapultGround0.Length; m++)
			{
				catapultGround0[m] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultGround0[m].GetComponent<CatapultLogic>().InitCatapultInPool();
				catapultGround0[m].SetActive(value: false);
			}
			original = Resources.Load<GameObject>("Prefabs/Enemy/" + groundEnemysPlatformNames[1]);
			for (int n = 0; n < catapultGround1.Length; n++)
			{
				catapultGround1[n] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultGround1[n].GetComponent<CatapultLogic>().InitCatapultInPool();
				catapultGround1[n].SetActive(value: false);
			}
			original = Resources.Load<GameObject>("Prefabs/Enemy/" + groundEnemysPlatformNames[2]);
			for (int num = 0; num < catapultGround2.Length; num++)
			{
				catapultGround2[num] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultGround2[num].GetComponent<CatapultLogic>().InitCatapultInPool();
				catapultGround2[num].SetActive(value: false);
			}
			original = Resources.Load<GameObject>("Prefabs/Enemy/" + groundEnemysPlatformNames[3]);
			for (int num2 = 0; num2 < catapultGround3.Length; num2++)
			{
				catapultGround3[num2] = UnityEngine.Object.Instantiate(original, catapultPoolParent);
				catapultGround3[num2].GetComponent<CatapultLogic>().InitCatapultInPool();
				catapultGround3[num2].SetActive(value: false);
			}
		}

		private GameObject GetFreeCatapultFromPool(int enemyType, int globalType)
		{
			GameObject result = null;
			if (globalType == 0)
			{
				if (enemyType == 0)
				{
					for (int i = 0; i < catapultSmallStone.Length; i++)
					{
						if (!catapultSmallStone[i].activeSelf)
						{
							return catapultSmallStone[i];
						}
					}
				}
				if (enemyType == 1)
				{
					for (int j = 0; j < catapultSmallGround.Length; j++)
					{
						if (!catapultSmallGround[j].activeSelf)
						{
							return catapultSmallGround[j];
						}
					}
				}
				if (enemyType == 2)
				{
					for (int k = 0; k < catapultBigGround.Length; k++)
					{
						if (!catapultBigGround[k].activeSelf)
						{
							return catapultBigGround[k];
						}
					}
				}
				if (enemyType == 3)
				{
					for (int l = 0; l < catapultBigStone.Length; l++)
					{
						if (!catapultBigStone[l].activeSelf)
						{
							return catapultBigStone[l];
						}
					}
				}
			}
			if (globalType == 1)
			{
				if (enemyType == 0)
				{
					for (int m = 0; m < catapultGround0.Length; m++)
					{
						if (!catapultGround0[m].activeSelf)
						{
							return catapultGround0[m];
						}
					}
				}
				if (enemyType == 1)
				{
					for (int n = 0; n < catapultGround1.Length; n++)
					{
						if (!catapultGround1[n].activeSelf)
						{
							return catapultGround1[n];
						}
					}
				}
				if (enemyType == 2)
				{
					for (int num = 0; num < catapultGround2.Length; num++)
					{
						if (!catapultGround2[num].activeSelf)
						{
							return catapultGround2[num];
						}
					}
				}
				if (enemyType == 3)
				{
					for (int num2 = 0; num2 < catapultGround3.Length; num2++)
					{
						if (!catapultGround3[num2].activeSelf)
						{
							return catapultGround3[num2];
						}
					}
				}
			}
			return result;
		}

		private void ReturnCatapultToPool(CatapultLogic _catapult)
		{
			if (!_catapult.isGroundCatapult)
			{
				_catapult.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				_catapult.gameObject.SetActive(value: false);
			}
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			isGameEnded = false;
			InitController.instance.guiPressed = false;
			_currentProjectileSlot = -1;
			isPaused = false;
			nextWave = false;
			respawnTime = 1.5f;
			enemyCatapults = new List<CatapultLogic>();
			enemySpawnTypes = 2;
			enemyGroundSpawnTypes = 4;
			spawnAttempt = 0;
			spawnedCatapultCount = 0;
			spawnedGroundCatapultCount = 0;
			minYCoord = 0.2f;
			canUpgradeBigOne = false;
			canUpgradeGroundOne = false;
			InitCatapultPools();
			InitController.instance.ResetPoint();
			AchievementManager.instance.ResetOneRoundAchievements();
			AchievementManager.instance.PrepareRoundData();
			_oneGameCoins = 0;
			FillCatapultPools();
			GenerateWorld();
		}

		public void AddCoins(int value, bool isBonus = false)
		{
			if (NewDataController.instance.GetCurrentCatapultIndex() == 1 && value != 30 && !bossSpawned && !isBonus)
			{
				value *= 3;
			}
			_oneGameCoins += value;
			NewDataController.instance.AddMoney(value);
			menuControl.AddToPopup(value);
			menuControl.RefreshCoins();
		}

		public bool BuyProjectile(int value)
		{
			if (NewDataController.instance.IsEnoughMoney(value))
			{
				NewDataController.instance.CheckoutMoney(value);
				_oneGameCoins -= value;
				if (_oneGameCoins < 0)
				{
					_oneGameCoins = 0;
				}
				menuControl.RefreshCoins();
				return true;
			}
			return false;
		}

		private void ClearData()
		{
			birdSpawnTime = UnityEngine.Random.Range(55f, 65f);
			birdSpawnAllowed = true;
			nextWave = false;
			gameEndedByBoss = false;
			bossRewardSumm = 200f;
			GameMenuControl.instance.UpdateScore();
			enemyCount = 1;
			minAiLevel = 0;
			gameDifficulty = 1;
			maxAiLevel = 1;
			minGroundAiLevel = 0;
			maxGroundAiLevel = 0;
			updatedCatapultCount = 0;
			nextSpawnBoss = false;
			bossSpawned = false;
			_currentBoss = null;
			bossStage = 1;
			bossSpawnScore = 50;
			if (playerCatapult != null)
			{
				if (playerCatapult.transform.parent != null)
				{
					UnityEngine.Object.Destroy(playerCatapult.transform.parent.gameObject);
				}
				else
				{
					UnityEngine.Object.Destroy(playerCatapult.gameObject);
				}
			}
			playerCatapult = null;
			ClearEnemyData();
		}

		public void GenerateWorld()
		{
			newGame = true;
			ClearData();
			SpawnPlayer();
			InitSpawnCatapult();
		}

		private void ClearEnemyData()
		{
			for (int i = 0; i < enemyCatapults.Count; i++)
			{
				if (!(enemyCatapults[i] != null))
				{
					continue;
				}
				for (int j = 0; j < enemyCatapults[i]._stickmans.Length; j++)
				{
					if (enemyCatapults[i]._stickmans[j] != null)
					{
					}
				}
				ReturnCatapultToPool(enemyCatapults[i]);
			}
			enemyCatapults.Clear();
		}

		private void SpawnPlayer()
		{
			int currentCatapultIndex = NewDataController.instance.GetCurrentCatapultIndex();
			GameObject original = Resources.Load<GameObject>("Prefabs/Player/" + playersPlatformNames[currentCatapultIndex]);
			if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand)
			{
				playerCatapult = UnityEngine.Object.Instantiate(original, Camera.main.ViewportToWorldPoint(new Vector3(0.13f, 0.043f, 10f)), Quaternion.identity).GetComponentInChildren<CatapultLogic>();
			}
			else
			{
				playerCatapult = UnityEngine.Object.Instantiate(original, Camera.main.ViewportToWorldPoint(new Vector3(0.87f, 0.043f, 10f)), Quaternion.identity).GetComponentInChildren<CatapultLogic>();
			}
			playerCatapult.InitCatapultInPool();
			playerCatapult.InitCatapultIngame();
		}

		private void DetectLevelDifficulty()
		{
			if (InitController.instance.GetPoint() >= bossSpawnScore)
			{
				nextSpawnBoss = true;
				bossSpawnScore += 50;
			}
			if (InitController.instance.GetPoint() >= 10 && gameDifficulty == 1)
			{
				maxAiLevel = 0;
				updatedCatapultCount = 0;
				enemyCount = 2;
				gameDifficulty = 2;
				respawnTime = 1.5f;
			}
			if (InitController.instance.GetPoint() >= 20 && gameDifficulty == 2)
			{
				maxAiLevel = 5;
				minAiLevel = 1;
				updatedCatapultCount = 1;
				gameDifficulty = 3;
				respawnTime = 1f;
			}
			if (InitController.instance.GetPoint() >= 30 && gameDifficulty == 3)
			{
				updatedCatapultCount = 2;
				gameDifficulty = 4;
				respawnTime = 0.75f;
			}
			if (InitController.instance.GetPoint() >= 40 && gameDifficulty == 4)
			{
				enemyCount = 3;
				gameDifficulty = 5;
				enemySpawnTypes += 2;
			}
			if (InitController.instance.GetPoint() >= 50 && gameDifficulty == 5)
			{
				gameDifficulty = 6;
				minAiLevel = 5;
				maxAiLevel = 11;
			}
			if (InitController.instance.GetPoint() >= 60 && gameDifficulty == 6)
			{
				gameDifficulty = 7;
				canUpgradeGroundOne = true;
			}
			if (InitController.instance.GetPoint() >= 70 && gameDifficulty == 7)
			{
				gameDifficulty = 8;
				canUpgradeBigOne = true;
			}
		}

		public void DestroyPlayer(GameObject parentObject)
		{
			if (!isGameEnded)
			{
				isGameEnded = true;
				StartCoroutine(DestroyPlayerByBoss());
			}
		}

		private IEnumerator DestroyPlayerByBoss()
		{
			yield return new WaitForSeconds(2f);
			GameMenuControl.instance.EndGame();
		}

		public void ChangeProjectile(int nextType)
		{
			_currentProjectileSlot = nextType;
			playerCatapult.ChangeProjectileTypeAtAiming();
		}

		private void SpawnBird()
		{
			if (birdSpawnAllowed)
			{
				birdSpawnAllowed = false;
				int side = 1;
				float x = 1.15f;
				if (UnityEngine.Random.Range(-1f, 1f) < 0f)
				{
					side = -1;
					x = -0.15f;
				}
				float y = UnityEngine.Random.Range(0.73f, 0.9f);
				BirdBonusLogic component = UnityEngine.Object.Instantiate(birdPrefab, Camera.main.ViewportToWorldPoint(new Vector3(x, y, 10f)), Quaternion.identity).GetComponent<BirdBonusLogic>();
				component.Init(side);
				birdSpawnTime = UnityEngine.Random.Range(55f, 65f);
			}
		}

		public void ContinueGameAfterDeath()
		{
			ClearEnemyData();
			for (int i = 0; i < playerCatapult._stickmans.Length; i++)
			{
				if (playerCatapult._stickmans[i] != null)
				{
					UnityEngine.Object.Destroy(playerCatapult._stickmans[i].gameObject);
				}
			}
			if (playerCatapult._tower != null)
			{
				UnityEngine.Object.Destroy(playerCatapult._tower.transform.parent.gameObject);
			}
			if (playerCatapult != null)
			{
				UnityEngine.Object.Destroy(playerCatapult.gameObject);
			}
			if (_currentBoss != null)
			{
				UnityEngine.Object.Destroy(_currentBoss.gameObject);
			}
			ClearSpawnPlace(playerCatapult.transform.position);
			SpawnPlayer();
			IngameShop.instance.ReInitShield();
			spawnedCatapultCount = 0;
			spawnedGroundCatapultCount = 0;
			spawnWavePlayer = false;
			_ExplodeHandler.ClearList();
			respawnTick = -1.5f;
			if (nextSpawnBoss || bossSpawned)
			{
				nextSpawnBoss = true;
				bossSpawned = false;
			}
			GameMenuControl.instance.donations.UnBlockPanel();
			AchievementManager.instance.AchievementProgress(AchieventType.ContinueGame, AchievementRegion.AllGame, 1);
			isGameEnded = false;
		}

		private void ClearSpawnPlace(Vector3 position)
		{
			RaycastHit2D[] array = Physics2D.CircleCastAll(position, 20f, Vector2.zero);
			RaycastHit2D[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D = array2[i];
				if (raycastHit2D.collider != null && !raycastHit2D.collider.isTrigger)
				{
					if (raycastHit2D.collider.gameObject.tag == "CatapultParticle" || raycastHit2D.collider.gameObject.tag == "ProjectileParticle" || raycastHit2D.collider.gameObject.tag == "ExploderFragment")
					{
						raycastHit2D.collider.gameObject.SetActive(value: false);
					}
					else if (raycastHit2D.collider.gameObject.tag == "Projectile")
					{
						raycastHit2D.collider.GetComponent<Projectile>().ReturnToPool();
					}
					else if (raycastHit2D.collider.gameObject.tag == "Player")
					{
						raycastHit2D.collider.gameObject.SetActive(value: false);
					}
				}
			}
		}

		public void DestroyEnemy(CatapultLogic _catapult, GameObject parentObject)
		{
			AchievementManager.instance.AchievementProgress(AchieventType.KillEnemy, AchievementRegion.AllGame, 1);
			enemyCatapults.Remove(_catapult);
			if (_catapult._ai.catapultLevel != 1)
			{
				upgradePerRound--;
			}
			if (_catapult.isGroundCatapult)
			{
				spawnedGroundCatapultCount--;
				minYCoord = 0.2f;
			}
			ReturnCatapultToPool(_catapult);
			spawnedCatapultCount--;
			DetectLevelDifficulty();
			respawnTick = 0f;
		}

		public void DestroyBoss()
		{
			if (bossSpawned)
			{
				AddCoins(bossRewards[bossStage - 1]);
				switch (bossStage)
				{
				case 1:
					AchievementManager.instance.AchievementProgress(AchieventType.Boss1Killer, AchievementRegion.OneRound, 1);
					break;
				case 2:
					AchievementManager.instance.AchievementProgress(AchieventType.Boss2Killer, AchievementRegion.OneRound, 1);
					break;
				case 3:
					AchievementManager.instance.AchievementProgress(AchieventType.Boss3Killer, AchievementRegion.OneRound, 1);
					break;
				case 4:
					AchievementManager.instance.AchievementProgress(AchieventType.Boss4Killer, AchievementRegion.OneRound, 1);
					break;
				case 5:
					AchievementManager.instance.AchievementProgress(AchieventType.Boss5Killer, AchievementRegion.OneRound, 1);
					break;
				}
				StartCoroutine(BossAlertEnded());
				nextSpawnBoss = false;
				bossSpawned = false;
				bossStage++;
				if (bossStage > bossRewards.Length)
				{
					bossStage = bossRewards.Length;
				}
			}
		}

		private bool RaycastToTarget(Vector2 point)
		{
			bool result = true;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(point, Vector2.left);
			raycastHit2D = Physics2D.Raycast(point, Vector2.left);
			if (raycastHit2D.collider != null)
			{
				if (raycastHit2D.collider.gameObject.tag == "Platform")
				{
					result = false;
				}
				else if (raycastHit2D.collider.gameObject.tag == "Catapult")
				{
					if (raycastHit2D.collider.GetComponent<CatapultComponent>()._catapult.playerSide == GameSides.AI)
					{
						result = false;
					}
				}
				else if (raycastHit2D.collider.gameObject.tag == "Player" && raycastHit2D.collider.GetComponent<CharacterPart>()._character.playerSide == GameSides.AI)
				{
					result = false;
				}
			}
			return result;
		}

		private void InitSpawnCatapult()
		{
			if (nextSpawnBoss)
			{
				if (spawnedCatapultCount == 0 && !bossSpawned)
				{
					SpawnBoss();
				}
				return;
			}
			spawnWavePlayer = true;
			nextWave = false;
			maxYCoord = 0.75f;
			checkRadius = 5.5f;
			canSpawnGround = false;
			if (UnityEngine.Random.Range(0f, 1f) <= 0.6f && InitController.instance.GetPoint() >= 50 && spawnedGroundCatapultCount == 0)
			{
				bool flag = true;
				if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand)
				{
					flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.4f)));
					if (flag)
					{
						flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.3f)));
					}
					if (flag)
					{
						flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.2f)));
					}
					if (flag)
					{
						flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.1f)));
					}
				}
				else
				{
					flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.4f)));
					if (flag)
					{
						flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.3f)));
					}
					if (flag)
					{
						flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.2f)));
					}
					if (flag)
					{
						flag = RaycastToTarget(Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.1f)));
					}
				}
				if (flag)
				{
					minYCoord = 0.45f;
					canSpawnGround = true;
					SpawnGroundCatapult();
				}
				respawnTick = 0.5f;
				spawnWavePlayer = false;
			}
			else if (enemyCatapults.Count != enemyCount)
			{
				StartCoroutine(SpawnOneEnemy());
			}
			else
			{
				respawnTick = 0.5f;
				spawnWavePlayer = false;
			}
		}

		public void DetachNextSpawn()
		{
			respawnTick -= 1f;
		}

		private IEnumerator SpawnOneEnemy()
		{
			spawnAttempt = 0;
			int enemyType;
			Vector3 enemyPosition;
			GameObject enemy;
			while (true)
			{
				if (newGame)
				{
					UnityEngine.Random.InitState(0);
				}
				else
				{
					UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				}
				enemyType = UnityEngine.Random.Range(0, enemySpawnTypes);
				if (enemyType != 0)
				{
					checkRadius = 8f;
					maxYCoord = 0.7f;
				}
				enemyPosition = ((NewDataController.instance.GetPlayerControl() != 0) ? Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.07f, 0.55f), UnityEngine.Random.Range(minYCoord, maxYCoord), 10f)) : Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.45f, 0.93f), UnityEngine.Random.Range(minYCoord, maxYCoord), 10f)));
				bool isEmpty = true;
				RaycastHit2D[] coll = Physics2D.CircleCastAll(enemyPosition + new Vector3(0f, 2f, 0f), checkRadius, base.transform.forward);
				RaycastHit2D[] array = coll;
				for (int i = 0; i < array.Length; i++)
				{
					RaycastHit2D raycastHit2D = array[i];
					if (!raycastHit2D.collider.isTrigger)
					{
						if (raycastHit2D.collider.gameObject.tag != "CatapultParticle" && raycastHit2D.collider.gameObject.tag != "ProjectileParticle" && raycastHit2D.collider.gameObject.tag != "BirdBonus" && raycastHit2D.collider.gameObject.tag != "ExploderFragment")
						{
							isEmpty = false;
						}
						else if (raycastHit2D.collider.gameObject.tag == "CatapultParticle" || raycastHit2D.collider.gameObject.tag == "ProjectileParticle" || raycastHit2D.collider.gameObject.tag == "ExploderFragment")
						{
							raycastHit2D.collider.gameObject.SetActive(value: false);
						}
					}
				}
				if (spawnAttempt > 100)
				{
					UnityEngine.Debug.LogWarning("Trying to restart spawning");
					spawnAttempt = 0;
					DetectLevelDifficulty();
					yield return new WaitForSeconds(0.25f);
					continue;
				}
				if (!isEmpty)
				{
					spawnAttempt++;
					continue;
				}
				enemy = GetFreeCatapultFromPool(enemyType, 0);
				if (!(enemy == null))
				{
					break;
				}
				yield return new WaitForSeconds(0.5f);
			}
			enemy.transform.position = enemyPosition;
			if (enemyType == 0 || enemyType == 1 || ((enemyType == 2 || enemyType == 3) && canUpgradeBigOne))
			{
				if (updatedCatapultCount > upgradePerRound)
				{
					upgradePerRound++;
					enemy.GetComponentInChildren<CatapultAI>().Init(minAiLevel, maxAiLevel);
				}
				else
				{
					enemy.GetComponentInChildren<CatapultAI>().Init(0, 0);
				}
			}
			CatapultLogic lg = enemy.GetComponentInChildren<CatapultLogic>();
			lg.InitCatapultIngame();
			enemyCatapults.Add(lg);
			enemy.SetActive(value: true);
			spawnedCatapultCount++;
			newGame = false;
			respawnTick = 0.5f;
			spawnWavePlayer = false;
		}

		private void SpawnGroundCatapult()
		{
			if (!canSpawnGround)
			{
				return;
			}
			canSpawnGround = false;
			float num = 0.85f;
			Vector3 position;
			if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand)
			{
				position = Camera.main.ViewportToWorldPoint(new Vector3(1.07f, 0.07f, 10f));
				Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.7f, 0.85f), 0f, 10f));
				num = vector.x;
			}
			else
			{
				position = Camera.main.ViewportToWorldPoint(new Vector3(-0.07f, 0.07f, 10f));
				Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.15f, 0.3f), 0f, 10f));
				num = vector2.x;
			}
			UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			int enemyType = UnityEngine.Random.Range(0, enemyGroundSpawnTypes);
			GameObject freeCatapultFromPool = GetFreeCatapultFromPool(enemyType, 1);
			if (!(freeCatapultFromPool == null))
			{
				freeCatapultFromPool.transform.position = position;
				if (canUpgradeGroundOne)
				{
					freeCatapultFromPool.GetComponentInChildren<CatapultAI>().Init(minGroundAiLevel, maxGroundAiLevel);
				}
				else
				{
					freeCatapultFromPool.GetComponentInChildren<CatapultAI>().Init(0, 0);
				}
				enemyCatapults.Add(freeCatapultFromPool.GetComponent<CatapultLogic>());
				freeCatapultFromPool.GetComponent<CatapultLogic>().InitCatapultIngame();
				freeCatapultFromPool.GetComponent<CatapultLogic>().InitGround(num);
				freeCatapultFromPool.SetActive(value: true);
				spawnedCatapultCount++;
				spawnedGroundCatapultCount++;
			}
		}

		private void SpawnBoss()
		{
			StartCoroutine(BossIncomingAlert());
			Vector2 v = (NewDataController.instance.GetPlayerControl() != 0) ? ((Vector2)Camera.main.ViewportToWorldPoint(new Vector3(-0.17f, 0.15f, 10f))) : ((Vector2)Camera.main.ViewportToWorldPoint(new Vector3(1.17f, 0.15f, 10f)));
			GameObject original = Resources.Load<GameObject>("Prefabs/Boss/Boss");
			GameObject gameObject = UnityEngine.Object.Instantiate(original, v, Quaternion.identity);
			_currentBoss = gameObject.GetComponent<BossLogic>();
			gameObject.GetComponent<BossLogic>().PrepareBoss();
			gameObject.GetComponent<BossLogic>().InitBoss(bossStage);
			bossSpawned = true;
		}

		private IEnumerator BossIncomingAlert()
		{
			do
			{
				Color color = background.color;
				if (color.r > 0.6f)
				{
					SpriteRenderer spriteRenderer = background;
					Color color2 = background.color;
					float r = color2.r - 0.4f * Time.deltaTime;
					Color color3 = background.color;
					float g = color3.g - 0.4f * Time.deltaTime;
					Color color4 = background.color;
					spriteRenderer.color = new Color(r, g, color4.b - 0.4f * Time.deltaTime);
					yield return null;
					continue;
				}
				yield break;
			}
			while (!isGameEnded);
			background.color = new Color(1f, 1f, 1f);
		}

		private IEnumerator BossAlertEnded()
		{
			while (true)
			{
				Color color = background.color;
				if (!(color.r < 0.95f))
				{
					break;
				}
				SpriteRenderer spriteRenderer = background;
				Color color2 = background.color;
				float r = color2.r + 0.4f * Time.deltaTime;
				Color color3 = background.color;
				float g = color3.g + 0.4f * Time.deltaTime;
				Color color4 = background.color;
				spriteRenderer.color = new Color(r, g, color4.b + 0.4f * Time.deltaTime);
				yield return null;
			}
			background.color = new Color(1f, 1f, 1f);
		}

		private void Update()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (!isPaused && !isGameEnded)
			{
				if (birdSpawnTime > 0f)
				{
					birdSpawnTime -= Time.deltaTime;
				}
				else
				{
					birdSpawnAllowed = true;
					SpawnBird();
				}
			}
			if (!isGameEnded && !spawnWavePlayer && spawnedCatapultCount < enemyCount)
			{
				if (respawnTick < respawnTime)
				{
					respawnTick += Time.deltaTime;
				}
				else
				{
					InitSpawnCatapult();
				}
			}
		}
	}
}
