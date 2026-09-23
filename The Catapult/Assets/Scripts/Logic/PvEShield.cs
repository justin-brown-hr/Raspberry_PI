using UnityEngine;

namespace Logic
{
	internal class PvEShield : MonoBehaviour
	{
		public GameObject destroyPrefab;

		public GameObject reflectPrefab;

		public ShieldTypes _shieldType;

		private CatapultLogic _protectedCatapult;

		private bool isActive;

		private bool isSpinning;

		private float spinningSpeed;

		private int rotateSide;

		private bool isActivating;

		private bool isDeactivating;

		private bool isReacting;

		private bool stopReacting;

		private Vector3 startSize;

		private Vector3 reactSize;

		private void Start()
		{
			isActivating = false;
			isDeactivating = false;
			isReacting = false;
			stopReacting = false;
		}

		public void Init(CatapultLogic _catapult)
		{
			_protectedCatapult = _catapult;
			base.transform.localScale = new Vector3(0f, 0f, 0f);
			if (_shieldType == ShieldTypes.SpoonDestroy || _shieldType == ShieldTypes.SpoonReflect)
			{
				base.transform.parent = _catapult.spoonShieldPosition.transform;
			}
			else
			{
				base.transform.parent = _catapult.transform.parent;
			}
			isActive = false;
			if (_shieldType == ShieldTypes.SpinDestroy || _shieldType == ShieldTypes.SpinReflect)
			{
				spinningSpeed = 100f;
				rotateSide = 1;
				isSpinning = true;
				base.transform.localScale = new Vector3(2.5f, 2.5f);
				startSize = new Vector3(2.5f, 2.5f);
				isActive = true;
			}
		}

		public void Activate()
		{
			SoundMgr.instance.ShieldReact();
			switch (_shieldType)
			{
			case ShieldTypes.FullDestroy:
				startSize = new Vector3(4f, 4f);
				isActivating = true;
				break;
			case ShieldTypes.FullReflect:
				startSize = new Vector3(4f, 4f);
				isActivating = true;
				break;
			case ShieldTypes.SpoonDestroy:
				startSize = new Vector3(1f, 1f);
				isActivating = true;
				break;
			case ShieldTypes.SpoonReflect:
				startSize = new Vector3(1f, 1f);
				isActivating = true;
				break;
			case ShieldTypes.SpinReflect:
				EnableSpin();
				break;
			case ShieldTypes.SpinDestroy:
				EnableSpin();
				break;
			}
			isActive = true;
		}

		public void Deactivate()
		{
			if (_shieldType != ShieldTypes.SpinDestroy && _shieldType != ShieldTypes.SpinReflect)
			{
				isActive = false;
				startSize = new Vector3(1f, 1f);
				isActivating = false;
				isReacting = false;
				stopReacting = false;
				isDeactivating = true;
			}
			else
			{
				spinningSpeed = 100f;
			}
		}

		public void DestroyShield()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!isActive)
			{
				return;
			}
			if (collision.gameObject.tag == "Projectile" && collision.gameObject.GetComponent<Projectile>().launchFrom != _protectedCatapult.playerSide)
			{
				ProjectileReact(collision.gameObject);
				SoundMgr.instance.ShieldHit();
			}
			if (collision.gameObject.tag == "CatapultParticle" || collision.gameObject.tag == "ProjectileParticle")
			{
				if (_shieldType == ShieldTypes.FullDestroy || _shieldType == ShieldTypes.SpinDestroy || _shieldType == ShieldTypes.SpoonDestroy)
				{
					DestroyParticle(collision.gameObject);
					SoundMgr.instance.ShieldHit();
				}
				else
				{
					ReflectParticle(collision.gameObject);
					SoundMgr.instance.ShieldHit();
				}
			}
		}

		private void ProjectileReact(GameObject hitObject)
		{
			switch (_shieldType)
			{
			case ShieldTypes.FullDestroy:
				DestroyProjectile(hitObject);
				break;
			case ShieldTypes.SpoonDestroy:
				DestroyProjectile(hitObject);
				break;
			case ShieldTypes.SpinDestroy:
				DestroyProjectile(hitObject);
				break;
			case ShieldTypes.FullReflect:
				ReflectProjectile(hitObject.GetComponent<Projectile>());
				break;
			case ShieldTypes.SpoonReflect:
				ReflectProjectile(hitObject.GetComponent<Projectile>());
				break;
			case ShieldTypes.SpinReflect:
				ReflectProjectile(hitObject.GetComponent<Projectile>());
				break;
			}
			if (!isReacting)
			{
				reactSize = startSize + new Vector3(0.25f, 0.25f);
				stopReacting = false;
				isReacting = true;
			}
		}

		private void DestroyProjectile(GameObject hitObject)
		{
			if (hitObject.activeSelf)
			{
				if (destroyPrefab != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(destroyPrefab, hitObject.transform.position, Quaternion.identity);
					Vector2 velocity = hitObject.GetComponent<Rigidbody2D>().velocity;
					float x = velocity.x / 10f;
					Vector2 velocity2 = hitObject.GetComponent<Rigidbody2D>().velocity;
					Vector2 velocity3 = new Vector2(x, velocity2.y / 10f);
					gameObject.GetComponent<Rigidbody2D>().velocity = velocity3;
					gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;
				}
				hitObject.GetComponent<Projectile>().ReturnToPool();
			}
		}

		private void DestroyParticle(GameObject hitObject)
		{
			UnityEngine.Object.Destroy(hitObject);
		}

		private void ReflectProjectile(Projectile _projectile)
		{
			if (reflectPrefab != null)
			{
				Object.Instantiate(reflectPrefab, _projectile.transform.position, Quaternion.identity, _projectile.transform);
			}
			Vector2 vector = _projectile.transform.position - base.transform.position;
			_projectile.GetComponent<Rigidbody2D>().velocity *= -1f;
			_projectile.GetComponent<Rigidbody2D>().velocity += vector;
			_projectile.GetComponent<Projectile>().launchFrom = _protectedCatapult.playerSide;
			_projectile.SetAsReflected();
		}

		private void ReflectParticle(GameObject collision)
		{
			collision.GetComponent<Rigidbody2D>().velocity *= -1f;
			Vector2 vector = collision.transform.position - base.transform.position;
			collision.GetComponent<Rigidbody2D>().velocity += vector;
		}

		private void EnableSpin()
		{
			rotateSide *= -1;
			spinningSpeed = 500f;
		}

		private void FixedUpdate()
		{
			if (Time.timeScale != 0f && isSpinning)
			{
				base.transform.Rotate(rotateSide * Vector3.forward * spinningSpeed * Time.fixedDeltaTime);
			}
			if (isActivating)
			{
				Vector3 localScale = base.transform.localScale;
				if (localScale.x < startSize.x)
				{
					base.transform.localScale += new Vector3(35f, 35f, 35f) * Time.fixedDeltaTime;
				}
				else
				{
					isActivating = false;
					base.transform.localScale = startSize;
				}
			}
			if (isDeactivating && _shieldType != ShieldTypes.SpinDestroy && _shieldType != ShieldTypes.SpinReflect)
			{
				Vector3 localScale2 = base.transform.localScale;
				if (localScale2.x > 0f)
				{
					base.transform.localScale -= new Vector3(35f, 35f, 35f) * Time.fixedDeltaTime;
				}
				else
				{
					isDeactivating = false;
					base.transform.localScale = Vector3.zero;
				}
			}
			if (isReacting)
			{
				Vector3 localScale3 = base.transform.localScale;
				if (localScale3.x < reactSize.x)
				{
					base.transform.localScale += new Vector3(20f, 20f, 20f) * Time.fixedDeltaTime;
					return;
				}
				isReacting = false;
				stopReacting = true;
			}
			else if (stopReacting)
			{
				Vector3 localScale4 = base.transform.localScale;
				if (localScale4.x > startSize.x)
				{
					base.transform.localScale -= new Vector3(20f, 20f, 20f) * Time.fixedDeltaTime;
					return;
				}
				stopReacting = false;
				base.transform.localScale = startSize;
			}
		}
	}
}
