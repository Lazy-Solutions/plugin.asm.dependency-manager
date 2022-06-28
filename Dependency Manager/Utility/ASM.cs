#pragma warning disable IDE1006 // Naming Styles

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace plugin.asm.dependency_manager.Utility
{

    public static class ASM
    {

        public static event Action OnUninstalled;

        internal static void RaiseOnUninstalled() =>
            OnUninstalled?.Invoke();

        /// <summary>Gets the current version of ASM.</summary>
        public static string version => GetVersion();

        const string versionFile = "Assets/AdvancedSceneManager/Dependency Manager/Resources/AdvancedSceneManager/version.txt";
        const string versionResource = "AdvancedSceneManager/version";

        static string GetVersion()
        {
            var resource = Resources.Load<TextAsset>(versionResource);
            if (resource)
                return resource.text;
            else
                return "1.3.1"; //This code was added in this version, and plugin code for earlier versions are compatible with this one
        }

        internal static void SetVersion(string version)
        {
            Directory.GetParent(versionFile).Create();
            File.WriteAllText(versionFile, version);
            AssetDatabase.Refresh();
        }

        internal static string[] assemblyNames =
        {
            "AdvancedSceneManager",
            "AdvancedSceneManager.Editor",
        };

        internal const string thisAssembly = "plugin.asm.dependency-manager";
        internal const string pragma = "ASM";

        public static readonly Dependency[] dependencies = new Dependency[]
        { };

        /// <summary>Gets if ASM is installed.</summary>
        public static bool IsInstalled()
        {
            var assemblies = AssetDatabase.FindAssets("t:asmdef").Select(AssetDatabase.GUIDToAssetPath);
            return assemblyNames.All(assembly => assemblies.Any(a => a.EndsWith(assembly + ".asmdef")));
        }

    }

}