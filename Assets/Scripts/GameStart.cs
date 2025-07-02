using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class GameStart : MonoBehaviour
{
    public string startButtonTag = "sta"; // Tag for all start buttons
    public string imageTag = "img";       // Tag for the title image
    public MonoBehaviour imageTracker;    // Assign your ImageTracker script here
    public ARTrackedImageManager trackedImageManager; // Assign in Inspector

    public Canvas landscapeCanvas;        // Assign in Inspector (not auto-
    public Canvas portraitCanvas;         // Assign in Inspector

    private List<Button> startButtons = new List<Button>();
    private GameObject titleImage;
    private bool gameStarted = false;

    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        // Find all start buttons by tag
        GameObject[] buttonObjects = GameObject.FindGameObjectsWithTag(startButtonTag);
        foreach (var obj in buttonObjects)
        {
            Button btn = obj.GetComponent<Button>(); // btn define here 
            if (btn != null)
            {
                btn.onClick.AddListener(OnStartButtonPressed);
                startButtons.Add(btn);
            }
        }

        // Find title image by tag
        GameObject imgObj = GameObject.FindGameObjectWithTag(imageTag);
        if (imgObj != null)
            titleImage = imgObj;

        // Disable image tracking at start
        if (imageTracker != null)
            imageTracker.enabled = false;

        // Disable ARTrackedImageManager at start
        if (trackedImageManager != null)
            trackedImageManager.enabled = false;

        // Show title image
        if (titleImage != null)
            titleImage.SetActive(true);

        // Set correct canvas and button at start
        UpdateCanvas();
    }

    void Update()
    {
        // Only call this if the resolution has actually changed
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateCanvas();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    void UpdateCanvas()
    {
        Debug.Log($"Updating Canvas. Current Resolution: {Screen.width}x{Screen.height}");
        bool isLandscape = Screen.width > Screen.height;

        if (landscapeCanvas != null)
            landscapeCanvas.enabled = isLandscape;
        if (portraitCanvas != null)
            portraitCanvas.enabled = !isLandscape;

        // Only show start buttons if game has not started
        foreach (var btn in startButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(!gameStarted);
        }
    }

    void OnStartButtonPressed()
    {
        gameStarted = true; // Set flag

        // Enable image tracking
        if (imageTracker != null)
            imageTracker.enabled = true;

        // Enable ARTrackedImageManager
        if (trackedImageManager != null)
            trackedImageManager.enabled = true;

        // Hide title image
        if (titleImage != null)
            titleImage.SetActive(false);

        // Hide all start buttons and remove listeners
        foreach (var btn in startButtons)
        {
            if (btn != null)
            {
                btn.gameObject.SetActive(false);
                btn.onClick.RemoveListener(OnStartButtonPressed);
            }
        }
    }
}
