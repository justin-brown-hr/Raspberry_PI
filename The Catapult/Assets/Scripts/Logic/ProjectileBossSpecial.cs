using Destructible2D;
using UnityEngine;

namespace Logic
{
	public class ProjectileBossSpecial : Projectile
	{
		public override void Init(Transform spawnPosition, BossStage launchedStage)
		{
			base.Init(spawnPosition, launchedStage);
			launchFrom = GameSides.AI;
		}

		public override void Shoot(float shootDistance = 0f, bool isAdditional = false)
		{
			base.Shoot(shootDistance, isAdditional);
			Vector2 a = _bossCannon.stageCharacter.projectileSpawnPosition.position - _bossCannon.stageCharacter.barrelPosition.position;
			_rigidbody.AddForce(a * 250f, ForceMode2D.Impulse);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (NewDataController.instance.GetGameMode() == GameMode.None)
			{
				return;
			}
			string tag = collision.gameObject.tag;
			if (tag == null)
			{
				return;
			}
			if (!(tag == "Catapult"))
			{
				if (!(tag == "Player"))
				{
					if (tag == "Projectile")
					{
						return;
					}
					if (!(tag == "Boss"))
					{
						if (tag == "BossCharacter")
						{
							BossCharacterHit(collision);
						}
					}
					else
					{
						BossHit(collision);
					}
				}
				else
				{
					PlayerHit(collision);
				}
			}
			else if (GetComponent<GroundedDetail>() == null)
			{
				CatapultHit(collision);
			}
		}

		private void BossCharacterHit(Collider2D collision)
		{
			if (collision.gameObject.GetComponent<BossCharacterPart>() != null && launchFrom == GameSides.Player1)
			{
				collision.gameObject.GetComponent<BossCharacterPart>().ComponentHit(_rigidbody.velocity.magnitude);
			}
		}

		private void CatapultHit(Collider2D collision)
		{
			if (NewDataController.instance.GetGameMode() == GameMode.None || NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				CatapultComponent catapultComponent = (!(collision.GetComponentInParent<D2dDestructible>() == null)) ? collision.GetComponentInParent<CatapultComponent>() : collision.GetComponent<CatapultComponent>();
				if (catapultComponent != null && catapultComponent._catapult != null)
				{
					float magnitude = _rigidbody.velocity.magnitude;
					CatapultComponent catapultComponent2 = catapultComponent;
					float hitPower = magnitude;
					Vector3? transmittedPoint = _rigidbody.position;
					catapultComponent2.ComponentHit(hitPower, null, transmittedPoint, _rigidbody.velocity);
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

		private void BossHit(Collider2D collision)
		{
			if (_rigidbody.velocity.magnitude < 2f || launchFrom != 0)
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
			float magnitude = _rigidbody.velocity.magnitude;
			BossComponent bossComponent = component;
			float hitForce = magnitude;
			Vector3? projectilePosition = base.transform.position;
			bossComponent.BossComponentHit(hitForce, null, projectilePosition);
			GetComponent<Rigidbody2D>().drag = 0.5f;
		}

		public override void ReturnToPool(bool needNew = false)
		{
			launchFrom = GameSides.AI;
			base.ReturnToPool();
		}
	}
}
