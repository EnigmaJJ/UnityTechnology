/******************************************************************************
 * DESCRIPTION: 以Job System模式管理对象
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.07.15, 12:41, CST
*******************************************************************************/

using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Jobs;

namespace UnityTechnology
{
    public class JobSystemManager : MonoBehaviour
    {
        #region Variables
        public Text ObjectCountText;
        public GameObject ObjectPrefab;
        public float MoveSpeed = 2.0f;
        public int ObjectInitialCount = 5000;
        public int ObjectIncreaseCount = 500;
        
        private static readonly float MAX_RADIUS = 50.0f;
        private static readonly float SQR_MAX_RADIUS = MAX_RADIUS * MAX_RADIUS;
        
        private TransformAccessArray m_TransAccessArray;
        private MovementJob m_MovementJob;
        private JobHandle m_MovementJobHandle;

        private int m_ObjectCount;
        #endregion
        
        #region MonoBehaviour
        private void OnEnable()
        {
            m_ObjectCount = 0;
            m_TransAccessArray = new TransformAccessArray(0, -1);
            
            InstantiateObject(ObjectInitialCount);
        }

        private void OnDisable()
        {
            m_MovementJobHandle.Complete();
            m_TransAccessArray.Dispose();
            
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void Update()
        {
            m_MovementJobHandle.Complete();
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InstantiateObject(ObjectIncreaseCount);
            }

            m_MovementJob = new MovementJob() {
                                    MoveSpeed = MoveSpeed,
                                    SqrMaxRadius = SQR_MAX_RADIUS,
                                    DeltaTime = Time.deltaTime,
            };
            m_MovementJobHandle = m_MovementJob.Schedule(m_TransAccessArray);
            
            JobHandle.ScheduleBatchedJobs();
        }
        #endregion
        
        #region Methods
        private void InstantiateObject(int _count)
        {
            m_MovementJobHandle.Complete();

            m_TransAccessArray.capacity = m_TransAccessArray.length + _count;
            for (int i = 0; i < _count; ++i)
            {
                var objectInstance = Instantiate(ObjectPrefab, transform);
                objectInstance.transform.forward = Random.insideUnitSphere;
                objectInstance.transform.position = Random.insideUnitSphere * Random.Range(0, MAX_RADIUS);
                
                m_TransAccessArray.Add(objectInstance.transform);
            }

            m_ObjectCount += _count;
            ObjectCountText.text = m_ObjectCount.ToString();
        }
        #endregion
    }
}
