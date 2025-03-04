using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private Vector2 m_minSize;
    
    private RectTransform m_rectTransform;
    private BoxCollider2D m_collider;

    virtual protected void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_collider = GetComponent<BoxCollider2D>();

        m_collider.size = new Vector2(m_rectTransform.rect.width + 10.0f, m_rectTransform.rect.height + 10.0f);

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

    virtual public void DragWindow(Vector2 delta) 
    {
        DragRectTransform(m_rectTransform, delta);
    }

    virtual public void ResizeWindow(Vector2 corner, Vector2 delta)
    {
        ResizeRectTransform(m_rectTransform, corner, delta);

        // Update collider box size
        m_collider.size = new Vector2(m_rectTransform.rect.width + 10.0f, m_rectTransform.rect.height + 10.0f);
    }

    protected void DragRectTransform(RectTransform rectTransform, Vector2 delta)
    {
        rectTransform.anchoredPosition += delta / m_canvas.scaleFactor;
    }

    protected void ResizeRectTransform(RectTransform rectTransform, Vector2 corner, Vector2 delta)
    {
        rectTransform.sizeDelta += (delta * corner) / m_canvas.scaleFactor;

        // Check to see if resize is below minimum dimensions
        float compareToMinWidth = rectTransform.sizeDelta.x - m_minSize.x;
        float compareToMinHeight = rectTransform.sizeDelta.y - m_minSize.y;

        Vector2 clampedSize = rectTransform.sizeDelta;

        // Don't allow resize below minimum width
        if (compareToMinWidth < 0)
        {
            delta.x -= compareToMinWidth * corner.x;
            clampedSize.x = m_minSize.x;
        }

        // Don't allow resize below minimum height
        if (compareToMinHeight < 0)
        {
            delta.y -= compareToMinHeight * corner.y;
            clampedSize.y = m_minSize.y;
        }

        rectTransform.sizeDelta = clampedSize;

        // Offset position while resizing for anchor effect
        rectTransform.anchoredPosition += (delta / 2.0f) / m_canvas.scaleFactor;
    }
}