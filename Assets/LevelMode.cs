using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelMode : MonoBehaviour
{
    public void levelchoose()
    {
        SceneManager.LoadSceneAsync("Levelselect");
    }
}
