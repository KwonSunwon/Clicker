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
    public List<RockState> Rocks = new();
    public List<VeinState> Veins = new();
}

public class RockState
{
    public int Id;  // HEX (depth * 100) + 1 ~ E
    //public int Type;
    public int Hp;
    //public int MaxHp;
    public bool IsBroken => Hp <= 0;
}

public class VeinState
{
    public int Id;    // HEX (depth * 100) + (1 ~ E * 10)
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

    readonly MineState _state;
    readonly IMineRules _rules;
    readonly System.Random _rng;

    public MineDomain(MineState state, IMineRules ruls, int seed)
    {
        _state = state;
        _rules = ruls;
        _rng = new(seed);
    }

    public void BreakIfHpZero()
    {
        foreach (var line in _state.Lines) {
            if (!line.IsTopLine) continue;
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
            var r = line.Rocks.Find(x => x.Id == rockId);
            if (r != null) return (line, r);
        }
        return (null, null);
    }

    (LineState, VeinState) FindVein(int veinId)
    {
        foreach (var line in _state.Lines) {
            var v = line.Veins.Find(x => x.Id == veinId);
            if (v != null) return (line, v);
        }
        return (null, null);
    }
}
