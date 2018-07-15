/******************************************************************************
 * DESCRIPTION: Classic_ECS_JobSystem UI逻辑
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.07.15, 14:36, CST
*******************************************************************************/

using UnityEngine;

namespace UnityTechnology
{
    public class Classic_ECS_JobSystemUIFormLogic : MonoBehaviour
    {
        #region Variables
        public GameObject[] Managers;

        private int m_ManagerIndex;
        #endregion
        
        #region Methods
        public void SwitchManager(int _index)
        {
            if (_index == m_ManagerIndex)
            {
                return;
            }

            Managers[m_ManagerIndex].SetActive(false);
            
            m_ManagerIndex = _index;
            Managers[m_ManagerIndex].SetActive(true);
        }
        #endregion
    }
}
