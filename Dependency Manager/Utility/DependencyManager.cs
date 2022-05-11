#pragma warning disable IDE0046 // Convert to conditional expression

using plugin.asm.dependency_manager.Intro;
using System;
using System.Linq;
using UnityEditor;

namespace plugin.asm.dependency_manager.Utility
{

    class Deactivator : AssetPostprocessor
    {

        static void OnPostprocessAllAssets(string[] __, string[] deletedAssets, string[] ___, string[] ____)
        {

            var removedAssemblies = deletedAssets.Where(a => AssetDatabase.IsValidFolder(a)).SelectMany(a => AssetDatabase.FindAssets("t:asmdef", new[] { a })).Concat(deletedAssets.Where(a => a.EndsWith(".asmdef"))).ToArray();
            var isDependencyRemoved = ASM.dependencies.Any(d => IsRemoved(d.packageName, isPackage: true));

            var isCoreAssemblyRemoved = !ASM.IsInstalled();

            if (isDependencyRemoved || isCoreAssemblyRemoved)
            {

                ASM.RaiseOnUninstalled();
                ScriptingDefineUtility.Unset(ASM.pragma);
                IntroWizard.Open();

            }

            bool IsRemoved(string packageName, bool isPackage) =>
                removedAssemblies.Any(a => a.StartsWith("Packages/" + packageName + "/")) || //Check package
                removedAssemblies.Any(a => a.EndsWith("/" + packageName + ".asmdef")); //Check in assets

        }

    }

    /// <summary>Represents a dependency.</summary>
    public class Dependency
    {

        public static Dependency OnGitPackage(string packageName, string uri) =>
            new Dependency() { packageName = packageName, uri = uri };

        public static Dependency OnUnityPackage(string packageName, string version = "1.0.0") =>
            new Dependency() { packageName = packageName, version = version };

        public Dependency WithTooltip(string tooltip)
        {
            this.tooltip = tooltip;
            return this;
        }

        /// <summary>The package name of the dependency.</summary>
        public string packageName;

        /// <summary>The tooltip of the dependency.</summary>
        public string tooltip;

        /// <summary>The version number of the dependency, not supported for git packages.</summary>
        public string version;

        /// <summary>The git uri of the dependency, not supported for unity packages.</summary>
        public string uri;

        /// <summary>Gets the value of the item in the package manifest, this is either <see cref="version"/> or <see cref="uri"/>.</summary>
        public string GetManifestValue()
        {
            if (!string.IsNullOrWhiteSpace(version))
                return packageName;
            else if (!string.IsNullOrWhiteSpace(uri))
                return uri;
            else
                throw new NullReferenceException($"Dependencies must define either '{nameof(version)}' or '{nameof(uri)}'.");
        }

    }

}