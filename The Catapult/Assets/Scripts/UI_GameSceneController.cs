using UnityEngine;

public class UI_GameSceneController : MonoBehaviour
{
	public void Button_ShowLeaderbord()
	{
		if (Unity_SavedGame.Instance.GetStatus_PlayService())
		{
			Leaderboard.Instance.ShowLeaderboard();
		}
	}
}
