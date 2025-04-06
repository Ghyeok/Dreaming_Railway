using UnityEngine;

public class SubwayCharacter : MonoBehaviour
{
    public float fatigue = 0f;
    public float maxFatigue = 100f;
    public bool isAsleep = false;

    void Update()
    {
        if (!isAsleep)
        {
            IncreaseFatigue(Time.deltaTime * 5f);
        }
    }

    public void IncreaseFatigue(float amount)
    {
        fatigue += amount;
        fatigue = Mathf.Clamp(fatigue, 0f, maxFatigue);

        if (fatigue >= maxFatigue)
        {
            FallAsleep();
        }
    }

    void FallAsleep()
    {
        isAsleep = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("DreamScene"); 
    }

    public void WakeUp()
    {
        fatigue = 0f;
        isAsleep = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SubwayScene"); 
    }
}

