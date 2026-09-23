using UnityEngine;

namespace Logic
{
	public class BossComponent : MonoBehaviour
	{
		public GameObject destroyedParticles;

		private Collider2D _collider;

		private Rigidbody2D _rigidbody;

		private FixedJoint2D _joint;

		public float _componentHealth;

		private bool _isDestroyed;

		private bool _isRoofComponent;

		public bool _isCriticalPart;

		[SerializeField]
		private bool _jointRedirect;

		private bool _isOnFire;

		private GameObject _fireOnComponent;

		private float _fireLiveTime;

		private Vector2[] initialDestroyedPartPositions;

		private Quaternion[] initialDestroyedPartRotations;

		public BossStage _parentStage
		{
			get;
			private set;
		}

		public bool _jointRedirectEnabled
		{
			get
			{
				return _jointRedirect;
			}
			private set
			{
				_jointRedirect = value;
			}
		}

		public bool _isRam
		{
			get;
			set;
		}

		public void PrepareBossComponent(BossStage parentStage, bool isRoofComponent = false, bool isRamComponent = false)
		{
			_isRoofComponent = isRoofComponent;
			_parentStage = parentStage;
			_jointRedirectEnabled = false;
			_isOnFire = false;
			_collider = GetComponent<Collider2D>();
			_rigidbody = GetComponent<Rigidbody2D>();
			_joint = GetComponent<FixedJoint2D>();
			_isRam = isRamComponent;
			if (destroyedParticles != null)
			{
				destroyedParticles.GetComponent<BossDestroyedPart>().PrepareDestroyedPart();
			}
		}

		public float InitBossStagePart(int stageIndex)
		{
			if (_parentStage != null && _parentStage._bossLogic != null)
			{
				int num = _parentStage._bossLogic._stagesCount - stageIndex;
				if (stageIndex != -1)
				{
					if (_isRoofComponent)
					{
						_componentHealth = 60f;
					}
					else
					{
						_componentHealth = 45f;
					}
				}
				else
				{
					_componentHealth = 30f * (float)num;
				}
				if (_rigidbody != null)
				{
					_rigidbody.mass = 10f * (float)num;
				}
			}
			else
			{
				_componentHealth = 50f;
				if (_rigidbody != null)
				{
					_rigidbody.mass = 10f;
				}
			}
			if (destroyedParticles != null)
			{
				destroyedParticles.SetActive(value: false);
			}
			base.gameObject.SetActive(value: true);
			return _componentHealth;
		}

		public void SetupRam()
		{
			_isRam = true;
			_rigidbody.mass = 75f;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "BombPickup" && _isRam && _parentStage != null)
			{
				_parentStage.StageExplode();
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.tag == "Boss")
			{
				if (!_jointRedirectEnabled)
				{
					Physics2D.IgnoreCollision(_collider, collision.gameObject.GetComponent<Collider2D>());
				}
				else if (collision.transform.parent != base.transform.parent)
				{
					_jointRedirectEnabled = false;
					_joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
					collision.gameObject.GetComponent<BossComponent>()._parentStage.PlayEmitterSystem();
					_joint.autoConfigureConnectedAnchor = false;
					_joint.enabled = true;
					_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
					if (_parentStage != null && (bool)_parentStage._bossLogic)
					{
						_parentStage._bossLogic.TowerBecomeLower();
					}
				}
			}
			if (collision.gameObject.tag == "Projectile")
			{
				if (_jointRedirectEnabled && collision.gameObject.GetComponent<Projectile>() != null)
				{
					Vector3 position = collision.gameObject.transform.position;
					float y = position.y;
					Vector3 position2 = base.transform.position;
					if (y < position2.y)
					{
						collision.gameObject.GetComponent<Projectile>().ReturnToPool();
					}
				}
			}
			else if (collision.gameObject.tag == "Exploder2D" && collision.gameObject.GetComponent<TowerComponent>() != null && _parentStage != null)
			{
				_parentStage.TowerTouched(collision.gameObject.GetComponent<TowerComponent>().parentTower);
			}
		}

		public void BossComponentHit(float hitForce, GameObject fireHitEffect = null, Vector3? projectilePosition = default(Vector3?), bool innerRequest = false)
		{
			if (base.gameObject.GetComponent<SpriteRenderer>().isVisible)
			{
				if (!innerRequest)
				{
					SoundMgr.instance.WoodPartHit();
				}
				_componentHealth -= hitForce;
				if (fireHitEffect != null && !_isOnFire)
				{
					_fireLiveTime = 3f;
					_isOnFire = true;
					_fireOnComponent = UnityEngine.Object.Instantiate(fireHitEffect, base.transform.position, Quaternion.identity, base.transform);
				}
				if (_parentStage != null)
				{
					_parentStage.StageComponentHit(hitForce, _isRoofComponent, projectilePosition);
				}
			}
		}

		private void StopFire()
		{
			_isOnFire = false;
		}

		public void ChangeLayer(bool inverse = false)
		{
			if (inverse && _joint.connectedBody != null && _joint.connectedBody.gameObject.activeSelf)
			{
				base.gameObject.layer = 11;
			}
			else if (_joint.connectedBody == null)
			{
				base.gameObject.layer = 19;
				_joint.enabled = false;
				_rigidbody.constraints = (RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation);
				_jointRedirectEnabled = true;
			}
			else if (!_joint.connectedBody.gameObject.activeSelf)
			{
				base.gameObject.layer = 19;
				_joint.enabled = false;
				_joint.connectedBody = null;
				_rigidbody.constraints = (RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation);
				_jointRedirectEnabled = true;
			}
		}

		public void ComponentDeath(bool fromProjectile = false, Vector3? projectilePosition = default(Vector3?))
		{
			if (_isDestroyed)
			{
				return;
			}
			_isDestroyed = true;
			if (destroyedParticles != null)
			{
				destroyedParticles.SetActive(value: true);
				destroyedParticles.GetComponent<BossDestroyedPart>().ActivatePart(_parentStage._stageIndex == -1);
				Vector3 vector = Vector2.zero;
				float num = 0f;
				for (int i = 0; i < destroyedParticles.transform.childCount; i++)
				{
					Rigidbody2D component = destroyedParticles.transform.GetChild(i).GetComponent<Rigidbody2D>();
					if (component != null)
					{
						component.gravityScale = 3f;
						component.mass = 2f;
						if (projectilePosition.HasValue)
						{
							vector = component.transform.position - projectilePosition.Value;
							num = 1f / vector.magnitude;
							num *= UnityEngine.Random.Range(-75f, 75f);
							component.velocity = vector;
							component.AddTorque(num, ForceMode2D.Impulse);
						}
					}
				}
			}
			base.gameObject.SetActive(value: false);
		}

		private void Update()
		{
			if (_isOnFire)
			{
				if (_fireLiveTime > 0f)
				{
					_fireLiveTime -= Time.deltaTime;
					BossComponentHit(50f * Time.deltaTime, null, null, innerRequest: true);
				}
				else
				{
					_isOnFire = false;
					UnityEngine.Object.Destroy(_fireOnComponent);
				}
			}
		}
	}
}
