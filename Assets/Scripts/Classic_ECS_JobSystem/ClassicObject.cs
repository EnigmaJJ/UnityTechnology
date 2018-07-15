/******************************************************************************
 * DESCRIPTION: 对象以传统模式运行
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.07.15, 10:35, CST
*******************************************************************************/

using UnityEngine;

namespace UnityTechnology
{
    public class ClassicObject : MonoBehaviour
    {
        #region Variables
        public float MoveSpeed = 2.0f;
        
        private static readonly float MAX_RADIUS = 50.0f;
        private static readonly float SQR_MAX_RADIUS = MAX_RADIUS * MAX_RADIUS;
        
        private Transform m_TransCache;
        #endregion
        
        #region MonoBehaviour
        private void Awake()
        {
            m_TransCache = transform;
        }

        private void Start()
        {
            m_TransCache.forward = Random.insideUnitSphere;
            m_TransCache.position = Random.insideUnitSphere * Random.Range(0, MAX_RADIUS);
        }

        private void Update()
        {
            m_TransCache.Translate(m_TransCache.forward * MoveSpeed * Time.deltaTime);
            if (m_TransCache.position.sqrMagnitude >= SQR_MAX_RADIUS)
            {
                m_TransCache.position = Vector3.zero;
            }
        }
        #endregion
    }
}
