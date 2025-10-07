using System;
using System.Collections.Generic;
using UnityEngine;

public class LineView : MonoBehaviour
{
    [SerializeField] Transform rockContainer;
    public int Depth { get; private set; }

    private readonly Dictionary<int, RockView> _rocks = new();

    public void Bind(int depth) => Depth = depth;

    public void BuildFrom(LineState lineState, Func<RockState, RockView> spawnRock)
    {
        Bind(lineState.Depth);
        foreach (var rockState in lineState.Rocks) {
            var rockView = spawnRock(rockState);
            rockView.transform.SetParent(rockContainer, false);
            _rocks.Add(rockView.Id, rockView);
        }
    }

    public bool TryGetRockView(int rockId, out RockView rockView) => _rocks.TryGetValue(rockId, out rockView);
}
