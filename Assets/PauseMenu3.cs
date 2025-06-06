using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
#if UNITY_EDITOR
using UnityEditor;
#endif*/
public class PauseMenu3 : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Home()
    {
        SceneManager.LoadScene("test-Menu");
        Time.timeScale = 1;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    
    
    
    public void Skip()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;

        SceneManager.LoadSceneAsync("End Scene");
    }

   /* public void Exit()
      {
          Time.timeScale = 1;
          Application.Quit();

      #if UNITY_EDITOR

          EditorApplication.isPlaying = false;
      #endif
      }*/

}
