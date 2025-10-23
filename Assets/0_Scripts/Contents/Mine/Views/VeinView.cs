using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum VeinType
{
	IronOre,
	CopperOre,
	Coal,
	GoldNugget,
	Salt,
	Bauxite,
	Lithium,
	Diamond,
	Ruby,
	Sapphire,
	Emerald,
	Quartz,
	Uranium,
	XenonCrystal,
	GammaStone,
	QuantumFlux,
	VoidCrystal,
	DarkMatterShard,
	StellarIron,
	NebulaDust,
	CosmicGlass,
	NeutroniumOre,
	AlienAlloy,
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
                var s = _atlas.GetSprite(val.ToString());
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
    private record PositionMarker
    {
        public Vector2[] Positions;
        public Vector2[] Sizes;

        public PositionMarker(GameObject[] objs)
        {
            Positions = new Vector2[objs.Length];
            Sizes = new Vector2[objs.Length];
            for (int i = 0; i < objs.Length; ++i) {
                var rt = objs[i].GetComponent<RectTransform>();
                Positions[i] = new(rt.anchoredPosition.x, -50);
                Sizes[i] = rt.sizeDelta;
            }
            Array.Sort(Positions, (a, b) => a.x.CompareTo(b.x));
            for (int i = 0; i < objs.Length; ++i) {
                Destroy(objs[i]);
            }
        }

        public Vector2 GetPosition(int idx)
        {
            return Positions[idx];
        }

        public Vector2 GetSize(int idx)
        {
            return Sizes[idx];
        }
    }

    private static PositionMarker Marker = null;

    public int Id { get; private set; }
    public int Pos { get; private set; }
    public int Type { get; private set; } = (int)VeinType.MAX_NUM;

    private Tweener _tw = null;

    Action<int> OnClick;

    public void Bind(VeinState data, Action<int> onClick)
    {
        Marker ??= new(GameObject.FindGameObjectsWithTag("VeinPositionMarker"));

        Id = data.Id;
        Type = data.Type;
        Pos = data.Pos;
        OnClick = onClick;

        SetImage();
        SetPosition();
    }

    private void SetImage(int type = (int)VeinType.MAX_NUM)
    {
        if (type == (int)VeinType.MAX_NUM)
            type = Type;
        GetComponent<Image>().sprite = VeinSpriteCatalog.GetSprite(type);
    }

    public void SetPosition()
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        var veinRT = GetComponent<RectTransform>();
        veinRT.sizeDelta = Marker.GetSize(Pos % 16 - 1);
        veinRT.anchoredPosition = Marker.GetPosition(Pos % 16 - 1);

        GetComponent<UIColliderSizeSync>().SetSize();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(Id);
    }

    public void PlayDoTween()
    {
        // DoTween
        _tw ??= GetComponent<RectTransform>()?.DOShakeAnchorPos(
             duration: 0.1f,
             strength: new Vector2(5, 5),
             vibrato: 100,
             randomness: 180,
             snapping: false,
             fadeOut: false
         ).SetAutoKill(false).SetLink(gameObject).Pause();

        _tw?.Restart();
    }
}
