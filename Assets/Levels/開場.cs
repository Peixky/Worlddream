using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Flower;

public class Pre_Introduction_Control : MonoBehaviour
{
    public FlowerSystem flowerSystem;  // 使用 public
    public const string defaultSceneName = "default";  // 使用 public const 

    // 初始化 FlowerSystem 並設置指令
    void Start()
    {
        InitializeFlowerSystem();
    }

    // 每次按鍵時進行對話進行
    void Update()
    {
        HandleInput();
    }

    // 初始化 FlowerSystem 並設置必要的對話和指令
    public void InitializeFlowerSystem()
    {
        flowerSystem = FlowerManager.Instance.CreateFlowerSystem(defaultSceneName, false);
        flowerSystem.SetupDialog();
        flowerSystem.ReadTextFromResource("Pre_Introduction");
        /*
        flowerSystem.AddDialog("你好，歡迎來到我的村莊");
        */
        RegisterCommands();
    }

    // 註冊指令，這些指令會在對話中觸發
    public void RegisterCommands()
    {
        flowerSystem.RegisterCommand("Load_Scene", (List<string> _params) => LoadScene(_params[0]));
        flowerSystem.RegisterCommand("Lock_Everything", (List<string> _params) => LockPlayer(true));
        flowerSystem.RegisterCommand("Unlock_Everything", (List<string> _params) => LockPlayer(false));
        flowerSystem.RegisterCommand("Start_Introduction", (List<string> _params) => StartIntroduction(_params[0]));
        flowerSystem.RegisterCommand("End_Introduction", (List<string> _params) => EndIntroduction());
        flowerSystem.RegisterCommand("Start_Dad", (List<string> _params) => StartDad());
        flowerSystem.RegisterCommand("Final", (List<string> _params) => ShowFinalHint());
    }

    // 處理用戶輸入，顯示下一段對話
    public void HandleInput()
    {
        if (Input.anyKeyDown)
        {
            flowerSystem.Next();
        }
    }

    // 加載場景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 鎖定或解鎖玩家
    public void LockPlayer(bool isLocked)
    {
        Player_Control.is_lock = isLocked;
    }

    // 開始介紹
    public void StartIntroduction(string introObjectName)
    {
        GameObject.Find(introObjectName).GetComponent<Intro_System_Control>().intro.SetActive(true);
    }

    // 結束介紹
    public void EndIntroduction()
    {
        Intro_System_Control.show_intro = true;
    }

    /*這行可以改成別的事件或是不加入
    public void StartDad()
    {
        player1_dad.timer_dad = 5;
        Camera.main.transform.position = new Vector3(-70.0f, -22.86f, -10.0f);
    }
*/
    // 顯示最終提示
    public void ShowFinalHint()
    {
        GameObject.Find("System").GetComponent<Final_Hint>().final_hint.SetActive(true);
    }
}
