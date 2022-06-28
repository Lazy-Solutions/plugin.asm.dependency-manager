using UnityEditor;
using UnityEditor.PackageManager;

namespace plugin.asm.dependency_manager.Utility
{

    static class EnsureEditorCoroutinesAdded
    {

        [InitializeOnLoadMethod]
        static void OnLoad() =>
            _ = Client.Add("com.unity.editorcoroutines@1.0");

    }

}
