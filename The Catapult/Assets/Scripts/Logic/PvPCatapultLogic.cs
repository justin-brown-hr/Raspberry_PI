using Model;
using UnityEngine;

namespace Logic
{
	public class PvPCatapultLogic : MonoBehaviour
	{
		internal Projectile _currentProjectile;

		public GameObject woodHitParticle;

		public CatapultComponent _spoon;

		public Transform _projectileSpawn;

		public CharacterLogic _character;

		internal Shield _shield;

		public CatapultComponent[] elementList;

		public GameSides playerSide;

		private ProjectileType currentType;

		internal bool isProtected;

		internal bool isDestroyed;

		internal bool isPoisoned;

		private float destroyTime;

		private bool playerScreenSidePressed;

		private bool swipeStarted;

		private Vector2 startPos;

		private Vector2 endPos;

		private float distance;

		private float angle;

		private int controllFinger;

		private bool spawnAllowed;

		private float waitTime;

		public GameObject projPath;

		private GameObject[] currentAim;

		private void Awake()
		{
			currentAim = new GameObject[14];
			playerScreenSidePressed = false;
			waitTime = 0.3f;
			isProtected = true;
			isDestroyed = false;
			isPoisoned = false;
			currentType = ProjectileType.Stone;
			_currentProjectile = null;
			_character.playerSide = playerSide;
			swipeStarted = false;
			distance = 0f;
			if (playerSide == GameSides.Player1)
			{
				angle = -66f;
			}
			else
			{
				angle = 66f;
			}
			for (int i = 0; i < elementList.Length; i++)
			{
				elementList[i].InitPartInPool();
				elementList[i].ReinitPart();
			}
		}

		public void InitShield(GameObject shield)
		{
			_shield = shield.GetComponent<Shield>();
			_shield.Init(this);
		}

		public void NextProjectileType(ProjectileType nextType)
		{
			currentType = nextType;
		}

		public void PartDestroyed(bool isCritical, bool isSpoon = false)
		{
			if (isCritical && !isDestroyed)
			{
				isDestroyed = true;
				PvPLogic.instance.CheckLastKill(playerSide);
				destroyTime = 2f;
				if (_currentProjectile != null)
				{
					_currentProjectile.Shoot();
				}
				if (_character != null)
				{
					_character.StickmanDeath();
				}
				ClearAim();
			}
		}

		public void CatapultDeath()
		{
			PvPLogic.instance.PlayerKilled(playerSide);
		}

		public void BorrowProjectile()
		{
			GameObject gameObject = PvPIngameShop.instance.BorrowProjectile(playerSide);
			if (gameObject != null)
			{
				_currentProjectile = gameObject.GetComponent<Projectile>();
			}
		}

		public void EndGameShoot()
		{
			if (_spoon != null && _currentProjectile != null)
			{
				playerScreenSidePressed = false;
				SoundMgr.instance.CatapultLaunch(playerShoot: true);
				_currentProjectile.Shoot();
				_character.Shoot();
				_spoon.GetComponent<SpringJoint2D>().enabled = true;
				_currentProjectile = null;
				swipeStarted = false;
				waitTime = 0f;
				distance = 0f;
				controllFinger = -1;
				if (playerSide == GameSides.Player1)
				{
					angle = -66f;
				}
				else
				{
					angle = 66f;
				}
				spawnAllowed = false;
			}
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
			if (PvPLogic.instance.isEndGame || !(_currentProjectile != null) || !(distance >= 0.5f) || NewDataController.instance.GetPlayerAiming() != AimingTipType.Dot)
			{
				return;
			}
			Vector2 vector = _currentProjectile.transform.position;
			float num = Time.fixedDeltaTime / (float)Physics2D.velocityIterations;
			Vector2 b = Physics2D.gravity * _currentProjectile.GetComponent<Rigidbody2D>().gravityScale * num * num;
			float d = 1f - num * _currentProjectile.GetComponent<Rigidbody2D>().drag;
			Vector2 a = GameMath.GetShootVector(playerSide, distance) * num;
			float mass = _currentProjectile.GetComponent<Rigidbody2D>().mass;
			a /= mass;
			int num2 = 0;
			for (int i = 0; i < 425; i++)
			{
				a += b;
				a *= d;
				vector += a;
				if (i % 25 == 0 && num2 < currentAim.Length)
				{
					if (currentAim[num2] != null)
					{
						UnityEngine.Object.Destroy(currentAim[num2]);
					}
					if (num2 != 0)
					{
						currentAim[num2] = UnityEngine.Object.Instantiate(projPath, vector, Quaternion.identity);
						Transform transform = currentAim[num2].transform;
						Vector3 localScale = currentAim[num2].transform.localScale;
						float x = localScale.x - 0.02f * (float)num2;
						Vector3 localScale2 = currentAim[num2].transform.localScale;
						transform.localScale = new Vector2(x, localScale2.y - 0.02f * (float)num2);
					}
					num2++;
				}
			}
		}

		private void ClearAim()
		{
			for (int i = 0; i < currentAim.Length; i++)
			{
				if (currentAim[i] != null)
				{
					UnityEngine.Object.Destroy(currentAim[i]);
				}
			}
		}

		public void PosionCatapult(GameObject initialParticle = null)
		{
			if (!isDestroyed && !isPoisoned)
			{
				isPoisoned = true;
				if (initialParticle != null)
				{
					Object.Instantiate(initialParticle, base.transform.position, Quaternion.identity, base.transform);
				}
				_character.PoisonStickman();
			}
		}

		private void Update()
		{
			if (Time.timeScale == 0f || PvPLogic.instance.isEndGame)
			{
				return;
			}
			if (PvPLogic.instance.isEndGame && _spoon != null)
			{
				if (_currentProjectile != null)
				{
					_currentProjectile.Shoot();
				}
				_spoon.GetComponent<SpringJoint2D>().enabled = true;
			}
			if (isDestroyed)
			{
				if (destroyTime > 0f)
				{
					destroyTime -= Time.deltaTime;
				}
				else
				{
					CatapultDeath();
				}
			}
			else if (waitTime < 0.25f)
			{
				waitTime += Time.deltaTime;
			}
			else if (playerSide == GameSides.Player1)
			{
				if (UnityEngine.Input.touchCount <= 0)
				{
					return;
				}
				float num = 10000f;
				for (int i = 0; i < UnityEngine.Input.touchCount; i++)
				{
					if ((float)Input.GetTouch(i).fingerId == InitController.instance.guiPressedFingerId || swipeStarted || UnityEngine.Input.GetTouch(i).phase != 0)
					{
						continue;
					}
					float num2 = Vector2.Distance(UnityEngine.Input.GetTouch(i).position, base.transform.position);
					if (num2 < num)
					{
						Vector2 position = UnityEngine.Input.GetTouch(i).position;
						if (position.x < (float)(Screen.width / 2))
						{
							playerScreenSidePressed = true;
							controllFinger = UnityEngine.Input.GetTouch(i).fingerId;
							Vector3 vector = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(i).position);
							startPos = new Vector2(vector.x, vector.y);
							num = num2;
							spawnAllowed = false;
						}
					}
				}
				if (!playerScreenSidePressed)
				{
					return;
				}
				for (int j = 0; j < UnityEngine.Input.touchCount; j++)
				{
					if (UnityEngine.Input.GetTouch(j).fingerId != controllFinger)
					{
						continue;
					}
					if (UnityEngine.Input.GetTouch(j).phase == TouchPhase.Moved)
					{
						if (!spawnAllowed)
						{
							Vector3 vector2 = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(j).position);
							endPos = new Vector2(vector2.x, vector2.y);
							if (!(Vector2.Distance(startPos, endPos) > 1f))
							{
								break;
							}
							spawnAllowed = true;
						}
						if (_currentProjectile == null)
						{
							BorrowProjectile();
							if (_currentProjectile != null)
							{
								if (_shield != null)
								{
									UnityEngine.Object.Destroy(_shield.gameObject);
								}
								_character.Prepare();
								_currentProjectile.Init(_projectileSpawn, this);
								swipeStarted = true;
								Vector3 vector3 = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(j).position);
								startPos = new Vector2(vector3.x, vector3.y);
								angle = -75f;
							}
							else
							{
								swipeStarted = false;
							}
						}
						if (swipeStarted)
						{
							_spoon.GetComponent<SpringJoint2D>().enabled = false;
							Vector3 vector4 = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(j).position);
							endPos = new Vector2(vector4.x, vector4.y);
							SoundMgr.instance.AimingSound();
							distance = Vector2.Distance(startPos, endPos);
						}
					}
					if ((UnityEngine.Input.GetTouch(j).phase == TouchPhase.Ended || UnityEngine.Input.GetTouch(j).phase == TouchPhase.Canceled) && swipeStarted && _currentProjectile != null)
					{
						ClearAim();
						SoundMgr.instance.CatapultLaunch(playerShoot: true);
						_character.Shoot();
						CheckSpringPower();
						_spoon.GetComponent<SpringJoint2D>().enabled = true;
						swipeStarted = false;
						waitTime = 0f;
						controllFinger = -1;
						spawnAllowed = false;
						_currentProjectile.Shoot(distance);
						distance = 0f;
						angle = -70f;
						_currentProjectile = null;
					}
					if (swipeStarted)
					{
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
							angle = (75f - distance * 4f) * -1f;
						}
						_spoon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
						_character.NextTime(GameMath.DistanceToAnimation(distance));
						Aiming();
					}
				}
			}
			else
			{
				if (playerSide != GameSides.Player2 || UnityEngine.Input.touchCount <= 0)
				{
					return;
				}
				float num3 = 10000f;
				for (int k = 0; k < UnityEngine.Input.touchCount; k++)
				{
					if ((float)Input.GetTouch(k).fingerId == InitController.instance.guiPressedFingerId || swipeStarted || UnityEngine.Input.GetTouch(k).phase != 0)
					{
						continue;
					}
					float num4 = Vector2.Distance(UnityEngine.Input.GetTouch(k).position, base.transform.position);
					if (num4 < num3)
					{
						Vector2 position2 = UnityEngine.Input.GetTouch(k).position;
						if (position2.x >= (float)(Screen.width / 2))
						{
							playerScreenSidePressed = true;
							controllFinger = UnityEngine.Input.GetTouch(k).fingerId;
							Vector3 vector5 = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(k).position);
							startPos = new Vector2(vector5.x, vector5.y);
							num3 = num4;
							spawnAllowed = false;
						}
					}
				}
				if (!playerScreenSidePressed)
				{
					return;
				}
				for (int l = 0; l < UnityEngine.Input.touchCount; l++)
				{
					if (UnityEngine.Input.GetTouch(l).fingerId != controllFinger)
					{
						continue;
					}
					if (UnityEngine.Input.GetTouch(l).phase == TouchPhase.Moved)
					{
						if (!spawnAllowed)
						{
							Vector3 vector6 = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(l).position);
							endPos = new Vector2(vector6.x, vector6.y);
							if (!(Vector2.Distance(startPos, endPos) > 1f))
							{
								break;
							}
							spawnAllowed = true;
						}
						if (_currentProjectile == null)
						{
							BorrowProjectile();
							if (_currentProjectile != null)
							{
								if (_shield != null)
								{
									UnityEngine.Object.Destroy(_shield.gameObject);
								}
								_character.Prepare();
								_currentProjectile.Init(_projectileSpawn, this);
								swipeStarted = true;
								Vector3 vector7 = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(l).position);
								startPos = new Vector2(vector7.x, vector7.y);
								angle = 75f;
							}
							else
							{
								swipeStarted = false;
							}
						}
						if (swipeStarted)
						{
							_spoon.GetComponent<SpringJoint2D>().enabled = false;
							Vector3 vector8 = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(l).position);
							endPos = new Vector2(vector8.x, vector8.y);
							SoundMgr.instance.AimingSound();
							distance = Vector2.Distance(startPos, endPos);
						}
					}
					if ((UnityEngine.Input.GetTouch(l).phase == TouchPhase.Ended || UnityEngine.Input.GetTouch(l).phase == TouchPhase.Canceled) && swipeStarted && _currentProjectile != null)
					{
						ClearAim();
						SoundMgr.instance.CatapultLaunch(playerShoot: true);
						_character.Shoot();
						CheckSpringPower();
						_spoon.GetComponent<SpringJoint2D>().enabled = true;
						swipeStarted = false;
						waitTime = 0f;
						controllFinger = -1;
						spawnAllowed = false;
						_currentProjectile.Shoot(distance);
						distance = 0f;
						angle = 70f;
						_currentProjectile = null;
					}
					if (swipeStarted)
					{
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
							angle = 75f - distance * 4f;
						}
						_spoon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
						_character.NextTime(GameMath.DistanceToAnimation(distance));
						Aiming();
					}
				}
			}
		}
	}
}
