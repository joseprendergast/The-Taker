using System.IO;
using GameFoundation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GameProjectBootstrap
{
	static readonly string[] Folders =
	{
		"Assets/Game/Rooms",
		"Assets/Game/Characters",
		"Assets/Game/Inventory",
		"Assets/Game/UI",
		"Assets/Game/Scripts",
		"Assets/Game/Audio",
		"Assets/Game/Art",
		"Assets/Game/Debug",
		"Assets/Game/Atmosphere",
		"Assets/Game/Imported",
		"Assets/StreamingAssets",
		"Builds/WebGL",
		"docs"
	};

	static readonly string[] StarterScenes =
	{
		"RoomTitle",
		"RoomPrototypeA",
		"RoomPrototypeB",
		"RoomPrototypeC"
	};

	[MenuItem("Game Foundation/Bootstrap/Run Full Bootstrap")]
	public static void RunFullBootstrap()
	{
		EnsureFolders();
		CreatePlaceholderArt();
		GenerateStarterScenes();
		AddBuildScenes();
		SetWebGLTarget();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	[MenuItem("Game Foundation/Bootstrap/Ensure Folders")]
	public static void EnsureFolders()
	{
		foreach (string folder in Folders)
			Directory.CreateDirectory(folder);
	}

	[MenuItem("Game Foundation/Bootstrap/Create Placeholder Art")]
	public static void CreatePlaceholderArt()
	{
		Directory.CreateDirectory("Assets/Game/Art/Generated");
		CreateTexture("Assets/Game/Art/Generated/PlaceholderRoomA.png", new Color32(18, 24, 36, 255), new Color32(65, 76, 98, 255));
		CreateTexture("Assets/Game/Art/Generated/PlaceholderRoomB.png", new Color32(28, 22, 32, 255), new Color32(92, 66, 78, 255));
		CreateTexture("Assets/Game/Art/Generated/PlaceholderRoomC.png", new Color32(22, 30, 26, 255), new Color32(70, 94, 78, 255));
		AssetDatabase.Refresh();
		ConfigureImportedSprites();
	}

	[MenuItem("Game Foundation/Bootstrap/Configure Imported Sprites")]
	public static void ConfigureImportedSprites()
	{
		foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Game/Art", "Assets/Game/Imported" }))
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
			if (importer == null)
				continue;

			importer.textureType = TextureImporterType.Sprite;
			importer.spritePixelsPerUnit = 100;
			importer.filterMode = FilterMode.Point;
			importer.mipmapEnabled = false;
			importer.SaveAndReimport();
		}
	}

	[MenuItem("Game Foundation/Bootstrap/Generate Starter Scenes")]
	public static void GenerateStarterScenes()
	{
		EnsureFolders();
		CreateStarterScene("RoomTitle", "RoomPrototypeA", "Assets/Game/Art/Generated/PlaceholderRoomA.png", true);
		CreateStarterScene("RoomPrototypeA", "RoomPrototypeB", "Assets/Game/Art/Generated/PlaceholderRoomA.png", false);
		CreateStarterScene("RoomPrototypeB", "RoomPrototypeC", "Assets/Game/Art/Generated/PlaceholderRoomB.png", false);
		CreateStarterScene("RoomPrototypeC", "RoomPrototypeA", "Assets/Game/Art/Generated/PlaceholderRoomC.png", false);
		AssetDatabase.Refresh();
	}

	[MenuItem("Game Foundation/Bootstrap/Add Build Scenes")]
	public static void AddBuildScenes()
	{
		EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[StarterScenes.Length];
		for (int i = 0; i < StarterScenes.Length; ++i)
		{
			string path = $"Assets/Game/Rooms/{StarterScenes[i]}/{StarterScenes[i]}.unity";
			scenes[i] = new EditorBuildSettingsScene(path, true);
		}
		EditorBuildSettings.scenes = scenes;
	}

	[MenuItem("Game Foundation/WebGL/Set WebGL Target")]
	public static void SetWebGLTarget()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
		PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
		PlayerSettings.WebGL.dataCaching = true;
	}

	[MenuItem("Game Foundation/WebGL/Build WebGL")]
	public static void BuildWebGL()
	{
		SetWebGLTarget();
		BuildPipeline.BuildPlayer(GetBuildScenePaths(), "Builds/WebGL", BuildTarget.WebGL, BuildOptions.None);
		PatchWebGLShell("Builds/WebGL");
	}

	[MenuItem("Game Foundation/WebGL/Build WebGL and Copy to Docs")]
	public static void BuildWebGLAndCopyToDocs()
	{
		BuildWebGL();
		CopyBuildToDocs();
	}

	[MenuItem("Game Foundation/WebGL/Copy Build to Docs")]
	public static void CopyBuildToDocs()
	{
		CleanDocsWebGLOutput();
		CopyDirectory("Builds/WebGL", "docs");
		PatchWebGLShell("docs");
		AssetDatabase.Refresh();
	}

	static string[] GetBuildScenePaths()
	{
		string[] paths = new string[StarterScenes.Length];
		for (int i = 0; i < StarterScenes.Length; ++i)
			paths[i] = $"Assets/Game/Rooms/{StarterScenes[i]}/{StarterScenes[i]}.unity";
		return paths;
	}

	static void CreateStarterScene(string sceneName, string nextScene, string artPath, bool title)
	{
		string folder = $"Assets/Game/Rooms/{sceneName}";
		Directory.CreateDirectory(folder);

		Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
		GameObject systems = new GameObject("Persistent Systems");
		systems.AddComponent<GameGlobals>();
		systems.AddComponent<GameSceneManager>();
		systems.AddComponent<GameInventory>();
		systems.AddComponent<GameDialogue>();
		systems.AddComponent<GameSettings>();
		systems.AddComponent<GameNarration>();
		systems.AddComponent<GamePowerQuestBridge>();
		systems.AddComponent<GameDebug>();

		Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
		camera.tag = "MainCamera";
		camera.orthographic = true;
		camera.orthographicSize = 5f;
		camera.transform.position = new Vector3(0f, 0f, -10f);

		GameObject eventSystem = new GameObject("EventSystem");
		eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
		eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

		CreateUiCanvas(title);
		CreateRoomDefinition(sceneName, nextScene, artPath, title);
		CreatePlaceholderBackground(artPath);
		CreateHotspot(nextScene);

		EditorSceneManager.SaveScene(scene, $"{folder}/{sceneName}.unity");
	}

	static void CreateUiCanvas(bool title)
	{
		GameObject canvasObject = new GameObject("UI Canvas");
		Canvas canvas = canvasObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasObject.AddComponent<GraphicRaycaster>();
		canvasObject.AddComponent<GameUI>();

		GameObject subtitle = CreateText("Subtitle", canvasObject.transform, title ? "Press any button" : "");
		RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
		subtitleRect.anchorMin = new Vector2(0.1f, 0.04f);
		subtitleRect.anchorMax = new Vector2(0.9f, 0.18f);
		subtitleRect.offsetMin = Vector2.zero;
		subtitleRect.offsetMax = Vector2.zero;
	}

	static void CreateRoomDefinition(string sceneName, string nextScene, string artPath, bool title)
	{
		GameObject room = new GameObject("Room Definition");
		GameRoom gameRoom = room.AddComponent<GameRoom>();
		gameRoom.definition.sceneId = sceneName;
		gameRoom.definition.displayName = title ? "Title" : sceneName;
		gameRoom.definition.description = title ? "Landing cover and main menu." : "Prototype exploration room.";
		gameRoom.definition.backgroundArtReference = artPath;
		gameRoom.definition.completionFlag = $"{sceneName}.complete";
		gameRoom.definition.narrativeBeats.Add(new SubtitleLineData
		{
			speaker = "",
			text = title ? "Press any button." : $"This is {sceneName}.",
			holdSeconds = 2f
		});
	}

	static void CreatePlaceholderBackground(string artPath)
	{
		Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(artPath);
		GameObject background = new GameObject("Placeholder Background");
		SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
		renderer.sprite = sprite;
		background.transform.localScale = new Vector3(8f, 4.5f, 1f);
	}

	static void CreateHotspot(string nextScene)
	{
		GameObject hotspot = new GameObject("Hotspot Door");
		hotspot.transform.position = new Vector3(2.6f, -1.2f, 0f);
		BoxCollider2D collider = hotspot.AddComponent<BoxCollider2D>();
		collider.isTrigger = true;
		collider.size = new Vector2(1.6f, 1.2f);
		GameHotspot gameHotspot = hotspot.AddComponent<GameHotspot>();
		gameHotspot.hotspotId = "door";
		gameHotspot.displayName = "Exit";
		gameHotspot.inspectLine = "A quiet way into the next room.";
		gameHotspot.interactLine = "You move on.";
		gameHotspot.roomChangeSceneId = nextScene;
	}

	static GameObject CreateText(string name, Transform parent, string value)
	{
		GameObject obj = new GameObject(name);
		obj.transform.SetParent(parent, false);
		Text text = obj.AddComponent<Text>();
		text.text = value;
		text.alignment = TextAnchor.MiddleCenter;
		text.color = Color.white;
		text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		return obj;
	}

	static void CreateTexture(string path, Color32 a, Color32 b)
	{
		if (File.Exists(path))
			return;

		Texture2D texture = new Texture2D(320, 180, TextureFormat.RGBA32, false);
		for (int y = 0; y < texture.height; ++y)
		for (int x = 0; x < texture.width; ++x)
		{
			float t = (float)y / texture.height;
			Color color = Color.Lerp(a, b, t);
			texture.SetPixel(x, y, color);
		}
		texture.Apply();
		File.WriteAllBytes(path, texture.EncodeToPNG());
		Object.DestroyImmediate(texture);
	}

	static void PatchWebGLShell(string folder)
	{
		string index = Path.Combine(folder, "index.html");
		if (!File.Exists(index))
			return;

		string html = File.ReadAllText(index);
		if (!html.Contains("game-foundation-responsive-canvas"))
		{
			html = html.Replace("</head>", "<style id=\"game-foundation-responsive-canvas\">html,body{margin:0;width:100%;height:100%;background:#05070d;overflow:hidden}#unity-container,.unity-canvas,canvas{width:100%!important;height:100%!important}</style></head>");
			File.WriteAllText(index, html);
		}
	}

	static void CopyDirectory(string source, string destination)
	{
		if (!Directory.Exists(source))
			return;

		Directory.CreateDirectory(destination);

		foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
			Directory.CreateDirectory(dir.Replace(source, destination));

		foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
		{
			string target = file.Replace(source, destination);
			Directory.CreateDirectory(Path.GetDirectoryName(target));
			File.Copy(file, target, true);
		}
	}

	static void CleanDocsWebGLOutput()
	{
		string[] files = { "docs/index.html", "docs/favicon.ico" };
		string[] folders = { "docs/Build", "docs/TemplateData", "docs/StreamingAssets" };

		foreach (string file in files)
			if (File.Exists(file))
				File.Delete(file);

		foreach (string folder in folders)
			if (Directory.Exists(folder))
				Directory.Delete(folder, true);
	}
}
