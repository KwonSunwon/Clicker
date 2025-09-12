using TMPro;
using UnityEngine;

public class UI_Reincarnation_Explain_Panel : UI_Base
{
    enum Texts
    {
		Reincarnation_Name_Text,
		Reincarnation_Explain_Text
	}
	public override void Init()
	{
        

	}

	public void SetText(ReincarnationNodeData nodeData)
	{
		if (Get<TextMeshProUGUI>((int)Texts.Reincarnation_Explain_Text) == null) Bind<TextMeshProUGUI>(typeof(Texts));
		Get<TextMeshProUGUI>((int)Texts.Reincarnation_Explain_Text).text = nodeData.Description;
		Get<TextMeshProUGUI>((int)Texts.Reincarnation_Name_Text).text = nodeData.Name;

	}
}
