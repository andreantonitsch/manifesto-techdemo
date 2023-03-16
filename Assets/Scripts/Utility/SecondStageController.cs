using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SecondStageController : MonoBehaviour
{

    public TMPro.TMP_Text text;
    public GameObject text_panel;

    public void Win(float time)
    {
        text.text = $"Congratulations for LIBERATING this CITY!.\nClick HERE to protest against\nthe oppression you faced before.\n\nYou took {time} seconds.\n\nHopefully this helped inspire you to change your ACTUAL life.\nTHANKS FOR PLAYING.";
        text_panel.SetActive(true);
    }

    public void NextStage()
    {
        SceneManager.LoadScene("StartScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.onAllObjectivesCaptured += Win;
    }

}
