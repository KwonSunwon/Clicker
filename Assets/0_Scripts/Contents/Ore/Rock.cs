using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 기본 바위 클래스
/// 기본 자원으로 모든 벽면을 채움
/// 깊이 내려갈 수록 단단해지고 이 바위를 부수면 뒤에 자원 광맥이 나타남
/// </summary>
public class Rock : MonoBehaviour
{
    public int _hp;
    private bool _isBroken = false;
    public bool IsBroken {
        get { return _isBroken; }
        set {
            _isBroken = value;
            if (_isBroken == false) {
                Init();
            }
        }
    }

    [SerializeField] private OreSpriteSet _spriteSet;

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        //TODO: 라인의 깊이 정보를 가져와 단단함(hp)을 설정
        //TODO: 바위의 스프라이트 설정
        _hp = 6;
        _isBroken = false;

        GetComponent<Image>().sprite = _spriteSet.sprites[0];
    }

    public void OnClick()
    {
        Debug.Log($"Rock {gameObject.GetInstanceID()} Clicked");
        --_hp;
        if (_hp < 6)
            GetComponent<Image>().sprite = _spriteSet.sprites[_hp];
        if (_hp <= 0) {
            Break();
        }
    }

    public void Break()
    {
        GetComponent<Image>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        _isBroken = true;
    }
}

//IDEA: 기본적으로 만들어질 때 한 줄을 바위로 다 채움
// 깊이에 따라 특정한 광맥을 특정한 개수 만큼 생성
// 바위는 다 캐서 사라지면 오브젝트 삭제
// 바위 오브젝트에 광맥 컴포넌트를 추가하는 것이 아닌
// Line에 별개로 광맥을 생성
// Layout Element를 사용해 기존 정렬을 무시하도록 설정