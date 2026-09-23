using Exploder2D;
using Exploder2D.Utils;
using UnityEngine;

namespace Logic
{
	public class TowerComponent : MonoBehaviour
	{
		public TowerLogic parentTower;

		private Exploder2DObject exploder;

		private float health = 25f;

		public bool isCriticlal;

		private void Start()
		{
			if (isCriticlal)
			{
				health = 40f;
			}
			else
			{
				health = 25f;
			}
			exploder = Exploder2DSingleton.Exploder2DInstance;
		}

		private void ComponentHit(float power)
		{
			health -= power;
			if (health < 0f)
			{
				ExplodeSprite();
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Projectile")
			{
				switch (collision.gameObject.GetComponent<Projectile>().projectileType)
				{
				case ProjectileType.Steel:
					ComponentHit(collision.GetComponent<Rigidbody2D>().velocity.magnitude);
					break;
				case ProjectileType.BossProjectile:
					ComponentHit(collision.GetComponent<Rigidbody2D>().velocity.magnitude / 2f);
					break;
				}
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (!(collision.gameObject.tag == "Boss"))
			{
			}
		}

		private void ExplodeSprite()
		{
			SoundMgr.instance.StoneBreak();
			parentTower.DestroyComponent(isCriticlal);
			GlobalLogic.instance._ExplodeHandler.ExplodeObject(base.gameObject);
		}

		private void Update()
		{
			if (parentTower.isDestroyed)
			{
				ExplodeSprite();
			}
		}
	}
}
