/******************************************************************************
 * DESCRIPTION: 显示切线空间
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.08.07, 21:23, CST
*******************************************************************************/

using UnityEngine;

namespace UnityTechnology
{
    [RequireComponent(typeof(MeshFilter))]
    public class TangentSpaceVisualizer : MonoBehaviour
    {
        #region Variables
        public float Offset = 0.01f;
        public float Length = 0.1f;

        private Vector3[] m_TangentLines;
        private Vector3[] m_BinormalLines;
        private Vector3[] m_NormalLines;

        private float m_PreOffset;
        private float m_PreLength;
        private Vector3 m_PrevPosition;
        private Vector3 m_PrevScale;
        private Quaternion m_PrevRotation;
        #endregion
        
        #region Properties
        public Vector3[] TangentLines
        {
            get { return m_TangentLines; }
        }

        public Vector3[] BinormalLines
        {
            get { return m_BinormalLines; }
        }
        
        public Vector3[] NormalLines
        {
            get { return m_NormalLines; }
        }
        #endregion

        #region Methods
        public void UpdateIfDirty()
        {
            bool isDirty = false;
            if (IsTransformDirty())
            {
                isDirty = true;
                m_PrevPosition = transform.position;
                m_PrevRotation = transform.rotation;
                m_PrevScale = transform.lossyScale;
            }
            if (IsPropertyDirty())
            {
                isDirty = true;
                m_PreOffset = Offset;
                m_PreLength = Length;
            }

            if (isDirty)
            {
                GenerateData();
            }
        }

        private void GenerateData()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (null == meshFilter)
            {
                Debug.LogWarning("Failed to get MeshFilter component.");
                return;
            }

            Mesh mesh = meshFilter.sharedMesh;
            if (null == mesh)
            {
                Debug.LogWarning("MeshFilter has no mesh.");
                return;
            }
            
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;

            int lineCount = 2 * vertices.Length;
            if ((null != m_NormalLines) && (lineCount > m_NormalLines.Length))
            {
                m_TangentLines = null;
                m_BinormalLines = null;
                m_NormalLines = null;
            }

            if (null == m_NormalLines)
            {
                m_TangentLines = new Vector3[lineCount];
                m_BinormalLines = new Vector3[lineCount];
                m_NormalLines = new Vector3[lineCount];
            }

            var trans = transform;
            var inverseTransposeMatrix = trans.localToWorldMatrix.transpose;
            for (int i = 0; i < vertices.Length; ++i)
            {
                Vector3 normal = inverseTransposeMatrix * normals[i];
                Vector3 tangent = trans.TransformDirection(tangents[i]);
                Vector3 vertex = trans.TransformPoint(vertices[i]) + (normal * Offset);

                int firstIndex = i * 2;
                int secondIndex = i * 2 + 1;
                
                m_NormalLines[firstIndex] = vertex;
                m_NormalLines[secondIndex] = vertex + (normal * Length);

                m_TangentLines[firstIndex] = vertex;
                m_TangentLines[secondIndex] = vertex + (tangent * Length);

                m_BinormalLines[firstIndex] = vertex;
                m_BinormalLines[secondIndex] = vertex + Vector3.Cross(normal, tangent) * Length * tangents[i].w;
            }
        }

        private bool IsTransformDirty()
        {
            return (m_PrevPosition != transform.position) ||
                   (m_PrevRotation != transform.rotation) ||
                   (m_PrevScale != transform.lossyScale);
        }

        private bool IsPropertyDirty()
        {
            return (m_PreOffset != Offset) ||
                   (m_PreLength != Length);
        }
        #endregion
    }
}
