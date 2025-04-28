using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("目标场景名称")]
    [Tooltip("要传送到的场景名称（要和 Build Settings 里的名字一致）")]
    public string targetSceneName;

    [Header("出生点编号")]
    [Tooltip("传送到目标场景时，玩家出现的出生点编号")]
    public int spawnPointNumber = 0;

    [Header("过关音乐 (Jingle)")]
    [Tooltip("触发传送前播放的音乐片段")]
    public AudioClip passMusic;

    private bool isTransitioning = false;

    void Start()
    {
        // 确保这个物体打上 Portal 标签
        gameObject.tag = "Portal";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只有玩家碰到，并且还没在过场中，才启动过场协程
        if (!isTransitioning && other.CompareTag("Player"))
        {
            StartCoroutine(PlayJingleAndChangeScene());
        }
    }

    private IEnumerator PlayJingleAndChangeScene()
    {
        isTransitioning = true;

        // 如果有指定音乐，就先播放
        if (passMusic != null)
        {
            // 创建一个临时音源来播放，且不随场景销毁
            GameObject go = new GameObject("JinglePlayer");
            AudioSource src = go.AddComponent<AudioSource>();
            src.clip = passMusic;
            src.playOnAwake = false;
            src.Play();

            DontDestroyOnLoad(go);
            yield return new WaitForSeconds(passMusic.length);

            Destroy(go);
        }

        // 最后调用你的场景切换逻辑
        if (Game_Manager.instance != null)
        {
            Game_Manager.instance.Change_Scene(targetSceneName, spawnPointNumber);
        }
        else
        {
            // 作为备用，直接切场景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}

/*1. 把你的音樂檔放進 Unity
把你的 .mp3 或 .wav 檔案拖進 Unity 的 Assets 資料夾。（不是丟網址！是音樂檔案）

建議用 .wav 格式，載入比較快；.mp3 也可以。

例如拖進去後，會看到一個 PassMusic.wav 出現在 Assets 裡面。

2. 設定到 Portal 物件上
在場景(Scene)裡，選到你的 Portal（傳送門）物件。

在 Inspector 裡會看到 Portal (Script) 這個欄位。

在 Pass Music 欄位旁邊會是空的，你只要把剛剛匯入的音樂檔 直接拉進去 就完成了。
加入音樂的方法*/