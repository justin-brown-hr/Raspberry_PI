using UnityEngine;

namespace Logic
{
	internal class PvPLogic : MonoBehaviour
	{
		public static PvPLogic instance;

		public GameObject[] playersPlatforms;

		public GameObject[] playersCatapults;

		public GameObject crowPrefab;

		public GameObject playerShield;

		public SpriteRenderer background;

		public PvPCatapultLogic firstPlayerCatapult;

		public PvPPlatformLogic firstPlayerPlatform;

		public PvPCatapultLogic secondPlayerCatapult;

		public PvPPlatformLogic secondPlayerPlatform;

		private int _firstPlayerScore;

		private int _secondPlayerScore;

		private bool crowSpawned;

		private float crowSpawnTime;

		private int endGameScore;

		internal bool isEndGame;

		private float endGameTick;

		private float endGameMaxTick = 1f;

		public bool prepareEndGame;

		private float endGameSlowMoTime;

		private bool slowMotionCalled;

		private int firstPlayerScore
		{
			get
			{
				return _firstPlayerScore;
			}
			set
			{
				_firstPlayerScore = value;
				if (PvPGameControl.instance != null)
				{
					PvPGameControl.instance.SetScore(GameSides.Player1, _firstPlayerScore);
				}
			}
		}

		private int secondPlayerScore
		{
			get
			{
				return _secondPlayerScore;
			}
			set
			{
				_secondPlayerScore = value;
				if (PvPGameControl.instance != null)
				{
					PvPGameControl.instance.SetScore(GameSides.Player2, _secondPlayerScore);
				}
			}
		}

		private void Awake()
		{
			instance = this;
			endGameScore = 20;
			firstPlayerScore = endGameScore;
			secondPlayerScore = endGameScore;
			crowSpawnTime = UnityEngine.Random.Range(20f, 30f);
			crowSpawned = false;
			isEndGame = false;
			endGameTick = 0f;
			SpawnInitialPlayers();
		}

		public void SpawnInitialPlayers()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(playersPlatforms[0], Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0.02f, 10f)), Quaternion.identity);
			firstPlayerPlatform = gameObject.transform.GetComponentInChildren<PvPPlatformLogic>();
			firstPlayerPlatform.InitPlatform(GameSides.Player1);
			firstPlayerCatapult = UnityEngine.Object.Instantiate(playersCatapults[0], firstPlayerPlatform.spawnPoint.position + new Vector3(7.4f, -0.55f, 0f), Quaternion.identity, firstPlayerPlatform.transform).GetComponentInChildren<PvPCatapultLogic>();
			GameObject shield = UnityEngine.Object.Instantiate(playerShield, firstPlayerPlatform.spawnPoint.position + new Vector3(0f, 3f, 0f), Quaternion.identity, firstPlayerCatapult.transform);
			firstPlayerCatapult.InitShield(shield);
			gameObject = UnityEngine.Object.Instantiate(playersPlatforms[1], Camera.main.ViewportToWorldPoint(new Vector3(0.965f, 0.02f, 10f)), Quaternion.identity);
			secondPlayerPlatform = gameObject.transform.GetComponentInChildren<PvPPlatformLogic>();
			secondPlayerPlatform.InitPlatform(GameSides.Player2);
			secondPlayerCatapult = UnityEngine.Object.Instantiate(playersCatapults[1], secondPlayerPlatform.spawnPoint.position + new Vector3(-7.4f, -0.55f, 0f), Quaternion.identity, secondPlayerPlatform.transform).GetComponentInChildren<PvPCatapultLogic>();
			shield = UnityEngine.Object.Instantiate(playerShield, secondPlayerPlatform.spawnPoint.position + new Vector3(0f, 3f, 0f), Quaternion.identity, secondPlayerCatapult.transform);
			secondPlayerCatapult.InitShield(shield);
		}

		private void ClearSpawnPlace(Vector3 position)
		{
			RaycastHit2D[] array = Physics2D.CircleCastAll(position, 6f, Vector2.zero);
			RaycastHit2D[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D = array2[i];
				if (!raycastHit2D.collider.isTrigger)
				{
					if (raycastHit2D.collider.gameObject.tag == "CatapultParticle" || raycastHit2D.collider.gameObject.tag == "ProjectileParticle" || raycastHit2D.collider.gameObject.tag == "Catapult" || raycastHit2D.collider.gameObject.tag == "ExploderFragment")
					{
						UnityEngine.Object.Destroy(raycastHit2D.collider.gameObject);
					}
					else if (raycastHit2D.collider.gameObject.tag == "Projectile")
					{
						raycastHit2D.collider.GetComponent<Projectile>().ReturnToPool();
					}
				}
			}
			array = Physics2D.CircleCastAll(position, 15f, Vector2.zero);
			RaycastHit2D[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				RaycastHit2D raycastHit2D2 = array3[j];
				if (!raycastHit2D2.collider.isTrigger && raycastHit2D2.collider.gameObject.tag == "Player")
				{
					raycastHit2D2.collider.GetComponent<CharacterPart>()._character.OutOfBounds();
				}
			}
		}

		public void PlatformReady(GameSides side)
		{
			if (isEndGame)
			{
				return;
			}
			switch (side)
			{
			case GameSides.Player1:
				if (firstPlayerCatapult == null)
				{
					ClearSpawnPlace(firstPlayerPlatform.spawnPoint.position);
					firstPlayerPlatform.InitPlatform(GameSides.Player1);
					firstPlayerCatapult = UnityEngine.Object.Instantiate(playersCatapults[0], firstPlayerPlatform.spawnPoint.position + new Vector3(7.2f, -0.55f, 0f), Quaternion.identity, firstPlayerPlatform.transform).GetComponentInChildren<PvPCatapultLogic>();
					GameObject shield2 = UnityEngine.Object.Instantiate(playerShield, firstPlayerPlatform.spawnPoint.position + new Vector3(0f, 3f, 0f), Quaternion.identity, firstPlayerCatapult.transform);
					firstPlayerCatapult.InitShield(shield2);
				}
				break;
			case GameSides.Player2:
				if (secondPlayerCatapult == null)
				{
					ClearSpawnPlace(secondPlayerPlatform.spawnPoint.position);
					secondPlayerPlatform.InitPlatform(GameSides.Player2);
					secondPlayerCatapult = UnityEngine.Object.Instantiate(playersCatapults[1], secondPlayerPlatform.spawnPoint.position + new Vector3(-7.2f, -0.55f, 0f), Quaternion.identity, secondPlayerPlatform.transform).GetComponentInChildren<PvPCatapultLogic>();
					GameObject shield = UnityEngine.Object.Instantiate(playerShield, secondPlayerPlatform.spawnPoint.position + new Vector3(0f, 3f, 0f), Quaternion.identity, secondPlayerCatapult.transform);
					secondPlayerCatapult.InitShield(shield);
				}
				break;
			}
		}

		public void PlayerKilled(GameSides side)
		{
			PvPIngameShop.instance.ClearPool(side);
			switch (side)
			{
			case GameSides.Player1:
				firstPlayerScore--;
				if (firstPlayerScore == 0)
				{
					isEndGame = true;
				}
				PvPIngameShop.instance.ResetPoints(GameSides.Player1);
				UnityEngine.Object.Destroy(firstPlayerCatapult.transform.parent.gameObject);
				firstPlayerCatapult = null;
				firstPlayerPlatform.Respawn();
				break;
			case GameSides.Player2:
				secondPlayerScore--;
				if (secondPlayerScore == 0)
				{
					isEndGame = true;
				}
				PvPIngameShop.instance.ResetPoints(GameSides.Player2);
				UnityEngine.Object.Destroy(secondPlayerCatapult.transform.parent.gameObject);
				secondPlayerCatapult = null;
				secondPlayerPlatform.Respawn();
				break;
			}
		}

		public bool CheckLastKill(GameSides playerSide)
		{
			switch (playerSide)
			{
			case GameSides.Player1:
				if (firstPlayerScore == 1)
				{
					prepareEndGame = true;
				}
				break;
			case GameSides.Player2:
				if (secondPlayerScore == 1)
				{
					prepareEndGame = true;
				}
				break;
			}
			if (prepareEndGame)
			{
				Time.timeScale = 0.5f;
				endGameSlowMoTime = 1f;
				slowMotionCalled = false;
			}
			return prepareEndGame;
		}

		private void SpawnCrow()
		{
			if (!crowSpawned)
			{
				crowSpawned = true;
				int side = 1;
				float x = 1.15f;
				if (UnityEngine.Random.Range(-1f, 1f) < 0f)
				{
					side = -1;
					x = -0.15f;
				}
				float y = UnityEngine.Random.Range(0.75f, 0.8f);
				BirdBonusLogic component = UnityEngine.Object.Instantiate(crowPrefab, Camera.main.ViewportToWorldPoint(new Vector3(x, y, 10f)), Quaternion.identity).GetComponent<BirdBonusLogic>();
				component.Init(side);
				crowSpawnTime = UnityEngine.Random.Range(20f, 30f);
			}
		}

		private void EndGame()
		{
			if (firstPlayerScore == secondPlayerScore)
			{
				PvPGameControl.instance.GameOver(0);
			}
			else if (firstPlayerScore > secondPlayerScore)
			{
				PvPGameControl.instance.GameOver(1);
				background.color = new Color(0.5f, 0.5f, 0.5f);
				firstPlayerPlatform.Winning();
				if (firstPlayerCatapult != null)
				{
					firstPlayerCatapult.EndGameShoot();
				}
			}
			else if (firstPlayerScore < secondPlayerScore)
			{
				PvPGameControl.instance.GameOver(2);
				background.color = new Color(0.5f, 0.5f, 0.5f);
				secondPlayerCatapult.EndGameShoot();
				if (secondPlayerPlatform != null)
				{
					secondPlayerPlatform.Winning();
				}
			}
		}

		public void PauseExit()
		{
			if (firstPlayerCatapult != null)
			{
				firstPlayerCatapult.EndGameShoot();
			}
			if (secondPlayerCatapult != null)
			{
				secondPlayerCatapult.EndGameShoot();
			}
		}

		private void Update()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (prepareEndGame && !slowMotionCalled)
			{
				if (endGameSlowMoTime > 0f)
				{
					endGameSlowMoTime -= Time.deltaTime;
				}
				else
				{
					Time.timeScale = 1f;
					slowMotionCalled = true;
				}
			}
			if (isEndGame)
			{
				if (endGameTick < endGameMaxTick)
				{
					endGameTick += 1f;
				}
				else
				{
					EndGame();
				}
			}
			else if (crowSpawnTime > 0f)
			{
				if (crowSpawned)
				{
					crowSpawned = false;
				}
				crowSpawnTime -= Time.deltaTime;
			}
			else
			{
				SpawnCrow();
			}
		}
	}
}
