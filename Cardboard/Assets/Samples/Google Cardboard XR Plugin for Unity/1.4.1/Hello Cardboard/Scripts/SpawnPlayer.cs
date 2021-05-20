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
    
    List<string> instrumentNames = new List<string>() {"Keyboard","Acoustic Guitar","Electric Guitar"};

    List<AvatarController> playerAvatarControllers = new List<AvatarController>();

    public Action addPlayer;
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

    public void addNewPlayer(int playerIndex, string instrumentName)
    {
        addPlayer = new Action(() => { spawnPlayer(playerIndex, instrumentName); });
    }

    private void Update()
    {
        if (addPlayer != null)
        {
            addPlayer();
            addPlayer = null;
        }
    }

    public void updateAvatar(int id, List<Quaternion> newBoneRotations)
    {
        playerAvatarControllers[id].updateAvatarBones(newBoneRotations);
    }

    public void spawnPlayer(int playerIndex, string instrumentName)
    {
        int i = 0;
        foreach (string model in instrumentNames)
        {
            if (model == instrumentName)
            {
                Vector3 playerPos = spawnPoints[playerIndex].position;
                playerPos -= spawnPoints[playerIndex].forward*0.3f;
                GameObject instrument = Instantiate(instrumentModels[i], spawnPoints[playerIndex].position, spawnPoints[playerIndex].rotation);
                GameObject player = Instantiate(playerModels[playerIndex], playerPos, spawnPoints[playerIndex].rotation);

                playerAvatarControllers.Add(player.GetComponent<AvatarControllerClassic>());
                Guitar g = instrument.GetComponent<Guitar>();

                g?.setRightHand(player.GetComponent<Musician>().getRightHand());
                g?.setLeftHand(player.GetComponent<Musician>().getLeftHand());

                Camera.main.gameObject.SetActive(false);
                spawnPointCameras[playerIndex].SetActive(true);

                break;
            }
            i++;
        }
    }
}
