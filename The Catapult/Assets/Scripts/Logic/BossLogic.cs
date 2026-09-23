using System.Collections;
using UnityEngine;

namespace Logic
{
	public class BossLogic : MonoBehaviour
	{
		public BossStage baseStage;

		public BossStage[] listOfStages;

		private TowerLogic _towerToDestroy;

		private int _currentStagesCount;

		private bool _isDestroyed;

		public int _stagesCount
		{
			get;
			private set;
		}

		public int _stagesCharactersCount
		{
			get;
			private set;
		}

		public bool _shootingAllowed
		{
			get;
			set;
		}

		private void Start()
		{
		}

		public void PrepareBoss()
		{
			baseStage.PrepareBossStage(this);
			for (int i = 0; i < listOfStages.Length; i++)
			{
				listOfStages[i].PrepareBossStage(this);
			}
		}

		public void InitBoss(int stagesCount)
		{
			_stagesCount = stagesCount;
			_currentStagesCount = _stagesCount;
			_stagesCharactersCount = _stagesCount;
			_shootingAllowed = false;
			if (_stagesCount < 1)
			{
				_stagesCount = 1;
			}
			if (_stagesCount > listOfStages.Length)
			{
				_stagesCount = listOfStages.Length;
			}
			baseStage.InitBossStage(-1, isLastStage: false);
			bool isLastStage = false;
			for (int i = 0; i < _stagesCount; i++)
			{
				if (i == _stagesCount - 1)
				{
					isLastStage = true;
				}
				listOfStages[i].InitBossStage(i, isLastStage);
			}
			if (NewDataController.instance.GetPlayerControl() == ControlType.LeftHand)
			{
				Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0f, 0f));
				float x = vector.x;
				baseStage.InitMovement(x);
			}
			else
			{
				Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0.2f, 0f, 0f));
				float x2 = vector2.x;
				baseStage.InitMovement(x2);
			}
			StartCoroutine(CheckCharacter());
		}

		public void StageDestroyed(int stageIndex)
		{
			if (_isDestroyed)
			{
				return;
			}
			switch (stageIndex)
			{
			case -1:
				StartCoroutine(BossDestroyed());
				StartCoroutine(StartPartDestroying(0));
				return;
			default:
				listOfStages[stageIndex - 1].PlayEmitterSystem();
				break;
			case 0:
				baseStage.PlayEmitterSystem();
				break;
			}
			_currentStagesCount--;
			for (int i = stageIndex + 1; i < _stagesCount; i++)
			{
				if (listOfStages[i].ChangeLayer())
				{
					baseStage.PauseMovement();
				}
			}
			int num = 0;
			for (int j = 0; j < _stagesCount && listOfStages[j]._isDestroyed; j++)
			{
				num++;
			}
			if (num == _stagesCount)
			{
				baseStage.StageCharge();
			}
			if (_currentStagesCount > 0)
			{
			}
		}

		public void CharacterKilled()
		{
			_stagesCharactersCount--;
			if (_stagesCharactersCount <= 0)
			{
				baseStage.StageCharge(allStagesDestroyed: false);
			}
		}

		public void TowerBecomeLower()
		{
			for (int i = 0; i < _stagesCount; i++)
			{
				listOfStages[i].ChangeLayer(inverse: true);
			}
		}

		private IEnumerator StartPartDestroying(int destroyFromStage)
		{
			for (int i = destroyFromStage; i < _stagesCount; i++)
			{
				listOfStages[i].StageDeath();
				yield return new WaitForSeconds(0.2f);
			}
		}

		public void TowerTouched(TowerLogic tower)
		{
			_towerToDestroy = tower;
		}

		public void DestroyPlayerTower()
		{
			_isDestroyed = true;
			SoundMgr.instance.BigBombExplosion();
			GlobalLogic.instance.gameEndedByBoss = true;
			_towerToDestroy.DestroyTower();
			StartCoroutine(StartPartDestroying(0));
		}

		private IEnumerator BossDestroyed()
		{
			_isDestroyed = true;
			InitController.instance.AddPoint();
			GameMenuControl.instance.UpdateScore();
			yield return new WaitForSeconds(5f);
			GlobalLogic.instance.DestroyBoss();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private IEnumerator CheckCharacter()
		{
			bool allDead;
			do
			{
				yield return new WaitForSeconds(1f);
				allDead = true;
				for (int i = 0; i < listOfStages.Length; i++)
				{
					if (!listOfStages[i].stageCharacter._isKilled)
					{
						allDead = false;
					}
				}
				if (_isDestroyed)
				{
					baseStage.StageCharge();
					yield break;
				}
			}
			while (!allDead);
			baseStage.StageCharge(allStagesDestroyed: false);
		}
	}
}
