using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class SceneEnumGenerator
{
    [MenuItem("Tools/Generate SceneEnum Enum")]
    public static void GenerateSceneEnum()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// This file is auto-generated. Modifications may be overwritten.");
        sb.AppendLine("public enum SceneEnum");
        sb.AppendLine("{");
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath).Replace(" ", "_");
            sb.AppendLine($"    {sceneName},");
        }
        sb.AppendLine("}");

        string filePath = Path.Combine(Application.dataPath, "Scripts/Generated/SceneEnum.cs");
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        File.WriteAllText(filePath, sb.ToString());
        AssetDatabase.Refresh();
    }
}
