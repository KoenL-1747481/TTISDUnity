using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musician : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Transform getLeftHand()
    {
        return transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1").Find("mixamorig:Spine2").Find("mixamorig:LeftShoulder").Find("mixamorig:LeftArm")
            .Find("mixamorig:LeftForeArm").Find("mixamorig:LeftHand");
    }

    public Transform getRightHand()
    {
        return transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1").Find("mixamorig:Spine2").Find("mixamorig:RightShoulder").Find("mixamorig:RightArm")
            .Find("mixamorig:RightForeArm").Find("mixamorig:RightHand");
    }

    public GameObject getLaser()
    {
        return transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1").Find("mixamorig:Spine2").Find("mixamorig:Neck").Find("mixamorig:Head").Find("Laser").gameObject;
    }
}
