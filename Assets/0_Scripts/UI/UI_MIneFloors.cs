using System.Collections.Generic;
using UnityEngine;

//NOTE: Mine 관련 오브젝트 구조
// MineFloors: Canvas - Mine(ScrollRect) - Viewport - Floors에 Componet로 존재, MiningLine을 추가 및 관리 하는 기능
//     - Sky
//     - Ground
//     - MiningLine: 버튼(바위)을 구성하고 광맥을 생성하는 기능, 바위가 다 사라지면 바위를 파괴하고 광맥만 유지, 아래 줄에 Line이 들어나도록 함
//         - MineButton * 6
//         - OreVein
//     - ...
//     - MiningLine
//         - MineButton * 6
//         - OreVein
//     - Dark: 가장 아래에 위치, MiningLine이 추가될 때마다 위로 올라감

//TODO: 맨 위에 MiningLine만 클릭 가능하도록 설정

public class UI_MineFloors : UI_Base
{
    enum GameObjects
    {
        Floors,
        Sky,
        Ground,
        Dark,
    }

    enum Buttons
    {
        //UI_MineButton,
    }

    //NOTE: Floors 맨 아래에 있는 오브젝트로 새로운 라인이 추가될 때 해당 오브젝트 위에 추가
    //TODO: 어두운 배경으로해서 깊이 있는 곳은 안보이는 것처럼 보이게하는 효과 적용해야함
    GameObject _dark;

    private List<UI_MiningLine> _lines = new List<UI_MiningLine>();
    public List<UI_MiningLine> Lines { get { return _lines; } }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _dark = GetObject((int)GameObjects.Dark);

        _lines = new List<UI_MiningLine>();
        _lines.AddRange(GetComponentsInChildren<UI_MiningLine>());
        int depth = 0;
        foreach (var line in _lines) {
            line.Depth = depth++;
            line.OnMiningLineCleared -= HandleMiningLineCleared;
            line.OnMiningLineCleared += HandleMiningLineCleared;
        }

        AddFloor();
        AddFloor();

        _lines[0].IsTopLine = true;
    }

    private void HandleMiningLineCleared(UI_MiningLine line)
    {
        //NOTE: 이 경우는 존재하지 않아야함
        if (line == _lines[^1]) {
            Debug.LogError($"@{line.gameObject.GetInstanceID()} UI_MiningLine is last line");
            return;
        }
        _lines[line.Depth + 1].IsTopLine = true;

        if (_lines[^1].IsTopLine) {
            AddFloor();
        }
    }

    [ContextMenu("Test_AddFloor")]
    public void AddFloor()
    {
        var floors = GetObject((int)GameObjects.Floors);
        //NOTE: MiningLine prefab 추가
        UI_MiningLine line = Managers.UI.MakeSubItem<UI_MiningLine>(floors.transform);
        line.transform.localScale = new Vector3(1, 1, 1);
        //NOTE: Hierarchy에서 Dark 위에 추가
        line.transform.SetSiblingIndex(_dark.transform.GetSiblingIndex());
        line.Depth = _lines.Count + 1;
        line.OnMiningLineCleared += HandleMiningLineCleared;
        _lines.Add(line);
    }
}