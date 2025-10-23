using UnityEngine;

public class Iron : OreBase
{
    public override void Init()
    {
        base.Init();
        oreType = OreType.Iron;
        _spriteSet = Managers.Resource.Load<OreSpriteSet>("SpriteSets/Iron").sprites;
        _image.sprite = _spriteSet[_spriteIndex];
    }

    public override void OnClick()
    {
        Debug.Log($"@Iron {gameObject.GetInstanceID()} Clicked, HP: {_hp}");
    }
}
