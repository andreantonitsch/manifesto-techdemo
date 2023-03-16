using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public delegate void OnAllObjectivesCaptured(float time);
    public static event OnAllObjectivesCaptured onAllObjectivesCaptured;


    [SerializeField] private List<CapturableObjective> _objectives;

    private int _completedObjectives;

    private float _time;

    private void CapturableObjective_ChargeMaxed(CapturableObjective obj)
    {
        if (_objectives.Contains(obj))
        {
            _completedObjectives++;
        }

        if(_completedObjectives >= _objectives.Count)
        {
            Debug.Log("--------------------- ! WON ! -------------------------");
            onAllObjectivesCaptured?.Invoke(Time.time);
        }

    }

    private void OnEnable()
    {
        CapturableObjective.ChargeMaxed += CapturableObjective_ChargeMaxed;
    }

    
    private void OnDisable()
    {
        CapturableObjective.ChargeMaxed -= CapturableObjective_ChargeMaxed;
    }
}
