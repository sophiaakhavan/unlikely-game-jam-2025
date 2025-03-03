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
    public GameObject windowPrefab;
    [SerializeField] private DialogueRunner mainDialogueRunner;
    [SerializeField] private DialogueRunner thoughtDialogueRunner;
    [SerializeField] private List<ColorTextPair> thoughtTextObjectsList;
    [SerializeField] private GameObject windowsParent;
    [SerializeField] private GameObject mainWindow;
    private Dictionary<string, GameObject> thoughtTextObjects; //string: color, val: text object

    void Start()
    {
        //dialogueRunner = FindObjectOfType<DialogueRunner>();
        mainDialogueRunner.AddCommandHandler<int>("UpdateThought", UpdateThoughtBubble);
        mainDialogueRunner.AddCommandHandler<string>("CreateNewWindow", CreateNewWindow);
        mainDialogueRunner.AddCommandHandler<int>("SetMainWindowSize", SetMainWindowSize);
        thoughtDialogueRunner.AddCommandHandler<string, string, string, string, string, string, string>("SetThoughtLines", SetThoughtLines);

        thoughtTextObjects = new Dictionary<string, GameObject>();

        // Populate the thoughtTextObjects dictionary
        foreach(ColorTextPair pair in thoughtTextObjectsList)
        {
            thoughtTextObjects[pair.color] = pair.textObject;
        }
    }

    private void CreateNewWindow(string color)
    {
        if(windowsParent == null)
        {
            Debug.LogError("Windows parent not assigned to Dialogue Controller! Failed to add new window.");
            return;
        }
        if (windowPrefab == null)
        {
            Debug.LogError("Window prefab not assigned to Dialogue Controller! Failed to add new window.");
            return;
        }

        GameObject newWindow = Instantiate(windowPrefab, windowsParent.transform);
        newWindow.transform.Find("Filter").GetComponent<Image>().material = Resources.Load<Material>("M_Mask_" + color);
        
    }

    private void SetMainWindowSize(int size)
    {
        if(mainWindow == null)
        {
            Debug.LogError("Main window not set! Failed to change main window size");
            return;
        }
        RectTransform mainWindowRect = mainWindow.GetComponent<RectTransform>();
        mainWindowRect.transform.localScale = new Vector3(size, size, mainWindowRect.transform.localScale.z);
    }

    /// <summary>
    /// Have thought dialogue jump to a node. Called from main dialogue
    /// </summary>
    /// <param name="node"></param>
    private void UpdateThoughtBubble(int node)
    {
        thoughtDialogueRunner.Stop();
        thoughtDialogueRunner.StartDialogue("Thought" + node);
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

            if (thoughtTextObjects.ContainsKey(filterColor))
            {
                //Disable line texts that aren't specified
                if (string.IsNullOrEmpty(line))
                {
                    thoughtTextObjects[filterColor].SetActive(false);
                }
                //Set the new text for this color text
                else
                {
                    thoughtTextObjects[filterColor].SetActive(true);
                    thoughtTextObjects[filterColor].GetComponent<TMPro.TextMeshProUGUI>().text = line;
                }
            }
        }
    }

}
