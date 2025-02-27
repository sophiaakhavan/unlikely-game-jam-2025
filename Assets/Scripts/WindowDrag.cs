using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowDrag : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject windowParent;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private RectTransform[] resizeHandles;
    [SerializeField] private List<RectTransform> draggableAreas;
    private bool isResizing = false;
    private bool canDrag = false;
    private Vector2 initialMousePosition;
    private Vector2 initialSize;
    private RectTransform activeResizeHandle;
    private RectTransform rectTransform;

    private void Start()
    {
        if(windowParent == null)
        {
            Debug.LogError("Window parent reference not set on window drag component!");
        }
        if(foregroundImage == null)
        {
            Debug.LogError("Foreground image reference not set on window drag component!");
        }

        rectTransform = GetComponent<RectTransform>();
        if(rectTransform == null)
        {
            Debug.LogError("Rect transform not set on window drag component!");
        }

        canvas = GetComponentInParent<Canvas>();
        if(canvas == null)
        {
            Debug.LogError("Couldn't find canvas in parent of draggable window!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        if (isResizing)
        {
            Vector2 currentMousePosition = eventData.position;
            Vector2 delta = (currentMousePosition - initialMousePosition) / canvas.scaleFactor;
            ResizeWindow(delta);
        }
        else
        {
            // Move window itself
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        // Bring window to bottom of heirarchy of canvas to bring it out on top
        windowParent.transform.SetAsLastSibling();

        // Check if the pointer is over any of the resize handles
        for (int i = 0; i < resizeHandles.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(resizeHandles[i], eventData.position, eventData.pressEventCamera))
            {
                if(!isResizing)
                {
                    isResizing = true;
                    activeResizeHandle = resizeHandles[i];
                    initialMousePosition = eventData.position;
                    initialSize = rectTransform.sizeDelta;
                }

                break;
            }
        }

        // Check if the pointer is over any of the draggable areas
        foreach (RectTransform draggableArea in draggableAreas)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(draggableArea, eventData.position, eventData.pressEventCamera))
            {
                canDrag = true;
                break;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isResizing = false;
        canDrag = false;
    }

    private void ResizeWindow(Vector2 delta)
    {
        Vector2 newPivot;
        if (activeResizeHandle == resizeHandles[0]) // Top left
        {
            newPivot = new Vector2(1, 0);
            if (rectTransform.pivot != newPivot)
            {
                AdjustPositionForNewPivot(newPivot);
            }

            rectTransform.sizeDelta = new Vector2(initialSize.x - delta.x, initialSize.y + delta.y);
        }
        else if (activeResizeHandle == resizeHandles[1]) // Top right
        {
            newPivot = new Vector2(0, 0);
            if (rectTransform.pivot != newPivot)
            {
                AdjustPositionForNewPivot(newPivot);
            }

            rectTransform.sizeDelta = new Vector2(initialSize.x + delta.x, initialSize.y + delta.y);
        }
        else if (activeResizeHandle == resizeHandles[2]) // Bottom left
        {
            newPivot = new Vector2(1, 1);
            if (rectTransform.pivot != newPivot)
            {
                AdjustPositionForNewPivot(newPivot);
            }

            rectTransform.sizeDelta = new Vector2(initialSize.x - delta.x, initialSize.y - delta.y);
        }
        else if (activeResizeHandle == resizeHandles[3]) // Bottom right
        {
            newPivot = new Vector2(0, 1);
            if (rectTransform.pivot != newPivot)
            {
                AdjustPositionForNewPivot(newPivot);
            }

            rectTransform.sizeDelta = new Vector2(initialSize.x + delta.x, initialSize.y - delta.y);
        }
    }

    /// <summary>
    /// Adjust the position of the window to offset the snapping that occurs when changing the pivot of the rect transform
    /// </summary>
    /// <param name="pivot"></param>
    private void AdjustPositionForNewPivot(Vector2 pivot)
    {
        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        //i apologize for hardcoding
        Vector3 deltaPosition = new Vector2(deltaPivot.x * (size.x / 3.3f), deltaPivot.y * (size.y / 3.3f));
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

}
