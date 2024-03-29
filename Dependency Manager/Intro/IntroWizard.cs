﻿using plugin.asm.dependency_manager.Utility;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace plugin.asm.dependency_manager.Intro
{

    public class IntroWizard : WizardWindow
    {

        public static bool didJustFinishInto
        {
            get => PlayerPrefs.GetInt("AdvancedSceneManager.Intro.didJustFinishIntro") == 1;
            set => PlayerPrefs.SetInt("AdvancedSceneManager.Intro.didJustFinishIntro", value ? 1 : 0);
        }

        public override WizardPage[] Pages { get; } = new WizardPage[]
        {
            new WelcomePage(),
            new LinksPage(),
            new BlacklistPage(),
            new ProfilePage(),
        };

        public override void OnDone()
        {
            didJustFinishInto = true;
            ScriptingDefineUtility.Set(ASM.pragma);
            Close();
        }

        #region Open

        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            if (!Application.isPlaying)
                EditorApplication.delayCall += EditorApplication.delayCall += () => OpenCorrectWindowForContext(isOnLoad: true);
        }

        [MenuItem("File/Scene Manager... %#m", priority = 205)]
        [MenuItem("Tools/Advanced Scene Manager/Window/Scene Manager Window", priority = 40)]
        static void MenuItem() =>
            OpenCorrectWindowForContext(isMenuItem: true);

        internal static IntroWizard instance =>
            Resources.FindObjectsOfTypeAll<IntroWizard>().FirstOrDefault() is IntroWizard window && window
            ? window
            : null;

        public static event Action<OnOpenArgs> OnRequestOpen;

        public static void RequestOpen(bool isMenuItem, bool isOnLoad, out bool isHandled)
        {
            var e = new OnOpenArgs(isMenuItem, isOnLoad, didJustFinishInto);
            OnRequestOpen?.Invoke(e);
            isHandled = e.isHandled;
        }

        public class OnOpenArgs : EventArgs
        {

            public OnOpenArgs(bool isMenuItem, bool isOnLoad, bool wasJustInstalled)
            {
                this.isMenuItem = isMenuItem;
                this.isOnLoad = isOnLoad;
                this.wasJustInstalled = wasJustInstalled;
            }

            public bool isMenuItem { get; }
            public bool isOnLoad { get; }
            public bool wasJustInstalled { get; }
            public bool isHandled { get; set; }

        }

        static void TryClose()
        {
            if (instance)
                instance.Close();
        }

        /// <summary>Opens the depencency manager window if any dependency missing, otherwise opens scene manager window.</summary>
        static void OpenCorrectWindowForContext(bool isOnLoad = false, bool isMenuItem = false)
        {

            if (!ScriptingDefineUtility.IsSet("ASM") || !ASM.IsInstalled())
                Open();
            else
            {
                RequestOpen(isMenuItem, isOnLoad, out var isHandled);
                if (isHandled)
                    TryClose();
                else if (!ASM.IsInstalled())
                    Open();
            }

        }

        internal static void Open()
        {

            if (instance)
            {
                instance.Show();
                instance.Focus();
            }
            else
            {
                var w = GetWindow<IntroWizard>();
                w.titleContent = new GUIContent("Scene Manager");
                w.Show();
            }

        }

        #endregion

    }

}

