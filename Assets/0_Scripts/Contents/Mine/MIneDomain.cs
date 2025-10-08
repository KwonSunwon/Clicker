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
    public string Id;  // HEX (depth * 100) + 1 ~ E
    //public int Type;
    public int Hp;
    //public int MaxHp;
    public bool IsBroken => Hp <= 0;
}

public class VeinState
{
    public string Id;    // HEX (depth * 100) + (1 ~ E * 10)
    public string Pos;   // Rock Id
    public int Type;
}
#endregion

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

        //TODO: vein 클릭 시 종류에 따른 자원 획득, 효과 발동 등 로직 처리
        var type = vein.Type;
        //IDEA: IVeinHandler 같은 인터페이스를 만들어서 종류별로 처리?

        OnVeinClicked?.Invoke(GetDec(vein.Id), 1);
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
            var newLine = new LineState { Depth = newDepth, IsTopLine = false };

            //TODO: IMineRules 를 통해 라인 생성 규칙 따로 빼기
            int rockCount = 15;
            for (int i = 0; i < rockCount; i++) {
                var rock = new RockState {
                    Id = MakeHexRockId(newDepth, i),
                    Hp = 6
                };
                newLine.Rocks.Add(rock);
            }

            AddVeinToLine(newLine);

            _state.Lines.Add(newLine);
            _state.CurrentDepth = newDepth;
            OnLineAdded?.Invoke(newDepth);
        }
    }

    // TODO: IMineRules 로 옮기기
    private void AddVeinToLine(LineState line)
    {
        // 1. Vein 개 수 결정
        var VeinCount = _rng.Next(1, 3); // 임시로 1~2개 랜덤 -> 깊이에 따라 개수 변경, 특정 층 고정 규칙 추가

        for (int i = 0; i < VeinCount; i++) {
            var vein = new VeinState {
                Id = MakeHexVeinId(line.Depth, i)
            };

            // 2. 위치 결정
            var idx = _rng.Next(0, line.Rocks.Count);
            vein.Pos = line.Rocks[idx].Id; ;

            // 3. 종류 결정
            vein.Type = _rng.Next(0, (int)VeinType.MAX_NUM - 1);

            // 5. LineState.Veins 에 추가
            line.Veins.Add(vein);
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
}
