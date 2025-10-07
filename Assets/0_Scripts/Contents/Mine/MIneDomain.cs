using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MineState
{
    public string Id;
    public int CurrentDepth;
    public List<LineState> Lines = new();
}

[Serializable]
public class LineState
{
    public int Depth;
    public bool IsTopLine;
    public List<RockState> Rocks = new();
    public List<VeinState> Veins = new();
}

[Serializable]
public class RockState
{
    public string Id;  // HEX (depth * 100) + 1 ~ E
    //public int Type;
    public int Hp;
    //public int MaxHp;
    public bool IsBroken => Hp <= 0;
}

[Serializable]
public class VeinState
{
    public string Id;    // HEX (depth * 100) + (1 ~ E * 10)
    public string Pos;   // Rock Id
    //public int Type;
}

public interface IMineRules
{

}

public sealed class MineDomain
{
    // Events
    public event Action<int, int> OnRockDamaged;    // <rockId, remainingHp>
    public event Action<int> OnRockBroken;          // <rockId>
    public event Action<int> OnLineAdded;           // <newLineDepth>
    public event Action<int, int> OnVeinClicked;    // <veinId, oreType> 

    readonly MineState _state;
    readonly IMineRules _rules;
    readonly System.Random _rng;

    public MineDomain(MineState state, IMineRules ruls, int seed)
    {
        _state = state;
        _rules = ruls;
        _rng = new(seed);
    }

    public void CheckAllRockBroken()
    {
        foreach (var line in _state.Lines) {
            if (!line.IsTopLine) continue;
            foreach (var rock in line.Rocks) {
                if (rock.IsBroken) OnRockBroken?.Invoke(GetDec(rock.Id));
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
        OnRockDamaged?.Invoke(GetDec(rock.Id), Math.Max(rock.Hp, 0));

        if (rock.IsBroken) {
            OnRockBroken?.Invoke(GetDec(rock.Id));
            TryExtendLineIfCleared(line);
        }
    }

    //TODO: damage 부분은 나중에 player 데이터를 직접 받아서 IMineRules 를 통해 계산하도록 변경
    public void ClickVein(int veinId, int damage)
    {
        Debug.Log($"ClickVein {veinId}");

        var (line, vein) = FindVein(veinId);
        if (vein == null) return;

        OnVeinClicked?.Invoke(GetDec(vein.Id), 1);
    }

    void TryExtendLineIfCleared(LineState line)
    {
        if (line.Rocks.TrueForAll(r => r.IsBroken)) {
            Debug.Log($"Line {line.Depth} Cleared, Adding New Line");

            //NOTE: 클리어된 다음 라인을 TopLine으로 설정해 클릭 가능하도록
            _state.Lines[line.Depth + 1].IsTopLine = true;

            //NOTE: 맨 아래에 새로운 라인 추가
            var newDepth = _state.Lines[^1].Depth + 1;
            var newLine = new LineState { Depth = newDepth, IsTopLine = false };

            int rockCount = 15;
            for (int i = 0; i < rockCount; i++) {
                var rock = new RockState {
                    Id = GetNextRockId(newDepth, i),
                    Hp = 6
                };
                newLine.Rocks.Add(rock);
            }

            _state.Lines.Add(newLine);
            _state.CurrentDepth = newDepth;
            OnLineAdded?.Invoke(newDepth);
        }
    }

    (LineState, RockState) FindRock(int rockId)
    {
        foreach (var line in _state.Lines) {
            var r = line.Rocks.Find(x => GetDec(x.Id) == rockId);
            if (r != null) return (line, r);
        }
        return (null, null);
    }

    (LineState, VeinState) FindVein(int veinId)
    {
        foreach (var line in _state.Lines) {
            var v = line.Veins.Find(x => GetDec(x.Id) == veinId);
            if (v != null) return (line, v);
        }
        return (null, null);
    }

    string GetNextRockId(int depth, int index) => GetHex(depth * 100 + index);

    static public int GetDec(string id) => Convert.ToInt32(id, 16);
    static public string GetHex(int id) => id.ToString("X");
}
