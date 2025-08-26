// Auto-generated data class from CSV
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TestSkillData
{
    public int ID;
    public string skillName;
    public string description;
    public string effectType;
    public int requiredLevel;
    public long cost;
    public List<string> parameters;
}

public class TestSkillDataLoader
{
    public Dictionary<int, TestSkillData> Load()
    {
        Dictionary<int, TestSkillData> dict = new Dictionary<int, TestSkillData>();
        string dataPath = Application.streamingAssetsPath + "/Data/TestSkillData.csv";
        if (!File.Exists(dataPath))
        {
            Debug.LogError("CSV file not found in Resources/Data/");
            return dict;
        }
        string[] lines = File.ReadAllLines(dataPath);
        for (int i = 2; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
            string[] values = line.Split(',');
            int cnt = 0;

            TestSkillData data = new TestSkillData();
            data.ID = int.Parse(values[cnt++].Trim());
            data.skillName = values[cnt++].Trim();
            data.description = values[cnt++].Trim();
            data.effectType = values[cnt++].Trim();
            data.requiredLevel = int.Parse(values[cnt++].Trim());
            data.cost = long.Parse(values[cnt++].Trim());

            data.parameters = new List<string>();
            do
            {
                data.parameters.Add(values[cnt].Trim().Replace("\"", ""));
            } while (values[cnt++] != values[^1]);

            dict.Add(data.ID, data);
        }
        return dict;
    }
}
