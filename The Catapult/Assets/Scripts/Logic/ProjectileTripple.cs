using Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Logic
{
	internal class ProjectileTripple : Projectile
	{
		public PointEffector2D _pointEffector;

		private ProjectileTripple _parent;

		public Collider2D _mainCollider;

		public SpriteRenderer[] additionalSprites;

		private float pointEffectorTime;

		private float ignoringTime;

		private bool isIgnoring;

		private bool trippleGenerated;

		private bool mainProjectile;

		private float generateTimer;

		private List<int> collidedObjects;

		protected override void Awake()
		{
			base.Awake();
			pointEffectorTime = 0f;
			_pointEffector.enabled = false;
			_parent = this;
		}

		public void InitChild(ProjectileTripple parent, Vector2 vector)
		{
			collidedObjects = new List<int>();
			mainProjectile = false;
			_trailRenderer.enabled = true;
			_parent = parent;
			GetComponent<Rigidbody2D>().velocity = vector;
			Invoke("EnablingCollider", 0.5f);
			rotateAllowed = true;
			for (int i = 0; i < additionalSprites.Length; i++)
			{
				additionalSprites[i].enabled = false;
			}
		}

		public override void Init(Transform spawnPosition, CatapultLogic spawnCatapult)
		{
			base.Init(spawnPosition, spawnCatapult);
			collidedObjects = new List<int>();
			trippleGenerated = false;
			mainProjectile = false;
			_mainCollider.enabled = false;
			for (int i = 0; i < additionalSprites.Length; i++)
			{
				additionalSprites[i].enabled = true;
			}
		}

		public override void Init(Transform spawnPosition, PvPCatapultLogic spawnCatapult)
		{
			base.Init(spawnPosition, spawnCatapult);
			collidedObjects = new List<int>();
			trippleGenerated = false;
			mainProjectile = false;
			_mainCollider.enabled = false;
			for (int i = 0; i < additionalSprites.Length; i++)
			{
				additionalSprites[i].enabled = true;
			}
		}

		public override void Shoot(float distance = 0f, bool isAdditional = false)
		{
			base.Shoot(distance);
			trippleGenerated = false;
			mainProjectile = true;
			Invoke("EnablingCollider", 0.5f);
			if (distance != 0f)
			{
				Vector2 shootVector = GameMath.GetShootVector(launchFrom, distance);
				_rigidbody.AddForce(shootVector, ForceMode2D.Impulse);
				GenerateTripple();
			}
		}

		private void EnablingCollider()
		{
			_mainCollider.enabled = true;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
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
			case "Shield":
				if (collision.gameObject.GetComponent<Shield>()._protectedCatapult.playerSide != launchFrom)
				{
					DestroySprite();
				}
				break;
			case "Platform":
				DestroySprite();
				break;
			case "Tower":
				DestroySprite();
				break;
			case "Boss":
				BossHit(collision);
				break;
			case "BossCharacter":
				BossCharacterHit(collision);
				break;
			}
			EnablingPointEffector();
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
			float num = _rigidbody.velocity.magnitude * 0.75f;
			BossComponent bossComponent = component;
			float hitForce = num;
			Vector3? projectilePosition = base.transform.position;
			bossComponent.BossComponentHit(hitForce, null, projectilePosition);
			GetComponent<Rigidbody2D>().drag = 0.5f;
		}

		private void EnablingPointEffector()
		{
			_pointEffector.enabled = true;
			pointEffectorTime = 0.25f;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if ((collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Ground") && collision.relativeVelocity.magnitude > 2f)
				{
					SoundMgr.instance.StoneSplash();
					if (collision.contacts.Length > 0)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(_groundHitPrefab, collision.contacts[0].point, Quaternion.identity);
						gameObject.AddComponent<ParticleDestroyer>();
					}
				}
				if (collision.gameObject.tag == "Projectile" && collision.gameObject.GetComponent<Projectile>().projectileType == ProjectileType.Tripple)
				{
					Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
				}
				if (collision.gameObject.tag == "Catapult")
				{
					if (NewDataController.instance.GetGameMode() == GameMode.Single)
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
				if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<CharacterPart>()._character.playerSide == launchFrom)
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
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				CatapultComponent component = collision.GetComponent<CatapultComponent>();
				if (!(component._catapult != null))
				{
					return;
				}
				if (component._catapult.playerSide != launchFrom)
				{
					float num = _rigidbody.velocity.magnitude * 3f;
					if (component.CheckIfCriticalHit(num))
					{
						if (!mainProjectile)
						{
							_parent.ProjectileKillCatapult(component._catapult.gameObject.GetInstanceID());
						}
						else
						{
							ProjectileKillCatapult(component._catapult.gameObject.GetInstanceID());
						}
					}
					component.ComponentHit(num);
				}
				else
				{
					base.gameObject.layer = 17;
					isIgnoring = true;
					ignoringTime = 0.05f;
				}
				return;
			}
			CatapultComponent component2 = collision.GetComponent<CatapultComponent>();
			if (component2._pvpCatapult != null)
			{
				if (component2._pvpCatapult.playerSide != launchFrom)
				{
					component2.ComponentHit(_rigidbody.velocity.magnitude * 3f);
					return;
				}
				base.gameObject.layer = 17;
				isIgnoring = true;
				ignoringTime = 0.05f;
			}
		}

		public void ProjectileHit(Collider2D collision)
		{
			Projectile component = collision.gameObject.GetComponent<Projectile>();
			if (component.projectileType == ProjectileType.Tripple)
			{
				Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), GetComponent<Collider2D>());
			}
			if (component.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
			{
				if (component.projectileType == ProjectileType.Stone)
				{
					CheckDestructionForPoints(component);
					DestroySprite();
				}
				else if (component.projectileType == ProjectileType.Tripple && (component as ProjectileTripple)._parent != _parent)
				{
					CheckDestructionForPoints(component);
					DestroySprite();
				}
			}
		}

		public void DestroySprite()
		{
			if (isDestroyed)
			{
				return;
			}
			SoundMgr.instance.StoneBreak();
			isDestroyed = true;
			base.transform.parent = null;
			if (NewDataController.instance.GetGameMode() != GameMode.PvPOneScreen && mainProjectile)
			{
				ReturnToPool(needNew: true);
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				GameObject fragmentFromPool = IngameShop.instance.GetFragmentFromPool(ProjectileType.Stone);
				fragmentFromPool.GetComponent<DestroyedProjectilePart>().ActivateDestroyedPart(base.transform.position, _rigidbody.velocity);
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			List<SpriteSlicer2DSliceInfo> slicedObjectInfo = new List<SpriteSlicer2DSliceInfo>();
			SpriteSlicer2D.ExplodeSprite(base.gameObject, 3, 10f, destroySlicedObjects: true, ref slicedObjectInfo);
			for (int i = 0; i < slicedObjectInfo.Count; i++)
			{
				for (int j = 0; j < slicedObjectInfo[i].ChildObjects.Count; j++)
				{
					bool isPlayer = false;
					slicedObjectInfo[i].ChildObjects[j].AddComponent<PartParticle>().Initialize(fromCatapult: true, isPlayer);
					slicedObjectInfo[i].ChildObjects[j].AddComponent<ParticleDestroyer>();
				}
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

		private void GenerateTripple()
		{
			trippleGenerated = true;
			Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
			for (int i = 0; i < additionalSprites.Length; i++)
			{
				additionalSprites[i].enabled = false;
			}
			ProjectileTripple component = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<ProjectileTripple>();
			component.InitChild(this, velocity + new Vector2(0f, 1f));
			component = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<ProjectileTripple>();
			component.InitChild(this, velocity - new Vector2(0f, 1f));
		}

		private void OnCollisionExit2D(Collision2D collision)
		{
			if (!mainProjectile || trippleGenerated || !(collision.gameObject.tag == "Catapult"))
			{
				return;
			}
			CatapultComponent component = collision.gameObject.GetComponent<CatapultComponent>();
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (component == _singleCatapult._spoon)
				{
					GenerateTripple();
				}
			}
			else if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen && component == _pvpCatapult._spoon)
			{
				GenerateTripple();
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
			if (mainProjectile)
			{
				base.ReturnToPool(needNew);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
