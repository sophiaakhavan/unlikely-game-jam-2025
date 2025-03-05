using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FilterWindow : DraggableWindow
{
    [SerializeField] private GameObject m_filter;
    [SerializeField] private GameObject m_filterParent;

    override protected void Start()
    {
        base.Start();

        if (m_filter == null)
        {
            Debug.LogError("Filter reference not set!");
            return;
        }
        if (m_filterParent == null)
        {
            Debug.LogError("Filter parent reference not set!");
            return;
        }

        // This parent is above the game window in hierarchy (for render order)
        m_filter.transform.SetParent(m_filterParent.transform);
    }

    override public void DragWindow(Vector2 delta)
    {
        base.DragWindow(delta);
        DragRectTransform(m_filter.GetComponent<RectTransform>(), delta);
    }

    override public void ResizeWindow(Vector2 corner, Vector2 delta)
    {
        base.ResizeWindow(corner, delta);
        ResizeRectTransform(m_filter.GetComponent<RectTransform>(), corner, delta);
    }
}