using System;
using UnityEngine;
using View;

namespace Logic
{
	internal class ProjectileShootPart : MonoBehaviour
	{
		private ProjectileShoot _parent;

		private bool isActive;

		private float shootTime;

		public void ShootPart(ProjectileShoot parent)
		{
			_parent = parent;
			isActive = true;
			base.gameObject.tag = "ProjectileParticle";
			base.gameObject.layer = 14;
			base.gameObject.GetComponent<PolygonCollider2D>().enabled = true;
			Rigidbody2D rigidbody2D = base.gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
			rigidbody2D.gravityScale = 3.5f;
			rigidbody2D.mass = 3.5f;
			rigidbody2D.freezeRotation = true;
			rigidbody2D.AddForce(base.transform.up * 200f, ForceMode2D.Impulse);
			GetComponent<TrailRenderer>().enabled = true;
			base.transform.parent = null;
		}

		public GameSides GetGameSides()
		{
			return _parent.launchFrom;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			try
			{
				BossComponent component2;
				if (collision.gameObject.tag == "Projectile")
				{
					if (collision.gameObject.GetComponent<Projectile>().projectileType != ProjectileType.Steel && collision.gameObject.GetComponent<Projectile>().projectileType != ProjectileType.StoneShoot && isActive)
					{
						isActive = false;
						UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
						base.transform.parent = collision.gameObject.transform;
						GetComponent<TrailRenderer>().enabled = false;
						GetComponent<Collider2D>().enabled = false;
					}
				}
				else if (collision.gameObject.tag == "Catapult")
				{
					CatapultComponent component = collision.GetComponent<CatapultComponent>();
					if (component._catapult != null)
					{
						float magnitude = GetComponent<Rigidbody2D>().velocity.magnitude;
						if (component.CheckIfCriticalHit(magnitude))
						{
							_parent.ProjectileKillCatapult(component._catapult.gameObject.GetInstanceID());
						}
						component.ComponentHit(magnitude);
						UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
						base.transform.parent = collision.gameObject.transform;
						GetComponent<TrailRenderer>().enabled = false;
						GetComponent<Collider2D>().enabled = false;
						base.gameObject.AddComponent<ParticleDestroyer>();
					}
					else if (component._pvpCatapult != null)
					{
						float magnitude2 = GetComponent<Rigidbody2D>().velocity.magnitude;
						component.ComponentHit(magnitude2);
						UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
						base.transform.parent = collision.gameObject.transform;
						GetComponent<TrailRenderer>().enabled = false;
						GetComponent<Collider2D>().enabled = false;
						base.gameObject.AddComponent<ParticleDestroyer>();
					}
				}
				else if (collision.gameObject.tag == "Boss")
				{
					component2 = collision.GetComponent<BossComponent>();
					if (component2 != null)
					{
						if (!component2._jointRedirectEnabled)
						{
							goto IL_026a;
						}
						if (component2._jointRedirectEnabled)
						{
							Vector3 position = component2.transform.position;
							float x = position.x;
							Vector3 position2 = base.transform.position;
							if (x < position2.x)
							{
								goto IL_026a;
							}
						}
					}
				}
				else if (collision.gameObject.tag == "BossCharacter")
				{
					BossCharacterPart component3 = collision.GetComponent<BossCharacterPart>();
					if (component3._character != null)
					{
						component3.ComponentHit(GetComponent<Rigidbody2D>().velocity.magnitude);
						UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
						base.transform.parent = collision.gameObject.transform;
						GetComponent<TrailRenderer>().enabled = false;
						GetComponent<Collider2D>().enabled = false;
						base.gameObject.AddComponent<ParticleDestroyer>();
					}
				}
				else if (collision.gameObject.tag == "Player")
				{
					CharacterPart component4 = collision.GetComponent<CharacterPart>();
					if (component4._character != null)
					{
						component4._character.UnitHit(GetComponent<Rigidbody2D>().velocity.magnitude);
						UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
						base.transform.parent = collision.gameObject.transform;
						GetComponent<TrailRenderer>().enabled = false;
						GetComponent<Collider2D>().enabled = false;
						base.gameObject.AddComponent<ParticleDestroyer>();
					}
				}
				else if (collision.gameObject.tag == "CatapultParticle" || collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Tower")
				{
					if (isActive)
					{
						isActive = false;
						UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
						base.transform.parent = collision.gameObject.transform;
						GetComponent<TrailRenderer>().enabled = false;
						GetComponent<Collider2D>().enabled = false;
						base.gameObject.AddComponent<ParticleDestroyer>();
					}
				}
				else if (collision.gameObject.tag == "ProjectileParticle" && collision.gameObject.GetComponent<ProjectileShootPart>() == null)
				{
					isActive = false;
					UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
					base.transform.parent = collision.gameObject.transform;
					GetComponent<TrailRenderer>().enabled = false;
					GetComponent<Collider2D>().enabled = false;
					base.gameObject.AddComponent<ParticleDestroyer>();
				}
				goto end_IL_0000;
				IL_026a:
				float num = 0f;
				num = GetComponent<Rigidbody2D>().velocity.magnitude;
				BossComponent bossComponent = component2;
				float hitForce = num;
				Vector3? projectilePosition = base.transform.position;
				bossComponent.BossComponentHit(hitForce, null, projectilePosition);
				UnityEngine.Object.Destroy(GetComponent<Rigidbody2D>());
				base.transform.parent = collision.gameObject.transform;
				GetComponent<TrailRenderer>().enabled = false;
				GetComponent<Collider2D>().enabled = false;
				base.gameObject.AddComponent<ParticleDestroyer>();
				end_IL_0000:;
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
	}
}
