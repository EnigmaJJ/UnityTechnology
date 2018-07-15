/******************************************************************************
 * DESCRIPTION: 以传统模式管理对象
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.07.15, 10:32, CST
*******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace UnityTechnology
{
    public class ClassicManager : MonoBehaviour
    {
        #region Variables
        public Text ObjectCountText;
        public GameObject ObjectPrefab;
        public int ObjectInitialCount = 5000;
        public int ObjectIncreaseCount = 500;

        private int m_ObjectCount;
        #endregion
        
        #region MonoBehaviour
        private void OnEnable()
        {
            for (int i = 0; i < ObjectInitialCount; ++i)
            {
                Instantiate(ObjectPrefab, transform);
            }

            m_ObjectCount = ObjectInitialCount;
            ObjectCountText.text = m_ObjectCount.ToString();
        }

        private void OnDisable()
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < ObjectIncreaseCount; ++i)
                {
                    Instantiate(ObjectPrefab, transform);
                }
                
                m_ObjectCount += ObjectIncreaseCount;
                ObjectCountText.text = m_ObjectCount.ToString();
            }
        }
        #endregion
    }
}
