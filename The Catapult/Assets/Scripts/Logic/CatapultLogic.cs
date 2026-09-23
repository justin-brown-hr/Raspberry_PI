using Destructible2D;
using DG.Tweening;
using Model;
using System;
using UnityEngine;

namespace Logic
{
	public class CatapultLogic : MonoBehaviour
	{
		public CatapultComponent _spoon;

		public CharacterLogic[] _stickmans;

		internal CatapultAI _ai;

		public GameObject[] catapultWheels;

		public GameObject _tower;

		public Transform spoonShieldPosition;

		public Transform _projectilePosition;

		internal Projectile _projectile;

		public Transform _additionalProjectilePosition;

		internal Projectile _additionalProjectile;

		public GameSides playerSide;

		internal bool beingDestroyed;

		private float destroyTime;

		private int stickmansCount;

		private Quaternion initialSpoonRotation;

		private bool swipeStarted;

		private Vector2 startPos;

		private Vector2 endPos;

		private float distance;

		private float minus;

		private int controllFinger;

		private bool spawnAllowed;

		private bool projectileSpawned;

		public bool menuCatapult;

		public GameObject projPrefab;

		private int criticalCount;

		private bool criticalEffectEnabled;

		private float criticalTime;

		private bool criticalTimerStarted;

		private bool isPoisoned;

		private bool isTargetSelected;

		private float waitTime;

		private float maxWaitTime;

		private float timeWastedForAiming;

		public bool isGroundCatapult;

		public bool isMoving;

		private float movePosition;

		public GameObject projPath;

		private GameObject[] currentAim;

		private int aimStep;

		internal PvEShield _currentShield;

		internal bool isShielded;

		public GameObject[] componentsToHeal;

		private float healTime;

		private bool isHealing;

		private Quaternion baseInitialRotation;

		public CatapultComponent[] allCatapultComponents;

		public bool destroyEffectCalled
		{
			get;
			set;
		}

		public void InitCatapultInPool()
		{
			if (_spoon != null)
			{
				initialSpoonRotation = _spoon.transform.rotation;
			}
			for (int i = 0; i < _stickmans.Length; i++)
			{
				_stickmans[i].InitCharacter();
			}
			for (int j = 0; j < allCatapultComponents.Length; j++)
			{
				allCatapultComponents[j].InitPartInPool();
			}
			baseInitialRotation = allCatapultComponents[0].transform.rotation;
		}

		public void InitCatapultIngame()
		{
			destroyEffectCalled = false;
			for (int i = 0; i < componentsToHeal.Length; i++)
			{
				if (componentsToHeal[i].GetComponent<D2dHealDamage>() != null)
				{
					componentsToHeal[i].GetComponent<D2dHealDamage>().enabled = true;
				}
			}
			healTime = 0.25f;
			isHealing = true;
			if (!isGroundCatapult)
			{
				for (int j = 0; j < catapultWheels.Length; j++)
				{
					if (catapultWheels[j] != null)
					{
						catapultWheels[j].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
					}
				}
			}
			for (int k = 0; k < allCatapultComponents.Length; k++)
			{
				allCatapultComponents[k].ReinitPart();
			}
			for (int l = 0; l < _stickmans.Length; l++)
			{
				_stickmans[l].ResetPlayer();
			}
			if (_spoon != null)
			{
				_spoon.GetComponent<SpringJoint2D>().enabled = true;
				_spoon.transform.rotation = initialSpoonRotation;
			}
		}

		public void ResetDestroy()
		{
			for (int i = 0; i < allCatapultComponents.Length; i++)
			{
				allCatapultComponents[i].ReinitPart();
			}
			for (int j = 0; j < allCatapultComponents.Length; j++)
			{
				allCatapultComponents[j].ResetDestroy();
			}
			ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[k].gameObject);
			}
			ResetCatapultData();
		}

		private void Awake()
		{
			if (playerSide == GameSides.AI)
			{
				_ai = GetComponent<CatapultAI>();
			}
			currentAim = new GameObject[17];
			ResetCatapultData();
		}

		private void ResetCatapultData()
		{
			timeWastedForAiming = 0f;
			controllFinger = -1;
			distance = 0f;
			isTargetSelected = false;
			waitTime = 0f;
			if (isGroundCatapult)
			{
				waitTime = -4f;
			}
			criticalCount = 0;
			criticalEffectEnabled = false;
			criticalTime = 0f;
			criticalTimerStarted = false;
			isPoisoned = false;
			beingDestroyed = false;
			destroyTime = 0f;
			swipeStarted = false;
			startPos = Vector3.zero;
			endPos = Vector3.zero;
			minus = -70f;
			projectileSpawned = false;
			_projectile = null;
			_additionalProjectile = null;
		}

		private void Start()
		{
			for (int i = 0; i < _stickmans.Length; i++)
			{
				_stickmans[i].playerSide = playerSide;
			}
			if (playerSide == GameSides.AI)
			{
				isTargetSelected = false;
				maxWaitTime = _ai.GetShootTime();
			}
			stickmansCount = _stickmans.Length;
			if (NewDataController.instance.GetPlayerControl() == ControlType.RightHand && NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				Transform transform = base.transform;
				Vector3 position = base.transform.position;
				float x = position.x;
				Vector3 position2 = base.transform.position;
				float y = position2.y;
				Vector3 position3 = base.transform.position;
				transform.position = new Vector3(x, y, -1f * position3.z);
			}
			if (playerSide != GameSides.AI)
			{
				for (int j = 0; j < currentAim.Length; j++)
				{
					currentAim[j] = UnityEngine.Object.Instantiate(projPath, base.transform.position, Quaternion.identity, base.transform);
					Transform transform2 = currentAim[j].transform;
					Vector3 localScale = currentAim[aimStep].transform.localScale;
					float x2 = localScale.x - 0.02f * (float)j;
					Vector3 localScale2 = currentAim[aimStep].transform.localScale;
					transform2.localScale = new Vector2(x2, localScale2.y - 0.02f * (float)j);
					currentAim[j].SetActive(value: false);
				}
			}
		}

		public void InitGround(float position)
		{
			isMoving = true;
			isMoving = false;
			allCatapultComponents[0].transform.rotation = baseInitialRotation;
			allCatapultComponents[0].GetComponent<Rigidbody2D>().freezeRotation = false;
			for (int i = 0; i < catapultWheels.Length; i++)
			{
				catapultWheels[i].GetComponent<FixedJoint2D>().enabled = false;
				catapultWheels[i].GetComponent<HingeJoint2D>().enabled = true;
			}
			waitTime = -4f;
			if (!beingDestroyed)
			{
				SoundMgr.instance.CatapultMoving();
			}
			movePosition = position;
		}

		public void PartDestroyed(bool isCritical, bool isSpoon = false, CharacterLogic _character = null)
		{
			if (beingDestroyed)
			{
				return;
			}
			if (isCritical && !criticalEffectEnabled && playerSide == GameSides.Player1)
			{
				if (criticalCount == 0)
				{
					criticalTime = 1f;
					criticalTimerStarted = true;
				}
				criticalCount++;
				if (criticalCount >= 2)
				{
					Camera.main.DOShakePosition(0.75f, 0.5f, 1);
					criticalEffectEnabled = true;
				}
			}
			if (_character != null)
			{
				for (int i = 0; i < _stickmans.Length; i++)
				{
					if (_stickmans[i] != null && _stickmans[i] == _character)
					{
						stickmansCount--;
					}
				}
			}
			if (stickmansCount <= 0)
			{
				isCritical = true;
			}
			else
			{
				bool flag = false;
				for (int j = 0; j < _stickmans.Length; j++)
				{
					if (_stickmans[j] != null && !_stickmans[j].isKilled && _stickmans[j].kinematicParts[0] != null)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					isCritical = true;
				}
			}
			if (!isCritical || beingDestroyed)
			{
				return;
			}
			ClearAim();
			beingDestroyed = true;
			for (int k = 0; k < componentsToHeal.Length; k++)
			{
				if (componentsToHeal[k].GetComponent<D2dHealDamage>() != null)
				{
					componentsToHeal[k].GetComponent<D2dHealDamage>().enabled = false;
				}
			}
			GlobalLogic.instance.DetachNextSpawn();
			for (int l = 0; l < _stickmans.Length; l++)
			{
				if (_stickmans[l] != null)
				{
					_stickmans[l].Escape();
				}
			}
			if (isGroundCatapult)
			{
				for (int m = 0; m < catapultWheels.Length; m++)
				{
					if (catapultWheels[m] != null)
					{
						catapultWheels[m].GetComponent<CatapultComponent>().ComponentHit(1000f);
						SoundMgr.instance.StopCatapultMoving();
					}
				}
			}
			FreeCatapult();
			_spoon.GetComponent<SpringJoint2D>().enabled = false;
			if (playerSide == GameSides.Player1)
			{
				GlobalLogic.instance.BlockDonationPanel();
				destroyTime = 2f;
				return;
			}
			if (!GlobalLogic.instance.isGameEnded)
			{
				InitController.instance.AddPoint();
				GameMenuControl.instance.UpdateScore();
				GlobalLogic.instance.AddCoins(1);
			}
			if (_spoon != null)
			{
				_spoon.SpoonConnectdestroy();
			}
			if (isSpoon)
			{
				destroyTime = 1.25f;
			}
			else
			{
				destroyTime = 2f;
			}
		}

		internal void CatapultDeath()
		{
			beingDestroyed = true;
			for (int i = 0; i < catapultWheels.Length; i++)
			{
				catapultWheels[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			}
			ClearAim();
			for (int j = 0; j < _stickmans.Length; j++)
			{
				if (_stickmans[j] != null)
				{
					_stickmans[j].StickmanDeath();
				}
			}
			GameObject parentObject = null;
			if (base.transform.parent != null)
			{
				parentObject = base.transform.parent.gameObject;
			}
			ShieldDestroy();
			if (playerSide == GameSides.AI)
			{
				ResetDestroy();
			}
			if (playerSide == GameSides.AI)
			{
				GlobalLogic.instance.DestroyEnemy(this, parentObject);
			}
			else
			{
				GlobalLogic.instance.DestroyPlayer(parentObject);
			}
		}

		private void ShieldDestroy()
		{
			if (_currentShield != null)
			{
				_currentShield.DestroyShield();
			}
		}

		public void PlayerTowerDestroyed()
		{
			CatapultDeath();
		}

		public void ChangeProjectileTypeAtAiming()
		{
			if (_projectile != null)
			{
				_projectile.ReturnToPool();
				_projectile = null;
				projectileSpawned = false;
				BorrowProjectile();
				if (_projectile != null)
				{
					_projectile.Init(_projectilePosition, this);
					projectileSpawned = true;
				}
			}
		}

		private void BorrowProjectile()
		{
			if ((isGroundCatapult && !_spoon.GetComponent<Renderer>().isVisible) || (projectileSpawned && playerSide == GameSides.Player1))
			{
				return;
			}
			if (menuCatapult)
			{
				_projectile = UnityEngine.Object.Instantiate(projPrefab, _projectilePosition.position, Quaternion.identity).GetComponent<Projectile>();
				_projectile._trailRenderer.enabled = false;
				_projectile.transform.localScale = new Vector2(0.7f, 0.7f);
				_projectile.gameObject.SetActive(value: true);
				if (_additionalProjectilePosition != null)
				{
					_additionalProjectile = UnityEngine.Object.Instantiate(projPrefab, _additionalProjectilePosition.position, Quaternion.identity).GetComponent<Projectile>();
					_additionalProjectile.transform.localScale = new Vector2(0.7f, 0.7f);
					_additionalProjectile.gameObject.SetActive(value: true);
				}
			}
			else if (playerSide != GameSides.AI)
			{
				if (!InitController.instance.guiPressed)
				{
					GameObject gameObject = IngameShop.instance.BorrowProjectile(GlobalLogic.instance._currentProjectileSlot, requestFromPlayer: true);
					if (gameObject != null)
					{
						_projectile = gameObject.GetComponent<Projectile>();
					}
				}
			}
			else
			{
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
				GameObject gameObject2 = null;
				gameObject2 = (((!(UnityEngine.Random.Range(0f, 1f) < _ai.enemyBonusChance) || NewDataController.instance.GetEquipedShield() == 999 || InitController.instance.GetPoint() <= 50) && !GameMenuControl.instance.isAlwaysSteel) ? IngameShop.instance.BorrowProjectile(-1) : IngameShop.instance.BorrowProjectile(3));
				if (gameObject2 != null)
				{
					_projectile = gameObject2.GetComponent<Projectile>();
				}
			}
		}

		public void FreeCatapult()
		{
			if (_projectile != null)
			{
				_projectile.Shoot();
				if (playerSide == GameSides.Player1)
				{
					_projectile.launchFrom = GameSides.AI;
				}
				else
				{
					_projectile.launchFrom = GameSides.Player1;
				}
			}
			if (_additionalProjectile != null)
			{
				_additionalProjectile.Shoot();
				if (playerSide == GameSides.Player1)
				{
					_additionalProjectile.launchFrom = GameSides.AI;
				}
				else
				{
					_additionalProjectile.launchFrom = GameSides.Player1;
				}
			}
			_projectile = null;
			_additionalProjectile = null;
			_spoon.GetComponent<SpringJoint2D>().enabled = true;
			swipeStarted = false;
			waitTime = 0f;
			distance = 0f;
		}

		private void CheckSpringPower()
		{
			if (distance <= 6.5f)
			{
				_spoon.GetComponent<SpringJoint2D>().frequency = 3.2f;
			}
			else if (distance >= 12f)
			{
				_spoon.GetComponent<SpringJoint2D>().frequency = 4.3f;
			}
			else
			{
				_spoon.GetComponent<SpringJoint2D>().frequency = 3f + (distance - 1f) * 0.1f;
			}
		}

		private void Aiming()
		{
			if (Time.timeScale == 0f || beingDestroyed || _spoon == null || _projectile == null || NewDataController.instance.GetGameMode() != GameMode.Single || GlobalLogic.instance.isGameEnded || !(_projectile != null) || !(distance >= 0.5f) || menuCatapult || NewDataController.instance.GetPlayerAiming() != AimingTipType.Dot)
			{
				return;
			}
			Vector2 vector = _projectile.transform.position;
			float num = Time.fixedDeltaTime / (float)Physics2D.velocityIterations;
			Vector2 b = Physics2D.gravity * _projectile.GetComponent<Rigidbody2D>().gravityScale * num * num;
			float d = 1f - num * _projectile.GetComponent<Rigidbody2D>().drag;
			Vector2 a = GameMath.GetShootVector(playerSide, distance) * num;
			float mass = _projectile.GetComponent<Rigidbody2D>().mass;
			a /= mass;
			aimStep = 0;
			for (int i = 0; i < 455; i++)
			{
				a += b;
				a *= d;
				vector += a;
				if (i % 35 != 0 || aimStep >= currentAim.Length)
				{
					continue;
				}
				if (aimStep != 0 && currentAim[aimStep] != null)
				{
					if (!currentAim[aimStep].activeSelf)
					{
						currentAim[aimStep].SetActive(value: true);
					}
					currentAim[aimStep].transform.position = vector;
				}
				aimStep++;
			}
		}

		private void ClearAim()
		{
			for (int i = 0; i < currentAim.Length; i++)
			{
				if (currentAim[i] != null)
				{
					currentAim[i].SetActive(value: false);
				}
			}
		}

		public void InitShield(GameObject shield)
		{
			if (shield.GetComponent<PvEShield>()._shieldType == ShieldTypes.SpoonDestroy || shield.GetComponent<PvEShield>()._shieldType == ShieldTypes.SpoonReflect)
			{
				_currentShield = UnityEngine.Object.Instantiate(shield, _spoon.transform).GetComponent<PvEShield>();
				_currentShield.transform.position = spoonShieldPosition.position;
			}
			else
			{
				_currentShield = UnityEngine.Object.Instantiate(shield, base.transform.position + new Vector3(-0.5f, 3f), Quaternion.identity).GetComponent<PvEShield>();
			}
			_currentShield.Init(this);
		}

		public void ActivateShield()
		{
			if (_currentShield != null)
			{
				_currentShield.Activate();
				isShielded = true;
			}
		}

		public void DeactivateShield()
		{
			isShielded = false;
			_currentShield.Deactivate();
		}

		public void PosionCatapult(GameObject initialParticle = null, GameObject playerParticle = null)
		{
			if (beingDestroyed || isPoisoned)
			{
				return;
			}
			isPoisoned = true;
			for (int i = 0; i < _stickmans.Length; i++)
			{
				if (_stickmans[i] != null)
				{
					_stickmans[i].PoisonStickman(playerParticle);
				}
			}
		}

		private bool _isTouchBegan = false;
		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown("3") && playerSide == GameSides.AI)
			{
				PartDestroyed(isCritical: true);
			}
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (!menuCatapult && GlobalLogic.instance.isGameEnded)
			{
				if (!isGroundCatapult)
				{
					return;
				}
				for (int i = 0; i < catapultWheels.Length; i++)
				{
					if (catapultWheels[i] != null)
					{
						isMoving = false;
						catapultWheels[i].GetComponent<FixedJoint2D>().enabled = true;
						allCatapultComponents[0].transform.rotation = baseInitialRotation;
						allCatapultComponents[0].GetComponent<Rigidbody2D>().freezeRotation = true;
						SoundMgr.instance.StopCatapultMoving();
					}
				}
				return;
			}
			if (isHealing)
			{
				if (healTime > 0f)
				{
					healTime -= Time.deltaTime;
				}
				else
				{
					isHealing = false;
					for (int j = 0; j < componentsToHeal.Length; j++)
					{
						if (componentsToHeal[j].GetComponent<D2dHealDamage>() != null)
						{
							componentsToHeal[j].GetComponent<D2dHealDamage>().enabled = false;
						}
					}
				}
			}
			if (beingDestroyed)
			{
				if (destroyTime < 4.5f)
				{
					destroyTime += Time.deltaTime;
				}
				else if (base.transform.parent != null || isGroundCatapult)
				{
					CatapultDeath();
				}
				return;
			}
			if (isGroundCatapult && !beingDestroyed)
			{
				if (catapultWheels[0] != null)
				{
					Vector3 position = catapultWheels[0].transform.position;
					if (position.x < movePosition)
					{
						for (int k = 0; k < catapultWheels.Length; k++)
						{
							if (catapultWheels[k] != null)
							{
								isMoving = false;
								catapultWheels[k].GetComponent<FixedJoint2D>().enabled = true;
								allCatapultComponents[0].transform.rotation = baseInitialRotation;
								allCatapultComponents[0].GetComponent<Rigidbody2D>().freezeRotation = true;
								SoundMgr.instance.StopCatapultMoving();
							}
						}
					}
				}
				else
				{
					for (int l = 0; l < catapultWheels.Length; l++)
					{
						if (catapultWheels[l] != null)
						{
							isMoving = false;
							catapultWheels[l].GetComponent<FixedJoint2D>().enabled = true;
							allCatapultComponents[0].transform.rotation = baseInitialRotation;
							allCatapultComponents[0].GetComponent<Rigidbody2D>().freezeRotation = true;
							SoundMgr.instance.StopCatapultMoving();
						}
					}
				}
			}
			if (criticalTimerStarted)
			{
				if (criticalTime > 0f)
				{
					criticalTime -= Time.deltaTime;
				}
				else
				{
					criticalEffectEnabled = true;
				}
			}
			if (playerSide == GameSides.Player1)
			{
				if (waitTime < 0.2f)
				{
					waitTime += Time.deltaTime;
				}
				else
				{
					// A connected gamepad drives the same synthetic touch the mouse path
					// builds, so the aiming logic below is shared by all three inputs.
					bool usePad = GamepadAim.Connected;
					if (usePad || Application.isEditor)
                    {
						Touch touch = new Touch();
						if (usePad)
						{
							if (!GamepadAim.TryGetTouch(base.transform, Camera.main, out touch))
							{
								return;
							}
						}
						else
						{
                        touch.position = Input.mousePosition;
						if (Input.GetMouseButtonDown(0))
                        {
                            touch.phase = TouchPhase.Began;
                            _isTouchBegan = true;
                        }
                        else if (Input.GetMouseButtonUp(0))
                        {
                            touch.phase = TouchPhase.Ended;
                            _isTouchBegan = false;
                        }
                        else if (_isTouchBegan && Input.GetMouseButton(0))
                        {
                            touch.phase = TouchPhase.Moved;
                        }
						}

                        float num = 10000f;
                        if (!swipeStarted && touch.phase == TouchPhase.Began)
                        {
                            float num2 = Vector2.Distance(touch.position, base.transform.position);
                            if (num2 < num)
                            {
                                controllFinger = touch.fingerId;
                                Vector3 vector = Camera.main.ScreenToWorldPoint(touch.position);
                                startPos = new Vector2(vector.x, vector.y);
                                num = num2;
                                spawnAllowed = false;
                            }
                        }

                        
						
                        if (touch.phase == TouchPhase.Moved)
                        {
                            if (!spawnAllowed)
                            {
                                Vector3 vector2 = Camera.main.ScreenToWorldPoint(touch.position);
                                projectileSpawned = false;
                                endPos = new Vector2(vector2.x, vector2.y);
								if (!(Vector2.Distance(startPos, endPos) > 1f)) return;
                                spawnAllowed = true;
                            }
                            if (_projectile == null)
                            {
                                BorrowProjectile();
                                if (_projectile != null)
                                {
                                    _projectile.Init(_projectilePosition, this);
                                    projectileSpawned = true;
                                    if (_additionalProjectilePosition != null && (_additionalProjectile == null || menuCatapult))
                                    {
                                        if (_additionalProjectilePosition.transform.childCount > 0)
                                        {
                                            Projectile component = _additionalProjectilePosition.transform.GetChild(0).GetComponent<Projectile>();
                                            if (component != null)
                                            {
                                                component.ReturnToPool();
                                            }
                                        }
                                        GameObject gameObject = null;
                                        if (!menuCatapult)
                                        {
                                            gameObject = IngameShop.instance.BorrowProjectile(-1, requestFromPlayer: true);
                                        }
                                        if (gameObject != null)
                                        {
                                            _additionalProjectile = gameObject.GetComponent<Projectile>();
                                            _additionalProjectile.Init(_additionalProjectilePosition, this);
                                        }
                                        else if (menuCatapult && _additionalProjectile != null)
                                        {
                                            _additionalProjectile.Init(_additionalProjectilePosition, this);
                                        }
                                    }
                                    for (int num3 = 0; num3 < _stickmans.Length; num3++)
                                    {
                                        if (_stickmans[num3] != null)
                                        {
                                            _stickmans[num3].Prepare();
                                        }
                                    }
                                    swipeStarted = true;
                                    Vector3 vector3 = Camera.main.ScreenToWorldPoint(touch.position);
                                    startPos = new Vector2(vector3.x, vector3.y);
                                    minus = -75f;
                                }
                                else
                                {
                                    swipeStarted = false;
                                }
                            }
                            if (swipeStarted)
                            {
                                _spoon.GetComponent<SpringJoint2D>().enabled = false;
                                Vector3 vector4 = Camera.main.ScreenToWorldPoint(touch.position);
                                endPos = new Vector2(vector4.x, vector4.y);
                                SoundMgr.instance.AimingSound();
                                distance = Vector2.Distance(startPos, endPos);
                            }
                        }

                        if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                            && swipeStarted && _projectile != null)
                        {
                            if (_projectile != null)
                            {
                                _projectile.Shoot(distance);
                            }
                            _projectile = null;
                            if (_additionalProjectile != null)
                            {
                                _additionalProjectile.Shoot(distance, isAdditional: true);
                            }
                            _additionalProjectile = null;
                            ClearAim();
                            SoundMgr.instance.CatapultLaunch(playerShoot: true);
                            for (int num4 = 0; num4 < _stickmans.Length; num4++)
                            {
                                if (_stickmans[num4] != null)
                                {
                                    _stickmans[num4].Shoot();
                                }
                            }
                            CheckSpringPower();
                            _spoon.GetComponent<SpringJoint2D>().enabled = true;
                            swipeStarted = false;
                            waitTime = 0f;
                            controllFinger = -1;
                            spawnAllowed = false;
                            distance = 0f;
                            minus = -70f;
                        }

						if (!swipeStarted) return;

                        if (distance < 1f)
                        {
                            distance = 1f;
                        }
                        if (distance > 12f)
                        {
                            distance = 12f;
                        }
                        if (distance >= 1f && distance <= 12f)
                        {
                            minus = GameMath.CalculateAngle(distance);
                        }
                        _spoon.transform.rotation = Quaternion.Euler(0f, 0f, minus);
                        for (int num5 = 0; num5 < _stickmans.Length; num5++)
                        {
                            if (_stickmans[num5] != null)
                            {
                                _stickmans[num5].NextTime(GameMath.DistanceToAnimation(distance) + (float)num5 * 0.2f);
                            }
                        }
                        Aiming();
                    }
                    else
                    {
                        if (UnityEngine.Input.touchCount <= 0)
                        {
                            return;
                        }
                        float num = 10000f;
                        for (int m = 0; m < UnityEngine.Input.touchCount; m++)
                        {
							Touch touch = UnityEngine.Input.GetTouch(m);
							if ((float)touch.fingerId != InitController.instance.guiPressedFingerId
                                && !swipeStarted && touch.phase == TouchPhase.Began)
                            {
                                float num2 = Vector2.Distance(touch.position, base.transform.position);
                                if (num2 < num)
                                {
                                    controllFinger = touch.fingerId;
                                    Vector3 vector = Camera.main.ScreenToWorldPoint(touch.position);
                                    startPos = new Vector2(vector.x, vector.y);
                                    num = num2;
                                    spawnAllowed = false;
                                }
                            }
                        }

                        for (int n = 0; n < UnityEngine.Input.touchCount; n++)
                        {
                            Touch touch = UnityEngine.Input.GetTouch(n);
                            if (touch.fingerId != controllFinger)
                            {
                                continue;
                            }
                            if (touch.phase == TouchPhase.Moved)
                            {
                                if (!spawnAllowed)
                                {
                                    Vector3 vector2 = Camera.main.ScreenToWorldPoint(touch.position);
                                    projectileSpawned = false;
                                    endPos = new Vector2(vector2.x, vector2.y);
                                    if (!(Vector2.Distance(startPos, endPos) > 1f))
                                    {
                                        break;
                                    }
                                    spawnAllowed = true;
                                }
                                if (_projectile == null)
                                {
                                    BorrowProjectile();
                                    if (_projectile != null)
                                    {
                                        _projectile.Init(_projectilePosition, this);
                                        projectileSpawned = true;
                                        if (_additionalProjectilePosition != null && (_additionalProjectile == null || menuCatapult))
                                        {
                                            if (_additionalProjectilePosition.transform.childCount > 0)
                                            {
                                                Projectile component = _additionalProjectilePosition.transform.GetChild(0).GetComponent<Projectile>();
                                                if (component != null)
                                                {
                                                    component.ReturnToPool();
                                                }
                                            }
                                            GameObject gameObject = null;
                                            if (!menuCatapult)
                                            {
                                                gameObject = IngameShop.instance.BorrowProjectile(-1, requestFromPlayer: true);
                                            }
                                            if (gameObject != null)
                                            {
                                                _additionalProjectile = gameObject.GetComponent<Projectile>();
                                                _additionalProjectile.Init(_additionalProjectilePosition, this);
                                            }
                                            else if (menuCatapult && _additionalProjectile != null)
                                            {
                                                _additionalProjectile.Init(_additionalProjectilePosition, this);
                                            }
                                        }
                                        for (int num3 = 0; num3 < _stickmans.Length; num3++)
                                        {
                                            if (_stickmans[num3] != null)
                                            {
                                                _stickmans[num3].Prepare();
                                            }
                                        }
                                        swipeStarted = true;
                                        Vector3 vector3 = Camera.main.ScreenToWorldPoint(touch.position);
                                        startPos = new Vector2(vector3.x, vector3.y);
                                        minus = -75f;
                                    }
                                    else
                                    {
                                        swipeStarted = false;
                                    }
                                }
                                if (swipeStarted)
                                {
                                    _spoon.GetComponent<SpringJoint2D>().enabled = false;
                                    Vector3 vector4 = Camera.main.ScreenToWorldPoint(touch.position);
                                    endPos = new Vector2(vector4.x, vector4.y);
                                    SoundMgr.instance.AimingSound();
                                    distance = Vector2.Distance(startPos, endPos);
                                }
                            }
                            
							if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) 
								&& swipeStarted && _projectile != null)
                            {
                                if (_projectile != null)
                                {
                                    _projectile.Shoot(distance);
                                }
                                _projectile = null;
                                if (_additionalProjectile != null)
                                {
                                    _additionalProjectile.Shoot(distance, isAdditional: true);
                                }
                                _additionalProjectile = null;
                                ClearAim();
                                SoundMgr.instance.CatapultLaunch(playerShoot: true);
                                for (int num4 = 0; num4 < _stickmans.Length; num4++)
                                {
                                    if (_stickmans[num4] != null)
                                    {
                                        _stickmans[num4].Shoot();
                                    }
                                }
                                CheckSpringPower();
                                _spoon.GetComponent<SpringJoint2D>().enabled = true;
                                swipeStarted = false;
                                waitTime = 0f;
                                controllFinger = -1;
                                spawnAllowed = false;
                                distance = 0f;
                                minus = -70f;
                            }
                            
							if (!swipeStarted)
                            {
                                continue;
                            }
                            if (distance < 1f)
                            {
                                distance = 1f;
                            }
                            if (distance > 12f)
                            {
                                distance = 12f;
                            }
                            if (distance >= 1f && distance <= 12f)
                            {
                                minus = GameMath.CalculateAngle(distance);
                            }
                            _spoon.transform.rotation = Quaternion.Euler(0f, 0f, minus);
                            for (int num5 = 0; num5 < _stickmans.Length; num5++)
                            {
                                if (_stickmans[num5] != null)
                                {
                                    _stickmans[num5].NextTime(GameMath.DistanceToAnimation(distance) + (float)num5 * 0.2f);
                                }
                            }
                            Aiming();
                        }
                    }
				}
			}
			else
			{
				if (playerSide != GameSides.AI)
				{
					return;
				}
				if (GlobalLogic.instance.isGameEnded)
				{
					if (_projectile != null)
					{
						_projectile.ReturnToPool();
						_projectile = null;
					}
					if (_spoon != null)
					{
						_spoon.GetComponent<SpringJoint2D>().enabled = true;
					}
				}
				else if (waitTime < maxWaitTime)
				{
					waitTime += Time.deltaTime;
				}
				else if (!isTargetSelected)
				{
					if (_projectile == null)
					{
						BorrowProjectile();
						if (_projectile != null)
						{
							_projectile.Init(_projectilePosition, this);
							swipeStarted = true;
						}
						else
						{
							swipeStarted = false;
						}
					}
					if (swipeStarted)
					{
						distance = _ai.GetShootDistance();
						if (distance <= 1f)
						{
							distance = 1f;
						}
						if (distance > 12f)
						{
							distance = 12f;
						}
						if (distance > 1f && distance <= 12f)
						{
							minus = GameMath.CalculateAIAngle(distance, _ai.catapultType);
						}
						_spoon.GetComponent<SpringJoint2D>().enabled = false;
						for (int num6 = 0; num6 < _stickmans.Length; num6++)
						{
							_stickmans[num6].StartAiming();
							_stickmans[num6].NextTime(GameMath.DistanceToAnimation(distance));
						}
						isTargetSelected = true;
					}
				}
				else if (_spoon != null && _projectile != null)
				{
					Quaternion rotation = _spoon.transform.rotation;
					double num7 = (double)rotation.z - 0.1;
					Quaternion quaternion = Quaternion.Euler(0f, 0f, minus);
					if (num7 < (double)quaternion.z || timeWastedForAiming > 2f)
					{
						SoundMgr.instance.CatapultLaunch();
						for (int num8 = 0; num8 < _stickmans.Length; num8++)
						{
							_stickmans[num8].Shoot();
						}
						CheckSpringPower();
						_spoon.GetComponent<SpringJoint2D>().enabled = true;
						_projectile.Shoot(distance);
						_projectile = null;
						maxWaitTime = _ai.GetShootTime();
						waitTime = 0f;
						isTargetSelected = false;
						swipeStarted = false;
						timeWastedForAiming = 0f;
					}
					else
					{
						timeWastedForAiming += Time.deltaTime;
					}
				}
				else if (_projectile == null && _spoon != null)
				{
					_projectile = null;
					_spoon.GetComponent<SpringJoint2D>().enabled = true;
					for (int num9 = 0; num9 < _stickmans.Length; num9++)
					{
						_stickmans[num9].Shoot();
					}
					maxWaitTime = _ai.GetShootTime();
					waitTime = 0f;
					isTargetSelected = false;
					swipeStarted = false;
				}
			}
		}
	}
}
