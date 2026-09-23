using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Logic
{
	public class BossCharacter : MonoBehaviour
	{
		public Animator canonAnimator;

		public Animator characterAnimator;

		public Transform projectileSpawnPosition;

		public Transform barrelPosition;

		public Rigidbody2D[] kinematicPartsCharacter;

		public Rigidbody2D[] kinematicPartsCannon;

		public GameObject shootEffect;

		private BossStage _parentStage;

		private float _animatorSpeed;

		private float _currentAnimatorTime;

		private float _nextAnimatorTime;

		private bool _isPaused;

		private bool _isShooting;

		private float initialCanonXPosition;

		private float backCanonXPosition;

		private float _characterHealth;

		private bool _isPoisoned;

		private bool _cannonDestroyed;

		public bool testDestroy;

		public bool _isKilled
		{
			get;
			private set;
		}

		public void PrepareBossStageCharcter(BossStage parentStage)
		{
			_parentStage = parentStage;
			_animatorSpeed = 0.5f;
			_isKilled = false;
			_isPaused = false;
			_isShooting = false;
			_cannonDestroyed = false;
			_currentAnimatorTime = 0f;
			_nextAnimatorTime = 0f;
			Vector3 localPosition = canonAnimator.transform.parent.localPosition;
			initialCanonXPosition = localPosition.x;
			backCanonXPosition = initialCanonXPosition + 0.6f;
			_characterHealth = 50f;
			for (int i = 0; i < kinematicPartsCannon.Length; i++)
			{
				if (kinematicPartsCannon[i].GetComponent<BossCharacterPart>() != null)
				{
					kinematicPartsCannon[i].GetComponent<BossCharacterPart>().InitPart(50f);
				}
			}
			PauseAnimator();
			StartAiming();
		}

		private void PauseAnimator()
		{
			_isPaused = true;
			if (canonAnimator != null)
			{
				canonAnimator.speed = 0f;
			}
			if (characterAnimator != null)
			{
				characterAnimator.speed = 0f;
			}
		}

		private void ResumeAnimator()
		{
			_isPaused = false;
			if (canonAnimator != null)
			{
				canonAnimator.speed = _animatorSpeed;
			}
			if (characterAnimator != null)
			{
				characterAnimator.speed = _animatorSpeed;
			}
		}

		public void StartAiming()
		{
			_nextAnimatorTime = UnityEngine.Random.Range(0.5f - 0.05f * (float)_parentStage._stageIndex, 0.9f - 0.05f * (float)_parentStage._stageIndex);
			_isShooting = false;
			ResumeAnimator();
		}

		private void AnimatorControll()
		{
			int num = 0;
			num = ((!(_currentAnimatorTime > _nextAnimatorTime)) ? 1 : (-1));
			_currentAnimatorTime += 0.02f * (float)num;
			if (canonAnimator != null)
			{
				canonAnimator.Play("cannon_aim", 0, _currentAnimatorTime);
			}
			if (characterAnimator != null)
			{
				characterAnimator.Play("stickman_cannon_aiming", 0, _currentAnimatorTime);
			}
			if (_currentAnimatorTime > _nextAnimatorTime - 0.05f && _currentAnimatorTime < _nextAnimatorTime + 0.05f)
			{
				PauseAnimator();
				StartCoroutine(Shoot());
			}
		}

		private IEnumerator Shoot()
		{
			if (_isShooting)
			{
				yield break;
			}
			_isShooting = true;
			yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
			if (_isKilled)
			{
				yield break;
			}
			ResumeAnimator();
			Projectile projObj = IngameShop.instance.BorrowProjectile(4).GetComponent<Projectile>();
			if (projObj != null)
			{
				if (canonAnimator != null)
				{
					canonAnimator.speed = 0f;
				}
				if (characterAnimator != null)
				{
					characterAnimator.SetBool("IsShooting", value: true);
				}
				StartCoroutine(CanonShooting(projObj));
				StartCoroutine(StopShooting());
			}
			else
			{
				PauseAnimator();
				StartAiming();
			}
		}

		private IEnumerator StopShooting()
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 7f));
			if (!_isKilled)
			{
				if (canonAnimator != null)
				{
					canonAnimator.speed = _animatorSpeed;
				}
				if (characterAnimator != null)
				{
					characterAnimator.SetBool("IsShooting", value: false);
				}
				StartAiming();
				_isShooting = false;
			}
		}

		private IEnumerator CanonShooting(Projectile proj)
		{
			yield return new WaitForSeconds(0.75f);
			if (_isKilled)
			{
				yield break;
			}
			Transform _canonTransform = canonAnimator.gameObject.transform.parent;
			proj.Init(projectileSpawnPosition, _parentStage);
			proj.Shoot();
			Object.Instantiate(shootEffect, projectileSpawnPosition.position, Quaternion.identity);
			SoundMgr.instance.ShootThorns();
			if (_isKilled || _cannonDestroyed)
			{
				yield break;
			}
			while (true)
			{
				Vector3 localPosition = _canonTransform.localPosition;
				if (localPosition.x < backCanonXPosition)
				{
					Transform transform = _canonTransform;
					Vector3 localPosition2 = _canonTransform.localPosition;
					float x = localPosition2.x + 2f * Time.deltaTime;
					Vector3 localPosition3 = _canonTransform.localPosition;
					float y = localPosition3.y;
					Vector3 localPosition4 = _canonTransform.localPosition;
					transform.localPosition = new Vector3(x, y, localPosition4.z);
					yield return null;
					continue;
				}
				break;
			}
			while (true)
			{
				Vector3 localPosition5 = _canonTransform.localPosition;
				if (localPosition5.x > initialCanonXPosition)
				{
					Transform transform2 = _canonTransform;
					Vector3 localPosition6 = _canonTransform.localPosition;
					float x2 = localPosition6.x - 0.6f * Time.deltaTime;
					Vector3 localPosition7 = _canonTransform.localPosition;
					float y2 = localPosition7.y;
					Vector3 localPosition8 = _canonTransform.localPosition;
					transform2.localPosition = new Vector3(x2, y2, localPosition8.z);
					yield return null;
					continue;
				}
				break;
			}
		}

		public void CharacterHit(float hitForce)
		{
			_characterHealth -= hitForce;
			if (_characterHealth <= 0f && !_isKilled)
			{
				CharacterDeath();
			}
		}

		public void StageDestroyed()
		{
			CharacterDeath();
			CannonDestroyed();
		}

		public void CharacterDeath(bool fromCannon = false)
		{
			if (_isKilled)
			{
				return;
			}
			_isKilled = true;
			StopCoroutine(StopShooting());
			StopCoroutine(CanonShooting(null));
			StopCoroutine(Shoot());
			_parentStage._bossLogic.CharacterKilled();
			if (characterAnimator != null)
			{
				SoundMgr.instance.StickmanDeath();
				UnityEngine.Object.Destroy(characterAnimator.gameObject, 4f);
				characterAnimator.enabled = false;
				characterAnimator.transform.parent = null;
				for (int i = 0; i < kinematicPartsCharacter.Length; i++)
				{
					kinematicPartsCharacter[i].bodyType = RigidbodyType2D.Dynamic;
					kinematicPartsCharacter[i].GetComponent<Collider2D>().isTrigger = true;
					kinematicPartsCharacter[i].mass = 1f;
				}
				if (!fromCannon)
				{
					kinematicPartsCharacter[0].velocity = new Vector2(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f));
				}
				else
				{
					kinematicPartsCharacter[0].velocity = new Vector2(UnityEngine.Random.Range(0f, 30f), UnityEngine.Random.Range(-30f, 30f));
				}
			}
			base.enabled = false;
			PauseAnimator();
		}

		public void PoisonCharacter()
		{
			if (!_isPoisoned)
			{
				_isPoisoned = true;
			}
		}

		public void CannonDestroyed()
		{
			if (_cannonDestroyed)
			{
				return;
			}
			_cannonDestroyed = true;
			if (canonAnimator != null)
			{
				UnityEngine.Object.Destroy(canonAnimator.gameObject, 4f);
				canonAnimator.enabled = false;
				canonAnimator.transform.parent = null;
				Dictionary<GameObject, float> dictionary = new Dictionary<GameObject, float>();
				for (int i = 0; i < kinematicPartsCannon.Length; i++)
				{
					kinematicPartsCannon[i].bodyType = RigidbodyType2D.Dynamic;
					dictionary.Add(kinematicPartsCannon[i].gameObject, kinematicPartsCannon[i].GetComponent<SpriteRenderer>().bounds.size.magnitude);
					kinematicPartsCannon[i].velocity = new Vector2(UnityEngine.Random.Range(0f, 10f), UnityEngine.Random.Range(-10f, 10f));
				}
				List<KeyValuePair<GameObject, float>> list = dictionary.ToList();
				list.Sort((KeyValuePair<GameObject, float> pair1, KeyValuePair<GameObject, float> pair2) => pair1.Value.CompareTo(pair2.Value));
				for (int j = 0; j < list.Count; j++)
				{
					float num = 10f + (float)j * 2.5f;
					if (num < 0f)
					{
						num *= -1f;
					}
					list[j].Key.GetComponent<Rigidbody2D>().mass = num;
					list[j].Key.GetComponent<Rigidbody2D>().gravityScale = 3f;
				}
				barrelPosition.transform.GetComponent<Rigidbody2D>().mass = 50f;
			}
			CharacterDeath();
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown("1"))
			{
				CharacterDeath();
			}
			if (!_isKilled && !GlobalLogic.instance.isGameEnded)
			{
				if (!_isPaused && !_isShooting && _parentStage._bossLogic._shootingAllowed && !_cannonDestroyed)
				{
					AnimatorControll();
				}
				if (_isPoisoned)
				{
					CharacterHit(50f * Time.deltaTime);
				}
			}
		}
	}
}
