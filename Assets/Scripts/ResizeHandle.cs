using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeHandle : MonoBehaviour, IDragHandler
{
    [SerializeField] private DraggableWindow m_parentWindow;
    [SerializeField] private Vector2 m_cornerQuadrant; // EX: (1, 1) for top right

    void Start()
    {
        // Find parent window in scene (if not already manually set)
        Transform parentTransform = transform.parent;
        while (parentTransform != null && m_parentWindow == null)
        {
            m_parentWindow = parentTransform.GetComponent<DraggableWindow>();
            parentTransform = parentTransform.parent;
        }

        if (m_parentWindow == null)
        {
            Debug.LogError("Parent window reference not set!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_parentWindow.ResizeWindow(m_cornerQuadrant, eventData.delta);
    }
}
