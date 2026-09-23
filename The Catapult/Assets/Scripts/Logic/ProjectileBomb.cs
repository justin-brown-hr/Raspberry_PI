using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
	internal class ProjectileBomb : Projectile
	{
		public PointEffector2D _pointEffector;

		public CircleCollider2D _explosionCollider;

		public GameObject _explosionPrefab;

		private AudioSource _source;

		public Collider2D _effectorCollider;

		public Collider2D _mainCollider;

		public GameObject bigBombExplosion;

		private bool isExploding;

		private float explosionRadius;

		private float effectorTime;

		private bool effectorEnabled;

		private bool isIgnoring;

		private float ignoringTime;

		private bool explodeInstantiated;

		protected override void Awake()
		{
			base.Awake();
			_pointEffector.enabled = false;
			explosionRadius = _explosionCollider.radius;
			effectorEnabled = false;
			isExploding = false;
		}

		public override void Init(Transform spawnPosition, CatapultLogic spawnCatapult)
		{
			base.Init(spawnPosition, spawnCatapult);
			explodeInstantiated = false;
			_effectorCollider.enabled = false;
			GetComponent<SpriteRenderer>().enabled = true;
			_source = GetComponent<AudioSource>();
			SoundMgr.instance.RegisterBombSound(_source);
			isExploding = false;
			effectorEnabled = false;
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
		}

		public override void Init(Transform spawnPosition, PvPCatapultLogic spawnCatapult)
		{
			base.Init(spawnPosition, spawnCatapult);
			_effectorCollider.enabled = false;
			GetComponent<SpriteRenderer>().enabled = true;
			_source = GetComponent<AudioSource>();
			SoundMgr.instance.RegisterBombSound(_source);
			isExploding = false;
			effectorEnabled = false;
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
		}

		public override void Shoot(float shootDistance = 0f, bool isAdditional = false)
		{
			base.Shoot(shootDistance);
			Invoke("EnablingCollider", 0.25f);
			if (shootDistance != 0f)
			{
				Vector2 shootVector = GameMath.GetShootVector(launchFrom, shootDistance);
				_rigidbody.AddForce(shootVector, ForceMode2D.Impulse);
				if (launchFrom == GameSides.Player1)
				{
					AchievementManager.instance.AchievementProgress(AchieventType.ProjectileBombLaunch, AchievementRegion.AllGame, 1);
				}
			}
		}

		private void EnablingCollider()
		{
			_mainCollider.enabled = true;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if (collision.gameObject.tag == "BirdBonus")
				{
					if (launchFrom != GameSides.AI)
					{
						base.gameObject.layer = 17;
						isIgnoring = true;
						ignoringTime = 0.2f;
					}
					Explode();
				}
				else if (collision.gameObject.tag == "Player")
				{
					if (collision.gameObject.GetComponent<CharacterPart>()._character.playerSide != launchFrom)
					{
						Explode();
					}
					else
					{
						Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
					}
				}
				else if (collision.gameObject.tag == "Catapult")
				{
					if (NewDataController.instance.GetGameMode() == GameMode.Single)
					{
						if (collision.gameObject.GetComponent<CatapultComponent>()._catapult.playerSide != launchFrom)
						{
							Explode();
						}
						else
						{
							Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
						}
					}
					else if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen)
					{
						if (collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.playerSide != launchFrom)
						{
							Explode();
						}
						else
						{
							Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
						}
					}
				}
				else if (collision.gameObject.tag == "Shield")
				{
					if (collision.gameObject.GetComponent<Shield>()._protectedCatapult.playerSide != launchFrom)
					{
						Explode();
					}
				}
				else if (collision.gameObject.tag == "ProjectileParticle" || collision.gameObject.tag == "CatapultParticle")
				{
					Explode();
				}
				else if (collision.gameObject.tag == "Projectile")
				{
					if (collision.gameObject.GetComponent<Projectile>().launchFrom == launchFrom)
					{
						Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
					}
					else
					{
						Explode();
					}
				}
				else
				{
					Explode();
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}

		private float DistanceBetween(Vector3 first, Vector3 second)
		{
			float num = 0f;
			return Mathf.Sqrt(Mathf.Pow(second.x - first.x, 2f) + Mathf.Pow(second.y - first.y, 2f));
		}

		private void Explode()
		{
			StartCoroutine(ExplodeItself());
		}

		private IEnumerator ExplodeItself()
		{
			if (isExploding)
			{
				yield break;
			}
			isExploding = true;
			SoundMgr.instance.UnregisterBombSound(_source);
			_source = null;
			if (projectileType == ProjectileType.Bomb)
			{
				SoundMgr.instance.BombExplosion();
			}
			else
			{
				SoundMgr.instance.BigBombExplosion();
			}
			GetComponent<SpriteRenderer>().enabled = false;
			if (projectileType == ProjectileType.BombThorns)
			{
				UnityEngine.Object.Instantiate(bigBombExplosion, base.transform.position, Quaternion.identity);
			}
			_effectorCollider.enabled = true;
			List<BossStage> bossStageHitted = new List<BossStage>();
			_rigidbody.bodyType = RigidbodyType2D.Static;
			Collider2D[] hitColliders = Physics2D.OverlapCircleAll(base.transform.position, explosionRadius);
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				StartCoroutine(EnablePointEfector(0.05f));
			}
			else
			{
				StartCoroutine(EnablePointEfector(0.025f));
			}
			for (int i = 0; i < hitColliders.Length; i++)
			{
				if (hitColliders[i] == null)
				{
					continue;
				}
				float hitDistance = DistanceBetween(base.transform.position, hitColliders[i].transform.position);
				switch (hitColliders[i].gameObject.tag)
				{
				case "Player":
					if (!(hitDistance < 15f))
					{
						break;
					}
					if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen)
					{
						if (hitColliders[i].GetComponent<CharacterPart>()._character._pvpCatapult._shield == null)
						{
							hitColliders[i].GetComponent<CharacterPart>()._character.UnitHit((explosionRadius - hitDistance) * 10f);
						}
					}
					else if (hitColliders[i].GetComponent<CharacterPart>()._character.playerSide != launchFrom)
					{
						hitColliders[i].GetComponent<CharacterPart>()._character.UnitHit((explosionRadius - hitDistance) * 10f);
					}
					break;
				case "Catapult":
					if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen)
					{
						if (hitColliders[i].GetComponent<CatapultComponent>()._pvpCatapult._shield == null)
						{
							hitColliders[i].GetComponent<CatapultComponent>().ComponentHit((explosionRadius - hitDistance) * 15f);
						}
					}
					else if (hitColliders[i].GetComponent<CatapultComponent>()._catapult.playerSide != launchFrom)
					{
						float num2 = (explosionRadius - hitDistance) * 15f;
						if (hitColliders[i].GetComponent<CatapultComponent>().CheckIfCriticalHit(num2))
						{
							ProjectileKillCatapult(hitColliders[i].GetComponent<CatapultComponent>()._catapult.gameObject.GetInstanceID());
						}
						hitColliders[i].GetComponent<CatapultComponent>().ComponentHit(num2);
					}
					break;
				case "Projectile":
					if (hitColliders[i].GetComponent<Projectile>().projectileType == ProjectileType.Stone && (NewDataController.instance.GetGameMode() != GameMode.Single || hitColliders[i].GetComponent<Projectile>().isLaunchedfromCatapult || hitColliders[i].GetComponent<Projectile>().launchFrom != launchFrom))
					{
						CheckDestructionForPoints(hitColliders[i].GetComponent<Projectile>());
						(hitColliders[i].GetComponent<Projectile>() as ProjectileStone).DestroySprite();
					}
					break;
				case "BirdBonus":
					hitColliders[i].GetComponent<BirdBonusLogic>().KilledByPlayer(launchFrom);
					break;
				case "Boss":
				{
					BossComponent component = hitColliders[i].GetComponent<BossComponent>();
					if (component != null && hitDistance < explosionRadius / 4f)
					{
						float num = explosionRadius / 2f - hitDistance;
						if (!(num <= 0f) && !bossStageHitted.Contains(component._parentStage))
						{
							bossStageHitted.Add(component._parentStage);
							component.BossComponentHit(num * 10f);
						}
					}
					break;
				}
				}
				if (i % 50 == 0 && i > 1)
				{
					yield return new WaitForSeconds(0.05f);
				}
			}
		}

		private IEnumerator EnablePointEfector(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			GameObject exp = UnityEngine.Object.Instantiate(_explosionPrefab, base.transform.position, Quaternion.identity);
			exp.transform.parent = null;
			_pointEffector.enabled = true;
			effectorTime = 0.05f;
			effectorEnabled = true;
		}

		public override void ReturnToPool(bool needNew = false)
		{
			if (!isExploding)
			{
				base.ReturnToPool(needNew);
			}
		}

		private void Update()
		{
			if (isIgnoring)
			{
				if (ignoringTime > 0f)
				{
					ignoringTime -= Time.deltaTime;
				}
				else
				{
					isIgnoring = false;
					base.gameObject.layer = 13;
				}
			}
			if (effectorEnabled)
			{
				if (effectorTime > 0f)
				{
					effectorTime -= Time.deltaTime;
					return;
				}
				_pointEffector.enabled = false;
				effectorEnabled = false;
				isExploding = false;
				ReturnToPool();
			}
		}
	}
}
