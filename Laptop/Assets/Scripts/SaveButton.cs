using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TTISDProject;

public class SaveButton : MonoBehaviour
{
    public static Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate { AudioHandler.SaveLoop("the_loop.wav"); });
    }
}
