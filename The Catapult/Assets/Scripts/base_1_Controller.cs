using UnityEngine;

public class base_1_Controller : MonoBehaviour
{
	private bool stat = true;

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!stat || !(other.gameObject.tag == "Projectile"))
		{
			return;
		}
		UnityEngine.Debug.Log("<---------- Collision ON GAME OBG NAME -------> " + base.gameObject.name);
		stat = false;
		string name = base.gameObject.name;
		if (name == null)
		{
			return;
		}
		if (!(name == "base_1"))
		{
			if (!(name == "base_2"))
			{
				if (!(name == "base_3"))
				{
					if (!(name == "wheel_1") && !(name == "wheel_2") && name == "val")
					{
					}
				}
				else
				{
					Cat1_Controller.Instance.Collision_Based_3();
				}
			}
			else
			{
				Cat1_Controller.Instance.Collision_Based_2();
			}
		}
		else
		{
			Cat1_Controller.Instance.Collision_Based_1();
		}
	}
}
