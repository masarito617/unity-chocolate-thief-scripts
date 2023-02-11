using UnityEngine;

namespace Chocolate.Common
{
    public class ScreenShotBehaviour : MonoBehaviour
    {
        // スクリーンショット保存パス
        private const string ScreenShotFilePath = "/Users/molegoro/Screenshot/Choco/";

        private void Awake()
        {
            // ゲーム内に一つだけ保持
            if (FindObjectsOfType<ScreenShotBehaviour>().Length > 1)
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            // スペースキーが押されたら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // スクリーンショットを保存
                System.DateTime date = System.DateTime.Now;
                string fileName = "Screenshot_" + date.ToString("yyyyMMddHHmmss") + ".png";
                ScreenCapture.CaptureScreenshot(ScreenShotFilePath + fileName);
                Debug.Log("Save to " + fileName);
            }
        }
    }
}
