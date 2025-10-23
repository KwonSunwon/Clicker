using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class MineSaveData
{
    public List<MiningLineSaveData> lines = new();

    void SaveMine(BinaryWriter bw)
    {
        bw.Write(lines.Count);
        foreach (MiningLineSaveData line in lines)
        {
            line.Write(bw);
        }
    }
}

[System.Serializable]
public class MiningLineSaveData
{
    public int Id; // == Depth
    public int VeinCount;
    public int RockCount;
    public byte IsCleared;
    public byte IsTop;
    public byte[] row = new byte[6];

    public void Write(BinaryWriter bw)
    {
        bw.Write(Id);
        bw.Write(VeinCount);
        bw.Write(IsCleared);
        bw.Write(IsTop);

        for (int i = 0; i < 6; i++)
        {
            bw.Write(row[i]);
        }
    }
}