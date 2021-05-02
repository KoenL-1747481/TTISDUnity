using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    private float maxDistance = 500f;
    private GameObject collider = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            GameObject newCollider = hit.collider.gameObject;
            if (newCollider == null || newCollider.GetComponent<UIButton>() == null)
            {
                collider?.GetComponent<UIButton>().onExit();
                return;
            }

            if (newCollider != collider)
            {
                collider?.GetComponent<UIButton>().onExit();
                newCollider.GetComponent<UIButton>().onEnter();
            }
            collider = newCollider;
        }
        
    }
}
