using UnityEngine;

namespace Logic
{
	public class TowerLogic : MonoBehaviour
	{
		public CatapultLogic _catapult;

		public GameObject[] otkats;

		public SpriteRenderer towerBckg;

		public Sprite[] towerStates;

		private float killTimer;

		private int numberOfPoints;

		private int currentPoints;

		internal bool isDestroyed;

		private int towerHits;

		private int destroyState;

		private void Start()
		{
			numberOfPoints = 4;
			currentPoints = 0;
			towerHits = 40;
			isDestroyed = false;
			destroyState = 0;
		}

		public void DestroyTower()
		{
			currentPoints = numberOfPoints + 1;
			DestroyComponent(isCritical: true);
		}

		public void DestroyComponent(bool isCritical)
		{
			if (isCritical)
			{
				currentPoints++;
			}
			towerHits--;
			if (destroyState == 0 && towerHits < 30)
			{
				destroyState = 1;
				towerBckg.sprite = towerStates[0];
			}
			else if (destroyState == 1 && towerHits < 15)
			{
				destroyState = 2;
				towerBckg.sprite = towerStates[1];
			}
			if (currentPoints >= numberOfPoints && !isDestroyed)
			{
				if (_catapult != null && !_catapult.beingDestroyed)
				{
					_catapult.PlayerTowerDestroyed();
				}
				towerBckg.sprite = towerStates[1];
				isDestroyed = true;
				UnityEngine.Object.Destroy(otkats[0]);
				UnityEngine.Object.Destroy(otkats[1]);
			}
		}
	}
}
