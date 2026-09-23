using System;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Headless build entry points for the Raspberry Pi cabinet (Android under Waydroid).
///   Unity -batchmode -quit -projectPath . -executeMethod PiBuild.BuildAndroidArm64
/// </summary>
public static class PiBuild
{
	private static readonly string[] Scenes =
	{
		"Assets/Scenes/Splash.unity",
		"Assets/Scenes/Menu.unity",
		"Assets/Scenes/Gameplay.unity"
	};

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
	/// Points Unity at the SDK/NDK/JDK. None were installed through the Hub, so the
	/// editor has no stored prefs and auto-detection fails.
	/// </summary>
	private static void ConfigureExternalTools()
	{
		string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		string sdk = Path.Combine(home, ".local/unity-sdk/android-sdk");
		// Unity 2022.3 requires NDK r23b and JDK 11
		string ndk = Path.Combine(sdk, "ndk/android-ndk-r23b");
		string jdk = Path.Combine(home, ".local/unity-sdk/jdk11");

		TrySet("sdk", () => AndroidExternalToolsSettings.sdkRootPath = sdk);
		TrySet("ndk", () => AndroidExternalToolsSettings.ndkRootPath = ndk);
		TrySet("jdk", () => AndroidExternalToolsSettings.jdkRootPath = jdk);

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
		PlayerSettings.companyName = "Flow Games";
		PlayerSettings.productName = "Smash Fest";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.pj.smashfest");
		PlayerSettings.bundleVersion = "1.0";
		PlayerSettings.Android.bundleVersionCode = 1;
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
		PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
		PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
		PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);

		// Waydroid presents GLES3; leaving Vulkan first risks a black screen there
		PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
		PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[]
		{
			UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3
		});

		// The cabinet TV is mounted portrait
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
		string apk = Path.Combine(outDir, "SmashFest-cabinet.apk");

		var options = new BuildPlayerOptions
		{
			scenes = Scenes,
			locationPathName = apk,
			target = BuildTarget.Android,
			targetGroup = BuildTargetGroup.Android,
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

	/// <summary>
	/// Desktop build of the same game for playtesting on the build host
	/// (run with -cabinetTest to drive it automatically and capture screenshots).
	/// </summary>
	public static void BuildLinuxTest()
	{
		PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
		PlayerSettings.defaultScreenWidth = 1080;
		PlayerSettings.defaultScreenHeight = 1920;
		PlayerSettings.resizableWindow = false;
		// Xvfb has no window manager, so the window never gets focus; without this the player pauses
		PlayerSettings.runInBackground = true;
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

		string outDir = Path.Combine(Directory.GetCurrentDirectory(), "Build/linux-test");
		Directory.CreateDirectory(outDir);
		var options = new BuildPlayerOptions
		{
			scenes = Scenes,
			locationPathName = Path.Combine(outDir, "SmashFest.x86_64"),
			target = BuildTarget.StandaloneLinux64,
			targetGroup = BuildTargetGroup.Standalone,
			options = BuildOptions.None
		};
		BuildSummary summary = BuildPipeline.BuildPlayer(options).summary;
		Console.WriteLine("PIBUILD: linux result=" + summary.result + " errors=" + summary.totalErrors);
		EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
	}
}
