using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLight : MonoBehaviour
{

    private Light light;
    [SerializeField] private float delay = 5f;
    [SerializeField] private bool startOff = false;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        if (startOff)
            light.enabled = false;

        StartCoroutine(flash());
    }

    IEnumerator flash()
    {
        yield return new WaitForSeconds(delay);
        light.enabled = !light.enabled;
        StartCoroutine(flash());
    }
}
