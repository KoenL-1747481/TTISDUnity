//-----------------------------------------------------------------------
// <copyright file="CameraPointer.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Sends messages to gazed GameObject.
/// </summary>
public class CameraPointer : MonoBehaviour
{
    private const float _maxDistance = 100;
    private GameObject _gazedAtObject = null;
    private bool isDraggingObject = false;
    private float touchDownTime;
    private bool isHoldingTouch = false;
    private bool sentHoldMessage = false;
    private float holdDetectionTime = 0.4f;
    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
        if (!(isDraggingObject && _gazedAtObject))
        {
            RaycastHit hit;
           /* if (GraphicRaycaster.Raycast(transform.position, transform.forward, out hit, _maxDistance))
            {
                print(hit.transform.gameObject.name);
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

            }*/
        }

        //on touch down
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            touchDownTime = Time.time;
            isHoldingTouch = true;
            sentHoldMessage = false;
        }
        //on touch up
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
        {
            isHoldingTouch = false;
            float currentTime = Time.time;
            bool hold = ((currentTime - touchDownTime) > holdDetectionTime);

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
        if (isDraggingObject && isHoldingTouch && Time.time - touchDownTime > holdDetectionTime && !sentHoldMessage)
        {
            sentHoldMessage = true;
            _gazedAtObject?.SendMessage("OnDepthChangeStart");
        }
    }
}
