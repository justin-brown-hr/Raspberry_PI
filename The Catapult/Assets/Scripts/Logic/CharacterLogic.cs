using Model;
using System.Collections;
using UnityEngine;

namespace Logic
{
	public class CharacterLogic : MonoBehaviour
	{
		public CatapultLogic _catapult;

		public PvPCatapultLogic _pvpCatapult;

		public GameObject _drinHolder;

		public Rigidbody2D[] kinematicParts;

		public GameObject[] bodyParts;

		private Vector2[] bodyPartsInitialPositions;

		private Vector2 playerInitialPosition;

		private Animator animator;

		public Transform anchorPosition;

		public GameObject hitParticle;

		public GameObject poisonEffect;

		public int animatorType;

		internal GameSides playerSide;

		internal bool isKilled;

		internal bool isEscaping;

		private bool jumpCalled;

		private bool jumpSoundCalled;

		[SerializeField]
		private float health;

		private int bloodCalled;

		private float escapingTime;

		private float timeToTick;

		private float animationTime;

		private bool deathScreamCalled;

		private bool isPoisoned;

		private float poisonTime;

		private bool isOnFire;

		private float fireTime;

		private GameObject fireParticle;

		private CharacterLogic deathCopy;

		private void Awake()
		{
			bloodCalled = 0;
			animator = GetComponent<Animator>();
			animator.SetBool("space_pressed", value: true);
			animator.speed = 1.5f;
			isKilled = false;
			jumpCalled = false;
			jumpSoundCalled = false;
			deathScreamCalled = false;
			isPoisoned = false;
			isOnFire = false;
		}

		public void InitCharacter()
		{
			playerInitialPosition = base.transform.localPosition;
			bodyPartsInitialPositions = new Vector2[bodyParts.Length];
			for (int i = 0; i < bodyParts.Length; i++)
			{
				bodyPartsInitialPositions[i] = bodyParts[i].transform.localPosition;
			}
		}

		public void ResetPlayer()
		{
			base.gameObject.SetActive(value: true);
			deathCopy = null;
			ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			isKilled = false;
			jumpCalled = false;
			jumpSoundCalled = false;
			deathScreamCalled = false;
			isPoisoned = false;
			isOnFire = false;
			isEscaping = false;
			bloodCalled = 0;
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			if (playerSide != GameSides.AI)
			{
				return;
			}
			animator.playbackTime = 0f;
			animator.enabled = false;
			animator.SetBool("Escape", value: false);
			animator.SetBool("space_pressed", value: true);
			base.transform.localPosition = playerInitialPosition;
			for (int j = 0; j < bodyParts.Length; j++)
			{
				if (bodyParts[j].GetComponent<Rigidbody2D>() != null)
				{
					bodyParts[j].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				}
				bodyParts[j].transform.localPosition = bodyPartsInitialPositions[j];
			}
			for (int k = 0; k < kinematicParts.Length; k++)
			{
				kinematicParts[k].bodyType = RigidbodyType2D.Kinematic;
			}
			animator.speed = 1.5f;
			animator.playbackTime = 0f;
			animator.enabled = true;
			_drinHolder.SetActive(value: true);
		}

		private void Start()
		{
			EquipHelmet();
			if (NewDataController.instance.GetPlayerControl() == ControlType.RightHand && NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				Transform transform = base.transform;
				Vector3 position = base.transform.position;
				float x = position.x;
				Vector3 position2 = base.transform.position;
				float y = position2.y;
				Vector3 position3 = base.transform.position;
				transform.position = new Vector3(x, y, -1f * position3.z);
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (playerSide == GameSides.Player1)
				{
					health = 50f * UpgradeShopData.instance.GetCurrentHelmetHPIncrease();
				}
				else
				{
					health = 50f;
				}
			}
			else
			{
				health = 50f;
			}
		}

		public void EquipHelmet()
		{
			if (playerSide != 0 && playerSide != GameSides.Player2)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < kinematicParts.Length)
				{
					if (kinematicParts[num].GetComponent<CharacterPart>() != null && kinematicParts[num].GetComponent<CharacterPart>().isHead)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			kinematicParts[num].GetComponent<CharacterPart>().EquipHelmet();
		}

		public void OutOfBounds()
		{
		}

		public void Prepare()
		{
			animator.SetBool("space_pressed", value: false);
			animator.speed = 1.5f * UnityEngine.Random.Range(0.8f, 1.2f);
		}

		private void DamageCalculation(float force)
		{
			health -= force;
		}

		public void UnitHit(float hitForce)
		{
			if (hitForce > 5f && !isKilled)
			{
				SoundMgr.instance.StickmanHit();
			}
			DamageCalculation(hitForce * 2f);
			if (health < 0f)
			{
				StickmanDeath();
			}
		}

		public void ParticleHit(float hitForce)
		{
			if (hitForce > 12f && !isKilled)
			{
				SoundMgr.instance.StickmanHit();
			}
			DamageCalculation(hitForce);
			if (health < 0f)
			{
				StickmanDeath();
			}
		}

		public void ParticleEffect(Vector2 hitPosition, float force, CharacterPart part)
		{
			if (force > 3f && bloodCalled < 2 && hitPosition != Vector2.zero)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(hitParticle, part.transform);
				gameObject.transform.position = part.transform.position;
				SoundMgr.instance.PlayerBleed();
				bloodCalled++;
			}
		}

		private void KillTransmition()
		{
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				_catapult.PartDestroyed(isCritical: false, isSpoon: false, this);
			}
			else
			{
				_pvpCatapult.PartDestroyed(isCritical: true);
			}
		}

		public void StickmanDeath()
		{
			if ((NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen && _pvpCatapult._shield != null) || isKilled)
			{
				return;
			}
			isEscaping = false;
			isKilled = true;
			animator.enabled = false;
			if (!jumpCalled && !deathScreamCalled)
			{
				SoundMgr.instance.StickmanDeath();
				deathScreamCalled = true;
			}
			KillTransmition();
			for (int i = 0; i < kinematicParts.Length; i++)
			{
				kinematicParts[i].bodyType = RigidbodyType2D.Dynamic;
				if (i > 0)
				{
					kinematicParts[i].mass = 1f;
				}
				else
				{
					kinematicParts[i].mass = 3f;
				}
				kinematicParts[i].gravityScale = 2f;
			}
			if (_drinHolder != null)
			{
				_drinHolder.SetActive(value: false);
			}
			if (deathCopy != null)
			{
				deathCopy.StickmanDeath();
			}
		}

		public void PlayerSplash()
		{
			if (!deathScreamCalled)
			{
				SoundMgr.instance.StickmanDeath();
				deathScreamCalled = true;
			}
		}

		public void Escape()
		{
			if (!isKilled && playerSide == GameSides.AI && NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (_catapult.isGroundCatapult)
				{
					StickmanDeath();
					return;
				}
				deathScreamCalled = true;
				Shoot();
				deathCopy = UnityEngine.Object.Instantiate(base.gameObject, base.transform.position, base.transform.rotation).GetComponent<CharacterLogic>();
				deathCopy.escapingTime = 0.25f;
				deathCopy.isEscaping = true;
				deathCopy.playerSide = GameSides.AI;
				UnityEngine.Object.Destroy(deathCopy.gameObject, 4f);
				base.gameObject.SetActive(value: false);
			}
		}

		public void Shoot()
		{
			if (!isKilled && !isEscaping)
			{
				animator.SetBool("space_pressed", value: true);
				animator.speed = 1.5f * UnityEngine.Random.Range(0.8f, 1.2f);
			}
		}

		public void NextTime(float newTime)
		{
			timeToTick = newTime;
		}

		public void StartAiming()
		{
			AnimatorControl(1);
		}

		public void AnimatorControl(int direction)
		{
			if (!(animator != null) || isEscaping || isKilled)
			{
				return;
			}
			animator.SetBool("space_pressed", value: false);
			if (direction == 1)
			{
				if (animationTime > 0.1f)
				{
					animationTime -= 0.02f;
				}
			}
			else if (animationTime < 0.9f)
			{
				animationTime += 0.02f;
			}
			animator.SetFloat("time", animationTime);
			if (animatorType == 1)
			{
				animator.Play("Stickman_aiming", 0, animationTime);
			}
			else if (animatorType == 2)
			{
				animator.Play("buz_sticman_twist5", 0, animationTime);
			}
			else if (animatorType == 3)
			{
				animator.Play("buz_sticman_twist", 0, animationTime);
			}
			animator.speed = 0f;
		}

		public void StopAnimation()
		{
			float @float = animator.GetFloat("time");
			timeToTick = animationTime;
		}

		private void JumpEffect()
		{
			if (!jumpCalled)
			{
				animator.SetBool("Escape", value: false);
				jumpCalled = true;
				StickmanDeath();
				kinematicParts[0].AddForce(new Vector2(135f, 75f), ForceMode2D.Impulse);
				kinematicParts[3].AddForce(Vector3.right * 16f, ForceMode2D.Impulse);
				kinematicParts[4].AddForce(Vector3.right * 16f, ForceMode2D.Impulse);
			}
		}

		public void PoisonStickman(GameObject initialParticle = null)
		{
			StartCoroutine(PoisonStickmanItself(initialParticle));
		}

		private IEnumerator PoisonStickmanItself(GameObject initialParticle)
		{
			if (!isPoisoned && !isEscaping && !isKilled && !isOnFire)
			{
				isPoisoned = true;
				poisonTime = 3f;
				yield return null;
				if (initialParticle != null)
				{
					Object.Instantiate(initialParticle, kinematicParts[0].transform.position, Quaternion.identity, kinematicParts[0].transform);
				}
			}
		}

		public void SetStickmanOnFire(GameObject particle)
		{
			if (!isOnFire)
			{
				isOnFire = true;
				fireTime = 3f;
				fireParticle = UnityEngine.Object.Instantiate(particle, kinematicParts[0].transform.position, Quaternion.identity, kinematicParts[0].transform);
			}
		}

		public void Update()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (isPoisoned)
			{
				if (poisonTime > 0f)
				{
					poisonTime -= Time.deltaTime;
					ParticleHit(Time.deltaTime * 75f);
				}
				else
				{
					isPoisoned = false;
					if (fireParticle != null && !isKilled)
					{
						UnityEngine.Object.Destroy(fireParticle);
					}
				}
			}
			if (isOnFire)
			{
				if (fireTime > 0f)
				{
					fireTime -= Time.deltaTime;
					ParticleHit(Time.deltaTime * 75f);
				}
				else
				{
					isOnFire = false;
					if (poisonEffect != null)
					{
						poisonEffect.SetActive(value: false);
					}
				}
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single && _catapult.playerSide == GameSides.AI && !_catapult.beingDestroyed)
			{
				base.transform.position = anchorPosition.position;
			}
			if (isEscaping)
			{
				if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump_out2"))
				{
					if (escapingTime > 0f)
					{
						escapingTime -= Time.deltaTime;
					}
					else
					{
						animator.SetBool("Escape", value: true);
					}
					return;
				}
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && !jumpSoundCalled)
				{
					SoundMgr.instance.PlayerJump();
					jumpSoundCalled = true;
				}
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
				{
					JumpEffect();
				}
			}
			else if (!isKilled && (timeToTick < animationTime - 0.01f || timeToTick > animationTime + 0.01f))
			{
				if (timeToTick > animationTime)
				{
					AnimatorControl(2);
				}
				else
				{
					AnimatorControl(1);
				}
			}
		}
	}
}
