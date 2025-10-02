using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Contents
    GameManagerEx _game = new GameManagerEx();
    AchievementManager _achievement = new AchievementManager();
    MineralManager _mineralManager = new MineralManager();
    SkillManager _skillManager = new SkillManager();

<<<<<<< HEAD
    public static GameManagerEx Game { get { return Instance._game; } }
    public static AchievementManager Achievement { get { return Instance._achievement; } }
    public static MineralManager Mineral { get { return Instance._mineralManager; } }
    #endregion
=======
    public static GameManagerEx Game {  get { return Instance._game; } }
    public static MineralManager Mineral {  get { return Instance._mineralManager; } }
    public static SkillManager Skill { get { return Instance._skillManager; } }
	#endregion
>>>>>>> origin/resource

    #region Core
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    SettingManager _setting = new SettingManager();


    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static SettingManager Setting { get { return Instance._setting; } }
    #endregion

    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        _input.OnUpdate();
    }


    static void Init()
    {
        if (s_instance == null) {
            GameObject go = GameObject.Find("@Managers");
            if (go == null) {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
            s_instance._setting.Load();
            s_instance._mineralManager.Init();
            s_instance._mineralManager.TickLoop();
<<<<<<< HEAD
            s_instance._achievement.Init();
        }
    }
=======
            s_instance._skillManager.Init();
        }		
	}
>>>>>>> origin/resource

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}