using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelect : MonoBehaviour
{
    [SerializeField] private List<GameObject> instrumentModels;

    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> spawnPointCameras = new List<GameObject>();

    [SerializeField] private GameObject playerModel;
    private int playerIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
            spawnPointCameras.Add(child.Find("Camera").gameObject);
            spawnPointCameras[spawnPointCameras.Count - 1].SetActive(false);
        }
    }


    public void spawnPlayer(int playerIndex, string instrumentName)
    {
        foreach (GameObject model in instrumentModels)
        {
            if (model.name == instrumentName)
            {
                Vector3 playerPos = spawnPoints[playerIndex].position;
                playerPos -= spawnPoints[playerIndex].forward*0.3f;
                Instantiate(model, spawnPoints[playerIndex].position, spawnPoints[playerIndex].rotation);
                Instantiate(playerModel, playerPos, spawnPoints[playerIndex].rotation);
                Camera.main.gameObject.SetActive(false);
                spawnPointCameras[playerIndex].SetActive(true);
                break;
            }
        }
    }
}
