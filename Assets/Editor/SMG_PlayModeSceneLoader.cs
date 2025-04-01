using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class SMG_PlayModeSceneLoader {
	private const string ScenePathKey = "PlayModeScenePath";
	private const string ToggleKey = "PlayModeSceneLoaderEnabled";
	private const string PreviousSceneKey = "";
	private static string defaultScenePath = "Assets/Scenes/MainMenuScene.unity";

	static SMG_PlayModeSceneLoader() {
		EditorApplication.playModeStateChanged += OnPlayModeChanged;
	}

	private static void OnPlayModeChanged(PlayModeStateChange state) {
		if (!IsEnabled()) return;

		if (state == PlayModeStateChange.ExitingEditMode) {
			EditorPrefs.SetString(PreviousSceneKey, EditorSceneManager.GetActiveScene().path);
			string scenePath = EditorPrefs.GetString(ScenePathKey, defaultScenePath);
			if (!EditorSceneManager.GetActiveScene().path.Equals(scenePath)) {
				EditorSceneManager.OpenScene(scenePath);
			}
		}

		if (state == PlayModeStateChange.EnteredEditMode && IsEnabled()) {
			string previousScene = EditorPrefs.GetString(PreviousSceneKey, "");
			if (!string.IsNullOrEmpty(previousScene) && previousScene != EditorSceneManager.GetActiveScene().path) {
				EditorSceneManager.OpenScene(previousScene);
			}
		}
	}

	public static bool IsEnabled() => EditorPrefs.GetBool(ToggleKey, false);
	public static void SetEnabled(bool enabled) => EditorPrefs.SetBool(ToggleKey, enabled);
	public static void SetScenePath(string path) => EditorPrefs.SetString(ScenePathKey, path);


	[SettingsProvider]
	public static SettingsProvider CreateSettingsProvider() {
		var provider = new SettingsProvider("Preferences/Play Mode Scene", SettingsScope.User) {
			label = "Play Mode Scene",
			guiHandler = (searchContext) => {
				EditorGUILayout.LabelField("Automatically load a specific scene before entering play mode", EditorStyles.boldLabel);

				bool enabled = EditorGUILayout.Toggle("Enable Auto Scene Load", IsEnabled());
				SetEnabled(enabled);

				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Scene to Load:");
				string scenePath = EditorGUILayout.TextField(EditorPrefs.GetString(ScenePathKey, defaultScenePath));
				SetScenePath(scenePath);
			}
		};

		return provider;
	}
}