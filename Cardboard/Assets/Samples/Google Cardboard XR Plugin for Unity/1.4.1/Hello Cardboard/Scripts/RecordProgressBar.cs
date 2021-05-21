using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RecordProgressBar : MonoBehaviour
{
    Slider slider;
    bool goingUp = true;
    List<GameObject> bars = new List<GameObject>();
    [SerializeField] private Color passedBarColor;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = 0;
        foreach (Transform child in transform)
            if (child.name.Contains("Bar"))
                bars.Add(child.gameObject);
    }

    public void changeProgress(float value)
    {
        slider.value += value;
        float width = transform.GetComponent<RectTransform>().sizeDelta.x;
        foreach (GameObject bar in bars)
        {
            float barXPos = bar.GetComponent<RectTransform>().anchoredPosition.x + width / 2;

            if ((slider.value * 100) > (100 / (width / barXPos)) && bar.GetComponent<Image>().color != passedBarColor)
            {
                bar.GetComponent<Image>().color = passedBarColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (goingUp)
        {
            changeProgress(0.001f);
            if (slider.value >= 1f)
                goingUp = false;
        } 
        else
        {
            changeProgress(-0.1f);
            if (slider.value <= 0f)
                goingUp = true;
        }
        
    }
}
