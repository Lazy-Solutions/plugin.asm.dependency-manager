using System.Linq;
using UnityEditor;
using UnityEngine;

namespace plugin.asm.dependency_manager.Utility
{

    public abstract class WizardPage
    {

        public WizardWindow window { get; set; }
        internal void SetWindow(WizardWindow window) =>
            this.window = window;

        public abstract void OnGUI();
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnSkip() { }
        public virtual bool CanContinue { get; set; } = true;
        public virtual bool CanSkip { get; set; } = false;

    }

    public abstract class WizardWindow : EditorWindow
    {

        public abstract WizardPage[] Pages { get; }

        public WizardPage CurrentPage => Pages.ElementAtOrDefault(currentIndex);

        int currentIndex;
        bool isOnLast;

        public virtual void OnEnable()
        {

            CurrentPage?.OnEnable();

            if (minSize.x < 100 || minSize.y < 100)
                minSize = new Vector2(Mathf.Max(100, minSize.x), Mathf.Max(100, minSize.y));


        }

        public virtual void OnDisable() => CurrentPage?.OnDisable();

        public abstract void OnDone();

        public virtual void OnBack()
        {

            if (currentIndex <= 0)
                return;

            Pages[currentIndex]?.SetWindow(this);
            Pages[currentIndex]?.OnDisable();
            currentIndex -= 1;
            Pages[currentIndex]?.SetWindow(this);
            Pages[currentIndex]?.OnEnable();
            isOnLast = false;

        }

        public virtual void OnContinue(bool skipped)
        {

            if (skipped) Pages[currentIndex]?.OnSkip();
            if (isOnLast)
                OnDone();

            if (currentIndex >= Pages.Length - 1)
                return;

            Pages[currentIndex]?.SetWindow(this);
            Pages[currentIndex]?.OnDisable();
            currentIndex += 1;
            Pages[currentIndex]?.SetWindow(this);
            Pages[currentIndex]?.OnEnable();
            isOnLast = currentIndex == Pages.Length - 1;

        }

        public virtual void OnFooter()
        {

            GUI.enabled = currentIndex > 0;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous"))
                OnBack();

            GUILayout.FlexibleSpace();

            if (Pages[currentIndex].CanSkip && GUILayout.Button("Skip"))
                OnContinue(skipped: true);

            GUI.enabled = Pages[currentIndex].CanContinue;
            if (GUILayout.Button(isOnLast ? "Done" : "Continue"))
                OnContinue(skipped: false);
            GUI.enabled = true;

            GUILayout.EndHorizontal();

        }

        public virtual void OnContent()
        {
            Pages[currentIndex]?.SetWindow(this);
            Pages[currentIndex].OnGUI();
        }

        GUIStyle style;
        void OnGUI()
        {

            if (style == null)
                style = new GUIStyle() { margin = new RectOffset(22, 22, 22, 22) };

            GUILayout.BeginVertical(style);
            OnContent();
            GUILayout.FlexibleSpace();
            OnFooter();
            GUILayout.EndVertical();

            if (minSize.x < 100 || minSize.y < 100)
                minSize = new Vector2(Mathf.Max(100, minSize.x), Mathf.Max(100, minSize.y));

            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl("");
                Repaint();
            }

        }

        public void SetDone() =>
            OnDone();

    }

}

