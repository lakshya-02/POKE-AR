using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;
    public GameObject[] ARPrefabs;

    List<GameObject> ARObjects = new List<GameObject>();
    
    void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    // Event Handler
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //Create object based on image tracked
        foreach (var trackedImage in eventArgs.added)
        {
            foreach (var arPrefab in ARPrefabs)
            {
                if(trackedImage.referenceImage.name == arPrefab.name)
                {
                    var newPrefab = Instantiate(arPrefab, trackedImage.transform);
                    newPrefab.name = trackedImage.referenceImage.name; 
                    ARObjects.Add(newPrefab);
                    
                    // Notify that a Pokemon was spawned
                    Debug.Log($"Pokemon spawned: {trackedImage.referenceImage.name}");
                }
            }
        }
        
        //Update tracking position
        foreach (var trackedImage in eventArgs.updated)
        {
            foreach (var gameObject in ARObjects)
            {
                // Compare with reference image name
                if(gameObject.name == trackedImage.referenceImage.name)
                {
                    gameObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
                }
            }
        }
        
    }
    
    // Helper to get spawned Pokemon by name
    public GameObject GetSpawnedPokemon(string pokemonName)
    {
        foreach (var obj in ARObjects)
        {
            if (obj.name == pokemonName)
                return obj;
        }
        return null;
    }
}