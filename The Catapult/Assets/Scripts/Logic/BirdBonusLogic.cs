using UnityEngine;

namespace Logic
{
	internal class BirdBonusLogic : MonoBehaviour
	{
		private Rigidbody2D _rigidbody;

		public GameObject hitPrefab;

		private AudioSource _source;

		public Transform particleSpawn;

		private float birdSpeed;

		private int birdBonusCoins;

		private bool isIgnoring;

		private float ignoringTime;

		private bool isMenuBird;

		private bool clicked;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			birdBonusCoins = 30;
			birdSpeed = 10f;
			clicked = false;
		}

		public void Init(int side, bool isMenu = false)
		{
			if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand || NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen)
			{
				birdSpeed *= side;
				Transform transform = base.transform;
				Vector3 localScale = base.transform.localScale;
				float x = localScale.x * -1f * (float)side;
				Vector3 localScale2 = base.transform.localScale;
				transform.localScale = new Vector3(x, localScale2.y);
			}
			else
			{
				birdSpeed *= side * -1;
				Transform transform2 = base.transform;
				Vector3 localScale3 = base.transform.localScale;
				float x2 = localScale3.x * (float)side;
				Vector3 localScale4 = base.transform.localScale;
				transform2.localScale = new Vector3(x2, localScale4.y);
			}
			SoundMgr.instance.CrowScream();
			isMenuBird = isMenu;
		}

		public void KilledByPlayer(GameSides side)
		{
			if (base.gameObject.GetComponent<Renderer>().isVisible && (!isMenuBird || clicked))
			{
				base.gameObject.layer = 0;
				SoundMgr.instance.CrowHit();
				Object.Instantiate(hitPrefab, particleSpawn.position - new Vector3(0f, 0.5f, 0f), Quaternion.identity);
				if (NewDataController.instance.GetGameMode() == GameMode.Single)
				{
					GlobalLogic.instance.AddCoins(birdBonusCoins, isBonus: true);
					AchievementManager.instance.AchievementProgress(AchieventType.CrowKillSimple, AchievementRegion.AllGame, 1);
					AchievementManager.instance.AchievementProgress(AchieventType.CrowKillReward, AchievementRegion.AllGame, birdBonusCoins);
				}
				else if (NewDataController.instance.GetGameMode() == GameMode.PvPOneScreen)
				{
					PvPIngameShop.instance.BirdKilled(side);
				}
				else if (NewDataController.instance.GetGameMode() == GameMode.None)
				{
					NewDataController.instance.AddMoney(1);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
			if (collision.gameObject.tag == "Projectile")
			{
				if (collision.gameObject.GetComponent<Projectile>().launchFrom != GameSides.AI && collision.gameObject.GetComponent<Projectile>().projectileType != ProjectileType.Bomb)
				{
					KilledByPlayer(collision.gameObject.GetComponent<Projectile>().launchFrom);
				}
				else if (collision.gameObject.GetComponent<Projectile>().launchFrom == GameSides.AI)
				{
					base.gameObject.layer = 1;
					isIgnoring = true;
					ignoringTime = 0.2f;
				}
			}
			if (collision.gameObject.tag == "ProjectileParticle" && collision.gameObject.GetComponent<ProjectileShootPart>() != null)
			{
				KilledByPlayer(collision.gameObject.GetComponent<ProjectileShootPart>().GetGameSides());
				AchievementManager.instance.AchievementProgress(AchieventType.CrowKillThorn, AchievementRegion.AllGame, 1);
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
			if (collision.gameObject.tag == "Projectile")
			{
				if (collision.gameObject.GetComponent<Projectile>().launchFrom != GameSides.AI && collision.gameObject.GetComponent<Projectile>().projectileType == ProjectileType.Bomb)
				{
					KilledByPlayer(collision.gameObject.GetComponent<Projectile>().launchFrom);
				}
				else
				{
					Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
				}
			}
			if (collision.gameObject.tag == "ProjectileParticle" && collision.gameObject.GetComponent<ProjectileShootPart>() != null)
			{
				KilledByPlayer(collision.gameObject.GetComponent<ProjectileShootPart>().GetGameSides());
				AchievementManager.instance.AchievementProgress(AchieventType.CrowKillThorn, AchievementRegion.AllGame, 1);
			}
		}

		private void OnMouseDown()
		{
			if (isMenuBird)
			{
				clicked = true;
				KilledByPlayer(GameSides.Player1);
			}
		}

		private void Update()
		{
			if (isIgnoring)
			{
				if (ignoringTime > 0f)
				{
					ignoringTime -= Time.deltaTime;
				}
				else
				{
					isIgnoring = false;
					base.gameObject.layer = 16;
				}
			}
			_rigidbody.MovePosition(base.transform.position + -1f * base.transform.right * birdSpeed * Time.deltaTime);
			Vector3 position = base.transform.position;
			if (!(position.x > 70f))
			{
				Vector3 position2 = base.transform.position;
				if (!(position2.x < -70f))
				{
					return;
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
