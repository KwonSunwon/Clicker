using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum VeinType
{
    Bauxiet,
    Coal,
    Copper,
    Diamond,
    Emerald,
    Gamma,
    Iron,
    MAX_NUM
}

public static class VeinSpriteCatalog
{
    private const string PATH = "Art/Ore/Ores";
    private static SpriteAtlas _atlas;
    private static readonly Dictionary<int, Sprite> _cache = new();

    public static Sprite GetSprite(int type)
    {
        if (_atlas == null) {
            _atlas = Resources.Load<SpriteAtlas>(PATH);
            if (_atlas == null) {
                Debug.LogError($"Failed to load SpriteAtlas at path: {PATH}");
                return null;
            }

            foreach (VeinType val in Enum.GetValues(typeof(VeinType))) {
                if (val == VeinType.MAX_NUM) continue;
                var s = _atlas.GetSprite(val.ToString() + "_0");
                if (s != null) {
                    _cache[(int)val] = s;
                }
            }
        }

        return _cache.TryGetValue(type, out var sprite) ? sprite : null;
    }
}

public class VeinView : MonoBehaviour, IPointerClickHandler
{
    public int Id { get; private set; }
    public int Type { get; private set; }

    Action<int> OnClick;

    public void Bind(VeinState data, RockView rock, Action<int> onClick)
    {
        Id = data.Id;
        Type = data.Type;
        OnClick = onClick;

        SetImage();
        SetPosition(rock);
    }

    private void SetImage(int type = (int)VeinType.MAX_NUM)
    {
        if (type == (int)VeinType.MAX_NUM)
            type = Type;
        GetComponent<Image>().sprite = VeinSpriteCatalog.GetSprite(type);
    }

    public void SetPosition(RockView rock)
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        var rockRT = rock.GetComponent<RectTransform>();
        var oreRT = GetComponent<RectTransform>();
        oreRT.sizeDelta = rockRT.sizeDelta;
        oreRT.anchoredPosition = rockRT.anchoredPosition;

        GetComponent<UIColliderSizeSync>().SetSize();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(Id);
    }

    public void PlayDoTween()
    {
        // DoTween
        GetComponent<RectTransform>()?.DOShakeAnchorPos(
            duration: 0.1f,
            strength: new Vector2(5, 5),
            vibrato: 100,
            randomness: 180,
            snapping: false,
            fadeOut: false
        ).SetLink(gameObject);
    }
}
