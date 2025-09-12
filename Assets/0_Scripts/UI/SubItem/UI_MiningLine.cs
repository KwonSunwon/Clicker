using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MiningLine : UI_Base
{
    private static int LineDepthIndex = 0;

    //NOTE: 첫 번째 라인은 0
    [SerializeField] private int _depth;
    public int Depth {
        get { return _depth; }
        set { _depth = value; }
    }

    //NOTE: 맨 위 라인인지 여부, 맨 위 라인만 클릭 가능
    public event Action OnTopLineChanged;

    [SerializeField] private bool _isTopLine = false;
    public bool IsTopLine {
        get { return _isTopLine; }
        set {
            _isTopLine = value;
            OnTopLineChanged?.Invoke();
        }
    }

    public event Action<UI_MiningLine> OnMiningLineCleared;

    private UI_MineRockButton[] _rocks;
    [SerializeField] private int _rockCount;
    public int RockCount {
        get { return _rockCount; }
        set {
            _rockCount = value;
            if (_rockCount <= 0)
                ClearLine();
        }
    }

    private void ClearLine()
    {
        foreach (var rock in _rocks) {
            Destroy(rock.gameObject);
        }
        OnMiningLineCleared?.Invoke(this);

        //NOTE: 더 이상 클리어가 호출될 일이 없으므로 이벤트 구독 해제
        OnMiningLineCleared = null;
    }

    public override void Init()
    {
        _depth = LineDepthIndex++;

        _rocks = GetComponentsInChildren<UI_MineRockButton>();
        _rockCount = _rocks.Length;
        foreach (var rock in _rocks) {
            OnTopLineChanged += rock.SetTopLine;
            rock.Line = this;
        }

        RandomVeinSeletor();
    }

    /// <summary>
    /// 라인 랜덤한 위치에 광맥을 생성
    /// 깊이 정보에 따라서 생성되는 광맥의 종류와 개수가 달라짐
    /// </summary>
    private void RandomVeinSeletor()
    {
        Debug.Log("Make Vein");

        List<int> veinPositions = new List<int>();
        int totalVeinCount = UnityEngine.Random.Range(2, 4);
        int index;
        for (int i = 0; i < totalVeinCount; i++) {
            do {
                index = UnityEngine.Random.Range(0, 14);
            } while (veinPositions.Contains(index));
            veinPositions.Add(index);

            //OreTypeSet.Dict.TryGetValue(Depth / 10, out var oreTypes);
            //int typeRandom = UnityEngine.Random.Range(0, oreTypes.Count);

            AddOreVein(OreBase.OreType.Coal, index, out UI_MineOreVeinButton vein);
            _rocks[index].OnMineRockBroken += vein.SetActiveByRock;
        }
    }

    private void AddOreVein(OreBase.OreType type, int index, out UI_MineOreVeinButton obj)
    {
        obj = null;
        obj = Managers.UI.MakeSubItem<UI_MineOreVeinButton>(transform);

        string className = type.ToString();
        var targetType = System.Type.GetType(className);
        obj.OreBase = (OreBase)obj.gameObject.AddComponent(targetType);
        obj.transform.SetAsFirstSibling();
    }

    #region Data
    [ContextMenu("세이브 테스트")]
    public MiningLineSaveData MakeSaveData()
    {
        MiningLineSaveData data = new MiningLineSaveData();
        UI_MineOreVeinButton[] veinButtons = GetComponentsInChildren<UI_MineOreVeinButton>();

        data.Id = Depth;
        data.VeinCount = veinButtons.Length;
        data.RockCount = _rockCount;
        data.IsCleared = (byte)((_rockCount == 0) ? 0 : 1);
        data.IsTop = Convert.ToByte(IsTopLine);
        for (int i = 0; i < 6; i++) {
            data.row[i] = (byte)(_rocks[i].Rock.IsBroken ? 0x80 : 0x00);
        }
        foreach (UI_MineOreVeinButton vein in veinButtons) {
            data.row[vein.PosIndex] |= (byte)((byte)vein.OreBase.Type & 0x7F);
        }

        return data;
    }

    public void LoadSaveData(MiningLineSaveData data)
    {
        Depth = data.Id;
        _rockCount = data.RockCount;
        IsTopLine = Convert.ToBoolean(data.IsTop);
        for (int i = 0; i < 6; i++) {
            byte b = data.row[i];
            _rocks[i].Rock.IsBroken = Convert.ToBoolean((b & 0x80) != 0);

            OreBase.OreType type = (OreBase.OreType)(b & 0x7F);
            //AddOreVein(type, i);
        }

    }
    #endregion
}
