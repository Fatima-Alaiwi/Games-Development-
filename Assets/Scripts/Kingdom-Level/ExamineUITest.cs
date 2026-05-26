using UnityEngine;
using UnityEngine.UI;

public class PosterTest : MonoBehaviour
{
    public GameObject examineUI;
    public Image posterDisplay;
    public Sprite posterImage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            examineUI.SetActive(!examineUI.activeSelf);
            posterDisplay.sprite = posterImage;
            Debug.Log("Toggled ExamineUI: " + examineUI.activeSelf);
        }
    }
}