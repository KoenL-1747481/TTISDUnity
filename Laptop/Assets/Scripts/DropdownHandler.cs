using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAudio;
using NAudio.Wave;
using TTISDProject;

public class DropdownHandler : MonoBehaviour
{
    Dropdown dropdownMenu;
    List<string> dropdownOptions = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        dropdownMenu = GetComponent<Dropdown>();
        dropdownMenu.ClearOptions();
        dropdownOptions.Add("None");
        foreach (string d in AsioOut.GetDriverNames())
            dropdownOptions.Add(d);

        dropdownMenu.AddOptions(dropdownOptions);
        dropdownMenu.value = dropdownOptions.IndexOf("");
        dropdownMenu.onValueChanged.AddListener(delegate
        {
            Debug.Log(dropdownOptions[dropdownMenu.value]);
            if (dropdownOptions[dropdownMenu.value] == "None")
            {
                AudioHandler.Dispose();
            }
            else
            {
                AudioHandler.SetAsio(dropdownOptions[dropdownMenu.value]);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
