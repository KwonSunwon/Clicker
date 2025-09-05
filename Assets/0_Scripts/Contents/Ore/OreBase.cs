using UnityEngine;
using UnityEngine.UI;

public abstract class OreBase : MonoBehaviour
{
    public enum OreType
    {
        Coal,
        Iron,
        Gold,
        Diamond
    }

    protected Sprite[] _spriteSet;
    protected int _spriteIndex;
    protected Image _image;
    protected int _hp = 10;
    protected OreType oreType;

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
