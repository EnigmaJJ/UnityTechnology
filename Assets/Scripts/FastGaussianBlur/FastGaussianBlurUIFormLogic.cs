/******************************************************************************
 * DESCRIPTION: FastGaussianBlur UI逻辑
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.06.22, 19:07, CST
*******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace UnityTechnology
{
    public class FastGaussianBlurUIFormLogic : MonoBehaviour
    {
        #region Variables
        public FastGaussianBlur Blur;
        
        public Text DownsampleValueText;
        public Slider DownsampleSlider;
        public Text IterationValueText;
        public Slider IterationSlider;
        public Text InterpolationValueText;
        public Slider InterpolationSlider;
        #endregion
        
        #region MonoBehaviour
        private void Start()
        {
            SetDownsample(DownsampleSlider.value);
            SetIteration(IterationSlider.value);
            SetInterpolation(InterpolationSlider.value);
        }
        #endregion
        
        #region Methods
        public void SetDownsample(float _value)
        {
            Blur.Downsample = (int)_value;
            DownsampleValueText.text = _value.ToString();
        }

        public void SetIteration(float _value)
        {
            Blur.Iteration = (int)_value;
            IterationValueText.text = _value.ToString();
        }

        public void SetInterpolation(float _value)
        {
            Blur.Interpolation = _value;
            InterpolationValueText.text = _value.ToString("F2");
        }

        public void SetKernelType(int _value)
        {
            Blur.BlurKernelType = (FastGaussianBlur.EBlurKernelType)_value;
        }

        public void ToggleGammaCorrection(bool _value)
        {
            Blur.GammaCorrection = _value;
        }
        #endregion
    }
}
