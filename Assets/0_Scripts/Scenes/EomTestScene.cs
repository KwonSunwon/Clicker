using UnityEngine;

public class EomTestScene : BaseScene
{
	protected override void Init()
	{
		base.Init();
		GoldManager goldManager = Managers.Gold;
	}

	public override void Clear()
	{
		throw new System.NotImplementedException();
	}
}
