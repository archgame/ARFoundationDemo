using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    //Part 1: Visual Indicator
    public GameObject Indicator;

    private ARRaycastManager _manager;
    private Pose _pos;
    private bool isValid = false;

    //Part 2: Interaction
    public GameObject prefab;

    private void Start()
    {
        _manager = FindObjectOfType<ARRaycastManager>();
    }

    private void Update()
    {
        UpdatePos();
        UpdateIndicator();

        //Part 2:
        if (!isValid) { return; }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Instantiate(prefab, _pos.position, _pos.rotation);
        }
    }

    private void UpdateIndicator()
    {
        Indicator.SetActive(isValid);
        if (!isValid) { return; }
        Indicator.transform.SetPositionAndRotation(_pos.position, _pos.rotation);
    }

    private void UpdatePos()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        _manager.Raycast(screenCenter, hits, TrackableType.Planes);
        isValid = hits.Count > 0;
        if (!isValid) { return; }
        _pos = hits[0].pose;

        //create direction based on camera direction
        var forward = Camera.current.transform.forward;
        var bearing = new Vector3(forward.x, forward.y, 0).normalized;
        _pos.rotation = Quaternion.LookRotation(bearing);
    }
}