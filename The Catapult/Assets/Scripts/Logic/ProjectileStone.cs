using Destructible2D;
using Exploder2D;
using Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Logic
{
	public class ProjectileStone : Projectile
	{
		public GameObject fragmentsPrefab;

		public PointEffector2D _pointEffector;

		public GameObject projPath;

		public Collider2D _mainCollider;

		public DestroyedCatapultPart part;

		private float pointEffectorTime;

		private float ignoringTime;

		private bool isIgnoring;

		private float pointEffectorUses;

		private List<int> collidedObjects;

		protected override void Awake()
		{
			base.Awake();
			pointEffectorTime = 0f;
			_pointEffector.enabled = false;
			collidedObjects = new List<int>();
		}

		public override void Init(Transform spawnPosition, CatapultLogic spawnCatapult)
		{
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
			collidedObjects.Clear();
			base.Init(spawnPosition, spawnCatapult);
		}

		public override void Init(Transform spawnPosition, PvPCatapultLogic spawnCatapult)
		{
			base.Init(spawnPosition, spawnCatapult);
			collidedObjects.Clear();
			_pointEffector.enabled = false;
			_mainCollider.enabled = false;
		}

		public override void Shoot(float distance = 0f, bool isAdditional = false)
		{
			base.Shoot();
			Invoke("EnablingCollider", 0.5f);
			pointEffectorUses = 5f;
			if (_singleCatapult != null || _pvpCatapult != null)
			{
				if (distance != 0f)
				{
					int aiType = -1;
					if (NewDataController.instance.GetGameMode() == GameMode.Single && _singleCatapult.playerSide == GameSides.AI)
					{
						aiType = _singleCatapult._ai.catapultType;
					}
					Vector2 shootVector = GameMath.GetShootVector(launchFrom, distance, isAdditional, aiType);
					_rigidbody.AddForce(shootVector, ForceMode2D.Impulse);
				}
			}
			else if (distance != 0f)
			{
				Vector2 bossShootVector = GameMath.GetBossShootVector(distance);
				_rigidbody.AddForce(bossShootVector, ForceMode2D.Impulse);
			}
			rotateAllowed = true;
		}

		private void EnablingCollider()
		{
			_mainCollider.enabled = true;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (NewDataController.instance.GetGameMode() == GameMode.None || collidedObjects.Contains(collision.gameObject.GetInstanceID()))
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
					base.gameObject.layer = 17;
					isIgnoring = true;
					ignoringTime = 0.4f;
					return;
				}
				if (projectileType != ProjectileType.Steel && _rigidbody.velocity.magnitude > 5f)
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
				if (projectileType != ProjectileType.Steel && _rigidbody.velocity.magnitude > 3f)
				{
					DestroySprite();
				}
				break;
			case "Exploder2D":
				if (projectileType != ProjectileType.Steel && _rigidbody.velocity.magnitude > 3f)
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
			EnablingPointEffector(collision);
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
			switch (projectileType)
			{
			case ProjectileType.Stone:
				num = _rigidbody.velocity.magnitude;
				break;
			case ProjectileType.StoneThorns:
				num = _rigidbody.velocity.magnitude * 2f;
				break;
			case ProjectileType.Steel:
				num = _rigidbody.velocity.magnitude * 3f;
				break;
			}
			BossComponent bossComponent = component;
			float hitForce = num;
			Vector3? projectilePosition = base.transform.position;
			bossComponent.BossComponentHit(hitForce, null, projectilePosition);
			GetComponent<Rigidbody2D>().drag = 0.5f;
		}

		private void EnablingPointEffector(Collider2D collision)
		{
			if (_rigidbody.bodyType != 0 || !(pointEffectorUses > 0f))
			{
				return;
			}
			if (collision.gameObject.tag == "Catapult")
			{
				if (NewDataController.instance.GetGameMode() == GameMode.Single)
				{
					if (collision.GetComponent<CatapultComponent>()._catapult.playerSide == launchFrom)
					{
						pointEffectorUses += 1f;
					}
				}
				else if (collision.GetComponent<CatapultComponent>()._pvpCatapult.playerSide == launchFrom)
				{
					pointEffectorUses += 1f;
				}
			}
			pointEffectorUses -= 1f;
			_pointEffector.enabled = true;
			pointEffectorTime = 0.1f;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if (NewDataController.instance.GetGameMode() == GameMode.None && collision.gameObject.tag == "Projectile")
				{
					Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
				}
				if ((collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Ground") && collision.relativeVelocity.magnitude > 4f)
				{
					SoundMgr.instance.StoneSplash();
					if (collision.contacts.Length > 0)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(_groundHitPrefab, collision.contacts[0].point, Quaternion.identity);
						gameObject.AddComponent<ParticleDestroyer>();
					}
				}
				if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<CharacterPart>()._character.playerSide == launchFrom)
				{
					Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
				}
				if (collision.gameObject.tag == "Catapult")
				{
					if (NewDataController.instance.GetGameMode() == GameMode.Single || NewDataController.instance.GetGameMode() == GameMode.None)
					{
						if (collision.gameObject.GetComponent<CatapultComponent>()._catapult.playerSide == launchFrom)
						{
							Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
						}
					}
					else if (collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.playerSide == launchFrom)
					{
						Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
					}
				}
				if (collision.gameObject.tag == "Boss" && launchFrom == GameSides.AI)
				{
					Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
				}
				if ((collision.gameObject.tag == "Tower" || collision.gameObject.tag == "Exploder2D") && collision.relativeVelocity.magnitude > 2f)
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
				CatapultComponent catapultComponent = (!(collision.GetComponentInParent<D2dDestructible>() == null)) ? collision.GetComponentInParent<CatapultComponent>() : collision.GetComponent<CatapultComponent>();
				if (!(catapultComponent != null) || !(catapultComponent._catapult != null))
				{
					return;
				}
				if (catapultComponent._catapult.playerSide != launchFrom)
				{
					float num = 0f;
					switch (projectileType)
					{
					case ProjectileType.Stone:
						num = _rigidbody.velocity.magnitude;
						break;
					case ProjectileType.StoneThorns:
						num = _rigidbody.velocity.magnitude * 2f;
						break;
					case ProjectileType.Steel:
						num = _rigidbody.velocity.magnitude * 4f;
						break;
					}
					if (catapultComponent.CheckIfCriticalHit(num))
					{
						ProjectileKillCatapult(catapultComponent._catapult.gameObject.GetInstanceID());
					}
					CatapultComponent catapultComponent2 = catapultComponent;
					float hitPower = num;
					Vector3? transmittedPoint = _rigidbody.position;
					catapultComponent2.ComponentHit(hitPower, null, transmittedPoint, _rigidbody.velocity);
				}
				else if (catapultComponent != null && _singleCatapult != null && catapultComponent._catapult != _singleCatapult)
				{
					base.gameObject.layer = 17;
					isIgnoring = true;
					ignoringTime = 0.4f;
				}
				return;
			}
			CatapultComponent component = collision.GetComponent<CatapultComponent>();
			if (component._pvpCatapult != null && component._pvpCatapult.playerSide != launchFrom)
			{
				switch (projectileType)
				{
				case ProjectileType.Stone:
					component.ComponentHit(_rigidbody.velocity.magnitude);
					break;
				case ProjectileType.StoneThorns:
					component.ComponentHit(_rigidbody.velocity.magnitude * 1.5f);
					break;
				case ProjectileType.Steel:
					component.ComponentHit(_rigidbody.velocity.magnitude * 2f);
					break;
				}
			}
		}

		public void ProjectileHit(Collider2D collision)
		{
			Projectile component = collision.gameObject.GetComponent<Projectile>();
			if (component.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
			{
				CheckDestructionForPoints(component);
				if (projectileType == ProjectileType.Stone && (component.projectileType == ProjectileType.Stone || component.projectileType == ProjectileType.StoneThorns || component.projectileType == ProjectileType.Steel))
				{
					DestroySprite();
				}
				else if (projectileType == ProjectileType.StoneThorns && (component.projectileType == ProjectileType.StoneThorns || component.projectileType == ProjectileType.Steel))
				{
					DestroySprite();
				}
			}
		}

		public void DestroySprite()
		{
			if (isDestroyed || projectileType == ProjectileType.Steel)
			{
				return;
			}
			isDestroyed = true;
			SoundMgr.instance.StoneBreak();
			base.transform.parent = null;
			if (NewDataController.instance.GetGameMode() != GameMode.PvPOneScreen)
			{
				ReturnToPool(needNew: true);
			}
			else if (projectileType != 0)
			{
				UnityEngine.Object.Destroy(base.gameObject, 4f);
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				GameObject fragmentFromPool = IngameShop.instance.GetFragmentFromPool(projectileType);
				fragmentFromPool.GetComponent<DestroyedProjectilePart>().ActivateDestroyedPart(base.transform.position, _rigidbody.velocity);
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			ReturnToPool(needNew: true);
			if (projectileType == ProjectileType.Stone)
			{
				List<SpriteSlicer2DSliceInfo> slicedObjectInfo = new List<SpriteSlicer2DSliceInfo>();
				int numCuts = 3;
				SpriteSlicer2D.ExplodeSprite(base.gameObject, numCuts, 10f, destroySlicedObjects: true, ref slicedObjectInfo);
				for (int i = 0; i < slicedObjectInfo.Count; i++)
				{
					for (int j = 0; j < slicedObjectInfo[i].ChildObjects.Count; j++)
					{
						bool isPlayer = false;
						slicedObjectInfo[i].ChildObjects[j].AddComponent<PartParticle>().Initialize(fromCatapult: false, isPlayer);
						slicedObjectInfo[i].ChildObjects[j].AddComponent<ParticleDestroyer>();
					}
				}
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(fragmentsPrefab, base.transform.position, Quaternion.identity);
				gameObject.GetComponent<DestroyedProjectilePart>().InitPart();
				gameObject.GetComponent<DestroyedProjectilePart>().ActivateDestroyedPart(base.transform.position, _rigidbody.velocity);
				UnityEngine.Object.Destroy(gameObject, 3f);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void OnExplode(float time, Exploder2DObject.ExplosionState state)
		{
			if (state == Exploder2DObject.ExplosionState.ExplosionFinished)
			{
				IngameShop.instance.OnComponentDestroy();
			}
		}

		private void PlayerHit(Collider2D collision)
		{
			CharacterPart component = collision.GetComponent<CharacterPart>();
			if (component._character.playerSide != launchFrom)
			{
				component.HelmetHitEffect();
				float num = _rigidbody.velocity.magnitude;
				switch (projectileType)
				{
				case ProjectileType.StoneThorns:
					num *= 2f;
					break;
				case ProjectileType.Steel:
					num *= 4f;
					break;
				}
				if (component.isHead)
				{
					num *= 3f;
				}
				component._character.UnitHit(num);
			}
			else if ((NewDataController.instance.GetGameMode() == GameMode.None || NewDataController.instance.GetGameMode() == GameMode.Single) && component != null && _singleCatapult != null && _singleCatapult._stickmans[0] != null && component._character != _singleCatapult._stickmans[0])
			{
				base.gameObject.layer = 12;
				isIgnoring = true;
				ignoringTime = 0.4f;
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
