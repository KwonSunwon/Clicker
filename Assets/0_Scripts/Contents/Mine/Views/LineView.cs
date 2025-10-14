using System;
using System.Collections.Generic;
using UnityEngine;

using static Util;

public class LineView : MonoBehaviour
{
    private Transform _container;

    [SerializeField] private int _depth;
    public int Depth => _depth;

    private readonly Dictionary<int, RockView> _rocks = new();
    private readonly Dictionary<int, VeinView> _veins = new();

    public void Bind(int depth)
    {
        _depth = depth;
        _container = GetComponent<Transform>();
    }

    public void BuildFrom(
        LineState lineState,
        Func<RockState, RockView> SpawnRock,                // RockView SpawnRock(RockState rock)
        Func<VeinState, RockView, VeinView> SpawnVein)      // VeinView SpawnVein(VeinState vein, RockView rock)
    {
        Bind(lineState.Depth);
        foreach (var rockState in lineState.Rocks) {
            var rockView = SpawnRock(rockState);
            rockView.transform.SetParent(_container, false);
            _rocks.Add(rockView.Id, rockView);
        }
    }

    public void AddVeinView(VeinView veinView)
    {
        if (_veins.ContainsKey(veinView.Id)) return;
        veinView.transform.SetParent(_container, false);
        _veins.Add(veinView.Id, veinView);
    }

    public void RemoveRock()
    {
        var cnt = _rocks.Count;
        for (int i = 1; i <= cnt; i++) {
            _rocks.Remove(MakeRockId(_depth, i), out var rock);
            if (rock != null) Destroy(rock.gameObject);
        }
        _rocks.Clear();
    }

    public bool TryGetRockView(int rockId, out RockView rockView) => _rocks.TryGetValue(rockId, out rockView);
    public bool TryGetVeinView(int veinId, out VeinView veinView) => _veins.TryGetValue(veinId, out veinView);
}
