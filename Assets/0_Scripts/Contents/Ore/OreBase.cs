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
