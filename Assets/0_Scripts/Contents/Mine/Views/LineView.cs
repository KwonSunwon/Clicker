using System;
using System.Collections.Generic;
using UnityEngine;

using static Util;

public class LineView : MonoBehaviour
{
    private Transform container;

    public int Depth { get; private set; }

    private readonly Dictionary<int, RockView> _rocks = new();
    private readonly Dictionary<int, VeinView> _veins = new();

    public void Bind(int depth)
    {
        Depth = depth;
        container = GetComponent<Transform>();
    }

    public void BuildFrom(
        LineState lineState,
        Func<RockState, RockView> SpawnRock,                // RockView SpawnRock(RockState rock)
        Func<VeinState, RockView, VeinView> SpawnVein)      // VeinView SpawnVein(VeinState vein, RockView rock)
    {
        Bind(lineState.Depth);
        foreach (var rockState in lineState.Rocks) {
            var rockView = SpawnRock(rockState);
            rockView.transform.SetParent(container, false);
            _rocks.Add(rockView.Id, rockView);
        }
    }

    public void AddVeinView(VeinView veinView)
    {
        if (_veins.ContainsKey(veinView.Id)) return;
        veinView.transform.SetParent(container, false);
        _veins.Add(veinView.Id, veinView);
    }

    public void RemoveRock()
    {
        var cnt = _rocks.Count;
        for (int i = 1; i <= cnt; i++) {
            _rocks.Remove(MakeRockId(Depth, i), out var rock);
            if (rock != null) Destroy(rock.gameObject);
        }
        _rocks.Clear();
    }

    public bool TryGetRockView(int rockId, out RockView rockView) => _rocks.TryGetValue(rockId, out rockView);
    public bool TryGetVeinView(int veinId, out VeinView veinView) => _veins.TryGetValue(veinId, out veinView);
}
