using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI를 산하로 가지고 있는 부모 오브젝트에 할당하자 ex) UI_Root
public class UI_Bind : MonoBehaviour
{
    // 테스트용 타입
    public enum TestButtons
    {
        TestDreamButton,
        TestSlapButton,
    }

    // 유니티 최상위 클래스인 Object 배열로 저장
    // 예를 들어, typeof(Button)을 키로 해서 Button 타입의 오브젝트들을 배열로 저장할 수 있음
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>(); 
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<Button>(typeof(TestButtons));
        Get<Button>((int)TestButtons.TestDreamButton).onClick.AddListener(DreamButtonTest);
        Get<Button>((int)TestButtons.TestSlapButton).onClick.AddListener(SlapButtonTest);

    }

    // 테스트용 메소드
    void DreamButtonTest()
    {
        Debug.Log("꿈 속 진입!");
    }

    // 테스트용 메소드
    void SlapButtonTest()
    {
        Debug.Log("뺨 때리기!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // <T> 컴포넌트를 가진 type형 오브젝트를 이름으로 자동 매핑한 후, objects[] 딕셔너리에 넣어준다
    // Enum 값과 게임 오브젝트의 이름을 같게 하자
    public void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type); 
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            objects[i] = Util.FindChild<T>(gameObject, names[i], true);
        }
    }

    // objects[] 딕셔너리에 있는 오브젝트를 반환하는 함수
    public T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }
}
