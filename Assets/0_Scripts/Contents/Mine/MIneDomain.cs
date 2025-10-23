using System;
using System.Collections.Generic;
using UnityEngine;

using static Util;

#region State Classes
public class MineState
{
    public string Id;
    public int CurrentDepth;
    public List<LineState> Lines = new();
}

public class LineState
{
    public int Depth;
    public bool IsTopLine;
    public List<RockState> Rocks = null;
    public List<VeinState> Veins = new();
}

//NOTE: 게임 진행중에 ID는 10진법으로 저장되지만 16진법으로 해석해서 사용, 파일에 저장은 16진법 문자열로 저장됨
// 총 3자리 16진법으 로 이루어짐
// Rock: 최하위 한 자리 사용, 1부터 사용 => depth * 256 + index + 1
// Vein: Rock 다음 하위 한 자리 사용, 1부터 사용 => depth * 256 + (index + 1) * 16
// Depth: 하위 두 자리 이후 Depth가 기록됨
// eg) Depth = 15이고 10번째에 위치한 Rock ID = 15 * 256 + (10 + 1) = 3851      => 0xF0B
//     Depth = 5 이고 2 번째로 생성된 Vein ID =  5 * 256 + (2 + 1) * 16 = 1344  => 0x540
public class RockState
{
    public int Id;
    //public int Type;
    public int Hp;
    //public int MaxHp;
    public bool IsBroken => Hp <= 0;
}

public class VeinState
{
    public int Id;
    public int Pos;   // Rock Id
    public int Type;
}
#endregion

public interface IMineRules
{
    public IReadOnlyList<VeinState> PlanVeinToLine(LineState line, System.Random rng);

    public int RocksPerLine();
    public int RockHpForDepth(int depth);
}

public sealed class MineDomain
{
    // Events
    public event Action<int, int> OnRockDamaged;    // <rockId, remainingHp>
    public event Action<int> OnRockBroken;          // <rockId>
    public event Action<int> OnLineAdded;           // <newLineDepth>
    public event Action<int, int> OnVeinClicked;    // <veinId, oreType>
    public event Action<int> OnLineClear;           // <lineDepth>

    private readonly MineState _state;
    private readonly IMineRules _rules;
    private readonly System.Random _rng;

    public MineDomain(MineState state, IMineRules rules, int seed)
    {
        _state = state;
        _rules = rules;
        _rng = new(seed);
    }

    public void BreakIfHpZero()
    {
        foreach (var line in _state.Lines) {
            if (!line.IsTopLine) continue;
            if (line.Rocks == null) continue;
            foreach (var rock in line.Rocks) {
                if (rock.IsBroken) OnRockBroken?.Invoke(rock.Id);
            }
        }
    }

    //TODO: damage 부분은 나중에 player 데이터를 직접 받아서 IMineRules 를 통해 계산하도록 변경
    public void ClickRock(int rockId, int damage)
    {
        Debug.Log($"ClickRock {rockId} with damage {damage}");

        var (line, rock) = FindRock(rockId);
        if (!line.IsTopLine || rock == null || rock.IsBroken) return;

        rock.Hp -= damage;
        OnRockDamaged?.Invoke(rock.Id, Math.Max(rock.Hp, 0));

        if (rock.IsBroken) {
            OnRockBroken?.Invoke(rock.Id);
            TryExtendLineIfCleared(line);
        }
    }

    //TODO: damage 부분은 나중에 player 데이터를 직접 받아서 IMineRules 를 통해 계산하도록 변경
    public void ClickVein(int veinId, int damage)
    {
        Debug.Log($"ClickVein {veinId}");

        var (line, vein) = FindVein(veinId);
        if (vein == null) return;

        //TODO: vein 클릭 시 종류에 따른 자원 획득, 효과 발동 등 로직 처리
        var type = vein.Type;
        //IDEA: IVeinHandler 같은 인터페이스를 만들어서 종류별로 처리?

        OnVeinClicked?.Invoke(vein.Id, 1);
    }

    private void TryExtendLineIfCleared(LineState line)
    {
        if (line.Rocks.TrueForAll(r => r.IsBroken)) {
            Debug.Log($"Line {line.Depth} Cleared, Adding New Line");

            //NOTE: 이 두 라인이 실행되어야 State에 있는 Rocks도 메모리를 차지하지 않고 제거됨
            line.Rocks.Clear();
            line.Rocks = null;

            OnLineClear?.Invoke(line.Depth);

            //NOTE: 클리어된 다음 라인을 TopLine으로 설정해 클릭 가능하도록
            _state.Lines[line.Depth + 1].IsTopLine = true;

            //NOTE: 맨 아래에 새로운 라인 추가
            var newDepth = _state.Lines[^1].Depth + 1;
            _state.Lines.Add(MakeNewLine(newDepth));
            _state.CurrentDepth = newDepth;
            OnLineAdded?.Invoke(newDepth);
        }
    }

    private LineState MakeNewLine(int depth)
    {
        var newLine = new LineState { Depth = depth, IsTopLine = false };

        AddRockToLine(newLine);
        AddVeinToLine(newLine);

        return newLine;
    }

    private void AddRockToLine(LineState line)
    {
        line.Rocks = new();
        for (int i = 0; i < _rules.RocksPerLine(); i++) {
            var rock = new RockState {
                Id = MakeRockId(line.Depth, i),
                Hp = _rules.RockHpForDepth(line.Depth)
            };
            line.Rocks.Add(rock);
        }
    }

    private void AddVeinToLine(LineState line)
    {
        var veins = _rules.PlanVeinToLine(line, _rng);
        line.Veins.AddRange(veins);
    }

    (LineState, RockState) FindRock(int rockId)
    {
        foreach (var line in _state.Lines) {
            var r = line.Rocks?.Find(x => x.Id == rockId);
            if (r != null) return (line, r);
        }
        return (null, null);
    }

    (LineState, VeinState) FindVein(int veinId)
    {
        foreach (var line in _state.Lines) {
            var v = line.Veins?.Find(x => x.Id == veinId);
            if (v != null) return (line, v);
        }
        return (null, null);
    }
}
