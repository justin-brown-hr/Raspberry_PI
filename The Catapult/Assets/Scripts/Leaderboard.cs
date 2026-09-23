using GooglePlayGames;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
	public static Leaderboard Instance;

	private int score;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Invoke("SignIn", 5f);
	}

	private void SignIn()
	{
		if (Unity_SavedGame.Instance != null && Unity_SavedGame.Instance.GetStatus_PlayService())
		{
			Social.localUser.Authenticate(delegate(bool success)
			{
				if (success)
				{
					UnityEngine.Debug.Log("------------------- Мы подключились к Сервисам от гугл ------------------->");
				}
				else
				{
					UnityEngine.Debug.Log(" --- Local User NO Authenticate  (NO success) ---");
					score++;
				}
			});
		}
	}

	public void IncludeScoreInLeaderboard(int q, int z)
	{
		int num = q + z;
		if (!Unity_SavedGame.Instance.GetStatus_PlayService())
		{
			return;
		}
		if (!PlayGamesPlatform.Instance.localUser.authenticated)
		{
			if (score < 3)
			{
				SignIn();
			}
		}
		else
		{
			Social.ReportScore(num, "CgkIkYaDjYwaEAIQAQ", delegate
			{
				UnityEngine.Debug.Log("------------------- Send data in Leaderboord ------------------->");
			});
		}
	}

	public void ShowLeaderboard()
	{
		if (Unity_SavedGame.Instance != null && Unity_SavedGame.Instance.GetStatus_PlayService())
		{
			if (!PlayGamesPlatform.Instance.localUser.authenticated)
			{
				SignIn();
			}
			else
			{
				PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIkYaDjYwaEAIQAQ");
			}
		}
	}
}
