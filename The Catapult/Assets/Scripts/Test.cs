using UnityEngine;

public class Test : MonoBehaviour
{
	private int textureCount;

	private void Start()
	{
		GoTest();
	}

	private void GoTest()
	{
		int num = 0;
		HideFlags hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset;
		HideFlags hideFlags2 = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontUnloadUnusedAsset;
		Object[] array = Resources.FindObjectsOfTypeAll(typeof(Texture));
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Texture texture = (Texture)array2[i];
			if (texture.hideFlags != HideFlags.HideAndDontSave && texture.hideFlags != hideFlags && texture.hideFlags != hideFlags2)
			{
				textureCount++;
				UnityEngine.Debug.Log("Texture: " + texture.name + " : hideFlags: " + texture.hideFlags.ToString() + ": Memory: " + 1);
			}
		}
	}
}
