using Logic;
using UnityEngine;

namespace View
{
	public class EnemySpoonView : MonoBehaviour
	{
		public Transform rotatePoint;

		public CatapultLogic _catapult;

		public float[] rotateBoundaries;

		private float angleToRotate;

		private bool isShooting;

		private void Awake()
		{
			rotateBoundaries = new float[2]
			{
				0.6f,
				0.05f
			};
			Quaternion localRotation = base.transform.localRotation;
			angleToRotate = localRotation.z;
		}

		public void SpoonControl(int direction)
		{
			if (direction == 1)
			{
				Quaternion localRotation = base.transform.localRotation;
				if (localRotation.z < rotateBoundaries[0])
				{
					base.transform.RotateAround(rotatePoint.position, new Vector3(0f, 0f, 1f), 200f * Time.deltaTime);
				}
			}
			if (direction == 2)
			{
				Quaternion localRotation2 = base.transform.localRotation;
				if (localRotation2.z > rotateBoundaries[1])
				{
					base.transform.RotateAround(rotatePoint.position, new Vector3(0f, 0f, -1f), 200f * Time.deltaTime);
				}
			}
		}

		public bool SpoonShoot()
		{
			Quaternion localRotation = base.transform.localRotation;
			if (localRotation.z < 0.6f)
			{
				base.transform.RotateAround(rotatePoint.position, new Vector3(0f, 0f, 1f), 400f * Time.deltaTime);
				return false;
			}
			isShooting = false;
			Quaternion localRotation2 = base.transform.localRotation;
			angleToRotate = localRotation2.z;
			return true;
		}

		public void NextAngle(float angle)
		{
			angleToRotate = angle;
		}

		private void Update()
		{
			if (isShooting)
			{
				return;
			}
			float num = angleToRotate;
			Quaternion localRotation = base.transform.localRotation;
			if (!(num < localRotation.z - 0.02f))
			{
				float num2 = angleToRotate;
				Quaternion localRotation2 = base.transform.localRotation;
				if (!(num2 > localRotation2.z + 0.02f))
				{
					return;
				}
			}
			float num3 = angleToRotate;
			Quaternion localRotation3 = base.transform.localRotation;
			if (num3 < localRotation3.z)
			{
				SpoonControl(2);
			}
			else
			{
				SpoonControl(1);
			}
		}
	}
}
