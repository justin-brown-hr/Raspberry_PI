using Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Logic
{
	internal class ProjectileShoot : Projectile
	{
		public PointEffector2D _pointEffector;

		public GameObject projPath;

		public Collider2D _mainCollider;

		public Collider2D _triggerCollider;

		public GameObject projectileBase;

		public GameObject[] spikeList;

		private float pointEffectorTime;

		private float ignoringTime;

		private bool isIgnoring;

		private bool spikesShooted;

		private bool deactivated;

		private bool isLaunched;

		private bool shootAllowed;

		private float shootTime;

		private List<int> collidedObjects;

		protected override void Awake()
		{
			base.Awake();
			pointEffectorTime = 0f;
			_pointEffector.enabled = false;
		}

		public override void Init(Transform spawnPosition, CatapultLogic spawnCatapult)
		{
			collidedObjects = new List<int>();
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
			spikesShooted = false;
			shootTime = 0f;
			shootAllowed = false;
			isLaunched = false;
			deactivated = false;
			base.Init(spawnPosition, spawnCatapult);
		}

		public override void Init(Transform spawnPosition, PvPCatapultLogic spawnCatapult)
		{
			collidedObjects = new List<int>();
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
			spikesShooted = false;
			shootTime = 0f;
			shootAllowed = false;
			isLaunched = false;
			deactivated = false;
			base.Init(spawnPosition, spawnCatapult);
		}

		public override void Shoot(float distance = 0f, bool isAdditional = false)
		{
			base.Shoot();
			Invoke("EnablingCollider", 0.5f);
			if (distance != 0f)
			{
				Vector2 shootVector = GameMath.GetShootVector(launchFrom, distance);
				_rigidbody.AddForce(shootVector, ForceMode2D.Impulse);
			}
			rotateAllowed = true;
			isLaunched = true;
		}

		private void EnablingCollider()
		{
			_mainCollider.enabled = true;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			EnablingPointEffector();
			if (collidedObjects.Contains(collision.gameObject.GetInstanceID()))
			{
				return;
			}
			collidedObjects.Add(collision.gameObject.GetInstanceID());
			switch (collision.gameObject.tag)
			{
			case "Projectile":
				_mainCollider.enabled = true;
				ProjectileHit(collision);
				break;
			case "Catapult":
				if (GetComponent<GroundedDetail>() == null)
				{
					CatapultHit(collision);
				}
				break;
			case "Player":
				PlayerHit(collision);
				break;
			case "Platform":
				if (collision.gameObject.GetComponentInChildren<CatapultLogic>() != null && collision.gameObject.GetComponentInChildren<CatapultLogic>().playerSide == launchFrom)
				{
					base.gameObject.layer = 12;
					isIgnoring = true;
					ignoringTime = 0.35f;
				}
				else if (projectileType != ProjectileType.Steel)
				{
					DestroySprite();
				}
				break;
			case "Shield":
				if (collision.gameObject.GetComponent<Shield>()._protectedCatapult.playerSide != launchFrom)
				{
					DestroySprite();
				}
				break;
			case "Tower":
				if (projectileType != ProjectileType.Steel)
				{
					DestroySprite();
				}
				break;
			case "Boss":
				BossHit(collision);
				break;
			case "BossCharacter":
				BossCharacterHit(collision);
				break;
			}
		}

		private void BossCharacterHit(Collider2D collision)
		{
			if (collision.gameObject.GetComponent<BossCharacterPart>() != null)
			{
				collision.gameObject.GetComponent<BossCharacterPart>().ComponentHit(_rigidbody.velocity.magnitude);
			}
		}

		private void BossHit(Collider2D collision)
		{
			if (_rigidbody.velocity.magnitude < 2f)
			{
				return;
			}
			BossComponent component = collision.GetComponent<BossComponent>();
			if (!(component != null))
			{
				return;
			}
			if (component._jointRedirectEnabled)
			{
				if (!component._jointRedirectEnabled)
				{
					return;
				}
				Vector3 position = component.transform.position;
				float x = position.x;
				Vector3 position2 = base.transform.position;
				if (!(x < position2.x))
				{
					return;
				}
			}
			float num = 0f;
			num = _rigidbody.velocity.magnitude;
			BossComponent bossComponent = component;
			float hitForce = num;
			Vector3? projectilePosition = base.transform.position;
			bossComponent.BossComponentHit(hitForce, null, projectilePosition);
			GetComponent<Rigidbody2D>().drag = 0.5f;
		}

		private void EnablingPointEffector()
		{
			if (_pointEffector != null && _rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				_pointEffector.enabled = true;
				pointEffectorTime = 0.25f;
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if (collision.gameObject.tag == "Catapult")
				{
					if (NewDataController.instance.GetGameMode() == GameMode.Single)
					{
						if (collision.gameObject.GetComponent<CatapultComponent>()._catapult.playerSide != launchFrom && !collision.gameObject.GetComponent<CatapultComponent>()._catapult.beingDestroyed && collision.gameObject.GetComponent<CatapultComponent>().material != GameMaterial.SinglePart)
						{
							deactivated = true;
							ReturnToPool(needNew: true);
							base.transform.parent = collision.gameObject.transform;
							UnityEngine.Object.Destroy(_mainCollider);
							UnityEngine.Object.Destroy(_triggerCollider);
							UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
							for (int i = 0; i < spikeList.Length; i++)
							{
								if (spikeList[i] != null && spikeList[i].GetComponent<Rigidbody2D>() == null)
								{
									UnityEngine.Object.Destroy(spikeList[i].GetComponent<Collider2D>());
								}
							}
							UnityEngine.Object.Destroy(projectileBase.GetComponent<Collider2D>());
						}
						else
						{
							Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
						}
					}
					else if (collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.playerSide != launchFrom && !collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.isDestroyed && collision.gameObject.GetComponent<CatapultComponent>().material != GameMaterial.SinglePart)
					{
						deactivated = true;
						ReturnToPool(needNew: true);
						base.transform.parent = collision.gameObject.transform;
						UnityEngine.Object.Destroy(_mainCollider);
						UnityEngine.Object.Destroy(_triggerCollider);
						UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
						for (int j = 0; j < spikeList.Length; j++)
						{
							if (spikeList[j] != null && spikeList[j].GetComponent<Rigidbody2D>() == null)
							{
								UnityEngine.Object.Destroy(spikeList[j].GetComponent<Collider2D>());
							}
						}
						UnityEngine.Object.Destroy(projectileBase.GetComponent<Collider2D>());
					}
					else
					{
						Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
					}
				}
				if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<CharacterPart>()._character.playerSide == launchFrom)
				{
					Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
				}
				if ((collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Ground") && collision.relativeVelocity.magnitude > 2f)
				{
					SoundMgr.instance.StoneSplash();
					if (collision.contacts.Length > 0)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(_groundHitPrefab, collision.contacts[0].point, Quaternion.identity);
						gameObject.AddComponent<ParticleDestroyer>();
					}
				}
				if (collision.gameObject.tag == "Tower" && collision.relativeVelocity.magnitude > 2f)
				{
					SoundMgr.instance.StonePartHit();
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}

		private void CatapultHit(Collider2D collision)
		{
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				CatapultComponent component = collision.GetComponent<CatapultComponent>();
				if (!(component._catapult != null))
				{
					return;
				}
				if (component._catapult.playerSide != launchFrom)
				{
					float magnitude = _rigidbody.velocity.magnitude;
					if (component.CheckIfCriticalHit(magnitude))
					{
						ProjectileKillCatapult(component._catapult.gameObject.GetInstanceID());
					}
					component.ComponentHit(magnitude);
				}
				else if (component != null && _singleCatapult != null && component._catapult != _singleCatapult)
				{
					base.gameObject.layer = 17;
					isIgnoring = true;
					ignoringTime = 0.2f;
				}
			}
			else
			{
				CatapultComponent component2 = collision.GetComponent<CatapultComponent>();
				if (component2._pvpCatapult != null && component2._pvpCatapult.playerSide != launchFrom)
				{
					component2.ComponentHit(_rigidbody.velocity.magnitude);
				}
			}
		}

		public void ProjectileHit(Collider2D collision)
		{
			Projectile component = collision.gameObject.GetComponent<Projectile>();
			if (component.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && (component.projectileType == ProjectileType.Stone || component.projectileType == ProjectileType.Tripple))
			{
				CheckDestructionForPoints(component);
				if (projectileType == ProjectileType.Stone)
				{
					DestroySprite();
				}
			}
		}

		public void DestroySprite()
		{
			if (!isDestroyed)
			{
				isDestroyed = true;
			}
		}

		private void PlayerHit(Collider2D collision)
		{
			CharacterPart component = collision.GetComponent<CharacterPart>();
			if (component._character.playerSide != launchFrom)
			{
				component.HelmetHitEffect();
				float num = _rigidbody.velocity.magnitude;
				if (component.isHead)
				{
					num *= 3f;
				}
				component._character.UnitHit(num);
			}
		}

		public void ShootSpike()
		{
			if (!(GetComponent<GroundedDetail>() == null))
			{
				return;
			}
			spikesShooted = true;
			SoundMgr.instance.ShootThorns();
			Vector2 vector = projectileBase.transform.position;
			for (int i = 0; i < spikeList.Length; i++)
			{
				if (!(spikeList[i] != null))
				{
					continue;
				}
				if (launchFrom == GameSides.Player1)
				{
					Vector3 position = spikeList[i].transform.position;
					if (position.x > vector.x)
					{
						spikeList[i].GetComponent<ProjectileShootPart>().ShootPart(this);
					}
				}
				else
				{
					Vector3 position2 = spikeList[i].transform.position;
					if (position2.x < vector.x)
					{
						spikeList[i].GetComponent<ProjectileShootPart>().ShootPart(this);
					}
				}
			}
		}

		private void ShootPvP()
		{
			if (spikesShooted || deactivated || !shootAllowed)
			{
				return;
			}
			if (launchFrom == GameSides.Player1)
			{
				Vector3 vector = Camera.main.WorldToViewportPoint(base.transform.position);
				if (vector.x > 0.75f)
				{
					spikesShooted = true;
					ShootSpike();
				}
			}
			else
			{
				Vector3 vector2 = Camera.main.WorldToViewportPoint(base.transform.position);
				if (vector2.x < 0.25f)
				{
					spikesShooted = true;
					ShootSpike();
				}
			}
		}

		private bool CheckDistance()
		{
			if (NewDataController.instance.GetGameMode() == GameMode.Single && _singleCatapult != null)
			{
				Vector3 position = base.transform.position;
				float x = position.x;
				Vector3 position2 = _singleCatapult.transform.position;
				float num = Mathf.Pow(x - position2.x, 2f);
				Vector3 position3 = base.transform.position;
				float y = position3.y;
				Vector3 position4 = _singleCatapult.transform.position;
				float num2 = Mathf.Sqrt(num + Mathf.Pow(y - position4.y, 2f));
				if (num2 > 15f)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void Update()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (Input.GetMouseButtonDown(0) && !InitController.instance.guiPressed && CheckDistance())
				{
					if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand)
					{
						Vector3 mousePosition = UnityEngine.Input.mousePosition;
						if (mousePosition.x > (float)(Screen.width / 2) && !spikesShooted && shootAllowed && !deactivated)
						{
							ShootSpike();
						}
					}
					else
					{
						Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
						if (mousePosition2.x < (float)(Screen.width / 2) && !spikesShooted && shootAllowed && !deactivated)
						{
							ShootSpike();
						}
					}
				}
			}
			else
			{
				ShootPvP();
			}
			if (isLaunched && !shootAllowed)
			{
				if (shootTime > 0f)
				{
					shootTime -= Time.deltaTime;
				}
				else
				{
					shootAllowed = true;
				}
			}
			if (pointEffectorTime > 0f && _pointEffector.enabled)
			{
				pointEffectorTime -= Time.deltaTime;
			}
			else
			{
				_pointEffector.enabled = false;
			}
			if (isIgnoring)
			{
				if (ignoringTime > 0f)
				{
					ignoringTime -= Time.deltaTime;
					return;
				}
				isIgnoring = false;
				base.gameObject.layer = 13;
			}
		}

		private void OnBecameInvisible()
		{
			_trailRenderer.enabled = false;
		}

		public override void ReturnToPool(bool needNew = false)
		{
			needNew = true;
			base.ReturnToPool(needNew);
			if (_pointEffector != null)
			{
				_pointEffector.enabled = false;
			}
			if (!deactivated)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
