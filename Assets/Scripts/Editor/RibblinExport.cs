#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Izvoz gotove igre za Windows: izbornik "Ribblin → Izvezi igru (Windows)".
// Igra se spremi u mapu Build/Windows (Ribblin.exe), a zatim se spakira u Build/Ribblin_Windows.zip
// koji se može izravno objaviti (npr. na itch.io). Mapa Build ne ide na GitHub (.gitignore).
public static class RibblinExport
{
    const string OutDir = "Build/Windows";
    const string ZipPath = "Build/Ribblin_Windows.zip";

    [MenuItem("Ribblin/Izvezi igru (Windows)")]
    public static void ExportWindows()
    {
        // scene redom kako su u Build Settings (postavlja ih "Izgradi cijelu igru")
        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
        {
            EditorUtility.DisplayDialog("Ribblin", "Nema scena u Build Settings. Prvo pokreni Ribblin → Izgradi cijelu igru.", "OK");
            return;
        }

        PlayerSettings.productName = "Ribblin";
        PlayerSettings.companyName = "Lidia Butkovic";
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.resizableWindow = true;

        if (Directory.Exists(OutDir)) Directory.Delete(OutDir, true);
        Directory.CreateDirectory(OutDir);

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutDir + "/Ribblin.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,   // gotova verzija: tipke za testiranje (F1–F6) ne rade
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError("Ribblin: izvoz nije uspio (" + report.summary.result + "). Pogledaj greške u konzoli.");
            return;
        }

        // mapa s podacima za otklanjanje grešaka ne treba igračima
        foreach (var dir in Directory.GetDirectories(OutDir, "*DoNotShip*")) Directory.Delete(dir, true);

        if (File.Exists(ZipPath)) File.Delete(ZipPath);
        ZipFile.CreateFromDirectory(OutDir, ZipPath, System.IO.Compression.CompressionLevel.Optimal, false);

        long mb = new FileInfo(ZipPath).Length / (1024 * 1024);
        Debug.Log($"Ribblin: igra je izvezena u {Path.GetFullPath(OutDir)} i spakirana u {Path.GetFullPath(ZipPath)} ({mb} MB).");
        EditorUtility.RevealInFinder(ZipPath);
    }
}
#endif
