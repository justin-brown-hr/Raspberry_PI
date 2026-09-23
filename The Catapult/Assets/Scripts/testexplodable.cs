using Logic;
using UnityEngine;

public class testexplodable : MonoBehaviour
{
	public void Update()
	{
		if (UnityEngine.Input.GetKeyDown("g"))
		{
			GlobalLogic.instance._ExplodeHandler.ExplodeObject(base.gameObject);
		}
	}
}
