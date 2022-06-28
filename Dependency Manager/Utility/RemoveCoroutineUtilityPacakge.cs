using UnityEditor;

namespace plugin.asm.dependency_manager.Utility
{

    static class RemoveCoroutineUtilityPacakge
    {

        [InitializeOnLoadMethod]
        static void OnLoad() =>
            _ = UnityEditor.PackageManager.Client.Remove("utility.lazy.coroutines");

    }

}