using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowDrag : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private GameObject windowParent;
    [SerializeField] private RectTransform parentRectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image foregroundImage;

    private void Start()
    {
        if(windowParent == null)
        {
            Debug.LogError("Window parent reference not set on window drag component of title bar!");
        }
        if(foregroundImage == null)
        {
            Debug.LogError("Foreground image reference not set on window drag component of title bar!");
        }
        if(parentRectTransform == null)
        {
            Debug.LogError("Parent rect transform not set on window drag component of title bar!");
        }

        canvas = GetComponentInParent<Canvas>();
        if(canvas == null)
        {
            Debug.LogError("Couldn't find canvas in parent of draggable window title bar!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //TODO: check foregroundImage overlapping with certain areas to reveal hidden dialogue elements
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        // Bring window to bottom of heirarchy of canvas to bring it out on top
        windowParent.transform.SetAsLastSibling();
    }
}
