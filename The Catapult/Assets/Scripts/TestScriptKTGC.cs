using System.Collections;
using UnityEngine;

public class TestScriptKTGC : MonoBehaviour
{
	private void Start()
	{
		KTGameCenter.SharedCenter().Authenticate();
	}

	private void OnEnable()
	{
		StartCoroutine(RegisterForGameCenter());
	}

	private void OnDisable()
	{
		KTGameCenter.SharedCenter().GCUserAuthenticated -= GCAuthentication;
		KTGameCenter.SharedCenter().GCScoreSubmitted -= ScoreSubmitted;
		KTGameCenter.SharedCenter().GCAchievementSubmitted -= AchievementSubmitted;
		KTGameCenter.SharedCenter().GCAchievementsReset -= AchivementsReset;
		KTGameCenter.SharedCenter().GCMyScoreFetched -= MyScoreFetched;
		KTGameCenter.SharedCenter().GCAchivementsFetched -= AchievementsFetched;
	}

	private IEnumerator RegisterForGameCenter()
	{
		yield return new WaitForSeconds(0.5f);
		KTGameCenter.SharedCenter().GCUserAuthenticated += GCAuthentication;
		KTGameCenter.SharedCenter().GCScoreSubmitted += ScoreSubmitted;
		KTGameCenter.SharedCenter().GCAchievementSubmitted += AchievementSubmitted;
		KTGameCenter.SharedCenter().GCAchievementsReset += AchivementsReset;
		KTGameCenter.SharedCenter().GCMyScoreFetched += MyScoreFetched;
		KTGameCenter.SharedCenter().GCAchivementsFetched += AchievementsFetched;
	}

	private void OnGUI()
	{
		if (!KTGameCenter.SharedCenter().IsGameCenterAuthenticated())
		{
			GUI.skin.label.fontSize = 20;
			GUI.Label(new Rect(10f, 150f, 200f, 50f), "Authenticating!");
			return;
		}
		GUI.skin.button.fontSize = 20;
		if (GUI.Button(new Rect(10f, 150f, 300f, 60f), "Show Leaderboards"))
		{
			KTGameCenter.SharedCenter().ShowLeaderboard();
		}
		if (GUI.Button(new Rect(10f, 250f, 250f, 60f), "Submit Score"))
		{
			KTGameCenter.SharedCenter().SubmitScore(110, "grp.com.kashiftasneem.thedarkshadow.highestscoresinglerun");
		}
		if (GUI.Button(new Rect(300f, 250f, 250f, 60f), "Submit Achievement"))
		{
			KTGameCenter.SharedCenter().SubmitAchievement(100, "grp.com.kashiftasneem.thedarkshadow.kill1zombie", showBanner: true);
		}
		if (GUI.Button(new Rect(10f, 350f, 300f, 60f), "Reset Achievement"))
		{
			KTGameCenter.SharedCenter().ResetAchievements();
		}
		if (GUI.Button(new Rect(330f, 350f, 250f, 60f), "Submit Float Score"))
		{
			KTGameCenter.SharedCenter().SubmitFloatScore(110.123f, 3, "com.kashiftasneem.flyingbird.testfloat");
		}
		if (GUI.Button(new Rect(10f, 450f, 250f, 60f), "Submit Time"))
		{
			KTGameCenter.SharedCenter().SubmitFloatScore(2459.3f, 2, "com.kashiftasneem.flyingbird.testtime");
		}
		if (GUI.Button(new Rect(10f, 550f, 250f, 60f), "Fetch my Score"))
		{
			KTGameCenter.SharedCenter().FetchMyScore("grp.com.kashiftasneem.thedarkshadow.highestscoresinglerun");
		}
		if (GUI.Button(new Rect(330f, 550f, 250f, 60f), "Fetch Achievement"))
		{
			KTAchievementData kTAchievementData = KTGameCenter.SharedCenter().FetchAchievementData("grp.com.kashiftasneem.thedarkshadow.kill1zombie");
			if (kTAchievementData != null)
			{
				MonoBehaviour.print("identifer= " + kTAchievementData.identifier + " percantageComplete= " + kTAchievementData.percantageComplete + " isComplete= " + kTAchievementData.isComplete);
			}
		}
	}

	private void GCAuthentication(string status)
	{
		MonoBehaviour.print("delegate call back status= " + status);
		StartCoroutine(CheckAttributes());
	}

	private void ScoreSubmitted(string leaderboardId, string error)
	{
		MonoBehaviour.print("score submitted with id " + leaderboardId + " and error= " + error);
	}

	private void AchievementSubmitted(string achId, string error)
	{
		MonoBehaviour.print("achievement submitted with id " + achId + " and error= " + error);
	}

	private void AchivementsReset(string error)
	{
		MonoBehaviour.print("Achievment reset with error= " + error);
	}

	private void MyScoreFetched(string leaderboardId, int score, string error)
	{
		MonoBehaviour.print("My score for leaderboardId= " + leaderboardId + " is " + score + " with error= " + error);
	}

	private void AchievementsFetched()
	{
		MonoBehaviour.print("Achievements data fetched");
	}

	private IEnumerator CheckAttributes()
	{
		yield return new WaitForSeconds(1f);
		MonoBehaviour.print(" alias= " + KTGameCenter.SharedCenter().PlayerAlias + " name= " + KTGameCenter.SharedCenter().PlayerName + " id= " + KTGameCenter.SharedCenter().PlayerId);
	}
}
