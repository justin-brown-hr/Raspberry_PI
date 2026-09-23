using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Android;
using UnityEngine;

/// <summary>
/// The cabinet has no in-app purchases. com.unity.purchasing still injects Play Billing 9.0,
/// whose androidx/kotlin dependencies are too new for Unity 2022.3's Android Gradle Plugin 7.4
/// (duplicate kotlin classes, then a StackOverflowError while dexing androidx.core 1.15).
/// Strip the dependency from the generated Gradle project.
/// </summary>
public class CabinetGradlePostprocess : IPostGenerateGradleAndroidProject
{
	public int callbackOrder => int.MaxValue;

	public void OnPostGenerateGradleAndroidProject(string path)
	{
		string gradleFile = Path.Combine(path, "build.gradle");
		if (!File.Exists(gradleFile))
		{
			return;
		}
		string original = File.ReadAllText(gradleFile);
		string stripped = Regex.Replace(original, @"^.*com\.android\.billingclient:billing.*\r?\n", string.Empty, RegexOptions.Multiline);
		if (stripped != original)
		{
			File.WriteAllText(gradleFile, stripped);
			Debug.Log("PIBUILD: removed Play Billing dependency from " + gradleFile);
		}
	}
}
