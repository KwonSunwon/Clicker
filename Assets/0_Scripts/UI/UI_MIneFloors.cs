using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MineFloors : UI_Base
{
    enum GameObjects
    {
        Floors,
        Sky,
        Ground,
    }

    enum Buttons
    {
        //UI_MineButton,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        //Bind<Button>(typeof(Buttons));

        //GetButton((int)Buttons.UI_MineButton).gameObject.BindEvent(OnClickMineButton);
    }

    private void OnClickMineButton(PointerEventData data)
    {
        Debug.Log("UI_MineFloors: Mine Button Clicked");
    }

    [ContextMenu("Test_AddFloor")]
    public void TestAddFloor()
    {
        var floors = GetObject((int)GameObjects.Floors);
        // Add a new mining line prefab
        UI_MiningLine line = Managers.UI.MakeSubItem<UI_MiningLine>(floors.transform);
        line.Init();
    }
}
