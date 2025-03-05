using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

[System.Serializable]
public struct ColorTextPair
{
    public string color;            // filter color (ie "Red", "Blue")
    public GameObject textObject;   // corresponding text object
}

[System.Serializable]
public struct ColorWindowPair
{
    public string color;            // filter color (ie "Red", "Blue")
    public GameObject window;       // corresponding filter window object
}

/// <summary>
/// Manage the dialogue runners and the custom commands
/// </summary>
public class DialogueController : MonoBehaviour
{
    [SerializeField] private List<ColorWindowPair> m_filterWindows;
    [SerializeField] private List<ColorTextPair> m_thoughtTextObjs;

    private Dictionary<string, GameObject> m_filterWindowDict;      // string: color, val: filter window obj
    private Dictionary<string, GameObject> m_thoughtTextObjDict;    // string: color, val: text object

    [SerializeField] private AudioClip m_childAudio;
    [SerializeField] private AudioClip m_parentAudio;
    [SerializeField] private CustomLineView m_customLineView; // main dialogue system line view

    [SerializeField] private GameObject m_mainWindow;
    private Coroutine windowScaleCoroutine;

    private DialogueRunner m_dialogueRunner;

    void Start()
    {
        m_dialogueRunner = FindObjectOfType<DialogueRunner>();

        m_dialogueRunner.AddCommandHandler<string>("EnableFilter", EnableFilter);
        m_dialogueRunner.AddCommandHandler<string>("DisableFilter", DisableFilter);
        m_dialogueRunner.AddCommandHandler<string, string, string, string, string, string, string>("SetThoughtLines", SetThoughtLines);

        m_dialogueRunner.AddCommandHandler<string>("SetCharacterAudio", SetCharacterAudio);
        m_dialogueRunner.AddCommandHandler<float>("SetMainWindowSize", ScaleMainWindow);

        m_filterWindowDict = new Dictionary<string, GameObject>();
        m_thoughtTextObjDict = new Dictionary<string, GameObject>();

        // Populate the thought text objects dictionary
        foreach(ColorTextPair pair in m_thoughtTextObjs)
        {
            m_thoughtTextObjDict[pair.color] = pair.textObject;
        }

        // Poplate the filter window objects dictionary
        foreach (ColorWindowPair pair in m_filterWindows)
        {
            m_filterWindowDict[pair.color] = pair.window;
        }
    }

    private void EnableFilter(string color)
    {
        if (m_filterWindowDict.ContainsKey(color) && !m_filterWindowDict[color].activeInHierarchy)
        {
            m_filterWindowDict[color].GetComponent<FilterWindow>().EnableWindow();
            // TODO: Call resize function?
            // TODO: Set position?
        }
    }

    private void DisableFilter(string color)
    {
        if (m_filterWindowDict.ContainsKey(color) && m_filterWindowDict[color].activeInHierarchy)
        {
            m_filterWindowDict[color].GetComponent<FilterWindow>().DisableWindow();
            // TODO: Call resize function?
        }
    }

    private void SetCharacterAudio(string character)
    {
        if (m_customLineView == null)
        {
            Debug.LogError("Custom line view not found on main dialogue system! Failed to set character audio.");
            return;
        }
        if (character == "Parent")
        {
            m_customLineView.sound.clip = m_parentAudio;
        }
        else if (character == "Child")
        {
            m_customLineView.sound.clip = m_childAudio;
        }
        else if (character == "None")
        {
            m_customLineView.sound.clip = null;
        }
    }

    public void ScaleMainWindow(float scale)
    {
        if(m_mainWindow == null)
        {
            Debug.LogError("Main window not set! Failed to change main window size");
            return;
        }

        if (windowScaleCoroutine != null)
        {
            StopCoroutine(windowScaleCoroutine);
        }
        windowScaleCoroutine = StartCoroutine(LerpWindowScale(scale));
    }

    private IEnumerator LerpWindowScale(float scale)
    {
        RectTransform mainWindowRect = m_mainWindow.GetComponent<RectTransform>();
        Vector3 initialScale = mainWindowRect.localScale;
        Vector3 targetScale = new Vector3(scale, scale, initialScale.z);

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            mainWindowRect.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        mainWindowRect.localScale = targetScale;
        windowScaleCoroutine = null; // Reset coroutine
    }

    /// <summary>
    /// Set the thought text for the different colors.
    /// </summary>
    /// <param name="redLine"></param>
    /// <param name="blueLine"></param>
    /// <param name="greenLine"></param>
    /// <param name="magentaLine"></param>
    /// <param name="cyanLine"></param>
    /// <param name="yellowLine"></param>
    /// <param name="whiteLine"></param>
    private void SetThoughtLines(string redLine = "", string blueLine = "", string greenLine = "", string magentaLine = "", string cyanLine = "", string yellowLine = "", string whiteLine = "")
    {
        Dictionary<string, string> filterLines = new Dictionary<string, string>
        {
            { "Red", redLine },
            { "Blue", blueLine },
            { "Green", greenLine },
            { "Magenta", magentaLine },
            { "Cyan", cyanLine },
            { "Yellow", yellowLine },
            { "White", whiteLine }
        };
        
        foreach (var filterLine in filterLines)
        {
            string filterColor = filterLine.Key;
            string line = filterLine.Value;

            if (m_thoughtTextObjDict.ContainsKey(filterColor))
            {
                // Line texts that aren't specified should default to "..."
                if (string.IsNullOrEmpty(line))
                {
                    m_thoughtTextObjDict[filterColor].GetComponent<TMPro.TextMeshProUGUI>().text = "...";
                }
                // Set the new text for this color text
                else
                {
                    m_thoughtTextObjDict[filterColor].GetComponent<TMPro.TextMeshProUGUI>().text = line;
                }
            }
        }
    }
}
