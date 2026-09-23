using UnityEngine;

public class Cat1_Controller : MonoBehaviour
{
	public static Cat1_Controller Instance;

	public GameObject base_1;

	public GameObject base_2;

	public GameObject base_3;

	public GameObject wheel_1;

	public GameObject wheel_2;

	public GameObject val;

	public GameObject base_1_1;

	public GameObject base_1_2;

	public GameObject base_1_3;

	public GameObject base_2_1;

	public GameObject base_2_2;

	public GameObject base_2_3;

	public GameObject base_3_1;

	public GameObject base_3_2;

	public GameObject base_3_3;

	public GameObject wheel_1_1;

	public GameObject wheel_1_2;

	public GameObject wheel_2_1;

	public GameObject wheel_2_2;

	public GameObject val_1;

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

	public void Collision_Based_1()
	{
		base_1.SetActive(value: false);
		base_1_1.SetActive(value: true);
		base_1_2.SetActive(value: true);
		base_1_3.SetActive(value: true);
	}

	public void Collision_Based_2()
	{
		base_2.SetActive(value: false);
		base_2_1.SetActive(value: true);
		base_2_2.SetActive(value: true);
		base_2_3.SetActive(value: true);
	}

	public void Collision_Based_3()
	{
		base_3.SetActive(value: false);
		base_3_1.SetActive(value: true);
		base_3_2.SetActive(value: true);
		base_3_3.SetActive(value: true);
	}

	public void Collision_Wheel_1()
	{
	}

	public void Collision_Wheel_2()
	{
	}

	public void Collision_Val()
	{
	}
}
