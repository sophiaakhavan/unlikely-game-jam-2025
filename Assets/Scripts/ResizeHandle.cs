using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeHandle : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private DraggableWindow m_parentWindow;
    [SerializeField] private Vector2 m_cornerQuadrant; // EX: (1, 1) for top right
    private GameObject m_cursorManager;

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

        m_cursorManager = GameObject.Find("Cursor Manager");
        if(m_cursorManager == null)
        {
            Debug.LogError("Couldn't find cursor manager in ResizeHandle!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_parentWindow.ResizeWindow(m_cornerQuadrant, eventData.delta);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_cursorManager.GetComponent<CursorManager>().SetCursorResize();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_cursorManager.GetComponent<CursorManager>().SetCursorNormal();
    }
}
