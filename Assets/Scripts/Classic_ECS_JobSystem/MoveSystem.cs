/******************************************************************************
 * DESCRIPTION: 移动系统
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.07.15, 15:05, CST
*******************************************************************************/

using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace UnityTechnology
{
    public class MoveSystem : JobComponentSystem
    {
        #region MovementJob
        [ComputeJobOptimization]
        private struct MovementJob : IJobProcessComponentData<Position, Rotation, MoveSpeed>
        {
            #region Variables
            public float SqrMaxRadius;
            public float DeltaTime;
            #endregion
            
            #region Methods
            public void Execute(ref Position _position, ref Rotation _rotation, ref MoveSpeed _moveSpeed)
            {
                float3 pos = _position.Value;
                pos += math.forward(_rotation.Value) * _moveSpeed.speed * DeltaTime;
                if (math.lengthSquared(pos) >= SqrMaxRadius)
                {
                    pos = new float3(0, 0, 0);
                }

                _position.Value = pos;
            }
            #endregion
        }
        #endregion
        
        #region Methods
        protected override JobHandle OnUpdate(JobHandle _inputDeps)
        {
            var movementJob = new MoveSystem.MovementJob() {
                                    SqrMaxRadius = ECSManager.SQR_MAX_RADIUS,
                                    DeltaTime = Time.deltaTime,
            };

            var movementJobHandle = movementJob.Schedule(this, 64, _inputDeps);
            return movementJobHandle;
        }
        #endregion
    }
}
