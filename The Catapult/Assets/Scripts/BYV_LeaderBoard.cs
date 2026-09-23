using UnityEngine;

public class BYV_LeaderBoard : MonoBehaviour
{
	public static BYV_LeaderBoard Instance;

	private bool loginSuccessful;

	private string leaderboardID = "TheCatapult_LeaderBoard";

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	public void AuthenticateUser()
	{
		Social.localUser.Authenticate(delegate(bool success)
		{
			if (success)
			{
				loginSuccessful = true;
				UnityEngine.Debug.Log("---- 1 -- LeaderBord ------> succes");
			}
			else
			{
				UnityEngine.Debug.Log("---- 1 -- LeaderBord ------> “unsuccessful”");
			}
		});
	}

	public void Show_LeaderBoard()
	{
		UnityEngine.Debug.Log("---- Нажали кнопку Показать Таблицу лидеров --------");
		Social.ShowLeaderboardUI();
	}

	public void PostScoreOnLeaderBoard(int q, int z)
	{
		int zxc = q + z;
		if (loginSuccessful)
		{
			Social.ReportScore(zxc, leaderboardID, delegate(bool success)
			{
				if (success)
				{
					UnityEngine.Debug.Log("------ Загрузка данных на LiderBoard прошла удачно -----");
				}
			});
		}
		else
		{
			Social.localUser.Authenticate(delegate(bool success)
			{
				if (success)
				{
					loginSuccessful = true;
					Social.ReportScore(zxc, leaderboardID, delegate
					{
						UnityEngine.Debug.Log("---- 2 -- LeaderBord ------> succes");
					});
				}
				else
				{
					UnityEngine.Debug.Log("---- 2 -- LeaderBord ------> “unsuccessful”");
				}
			});
		}
	}
}
