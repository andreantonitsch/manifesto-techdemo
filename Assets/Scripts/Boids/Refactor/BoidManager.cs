using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using static Unity.Mathematics.math;
using Unity.Jobs;
using Unity.Mathematics;

namespace ManifestoTechDemo
{

    public class BoidManager : MonoBehaviour
    {
        [SerializeField]
        public BoidParameters parameters;

        private NativeArray<float2> separation;
        private NativeArray<float2> cohesion;
        private NativeArray<float2> alignment;
        private NativeArray<float2> follow;

        private NativeArray<float2> velocity;
        private NativeArray<float2> position;

        private NativeArray<int> leader;

        // grid of cell id -> neighboring boid ids.
        private NativeMultiHashMap<int, int> grid;

        public GridDimensions grid_dimensions;
        public int2[] neigh_offset = {
            int2(-1, -1),
            int2(-1, 0),
            int2(-1, 1),

            int2(0, -1),
            int2(0, 0),
            int2(0, 1),

            int2(1, -1),
            int2(1, 0),
            int2(1, 1)};
        public NativeArray<int2> offset_array;


        public float vMax = 25f;

        public Boid[] boids;
        public int active_boids;


        // Initialize data structures.
        void Start()
        {
            separation = new NativeArray<float2>(active_boids, Allocator.Persistent);
            cohesion = new NativeArray<float2>(active_boids, Allocator.Persistent);
            alignment = new NativeArray<float2>(active_boids, Allocator.Persistent);
            follow = new NativeArray<float2>(active_boids, Allocator.Persistent);

            velocity = new NativeArray<float2>(active_boids, Allocator.Persistent);
            position = new NativeArray<float2>(active_boids, Allocator.Persistent);

            leader = new NativeArray<int>(active_boids, Allocator.Persistent);

            offset_array = new NativeArray<int2>(neigh_offset, Allocator.Persistent);

            grid = new NativeMultiHashMap<int, int>(active_boids * 18, Allocator.Persistent);


            active_boids = boids.Length;



            // Initialize data structures.
            for (int i = 0; i < active_boids; i++)
            {
                //Set everyone to follow the first element. AKA the mouse.
                leader[i] = 0;

                var p = boids[i].transform.position;
                position[i] = float2(p.x, p.z);
            }
        }

        // Clean up structures.
        private void OnApplicationQuit()
        {
            separation.Dispose();
            cohesion.Dispose();
            alignment.Dispose();
            follow.Dispose();

            velocity.Dispose();
            position.Dispose();

            leader.Dispose();

            grid.Dispose();
            offset_array.Dispose();
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 1; i < active_boids; i++)
            {
                var p = boids[i].rb.position;
                boids[i].rb.position = new Vector3(position[i].x, p.y, position[i].y);
            }

            var leader_p = boids[0].rb.position;
            position[0] = float2(leader_p.x, leader_p.z);


        }

        // Update is called once per frame
        void FixedUpdate()
        {
            ScheduleMovement();
        }


        void ScheduleMovement()
        {


            var neighboursJob = new FillNeighboursJob
            {
                grid = grid,

                position = position,

                offset = offset_array,
                boid_amount = active_boids,
                grid_dimensions = grid_dimensions
            };

            var componentsJob = new CalculateComponentsJob
            {
                alignment = alignment,
                cohesion = cohesion,
                separation = separation,
                follow = follow,

                position = position,
                velocity = velocity,
                leader = leader,
                grid = grid,
                

                grid_dimensions = grid_dimensions,
                boid_amount = active_boids,
                parameters = parameters
            };

            var velocityJob = new UpdateVelocityJob
            {
                alignment = alignment,
                cohesion = cohesion,
                follow = follow,
                separation = separation,

                velocity = velocity,

                boid_amount = active_boids,
                deltaTime = Time.fixedDeltaTime,
                parameters = parameters,
                vMax = vMax
            };

            var positionJob = new UpdatePositionJob
            {
                velocity = velocity,

                position = position,

                boid_amount = active_boids,
                deltaTime = Time.fixedDeltaTime,
                parameters = parameters,
                vMax = vMax
            };

            grid.Clear();
            var neighborFillHandle = neighboursJob.Schedule();
            var componentsHandle = componentsJob.Schedule(neighborFillHandle);
            var velocityHandle = velocityJob.Schedule(componentsHandle);
            var positionHandle = positionJob.Schedule(velocityHandle);

            positionHandle.Complete();
        }


        public struct FillNeighboursJob : IJob
        {
            [ReadOnly]
            public NativeArray<float2> position;

            [ReadOnly]
            public NativeArray<int2> offset;

            [WriteOnly]
            public NativeMultiHashMap<int, int> grid;
            public GridDimensions grid_dimensions;
            public int boid_amount;

            public void Execute()
            {
                for (int i = 0; i < boid_amount; i++)
                {
                    var cell = grid_dimensions.position_to_cell(position[i]);

                    for (int j = 0; j < 9; j++)
                    {
                        var id = grid_dimensions.cell_to_id(cell + offset[j]);
                        grid.Add(id, i);
                    }
                }
            }

        }


        public struct CalculateComponentsJob : IJob
        {
            [WriteOnly]
            public NativeArray<float2> separation;
            [WriteOnly]
            public NativeArray<float2> cohesion;
            [WriteOnly]
            public NativeArray<float2> alignment;
            [WriteOnly]
            public NativeArray<float2> follow;

            [ReadOnly]
            public NativeArray<float2> position;
            [ReadOnly]
            public NativeArray<float2> velocity;
            [ReadOnly]
            public NativeArray<int> leader;

            [ReadOnly]
            public NativeMultiHashMap<int, int> grid;

            public GridDimensions grid_dimensions;
            public int boid_amount;
            public BoidParameters parameters;

            public void Execute()
            {

                for (int i = 1; i < boid_amount; i++)
                {
                    float2 s = float2(0f, 0f);
                    float2 c = float2(0f, 0f);
                    float2 m = float2(0f, 0f);

                    var boid_p = position[i];

                    var cell_id = grid_dimensions.position_to_id(boid_p);
                    var neigh_count = 0;

                    if (grid.TryGetFirstValue(cell_id, out int neigh, out var it))
                        do
                        {
                            var neigh_id = neigh;
                            if (neigh_id == i)
                                continue;

                            var neigh_vel = velocity[neigh_id];
                            var neigh_pos = position[neigh_id];

                            var t = boid_p - neigh_pos;
                            var l = length(t);
                            var tm = l > 0f ? l : 1f;
                            s += normalize(t) / tm;

                            m += neigh_vel;

                            c += neigh_pos;

                            neigh_count++;

                        } while (grid.TryGetNextValue(out neigh, ref it));


                    var d_leader = position[leader[i]] - position[i];
                    var behind_leader = normalize(-d_leader);
                    behind_leader *= parameters.distance_behind_leader;


                    if (neigh_count != 0)
                    {
                        m /= neigh_count;
                        c = (c / neigh_count) - boid_p;
                    }

                    separation[i] = s;
                    alignment[i] = m;
                    cohesion[i] = c;

                    follow[i] = d_leader - behind_leader;

                }

            }
        }

        public struct UpdatePositionJob : IJob
        {
            [ReadOnly]
            public NativeArray<float2> velocity;

            public NativeArray<float2> position;

            public float vMax;
            public int boid_amount;
            public float deltaTime;
            public BoidParameters parameters;

            public void Execute()
            {

                for (int i = 1; i < boid_amount; i++)
                {

                    var v = velocity[i];
                    var p = position[i];

                    p += v * deltaTime;

                    position[i] = p;

                }
            }
        }

        public struct UpdateVelocityJob : IJob
        {
            [ReadOnly]
            public NativeArray<float2> separation;
            [ReadOnly]
            public NativeArray<float2> cohesion;
            [ReadOnly]
            public NativeArray<float2> alignment;
            [ReadOnly]
            public NativeArray<float2> follow;

            public NativeArray<float2> velocity;

            public float vMax;
            public int boid_amount;
            public float deltaTime;
            public BoidParameters parameters;

            public void Execute()
            {

                for (int i = 1; i < boid_amount; i++)
                {
                    var sep = separation[i] * parameters.separation;
                    var coh = cohesion[i] * parameters.cohesion;
                    var ali = alignment[i] * parameters.alignment;
                    var fol = follow[i] * parameters.follow;

                    var vel = sep /*+ coh + ali*/ + fol;

                    var v = velocity[i];
                    var vl = length(v);
                    if (vl > vMax)
                    {
                        v *= (vMax * vMax) / (vl * vl);
                    }

                    v += vel * deltaTime;
                    velocity[i] = v;

                }

            }
        }




    }
}