using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Chocolate.Editor
{
    /// <summary>
    /// ビルド後処理
    /// </summary>
    public class PostBuildProcessor : IPostprocessBuildWithReport
    {
        /// <summary>
        /// 実行順
        /// </summary>
        public int callbackOrder => 100;

        public void OnPostprocessBuild(BuildReport report)
        {
#if UNITY_IOS
            // plistの読込
            var plist = new PlistDocument();
            var plistPath = report.summary.outputPath + "/Info.plist";
            plist.ReadFromString(File.ReadAllText(plistPath));

            // ATTトラッキング対応：NSUserTrackingUsageDescription の追加
            var plistRoot = plist.root;
            plistRoot.SetString("NSUserTrackingUsageDescription", "許可すると好みに合った広告が表示されやすくなります。");
            File.WriteAllText(plistPath, plist.WriteToString());

            // pbxプロジェクトの読込
            var project = new PBXProject();
            var projectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
            project.ReadFromString(File.ReadAllText(projectPath));

            // pbxプロジェクトの設定
            var targetGuid = project.GetUnityFrameworkTargetGuid();
            // ATTトラッキング対応：UnityFramework に AuthenticationServices.framework を追加
            project.AddFrameworkToProject(targetGuid, "AuthenticationServices.framework", false);
            // Archiveビルド対応：BitCodeを無効にする
            project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            File.WriteAllText(projectPath, project.WriteToString());
#endif
        }
    }
}