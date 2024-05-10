using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;
    private void Update()
    {
        timer.text = $"Time: {Time.realtimeSinceStartup:0.000}";
    }

}
