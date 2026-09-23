using System;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Logic
{
	public class CatapultComponent : MonoBehaviour
	{
		public CatapultLogic _catapult;

		public PvPCatapultLogic _pvpCatapult;

		public GameObject _spoonDebris;

		public GameMaterial material;

		public float health;

		[SerializeField]
		private float _health;

		public bool isCriticalPart;

		public bool isSpoon;

		public int upgradeShopIndex;

		public CharacterLogic animationDependency;

		private bool destroyed;

		private bool isOnFire;

		private int exploderCalledTimes;

		private bool exploderCalled;

		private Vector2 initPartPosition;

		private Quaternion initPartRotation;

		public DestroyedCatapultPart destroyComponents;

		public void InitPartInPool()
		{
			initPartPosition = base.transform.localPosition;
			initPartRotation = base.transform.localRotation;
			if (destroyComponents != null)
			{
				destroyComponents.InitPart();
			}
		}

		public void ReinitPart()
		{
			ResetDestroy();
			base.gameObject.SetActive(value: true);
			destroyed = false;
			isOnFire = false;
			exploderCalled = false;
			exploderCalledTimes = 0;
			_health = health;
			PartBasedOnLevel();
		}

		public void ResetDestroy()
		{
			base.transform.localPosition = initPartPosition;
			base.transform.localRotation = initPartRotation;
			if (destroyComponents != null && _catapult != null)
			{
				if (_catapult.beingDestroyed)
				{
					destroyComponents.DeactivateDestroyedPart();
				}
				else
				{
					destroyComponents.DeatcivateItself();
				}
			}
			ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			ProjectileShoot[] componentsInChildren2 = base.transform.GetComponentsInChildren<ProjectileShoot>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				UnityEngine.Object.Destroy(componentsInChildren2[j].gameObject);
			}
		}

		private void PartBasedOnLevel()
		{
			if (_catapult != null)
			{
				if (_catapult.playerSide == GameSides.Player1)
				{
					Sprite sprite = InitController.instance.AskCatapultSprite(upgradeShopIndex);
					if (sprite == null)
					{
						base.gameObject.SetActive(value: false);
					}
					else
					{
						GetComponent<SpriteRenderer>().sprite = sprite;
						_health *= InitController.instance.GetCatapultHPIncrease();
					}
					if (destroyComponents != null)
					{
						destroyComponents.SetPlayerDestroyedSprites(NewDataController.instance.GetCurrentCatapultIndex(), NewDataController.instance.GetCurrentCatapultUpgrade());
					}
				}
				else
				{
					if (_catapult.playerSide != GameSides.AI || _catapult._ai == null)
					{
						return;
					}
					if (_catapult._ai.levelInitialized)
					{
						Sprite sprite2 = InitController.instance.AskAICatapultSprite(_catapult._ai.catapultType, upgradeShopIndex, _catapult._ai.catapultLevel);
						if (sprite2 == null)
						{
							base.gameObject.SetActive(value: false);
						}
						else
						{
							GetComponent<SpriteRenderer>().sprite = sprite2;
							_health *= InitController.instance.GetAICatapultHPIncrease(_catapult._ai.catapultType, _catapult._ai.catapultLevel);
						}
					}
					if (destroyComponents != null)
					{
						destroyComponents.SetDestroyedSprites(_catapult._ai.catapultType, _catapult._ai.catapultLevel);
					}
				}
			}
			else if (_pvpCatapult != null)
			{
				Sprite sprite3 = InitController.instance.AskCatapultSprite(upgradeShopIndex, isPvp: true);
				if (sprite3 == null)
				{
					base.gameObject.SetActive(value: false);
					return;
				}
				GetComponent<SpriteRenderer>().sprite = sprite3;
				_health *= InitController.instance.GetCatapultHPIncrease();
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Catapult" || collision.gameObject.tag == "BirdBonus")
				{
					Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
				}
				if ((collision.gameObject.tag == "ProjectileParticle" || collision.gameObject.tag == "ExploderFragment") && collision.relativeVelocity.magnitude > 7f && collision.gameObject.GetComponent<GroundedDetail>() == null)
				{
					ComponentHit(collision.relativeVelocity.magnitude / 3f);
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}

		public bool CheckIfCriticalHit(float force)
		{
			if (isCriticalPart && _health - force <= 0f)
			{
				return true;
			}
			return false;
		}

		public void ComponentHit(float hitPower, GameObject fireHitEffectPrefab = null, Vector3? transmittedPoint = default(Vector3?), Vector2? transmittedVelocity = default(Vector2?))
		{
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (GlobalLogic.instance.isGameEnded)
				{
					return;
				}
			}
			else if (PvPLogic.instance.isEndGame)
			{
				return;
			}
			if (destroyed || _health <= 0f)
			{
				return;
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (_catapult.isGroundCatapult && !base.gameObject.GetComponent<Renderer>().isVisible)
				{
					return;
				}
				if (material == GameMaterial.Wood)
				{
					SoundMgr.instance.WoodPartHit();
					if (!isOnFire && fireHitEffectPrefab != null)
					{
						UnityEngine.Object.Instantiate(fireHitEffectPrefab, _catapult.transform.position, base.transform.rotation, base.transform);
						isOnFire = true;
					}
				}
				Vector3 projectilePosition = Vector2.zero;
				Vector3 projectileVelocity = Vector2.zero;
				if (transmittedPoint.HasValue && !_catapult.destroyEffectCalled)
				{
					projectilePosition = transmittedPoint.Value;
					projectileVelocity = transmittedVelocity.Value;
				}
				_health -= hitPower;
				if (!(_health <= 0f) || destroyed)
				{
					return;
				}
				destroyed = true;
				if (animationDependency != null)
				{
					animationDependency.UnitHit(200f);
				}
				_catapult.PartDestroyed(isCriticalPart);
				bool destroyEffectCalled = false;
				if (!isSpoon)
				{
					if (material == GameMaterial.Wood)
					{
						SoundMgr.instance.WoodDestroy();
						if (destroyComponents != null)
						{
							destroyEffectCalled = destroyComponents.ActivateDestroyedPart(base.transform.position, base.transform.rotation, GetComponent<SpriteRenderer>().sortingOrder, projectilePosition, projectileVelocity);
						}
						base.gameObject.SetActive(value: false);
					}
					else if (material == GameMaterial.SinglePart)
					{
						if (destroyComponents != null)
						{
							destroyComponents.ActivateDestroyedPart(base.transform.position, base.transform.rotation, GetComponent<SpriteRenderer>().sortingOrder, projectilePosition, projectileVelocity);
						}
						base.gameObject.SetActive(value: false);
					}
				}
				else
				{
					SoundMgr.instance.SpoonDestroy();
					if (_catapult._projectile != null)
					{
						_catapult._projectile.Shoot();
					}
					_catapult._projectile = null;
					if (destroyComponents != null)
					{
						destroyComponents.ActivateDestroyedPart(base.transform.position, base.transform.rotation, GetComponent<SpriteRenderer>().sortingOrder, projectilePosition, projectileVelocity);
					}
					base.gameObject.SetActive(value: false);
				}
				if (!_catapult.destroyEffectCalled)
				{
					_catapult.destroyEffectCalled = destroyEffectCalled;
				}
			}
			else
			{
				if (NewDataController.instance.GetGameMode() != GameMode.PvPOneScreen || (_pvpCatapult != null && _pvpCatapult._shield != null))
				{
					return;
				}
				if (material == GameMaterial.Wood)
				{
					SoundMgr.instance.WoodPartHit();
					if (!isOnFire && fireHitEffectPrefab != null)
					{
						UnityEngine.Object.Instantiate(fireHitEffectPrefab, _pvpCatapult.transform.position, Quaternion.identity, _pvpCatapult.transform);
						isOnFire = true;
					}
				}
				_health -= hitPower;
				if (!(_health <= 0f) || destroyed)
				{
					return;
				}
				destroyed = true;
				if (animationDependency != null)
				{
					animationDependency.UnitHit(200f);
				}
				_pvpCatapult.PartDestroyed(isCriticalPart);
				if (!isSpoon)
				{
					if (material == GameMaterial.Wood)
					{
						SoundMgr.instance.WoodDestroy();
						SlicerWork();
					}
					else if (material == GameMaterial.SinglePart)
					{
						GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
						if (GetComponent<FixedJoint2D>() != null)
						{
							UnityEngine.Object.Destroy(GetComponent<FixedJoint2D>());
						}
						if (GetComponent<SpringJoint2D>() != null)
						{
							UnityEngine.Object.Destroy(GetComponent<SpringJoint2D>());
						}
						if (GetComponent<HingeJoint2D>() != null)
						{
							UnityEngine.Object.Destroy(GetComponent<HingeJoint2D>());
						}
					}
					return;
				}
				SoundMgr.instance.SpoonDestroy();
				if (_pvpCatapult._currentProjectile != null)
				{
					_pvpCatapult._currentProjectile.Shoot();
				}
				_pvpCatapult._currentProjectile = null;
				if (_spoonDebris != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(_spoonDebris, base.transform.position, base.transform.rotation, _pvpCatapult.transform);
					if (gameObject != null)
					{
						gameObject.GetComponent<DebrisPartView>().RedirectJoint(GetComponent<HingeJoint2D>());
					}
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		public void SpoonConnectdestroy()
		{
		}

		private void SlicerWork()
		{
			List<SpriteSlicer2DSliceInfo> slicedObjectInfo = new List<SpriteSlicer2DSliceInfo>();
			SpriteSlicer2D.ExplodeSprite(base.gameObject, 3, 10f, destroySlicedObjects: true, ref slicedObjectInfo);
			for (int i = 0; i < slicedObjectInfo.Count; i++)
			{
				for (int j = 0; j < slicedObjectInfo[i].ChildObjects.Count; j++)
				{
					bool isPlayer = false;
					slicedObjectInfo[i].ChildObjects[j].AddComponent<PartParticle>().Initialize(fromCatapult: true, isPlayer);
				}
			}
		}

		private void Update()
		{
			if (isOnFire)
			{
				ComponentHit(100f * Time.deltaTime);
			}
		}
	}
}
