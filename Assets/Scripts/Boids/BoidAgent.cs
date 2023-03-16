using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BoidTeam
{
    NeutralCrowd,
    ForChangeRiot,
    StatusQuoRiot,
    Police
}

[RequireComponent(typeof(Rigidbody))]
public class BoidAgent : MonoBehaviour
{
    public BoidParams boid_params;

    public HashSet<BoidAgent> TeamNeighboors;
    public HashSet<BoidAgent> EnemyNeighboors;

    public int neighVis;
    public int EnemyNeighVis;

    //obstacle list
    public HashSet<Collider> obstacles;

    public SphereCollider Radius_collider;
    public Rigidbody rb { get; private set; }

    public float vMax;

    public BoidLeader Leader;

    private const float _lostLeaderTimerValue = 1f;
    private const float _lostLeaderBuffer = 10f;

    private float _lostLeaderTimer = _lostLeaderTimerValue;
    private bool _isLeaderLost = false;


    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        TeamNeighboors = new HashSet<BoidAgent>();
        EnemyNeighboors = new HashSet<BoidAgent>();
        obstacles = new HashSet<Collider>();


    }

    protected void Update()
    {
        Radius_collider.radius = boid_params.LOS;
        EnemyNeighVis = EnemyNeighboors.Count;
        neighVis = TeamNeighboors.Count;

        if (Leader && boid_params.team == BoidTeam.ForChangeRiot)
        {
            IsLeaderLost();

            HandleLeaderLost();
        }

    }

    private void IsLeaderLost()
    {
        //if (!Leader) return;

        if (EnemyNeighboors.Count > TeamNeighboors.Count)
        {
            _isLeaderLost = true;
            //_lostLeaderTimer = _lostLeaderTimerValue;
        }
        else
        {
            _isLeaderLost = false;
            _lostLeaderTimer = _lostLeaderTimerValue;
        }

    }
    private void HandleLeaderLost()
    {
        if (_isLeaderLost)
        {
            _lostLeaderTimer -= Time.deltaTime;

            if (_lostLeaderTimer <= 0f)
            {
                StartCoroutine(ResetLeaderLost());
                Leader = null;

            }
        }
    }

    private IEnumerator ResetLeaderLost()
    {
        yield return new WaitForSeconds(_lostLeaderBuffer);

        _lostLeaderTimer = _lostLeaderTimerValue;
        _isLeaderLost = false;

    }

    // Update is called once per frame
    protected void FixedUpdate()
    {

        BoidMove(Time.fixedDeltaTime);
    }

    // detected another boid entering neighbour radius
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Leader"))
        {
            if (!Leader)
            {
                var boid = other.GetComponent<BoidAgent>();

                if (boid.boid_params.team != boid_params.team)
                    return;

                //_isLeaderLost = false;
                //_lostLeaderTimer = _lostLeaderTimerValue;
                if (!_isLeaderLost)
                {
                    Leader = other.GetComponent<BoidLeader>();
                    boid_params = Leader.boid_params;
                }
            }
        }

        if (other.CompareTag("Boid"))
        {
            var boid = other.GetComponent<BoidAgent>();
            if (boid.boid_params.team != boid_params.team)
            {
                EnemyNeighboors.Add(boid);
            }
            else
            {
                if (TeamNeighboors.Count < 100)
                    TeamNeighboors.Add(boid);
            }
        }

        if (other.CompareTag("Obstacle"))
            obstacles.Add(other);
    }

    // detected another boid leaving neighbour radius
    private void OnTriggerExit(Collider other)
    {


        if (other.CompareTag("Boid"))
        {
            var boid = other.GetComponent<BoidAgent>();

            if (boid.boid_params.team != boid_params.team)
            {
                EnemyNeighboors.Remove(boid);
            }
            else
                TeamNeighboors.Remove(boid);
        }

        if (other.CompareTag("Obstacle"))
            obstacles.Remove(other);
    }




    public virtual void BoidMove(float rate)
    {

        Vector2 separation = CalculateSeparation() * boid_params.separations;
        Vector2 cohesion_displacement = CalculateDisplacement() * boid_params.cohesion;
        Vector2 alignment = CalculateAlignment() * boid_params.alignment;

        Vector2 follow = Leader ? FollowLeader() * boid_params.FollowLeader : Vector2.zero;
        Vector2 obst = AvoidObstacles();

        Vector2 others = boid_params.team != BoidTeam.Police ? CalculateSeparationOtherTeams() : Vector2.zero;


        Vector2 velocity = separation + cohesion_displacement + alignment + follow + (others * 25f) /*+ obst * (boid_params.obstacles + follow.magnitude)*/;

        var v = new Vector3(velocity.x, 0f, velocity.y);

        if (rb.velocity.sqrMagnitude > vMax * vMax)
        {
            rb.velocity *= (vMax * vMax) / rb.velocity.sqrMagnitude;
        }

        rb.velocity = rb.velocity + v * rate;
        //rb.AddForce(rb.velocity + v/* * rate*/);


    }

    public virtual Vector2 CalculateSeparation()
    {
        Vector2 s = Vector2.zero;

        foreach (var neigh in TeamNeighboors)
        {
            var t = rb.position - neigh.rb.position;

            t.y = 0f;

            var tm = t.magnitude > 0 ? t.magnitude : 1f;

            s += new Vector2(t.x, t.z).normalized * 1f / tm;

            //s -= new Vector2(t.x, t.z);
        }

        return s;
    }


    public virtual Vector2 CalculateSeparationOtherTeams()
    {
        Vector2 s = Vector2.zero;

        foreach (var neigh in EnemyNeighboors)
        {

            var t = rb.position - neigh.rb.position;

            t.y = 0f;

            var tm = t.magnitude > 0 ? t.magnitude : 1f;

            s += new Vector2(t.x, t.z).normalized * 1f / tm;

            //s -= new Vector2(t.x, t.z);
        }

        return s;
    }



    public virtual Vector2 CalculateDisplacement()
    {

        Vector2 c = Vector2.zero;

        foreach (var neigh in TeamNeighboors)
        {
            var t = neigh.rb.position;

            c += new Vector2(t.x, t.z);
        }
        var t2 = rb.position;
        return TeamNeighboors.Count != 0 ? ((c / TeamNeighboors.Count) - new Vector2(t2.x, t2.z)) : Vector2.zero;
    }

    public virtual Vector2 CalculateAlignment()
    {
        var m = Vector2.zero;

        foreach (var neigh in TeamNeighboors)
        {
            var t = new Vector2(neigh.rb.velocity.x, neigh.rb.velocity.z);
            m += t;
        }

        return TeamNeighboors.Count != 0 ? (m / TeamNeighboors.Count) : Vector2.zero;
    }

    public virtual Vector2 FollowLeader()
    {
        var tv = Leader.rb.position - rb.position;
        var behind = tv * -1f;
        behind = behind.normalized;

        behind *= boid_params.DistanceBehindLeader;



        return new Vector2(tv.x, tv.z) - new Vector2(behind.x, behind.z);

    }

    public virtual Vector2 AvoidObstacles()
    {
        Vector2 s = Vector2.zero;

        foreach (var obst in obstacles)
        {
            Physics.Raycast(rb.position, obst.transform.position, out RaycastHit hit);
            var t = rb.position - hit.point;

            var tm = t.magnitude > 0 ? t.magnitude : 1f;

            s += new Vector2(t.x, t.z)/*.normalized * 1f / tm*/;
        }

        //var x = s.x > 0 ? 1 / s.x : 0f;
        //var y = s.y > 0 ? 1 / s.y : 0f;

        return s;
    }


    public void SetParams(BoidParams boidparams)
    {
        boid_params = boidparams;
    }

}

