// Auto-generated data class from CSV
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TestData
{
    public int ID;
    public string Name;
    public float HP;
    public int Gold;
}

public class TestDataLoader
{
    public Dictionary<int, TestData> Load()
    {
        Dictionary<int, TestData> dict = new Dictionary<int, TestData>();
        string dataPath = Application.streamingAssetsPath + "/Data/TestData.csv";
        if (!File.Exists(dataPath))
        {
            Debug.LogError("CSV file not found in Resources/Data/");
            return dict;
        }
        string[] lines = File.ReadAllLines(dataPath);
        for (int i = 2; i < lines.Length; i++)
        {
            int cnt = 0;
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
            string[] values = line.Split(',');
            TestData data = new TestData();
            data.ID = int.Parse(values[cnt++].Trim());
            data.Name = values[cnt++].Trim();
            data.HP = float.Parse(values[cnt++].Trim());
            data.Gold = int.Parse(values[cnt++].Trim());
            dict.Add(data.ID, data);
        }
        return dict;
    }
}
