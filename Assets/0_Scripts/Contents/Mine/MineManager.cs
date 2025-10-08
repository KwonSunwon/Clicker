using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MineData, MineUI 관리
/// Mine에 object를 추가하고 제거하는 모든 작업을 여기에 요청해서 처리
/// </summary>
public class MineManager : MonoBehaviour
{
    [SerializeField] Transform lineContainer;
    [SerializeField] Transform lineAddPosition;

    MineState _state;
    MineDomain _domain;

    readonly Dictionary<int, LineView> _lines = new();

    void Awake()
    {
        #region CreateTempState
        // TODO: 나중에 저장된 데이터 불러오기
        _state = new MineState {
            Id = "Player1_Mine",
            CurrentDepth = 0,
            Lines = new()
        };
        {
            var line = new LineState {
                Depth = 0,
                IsTopLine = true,
                Rocks = new() {
                new RockState { Id = "001", Hp = 3 },
                new RockState { Id = "002", Hp = 0 },
                new RockState { Id = "003", Hp = 5 },
                new RockState { Id = "004", Hp = 5 },
                new RockState { Id = "005", Hp = 0 },
                new RockState { Id = "006", Hp = 4 },
                new RockState { Id = "007", Hp = 2 },
                new RockState { Id = "008", Hp = 3 },
                new RockState { Id = "009", Hp = 0 },
                new RockState { Id = "00A", Hp = 2 },
                new RockState { Id = "00B", Hp = 1 },
                new RockState { Id = "00C", Hp = 5 },
                new RockState { Id = "00D", Hp = 4 },
                new RockState { Id = "00E", Hp = 2 },
                new RockState { Id = "00F", Hp = 1 }
                },
                Veins = new() {
                    new VeinState { Id = "010", Pos = "005", Type = (int)VeinType.Bauxiet },
                    new VeinState { Id = "020", Pos = "00B", Type = (int)VeinType.Coal }
                }
            };
            _state.Lines.Add(line);
        }
        {
            var line = new LineState {
                Depth = 1,
                IsTopLine = false,
                Rocks = new() {
                new RockState { Id = "101", Hp = 6 },
                new RockState { Id = "102", Hp = 6 },
                new RockState { Id = "103", Hp = 6 },
                new RockState { Id = "104", Hp = 6 },
                new RockState { Id = "105", Hp = 6 },
                new RockState { Id = "106", Hp = 6 },
                new RockState { Id = "107", Hp = 6 },
                new RockState { Id = "108", Hp = 6 },
                new RockState { Id = "109", Hp = 6 },
                new RockState { Id = "10A", Hp = 6 },
                new RockState { Id = "10B", Hp = 6 },
                new RockState { Id = "10C", Hp = 6 },
                new RockState { Id = "10D", Hp = 6 },
                new RockState { Id = "10E", Hp = 6 },
                new RockState { Id = "10F", Hp = 6 }
                },
                Veins = new() {
                    new VeinState { Id = "110", Pos = "10A", Type = (int)VeinType.Copper },
                    new VeinState { Id = "120", Pos = "102", Type = (int)VeinType.Diamond }
                }
            };
            _state.Lines.Add(line);
        }
        #endregion

        _domain = new MineDomain(_state, new DefaultMineRules(), seed: 12345);

        _domain.OnRockDamaged += HandleRockDamaged;
        _domain.OnRockBroken += HandleRockBroken;
        _domain.OnLineAdded += HandleLineAdded;
        _domain.OnVeinClicked += HandleVeinDamaged;

        ReBuildAll();

        _domain.BreakIfHpZero();
    }

    void OnRockClicked(int rockId)
    {
        Debug.Log($"Rock Clicked: {rockId}");

        _domain.ClickRock(rockId, damage: 6);
    }

    void OnVeinClick(int veinId)
    {
        Debug.Log($"Vein Clicked: {veinId}");

        _domain.ClickVein(veinId, damage: 1);
    }

    #region EventHandlers
    private void HandleRockDamaged(int rockId, int remainingHp)
    {
        Debug.Log($"Rock {rockId} Damaged, Remaining HP: {remainingHp}");

        var rock = FindRockState(rockId, out var line);
        if (rock == null || line == null) return;

        if (_lines.TryGetValue(line.Depth, out var lineView))
            if (lineView.TryGetRockView(rockId, out var rockView)) {
                rock.Hp = remainingHp;
                rockView.Refresh(rock);
                rockView.PlayDoTween();
            }
    }

    private void HandleRockBroken(int rockId)
    {
        Debug.Log($"Rock {rockId} Broken");

        var rock = FindRockState(rockId, out var line);
        if (rock == null || line == null) return;

        RockView rockView = null;
        LineView lineView = null;
        if (_lines.TryGetValue(line.Depth, out lineView) &&
            lineView.TryGetRockView(rockId, out rockView)) {
            rockView.PlayBreak();
        }

        //TODO: VeinView 활성화
        var vein = line.Veins.Find(x => MineDomain.GetDec(x.Pos) == rockId);
        if (vein == null) return;
        var veinView = SpawnVeinView(vein, rockView);
        lineView.AddVeinView(veinView);
    }

    private void HandleLineAdded(int lineDepth)
    {
        Debug.Log($"Line {lineDepth} Added");

        var line = _state.Lines.Find(l => l.Depth == lineDepth);
        if (line == null) return;

        AddLineView(line);
    }

    private void HandleVeinDamaged(int veinId, int damage)
    {
        Debug.Log($"Vein {veinId} Clicked, Damage: {damage}");

        var vein = FindVeinState(veinId, out var line);
        if (vein == null || line == null) return;

        if (_lines.TryGetValue(line.Depth, out var lineView) &&
            lineView.TryGetVeinView(veinId, out var veinView)) {
            veinView.PlayDoTween();
            //TODO: 자원 획득 처리
        }
    }
    #endregion

    /// <summary>
    /// MineState 데이터를 기반으로 모든 UI를 재구성
    /// </summary>
    void ReBuildAll()
    {
        foreach (var line in _state.Lines) {
            AddLineView(line);
        }
        //NOTE: UI 업데이트 강제
        Canvas.ForceUpdateCanvases();
    }

    #region View Spawning
    void AddLineView(LineState line)
    {
        var lineView = SpawnLineView(line);
        lineView.transform.SetParent(lineContainer, false);
        lineView.transform.SetSiblingIndex(lineAddPosition.GetSiblingIndex());
        _lines[line.Depth] = lineView;
    }

    LineView SpawnLineView(LineState line)
    {
        var lineView = Managers.Resource.Instantiate("UI/SubItem/LineView").GetOrAddComponent<LineView>();
        lineView.BuildFrom(line, SpawnRockView, SpawnVeinView);
        return lineView;
    }

    RockView SpawnRockView(RockState rock)
    {
        var rockView = Managers.Resource.Instantiate("UI/SubItem/RockView").GetOrAddComponent<RockView>();
        rockView.Bind(rock, OnRockClicked);
        return rockView;
    }

    VeinView SpawnVeinView(VeinState vein, RockView rock)
    {
        var veinView = Managers.Resource.Instantiate("UI/SubItem/VeinView").GetOrAddComponent<VeinView>();
        veinView.Bind(vein, rock, OnVeinClick);
        return veinView;
    }
    #endregion

    #region Save/Load
    public void Save()
    {

    }

    public void Load()
    {

    }
    #endregion

    #region Utility
    /// <summary>
    /// rockId로 RockState와 해당 Rock이 속한 LineState를 찾음
    /// </summary>
    RockState FindRockState(int rockId, out LineState lineOut)
    {
        foreach (var line in _state.Lines) {
            var r = line.Rocks.Find(x => MineDomain.GetDec(x.Id) == rockId);
            if (r != null) {
                lineOut = line;
                return r;
            }
        }
        lineOut = null;
        return null;
    }

    VeinState FindVeinState(int veinId, out LineState lineOut)
    {
        foreach (var line in _state.Lines) {
            var v = line.Veins.Find(x => MineDomain.GetDec(x.Id) == veinId);
            if (v != null) {
                lineOut = line;
                return v;
            }
        }
        lineOut = null;
        return null;
    }
    #endregion
}
