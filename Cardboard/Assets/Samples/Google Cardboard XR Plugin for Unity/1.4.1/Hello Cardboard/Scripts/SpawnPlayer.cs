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
    
    List<AvatarControllerClassic> playerAvatarControllers = new List<AvatarControllerClassic>();


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
    }

    public void addNewPlayer(int playerID, string instrumentName)
    {
        print("KEERERERREEREL IK ZIT IN DE ADD NEW PLAYER");
        
        //print(playerIDs.Count);
        ThreadManager.ExecuteOnMainThread(() => {
            playerIDs.Add(playerID);
            spawnPlayer(playerID, instrumentName); 
        });
    }

    public void updateAvatar(int id, List<Quaternion> newBoneRotations)
    {
        try
        {
            playerAvatarControllers[playerIDs.IndexOf(id)].updateAvatarBones(newBoneRotations);
        } catch(Exception e)
        {
            print(e);
        }
        
    }

    public void spawnPlayer(int playerID, string instrumentName)
    {
        int i = 0;
        int indexOfPlayerID = playerIDs.IndexOf(playerID);
        foreach (GameObject model in instrumentModels)
        {
            if (model.name == instrumentName)
            {
                Vector3 playerPos = spawnPoints[indexOfPlayerID].position;
                playerPos -= spawnPoints[indexOfPlayerID].forward*0.3f;
                GameObject instrument = Instantiate(instrumentModels[i], spawnPoints[indexOfPlayerID].position, spawnPoints[indexOfPlayerID].rotation);
                GameObject player = Instantiate(playerModels[playerID], playerPos, spawnPoints[indexOfPlayerID].rotation);

                playerAvatarControllers.Add(player.GetComponent<AvatarControllerClassic>());
                Guitar g = instrument.GetComponent<Guitar>();

                g?.setRightHand(player.GetComponent<Musician>().getRightHand());
                g?.setLeftHand(player.GetComponent<Musician>().getLeftHand());

                if (firstPlayer)
                {
                    Camera.main.gameObject.SetActive(false);
                    spawnPointCameras[indexOfPlayerID].SetActive(true);
                    firstPlayer = false;
                }
                break;
            }
            i++;
        }
    }
}
