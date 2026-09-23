using UnityEngine;

namespace Logic
{
	internal class GroundedDetail : MonoBehaviour
	{
		internal float liveTime;

		private int detailType;

		private bool enabledTransmition;

		public void Init(int type, bool catapultShieldCalled = false)
		{
			detailType = type;
			enabledTransmition = !catapultShieldCalled;
			switch (detailType)
			{
			case 1:
				GetComponent<Projectile>().rotateAllowed = false;
				if (!catapultShieldCalled)
				{
					GetComponent<Projectile>().enabled = false;
				}
				GetComponent<Rigidbody2D>().drag = 0.5f;
				break;
			}
			if (NewDataController.instance.GetGameMode() == GameMode.Single)
			{
				if (catapultShieldCalled)
				{
					liveTime = 3.5f;
				}
				else if (GetComponentInChildren<ParticleSystem>() != null)
				{
					liveTime = 4f;
				}
				else if (NewDataController.instance.GetCurrentCatapultIndex() == 2)
				{
					liveTime = 3.5f;
				}
				else
				{
					liveTime = 4.5f;
				}
			}
			else
			{
				liveTime = 4f;
			}
		}

		private void DestroyDetail()
		{
			switch (detailType)
			{
			case 1:
				GetComponent<Projectile>().enabled = true;
				GetComponent<Projectile>().ReturnToPool();
				break;
			case 2:
				base.gameObject.SetActive(value: false);
				break;
			}
			UnityEngine.Object.Destroy(this);
		}

		private void Update()
		{
			if (!GetComponent<Renderer>().isVisible)
			{
				DestroyDetail();
			}
			if (liveTime > 0f)
			{
				liveTime -= Time.deltaTime;
			}
			else
			{
				DestroyDetail();
			}
		}
	}
}
