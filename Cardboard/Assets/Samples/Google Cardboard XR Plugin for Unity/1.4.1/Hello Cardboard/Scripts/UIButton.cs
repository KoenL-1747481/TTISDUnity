using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
public class UIButton : MonoBehaviour
{
    [SerializeField] private UnityEvent<string> clickResult = null;
    [SerializeField] private TextMeshProUGUI t;
    private Color startColor;
    private bool entered = false;
    private bool heldButton = false;
    // Start is called before the first frame update
    void Start()
    {
        startColor = t.color;
    }

    public void onEnter()
    {
        t.color = new Color(0.3568628f, 0.6332799f, 0.7647f);
        entered = true;
    }

    public void onExit()
    {
        t.color = startColor;
        entered = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && entered && !heldButton)
        {
            clickResult?.Invoke(name);
            entered = false;
            heldButton = true;
        }
        heldButton = false;
    }
}
