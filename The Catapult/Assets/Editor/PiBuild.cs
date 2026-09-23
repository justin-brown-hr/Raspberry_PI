using System;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Headless build entry points for the Raspberry Pi target.
/// Invoked from the command line, e.g.
///   Unity -batchmode -quit -projectPath . -executeMethod PiBuild.BuildAndroidArm64
/// </summary>
public static class PiBuild
{
	private static readonly string[] Scenes =
	{
		"Assets/Scenes 1/GDPR_Scene.unity",
		"Assets/Scenes 1/InitScene.unity",
		"Assets/Scenes 1/MainScene.unity",
		"Assets/Scenes 1/PlayModeSelect.unity",
		"Assets/Scenes 1/GameScene.unity",
		"Assets/Scenes 1/PvPScene.unity"
	};

	/// <summary>
	/// Compile-only pass. Opening the project in batchmode already forces a script
	/// compile, so this simply reports whether the editor considers the code valid.
	/// </summary>
	public static void CompileOnly()
	{
		if (EditorUtility.scriptCompilationFailed)
		{
			Console.WriteLine("PIBUILD: COMPILE FAILED");
			EditorApplication.Exit(1);
			return;
		}
		Console.WriteLine("PIBUILD: COMPILE OK");
		EditorApplication.Exit(0);
	}

	/// <summary>
	/// Applies the settings the Pi needs: 64-bit ARM, which in turn requires IL2CPP,
	/// plus an explicit target SDK. The project ships as ARMv7 + auto SDK, neither of
	/// which suits a modern arm64 Android runtime.
	/// </summary>
	/// <summary>
	/// Points Unity at the SDK/NDK/JDK. Auto-detection fails here because none of
	/// them were installed through the Hub, so the editor has no stored prefs.
	/// </summary>
	/// <summary>
	/// Points Unity at the SDK/NDK/JDK. Auto-detection fails here because none of
	/// them were installed through the Hub, so the editor has no stored prefs.
	/// Each assignment is guarded: Unity validates these paths and throws, and we
	/// want to know which one it rejects rather than losing the whole build.
	/// </summary>
	private static void ConfigureExternalTools()
	{
		string home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
		string sdk = Path.Combine(home, ".local/unity-sdk/android-sdk");
		string ndk = Path.Combine(sdk, "ndk/android-ndk-r21d");
		// Unity 2021.3.23f1 expects OpenJDK 8 (module id android-open-jdk-8u172-b11) and
		// rejects JDK 11 outright. This is Unity's own JDK, unzipped into the engine dir.
		string jdk = Path.Combine(home, ".local/unity/Editor/Data/PlaybackEngines/AndroidPlayer/OpenJDK");

		TrySet("sdk", () => AndroidExternalToolsSettings.sdkRootPath = sdk);
		TrySet("ndk", () => AndroidExternalToolsSettings.ndkRootPath = ndk);
		TrySet("jdk", () => AndroidExternalToolsSettings.jdkRootPath = jdk);

		// Mirror into EditorPrefs; the embedded copies are symlinked inside
		// PlaybackEngines/AndroidPlayer, so embedded mode resolves to the same tools.
		EditorPrefs.SetString("AndroidSdkRoot", sdk);
		EditorPrefs.SetString("AndroidNdkRootR21D", ndk);
		EditorPrefs.SetString("JdkPath", jdk);
		EditorPrefs.SetBool("SdkUseEmbedded", false);
		EditorPrefs.SetBool("NdkUseEmbedded", false);
		EditorPrefs.SetBool("JdkUseEmbedded", true);

		Console.WriteLine("PIBUILD: sdk=" + AndroidExternalToolsSettings.sdkRootPath);
		Console.WriteLine("PIBUILD: ndk=" + AndroidExternalToolsSettings.ndkRootPath);
		Console.WriteLine("PIBUILD: jdk=" + AndroidExternalToolsSettings.jdkRootPath);
	}

	private static void TrySet(string label, Action set)
	{
		try
		{
			set();
			Console.WriteLine("PIBUILD: " + label + " path accepted");
		}
		catch (Exception e)
		{
			Console.WriteLine("PIBUILD: " + label + " path REJECTED -> " + e.Message);
		}
	}

	public static void ConfigureAndroid()
	{
		ConfigureExternalTools();
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
		PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
		// This Unity version's enum has no AndroidApiLevel33 member. Auto resolves to
		// the highest SDK platform installed, which is android-33 here.
		PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
		PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);

		// Waydroid presents GLES3; leaving Vulkan first risks a black screen there.
		PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
		PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[]
		{
			UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3
		});

		// The cabinet screen is mounted portrait. The game was authored for landscape,
		// so this is expected to expose UI layout problems - build it and look.
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
		PlayerSettings.allowedAutorotateToPortrait = true;
		PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
		PlayerSettings.allowedAutorotateToLandscapeLeft = false;
		PlayerSettings.allowedAutorotateToLandscapeRight = false;
		PlayerSettings.useAnimatedAutorotation = false;

		AssetDatabase.SaveAssets();
		Console.WriteLine("PIBUILD: CONFIGURED arm64 / IL2CPP / GLES3 / portrait / release");
	}

	public static void BuildAndroidArm64()
	{
		ConfigureAndroid();

		string outDir = Path.Combine(Directory.GetCurrentDirectory(), "Build");
		Directory.CreateDirectory(outDir);
		string apk = Path.Combine(outDir, "TheCatapult-cabinet.apk");

		var options = new BuildPlayerOptions
		{
			scenes = Scenes,
			locationPathName = apk,
			target = BuildTarget.Android,
			targetGroup = BuildTargetGroup.Android,
			// Release build: no Development flag, so no "Development Build" watermark
			// on screen and no profiler overhead. This is the cabinet build.
			options = BuildOptions.None
		};

		BuildReport report = BuildPipeline.BuildPlayer(options);
		BuildSummary summary = report.summary;

		Console.WriteLine("PIBUILD: result=" + summary.result
			+ " errors=" + summary.totalErrors
			+ " size=" + summary.totalSize
			+ " output=" + apk);

		EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
	}
}
