using UnityEngine;
using UnityEngine.EventSystems;

public class DragThresholdSetter : MonoBehaviour
{
    [SerializeField] private int dragThreshold = 15; // 기본 5~10 정도

    void Awake()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.pixelDragThreshold = dragThreshold;
            Debug.Log($"DragThresholdSetter: Set drag threshold to {dragThreshold}");
        }
        else
        {
            Debug.LogWarning("DragThresholdSetter: No EventSystem found in the scene.");
        }
    }
}
