using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Canvas m_canvas;
    
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Find canvas in scene (if not already manually set)
        Transform parentTransform = transform.parent;
        while (parentTransform != null && m_canvas == null)
        {
            m_canvas = parentTransform.GetComponent<Canvas>();
            parentTransform = parentTransform.parent;
        }

        if (m_canvas == null)
        {
            Debug.LogError("Canvas reference not set!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Move to bottom of parent's hierarchy (bring to front)
        transform.SetAsLastSibling();
    }

    public void DragWindow(Vector2 delta) 
    {
        rectTransform.anchoredPosition += delta / m_canvas.scaleFactor;
    }

    public void ResizeWindow(Vector2 corner, Vector2 delta)
    {
        rectTransform.sizeDelta += (delta * corner) / m_canvas.scaleFactor;

        // Offset position while resizing for anchor effect
        rectTransform.anchoredPosition += (delta / 2.0f) / m_canvas.scaleFactor;
    }
}