using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        // ȷ���� GameObject �ڼ����³���ʱ��������
        DontDestroyOnLoad(gameObject);
    }
}
