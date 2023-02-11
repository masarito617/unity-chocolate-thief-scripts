using UnityEditor;
using UnityEngine;

namespace Chocolate.Editor
{
    // スクリプト生成の度にコンパイル走るのが苦痛なのでロックメニューを追加
    public class CompileLock : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Compile/Lock", false, 1)]
        static void Lock ()
        {
            Debug.Log("Lock Compile!!");
            EditorApplication.LockReloadAssemblies();
        }

        [MenuItem("Compile/UnLock", false, 1)]
        static void Unlock ()
        {
            Debug.Log("UnLock Compile!!");
            EditorApplication.UnlockReloadAssemblies();
        }
#endif
    }
}
