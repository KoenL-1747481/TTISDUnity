using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Record : MonoBehaviour
{
    Slider slider;
    bool filling = false;
    List<GameObject> bars = new List<GameObject>();
    [SerializeField] private Color passedBarColor;
    public static int recordCount = 0;
    List<string> recordStages = new List<string>() { "Record Intro","Recording: bar ","Tracks recorded: "};
    [SerializeField] public static TextMeshProUGUI info;
    [SerializeField] private SpawnPlayer spawnPlayer;
    float increaseValue = 0;
    
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

    IEnumerator fill(float duration)
    {
        float time = 0.0f;
        duration = duration / 1000;
        while (time < duration)
        {
            slider.value = time / duration;
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void reset(float newFillTime)
    {
        slider.value = 0;
        foreach (GameObject bar in bars)
            bar.GetComponent<Image>().color = new Color(0,0,0);

        if (newFillTime != 0)
            StartCoroutine(fill(newFillTime));
    }


    IEnumerator NewBar(int barNumber, int totalBarCount, float timeOf1Bar)
    {
        yield return new WaitForSeconds((timeOf1Bar*barNumber)/1000);
        info.text = recordStages[1] + barNumber + "/"+totalBarCount;
        reset(timeOf1Bar);
    }

    public void StartedRecording(int clientId, double clickInterval, double timeoutInterval, int totalBarCount)
    {
        GameObject player = spawnPlayer.getPlayerByID(clientId);
        GameObject laser = player.GetComponent<Musician>().getLaser();
        laser.SetActive(true);

        info.text = recordStages[0];
        float timeOf1Bar = (float)(timeoutInterval / (totalBarCount + 1));
        reset(timeOf1Bar);

        for (int i=1; i< totalBarCount + 1; i++)
        {
            StartCoroutine(NewBar(i, totalBarCount, timeOf1Bar));
        }
        StartCoroutine(StoppedRecording(laser, (float)timeoutInterval/1000));
    }

    IEnumerator StoppedRecording(GameObject laser, float delay)
    {
        yield return new WaitForSeconds(delay);
        laser.SetActive(false);
        reset(0);
        recordCount += 1;
        info.text = recordStages[2]+recordCount;
    }
}
