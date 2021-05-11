//-----------------------------------------------------------------------
// <copyright file="ObjectController.cs" company="Google LLC">
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

/// <summary>
/// Controls target objects behaviour.
/// </summary>
public class ObjectController : MonoBehaviour
{
    private Outline outline;

    private Renderer _myRenderer;
    private Vector3 _startingPosition;
    private Camera mainCamera;
    private Vector3 cameraToObjectVector;
    private Vector3 targetLocation;
    private bool isChangingDepth = false;
    private float depthSpeed = 2f;
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        _startingPosition = transform.localPosition;
        _myRenderer = GetComponent<Renderer>();
        mainCamera = FindObjectOfType<Camera>();
        outline = gameObject.AddComponent(typeof(Outline)) as Outline;
        outline.OutlineWidth = 8;
        outline.OutlineColor = new Color(1, 0.46f, 0, 0.8f);
        outline.OutlineMode = Outline.Mode.OutlineAll;
        SetMaterial(false);
    }

    private void Update()
    {
        if (isChangingDepth)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, Time.deltaTime*depthSpeed);
        }
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter()
    {
        SetMaterial(true);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
        SetMaterial(false);
    }

    /// <summary>
    /// This method is called by the Main Camera when it is gazing at this GameObject and the screen
    /// is touched.
    /// </summary>
    public void OnGrab()
    {
        transform.SetParent(mainCamera.transform, true);
    }

    public void OnRelease()
    {
        Vector3 pos = transform.position;
        transform.SetParent(null);
        transform.position = pos;
    }
    public void OnDepthChangeStart()
    {
        OnRelease();
        cameraToObjectVector = transform.position - mainCamera.transform.position;
        transform.position = mainCamera.transform.position + cameraToObjectVector * 0.2f;
        targetLocation = transform.position + cameraToObjectVector*1000;
        isChangingDepth = true;
    }
    public void OnDepthChangeEnd()
    {
        isChangingDepth = false;
    }

    private void SetMaterial(bool gazedAt)
    {
        outline.enabled = gazedAt;
    }
}
