using System.Diagnostics;
using UnityEngine;

namespace Exploder2D
{
	public static class Exploder2DUtils
	{
		[Conditional("UNITY_EDITOR")]
		public static void Assert(bool condition, string message = "")
		{
			if (!condition)
			{
				UnityEngine.Debug.LogError("Assert! " + message);
				UnityEngine.Debug.Break();
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Warning(bool condition, string message)
		{
			if (!condition)
			{
				UnityEngine.Debug.LogWarning("Warning! " + message);
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Log(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		public static Vector2 GetCentroid(GameObject obj)
		{
			SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
			if ((bool)component && (bool)component.sprite)
			{
				Vector2 a = Vector2.zero;
				Vector2[] vertices = component.sprite.vertices;
				for (int i = 0; i < vertices.Length; i++)
				{
					a += vertices[i];
				}
				return obj.transform.TransformPoint(a / vertices.Length);
			}
			return obj.transform.position;
		}

		public static void SetVisible(GameObject obj, bool status)
		{
			if ((bool)obj)
			{
				MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = status;
				}
			}
		}

		public static void ClearLog()
		{
		}

		public static bool IsActive(GameObject obj)
		{
			return (bool)obj && obj.activeSelf;
		}

		public static void SetActive(GameObject obj, bool status)
		{
			if ((bool)obj)
			{
				obj.SetActive(status);
			}
		}

		public static void SetActiveRecursively(GameObject obj, bool status)
		{
			if ((bool)obj)
			{
				int childCount = obj.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					SetActiveRecursively(obj.transform.GetChild(i).gameObject, status);
				}
				obj.SetActive(status);
			}
		}

		public static void EnableCollider(GameObject obj, bool status)
		{
			if ((bool)obj)
			{
				Collider[] componentsInChildren = obj.GetComponentsInChildren<Collider>();
				Collider[] array = componentsInChildren;
				foreach (Collider collider in array)
				{
					collider.enabled = status;
				}
			}
		}

		public static bool IsExplodable(GameObject obj)
		{
			bool flag = obj.GetComponent<Explodable2D>() != null;
			if (!flag)
			{
				flag = obj.CompareTag(Exploder2DObject.Tag);
			}
			return flag;
		}
	}
}
