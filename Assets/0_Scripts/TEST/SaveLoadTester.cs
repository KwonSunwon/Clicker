// SaveLoadTester.cs
using UnityEngine;

public class SaveLoadTester : MonoBehaviour
{
    private SecureDataManager dataManager = new SecureDataManager();
    private PlayerData currentPlayerData;

    void Start()
    {
        // 게임 시작 시 자동으로 데이터를 불러옴
        currentPlayerData = dataManager.Load() as PlayerData;
        PrintCurrentData("게임 시작 시 불러온 데이터");
    }

    [ContextMenu("1. 테스트 데이터로 저장하기")]
    void TestSave()
    {
        Debug.Log("--- 테스트 저장 시작 ---");
        // 새로운 테스트용 데이터 생성
        PlayerData testData = new PlayerData();
        //testData.Gold = BigInteger.Parse("12345678901234567890");
        //BigInteger는 Unity의 JsonUtility로 직접 변환되지 않으므로 int로 변경(외부 Json 라이브러리 필요)
        testData.gold = 1000; // Test를 위해 BigInteger 대신 int로 변경
        testData.playerLevel = 15;
        testData.unlockedItemIDs.Add(101);
        testData.unlockedItemIDs.Add(205);

        // 데이터 저장
        dataManager.Save(testData);
        currentPlayerData = testData;
        PrintCurrentData("저장된 테스트 데이터");
    }

    [ContextMenu("2. 파일에서 데이터 불러오기")]
    void TestLoad()
    {
        Debug.Log("--- 테스트 불러오기 시작 ---");
        currentPlayerData = dataManager.Load() as PlayerData;
        PrintCurrentData("파일에서 불러온 데이터");
    }

    [ContextMenu("3. 저장 파일 경로 열기")]
    void OpenSavePath()
    {
        // 에디터에서만 동작합니다.
#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
#endif
    }

    // 현재 데이터를 콘솔에 출력하는 도우미 함수
    private void PrintCurrentData(string title)
    {
        if (currentPlayerData == null) {
            Debug.LogWarning($"[{title}] 현재 데이터가 없습니다.");
            return;
        }

        string log = $"<b>--- [{title}] ---</b>\n";
        log += $"골드: {currentPlayerData.gold}\n";
        log += $"레벨: {currentPlayerData.playerLevel}\n";
        log += $"아이템: {string.Join(", ", currentPlayerData.unlockedItemIDs)}\n";
        log += "--------------------";
        Debug.Log(log);
    }
}