using plugin.asm.dependency_manager.Blacklist;
using plugin.asm.dependency_manager.Utility;
using UnityEditor;
using UnityEngine;

namespace plugin.asm.dependency_manager.Intro
{

    public class BlacklistPage : WizardPage
    {

        public static SettingsModule settings
        {
            get => JsonUtility.FromJson<SettingsModule>(PlayerPrefs.GetString("AdvancedSceneManager.Intro.Blacklist"));
            set => PlayerPrefs.SetString("AdvancedSceneManager.Intro.Blacklist", JsonUtility.ToJson(value));
        }

        public override bool CanSkip => true;

        public override void OnSkip()
        {
            settings = null;
        }

        GUIStyle label;

        Vector2 scroll;
        public override void OnGUI()
        {

            if (window)
                window.minSize = window.maxSize = new Vector2(640, 450);

            if (label == null) label = new GUIStyle(EditorStyles.label) { wordWrap = true, margin = new RectOffset(0, 0, 0, 12) };

            GUILayout.Label("In ASM we use scriptable objects to reference scenes instead of string paths / name or build index.", label);
            GUILayout.Label("In order to facilitate this, we have an asset refresh system which runs when a scene has changed. This will normally not take much time, but whenever the changes are large or when ASM is first imported into project, it certainly can.", label);
            GUILayout.Label("As such, we have a blacklist / whitelist system in place to ensure ASM will only process the actually relevant scenes.", label);

            GUILayout.Space(22);

            var settings = BlacklistPage.settings ?? new SettingsModule();

            scroll = GUILayout.BeginScrollView(scroll);

            BlacklistGUIUtility.DrawGUI(settings, out var didDirty);
            if (didDirty)
                BlacklistPage.settings = settings;

            GUILayout.EndScrollView();

            GUI.Label(new Rect(22, Screen.height - 112, 600, 22), "After this, blacklist / whitelist settings can be found in the Scene Manager Window settings.", label);
            GUI.Label(new Rect(22, Screen.height - 92, 600, 22), "Note that this will override settings in current profile, if you previously had one.", label);

        }

    }

}

