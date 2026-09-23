using System.Collections;
using UnityEngine;

namespace Logic
{
	public class BossStage : MonoBehaviour
	{
		public BossCharacter stageCharacter;

		public GameObject roofStage;

		public GameObject roofDestroyedStage;

		private Vector2 initialDestroyedRoofPosition;

		public BossComponent[] roofStageComponents;

		public GameObject activeStage;

		public GameObject activeDestroyedStage;

		private Vector2 initialDestroyedActivePosition;

		public BossComponent[] activeStageComponents;

		public GameObject[] catapultWheels;

		public BossComponent ramComponent;

		public BoxCollider2D explodeCollider;

		public GameObject partDestroyPrefab;

		public GameObject explosionGameobject;

		public ParticleSystem _topParticleSystem;

		private Transform _transform;

		private bool _isBaseStage;

		private float _movementPoint;

		private bool _isMoving;

		private bool _isCharging;

		private bool _isLastStage;

		private bool _isPaused;

		[SerializeField]
		private float _roofHealth;

		private bool roofKilled;

		[SerializeField]
		private float _activeHealth;

		private bool activeKilled;

		private bool _chargeSoundCalled;

		private bool _isPlayerTowerTouched;

		public bool testDestroy;

		private int _numberOfPauses;

		public BossLogic _bossLogic
		{
			get;
			private set;
		}

		public int _stageIndex
		{
			get;
			private set;
		}

		public bool _isDestroyed
		{
			get;
			private set;
		}

		public void PrepareBossStage(BossLogic bossLogic)
		{
			_isDestroyed = false;
			if (_topParticleSystem != null)
			{
				_topParticleSystem.Stop();
			}
			if (explodeCollider != null)
			{
				explodeCollider.enabled = false;
			}
			_isPlayerTowerTouched = false;
			_chargeSoundCalled = false;
			_bossLogic = bossLogic;
			_numberOfPauses = 0;
			_transform = base.gameObject.GetComponent<Transform>();
			if (stageCharacter != null)
			{
				stageCharacter.PrepareBossStageCharcter(this);
			}
			for (int i = 0; i < roofStageComponents.Length; i++)
			{
				roofStageComponents[i].PrepareBossComponent(this, isRoofComponent: true);
			}
			for (int j = 0; j < activeStageComponents.Length; j++)
			{
				activeStageComponents[j].PrepareBossComponent(this);
			}
			if (roofStage != null)
			{
				roofStage.SetActive(value: false);
				initialDestroyedRoofPosition = roofDestroyedStage.transform.localPosition;
			}
			activeStage.SetActive(value: false);
			initialDestroyedActivePosition = activeDestroyedStage.transform.localPosition;
			if (ramComponent != null)
			{
				ramComponent.SetupRam();
			}
		}

		public void InitBossStage(int stageIndex, bool isLastStage)
		{
			base.gameObject.SetActive(value: true);
			_stageIndex = stageIndex;
			if (_stageIndex == -1)
			{
				_isBaseStage = true;
			}
			_roofHealth = 0f;
			roofKilled = false;
			_activeHealth = 0f;
			activeKilled = false;
			activeStage.SetActive(value: true);
			for (int i = 0; i < activeStageComponents.Length; i++)
			{
				_activeHealth += activeStageComponents[i].InitBossStagePart(stageIndex);
			}
			_isLastStage = isLastStage;
			if (isLastStage)
			{
				roofStage.SetActive(value: true);
				for (int j = 0; j < roofStageComponents.Length; j++)
				{
					_roofHealth += roofStageComponents[j].InitBossStagePart(stageIndex);
				}
			}
		}

		public void InitMovement(float movementPoint)
		{
			if (!_isDestroyed)
			{
				_movementPoint = movementPoint;
				for (int i = 0; i < catapultWheels.Length; i++)
				{
					HingeJoint2D component = catapultWheels[i].GetComponent<HingeJoint2D>();
					JointMotor2D motor = component.motor;
					motor.motorSpeed = -300f;
					component.motor = motor;
					component.useMotor = true;
					catapultWheels[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
					FixedJoint2D component2 = catapultWheels[i].GetComponent<FixedJoint2D>();
					component2.enabled = false;
				}
				SoundMgr.instance.CatapultMoving();
				_isMoving = true;
			}
		}

		public void PauseMovement()
		{
			_isPaused = true;
			for (int i = 0; i < catapultWheels.Length; i++)
			{
				catapultWheels[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			}
			StartCoroutine(UnpauseMovement(1.5f));
		}

		private IEnumerator UnpauseMovement(float time)
		{
			yield return new WaitForSeconds(time);
			if (!_isMoving && !_isCharging)
			{
				yield break;
			}
			_isPaused = false;
			if (_numberOfPauses <= 0)
			{
				for (int i = 0; i < catapultWheels.Length; i++)
				{
					catapultWheels[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				}
			}
		}

		public void StopMovement()
		{
			_isMoving = false;
			_bossLogic._shootingAllowed = true;
			for (int i = 0; i < catapultWheels.Length; i++)
			{
				HingeJoint2D component = catapultWheels[i].GetComponent<HingeJoint2D>();
				component.useMotor = false;
				catapultWheels[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			}
			SoundMgr.instance.StopCatapultMoving();
			SoundMgr.instance.StopBossCharge();
		}

		public void StageCharge(bool allStagesDestroyed = true)
		{
			if (_isDestroyed)
			{
				return;
			}
			_isMoving = false;
			StartCoroutine(UnpauseMovement(0f));
			SoundMgr.instance.StopCatapultMoving();
			for (int i = 0; i < catapultWheels.Length; i++)
			{
				HingeJoint2D component = catapultWheels[i].GetComponent<HingeJoint2D>();
				JointMotor2D motor = component.motor;
				if (allStagesDestroyed)
				{
					motor.motorSpeed = -150f;
				}
				else
				{
					motor.motorSpeed = -50f;
				}
				component.motor = motor;
				component.useMotor = true;
				catapultWheels[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			}
			SoundMgr.instance.BossCharge();
			_chargeSoundCalled = true;
			if (allStagesDestroyed)
			{
				if (_activeHealth > 150f)
				{
					_activeHealth = 150f;
				}
			}
			else
			{
				_activeHealth /= 2f;
			}
			_isCharging = true;
		}

		public void StageComponentHit(float hitValue, bool isFromRoof = false, Vector3? projectilePosition = default(Vector3?))
		{
			if (isFromRoof)
			{
				_roofHealth -= hitValue;
			}
			else
			{
				_activeHealth -= hitValue;
			}
			if (_roofHealth <= 0f && _isLastStage && !roofKilled)
			{
				RoofDestroyed(fromProjectile: true, projectilePosition);
				SoundMgr.instance.WoodDestroy();
			}
			else if (_activeHealth <= 0f && !activeKilled)
			{
				ActiveDestroyed(fromProjectile: true, projectilePosition);
				SoundMgr.instance.WoodDestroy();
			}
		}

		public void RoofDestroyed(bool fromProjectile = false, Vector3? projectilePosition = default(Vector3?))
		{
			if (roofStage != null && _isLastStage && !roofKilled)
			{
				roofKilled = true;
				if (projectilePosition.HasValue)
				{
					Object.Instantiate(partDestroyPrefab, projectilePosition.Value, Quaternion.identity);
				}
				for (int i = 0; i < roofStageComponents.Length; i++)
				{
					roofStageComponents[i].ComponentDeath(fromProjectile, projectilePosition);
				}
			}
		}

		public void ActiveDestroyed(bool fromProjectile = false, Vector3? projectilePosition = default(Vector3?))
		{
			if (!activeKilled)
			{
				activeKilled = true;
				if (projectilePosition.HasValue)
				{
					Object.Instantiate(partDestroyPrefab, projectilePosition.Value, Quaternion.identity);
				}
				for (int i = 0; i < activeStageComponents.Length; i++)
				{
					activeStageComponents[i].ComponentDeath(fromProjectile, projectilePosition);
				}
				StageDeath();
			}
		}

		public void StageDeath()
		{
			if (!_isDestroyed)
			{
				_isDestroyed = true;
				SoundMgr.instance.StopCatapultMoving();
				SoundMgr.instance.StopBossCharge();
				_chargeSoundCalled = false;
				StopMovement();
				if (stageCharacter != null)
				{
					stageCharacter.StageDestroyed();
				}
				RoofDestroyed();
				ActiveDestroyed();
				_bossLogic.StageDestroyed(_stageIndex);
			}
		}

		public bool ChangeLayer(bool inverse = false)
		{
			for (int i = 0; i < activeStageComponents.Length; i++)
			{
				activeStageComponents[i].ChangeLayer(inverse);
			}
			if (!_isDestroyed)
			{
				return true;
			}
			return false;
		}

		public void TowerTouched(TowerLogic tower)
		{
			_isPlayerTowerTouched = true;
			_bossLogic.TowerTouched(tower);
			if (explodeCollider != null)
			{
				explodeCollider.enabled = true;
			}
		}

		public void StageExplode()
		{
			if (!_isDestroyed && _isPlayerTowerTouched)
			{
				SoundMgr.instance.BigBombExplosion();
				explosionGameobject.transform.parent = null;
				explosionGameobject.SetActive(value: true);
				UnityEngine.Object.Destroy(explosionGameobject, 5f);
				explodeCollider.gameObject.transform.parent = null;
				_bossLogic.DestroyPlayerTower();
				StageDeath();
				explodeCollider.GetComponent<CircleCollider2D>().enabled = true;
				explodeCollider.GetComponent<PointEffector2D>().enabled = true;
				UnityEngine.Object.Destroy(explodeCollider.gameObject, 0.2f);
			}
		}

		public void PlayEmitterSystem()
		{
			if (_topParticleSystem != null)
			{
				if (!_topParticleSystem.gameObject.activeSelf)
				{
					_topParticleSystem.gameObject.SetActive(value: true);
				}
				_topParticleSystem.Stop();
				_topParticleSystem.Play();
			}
		}

		private void Update()
		{
			if (testDestroy && UnityEngine.Input.GetKeyDown("2"))
			{
				StageDeath();
			}
			if (!_isBaseStage)
			{
				return;
			}
			if (_isMoving)
			{
				Vector3 position = catapultWheels[0].transform.position;
				if (position.x <= _movementPoint)
				{
					StopMovement();
				}
			}
			if (_isCharging && GlobalLogic.instance.isGameEnded)
			{
				StopMovement();
			}
		}
	}
}
