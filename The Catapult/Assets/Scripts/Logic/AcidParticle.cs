using System;
using UnityEngine;

namespace Logic
{
	internal class AcidParticle : MonoBehaviour
	{
		public GameObject playerHit;

		public GameObject catapultHit;

		private GameSides launchedFrom;

		public void Init(GameSides from)
		{
			Rigidbody2D component = GetComponent<Rigidbody2D>();
			UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			float num = UnityEngine.Random.Range(-0.5f, 1f);
			float num2 = UnityEngine.Random.Range(-1f, 1f);
			float num3 = UnityEngine.Random.Range(25f, 37.5f);
			component.velocity = new Vector2(num * num3, num2 * num3);
			launchedFrom = from;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Tower" || collision.gameObject.tag == "Boss")
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (collision.gameObject.tag == "Player")
			{
				CharacterPart component = collision.gameObject.GetComponent<CharacterPart>();
				if (component != null && component._character.playerSide != launchedFrom)
				{
					SoundMgr.instance.AcidExplode();
					component._character.PoisonStickman(playerHit);
				}
			}
			else if (collision.gameObject.tag == "Catapult")
			{
				SoundMgr.instance.AcidExplode();
				if (NewDataController.instance.GetGameMode() == GameMode.Single)
				{
					if (collision.gameObject.GetComponent<CatapultComponent>()._catapult.playerSide != launchedFrom)
					{
						collision.gameObject.GetComponent<CatapultComponent>()._catapult.PosionCatapult(catapultHit, playerHit);
					}
				}
				else
				{
					collision.gameObject.GetComponent<CatapultComponent>()._pvpCatapult.PosionCatapult(catapultHit);
				}
			}
			else if (collision.gameObject.tag == "BossCharacter" && collision.GetComponent<BossCharacterPart>()._canBePoisoned)
			{
				SoundMgr.instance.AcidExplode();
				collision.GetComponent<BossCharacterPart>().PoisonCharacter(playerHit);
			}
		}
	}
}
