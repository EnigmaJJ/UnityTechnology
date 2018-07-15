/******************************************************************************
 * DESCRIPTION: 优化的高斯模糊效果
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.06.22, 19:07, CST
*******************************************************************************/

using UnityEngine;

namespace UnityTechnology
{
    public class FastGaussianBlur : MonoBehaviour
    {
        #region EBlurKernelType
        public enum EBlurKernelType
        {
            Tap5,
            Tap9,
            Tap13,
        }
        #endregion
        
        #region Variables
        public Material BlurMtrl;

        public bool GammaCorrection = true;
        [Range(0, 4)] public int Downsample = 1;
        [Range(0, 8)] public int Iteration = 1;
        [Range(0, 1)] public float Interpolation = 0.5f;
        public EBlurKernelType BlurKernelType = EBlurKernelType.Tap5;

        private int m_BlurRadiusPropertyNameID;
        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            m_BlurRadiusPropertyNameID = Shader.PropertyToID("_BlurRadius");
        }

        private void OnRenderImage(RenderTexture _src, RenderTexture _dest)
        {
            if (0 == Iteration)
            {
                Graphics.Blit(_src, _dest);
                return;
            }
            
            if (GammaCorrection)
            {
                Shader.EnableKeyword("GAMMA_CORRECTION");
            }
            else
            {
                Shader.DisableKeyword("GAMMA_CORRECTION");
            }
            
            int width = _src.width >> Downsample;
            int height = _src.height >> Downsample;
            var temporaryRT_1 = RenderTexture.GetTemporary(width, height, 0, _src.format);
            var temporaryRT_2 = RenderTexture.GetTemporary(width, height, 0, _src.format);

            int pass = (int)BlurKernelType * 2;
            for (int i = 0; i < Iteration; ++i)
            {
                float radius = i * Interpolation + Interpolation;
                BlurMtrl.SetFloat(m_BlurRadiusPropertyNameID, radius);
                
                if (0 == i)
                {
                    Graphics.Blit(_src, temporaryRT_1, BlurMtrl, pass);
                }
                else
                {
                    Graphics.Blit(temporaryRT_2, temporaryRT_1, BlurMtrl, pass);
                    temporaryRT_2.DiscardContents();
                }

                if ((Iteration - 1) == i)
                {
                    Graphics.Blit(temporaryRT_1, _dest, BlurMtrl, pass + 1);
                }
                else
                {
                    Graphics.Blit(temporaryRT_1, temporaryRT_2, BlurMtrl, pass + 1);
                }
                temporaryRT_1.DiscardContents();
            }
            
            RenderTexture.ReleaseTemporary(temporaryRT_1);
            RenderTexture.ReleaseTemporary(temporaryRT_2);
        }
        #endregion
    }
}
