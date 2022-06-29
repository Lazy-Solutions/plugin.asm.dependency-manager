using System.Linq;
using UnityEditor;

namespace plugin.asm.dependency_manager.Utility
{

    class OnDeactivated : AssetPostprocessor
    {

        static void OnPostprocessAllAssets(string[] __, string[] deletedAssets, string[] ___, string[] ____)
        {
            var removedAssemblies = deletedAssets.Where(a => AssetDatabase.IsValidFolder(a)).SelectMany(a => AssetDatabase.FindAssets("t:asmdef", new[] { a })).Concat(deletedAssets.Where(a => a.EndsWith(".asmdef"))).ToArray();
            var isCoreAssemblyRemoved = !ASM.IsInstalled();
            if (isCoreAssemblyRemoved)
                ScriptingDefineUtility.Unset(ASM.pragma);
        }

    }

}
