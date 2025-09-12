using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 광맥의 실질적인 데이터 관리
/// 종류나 자원 획득 등
/// </summary>
public abstract class OreBase : MonoBehaviour
{
    public enum OreType
    {
        Coal = 1,
        Iron = 2,
        Gold,
        Diamond
    }

    protected Sprite[] _spriteSet;
    protected int _spriteIndex;
    protected Image _image;
    protected int _hp = 10;
    protected OreType oreType;
    public OreType Type { get { return oreType; } }

    [SerializeField]
    private OreSpriteSet SpriteSet;

    public virtual void Init()
    {
        _image = GetComponent<Image>();
        _hp = 10;
        //_spriteSet = SpriteSet.sprites;
        _spriteIndex = 0;

        //_image.sprite = _spriteSet[_spriteIndex];
    }

    public abstract void OnClick();
}

public static class OreTypeSet
{
    static Dictionary<int, List<OreBase.OreType>> dict = new Dictionary<int, List<OreBase.OreType>> {
        {0, d1}, {1, d2}, {2, d3}
    };

    static OreBase.OreType Coal = OreBase.OreType.Coal;
    static OreBase.OreType Iron = OreBase.OreType.Iron;
    static OreBase.OreType Gold = OreBase.OreType.Gold;
    static OreBase.OreType Diamond = OreBase.OreType.Diamond;

    static List<OreBase.OreType> d1 = new List<OreBase.OreType> { Coal, Iron };
    static List<OreBase.OreType> d2 = new List<OreBase.OreType> { Iron, Gold };
    static List<OreBase.OreType> d3 = new List<OreBase.OreType> { Gold, Diamond };

    static public Dictionary<int, List<OreBase.OreType>> Dict { get { return dict; } }
}