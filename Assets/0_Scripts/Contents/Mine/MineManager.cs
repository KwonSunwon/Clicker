using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// MineData, MineUI 관리
/// Mine에 object를 추가하고 제거하는 모든 작업을 여기에 요청해서 처리
/// </summary>
public class MineManager : MonoBehaviour
{
    [SerializeField] private Transform lineContainer;
    [SerializeField] private Transform lineAddPosition;

    private MineState _state;
    private MineDomain _domain;

    // <Depth, View>
    private readonly Dictionary<int, LineView> _lines = new();

    private string SAVE_PATH;

    void Awake()
    {
        SAVE_PATH = Path.Combine(Application.persistentDataPath, "save_mine.json");

        #region CreateTempState 

        // TODO: 나중에 저장된 데이터 불러오기
        var dto = new MineSaveDTO {
            Id = "Player1_Mine",
            CurrentDepth = 0,
            Lines = new()
        };
        {
            var line = new LineSaveDTO {
                Depth = 0,
                IsTopLine = true,
                Rocks = null,
                //Rocks = new() {
                //new RockSaveDTO { Id = "001", Hp = 3 },
                //new RockSaveDTO { Id = "002", Hp = 0 },
                //new RockSaveDTO { Id = "003", Hp = 5 },
                //new RockSaveDTO { Id = "004", Hp = 5 },
                //new RockSaveDTO { Id = "005", Hp = 0 },
                //new RockSaveDTO { Id = "006", Hp = 4 },
                //new RockSaveDTO { Id = "007", Hp = 2 },
                //new RockSaveDTO { Id = "008", Hp = 3 },
                //new RockSaveDTO { Id = "009", Hp = 0 },
                //new RockSaveDTO { Id = "00A", Hp = 2 },
                //new RockSaveDTO { Id = "00B", Hp = 1 },
                //new RockSaveDTO { Id = "00C", Hp = 5 },
                //new RockSaveDTO { Id = "00D", Hp = 4 },
                //new RockSaveDTO { Id = "00E", Hp = 2 },
                //new RockSaveDTO { Id = "00F", Hp = 1 }
                //},
                Veins = new() {
                    new VeinSaveDTO { Id = "010", Pos = "005", Type = (int)VeinType.Bauxite },
                    new VeinSaveDTO { Id = "020", Pos = "00B", Type = (int)VeinType.Coal }
                }
            };
            dto.Lines.Add(line);
        }
        {
            var line = new LineSaveDTO {
                Depth = 1,
                IsTopLine = true,
                Rocks = new() {
                new RockSaveDTO { Id = "101", Hp = 6 },
                new RockSaveDTO { Id = "102", Hp = 6 },
                new RockSaveDTO { Id = "103", Hp = 6 },
                new RockSaveDTO { Id = "104", Hp = 6 },
                new RockSaveDTO { Id = "105", Hp = 6 },
                new RockSaveDTO { Id = "106", Hp = 6 },
                new RockSaveDTO { Id = "107", Hp = 6 },
                new RockSaveDTO { Id = "108", Hp = 6 },
                new RockSaveDTO { Id = "109", Hp = 6 },
                new RockSaveDTO { Id = "10A", Hp = 6 },
                new RockSaveDTO { Id = "10B", Hp = 6 },
                new RockSaveDTO { Id = "10C", Hp = 6 },
                new RockSaveDTO { Id = "10D", Hp = 6 },
                new RockSaveDTO { Id = "10E", Hp = 6 },
                new RockSaveDTO { Id = "10F", Hp = 6 }
                },
                Veins = new() {
                    new VeinSaveDTO { Id = "110", Pos = "10A", Type = (int)VeinType.CopperOre },
                    new VeinSaveDTO { Id = "120", Pos = "102", Type = (int)VeinType.Diamond }
                }
            };
            dto.Lines.Add(line);
        }
        {
            var line = new LineSaveDTO {
                Depth = 2,
                IsTopLine = false,
                Rocks = new() {
                new RockSaveDTO { Id = "201", Hp = 6 },
                new RockSaveDTO { Id = "202", Hp = 6 },
                new RockSaveDTO { Id = "203", Hp = 6 },
                new RockSaveDTO { Id = "204", Hp = 6 },
                new RockSaveDTO { Id = "205", Hp = 6 },
                new RockSaveDTO { Id = "206", Hp = 6 },
                new RockSaveDTO { Id = "207", Hp = 6 },
                new RockSaveDTO { Id = "208", Hp = 6 },
                new RockSaveDTO { Id = "209", Hp = 6 },
                new RockSaveDTO { Id = "20A", Hp = 6 },
                new RockSaveDTO { Id = "20B", Hp = 6 },
                new RockSaveDTO { Id = "20C", Hp = 6 },
                new RockSaveDTO { Id = "20D", Hp = 6 },
                new RockSaveDTO { Id = "20E", Hp = 6 },
                new RockSaveDTO { Id = "20F", Hp = 6 }
                },
                Veins = new() {
                    new VeinSaveDTO { Id = "210", Pos = "20C", Type = (int)VeinType.IronOre },
                    new VeinSaveDTO { Id = "220", Pos = "204", Type = (int)VeinType.Coal }
                }
            };
            dto.Lines.Add(line);
        }
        #endregion

        //_domain = new MineDomain(_state, new DefaultMineRules(), seed: 12345);
        _state = new();
        MineMapper.FromDTO(dto, _state);
        _domain = new MineDomain(_state, new DefaultMineRules(), 12345);

        _domain.OnRockDamaged += HandleRockDamaged;
        _domain.OnRockBroken += HandleRockBroken;
        _domain.OnLineAdded += HandleLineAdded;
        _domain.OnVeinClicked += HandleVeinDamaged;
        _domain.OnLineClear += HandleLineClear;

        ReBuildAll();
        _domain.BreakIfHpZero();
    }

    void OnRockClicked(int rockId)
    {
        Debug.Log($"Rock Clicked: {rockId}");

        _domain.ClickRock(rockId, damage: 2);
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

        var vein = line.Veins.Find(x => x.Pos == rockId);
        if (vein == null) return;
        var veinView = SpawnVeinView(vein);
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

    private void HandleLineClear(int lineDepth)
    {
        _lines[lineDepth].RemoveRock();
    }
    #endregion

    /// <summary>
    /// MineState 데이터를 기반으로 모든 UI를 재구성
    /// </summary>
    void ReBuildAll()
    {
        //NOTE: Canvas 강제 갱신
        Canvas.ForceUpdateCanvases();
        //NOTE: 기존 UI 제거
        foreach (Transform child in lineContainer) {
            if (child.GetComponent<LineView>() != null)
                Destroy(child.gameObject);
        }

        foreach (var line in _state.Lines) {
            AddLineView(line);
        }
        //NOTE: Canvas 강제 갱신
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

    VeinView SpawnVeinView(VeinState vein)
    {
        var veinView = Managers.Resource.Instantiate("UI/SubItem/VeinView").GetOrAddComponent<VeinView>();
        veinView.Bind(vein, OnVeinClick);
        return veinView;
    }
    #endregion

    #region Save/Load
    [ContextMenu("Save")]
    public void Save()
    {
        var dto = MineMapper.ToDTO(_state);
        var json = JsonUtility.ToJson(dto);
        File.WriteAllText(SAVE_PATH, json);
    }

    [ContextMenu("Load")]
    public MineState Load()
    {
        var json = File.ReadAllText(SAVE_PATH);
        var dto = JsonUtility.FromJson<MineSaveDTO>(json);
        MineMapper.FromDTO(dto, _state);

        ReBuildAll();
        _domain.BreakIfHpZero();

        return _state;
    }
    #endregion

    #region Utility
    /// <summary>
    /// rockId로 RockState와 해당 Rock이 속한 LineState를 찾음
    /// </summary>
    RockState FindRockState(int rockId, out LineState lineOut)
    {
        foreach (var line in _state.Lines) {
            var r = line.Rocks?.Find(x => x.Id == rockId);
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
            var v = line.Veins?.Find(x => x.Id == veinId);
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
