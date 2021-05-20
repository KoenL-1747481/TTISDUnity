using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTISDProject
{
    public class ServerManager : MonoBehaviour
    {
        public static ServerManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        private void Start()
        {
            Server.Start();
        }

        private void OnApplicationQuit()
        {
            Server.Dispose();       
        }
    }
}