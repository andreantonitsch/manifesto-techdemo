using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManifestoTechDemo
{
    public enum BoidTeam
    {
        NeutralCrowd,
        ForChangeRiot,
        StatusQuoRiot,
        Police
    }

    [System.Serializable]
    public struct BoidParameters
    {
        [Range(0f, 100f)] public float separation;
        [Range(0f, 1f)] public float alignment;
        [Range(0, 1f)] public float cohesion;
        [Range(0, 1f)] public float obstacles;
        [Range(5f, 15f)] public float LOS;

        [Range(0f, 1f)] public float follow;
        [Range(0f, 1f)] public float distance_behind_leader;

        public BoidTeam team;
    }
}