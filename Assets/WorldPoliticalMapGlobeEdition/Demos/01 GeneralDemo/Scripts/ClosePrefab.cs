using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePrefab : MonoBehaviour
{

    public void ClosePanel(GameObject currentPanel)
    {
        Destroy(currentPanel);
    }
    
    
}
