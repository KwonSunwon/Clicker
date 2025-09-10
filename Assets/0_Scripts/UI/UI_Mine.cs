using UnityEngine;

public class UI_Mine : UI_Base
{
    enum GameObjects
    {
        Mine,
        Viewport,
        Floors
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        for (int i = 0; i < 3; i++)
        {
            //AddMiningLine();
        }
    }

    public void AddMiningLine()
    {
        var floors = Get<GameObject>((int)GameObjects.Floors);
        // Add a new mining line prefab
        UI_MiningLine line = Managers.UI.MakeSubItem<UI_MiningLine>(floors.transform);
    }

    [ContextMenu("Test_AddMiningLine")]
    public void Test_AddMiningLine()
    {
        Debug.Log("Test_AddMiningLine");
        AddMiningLine();
    }
}
