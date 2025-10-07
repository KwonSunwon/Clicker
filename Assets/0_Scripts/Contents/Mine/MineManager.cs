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
                Rocks = new()
                {
                new RockState { Id = 1, Hp = 3 },
                new RockState { Id = 2, Hp = 2 },
                new RockState { Id = 3, Hp = 5 },
                new RockState { Id = 4, Hp = 5 },
                new RockState { Id = 5, Hp = 1  },
                new RockState { Id = 6, Hp = 4  },
                new RockState { Id = 7, Hp = 2  },
                new RockState { Id = 8, Hp = 3  },
                new RockState { Id = 9, Hp = 4  },
                new RockState { Id = 10, Hp = 2 },
                new RockState { Id = 11, Hp = 3 },
                new RockState { Id = 12, Hp = 5 },
                new RockState { Id = 13, Hp = 4 },
                new RockState { Id = 14, Hp = 2 },
                new RockState { Id = 15, Hp = 1 }
            }
            };
            _state.Lines.Add(line);
        }
        {
            var line = new LineState {
                Depth = 1,
                IsTopLine = false,
                Rocks = new()
                {
                new RockState { Id = 101, Hp = 6 },
                new RockState { Id = 102, Hp = 6 },
                new RockState { Id = 103, Hp = 6 },
                new RockState { Id = 104, Hp = 6 },
                new RockState { Id = 105, Hp = 6  },
                new RockState { Id = 106, Hp = 6  },
                new RockState { Id = 107, Hp = 6  },
                new RockState { Id = 108, Hp = 6  },
                new RockState { Id = 109, Hp = 6  },
                new RockState { Id = 110, Hp = 6 },
                new RockState { Id = 111, Hp = 6 },
                new RockState { Id = 112, Hp = 6 },
                new RockState { Id = 113, Hp = 6 },
                new RockState { Id = 114, Hp = 6 },
                new RockState { Id = 115, Hp = 6 }
            }
            };
            _state.Lines.Add(line);
        }

        _domain = new MineDomain(_state, new DefaultMineRules(), seed: 12345);

        _domain.OnRockDamaged += HandleRockDamaged;
        _domain.OnRockBroken += HandleRockBroken;
        _domain.OnLineAdded += HandleLineAdded;

        ReBuildAll();
    }

    private void OnDestroy()
    {
        _domain.OnRockDamaged -= HandleRockDamaged;
        _domain.OnRockBroken -= HandleRockBroken;
        _domain.OnLineAdded -= HandleLineAdded;
    }

    void OnRockClicked(int rockId)
    {
        Debug.Log($"Rock Clicked: {rockId}");

        _domain.ClickRock(rockId, damage: 1);
    }

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

        if (_lines.TryGetValue(line.Depth, out var lineView) &&
            lineView.TryGetRockView(rockId, out var rockView)) {
            rockView.PlayBreak();
        }
    }

    private void HandleLineAdded(int lineDepth)
    {
        Debug.Log($"Line {lineDepth} Added");

        var line = _state.Lines.Find(l => l.Depth == lineDepth);
        if (line == null) return;

        AddLineView(line);
    }

    void ReBuildAll()
    {
        foreach (var line in _state.Lines) {
            AddLineView(line);
        }
    }

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
        lineView.BuildFrom(line, SpawnRockView);
        return lineView;
    }

    RockView SpawnRockView(RockState rock)
    {
        var rockView = Managers.Resource.Instantiate("UI/SubItem/UI_MineRockButton").GetOrAddComponent<RockView>();
        rockView.Bind(rock, OnRockClicked);
        return rockView;
    }

    RockState FindRockState(int rockId, out LineState lineOut)
    {
        foreach (var line in _state.Lines) {
            var r = line.Rocks.Find(x => x.Id == rockId);
            if (r != null) {
                lineOut = line;
                return r;
            }
        }
        lineOut = null;
        return null;
    }
}
