using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class SubwayUIManager : MonoBehaviour
{
    [Header("UI Components")]
    public Text stationText;
    public Slider nextStationSlider;
    public Slider fatigueSlider;
    public Text timerText;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public Button sleepButton;
    public Button stayAwakeButton;

    [Header("Settings")]
    public string[] stations = { "12역", "13역", "14역" };
    public float stationTime = 10f; // 한 역 도달 시간
    private int currentStationIndex = 0;

    private float timeElapsed = 0f;
    private float fatigue = 0f;
    private float stationProgress = 0f;

    void Start()
    {
        UpdateStationUI();

        sleepButton.onClick.AddListener(OnSleepButtonClicked);
        stayAwakeButton.onClick.AddListener(OnStayAwakeButtonClicked);

        nextStationSlider.maxValue = stationTime;
        nextStationSlider.value = stationTime;

        fatigueSlider.maxValue = 100f;
        fatigueSlider.value = fatigue;

        dialoguePanel.SetActive(false); // 처음엔 말풍선 비활성화
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        timerText.text = timeElapsed.ToString("F2");

        // 피로도 증가
        fatigue += Time.deltaTime * 5f;
        fatigueSlider.value = fatigue;

        // 피로도 일정 이상 -> 선택지 등장
        if (fatigue >= 50f && !dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
        }

        // 역 진행도 업데이트
        stationProgress += Time.deltaTime;
        nextStationSlider.value = stationTime - stationProgress;

        if (stationProgress >= stationTime)
        {
            stationProgress = 0f;
            UpdateStation();
        }
    }

    void UpdateStation()
    {
        currentStationIndex++;
        if (currentStationIndex < stations.Length)
        {
            UpdateStationUI();
        }
        else
        {
            stationText.text = "종착역";
        }
    }

    void UpdateStationUI()
    {
        stationText.text = $"{stations[currentStationIndex]}";
        nextStationSlider.value = stationTime;
    }

    void OnSleepButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DreamScene");

    }

    void OnStayAwakeButtonClicked()
    {
        dialoguePanel.SetActive(false);
    }
}

