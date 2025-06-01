using UnityEngine;
using UnityEngine.UI; // 使用 UI 元件 (Image, Text)
using TMPro; // 使用 TextMeshPro 文本
using System.Collections.Generic; // 使用 List
using UnityEngine.SceneManagement; // 使用 SceneManager

public class DialogueManager : MonoBehaviour
{
    [Header("UI 元素引用")]
    public Image backgroundImageUI; // 拖曳 DialogueBackground Image 到這裡
    public GameObject dialoguePanelObject; // 拖曳 DialoguePanel 到這裡
    public TextMeshProUGUI dialogueTextUI; // 拖曳 DialogueText (TextMeshPro) 到這裡

    [Header("劇情內容")]
    public List<Sprite> backgrounds; // 拖曳所有背景圖片 Sprite (依序) 到這裡
    public List<string> dialogues; // 填寫所有對話內容 (依序) 到這裡

    [Header("劇情結束後動作")]
    // 這個枚舉讓您在 Inspector 中選擇劇情結束後要執行的動作
    public DialogueEndAction dialogueEndAction; 
    public enum DialogueEndAction {
        LoadNextGameScene,              // 劇情結束後加載當前索引的遊戲 Scene (不推進索引)
        LoadLobbyScene,                 // 劇情結束後加載大廳 Scene
        LoadNextStoryScene,             // 劇情結束後加載當前索引的劇情 Scene (不推進索引)
        AdvanceLevelAndLoadNextGameScene, // <<<< 新增這個選項！先推進關卡索引再載入下一關遊戲 Scene >>>>
        EndGame             // 劇情結束後遊戲結束 (例如劇情四之後)
    }

    private int currentDialogueIndex = 0; // 當前對話步驟的索引

    void Start()
    {
        // 檢查是否所有必要的 UI 引用都已設定
        if (backgroundImageUI == null || dialoguePanelObject == null || dialogueTextUI == null)
        {
            Debug.LogError("DialogueManager: UI 元素未完整設定！請檢查 Inspector 中的引用。", this);
            enabled = false; // 禁用腳本
            return;
        }
        // 檢查劇情內容是否設定正確
        if (backgrounds.Count == 0 || dialogues.Count == 0 || backgrounds.Count != dialogues.Count)
        {
            Debug.LogError("DialogueManager: 劇情內容未完整設定或數量不一致！請在 Inspector 中設定背景圖片和對話內容。", this);
            enabled = false; // 禁用腳本
            return;
        }

        // 確保劇情 UI 初始是顯示的 (在遊戲開始時會被激活)
        backgroundImageUI.gameObject.SetActive(true);
        dialoguePanelObject.SetActive(true);

        // 顯示第一步劇情
        UpdateDialogueContent();
        Debug.Log($"DialogueManager: 開始顯示劇情，當前索引 {currentDialogueIndex}。");
    }

    void Update()
    {
        // 偵測滑鼠左鍵點擊，推進劇情
        if (Input.GetMouseButtonDown(0))
        {
            currentDialogueIndex++; 

            if (currentDialogueIndex < backgrounds.Count)
            {
                // 還有劇情，更新內容
                UpdateDialogueContent();
            }
            else
            {
                // 劇情結束
                Debug.Log("DialogueManager: 所有對話結束！");
                EndDialogue();
            }
        }
    }

    void UpdateDialogueContent()
    {
        // 更新背景圖片
        backgroundImageUI.sprite = backgrounds[currentDialogueIndex];
        // 更新對話文字
        dialogueTextUI.text = dialogues[currentDialogueIndex];

        Debug.Log($"DialogueManager: 顯示第 {currentDialogueIndex + 1} 步劇情。");
    }

    void EndDialogue()
    {
        // 隱藏所有劇情 UI
        backgroundImageUI.gameObject.SetActive(false);
        dialoguePanelObject.SetActive(false);
        enabled = false; // 禁用對話管理器，防止重複觸發

        // <<<< 關鍵修改點：根據設定的動作類型，呼叫 GameProgressionManager 執行下一步 >>>>>>
        switch (dialogueEndAction)
        {
            case DialogueEndAction.LoadNextGameScene:
                Debug.Log("DialogueManager: 劇情結束，加載當前遊戲 Scene。");
                GameProgressionManager.LoadNextGameScene(); 
                break;
            case DialogueEndAction.LoadLobbyScene:
                Debug.Log("DialogueManager: 劇情結束，加載大廳 Scene。");
                GameProgressionManager.LoadLobbyScene(); 
                break;
            case DialogueEndAction.LoadNextStoryScene:
                Debug.Log("DialogueManager: 劇情結束，加載當前劇情 Scene。");
                GameProgressionManager.LoadNextStoryScene(); 
                break;
            case DialogueEndAction.AdvanceLevelAndLoadNextGameScene: // <<<< 新增的處理邏輯 >>>>
                Debug.Log("DialogueManager: 劇情結束，推進關卡並加載下一關遊戲 Scene。");
                GameProgressionManager.AdvanceLevel(); // 先推進關卡索引
                GameProgressionManager.LoadNextGameScene(); // 再載入新的 CurrentLevelIndex 對應的遊戲場景
                break;
            case DialogueEndAction.EndGame:
                Debug.Log("DialogueManager: 遊戲已全部結束！加載結局 Scene。");
                SceneManager.LoadScene(GameProgressionManager.instance.endingSceneName);
                break;
        }
    }
}