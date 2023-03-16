using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FirstStageController : MonoBehaviour
{

    public TMPro.TMP_Text text;
    public GameObject text_panel;

    public void Win(float time)
    {
        text.text = $"Congratulations for LIBERATING this playground.\nClick HERE to protest against\nEVEN GREATER OPPRESSORS.\n\nYou took {time} seconds.";
        text_panel.SetActive(true);
    }

    public void NextStage()
    {
        SceneManager.LoadScene("02");
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.onAllObjectivesCaptured += Win;
    }

}
