using UnityEngine;

public class Coal : OreBase
{
    public override void Init()
    {
        base.Init();
        oreType = OreType.Coal;
        _spriteSet = Managers.Resource.Load<OreSpriteSet>("SpriteSets/Coal").sprites;
        _image.sprite = _spriteSet[_spriteIndex];
    }

    public override void OnClick()
    {
        _hp--;
        Debug.Log($"@Coal {gameObject.GetInstanceID()} Clicked, HP: {_hp}");

        _spriteIndex = (++_spriteIndex) % _spriteSet.Length;
        _image.sprite = _spriteSet[_spriteIndex];
    }
}
