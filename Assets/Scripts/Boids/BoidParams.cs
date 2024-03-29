using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidParams : MonoBehaviour
{
    [Range(0f, 5f)] public float separations;
    [Range(0f, 1f)] public float alignment;
    [Range(0, 1f)] public float cohesion;
    [Range(0, 1f)] public float obstacles;
    [Range(5f, 15f)] public float LOS;

    [Range(0f, 1f)] public float FollowLeader;
    [Range(0f, 1f)] public float DistanceBehindLeader;

    public BoidTeam team;
}
