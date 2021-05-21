using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class KinectData
{
    public int CardboardId { get; set; }
    public List<Quaternion> BoneRotations { get; set; }

    public KinectData(int cardboardId, List<Quaternion> boneRotations)
    {
        CardboardId = cardboardId;
        BoneRotations = boneRotations;
    }
}

