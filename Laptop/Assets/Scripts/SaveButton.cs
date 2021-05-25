using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TTISDProject;

public class SaveButton : MonoBehaviour
{
    public static Button btn;
    private string loopName = "the_loop";
    private int numSaves = 0;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate { AudioHandler.SaveLoop(loopName + (++numSaves).ToString() + ".wav"); });
    }
}
