/******************************************************************************
 * DESCRIPTION: 单例
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.10.16, 19:36, CST
*******************************************************************************/

public class Singleton<T> where T : new()
{
    #region Variables
    private static T m_Instance;
    
#if THREAD_SAFE
    private static object m_Lock = new object();
#endif
    #endregion
    
    #region Methods
    public static T Instance
    {
        get
        {
        #if THREAD_SAFE
            lock (m_Lock)
            {
        #endif
                if (null == m_Instance)
                {
                    m_Instance = new T();
                }
        #if THREAD_SAFE
            }
        #endif

            return m_Instance;
        }
    }
    #endregion
}
