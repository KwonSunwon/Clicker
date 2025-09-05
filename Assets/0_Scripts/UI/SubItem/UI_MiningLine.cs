using System;
using UnityEngine.UI;

public class UI_MiningLine : UI_Base
{
    Image _backGround;

    public override void Init()
    {
        // 이미지 컴포넌트 가져와서 깊이에 따라 배경 이미지 변경
        _backGround = GetComponent<Image>();
        MakeRandomOre();
    }

    private void MakeRandomOre()
    {
        Random random = new Random();
        var Obejcts = GetComponentsInChildren<UI_MineButton>();
        foreach (var ore in Obejcts)
        {
            //int r = random.Next(0, 4);
            int r = 1;
            switch (r)
            {
                case 1:
                    ore.gameObject.AddComponent<Coal>();
                    ore.Init();
                    break;
            }
        }
    }
}
