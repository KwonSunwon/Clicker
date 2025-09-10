using UnityEngine;

/// <summary>
/// 광맥의 Button 기능을 담당, 위치 조정이나 이펙트, 효과음 등 데이터 외적인 기능
/// </summary>
public class UI_MineOreVeinButton : UI_MineButtonBase
{
    [SerializeField] private OreBase _oreBase;
    public OreBase OreBase
    {
        get { return _oreBase; }
        set
        {
            if (_oreBase)
                Debug.LogError("OreBase is already set!");
            else
            {
                _oreBase = value;
                _oreBase.Init();
            }
        }
    }

    //NOTE: MiningLine에서 광맥의 위치 인덱스
    private int _posIndex;
    public int PosIndex
    {
        get { return _posIndex; }
        set { _posIndex = value; }
    }

    public override void Init()
    {

    }

    protected override void HandlePointerClick()
    {
        if (!OreBase)
            Debug.LogError($"@{gameObject.GetInstanceID()} OreBase Missing in UI_MineOreVeinButton!");
        OreBase?.OnClick();
    }
}
