using System;
using System.Collections.Generic;
using static Util;

public class DefaultMineRules : IMineRules
{
    public IReadOnlyList<VeinState> PlanVeinToLine(LineState line, Random rng)
    {
        // 1. 개수 결정 
        var veinCount = rng.Next(2, 5); //TODO: 임시로 1~2개 랜덤 -> 깊이에 따라 개수 변경, 특정 층 고정 규칙 추가
        List<VeinState> veins = new(veinCount);
        var occupied = new HashSet<int>();

        for (int i = 0; i < veinCount; i++) {
            var vein = new VeinState {
                Id = MakeVeinId(line.Depth, i)
            };

            // 2. 위치 결정
            do {
                var idx = rng.Next(0, line.Rocks.Count);
                vein.Pos = line.Rocks[idx].Id;
            } while (!occupied.Add(vein.Pos));

            // 3. 종류 결정
            vein.Type = rng.Next(0, (int)VeinType.MAX_NUM - 1);

            veins.Add(vein);
        }

        return veins;
    }

    public int RocksPerLine() => 15;

    public int RockHpForDepth(int depth)
    {
        //TODO: 임시로 고정 -> 깊이에 따라 증가하도록 변경
        return 6;
    }
}
