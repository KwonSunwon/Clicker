using System;
using System.Collections.Generic;
using UnityEngine;

public class LineView : MonoBehaviour
{
    private Transform _container;

    [SerializeField] private int _depth;
    public int Depth => _depth;

    private Dictionary<int, RockView> _rocks = null;
    private readonly Dictionary<int, VeinView> _veins = new();

    public void Bind(int depth)
    {
        _depth = depth;
        _container = GetComponent<Transform>();
    }

    public void BuildFrom(
        LineState lineState,
        Func<RockState, RockView> SpawnRock,      // RockView SpawnRock(RockState rock)
        Func<VeinState, VeinView> SpawnVein)      // VeinView SpawnVein(VeinState vein)
    {
        Bind(lineState.Depth);

        if (lineState.Rocks != null) {
            _rocks = new Dictionary<int, RockView>();
            foreach (var rockState in lineState.Rocks) {
                var rockView = SpawnRock(rockState);
                rockView.transform.SetParent(_container, false);
                _rocks.Add(rockView.Id, rockView);
            }
        }
        else {
            foreach (var veinState in lineState.Veins)
                AddVeinView(SpawnVein(veinState));
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
        //var cnt = _rocks.Count;
        //for (int i = 1; i <= cnt; i++) {
        //    _rocks.Remove(MakeRockId(_depth, i), out var rock);
        //    if (rock != null) Destroy(rock.gameObject);
        //}
        foreach (var rock in _rocks.Values) {
            if (rock != null) Destroy(rock.gameObject);
        }
        _rocks.Clear();
        _rocks = null;
    }

    public bool TryGetRockView(int rockId, out RockView rockView)
    {
        if (_rocks == null) {
            rockView = null;
            return false;
        }
        return _rocks.TryGetValue(rockId, out rockView);
    }
    public bool TryGetVeinView(int veinId, out VeinView veinView) => _veins.TryGetValue(veinId, out veinView);
}
