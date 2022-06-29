using plugin.asm.dependency_manager.Utility;
using UnityEditor;
using UnityEngine;

namespace plugin.asm.dependency_manager.Intro
{

    public class WelcomePage : WizardPage
    {

        static GUIStyle labelBold;

        public override void OnGUI()
        {

            if (labelBold is null) labelBold = new GUIStyle(EditorStyles.boldLabel) { margin = new RectOffset(0, 0, 42, 8), alignment = TextAnchor.UpperCenter, fontSize = 14 };

            if (window)
                window.minSize = window.maxSize = new Vector2(635, 506);

            GUILayout.Label("Welcome to Advanced Scene Manager!", labelBold);

            var r = GUILayoutUtility.GetLastRect();

            var texture = Resources.Load<Sprite>("AdvancedSceneManager/image");
            if (texture)
                GUI.DrawTexture(new Rect(r.x + (r.width / 2) - (512 / 2), r.y - 44, 512, 512), texture.texture, ScaleMode.ScaleToFit);

        }

    }

}

