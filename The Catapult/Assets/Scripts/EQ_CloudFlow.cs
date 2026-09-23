using System;
using System.Collections;
using UnityEngine;

public class EQ_CloudFlow : MonoBehaviour
{
	[HideInInspector]
	public Cloud[] m_CloudList;

	public bool m_EnableLargeCloudLoop;

	public eCloudFlowBehavior m_Behavior = eCloudFlowBehavior.FlowTheSameWay;

	public float m_MinSpeed = 0.05f;

	public float m_MaxSpeed = 0.3f;

	public Camera m_Camera;

	private Vector3 LeftMostOfScreen;

	private Vector3 RightMostOfScreen;

	private void Start()
	{
		m_CloudList = new Cloud[base.transform.childCount];
		int num = UnityEngine.Random.Range(0, 2);
		int num2 = 0;
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				m_CloudList[num2] = new Cloud();
				m_CloudList[num2].m_MoveSpeed = UnityEngine.Random.Range(m_MinSpeed, m_MaxSpeed);
				if (num == 0)
				{
					m_CloudList[num2].m_MoveSpeed *= -1f;
					if (m_Behavior == eCloudFlowBehavior.SwitchLeftRight)
					{
						num = 1;
					}
				}
				else if (m_Behavior == eCloudFlowBehavior.SwitchLeftRight)
				{
					num = 0;
				}
				m_CloudList[num2].m_Cloud = transform.gameObject;
				if (m_EnableLargeCloudLoop)
				{
					m_CloudList[num2].m_CloudFollower = UnityEngine.Object.Instantiate(transform.gameObject);
				}
				m_CloudList[num2].m_OriginalLocalPos = m_CloudList[num2].m_Cloud.transform.localPosition;
				num2++;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		if (m_EnableLargeCloudLoop)
		{
			Cloud[] cloudList = m_CloudList;
			foreach (Cloud cloud in cloudList)
			{
				cloud.m_CloudFollower.transform.parent = base.transform;
			}
		}
		FindTheOrthographicCamera();
	}

	private void Update()
	{
		if (m_Camera == null)
		{
			FindTheOrthographicCamera();
		}
		if (m_Camera == null)
		{
			UnityEngine.Debug.LogWarning("There is no Orthographic camera in the scene.");
			return;
		}
		int num = 0;
		Cloud[] cloudList = m_CloudList;
		foreach (Cloud cloud in cloudList)
		{
			if (cloud.m_Cloud.activeSelf)
			{
				Transform transform = m_CloudList[num].m_Cloud.transform;
				Vector3 localPosition = m_CloudList[num].m_Cloud.transform.localPosition;
				float x = localPosition.x + m_CloudList[num].m_MoveSpeed * Time.deltaTime;
				Vector3 localPosition2 = m_CloudList[num].m_Cloud.transform.localPosition;
				float y = localPosition2.y;
				Vector3 localPosition3 = m_CloudList[num].m_Cloud.transform.localPosition;
				transform.localPosition = new Vector3(x, y, localPosition3.z);
				if (m_CloudList[num].m_MoveSpeed > 0f)
				{
					if (m_CloudList[num].m_CloudFollower != null)
					{
						Transform transform2 = m_CloudList[num].m_CloudFollower.transform;
						Vector3 localPosition4 = m_CloudList[num].m_Cloud.transform.localPosition;
						float x2 = localPosition4.x;
						Vector3 size = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
						float x3 = x2 - size.x;
						Vector3 localPosition5 = m_CloudList[num].m_Cloud.transform.localPosition;
						float y2 = localPosition5.y;
						Vector3 localPosition6 = m_CloudList[num].m_Cloud.transform.localPosition;
						transform2.localPosition = new Vector3(x3, y2, localPosition6.z);
					}
					Vector3 localPosition7 = m_CloudList[num].m_Cloud.transform.localPosition;
					float x4 = localPosition7.x;
					float x5 = RightMostOfScreen.x;
					Vector3 size2 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
					if (x4 > x5 + size2.x / 2f)
					{
						if (m_EnableLargeCloudLoop)
						{
							GameObject cloud2 = m_CloudList[num].m_Cloud;
							m_CloudList[num].m_Cloud = m_CloudList[num].m_CloudFollower;
							m_CloudList[num].m_CloudFollower = cloud2;
						}
						else
						{
							m_CloudList[num].m_MoveSpeed = UnityEngine.Random.Range(m_MinSpeed, m_MaxSpeed);
							Transform transform3 = m_CloudList[num].m_Cloud.transform;
							float x6 = LeftMostOfScreen.x;
							Vector3 size3 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
							float x7 = x6 - size3.x;
							float y3 = UnityEngine.Random.Range((0f - m_Camera.orthographicSize) / 2f, m_Camera.orthographicSize / 2f);
							Vector3 size4 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
							transform3.localPosition = new Vector3(x7, y3, size4.z);
						}
					}
				}
				else
				{
					if (m_CloudList[num].m_CloudFollower != null)
					{
						Transform transform4 = m_CloudList[num].m_CloudFollower.transform;
						Vector3 localPosition8 = m_CloudList[num].m_Cloud.transform.localPosition;
						float x8 = localPosition8.x;
						Vector3 size5 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
						float x9 = x8 + size5.x;
						Vector3 localPosition9 = m_CloudList[num].m_Cloud.transform.localPosition;
						float y4 = localPosition9.y;
						Vector3 localPosition10 = m_CloudList[num].m_Cloud.transform.localPosition;
						transform4.localPosition = new Vector3(x9, y4, localPosition10.z);
					}
					Vector3 localPosition11 = m_CloudList[num].m_Cloud.transform.localPosition;
					float x10 = localPosition11.x;
					float x11 = LeftMostOfScreen.x;
					Vector3 size6 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
					if (x10 < x11 - size6.x / 2f)
					{
						if (m_EnableLargeCloudLoop)
						{
							GameObject cloud3 = m_CloudList[num].m_Cloud;
							m_CloudList[num].m_Cloud = m_CloudList[num].m_CloudFollower;
							m_CloudList[num].m_CloudFollower = cloud3;
						}
						else
						{
							m_CloudList[num].m_MoveSpeed = 0f - UnityEngine.Random.Range(m_MinSpeed, m_MaxSpeed);
							Transform transform5 = m_CloudList[num].m_Cloud.transform;
							float x12 = RightMostOfScreen.x;
							Vector3 size7 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
							float x13 = x12 + size7.x;
							float y5 = m_CloudList[num].m_OriginalLocalPos.y;
							Vector3 size8 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
							float min = y5 - size8.y;
							float y6 = m_CloudList[num].m_OriginalLocalPos.y;
							Vector3 size9 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
							float y7 = UnityEngine.Random.Range(min, y6 + size9.y);
							Vector3 size10 = m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size;
							transform5.localPosition = new Vector3(x13, y7, size10.z);
						}
					}
				}
			}
			num++;
		}
	}

	private void FindTheOrthographicCamera()
	{
		if (m_Camera == null)
		{
			Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
			Camera[] array2 = array;
			foreach (Camera camera in array2)
			{
				if (camera.orthographic)
				{
					m_Camera = camera;
					break;
				}
			}
		}
		if (m_Camera != null)
		{
			LeftMostOfScreen = m_Camera.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
			RightMostOfScreen = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
		}
	}
}
