using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(BoxCollider2D))]
public class UIColliderSizeSync : MonoBehaviour
{
    private RectTransform _rt;
    private BoxCollider2D _bc;

    void Awake()
    {
        SetSize();
    }

    public void SetSize()
    {
        _rt = GetComponent<RectTransform>();
        _bc = GetComponent<BoxCollider2D>();

        var size = _rt.rect.size;
        if (_bc.size != size)
            _bc.size = size;
    }
}
