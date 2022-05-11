using plugin.asm.dependency_manager.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace plugin.asm.dependency_manager.Intro
{

    public class DependencyManagerPage : WizardPage
    {

        #region Git

        static bool IsGitInstalled() =>
             Environment.GetEnvironmentVariable("path").ToLower().Contains(@"\git");

        readonly static Uri Git32Link = new Uri("https://github.com/git-for-windows/git/releases/download/v2.35.2.windows.1/Git-2.35.2-32-bit.exe");
        readonly static Uri Git64Link = new Uri("https://github.com/git-for-windows/git/releases/download/v2.35.2.windows.1/Git-2.35.2-64-bit.exe");

        async static void InstallGit()
        {

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                if (EditorUtility.DisplayDialog("Installing git...", "We will now open downloads page for git. Please be aware that Unity + Hub may have to be restarted in order to access it from unity.", ok: "ok"))
                    _ = Process.Start("https://git-scm.com/downloads");

            }
            else
            {

                var paths = new List<string>();
                try
                {

                    EditorUtility.DisplayProgressBar("Installing git...", "Downloading...", 0);
                    var path = await DownloadGit();

                    EditorUtility.DisplayProgressBar("Installing git...", "Installing...", 0.5f);
                    await Install(path);
                    EditorUtility.DisplayProgressBar("Installing git...", "Cleaning up...", 1f);
                    Cleanup();
                    EditorUtility.ClearProgressBar();
                    Restart();

                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    EditorUtility.DisplayProgressBar("Installing git...", "Cleaning up...", 1f);
                    Cleanup();
                    EditorUtility.ClearProgressBar();
                }

                async Task<string> DownloadGit()
                {

                    var uri = Environment.Is64BitOperatingSystem ? Git64Link : Git32Link;

                    var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".exe");

                    if (File.Exists(path))
                        throw new IOException("The file already exists.");

                    paths.Add(path);

                    using (var client = new HttpClient())
                    using (var stream = await client.GetStreamAsync(uri))
                    using (var fs = new FileStream(path, FileMode.Create))
                        await stream.CopyToAsync(fs);

                    return path;

                }

                Task Install(string path) =>
                    Task.Run(() =>
                    {

                        try
                        {

                            var p = Process.Start(path, "/SILENT /NOCANCEL");
                            p.WaitForExit();

                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.Message);
                            throw;
                        }

                    });

                void Restart()
                {

                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        AssetDatabase.SaveAssets();

                    if (EditorUtility.DisplayDialog("Installing git...", "In order to finish installing git, unity + hub must be restarted.\n\nThis will close all unity instances, please save changes in other instances before proceeding.", ok: "Close unity + hub", cancel: "Restart later"))
                    {
                        _ = Process.Start(
                            new ProcessStartInfo("cmd", "/C taskkill /IM \"Unity Hub.exe\" /T /F")
                            { UseShellExecute = true, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden });
                    }

                }

                void Cleanup()
                {
                    foreach (var file in paths)
                        if (File.Exists(file))
                            File.Delete(file);
                }

            }

        }

        #endregion
        #region GUI

        public override void OnEnable() =>
            Reload();

        static GUIStyle margin;
        static GUIStyle labelBold;
        static GUIStyle label;

        enum DependencyStatus
        {
            Uninstalled, Installing, Installed
        }

        static readonly List<Request> requests = new List<Request>();
        readonly static Dictionary<Dependency, DependencyStatus> dependencies = new Dictionary<Dependency, DependencyStatus>();
        static bool isGitInstalled;
        static bool isReloading;

        public override void OnGUI()
        {

            if (window)
                window.minSize = window.maxSize = new Vector2(635, 300);


            //Fix for window becoming uninteractable sometimes when an error occurs during reload
            if (isReloading && (requests.Count == 0 || requests.Any(r => r.IsCompleted)))
                isReloading = false;

            if (margin is null) margin = new GUIStyle() { margin = new RectOffset(22, 22, 22, 22) };
            if (labelBold is null) labelBold = new GUIStyle(EditorStyles.boldLabel) { margin = new RectOffset(0, 0, 42, 8), alignment = TextAnchor.UpperCenter, fontSize = 14 };
            if (label is null) label = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.UpperCenter };

            if (dependencies.Count == 0)
            {
                dependencies.Add(new Dependency() { packageName = "git", tooltip = "Needed to download git packages (↓)" }, DependencyStatus.Uninstalled);
                foreach (var dependency in ASM.dependencies)
                    dependencies.Add(dependency, DependencyStatus.Uninstalled);
                Reload();
            }

            GUILayout.Label("Welcome to Advanced Scene Manager!", labelBold);
            GUILayout.Label("Before we can get started, ASM needs just a few dependencies:", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });

            GUI.enabled = !isReloading && isGitInstalled;
            GUILayout.BeginVertical(margin);

            foreach (var dependency in dependencies.Keys.OrderByDescending(d => d.packageName == "git").ToArray())
            {

                GUILayout.BeginHorizontal();

                GUILayout.Label(dependency.packageName, GUILayout.Width(200), GUILayout.Height(22));

                var c = GUI.color;
                GUI.color = Color.gray;
                GUILayout.Label("- " + dependency.tooltip, GUILayout.Width(290));
                GUI.color = c;

                var e = GUI.enabled;
                if (dependency.packageName == "git" && dependencies[dependency] == DependencyStatus.Uninstalled)
                    GUI.enabled = true;

                if (!isReloading)
                    DrawItemStatus[dependencies[dependency]].Invoke(dependency);

                GUI.enabled = e;

                GUILayout.EndHorizontal();

            }

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Space(isReloading ? 480 : 502);
            if (!CanContinue)
                if (GUILayout.Button(isReloading ? "Reloading..." : "Reload", GUILayout.Width(isReloading ? 86 : 64)))
                    Reload();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.enabled = true;

        }

        readonly Dictionary<DependencyStatus, Action<Dependency>> DrawItemStatus = new Dictionary<DependencyStatus, Action<Dependency>>()
        {
            { DependencyStatus.Uninstalled, (d) => { if (GUILayout.Button("Install", GUILayout.Width(64))) Install(d); } },
            { DependencyStatus.Installed, (_) => GUILayout.Label("✓", label, GUILayout.Width(64)) },
            { DependencyStatus.Installing, (_) => GUILayout.Label("Installing...", label, GUILayout.Width(64)) },
        };

        #endregion
        #region Install / Reload

        static void Install(Dependency dependency)
        {

            if (dependency.packageName == "git")
            {
                InstallGit();
                return;
            }

            isReloading = true;
            dependencies[dependency] = DependencyStatus.Installing;
            if (IntroWizard.instance) IntroWizard.instance.Repaint();

            var request = Client.Add(dependency.GetManifestValue());
            requests.Add(request);

            EditorApplication.update += Update;

            void Update()
            {

                if (!request.IsCompleted)
                    return;
                EditorApplication.update -= Update;

                if (request.Status == StatusCode.Failure)
                    Debug.LogError(request.Error.message);

                _ = requests.Remove(request);

                Reload();

            }

        }

        static void Reload()
        {

            isReloading = true;
            if (IntroWizard.instance) IntroWizard.instance.Repaint();

            var request = Client.List();
            requests.Add(request);
            EditorApplication.update += Update;

            void Update()
            {

                if (!request.IsCompleted)
                    return;

                EditorApplication.update -= Update;
                if (request.Status == StatusCode.Success)
                    AfterUpdate();
                else
                    Debug.LogError(request.Error.message);

            }

            void AfterUpdate()
            {

                foreach (var dependency in dependencies.Keys.ToArray())
                {

                    var isInstalled = dependency.packageName == "git"
                        ? isGitInstalled = IsGitInstalled()
                        : request.Result.Any(p => p.name == dependency.packageName);

                    dependencies[dependency] = isInstalled ? DependencyStatus.Installed : DependencyStatus.Uninstalled;

                }

                _ = requests.Remove(request);

                if (dependencies.Values.All(status => status == DependencyStatus.Installed))
                    OnAllInstalled();
                else
                    ScriptingDefineUtility.Unset(ASM.pragma);

                isReloading = false;
                if (IntroWizard.instance)
                    IntroWizard.instance.Repaint();
            }

        }

        public override bool CanContinue => dependencies.Values.All(status => status == DependencyStatus.Installed);

        public static bool hasJustInstalledDependencies
        {
            get => EditorPrefs.GetBool("AdvancedSceneManager.hasJustInstalledDependencies");
            internal set => EditorPrefs.SetBool("AdvancedSceneManager.hasJustInstalledDependencies", value);
        }

        static void OnAllInstalled() =>
            hasJustInstalledDependencies = true;

        #endregion

    }

}

