using UnityEngine;

public class PersistentIntroduction : MonoBehaviour
{
    private static PersistentIntroduction instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // ·ÀÖ¹ÖØ¸´ÊµÀý»¯
        }
    }
}

