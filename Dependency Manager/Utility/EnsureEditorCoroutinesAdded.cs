using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;

namespace plugin.asm.dependency_manager.Utility
{

    static class EnsureEditorCoroutinesAdded
    {

        [InitializeOnLoadMethod]
        static void OnLoad()
        {

            //Editor coroutine package is a dependency for CoroutineUtility (a direct dependency of ASM, which is embedded)
            if (!File.ReadAllText("Packages/manifest.json").Contains("com.unity.editorcoroutines"))
                _ = Client.Add("com.unity.editorcoroutines@1.0");

        }

    }

}
