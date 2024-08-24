using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    void OnEnable()
    {
        utilitiesManager.Instance.ToggleConnectionMode(true);
    }

    void OnDisable()
    {
        utilitiesManager.Instance.ToggleConnectionMode(false);
    }
}
