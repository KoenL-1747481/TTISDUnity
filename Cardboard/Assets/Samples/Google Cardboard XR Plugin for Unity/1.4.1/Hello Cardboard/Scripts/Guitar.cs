using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guitar : MonoBehaviour
{

    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = (leftHand.position + rightHand.position) / 2;
        transform.forward = (leftHand.position - rightHand.position);
    }

    public void setLeftHand(Transform leftHand)
    {
        this.leftHand = leftHand;
    }
    public void setRightHand(Transform rightHand)
    {
        this.rightHand = rightHand;
    }
}
