/******************************************************************************
 * DESCRIPTION: 补间动画的曲线轨迹运动
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.08.03, 11:42, CST
*******************************************************************************/

using UnityEngine;

namespace UnityTechnology
{
    public class TweenCurve : MonoBehaviour
    {
        #region Variables
        private static readonly float CURVE_MIN_X = 0.0f;
        private static readonly float CURVE_MAX_X = 1.0f;
        
        public bool XFlip, YFlip;
        public float Duration = 1.0f;    // In seconds
        public AnimationCurve Curve;
        public float CurveOffset, CurveScale;
        public Transform From, To;
        public EasingFunction.Ease EaseType = EasingFunction.Ease.Linear;

        private float m_ElapsedTime;    // In seconds
        private float m_Distance;
        private Vector3 m_From, m_To;
        private Vector3 m_Dir, m_CurveEvalDir;
        private EasingFunction.Function m_EaseFunction;

        private Transform m_TransCache;
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            enabled = false;
            m_TransCache = transform;
        }

        private void Update()
        {
            m_ElapsedTime += Time.deltaTime;

            float factor = Mathf.Clamp01(m_ElapsedTime / Duration);
            factor = m_EaseFunction(CURVE_MIN_X, CURVE_MAX_X, factor);

            float evalTime = factor;
            if (XFlip)
            {
                evalTime = CURVE_MAX_X - evalTime;
            }
            
            float eval = (Curve.Evaluate(evalTime) + CurveOffset) * CurveScale;
            if (YFlip)
            {
                eval = -eval;
            }
            
            m_TransCache.position = m_From + (m_Dir * (m_Distance * factor)) + (eval * m_CurveEvalDir);

            if (m_ElapsedTime >= Duration)
            {
                enabled = false;
            }
        }
        #endregion
        
        #region Methods
        public bool Play()
        {
            if ((null == From) || (null == To))
            {
                Debug.LogError("From or To is null.");
                enabled = false;
                return false;
            }

            return Play(From.position, To.position);
        }

        public bool Play(Vector3 _from, Vector3 _to)
        {
            m_From = _from;
            m_To = _to;

            m_Dir = (m_To - m_From);
            m_Distance = m_Dir.magnitude;
            m_Dir.Normalize();
            m_CurveEvalDir = new Vector3(-m_Dir.y, m_Dir.x, 0.0f);    // 逆时针旋转90度

            m_ElapsedTime = 0.0f;
            m_EaseFunction = EasingFunction.GetEasingFunction(EaseType);
            enabled = true;

            return true;
        }
        #endregion
    }
}
