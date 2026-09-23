using UnityEngine;
using UnityEngine.UI;

public class NamesGame : MonoBehaviour
{
	public static NamesGame Instance;

	public Text textGame;

	private void Start()
	{
		Instance = this;
	}

	private void Update()
	{
	}

	public void GetName(string nameGame)
	{
		textGame.text = nameGame;
	}
}
