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
