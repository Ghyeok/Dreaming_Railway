using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 플레이어가 지하철에 있을 때의 전체적인 흐름을 관리하는 매니저이다.
 * 게임 모드, 뺨 치기 쿨타임, 뺨 치기 피로도 감소량, 현재 Day 몇인지 등등을 다룬다.
 */
public class SubwayGameManager : SingletonManagers<SubwayGameManager>
{
    public bool isSlapCoolTime;
    public float tiredDecreaseBySlap;
    public int dayCount;

    public bool isStopping; // 정차중

    [SerializeField]
    public Timer timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Init()
    {
        UI_SubwayScene _subway = UIManager.Instance.ShowSceneUI<UI_SubwayScene>("UI_SubwayScene");
        SoundManager.Instance.SubwayBGM();

        isStopping = false;
        isSlapCoolTime = false;
        tiredDecreaseBySlap = 3f;
        dayCount = 1;
        timer = GetComponent<Timer>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestSubwayScene")
        {
            Debug.Log($"지하철 씬 로드 : {gameObject.name}");
            Init();
        }
    }
}
