using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace plugin.asm.dependency_manager.Blacklist
{

    public static class BlacklistGUIUtility
    {

        public static void DrawGUI(SettingsModule settings, out bool didDirty)
        {

            didDirty = false;

            EditorGUI.BeginChangeCheck();
            Blacklist(settings);

            if (EditorGUI.EndChangeCheck())
                didDirty = true;

        }

        static readonly ReorderableList list = new ReorderableList(null, typeof(string), true, true, true, true);
        static void Blacklist(SettingsModule settings)
        {

            settings.isWhitelist = EditorGUILayout.Popup("Mode:", settings.isWhitelist ? 1 : 0, new[] { "Blacklist", "Whitelist" }) == 1;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (settings.isWhitelist)
                GUILayout.Label("Only the following scenes will be processed by ASM:");
            else
                GUILayout.Label("The following scenes will not be processed by ASM:");
            EditorGUILayout.Space();

            list.onCanRemoveCallback = (_) => true;
            list.list = settings.paths;
            list.onAddCallback = (_) => settings.paths.Add(GetCurrentPath());
            list.drawHeaderCallback = (position) => GUI.Label(position, "Paths:");
            list.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
            {

                GUI.SetNextControlName("blacklist-" + index);
                settings.paths[index] = GUI.TextField(new Rect(position.x + 3, position.y + 2, position.width - 3 - 28, position.height - 4), settings.paths[index]);

                if (GUI.Button(new Rect(position.xMax - 22, position.y, 22, position.height), new GUIContent("...", "Pick folder..."), new GUIStyle(GUI.skin.button) { padding = new RectOffset(2, 2, 2, 2) }))
                {

                    var path =
                        AssetDatabase.IsValidFolder(settings.paths[index])
                        ? settings.paths[index]
                        : "Assets/";

                    path = EditorUtility.OpenFolderPanel("Pick folder", path, "");
                    if (!string.IsNullOrWhiteSpace(path))
                        settings.paths[index] = "Assets" + path.Replace(Application.dataPath, "");

                }

            };

            list.DoLayoutList();

            var name = GUI.GetNameOfFocusedControl();
            if (name.StartsWith("blacklist-"))
                list.index = int.Parse(name.Substring(name.IndexOf("-") + 1));

            var r = GUILayoutUtility.GetRect(Screen.width - 44, 0);
            var c = GUI.color;
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.Label(new Rect(r.x, r.y - 20, r.width, 22), "Paths can either lead to a folder or SceneAsset.");
            GUI.color = c;

            if (Event.current.type == EventType.MouseDown)
                GUI.FocusControl("");

        }

        static string GetCurrentPath()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            var path = (string)getActiveFolderPath.Invoke(null, null);
            return path;
        }

    }

}
