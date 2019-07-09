using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VISPanel : MonoBehaviour
{
    [SerializeField]
    GameObject angleText;  //must be the exact gameobject holding the angle label used in CanvasScript

    //Button:Reset.OnClick()
    public void Reset()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    //Button:Exit.OnClick()
    public void Exit()
    {
        Application.Quit(); 
    }

    //Button:ToggleAnswers.OnClick()
    public void ShowAnswers()
    {
        if (angleText.activeSelf == true)
            angleText.SetActive(false);
        else
            angleText.SetActive(true);
    }
}
