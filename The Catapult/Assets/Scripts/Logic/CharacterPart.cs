using Model;
using System;
using UnityEngine;

namespace Logic
{
	public class CharacterPart : MonoBehaviour
	{
		public CharacterLogic _character;

		public bool isHead;

		public bool isHands;

		public SpriteRenderer helmetSprite;

		public void EquipHelmet()
		{
			if (!(_character != null) || !isHead || (_character.playerSide != 0 && _character.playerSide != GameSides.Player2))
			{
				return;
			}
			Sprite currentHelmetSprite = UpgradeShopData.instance.GetCurrentHelmetSprite();
			if (currentHelmetSprite != null)
			{
				helmetSprite.sprite = currentHelmetSprite;
				if (NewDataController.instance.GetPlayerControl() == ControlType.RightHand && NewDataController.instance.GetGameMode() == GameMode.Single)
				{
					Transform transform = helmetSprite.transform;
					Vector3 position = helmetSprite.transform.position;
					float x = position.x;
					Vector3 position2 = helmetSprite.transform.position;
					float y = position2.y;
					Vector3 position3 = Camera.main.transform.position;
					transform.position = new Vector3(x, y, position3.z - 10f);
				}
			}
		}

		public void HelmetHitEffect()
		{
			if (NewDataController.instance.GetEquipedHelmet() > 0 && NewDataController.instance.GetEquipedHelmet() != 999 && isHead && (_character.playerSide == GameSides.Player1 || _character.playerSide == GameSides.Player2))
			{
				SoundMgr.instance.HelmetHit();
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Catapult" || collision.gameObject.tag == "FlyPlatform")
				{
					Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
				}
				if ((collision.gameObject.tag == "CatapultParticle" || collision.gameObject.tag == "ProjectileParticle" || collision.gameObject.tag == "ExploderFragment") && !isHands && GetComponent<GroundedDetail>() == null && collision.gameObject.GetComponent<GroundedDetail>() == null)
				{
					_character.ParticleHit(collision.relativeVelocity.magnitude);
					if (collision.contacts.Length > 0)
					{
						_character.ParticleEffect(collision.contacts[0].point, collision.relativeVelocity.magnitude, this);
					}
				}
				if (collision.gameObject.tag == "Projectile")
				{
					if (collision.gameObject.GetComponent<Projectile>().launchFrom != _character.playerSide)
					{
						if (GetComponent<GroundedDetail>() == null && collision.gameObject.GetComponent<GroundedDetail>() == null)
						{
							if (collision.gameObject.GetComponent<Projectile>().projectileType != ProjectileType.StoneFire)
							{
								if (collision.contacts.Length > 0)
								{
									_character.ParticleEffect(collision.contacts[0].point, collision.relativeVelocity.magnitude, this);
								}
							}
							else
							{
								_character.SetStickmanOnFire((collision.gameObject.GetComponent<Projectile>() as ProjectileFire).fireOnPlayer);
							}
						}
					}
					else
					{
						Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
					}
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
	}
}
