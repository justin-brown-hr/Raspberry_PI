using UnityEngine;

namespace Logic
{
	internal class Ground : MonoBehaviour
	{
		public Texture2D StampTex;

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (!(collision.gameObject.GetComponent<GroundedDetail>() == null))
			{
				return;
			}
			string tag = collision.gameObject.tag;
			if (tag == null)
			{
				return;
			}
			if (!(tag == "Projectile"))
			{
				if (tag == "Player")
				{
					return;
				}
				if (!(tag == "CatapultParticle"))
				{
					if (tag == "ProjectileParticle" || !(tag == "Catapult"))
					{
						return;
					}
					if (NewDataController.instance.GetGameMode() == GameMode.Single)
					{
						if (collision.gameObject.GetComponent<CatapultComponent>()._catapult.beingDestroyed && collision.gameObject.GetComponent<CatapultComponent>().material == GameMaterial.Wood && collision.relativeVelocity.magnitude > 5f)
						{
							SoundMgr.instance.WoodSplash();
						}
					}
					else if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen && collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.isDestroyed && collision.gameObject.GetComponent<CatapultComponent>().material == GameMaterial.Wood && collision.relativeVelocity.magnitude > 5f)
					{
						SoundMgr.instance.WoodSplash();
					}
				}
				else if (NewDataController.instance.GetGameMode() == GameMode.Single && GlobalLogic.instance._currentBoss != null)
				{
					if (collision.relativeVelocity.magnitude > 15f)
					{
						SoundMgr.instance.WoodSplash();
					}
				}
				else if (collision.relativeVelocity.magnitude > 10f)
				{
					SoundMgr.instance.WoodSplash();
				}
			}
			else if (!(collision.gameObject.GetComponent<Projectile>() is ProjectileBomb))
			{
				collision.gameObject.AddComponent<GroundedDetail>().Init(1);
			}
		}
	}
}
