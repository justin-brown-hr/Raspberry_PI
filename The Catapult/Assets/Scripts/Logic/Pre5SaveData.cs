namespace Logic
{
	internal class Pre5SaveData
	{
		public int catapultLevel;

		public int catapultType;

		public int[] catapultProgress;

		public bool moneySet;

		public float money;

		public bool aimingSet;

		public int aiming;

		public bool ControlSet;

		public int control;

		public bool helmetSetted;

		public int currentHelmet;

		public bool[] helmetStatesSetted;

		public int[] helmetStates;

		public bool shieldSetted;

		public int currentShield;

		public bool[] shieldStatesSetted;

		public int[] shieldStates;

		public Pre5SaveData()
		{
			catapultProgress = new int[3];
			helmetStates = new int[10];
			helmetStatesSetted = new bool[10];
			shieldStates = new int[6];
			shieldStatesSetted = new bool[6];
		}
	}
}
