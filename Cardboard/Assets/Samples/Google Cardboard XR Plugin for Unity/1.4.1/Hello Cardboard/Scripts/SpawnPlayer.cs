using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private List<GameObject> instrumentModels;

    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> spawnPointCameras = new List<GameObject>();

    [SerializeField] private List<GameObject> playerModels;
    private List<int> chosenPlayerIndexes = new List<int>();
    
    //List<string> instrumentNames = new List<string>() {"Keyboard","Acoustic Guitar","Electric Guitar"};

    List<AvatarController> playerAvatarControllers = new List<AvatarController>();


    List<int> playerIDs = new List<int>();

    bool firstPlayer = true;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
            spawnPointCameras.Add(child.Find("Camera").gameObject);
            spawnPointCameras[spawnPointCameras.Count - 1].SetActive(false);
        }
        //spawnPlayer(0, "Keyboard");
    }

    public void addNewPlayer(int playerID, string instrumentName)
    {
        playerIDs.Add(playerID);
        ThreadManager.ExecuteOnMainThread(() => { 
            spawnPlayer(playerIDs.Count-1, instrumentName); 
        });
    }

    public void updateAvatar(int id, List<Quaternion> newBoneRotations)
    {
        playerAvatarControllers[playerIDs[playerIDs.IndexOf(id)]].updateAvatarBones(newBoneRotations);
    }

    public void spawnPlayer(int playerIndex, string instrumentName)
    {
        int i = 0;
        foreach (GameObject model in instrumentModels)
        {
            if (model.name == instrumentName)
            {
                Vector3 playerPos = spawnPoints[playerIndex].position;
                playerPos -= spawnPoints[playerIndex].forward*0.3f;
                GameObject instrument = Instantiate(instrumentModels[i], spawnPoints[playerIndex].position, spawnPoints[playerIndex].rotation);
                GameObject player = Instantiate(playerModels[playerIndex], playerPos, spawnPoints[playerIndex].rotation);

                playerAvatarControllers.Add(player.GetComponent<AvatarControllerClassic>());
                Guitar g = instrument.GetComponent<Guitar>();

                g?.setRightHand(player.GetComponent<Musician>().getRightHand());
                g?.setLeftHand(player.GetComponent<Musician>().getLeftHand());

                if (firstPlayer)
                {
                    Camera.main.gameObject.SetActive(false);
                    spawnPointCameras[playerIndex].SetActive(true);
                    firstPlayer = false;
                }

                break;
            }
            i++;
        }
    }
}
