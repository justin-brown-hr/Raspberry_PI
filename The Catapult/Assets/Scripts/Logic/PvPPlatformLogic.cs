using UnityEngine;

namespace Logic
{
	internal class PvPPlatformLogic : MonoBehaviour
	{
		public Transform spawnPoint;

		public GameObject[] winnerSalutes;

		private GameSides _side;

		private float targetHeight;

		private bool isRespawning;

		private bool isPreparing;

		private float lowerHeight;

		private bool soundPlaying;

		private void Awake()
		{
			for (int i = 0; i < winnerSalutes.Length; i++)
			{
				winnerSalutes[i].SetActive(value: false);
			}
			lowerHeight = -999f;
		}

		public void InitPlatform(GameSides side)
		{
			if (lowerHeight == -999f)
			{
				Vector3 position = base.transform.position;
				lowerHeight = position.y;
			}
			_side = side;
			targetHeight = UnityEngine.Random.Range(lowerHeight, lowerHeight + 16f);
			isPreparing = true;
			soundPlaying = true;
			SoundMgr.instance.TowerMove(_side);
		}

		public void Respawn()
		{
			isPreparing = false;
			targetHeight = lowerHeight;
			isRespawning = true;
		}

		public void Winning()
		{
			for (int i = 0; i < winnerSalutes.Length; i++)
			{
				winnerSalutes[i].SetActive(value: true);
			}
		}

		private void Update()
		{
			if (isPreparing)
			{
				Vector3 position = base.transform.position;
				if (position.y < targetHeight)
				{
					Transform transform = base.transform;
					Vector3 position2 = base.transform.position;
					Vector3 position3 = base.transform.position;
					float x = position3.x;
					float y = targetHeight;
					Vector3 position4 = base.transform.position;
					transform.position = Vector3.MoveTowards(position2, new Vector3(x, y, position4.z), 10f * Time.deltaTime);
				}
				else
				{
					isPreparing = false;
					if (soundPlaying)
					{
						soundPlaying = false;
						SoundMgr.instance.TowerStop(_side);
					}
				}
			}
			if (isRespawning && !PvPLogic.instance.isEndGame)
			{
				Vector3 position5 = base.transform.position;
				if (position5.y > targetHeight)
				{
					Transform transform2 = base.transform;
					Vector3 position6 = base.transform.position;
					Vector3 position7 = base.transform.position;
					float x2 = position7.x;
					float y2 = targetHeight;
					Vector3 position8 = base.transform.position;
					transform2.position = Vector3.MoveTowards(position6, new Vector3(x2, y2, position8.z), 20f * Time.deltaTime);
				}
				else
				{
					isRespawning = false;
					PvPLogic.instance.PlatformReady(_side);
				}
			}
		}
	}
}
