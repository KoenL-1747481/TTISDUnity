using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelect : MonoBehaviour
{
    [SerializeField] private List<GameObject> instrumentModels;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private List<GameObject> spawnPointCameras = new List<GameObject>();

    [SerializeField] private GameObject playerModel;
    private int playerIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            spawnPointCameras.Add(spawnPoint.Find("Camera").gameObject);
            spawnPointCameras[spawnPointCameras.Count - 1].SetActive(false);
        }
    }

    
    public void chooseInstrument(string name)
    {
        foreach (GameObject model in instrumentModels)
        {
            if (model.name == name)
            {
                Instantiate(model, spawnPoints[playerIndex].position, spawnPoints[playerIndex].rotation);
                Camera.main.gameObject.SetActive(false);
                spawnPointCameras[playerIndex].SetActive(true);
                break;
            }
        }
        playerIndex++;
    }
}
