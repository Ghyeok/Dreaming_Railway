using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WhitePanelSpawn : MonoBehaviour
{
    public Image whitePanel;
    public float fadeDuration = 0.5f;
    public float sceneDelay;


    void Awake()
    {
        whitePanel.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {

    }

    public void StartFadeAndLoadScene()
    {
        StartCoroutine(FadeInAndLoadScene());
    }
    
    private IEnumerator FadeInAndLoadScene()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            whitePanel.color = new Color(1, 1, 1, alpha);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        whitePanel.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(sceneDelay);
        SceneManager.LoadScene("TestSubwayScene");
    }
} 