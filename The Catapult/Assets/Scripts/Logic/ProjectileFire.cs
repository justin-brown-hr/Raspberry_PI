using Model;
using System;
using UnityEngine;
using View;

namespace Logic
{
	internal class ProjectileFire : Projectile
	{
		public PointEffector2D _pointEffector;

		public GameObject projPath;

		public Collider2D _mainCollider;

		public GameObject fireDebris;

		public GameObject fireOnCatapult;

		public GameObject fireOnParticle;

		public GameObject fireOnPlayer;

		private AudioSource _source;

		private float pointEffectorTime;

		private float ignoringTime;

		private bool isIgnoring;

		protected override void Awake()
		{
			base.Awake();
			pointEffectorTime = 0f;
			_pointEffector.enabled = false;
		}

		public override void Init(Transform spawnPosition, CatapultLogic spawnCatapult)
		{
			base.Init(spawnPosition, spawnCatapult);
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
			_source = GetComponent<AudioSource>();
			_source.enabled = true;
			SoundMgr.instance.RegisterFireSound(_source);
		}

		public override void Init(Transform spawnPosition, PvPCatapultLogic spawnCatapult)
		{
			base.Init(spawnPosition, spawnCatapult);
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
			_source = GetComponent<AudioSource>();
			_source.enabled = true;
			SoundMgr.instance.RegisterFireSound(_source);
		}

		public override void Shoot(float distance = 0f, bool isAdditional = false)
		{
			base.Shoot();
			Invoke("EnablingCollider", 0.5f);
			if (distance != 0f)
			{
				Vector2 shootVector = GameMath.GetShootVector(launchFrom, distance);
				_rigidbody.AddForce(shootVector, ForceMode2D.Impulse);
				if (launchFrom == GameSides.Player1)
				{
					AchievementManager.instance.AchievementProgress(AchieventType.ProjectileFireLaunch, AchievementRegion.AllGame, 1);
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
				if (collision.gameObject.GetComponentInChildren<CatapultLogic>() != null && collision.gameObject.GetComponentInChildren<CatapultLogic>().playerSide == launchFrom)
				{
					base.gameObject.layer = 12;
					isIgnoring = true;
					ignoringTime = 0.35f;
				}
				break;
			case "Shield":
				if (collision.gameObject.GetComponent<Shield>()._protectedCatapult.playerSide == launchFrom)
				{
				}
				break;
			case "Tower":
				if (projectileType == ProjectileType.Steel)
				{
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
			component.BossComponentHit(num, fireOnCatapult, base.transform.position);
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
				if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Ground")
				{
					if (collision.relativeVelocity.magnitude > 2f)
					{
						SoundMgr.instance.StoneSplash();
						if (collision.contacts.Length > 0)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(_groundHitPrefab, collision.contacts[0].point, Quaternion.identity);
							gameObject.AddComponent<ParticleDestroyer>();
						}
						DestroySprite();
					}
				}
				else if (collision.gameObject.tag == "CatapultParticle")
				{
					if (collision.relativeVelocity.magnitude > 4f)
					{
						UnityEngine.Object.Instantiate(fireOnParticle, collision.transform.position, Quaternion.identity, collision.transform);
					}
					DestroySprite();
				}
				else if (collision.gameObject.tag == "Player")
				{
					if (collision.gameObject.GetComponent<CharacterPart>()._character.playerSide == launchFrom)
					{
						Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
					}
				}
				else if (collision.gameObject.tag == "Catapult")
				{
					if (NewDataController.instance.GetGameMode() == GameMode.Single)
					{
						if (collision.gameObject.GetComponent<CatapultComponent>()._catapult.playerSide == launchFrom)
						{
							Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
						}
						else
						{
							DestroySprite();
						}
					}
					else if (collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.playerSide == launchFrom)
					{
						Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
					}
					else
					{
						DestroySprite();
					}
				}
				else if (collision.gameObject.tag != "PvEShield")
				{
					if (collision.gameObject.tag == "Tower" && collision.relativeVelocity.magnitude > 2f)
					{
						SoundMgr.instance.StonePartHit();
					}
					DestroySprite();
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
				if (component._catapult != null)
				{
					if (component._catapult.playerSide != launchFrom)
					{
						component.ComponentHit(_rigidbody.velocity.magnitude, fireOnCatapult);
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
					component2.ComponentHit(_rigidbody.velocity.magnitude);
				}
			}
		}

		public void ProjectileHit(Collider2D collision)
		{
			Projectile component = collision.gameObject.GetComponent<Projectile>();
			if (component.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
			{
				DestroySprite();
			}
		}

		private void PlayerHit(Collider2D collision)
		{
			CharacterPart component = collision.GetComponent<CharacterPart>();
			if (component._character.playerSide != launchFrom)
			{
				component.HelmetHitEffect();
				component._character.UnitHit(_rigidbody.velocity.magnitude);
				DestroySprite();
			}
		}

		public void DestroySprite()
		{
			if (!isDestroyed)
			{
				isDestroyed = true;
				ReturnToPool(needNew: true);
				SoundMgr.instance.StoneBreak();
				SoundMgr.instance.UnregisterFireSound(_source);
				GameObject gameObject = UnityEngine.Object.Instantiate(fireDebris, base.transform.position, Quaternion.identity);
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					Vector2 velocity = _rigidbody.velocity;
					Vector2 a = gameObject.transform.GetChild(i).transform.position - base.transform.position;
					a *= 1000f;
					velocity += a;
					gameObject.transform.GetChild(i).GetComponent<FireDebris>().Init(_rigidbody.velocity);
				}
				base.transform.parent = null;
				UnityEngine.Object.Destroy(base.gameObject);
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
			base.ReturnToPool(needNew);
			if (_pointEffector != null)
			{
				_pointEffector.enabled = false;
			}
		}
	}
}
