using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{

    public GameObject IntroPanel;
    public GameObject CreditsPanel;


    [SerializeField] private GameObject _title;
    [SerializeField] private GameObject _start;
    [SerializeField] private GameObject _instructions;
    [SerializeField] private GameObject _credits;

    public void Introduction()
    {
        IntroPanel.SetActive(!IntroPanel.activeSelf);
        _title.SetActive(!_title.activeSelf);
        _start.SetActive(!_start.activeSelf);
        _instructions.SetActive(!_instructions.activeSelf);
        _credits.SetActive(!_credits.activeSelf);
    }

    public void Credits()
    {
        CreditsPanel.SetActive(!CreditsPanel.activeSelf);
        _title.SetActive(!_title.activeSelf);
        _start.SetActive(!_start.activeSelf);
        _instructions.SetActive(!_instructions.activeSelf);
        _credits.SetActive(!_credits.activeSelf);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BoidsScene");
    }

}
