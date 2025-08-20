using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CreateDataScriptsFromCSV
{
    [MenuItem("Tools/Data/Generate Scripts from CSV")]
    public static void GenerateScripts()
    {
        string csvFilePath = EditorUtility.OpenFilePanel("Select CSV File", Path.Combine(Application.dataPath, "1_Resources/Data/"), "csv");

        if (string.IsNullOrEmpty(csvFilePath)) { return; }

        string fileName = Path.GetFileNameWithoutExtension(csvFilePath);
        string dataClassName = fileName;

        string dataClassPath = Path.Combine(Application.dataPath, "0_Scripts/Data", dataClassName + ".cs");

        string[] lines = File.ReadAllLines(csvFilePath);
        if (lines.Length < 2)
        {
            Debug.LogError("CSV file must contain at least a header and one data row.");
            return;
        }

        string dataClassCode = GenerateDataClassCode(dataClassName, lines);
        string dataLoaderCode = GenerateDataLoaderCode(dataClassName, lines);

        File.WriteAllText(dataClassPath, dataClassCode + dataLoaderCode);

        AssetDatabase.Refresh();
    }

    private static string GenerateDataClassCode(string dataClassName, string[] lines)
    {
        StringBuilder sb = new StringBuilder();

        string[] headers = lines[0].Trim().Split(',');
        string[] dataTypes = lines[1].Trim().Split(',');

        sb.AppendLine("// Auto-generated data class from CSV");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.IO;");
        sb.AppendLine("using System.Text;");
        sb.AppendLine("using UnityEditor;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine("[System.Serializable]");
        sb.AppendLine($"public class {dataClassName}");
        sb.AppendLine("{");
        for (int i = 0; i < headers.Length; i++)
        {
            string fieldName = headers[i].Trim();
            string fieldType = dataTypes[i].Trim();

            sb.AppendLine($"    public {fieldType} {fieldName};");
        }
        sb.AppendLine("}");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string GenerateDataLoaderCode(string dataClassName, string[] lines)
    {
        StringBuilder sb = new StringBuilder();

        string[] headers = lines[0].Trim().Split(',');
        string[] dataTypes = lines[1].Trim().Split(',');

        string dataPath = Path.Combine(Application.streamingAssetsPath, $"Data/{dataClassName}");

        sb.AppendLine($"public class {dataClassName}Loader");
        sb.AppendLine("{");
        sb.AppendLine($"    public Dictionary<int, {dataClassName}> Load()");
        sb.AppendLine($"    {{");
        sb.AppendLine($"        Dictionary<int, {dataClassName}> dict = new Dictionary<int, {dataClassName}>();");
        // sb.AppendLine($"        TextAsset csvFile = Resources.Load<TextAsset>(\"{dataPath}\");");
        sb.AppendLine($"        string dataPath = Application.streamingAssetsPath + \"/Data/{dataClassName}.csv\";");
        sb.AppendLine($"        if (!File.Exists(dataPath))");
        sb.AppendLine($"        {{");
        sb.AppendLine($"            Debug.LogError(\"CSV file not found in Resources/Data/\");");
        sb.AppendLine($"            return dict;");
        sb.AppendLine($"        }}");
        sb.AppendLine($"        string[] lines = File.ReadAllLines(dataPath);");
        sb.AppendLine($"        for (int i = 2; i < lines.Length; i++)");
        sb.AppendLine($"        {{");
        sb.AppendLine($"            string line = lines[i].Trim();");
        sb.AppendLine($"            if (string.IsNullOrEmpty(line) || line.StartsWith(\"#\")) continue;");

        sb.AppendLine($"            string[] values = line.Split(',');");
        sb.AppendLine($"            {dataClassName} data = new {dataClassName}();");
        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i].Trim();
            string type = dataTypes[i].Trim();

            if (type == "string")
                sb.AppendLine($"            data.{header} = values[{i}].Trim();");
            else
                sb.AppendLine($"            data.{header} = {type}.Parse(values[{i}].Trim());");
        }
        sb.AppendLine($"            dict.Add(data.ID, data);");
        sb.AppendLine($"        }}");
        sb.AppendLine($"        return dict;");
        sb.AppendLine($"    }}");
        sb.AppendLine("}");

        return sb.ToString();
    }
}