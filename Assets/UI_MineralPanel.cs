using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MineralPanel : UI_Base
{
    enum Texts
    {
		MineralAmountText
	}
    enum Images
    {
		MineralImage
	}
	public override void Init()
	{
	}

	void Start()
    {
        
    }

    void Update()
    {
        
    }

    public TextMeshProUGUI GetMineralText()
    {
        if (GetTMP((int)Texts.MineralAmountText) == null) { 
			Bind<TextMeshProUGUI>(typeof(Texts));
		}
        return GetTMP((int)Texts.MineralAmountText);
    }
	public void SetMineralImage(Sprite sprite)
	{
		if (GetImage((int)Images.MineralImage) == null)
		{
			Bind<Image>(typeof(Images));
		}
		GetImage((int)Images.MineralImage).sprite = sprite;
	}
}
