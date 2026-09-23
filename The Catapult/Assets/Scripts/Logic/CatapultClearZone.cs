using UnityEngine;

namespace Logic
{
	public class CatapultClearZone : MonoBehaviour
	{
		public CatapultLogic _protectedCatapult;

		public PvPCatapultLogic _protectedPvPCatapult;

		private GameSides _protectedSide;

		private void Start()
		{
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				_protectedSide = _protectedCatapult.playerSide;
			}
			else
			{
				_protectedSide = _protectedPvPCatapult.playerSide;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Projectile")
			{
				if (collision.gameObject.GetComponent<Projectile>().launchFrom != _protectedSide && collision.gameObject.GetComponent<GroundedDetail>() == null)
				{
					collision.gameObject.AddComponent<GroundedDetail>().Init(1, catapultShieldCalled: true);
				}
			}
			else if (collision.gameObject.tag == "CatapultParticle" || collision.gameObject.tag == "ProjectileParticle" || collision.gameObject.tag == "ExploderFragment")
			{
				collision.gameObject.AddComponent<GroundedDetail>().Init(2, catapultShieldCalled: true);
			}
		}
	}
}
