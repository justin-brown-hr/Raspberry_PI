using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
	public class Projectile : MonoBehaviour
	{
		public TrailRenderer _trailRenderer;

		protected CatapultLogic _singleCatapult;

		protected PvPCatapultLogic _pvpCatapult;

		protected BossStage _bossCannon;

		public GameObject _groundHitPrefab;

		protected Rigidbody2D _rigidbody;

		public ProjectileType projectileType;

		public GameSides launchFrom;

		protected bool isDestroyed;

		internal bool rotateAllowed;

		private int gameSlot;

		protected float rotatePower;

		protected bool returnToPoolCalled;

		private int bonucIncrement = 3;

		private List<int> killedCatapults;

		private float bonusTime;

		private bool canGetBonus;

		private float launchTime;

		public bool isLaunchedfromCatapult
		{
			get;
			private set;
		}

		public bool IsReflected
		{
			get;
			private set;
		}

		protected virtual void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			isLaunchedfromCatapult = false;
			isDestroyed = false;
		}

		private void Start()
		{
			if (_trailRenderer != null)
			{
				_trailRenderer.enabled = false;
			}
		}

		public void SetAsReflected()
		{
			IsReflected = true;
		}

		public void InitInPool(int inSlot)
		{
			if (_trailRenderer != null)
			{
				_trailRenderer.enabled = false;
			}
			killedCatapults = new List<int>();
			canGetBonus = true;
			gameSlot = inSlot;
		}

		public virtual void Init(Transform spawnPosition, CatapultLogic spawnCatapult)
		{
			if (spawnPosition != null && spawnCatapult != null)
			{
				isLaunchedfromCatapult = false;
				returnToPoolCalled = false;
				InitProjectileDefault(spawnPosition);
				launchFrom = spawnCatapult.playerSide;
				_singleCatapult = spawnCatapult;
				if (NewDataController.instance.GetGameMode() == GameMode.None)
				{
					killedCatapults = new List<int>();
				}
				killedCatapults.Clear();
			}
		}

		public virtual void Init(Transform spawnPosition, PvPCatapultLogic spawnCatapult)
		{
			if (spawnPosition != null && spawnCatapult != null)
			{
				InitProjectileDefault(spawnPosition);
				launchFrom = spawnCatapult.playerSide;
				_pvpCatapult = spawnCatapult;
			}
		}

		public virtual void Init(Transform spawnPosition, BossStage launchedStage)
		{
			if (launchedStage != null && spawnPosition != null)
			{
				InitProjectileDefault(spawnPosition);
				_bossCannon = launchedStage;
			}
		}

		private void InitProjectileDefault(Transform spawnPosition)
		{
			_singleCatapult = null;
			_pvpCatapult = null;
			GetComponent<Projectile>().enabled = true;
			base.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
			isDestroyed = false;
			base.gameObject.SetActive(value: true);
			_rigidbody.bodyType = RigidbodyType2D.Kinematic;
			_rigidbody.drag = 0f;
			base.gameObject.layer = 13;
			base.transform.position = spawnPosition.position;
			base.transform.SetParent(spawnPosition);
			rotateAllowed = false;
			rotatePower = UnityEngine.Random.Range(15f, 30f);
			ClearSpikes();
		}

		private void ClearSpikes()
		{
			if (projectileType != ProjectileType.StoneShoot)
			{
				ProjectileShootPart[] componentsInChildren = base.transform.GetComponentsInChildren<ProjectileShootPart>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
				}
			}
		}

		public virtual void Shoot(float shootDistance = 0f, bool isAdditional = false)
		{
			base.transform.parent = null;
			if (_trailRenderer != null)
			{
				_trailRenderer.enabled = true;
			}
			launchTime = 0.1f;
			isLaunchedfromCatapult = true;
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (_singleCatapult != null && !_singleCatapult.menuCatapult && _singleCatapult.playerSide == GameSides.Player1)
				{
					IngameShop.instance.StartRecharge(gameSlot);
				}
			}
			else if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen)
			{
				PvPIngameShop.instance.Shoot(launchFrom);
			}
			_rigidbody.bodyType = RigidbodyType2D.Dynamic;
			IsReflected = false;
			if (shootDistance > 0f)
			{
				rotateAllowed = true;
			}
		}

		public void ReturnToPoolDeleayed()
		{
			if (!returnToPoolCalled)
			{
				returnToPoolCalled = true;
				ReturnToPool(needNew: true);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public virtual void ReturnToPool(bool needNew = false)
		{
			if (_trailRenderer != null)
			{
				_trailRenderer.enabled = false;
			}
			if (GetComponent<GroundedDetail>() != null)
			{
				UnityEngine.Object.Destroy(GetComponent<GroundedDetail>());
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (IngameShop.instance != null)
				{
					IngameShop.instance.ReturnToPool(gameSlot, base.gameObject, needNew);
				}
				else
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen)
			{
				if (PvPIngameShop.instance != null)
				{
					PvPIngameShop.instance.ReturnToPool(base.gameObject, projectileType, needNew);
				}
				else
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		protected void CheckDestructionForPoints(Projectile hit)
		{
			if (canGetBonus && launchFrom == GameSides.Player1 && hit != null && _singleCatapult != null && NewDataController.instance.GetGameMode() == GameMode.Single && launchFrom != hit.launchFrom && GetComponent<GroundedDetail>() == null && hit.GetComponent<GroundedDetail>() == null && hit._singleCatapult != null && _singleCatapult._spoon != null && hit._singleCatapult._spoon != null)
			{
				bonusTime = 1f;
				canGetBonus = false;
				Vector2 vector = hit.transform.position;
				Vector2 vector2 = hit._singleCatapult._spoon.transform.position;
				float num = Mathf.Sqrt(Mathf.Pow(vector.x - vector2.x, 2f) + Mathf.Pow(vector.y - vector2.y, 2f));
				if (num > 5.5f)
				{
					GlobalLogic.instance.AddCoins(1);
					AchievementManager.instance.AchievementProgress(AchieventType.ProjectileDestroyMidair, AchievementRegion.AllGame, 1);
				}
			}
		}

		public void ProjectileKillCatapult(int catapultId)
		{
			if (NewDataController.instance.GetGameMode() == GameMode.Single && !killedCatapults.Contains(catapultId))
			{
				if (IsReflected)
				{
					AchievementManager.instance.AchievementProgress(AchieventType.ReflectKill, AchievementRegion.AllGame, 1);
				}
				if (killedCatapults.Count > 0)
				{
					GlobalLogic.instance.AddCoins(bonucIncrement, isBonus: true);
					GameMenuControl.instance.PopupBonusKill(killedCatapults.Count);
				}
				killedCatapults.Add(catapultId);
			}
		}

		private void FixedUpdate()
		{
			if (rotateAllowed)
			{
				rotatePower = UnityEngine.Random.Range(7f, 15f);
				if (projectileType == ProjectileType.StoneShoot)
				{
					rotatePower /= 3f;
				}
				_rigidbody.AddTorque(rotatePower, ForceMode2D.Impulse);
				rotateAllowed = false;
			}
			if (!canGetBonus)
			{
				if (bonusTime > 0f)
				{
					bonusTime -= Time.deltaTime;
				}
				else
				{
					canGetBonus = true;
				}
			}
		}
	}
}
