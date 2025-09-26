using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MineDataManager
{
    private SecureDataManager dataManager = new SecureDataManager();

    public MineData MineData { get; set; }

    /// <summary>
    /// 현재 Mine의 모든 상태를 한 번에 저장
    /// 수동 세이브 시 사용
    /// </summary>
    public void SaveAll(UI_MineFloors mine)
    {
        if (mine == null) {
            Debug.LogError("Mine is null!");
            return;
        }

        EnsureSize(MineData.Lines, mine.Lines.Count);
        foreach (var line in mine.Lines) {
            var idx = line.Depth;
            if (MineData.Lines[idx] == null) MineData.Lines[idx] = new LineData(line);
            else MineData.Lines[idx].Save(line);
        }
    }

    /// <summary>
    /// 특정 층(라인)만 저장
    /// 한 층이 클리어되어 새로운 층이 생겼을 때
    /// </summary>
    public void SaveLine(UI_MiningLine line)
    {
        if (line == null) return;
        var idx = line.Depth;
        EnsureSize(MineData.Lines, idx + 1);
        if (MineData.Lines[idx] == null) MineData.Lines[idx] = new LineData(line);
        else MineData.Lines[idx].Save(line);
    }

    /// <summary>
    /// List<T>의 사이즈를 다른 사이즈만큼 확보해주는 헬퍼 메서드
    /// </summary>
    static void EnsureSize<T>(List<T> list, int size) where T : class, new()
    {
        while (list.Count < size) list.Add(null);
    }

    public void Load(int saveIndex)
    {
        //MineData = dataManager.Load() as MineData;
        string path = Path.Combine(Application.persistentDataPath, $"save_0.json");
        string json = File.ReadAllText(path);
        MineData = JsonUtility.FromJson<MineData>(json);
    }
}

[Serializable]
public class MineData : ISaveable
{
    public string MineId = "default";
    //public int CurrentFloor = 0; // 최상단 층 인덱스(0 맨 위)
    public List<LineData> Lines = new();
}

[Serializable]
public class LineData
{
    public int LineId = 0;  // == line depth

    public bool IsTopLine = false;

    public List<RockData> Rocks = new(); // 바위
    public List<VeinData> Veins = new(); // 광맥

    public bool IsCleared {
        get {
            if (Rocks == null || Rocks.Count == 0) return true;
            for (int i = 0; i < Rocks.Count; i++)
                if (Rocks[i].Hp != 0) return false;
            return true;
        }
    }

    public LineData() { }

    public LineData(UI_MiningLine line)
    {
        Save(line);
    }

    public void Save(UI_MiningLine line)
    {
        LineId = line.Depth;
        IsTopLine = line.IsTopLine;

        Rocks.Clear();
        foreach (var rock in line._rocks) {
            RockData rockData = new RockData();
            rockData.Hp = rock.Rock._hp;
            Rocks.Add(rockData);
        }

        Veins.Clear();
        foreach (var vein in line._oreVeins) {
            VeinData veinData = new();
            veinData.Type = (int)vein.OreBase.Type;
            veinData.Index = vein.PosIndex;
            veinData.IsActive = vein.isActiveAndEnabled;
            Veins.Add(veinData);
        }
    }
}

[Serializable]
public class RockData
{
    public int Hp = 0;
    public bool IsBroken => Hp <= 0;
}

[Serializable]
public class VeinData
{
    public int Type = 0; // 광물 타입
    public int Index = 0;
    public bool IsActive = false;
}
