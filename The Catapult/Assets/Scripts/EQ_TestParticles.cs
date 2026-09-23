using System;
using System.Collections;
using UnityEngine;

public class EQ_TestParticles : MonoBehaviour
{
	public Transform[] m_CategoryList;

	private int m_CurrentCategoryIndex;

	private int m_CurrentCategoryIndexOld = -1;

	private int m_CurrentCategoryChildCount;

	private int m_CurrentParticleIndex;

	private int m_CurrentParticleIndexOld = -1;

	private ParticleSystem m_CurrentParticle;

	private string m_CurrentCategoryName = string.Empty;

	private string m_CurrentParticleName = string.Empty;

	private void Start()
	{
		if (m_CategoryList.Length > 0)
		{
			m_CurrentCategoryIndex = 0;
			m_CurrentCategoryIndexOld = -1;
			m_CurrentParticleIndex = 0;
			m_CurrentParticleIndexOld = -1;
			ShowParticle();
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.UpArrow))
		{
			m_CurrentCategoryIndexOld = m_CurrentCategoryIndex;
			m_CurrentCategoryIndex++;
			m_CurrentParticleIndex = 0;
			ShowParticle();
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.DownArrow))
		{
			m_CurrentCategoryIndexOld = m_CurrentCategoryIndex;
			m_CurrentCategoryIndex--;
			m_CurrentParticleIndex = 0;
			ShowParticle();
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.LeftArrow))
		{
			m_CurrentParticleIndexOld = m_CurrentParticleIndex;
			m_CurrentParticleIndex--;
			ShowParticle();
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.RightArrow))
		{
			m_CurrentParticleIndexOld = m_CurrentParticleIndex;
			m_CurrentParticleIndex++;
			ShowParticle();
		}
	}

	private void OnGUI()
	{
		GUI.Window(1, new Rect(Screen.width - 260, 5f, 250f, 105f), AppNameWindow, "FX Quest 0.3.0");
		GUI.Window(2, new Rect(10f, Screen.height - 65, 290f, 60f), DemoSceneWindow, "Demo Scenes");
		GUI.Window(3, new Rect(Screen.width - 360, Screen.height - 85, 350f, 80f), ParticleInformationWindow, "Information");
	}

	private void ShowParticle()
	{
		if (m_CurrentCategoryIndex >= m_CategoryList.Length)
		{
			m_CurrentCategoryIndex = 0;
		}
		else if (m_CurrentCategoryIndex < 0)
		{
			m_CurrentCategoryIndex = m_CategoryList.Length - 1;
		}
		int num = 0;
		if (m_CurrentCategoryIndex != m_CurrentCategoryIndexOld)
		{
			if (m_CurrentCategoryIndexOld >= 0)
			{
				num = 0;
				IEnumerator enumerator = m_CategoryList[m_CurrentCategoryIndexOld].GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform transform = (Transform)enumerator.Current;
						m_CurrentParticle = transform.gameObject.GetComponent<ParticleSystem>();
						if (m_CurrentParticle != null)
						{
							m_CurrentParticle.Stop();
							m_CurrentParticle.gameObject.SetActive(value: false);
						}
						num++;
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
			}
			if (m_CurrentCategoryIndex >= 0)
			{
				num = 0;
				IEnumerator enumerator2 = m_CategoryList[m_CurrentCategoryIndex].GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						Transform transform2 = (Transform)enumerator2.Current;
						m_CurrentParticle = transform2.gameObject.GetComponent<ParticleSystem>();
						if (m_CurrentParticle != null)
						{
							m_CurrentParticle.Stop();
							m_CurrentParticle.gameObject.SetActive(value: false);
						}
						num++;
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = (enumerator2 as IDisposable)) != null)
					{
						disposable2.Dispose();
					}
				}
			}
			if (m_CurrentCategoryIndexOld >= 0)
			{
				m_CategoryList[m_CurrentCategoryIndexOld].gameObject.SetActive(value: false);
			}
			if (m_CurrentCategoryIndex >= 0)
			{
				m_CategoryList[m_CurrentCategoryIndex].gameObject.SetActive(value: true);
			}
			m_CurrentCategoryName = m_CategoryList[m_CurrentCategoryIndex].name;
			m_CurrentCategoryChildCount = m_CategoryList[m_CurrentCategoryIndex].childCount;
		}
		if (m_CurrentParticleIndex >= m_CurrentCategoryChildCount)
		{
			m_CurrentParticleIndex = 0;
		}
		else if (m_CurrentParticleIndex < 0)
		{
			m_CurrentParticleIndex = m_CurrentCategoryChildCount - 1;
		}
		if (m_CurrentParticleIndex != m_CurrentParticleIndexOld || m_CurrentCategoryIndex != m_CurrentCategoryIndexOld)
		{
			if (m_CurrentParticle != null)
			{
				m_CurrentParticle.Stop();
				m_CurrentParticle.gameObject.SetActive(value: false);
			}
			num = 0;
			IEnumerator enumerator3 = m_CategoryList[m_CurrentCategoryIndex].GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					Transform transform3 = (Transform)enumerator3.Current;
					if (num == m_CurrentParticleIndex)
					{
						m_CurrentParticle = transform3.gameObject.GetComponent<ParticleSystem>();
						if (m_CurrentParticle != null)
						{
							m_CurrentParticle.gameObject.SetActive(value: true);
							m_CurrentParticle.Play();
							m_CurrentParticleName = m_CurrentParticle.name;
						}
						break;
					}
					num++;
				}
			}
			finally
			{
				IDisposable disposable3;
				if ((disposable3 = (enumerator3 as IDisposable)) != null)
				{
					disposable3.Dispose();
				}
			}
		}
	}

	private void AppNameWindow(int id)
	{
		if (GUI.Button(new Rect(15f, 25f, 220f, 20f), "www.ge-team.com"))
		{
			Application.OpenURL("http://ge-team.com/pages/unity-3d/");
		}
		if (GUI.Button(new Rect(15f, 50f, 220f, 20f), "geteamdev@gmail.com"))
		{
			Application.OpenURL("mailto:geteamdev@gmail.com");
		}
		if (GUI.Button(new Rect(15f, 75f, 220f, 20f), "Tutorial"))
		{
			Application.OpenURL("http://youtu.be/TWpKPCGYEyI");
		}
	}

	private void DemoSceneWindow(int id)
	{
		if (m_CurrentParticleIndex >= 0)
		{
			GUILayout.BeginHorizontal();
			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "2D_Demo")
			{
				GUI.enabled = false;
			}
			else
			{
				GUI.enabled = true;
			}
			if (GUI.Button(new Rect(12f, 25f, 125f, 25f), "2D Demo Scene"))
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene("2D_Demo");
			}
			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "3D_Demo")
			{
				GUI.enabled = false;
			}
			else
			{
				GUI.enabled = true;
			}
			if (GUI.Button(new Rect(155f, 25f, 125f, 25f), "3D Demo Scene"))
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene("3D_Demo");
			}
			GUILayout.EndHorizontal();
		}
	}

	private void ParticleInformationWindow(int id)
	{
		if (m_CurrentParticleIndex >= 0)
		{
			GUI.Label(new Rect(12f, 25f, 350f, 20f), "Up / Down: Type (" + (m_CurrentCategoryIndex + 1) + " of " + m_CategoryList.Length + " " + m_CurrentCategoryName + ")");
			GUI.Label(new Rect(12f, 50f, 350f, 20f), "Left / Right: Particle (" + (m_CurrentParticleIndex + 1) + " of " + m_CurrentCategoryChildCount + " " + m_CurrentParticleName + ")");
		}
	}
}
