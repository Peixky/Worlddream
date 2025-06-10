using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 元素引用")]
    public Image backgroundImageUI; 
    public GameObject dialoguePanelObject; 
    public TextMeshProUGUI dialogueTextUI; 
    [Header("劇情內容")]
    public List<Sprite> backgrounds; 
    public List<string> dialogues; 

    [Header("劇情結束後動作")]
    public DialogueEndAction dialogueEndAction;
    public enum DialogueEndAction
    {
        LoadNextGameScene,              
        LoadLobbyScene,                 
        LoadNextStoryScene,            
        LoadStoreScene,               
        EndGame                      
    }

    private int currentDialogueIndex = 0;

    void Start()
    {
        if (backgroundImageUI == null || dialoguePanelObject == null || dialogueTextUI == null)
        {
            Debug.LogError("DialogueManager: UI 元素未完整設定！請檢查 Inspector 中的引用。", this);
            enabled = false;
            return;
        }
        if (backgrounds.Count == 0 || dialogues.Count == 0 || backgrounds.Count != dialogues.Count)
        {
            Debug.LogError("DialogueManager: 劇情內容未完整設定或數量不一致！請在 Inspector 中設定背景圖片和對話內容。", this);
            enabled = false;
            return;
        }

        backgroundImageUI.gameObject.SetActive(true);
        dialoguePanelObject.SetActive(true);

        UpdateDialogueContent();
        Debug.Log($"DialogueManager: 開始顯示劇情，當前索引 {currentDialogueIndex}。");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentDialogueIndex++;

            if (currentDialogueIndex < backgrounds.Count)
            {
                UpdateDialogueContent();
            }
            else
            {
                Debug.Log("DialogueManager: 所有對話結束！");
                EndDialogue();
            }
        }
    }

    void UpdateDialogueContent()
    {
        backgroundImageUI.sprite = backgrounds[currentDialogueIndex];
        dialogueTextUI.text = dialogues[currentDialogueIndex];

        Debug.Log($"DialogueManager: 顯示第 {currentDialogueIndex + 1} 步劇情。");
    }

    void EndDialogue()
    {
        backgroundImageUI.gameObject.SetActive(false);
        dialoguePanelObject.SetActive(false);
        enabled = false;

        switch (dialogueEndAction)
        {
            case DialogueEndAction.LoadNextGameScene:
                Debug.Log("DialogueManager: 劇情結束，加載下一關遊戲 Scene (不推進索引)。");
                GameProgressionManager.LoadNextGameScene(); 
                break;
            case DialogueEndAction.LoadLobbyScene:
                Debug.Log("DialogueManager: 劇情結束，加載大廳 Scene。");
                GameProgressionManager.LoadLobbyScene(); 
                break;
            case DialogueEndAction.LoadNextStoryScene:
                Debug.Log("DialogueManager: 劇情結束，加載下一段劇情 Scene (不推進索引)。");
                GameProgressionManager.AdvanceStory(); 
                GameProgressionManager.LoadNextStoryScene(); 
                break;
            case DialogueEndAction.LoadStoreScene:
                Debug.Log("DialogueManager: 劇情結束，加載商店 Scene。");
                GameProgressionManager.LoadStoreScene();
                break;
            case DialogueEndAction.EndGame:
                Debug.Log("DialogueManager: 遊戲已全部結束！加載結局 Scene。");
                SceneManager.LoadScene("End Scene");
                //GameProgressionManager.ResetProgress();
                break;
        }
    }
}