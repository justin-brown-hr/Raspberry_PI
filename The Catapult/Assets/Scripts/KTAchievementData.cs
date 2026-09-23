public class KTAchievementData
{
	public string identifier;

	public int percantageComplete;

	public bool isComplete;

	public KTAchievementData(string identifer, int percantageComplete, bool isComplete)
	{
		identifier = identifer;
		this.percantageComplete = percantageComplete;
		this.isComplete = isComplete;
	}
}
