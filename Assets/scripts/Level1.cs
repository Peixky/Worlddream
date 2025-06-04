using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1 : MonoBehaviour
{
    public void level_option1()
    {
        SceneManager.LoadSceneAsync("Scene1");
    }
}

