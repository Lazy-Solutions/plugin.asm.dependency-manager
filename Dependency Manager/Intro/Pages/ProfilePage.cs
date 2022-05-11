using plugin.asm.dependency_manager.Utility;
using UnityEditor;
using UnityEngine;

namespace plugin.asm.dependency_manager.Intro
{

    public class ProfilePage : WizardPage
    {

        public override bool CanSkip => true;

        public override void OnSkip() =>
            didSkip = true;

        public override void OnEnable()
        {
            didSkip = false;
            profileName = "dev";
        }

        GUIStyle label;

        public static string profileName
        {
            get => PlayerPrefs.GetString("AdvancedSceneManager.Intro.ProfileName", "dev");
            set => PlayerPrefs.SetString("AdvancedSceneManager.Intro.ProfileName", value);
        }

        public static bool didSkip
        {
            get => PlayerPrefs.GetInt("AdvancedSceneManager.Intro.SkipProfile") == 1;
            set => PlayerPrefs.SetInt("AdvancedSceneManager.Intro.SkipProfile", value ? 1 : 0);
        }

        public override void OnGUI()
        {

            if (window)
                window.minSize = window.maxSize = new Vector2(600, 210);

            if (label == null) label = new GUIStyle(EditorStyles.label) { wordWrap = true, margin = new RectOffset(0, 0, 0, 12) };

            GUILayout.Label("ASM uses profiles to store collections and settings. This provides the ability to maintain dev and release profiles, for example. But in order to get started we'll need to create an initial one, so please give it a name:", label);
            EditorGUILayout.Space();
            profileName = GUILayout.TextField(profileName);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("And with that, setup is done!\nPress Done (or Skip) and then please wait a moment while we finish setting things up on our end.", label);

        }

    }

}

