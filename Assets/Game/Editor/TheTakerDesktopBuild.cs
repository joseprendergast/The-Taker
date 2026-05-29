using System.IO;
using UnityEditor;

public static class TheTakerDesktopBuild
{
	[MenuItem("The Taker/Build Desktop/Mac")]
	public static void BuildMac()
	{
		Build("Builds/Mac/The Taker.app", BuildTarget.StandaloneOSX);
	}

	[MenuItem("The Taker/Build Desktop/Windows")]
	public static void BuildWindows()
	{
		Build("Builds/Windows/TheTaker.exe", BuildTarget.StandaloneWindows64);
	}

	[MenuItem("The Taker/Build Desktop/Linux")]
	public static void BuildLinux()
	{
		Build("Builds/Linux/TheTaker.x86_64", BuildTarget.StandaloneLinux64);
	}

	[MenuItem("The Taker/Build WebGL/GitHub Pages")]
	public static void BuildWebGLForPages()
	{
		Build("Builds/Pages", BuildTarget.WebGL);
	}

	public static void BuildForPages()
	{
		BuildWebGLForPages();
	}

	static void Build(string path, BuildTarget target)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		BuildPipeline.BuildPlayer(
			new[] {
				"Assets/Game/Rooms/Title/SceneRoomTitle.unity",
				"Assets/Game/Rooms/Forest/SceneRoomForest.unity"
			},
			path,
			target,
			BuildOptions.None);
	}
}
