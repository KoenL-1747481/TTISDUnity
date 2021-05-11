using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    private const float _maxDistance = 100;
    private GameObject _gazedAtObject = null;
    private bool isDraggingObject = false;
    private float mouseDownTime;
    private bool isPressingMouse = false;
    private bool sentHoldMessage = false;
    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        //if already dragging an object, no need to cast ray
        if (!(isDraggingObject && _gazedAtObject))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance))
            {
                // GameObject detected in front of the camera.
                if (_gazedAtObject != hit.transform.gameObject)
                {
                    // New GameObject.
                    _gazedAtObject?.SendMessage("OnPointerExit");
                    _gazedAtObject = hit.transform.gameObject;
                    _gazedAtObject.SendMessage("OnPointerEnter");
                }
            }
            else
            {
                // No GameObject detected in front of the camera.
                _gazedAtObject?.SendMessage("OnPointerExit");
                _gazedAtObject = null;
            }
        }

        //on mouse down
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownTime = Time.time;
            isPressingMouse = true;
            sentHoldMessage = false;
        }
        //on mouse up
        if (Input.GetMouseButtonUp(0))
        {
            isPressingMouse = false;
            float currentTime = Time.time;
            bool hold = ((currentTime - mouseDownTime) > 0.4f);

            //if click and not dragging object yet: drag new object
            if (!hold && !isDraggingObject)
            {
                isDraggingObject = true;
                _gazedAtObject?.SendMessage("OnGrab");
            }
            //if click and dragging object: release object
            else if (!hold && isDraggingObject)
            {
                isDraggingObject = false;
                _gazedAtObject?.SendMessage("OnRelease");
            }
            //if holding ended
            else if (hold && isDraggingObject)
            {
                isDraggingObject = false;
                _gazedAtObject?.SendMessage("OnDepthChangeEnd");
            }
        }
        //if holding started
        if (isDraggingObject && isPressingMouse && Time.time-mouseDownTime > 0.4f && !sentHoldMessage)
        {
            sentHoldMessage = true;
            _gazedAtObject?.SendMessage("OnDepthChangeStart");
        }
    }
}
