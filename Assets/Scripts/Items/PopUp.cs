using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    float disappearTime = 0.6f;

    void Update()
    {
        if (disappearTime <= 0)
        {
            Destroy(gameObject);
        }
        disappearTime -= Time.deltaTime;
    }
}
