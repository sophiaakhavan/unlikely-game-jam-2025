using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeHandle : MonoBehaviour, IDragHandler
{
    [SerializeField] private Window parentWindow;
    [SerializeField] private Vector2 cornerQuadrant; // EX: (1, 1) for top right

    void Start()
    {
        // Find parent window in scene (if not already manually set)
        Transform parentTransform = transform.parent;
        while (parentTransform != null && parentWindow == null)
        {
            parentWindow = parentTransform.GetComponent<Window>();
            parentTransform = parentTransform.parent;
        }

        if (parentWindow == null)
        {
            Debug.LogError("Parent window reference not set!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentWindow.ResizeWindow(cornerQuadrant, eventData.delta);
    }
}
