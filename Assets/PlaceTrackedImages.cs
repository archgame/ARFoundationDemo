using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceTrackedImages : MonoBehaviour
{
    public GameObject[] ARPrefabs; //each element corresponds to a reference image in images manager
    private ARTrackedImageManager _trackedImagesManager;
    private Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>(); //prefabs we've created

    private void Awake()
    {
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged; //listen to events, looking at video feed for images
    }

    private void OnDisable()
    {
        _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    //Event Handler
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //1 get new tracked images
        foreach (var trackedImage in eventArgs.added)
        {
            var imageName = trackedImage.referenceImage.name;
            foreach (var curPrefab in ARPrefabs)
            {
                if (string.Compare(curPrefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) != 0) { continue; } //matching string
                if (_instantiatedPrefabs.ContainsKey(imageName)) { continue; } //make sure it doesn't already exist

                var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                //_instantiatedPrefabs[imageName] = newPrefab; //do we have to do .Add()?
                _instantiatedPrefabs.Add(imageName, newPrefab);
            }
        }

        //2 get updated tracked images
        foreach (var trackedImage in eventArgs.updated)
        {
            //bool state = trackedImage.trackingState == TrackingState.Tracking; //if corresponding image isn't being tracked, deactivate
            //_instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(state);
            _instantiatedPrefabs[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
            _instantiatedPrefabs[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;
        }

        //3 remove untracked images
        /*/
        foreach (var trackedImage in eventArgs.removed)
        {
            var name = trackedImage.referenceImage.name;
            Destroy(_instantiatedPrefabs[name]);
            _instantiatedPrefabs.Remove(name);
        }
        //*/
    }
}