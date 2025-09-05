using System.Collections.Generic;
using UnityEngine;

public class Ore : UI_Base
{
    //NOTE: 한 층에 총 10개의 광석이 존재하고 그에 대한 타입
    public enum OreType
    {
        None = 0,
        T1,
        T2,
        T3,
        T4,
        T5,
    }

    public enum OreState
    {
        None = 0,
        Digging,
        Clear
    }

    [SerializeField] public OreType oreType;
    [SerializeField] public OreState oreState;

    public override void Init()
    {

    }
}

public class FloorData
{
    public const int MAX_ORE_COUNT = 10;

    //NOTE: 깊이가 깊어질수록 레어도가 높아질지 특정 높이에 따라서 나오는 자원을 결정할지 필요
    // Sky, Ground 고정
    // Dark 밝혀지지 않은 구간
    public enum FloorType
    {
        Sky,
        Ground,
        Dark,
        Normal,
        Uncommon,
        Rare,
        Epic
    }

    //NOTE: 각 층의 상태
    //     None : 아무것도 하지 않은 상태
    //     Digging : 현재 파고 있는 상태
    //          이 상태에서는 아래층을 미리 로드 해놓기?
    //     Clear : 해당 층을 모두 파헤친 상태
    public enum FloorState
    {
        None,
        Digging,
        Clear
    }

    public FloorType floorType;
    public FloorState floorState;
    public Ore[] Ores = new Ore[MAX_ORE_COUNT];

    public FloorData(FloorType type)
    {
        Init(type);
    }

    private void Init(FloorType type)
    {
        floorType = type;
        floorState = FloorState.None;
        for (int i = 0; i < MAX_ORE_COUNT; i++)
        {
            //NOTE: 층에 따라서 나오는 광석의 타입이 다를 수 있음
            //Ores[i] = new Ore(Ore.OreType.T1);
        }
    }

    //NOTE: 그냥 전부 정수값으로 하고 비트마스크로 관리하기?
    // [00][0]0 0000
    // FloorType(2) | FloorState(1) | 
}

public class MiningScene : BaseScene
{
    public List<FloorData> floors = new List<FloorData>();

    protected override void Init()
    {
        base.Init();
    }

    public override void Clear()
    {
    }
}