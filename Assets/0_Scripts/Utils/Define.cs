using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public const float Epsilon = 0.01f;

	public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
    }

	public enum State
	{
		Die,
		Walk,
        Run,
		Idle,
        Jump,
		Skill,
        Tired
	}

    public enum Layer
    {
        Monster = 8,
        Ground = 9,
        Block = 10,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
        LoadingScene
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
        Enter,
        Exit,
        Scroll
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

    public enum CameraMode
    {
        QuarterView,
		FirstPerson,
	}
}
