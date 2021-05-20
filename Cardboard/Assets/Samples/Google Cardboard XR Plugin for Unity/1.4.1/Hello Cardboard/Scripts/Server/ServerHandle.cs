using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void KinectDataReceived(int _fromClient, Packet _packet)
    {
        try
        {
            Debug.Log("Received kinect data!");
            List<Quaternion> boneRotations = _packet.ReadQuaternionList();
            SessionManager.instance.HandleKinectData(_fromClient, boneRotations);
        } catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("Failed to handle received UDP kinect data");
        }
    }

}
