using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Homemenu : MonoBehaviour

{
    public void Home_option3()
    {
        SceneManager.LoadSceneAsync("test-Menu");
    }
}