using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour
{
    public void monster_option1()
    {
        SceneManager.LoadSceneAsync("Monster");
    }
}
