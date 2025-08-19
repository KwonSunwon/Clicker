using System.Collections.Generic;
using UnityEngine;

public class DataLoadTester : MonoBehaviour
{
    [ContextMenu("CSV 데이터 로드 테스트")]
    public void DataLoadTest()
    {
        Debug.Log("--- CSV 데이터 로드 테스트 시작 ---");

        TestDataLoader loader = new TestDataLoader();
        Dictionary<int, TestData> data = loader.Load();
        if (data.Count == 0)
        {
            Debug.LogError("로드된 데이터가 없습니다.");
            return;
        }

        foreach (var item in data)
        {
            Debug.Log($"ID: {item.Value.ID}, Name: {item.Value.Name}, HP: {item.Value.HP}, Gold: {item.Value.Gold}");
        }

        Debug.Log("--- CSV 데이터 로드 테스트 종료 ---");
    }
}
