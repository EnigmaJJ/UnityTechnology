/******************************************************************************
 * DESCRIPTION: 移动任务
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.07.15, 12:18, CST
*******************************************************************************/

using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;

namespace UnityTechnology
{
    [BurstCompile]
    public struct MovementJob : IJobParallelForTransform
    {
        #region Variables
        public float MoveSpeed;
        public float SqrMaxRadius;
        public float DeltaTime;
        #endregion
        
        #region Methods
        public void Execute(int _index, TransformAccess _transform)
        {
            Vector3 pos = _transform.position;
            pos += (_transform.rotation * Vector3.forward) * MoveSpeed * DeltaTime;
            if (pos.sqrMagnitude >= SqrMaxRadius)
            {
                pos = Vector3.zero;
            }

            _transform.position = pos;
        }
        #endregion
    }
}
