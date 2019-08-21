using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

public class VISPanel : MonoBehaviour
{
    [SerializeField]
    GameObject angleText, instructionLabel;  //must be the exact gameobject holding the angle label used in CanvasScript

    [SerializeField]
    MLInputModule _inputModule = null;

    [SerializeField]
    LineRenderer _beam = null;

    [SerializeField]
    Button angleButton; 

    [SerializeField]
    Text _angle, _instruct;

    //Button:Reset.OnClick()
    public void Reset()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
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
        {
            angleText.SetActive(false);
            _angle.text = "Answers: Off";
        }
        else
        {
            angleText.SetActive(true);
            _angle.text = "Answers: On";
        }
    }

    //Button:ToggleInstruction.OnClick()
    public void ShowInstructions()
    {
        if (instructionLabel.activeSelf == true)
        {
            instructionLabel.SetActive(false);
            _instruct.text = "Instructions: Off";
        }
        else
        {
            instructionLabel.SetActive(true);
            _instruct.text = "Instructions: On";
        }
    }

    void Update()
    {
        if (_inputModule.PointerLineSegment.End.HasValue)
        {
            _beam.enabled = (_inputModule.PointerInput == MLInputModule.PointerInputType.Controller);
            _beam.SetPosition(0, _inputModule.PointerLineSegment.Start);
            _beam.SetPosition(1, _inputModule.PointerLineSegment.End.Value);
        }
        else
        {
            _beam.enabled = false;
        }
    }
}
