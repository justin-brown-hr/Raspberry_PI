namespace Logic
{
	internal class Post5SaveData
	{
		public int currentCatapult;

		public int[] catapultProgress;

		public bool moneySet;

		public float money;

		public bool aimingSet;

		public int aiming;

		public bool ControlSet;

		public int control;

		public bool NotifySet;

		public int notify;

		public bool helmetSetted;

		public int currentHelmet;

		public bool[] helmetStatesSetted;

		public int[] helmetStates;

		public bool shieldSetted;

		public int currentShield;

		public bool[] shieldStatesSetted;

		public int[] shieldStates;

		public Post5SaveData()
		{
			catapultProgress = new int[3];
			helmetStates = new int[10];
			helmetStatesSetted = new bool[10];
			shieldStates = new int[6];
			shieldStatesSetted = new bool[6];
		}
	}
}
