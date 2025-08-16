using UnityEngine;
using System.Collections;


public class SubwayMiniMove : MonoBehaviour
{
    public RectTransform targetImage;

    public RectTransform position0;
    public RectTransform position1;
    public RectTransform position2;
    public RectTransform position3;
    public RectTransform position4;
    public RectTransform position5;

    private Coroutine moveCoroutine;
    public float duration;

    public void MoveToPosition(RectTransform GoalPos)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        Vector2 GoalPosition = new Vector2(GoalPos.anchoredPosition.x, targetImage.anchoredPosition.y);
        moveCoroutine = StartCoroutine(MoveRoutine(GoalPosition));
    }

    private IEnumerator MoveRoutine(Vector2 GoalPosition)
    {
        Vector2 start = targetImage.anchoredPosition;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / duration;

            float MovingTime = Mathf.SmoothStep(0, 1, time);
            targetImage.anchoredPosition = Vector2.Lerp(start, GoalPosition, MovingTime);
            yield return null;
        }

        targetImage.anchoredPosition = GoalPosition;
    }
}
