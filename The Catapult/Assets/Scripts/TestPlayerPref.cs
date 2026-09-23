using UnityEngine;

public class TestPlayerPref : MonoBehaviour
{
	private string str_StatusCatapult_1 = "status_Catepult_1";

	private string str_StatusCatapult_2 = "status_Catepult_2";

	private string str_StatusCatapult_3 = "status_Catepult_3";

	public int Sost_Catapult_1;

	public int Sost_Catapult_2;

	public int Sost_Catapult_3;

	public int Sost_OpenedCatapult;

	private void Start()
	{
		Inspection_OldCatapults();
	}

	private void Inspection_OldCatapults()
	{
	}

	private void Add_UpgradeCatapult()
	{
	}

	public int Next_ActualCatapult()
	{
		int result = 0;
		if (Sost_Catapult_1 > 0)
		{
			result = 0;
		}
		if (Sost_Catapult_2 > 0)
		{
			result = 1;
		}
		if (Sost_Catapult_3 > 0)
		{
			result = 2;
		}
		return result;
	}
}
