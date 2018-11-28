/******************************************************************************
 * DESCRIPTION: Mono单例
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.11.28, 15:00, CST
*******************************************************************************/

using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Variables
    private static T m_Instance;
    private static bool m_ApplicationIsQuitting;

#if THREAD_SAFE
    private static object m_Lock = new object();
#endif
    #endregion

    #region Properties
    public static T Instance
    {
        get
        {
            if (m_ApplicationIsQuitting && (null == m_Instance))
            {
                Debug.LogWarning(string.Format("应用程序正在退出，Mono单例（{0}）已销毁.", typeof(T)));
                return null;
            }

        #if THREAD_SAFE
            lock (m_Lock)
            {
        #endif
                if (null == m_Instance)
                {
                    var instances = FindObjectsOfType<T>();
                    if ((null != instances) && (instances.Length > 0))
                    {
                        m_Instance = instances[0];
                        if (instances.Length > 1)
                        {
                            Debug.LogError(string.Format("Mono单例（{0}）有多个存在.", typeof(T)));
                        }

                        return m_Instance;
                    }

                    var singleton = new GameObject("(singleton) " + typeof(T).Name);
                    DontDestroyOnLoad(singleton);
                    
                    m_Instance = singleton.AddComponent<T>();
                }
        #if THREAD_SAFE
            }
        #endif
            
            return m_Instance;
        }
    }
    #endregion

    #region Unity
    private void OnDestroy()
    {
        m_ApplicationIsQuitting = true;
    }
    #endregion
}
