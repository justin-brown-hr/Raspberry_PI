using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
	public int score;

	private void Awake()
	{
	}

	private void Update()
	{
		GetComponent<Text>().text = "Exploder 2D: " + score;
	}
}
