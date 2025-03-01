using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandle : MonoBehaviour, IDragHandler
{
    [SerializeField] private ApplicationWindow parentWindow;

    void Start()
    {
        // Find parent window in scene (if not already manually set)
        Transform parentTransform = transform.parent;
        while (parentTransform != null && parentWindow == null)
        {
            parentWindow = parentTransform.GetComponent<ApplicationWindow>();
            parentTransform = parentTransform.parent;
        }

        if (parentWindow == null)
        {
            Debug.LogError("Parent window reference not set!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentWindow.DragWindow(eventData);
    }
}