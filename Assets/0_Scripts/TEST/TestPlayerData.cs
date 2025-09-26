// PlayerData.cs
using System.Collections.Generic;

// 이 클래스는 JsonUtility로 변환할 수 있도록 [System.Serializable]을 붙여줍니다.
[System.Serializable]
public class PlayerData : ISaveable
{
    // BigInteger는 직접 Json으로 변환되지 않으므로 string으로 저장합니다.
    //public string goldString;
    public int gold; // BigInteger 대신 int로 변경 (테스트 용이성)
    public int playerLevel;
    public List<int> unlockedItemIDs;

    // 실제 게임에서는 이 프로퍼티를 통해 gold 값에 접근하고 수정합니다.
    //public BigInteger Gold
    //{
    //    get
    //    {
    //        // goldString이 비어있거나 null이면 0을 반환
    //        if (string.IsNullOrEmpty(goldString)) return 0;
    //        return BigInteger.Parse(goldString);
    //    }
    //    set
    //    {
    //        goldString = value.ToString();
    //    }
    //}

    // 새 게임을 시작할 때의 기본값 설정
    public PlayerData()
    {
        gold = 0;
        playerLevel = 1;
        unlockedItemIDs = new List<int>();
    }
}