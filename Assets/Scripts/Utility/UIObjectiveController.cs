using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObjectiveController : MonoBehaviour
{
    public CapturableObjective Objective;
    public float ObjectiveMaxCharge;

    public Image img;

    public void ChangeFill(float charge)
    {
        img.fillAmount = charge / ObjectiveMaxCharge;
    }

    public void Start()
    {
        ObjectiveMaxCharge = Objective.MaxCharge;
        Objective.ChargeChanged += ChangeFill;
    }

}
