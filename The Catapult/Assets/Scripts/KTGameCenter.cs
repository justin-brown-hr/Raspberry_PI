using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class KTGameCenter : MonoBehaviour
{
	private enum GCStatus
	{
		kGCAuthenticating,
		kGCAuthenticated
	}

	public delegate void UserAuthenticatationDelegate(string value);

	public delegate void ScoreSubmissionDelegate(string leaderboardId, string error);

	public delegate void AchievementSubmissionDelegate(string achId, string error);

	public delegate void ResetAchievementsDelegate(string error);

	public delegate void MyScoreDelegate(string leaderboardId, int score, string error);

	public delegate void AchievementsFetchedDelegate();

	private static KTGameCenter _instance;

	private GCStatus currentStatus;

	private string playerName;

	private string playerAlias;

	private string playerId;

	private List<KTAchievementData> achievements = new List<KTAchievementData>();

	public string PlayerAlias => playerAlias;

	public string PlayerName => playerName;

	public string PlayerId => playerId;

	public event UserAuthenticatationDelegate GCUserAuthenticated;

	public event ScoreSubmissionDelegate GCScoreSubmitted;

	public event AchievementSubmissionDelegate GCAchievementSubmitted;

	public event ResetAchievementsDelegate GCAchievementsReset;

	public event MyScoreDelegate GCMyScoreFetched;

	public event AchievementsFetchedDelegate GCAchivementsFetched;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
		}
		else if (_instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public static KTGameCenter SharedCenter()
	{
		if (!_instance)
		{
			_instance = (UnityEngine.Object.FindObjectOfType(typeof(KTGameCenter)) as KTGameCenter);
			if (!_instance)
			{
				GameObject gameObject = new GameObject("KTGameCenter");
				_instance = gameObject.AddComponent<KTGameCenter>();
			}
			else
			{
				_instance.gameObject.name = "KTGameCenter";
			}
		}
		return _instance;
	}

	public void Authenticate()
	{
	}

	public void ShowLeaderboard(string leadboardId = null)
	{
	}

	public void ShowAchievements()
	{
	}

	public void ResetAchievements()
	{
	}

	public void SubmitScore(int score, string leaderboardId)
	{
	}

	public void SubmitFloatScore(float score, int decimals, string leaderboardId)
	{
	}

	public void SubmitAchievement(int percantage, string achivementId, bool showBanner)
	{
	}

	public void SubmitIncrementalAchievement(float percantage, string achivementId, bool showBanner)
	{
	}

	public bool IsGameCenterAuthenticated()
	{
		if (currentStatus == GCStatus.kGCAuthenticated)
		{
			return true;
		}
		return false;
	}

	public void FetchMyScore(string leaderboardId)
	{
	}

	private void IsAuthenticated(string error)
	{
		if (error == string.Empty)
		{
			currentStatus = GCStatus.kGCAuthenticated;
			if (this.GCUserAuthenticated != null)
			{
				this.GCUserAuthenticated("Authenticated");
			}
		}
		else if (this.GCUserAuthenticated != null)
		{
			this.GCUserAuthenticated(error);
		}
	}

	private void GameCenterAvailable(string value)
	{
	}

	private void ProcessGC(string error)
	{
	}

	private void ReloadScoresCompleted(string result)
	{
	}

	private void ScoreSubmitted(string result)
	{
		string[] array = result.Split('_');
		string leaderboardId = array[0];
		string error = array[1];
		if (this.GCScoreSubmitted != null)
		{
			this.GCScoreSubmitted(leaderboardId, error);
		}
	}

	private void AchievementSubmitted(string result)
	{
		string[] array = result.Split('_');
		string achId = array[0];
		string error = array[1];
		if (this.GCAchievementSubmitted != null)
		{
			this.GCAchievementSubmitted(achId, error);
		}
	}

	private void AchievementReset(string error)
	{
		if (this.GCAchievementsReset != null)
		{
			this.GCAchievementsReset(error);
		}
	}

	private void SetVariables(string val)
	{
		string[] array = val.Split('_');
		playerAlias = array[0];
		playerName = array[1];
		playerId = array[2];
	}

	private void ScoreFetched(string val)
	{
		string[] array = val.Split('_');
		string leaderboardId = array[0];
		string s = array[1];
		string error = array[2];
		int result = 0;
		int.TryParse(s, out result);
		if (this.GCMyScoreFetched != null)
		{
			this.GCMyScoreFetched(leaderboardId, result, error);
		}
	}

	private void AchievementsUpdated(string val)
	{
		achievements = new List<KTAchievementData>();
		string[] array = val.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				string[] array3 = text.Split('_');
				string identifer = array3[0];
				int percantageComplete = int.Parse(array3[1]);
				int num = int.Parse(array3[2]);
				KTAchievementData item = new KTAchievementData(identifer, percantageComplete, (num == 1) ? true : false);
				achievements.Add(item);
			}
		}
		if (this.GCAchivementsFetched != null)
		{
			this.GCAchivementsFetched();
		}
	}

	public KTAchievementData FetchAchievementData(string identifer)
	{
		KTAchievementData kTAchievementData = null;
		if (achievements != null)
		{
			for (int i = 0; i < achievements.Count; i++)
			{
				if (achievements[i].identifier == identifer)
				{
					kTAchievementData = achievements[i];
					break;
				}
			}
		}
		if (kTAchievementData == null)
		{
			kTAchievementData = new KTAchievementData(identifer, 0, isComplete: false);
		}
		return kTAchievementData;
	}
}
