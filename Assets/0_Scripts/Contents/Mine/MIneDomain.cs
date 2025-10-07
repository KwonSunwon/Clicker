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
    //public List<VeinState> Veins = new();
}

[Serializable]
public class RockState
{
    public int Id;
    //public int Type;
    public int Hp;
    //public int MaxHp;
    public bool IsBroken => Hp <= 0;
}

[Serializable]
public class VeinState
{
    public int Id;
    public int Type;
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

    readonly MineState _state;
    readonly IMineRules _rules;
    readonly System.Random _rng;

    public MineDomain(MineState state, IMineRules ruls, int seed)
    {
        _state = state;
        _rules = ruls;
        _rng = new(seed);
    }

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
            var r = line.Rocks.Find(x => x.Id == rockId);
            if (r != null) return (line, r);
        }
        return (null, null);
    }

    int GetNextRockId(int depth, int index) => depth * 100 + index;
}
