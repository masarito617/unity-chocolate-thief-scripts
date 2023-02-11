using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Chocolate.Services.Impl
{
    /// <summary>
    /// SocialConnector v0.2.9 を使用
    /// </summary>
    public class SocialService : ISocialService
    {
        private static readonly string ScreenShotFileName = "social_connect_image.png";

        public async void TweetWithScreenShotAsync(string tweetText, string tweetUrl)
        {
            var imagePass = Application.persistentDataPath + "/" + ScreenShotFileName;

            // 前回の画像を削除して新しいスクリーンショットを取得
            File.Delete(imagePass);
            ScreenCapture.CaptureScreenshot(ScreenShotFileName);

            // スクショ撮影のラグがあるため終わるまで待機
            await UniTask.WaitUntil(() => File.Exists(imagePass));

            // 投稿
            SocialConnector.PostMessage(SocialConnector.ServiceType.Twitter,tweetText, tweetUrl, imagePass);
        }
    }
}
