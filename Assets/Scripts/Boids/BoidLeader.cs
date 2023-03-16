using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidLeader : BoidAgent
{

    [field: SerializeField]
    public Vector2 velocity { get; private set; }
    private Vector3 _lastPos;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        _lastPos = transform.position;
    }
    
    new void FixedUpdate()
    {

    }

    // Update is called once per frame
    new void Update()
    {
        var delta = transform.position - _lastPos;
        velocity = new Vector2(delta.x, delta.z);

        _lastPos = transform.position;

    }
}
