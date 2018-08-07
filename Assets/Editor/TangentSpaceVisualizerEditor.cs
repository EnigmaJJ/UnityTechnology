/******************************************************************************
 * DESCRIPTION: 显示切线空间
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.08.07, 21:49, CST
*******************************************************************************/

using UnityEditor;
using UnityEngine;

namespace UnityTechnology
{
    [CustomEditor(typeof(TangentSpaceVisualizer))]
    public class TangentSpaceVisualizerEditor : Editor
    {
        #region Unity
        private void OnSceneGUI()
        {
            var tangentSpaceVisualizer = target as TangentSpaceVisualizer;
            tangentSpaceVisualizer.UpdateIfDirty();

            var color = Handles.color;
                Handles.color = Color.red;
                Handles.DrawLines(tangentSpaceVisualizer.TangentLines);
            
                Handles.color = Color.green;
                Handles.DrawLines(tangentSpaceVisualizer.BinormalLines);
            
                Handles.color = Color.blue;
                Handles.DrawLines(tangentSpaceVisualizer.NormalLines);
            Handles.color = color;
        }
        #endregion
    }
}
