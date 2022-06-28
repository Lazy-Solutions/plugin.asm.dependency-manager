using UnityEditor;

namespace plugin.asm.dependency_manager.Utility
{

    static class CoroutineUtilityPackageFix
    {

        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            _ = UnityEditor.PackageManager.Client.Add("com.unity.editorcoroutines@1.0");
            _ = UnityEditor.PackageManager.Client.Remove("utility.lazy.coroutines");
        }

    }

}
