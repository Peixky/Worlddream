using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Backreturn : MonoBehaviour
{
    public void backreturn()
    {
        SceneManager.LoadSceneAsync("test-LobbyScene");
    }
}
