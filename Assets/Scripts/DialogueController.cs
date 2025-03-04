using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

[System.Serializable]
public struct ColorTextPair
{
    public string color; // filter color (ie "Red", "Blue")
    public GameObject textObject; // corresponding text object
}

/// <summary>
/// Manage the dialogue runners and the custom commands
/// </summary>
public class DialogueController : MonoBehaviour
{
    [SerializeField] private GameObject m_windowPrefab;
    [SerializeField] private DialogueRunner m_mainDialogueRunner;
    [SerializeField] private DialogueRunner m_thoughtDialogueRunner;
    [SerializeField] private List<ColorTextPair> m_thoughtTextObjectsList;
    [SerializeField] private GameObject m_windowsParent;
    [SerializeField] private GameObject m_mainWindow;
    [SerializeField] private AudioClip m_continueAudioClip;
    private Dictionary<string, GameObject> m_thoughtTextObjects; //string: color, val: text object
    private AudioSource m_audioSource;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        if(m_audioSource == null)
        {
            Debug.LogError("Audio source not found on Dialogue Controller!");
        }

        //dialogueRunner = FindObjectOfType<DialogueRunner>();
        m_mainDialogueRunner.AddCommandHandler<int>("UpdateThought", UpdateThoughtBubble);
        m_mainDialogueRunner.AddCommandHandler<string>("CreateNewWindow", CreateNewWindow);
        m_mainDialogueRunner.AddCommandHandler<int>("SetMainWindowSize", SetMainWindowSize);
        m_thoughtDialogueRunner.AddCommandHandler<string, string, string, string, string, string, string>("SetThoughtLines", SetThoughtLines);

        m_thoughtTextObjects = new Dictionary<string, GameObject>();

        // Populate the thoughtTextObjects dictionary
        foreach(ColorTextPair pair in m_thoughtTextObjectsList)
        {
            m_thoughtTextObjects[pair.color] = pair.textObject;
        }
    }

    public void OnContinue()
    {
        m_audioSource.Stop();
        m_audioSource.clip = m_continueAudioClip;
        m_audioSource.Play();
    }

    private void CreateNewWindow(string color)
    {
        if(m_windowsParent == null)
        {
            Debug.LogError("Windows parent not assigned to Dialogue Controller! Failed to add new window.");
            return;
        }
        if (m_windowPrefab == null)
        {
            Debug.LogError("Window prefab not assigned to Dialogue Controller! Failed to add new window.");
            return;
        }

        GameObject newWindow = Instantiate(m_windowPrefab, m_windowsParent.transform);
        newWindow.transform.Find("Filter").GetComponent<Image>().material = Resources.Load<Material>("M_Mask_" + color);
        
    }

    private void SetMainWindowSize(int size)
    {
        if(m_mainWindow == null)
        {
            Debug.LogError("Main window not set! Failed to change main window size");
            return;
        }
        RectTransform mainWindowRect = m_mainWindow.GetComponent<RectTransform>();
        mainWindowRect.transform.localScale = new Vector3(size, size, mainWindowRect.transform.localScale.z);
    }

    /// <summary>
    /// Have thought dialogue jump to a node. Called from main dialogue
    /// </summary>
    /// <param name="node"></param>
    private void UpdateThoughtBubble(int node)
    {
        m_thoughtDialogueRunner.Stop();
        m_thoughtDialogueRunner.StartDialogue("Thought" + node);
    }

    /// <summary>
    /// Set the thought text and enable any of the text lines for the different colors
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

            if (m_thoughtTextObjects.ContainsKey(filterColor))
            {
                //Disable line texts that aren't specified
                if (string.IsNullOrEmpty(line))
                {
                    m_thoughtTextObjects[filterColor].SetActive(false);
                }
                //Set the new text for this color text
                else
                {
                    m_thoughtTextObjects[filterColor].SetActive(true);
                    m_thoughtTextObjects[filterColor].GetComponent<TMPro.TextMeshProUGUI>().text = line;
                }
            }
        }
    }

}
