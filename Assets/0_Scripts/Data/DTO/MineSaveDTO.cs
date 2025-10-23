using System;
using System.Collections.Generic;

[Serializable]
public class MineSaveDTO
{
    public string Id;
    public int CurrentDepth;
    public List<LineSaveDTO> Lines;
}

[Serializable]
public class LineSaveDTO
{
    public int Depth;
    public bool IsTopLine;
    public List<RockSaveDTO> Rocks;
    public List<VeinSaveDTO> Veins;
}

[Serializable]
public class RockSaveDTO
{
    public string Id;  // HEX (depth * 100) + 1 ~ E
    public int Hp;
}

[Serializable]
public class VeinSaveDTO
{
    public string Id;    // HEX (depth * 100) + (1 ~ E * 10)
    public string Pos;   // Rock Id
    public int Type;
}

