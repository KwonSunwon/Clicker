using UnityEngine;

public class AchievementManager
{
    int m_totalClicks;
    int m_totalStonesLearned;
    Time m_startTime;
    Time m_totalPlayTime;

    public void Init()
    {
        m_totalClicks = 0;
        m_totalStonesLearned = 0;
        m_startTime = new Time();
        m_totalPlayTime = new Time();
    }

    public void Load()
    {
        //TODO: 도전과제 정보를 세이브 데이터에서 불러오기
    }

    public void Save()
    {
        //TODO: 도전과제 정보를 세이브 데이터에 저장하기
    }

    public void UnlockAchievement(string achievementID)
    {
        if (!SteamManager.Initialized) return;

        // TODO: 추후 Steamworks 개발자 등록 후 스팀에 도전과제 등록 후 활성화
        //SteamUserStats.GetAchievement(achievementID, out bool isAchieved);
        //if (!isAchieved)
        //{
        //    SteamUserStats.SetAchievement(achievementID);
        //    SteamUserStats.StoreStats();
        //    Debug.Log($"Achievement Unlocked: {achievementID}");
        //}
    }

    public void IncrementClicks(int count)
    {
        m_totalClicks += count;
        CheckClickAchievements();
    }

    #region Check Achievements
    private void CheckClickAchievements()
    {
        if (m_totalClicks >= 100)
            UnlockAchievement("ACH_CLICK_100");
        if (m_totalClicks >= 1000)
            UnlockAchievement("ACH_CLICK_1000");
        if (m_totalClicks >= 10000)
            UnlockAchievement("ACH_CLICK_10000");
    }
    #endregion
}