using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _togglePlanesAction;

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;
    private int _numPlanesAddedOccured = 0;


    void Start()
    {
        Debug.Log("-> SceneController::Start()");

        _planeManager= GetComponent<ARPlaneManager>();
        if (_planeManager != null)
        {
            Debug.Log("-> Can't find 'ARPlaneManager' :'(");
        }

        _togglePlanesAction.action.performed += OnTogglePlanesAction;
        _planeManager.planesChanged += OnPlanesChanged;

    }

    void Update()
    {
        
    }

    private void OnTogglePlanesAction(InputAction.CallbackContext obj) {
        _isVisible = !_isVisible;
        float fillAlpha = _isVisible ? 0.3f : 0.0f;
        float lineAlpha = _isVisible ? 1.0f : 0.0f;

        Debug.Log("-> OnTogglePlanesAction() - trackables.count: " + _planeManager.trackables.count);
        foreach (var plane in _planeManager.trackables)
        {
            SetPlaneAlpha(plane, fillAlpha, lineAlpha);
        }

    }

    private void SetPlaneAlpha(ARPlane plane, float fillAlpha, float lineAlpha)
    {
        var meshRenderer = plane.GetComponentInChildren<MeshRenderer>();
        var lineRenderer = plane.GetComponentInChildren<LineRenderer>();

        if (meshRenderer != null)
        {
            Color color = meshRenderer.material.color;
            color.a = fillAlpha;
            meshRenderer.material.color = color;
        }

        if (lineRenderer != null)
        {
            //rn start and end colours
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            //set alpha comp
            startColor.a = lineAlpha;
            endColor.a = lineAlpha;

            //apply new updated alpha
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
        if (args.added.Count> 0)
        {
            _numPlanesAddedOccured++;
            foreach (var plane in _planeManager.trackables)
            {
                PrintPlaneLabel(plane);
            }

            Debug.Log("-> Number of planes: " + _planeManager.trackables.count);
            Debug.Log("-> Num Planes Added Occured:" + _numPlanesAddedOccured);
        }
    
    }

    private void PrintPlaneLabel(ARPlane plane)
    {
        string label = plane.classification.ToString();
        string log = $"Plane ID: {plane.trackableId}, Label: {label}";
        Debug.Log(log);
    }

    void OnDestroy()
    {
        Debug.Log("-> SceneController::OnDestroy()");
        _togglePlanesAction.action.performed -= OnTogglePlanesAction;
        _planeManager.planesChanged -= OnPlanesChanged;
    }
}
