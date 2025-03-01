using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ApplicationWindow : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Find canvas in scene (if not already manually set)
        Transform parentTransform = transform.parent;
        while (parentTransform != null && canvas == null)
        {
            canvas = parentTransform.GetComponent<Canvas>();
            parentTransform = parentTransform.parent;
        }

        if (canvas == null)
        {
            Debug.LogError("Canvas reference not set!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Move to bottom of parent's hierarchy (bring to front)
        transform.SetAsLastSibling();
    }

    public void DragWindow(PointerEventData eventData) 
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}