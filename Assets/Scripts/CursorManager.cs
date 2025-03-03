using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D m_cursorNormal;
    [SerializeField] private Texture2D m_cursorResize;

    // Start is called before the first frame update
    void Start()
    {
        SetCursorNormal();
    }

    public void SetCursorResize()
    {
        Cursor.SetCursor(m_cursorResize, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void SetCursorNormal()
    {
        Cursor.SetCursor(m_cursorNormal, Vector2.zero, CursorMode.ForceSoftware);
    }
}
