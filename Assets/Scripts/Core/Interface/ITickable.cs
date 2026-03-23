using UnityEngine;

public interface ITickable
{
    // 매 프레임 실행되는 함수
    void Tick(float deltaTime, float speed = 1.0f);
}
