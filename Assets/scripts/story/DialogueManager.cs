using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class DialogueManager : MonoBehaviour
{
    [Header("UI 元素引用")]
    public Image backgroundImageUI; // 拖曳 DialogueBackground Image 到這裡
    public GameObject dialoguePanelObject; // 拖曳 DialoguePanel 到這裡
    public TextMeshProUGUI dialogueTextUI; // 拖曳 DialogueText (TextMeshPro) 到這裡

    [Header("劇情內容")]
    public List<Sprite> backgrounds; // 拖曳所有背景圖片 Sprite (依序) 到這裡
    public List<string> dialogues; // 填寫所有對話內容 (依序) 到這裡

    [Header("流程設定")]
    public int nextSceneIndex = -1; // 劇情結束後要跳轉的 Scene 索引 (如果沒有，設為 -1)

    private int currentDialogueIndex = 0; // 當前對話步驟的索引

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

        // 確保劇情 UI 初始是顯示的 (在遊戲開始時會被激活)
        backgroundImageUI.gameObject.SetActive(true);
        dialoguePanelObject.SetActive(true);

        // 顯示第一步劇情
        UpdateDialogueContent();
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
                Debug.Log("DialogueManager: 劇情結束！");
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

        Debug.Log("DialogueManager: 顯示第 " + (currentDialogueIndex + 1) + " 步劇情。");
    }

    void EndDialogue()
    {
        // 隱藏所有劇情 UI
        backgroundImageUI.gameObject.SetActive(false);
        dialoguePanelObject.SetActive(false);
        enabled = false; // 禁用對話管理器

        // 如果設定了下一個 Scene，則加載它
        if (nextSceneIndex != -1)
        {
            Debug.Log("DialogueManager: 加載下一個場景，索引: " + nextSceneIndex);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("DialogueManager: 劇情結束，但未設定下一個 Scene 索引。請在 Inspector 中設定 nextSceneIndex。", this);
            // 如果沒有下一個 Scene，這裡可以處理遊戲結束或回到主選單
        }
    }
}