using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        // 确保该 GameObject 在加载新场景时不被销毁
        DontDestroyOnLoad(gameObject);
    }
}
