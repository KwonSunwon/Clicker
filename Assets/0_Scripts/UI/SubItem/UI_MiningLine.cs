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
    public event Action<bool> OnTopLineChanged;

    [SerializeField] private bool _isTopLine = false;
    public bool IsTopLine {
        get { return _isTopLine; }
        set {
            _isTopLine = value;
            OnTopLineChanged?.Invoke(true);
        }
    }

    public event Action<UI_MiningLine> OnMiningLineCleared;

    public UI_MineRockButton[] _rocks;
    [SerializeField] private int _rockCount;
    public int RockCount {
        get { return _rockCount; }
    }

    public List<UI_MineOreVeinButton> _oreVeins;

    private void ClearLine()
    {
        foreach (var rock in _rocks) {
            Destroy(rock.gameObject);
        }
        OnMiningLineCleared?.Invoke(this);

        //NOTE: 더 이상 클리어가 호출될 일이 없으므로 이벤트 구독 해제
        OnMiningLineCleared = null;
    }

    private void RockBroken(UI_MineRockButton rock)
    {
        _rockCount--;
        if (_rockCount <= 0)
            ClearLine();
    }

    public void Awake()
    {
        _depth = LineDepthIndex++;
        _rocks = GetComponentsInChildren<UI_MineRockButton>();
        _rockCount = _rocks.Length;
        foreach (var rock in _rocks) {
            OnTopLineChanged += rock.SetTopLine;
            rock.OnMineRockBroken += RockBroken;
        }

        _oreVeins = new List<UI_MineOreVeinButton>();
    }

    public override void Init()
    {
        //_depth = LineDepthIndex++;

        //RandomVeinSeletor();
    }

    /// <summary>
    /// 라인 랜덤한 위치에 광맥을 생성
    /// 깊이 정보에 따라서 생성되는 광맥의 종류와 개수가 달라짐
    /// </summary>
    public void RandomVeinSeletor()
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

            AddOreVein(OreBase.OreType.Coal, index, out var vein);
            //_rocks[index].OnMineRockBroken += vein.SetActiveByRock;
            //_oreVeins.Add(vein);
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
        _rocks[index].OnMineRockBroken += obj.SetActiveByRock;
        _oreVeins.Add(obj);
    }

    #region Data
    public void Load(LineData ld)
    {
        Canvas.ForceUpdateCanvases();

        Depth = ld.LineId;
        IsTopLine = ld.IsTopLine;

        foreach (var vein in ld.Veins) {
            AddOreVein((OreBase.OreType)vein.Type, vein.Index, out UI_MineOreVeinButton veinObj);
        }

        if (!ld.IsCleared) {
            for (int i = 0; i < ld.Rocks.Count; i++) {
                _rocks[i].Rock._hp = ld.Rocks[i].Hp;

                _rocks[i].SetTopLine(IsTopLine);

                if (IsTopLine)
                    if (ld.Rocks[i].IsBroken)
                        _rocks[i].Break();
            }
        }
        else {
            _rockCount = 0;
            foreach (var r in _rocks)
                r.Break();
            //OnMiningLineCleared = null;
            ClearLine();
        }
    }
    //FIXME: Vein 활성화 안됨, Rock BoxCollider 활성화 안됨
    // 일단 SetActiveByRock 이 호출은 됨, 그 후에 Init 이 Start 에서 호출되면서 다시 비활성화됨
    // -> Awake 에서 Init 작업을 하도록 변경해서 해결
    // vein 에서 SetActiveByRock 을 진행할 때 rock 의 anchoredPosition 도 0, 0 으로 나옴
    // 위치가 이상하게 잡히는 문제 해결해야됨
    // -> Rock들이 Awake까지는 호출되는데 anchoredPosition 이 정상적으로 잡히지 않음
    // => 아,씹, 진짜 Canvas.ForceUpdateCanvases()로 강제로 갱신하거나, 한 프레임 뒤에 처리해야
    //   RectTransform 정보가 제대로 갱신되어 있음
    // -> 다른 버그 맨 위 아니어도 부셔지고, 맨 아래를 다 부셨을 때 아래 라인이 생기지 않음
    // 맨 위가 아닌 다른 라인 부셔지는 문제는 수정
    // 아래 라인이 생기지 않는 문제 해결해야됨 + 맨 위 라인 다 부셔도 다른 활성화 안됨
    // 해결 완

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
