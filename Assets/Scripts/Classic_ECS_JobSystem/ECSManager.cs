/******************************************************************************
 * DESCRIPTION: 以ECS模式管理对象
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.07.15, 14:16, CST
*******************************************************************************/

using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTechnology
{
    public class ECSManager : MonoBehaviour
    {
        #region Variables
        public Text ObjectCountText;
        public GameObject ObjectPrefab;
        public int ObjectInitialCount = 5000;
        public int ObjectIncreaseCount = 500;
        
        public static readonly float MAX_RADIUS = 50.0f;
        public static readonly float SQR_MAX_RADIUS = MAX_RADIUS * MAX_RADIUS;
        
        private EntityManager m_EntityManager;
        private int m_ObjectCount;
        #endregion
        
        #region MonoBehaviour
        private void OnEnable()
        {
            m_ObjectCount = 0;

            m_EntityManager = World.Active.GetOrCreateManager<EntityManager>();
            InstantiateObject(ObjectInitialCount);
        }
        
        private void OnDisable()
        {
            var entities = m_EntityManager.GetAllEntities(Allocator.Temp);
            m_EntityManager.DestroyEntity(entities);
            entities.Dispose();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InstantiateObject(ObjectIncreaseCount);
            }
        }
        #endregion
        
        #region Methods
        private void InstantiateObject(int _count)
        {
            NativeArray<Entity> entities = new NativeArray<Entity>(_count, Allocator.Temp);
            m_EntityManager.Instantiate(ObjectPrefab, entities);
        
            for (int i = 0; i < _count; ++i)
            {
                var entity = entities[i];
                m_EntityManager.SetComponentData(entity, new Position(Random.insideUnitSphere * Random.Range(0, MAX_RADIUS)));
                m_EntityManager.SetComponentData(entity, new Rotation(Quaternion.LookRotation(Random.insideUnitSphere)));
            }
            
            entities.Dispose();
        
            m_ObjectCount += _count;
            ObjectCountText.text = m_ObjectCount.ToString();
        }
        #endregion
    }
}
