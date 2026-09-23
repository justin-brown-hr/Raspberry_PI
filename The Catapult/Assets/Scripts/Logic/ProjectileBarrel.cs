using Model;
using System;
using UnityEngine;
using View;

namespace Logic
{
	internal class ProjectileBarrel : Projectile
	{
		public PointEffector2D _pointEffector;

		public GameObject projPath;

		public Collider2D _mainCollider;

		public GameObject barrelsDebris;

		public GameObject acidHitParticle;

		public GameObject acidParts;

		private float pointEffectorTime;

		private float ignoringTime;

		private bool isIgnoring;

		private int acidParticlesCount = 3;

		protected override void Awake()
		{
			base.Awake();
			pointEffectorTime = 0f;
			_pointEffector.enabled = false;
		}

		public override void Init(Transform spawnPosition, CatapultLogic spawnCatapult)
		{
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
			base.Init(spawnPosition, spawnCatapult);
		}

		public override void Init(Transform spawnPosition, PvPCatapultLogic spawnCatapult)
		{
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
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
				if (launchFrom == GameSides.Player1 && projectileType == ProjectileType.Barrel)
				{
					AchievementManager.instance.AchievementProgress(AchieventType.ProjectileBarrelLaunch, AchievementRegion.AllGame, 1);
				}
			}
			rotateAllowed = true;
		}

		private void EnablingCollider()
		{
			_mainCollider.enabled = true;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			EnablingPointEffector();
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
				if (NewDataController.instance.GetGameMode() == GameMode.Single && collision.gameObject.GetComponentInChildren<CatapultLogic>() != null && collision.gameObject.GetComponentInChildren<CatapultLogic>().playerSide == launchFrom)
				{
					base.gameObject.layer = 12;
					isIgnoring = true;
					ignoringTime = 0.35f;
				}
				else
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
				DestroySprite();
				break;
			case "Boss":
				BossHit(collision);
				DestroySprite();
				break;
			case "BossCharacter":
				BossCharacterHit(collision);
				DestroySprite();
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
			BossComponent bossComponent = component;
			float hitForce = _rigidbody.velocity.magnitude * 0.1f;
			Vector3? projectilePosition = base.transform.position;
			bossComponent.BossComponentHit(hitForce, null, projectilePosition);
			DestroySprite();
		}

		private void EnablingPointEffector()
		{
			if (_rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				_pointEffector.enabled = true;
				pointEffectorTime = 0.25f;
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if ((collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Ground") && !isDestroyed)
				{
					if (collision.contacts.Length > 0)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(_groundHitPrefab, collision.contacts[0].point, Quaternion.identity);
						gameObject.AddComponent<ParticleDestroyer>();
					}
					DestroySprite();
				}
				if (collision.gameObject.tag == "Catapult")
				{
					if (NewDataController.instance.GetGameMode() == GameMode.Single)
					{
						if (collision.gameObject.GetComponent<CatapultComponent>()._catapult.playerSide != launchFrom)
						{
							DestroySprite();
						}
						else
						{
							Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
						}
					}
					else if (collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.playerSide != launchFrom)
					{
						DestroySprite();
					}
					else
					{
						Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
					}
				}
				if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<CharacterPart>()._character.playerSide != launchFrom)
				{
					Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
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
			if (NewDataController.instance.GetGameMode() == GameMode.None || NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				CatapultComponent component = collision.GetComponent<CatapultComponent>();
				if (component._catapult != null)
				{
					if (component._catapult.playerSide != launchFrom)
					{
						component.ComponentHit(_rigidbody.velocity.magnitude * 0.1f);
						DestroySprite();
					}
					else if (component != null && _singleCatapult != null && component._catapult != _singleCatapult)
					{
						base.gameObject.layer = 17;
						isIgnoring = true;
						ignoringTime = 0.2f;
					}
				}
			}
			else
			{
				CatapultComponent component2 = collision.GetComponent<CatapultComponent>();
				if (component2._pvpCatapult != null && component2._pvpCatapult.playerSide != launchFrom)
				{
					component2.ComponentHit(_rigidbody.velocity.magnitude * 0.5f);
				}
			}
		}

		public void ProjectileHit(Collider2D collision)
		{
			Projectile component = collision.gameObject.GetComponent<Projectile>();
			if (component.launchFrom != launchFrom)
			{
				DestroySprite();
			}
			else
			{
				Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), GetComponent<Collider2D>());
			}
		}

		public void DestroySprite()
		{
			Invoke("DestroySpriteItself", 0.05f);
		}

		public void DestroySpriteItself()
		{
			if (isDestroyed)
			{
				return;
			}
			isDestroyed = true;
			SoundMgr.instance.BarrelDestroy();
			ReturnToPool(needNew: true);
			Vector2 velocity = _rigidbody.velocity;
			if (projectileType == ProjectileType.BarrelAcid)
			{
				UnityEngine.Object.Instantiate(acidHitParticle, base.transform.position, Quaternion.identity);
				for (int i = 0; i < acidParticlesCount; i++)
				{
					UnityEngine.Object.Instantiate(acidParts, base.transform.position, Quaternion.identity).GetComponent<AcidParticle>().Init(launchFrom);
				}
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(barrelsDebris, base.transform.position, Quaternion.identity, null);
			for (int j = 0; j < gameObject.transform.childCount; j++)
			{
				gameObject.transform.GetChild(j).GetComponent<Rigidbody2D>().velocity = velocity + new Vector2(UnityEngine.Random.Range(-10f, 16f), UnityEngine.Random.Range(-4f, 16f));
			}
			UnityEngine.Object.Destroy(gameObject, 3f);
			UnityEngine.Object.Destroy(base.gameObject);
			base.transform.parent = null;
		}

		private void PlayerHit(Collider2D collision)
		{
			CharacterPart component = collision.GetComponent<CharacterPart>();
			if (component._character.playerSide != launchFrom)
			{
				component.HelmetHitEffect();
				if (component.isHead && NewDataController.instance.GetEquipedHelmet() == 999)
				{
					component._character.UnitHit(100f);
				}
				else
				{
					component._character.UnitHit(_rigidbody.velocity.magnitude * 0.1f);
				}
				DestroySprite();
			}
		}

		protected virtual void Update()
		{
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
			if (isDestroyed)
			{
				base.ReturnToPool(needNew);
				if (_pointEffector != null)
				{
					_pointEffector.enabled = false;
				}
			}
		}
	}
}
