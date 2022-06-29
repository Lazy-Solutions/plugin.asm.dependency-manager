using plugin.asm.dependency_manager.Utility;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace plugin.asm.dependency_manager.Intro
{

    public class LinksPage : WizardPage
    {

        GUIStyle label;
        GUIStyle label2;
        GUIStyle boldLabel;
        GUIStyle link;

        public override void OnGUI()
        {

            if (window)
                window.minSize = window.maxSize = new Vector2(700, 460);

            if (label == null) label = new GUIStyle(EditorStyles.label) { wordWrap = true, margin = new RectOffset(0, 0, 0, 12) };
            if (label2 == null) label2 = new GUIStyle(label) { margin = new RectOffset(0, 0, 0, 6) };
            if (boldLabel == null) boldLabel = new GUIStyle(label) { fontStyle = FontStyle.Bold, margin = new RectOffset(0, 0, 22, 6), fontSize = 14 };
            if (link == null) link = new GUIStyle(label2) { normal = new GUIStyleState() { textColor = new Color32(79, 128, 248, 255) } };

            GUILayout.Label("Documentation", boldLabel);
            GUILayout.Label("Offline documentation is available in pdf format, but please note that it is not always up-to-date with online version.", label);

            LinkButton("Offline documentation:", "Assets/AdvancedSceneManager/Documentation.pdf", onClick: () => EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>("Assets/AdvancedSceneManager/Documentation.pdf")));
            LinkButton("Online documentation:", "https://lazy-solutions.github.io/AdvancedSceneManager");

            GUILayout.Label("Patches", boldLabel);
            GUILayout.Label("Patches are smaller updates that are provided in-between the larger asset store updates. It is not required to install these, but it might be good idea if you experience or wish to avoid bugs. They might sometimes include features.", label);
            LinkButton("Patches:", "https://github.com/Lazy-Solutions/AdvancedSceneManager/tree/main/patches");

            GUILayout.Label("Support", boldLabel);
            GUILayout.Label("If you experience any bugs, that have not yet been fixed in a patch, or need help with anything, that is not covered in documentation (or documentation is unclear), feel free to reach out to us at any of the following:", label);

            LinkButton("Github Issues:", "https://github.com/Lazy-Solutions/AdvancedSceneManager/issues", labelWidth: 82);
            LinkButton("Mail:", "mailto:support@lazy.solutions", labelWidth: 82);
            LinkButton("Discord:", "https://discord.gg/pnRn6zeFEJ", labelWidth: 82);

        }

        void LinkButton(string content, string url, float labelWidth = float.NaN, Action onClick = null)
        {

            GUILayout.BeginHorizontal();

            if (float.IsNaN(labelWidth))
                GUILayout.Label(content, label2, GUILayout.ExpandWidth(false));
            else
                GUILayout.Label(content, label2, GUILayout.Width(labelWidth));
            GUILayout.Space(12);

            if (GUILayout.Button(url, link, GUILayout.ExpandWidth(false)))
                if (onClick == null)
                    Application.OpenURL(url);
                else
                    onClick.Invoke();

            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);

            GUILayout.EndHorizontal();

        }

    }

}

