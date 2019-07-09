using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class ResetApplication : MonoBehaviour
{
    public void Reset()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
