/******************************************************************************
 * DESCRIPTION: 清理材质未使用的纹理贴图的引用
 * 
 *     Copyright (c) 2020, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2020.03.01, 15:20, CST
*******************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class CleanMaterial
    {
        [MenuItem("Assets/Tools/Clean Material")]
        public static void Clean()
        {
            Material[] materials = Selection.GetFiltered<Material>(SelectionMode.Assets | SelectionMode.DeepAssets);
            foreach (var material in materials)
            {
                CleanOneMaterial(material);
            }
        }

        private static bool CleanOneMaterial(Material _material)
        {
            // 收集材质使用到的所有纹理贴图
            HashSet<string> textureGUIDs = CollectTextureGUIDs(_material);

            string materialPathName = Path.GetFullPath(AssetDatabase.GetAssetPath(_material));
            
            StringBuilder strBuilder = new StringBuilder();
            using (StreamReader reader = new StreamReader(materialPathName))
            {
                Regex regex = new Regex(@"\s+guid:\s+(\w+),");
                string line = reader.ReadLine();
                while (null != line)
                {
                    if (line.Contains("m_Texture:"))
                    {
                        // 包含纹理贴图引用的行，使用正则表达式获取纹理贴图的guid
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            string textureGUID = match.Groups[1].Value;
                            if (textureGUIDs.Contains(textureGUID))
                            {
                                strBuilder.AppendLine(line);
                            }
                            else
                            {
                                // 材质没有用到纹理贴图，guid赋值为0来清除引用关系
                                strBuilder.AppendLine(line.Substring(0, line.IndexOf("fileID:") + 7) + " 0}");
                            }
                        }
                        else
                        {
                            strBuilder.AppendLine(line);
                        }
                    }
                    else
                    {
                        strBuilder.AppendLine(line);
                    }
                    
                    line = reader.ReadLine();
                }
            }

            using (StreamWriter writer = new StreamWriter(materialPathName))
            {
                writer.Write(strBuilder.ToString());
            }

            return true;
        }

        private static HashSet<string> CollectTextureGUIDs(Material _material)
        {
            HashSet<string> textureGUIDs = new HashSet<string>();
            for (int i = 0; i < ShaderUtil.GetPropertyCount(_material.shader); ++i)
            {
                if (ShaderUtil.ShaderPropertyType.TexEnv == ShaderUtil.GetPropertyType(_material.shader, i))
                {
                    Texture texture = _material.GetTexture(ShaderUtil.GetPropertyName(_material.shader, i));
                    if (null == texture)
                    {
                        continue;
                    }
                    
                    string textureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
                    if (!textureGUIDs.Contains(textureGUID))
                    {
                        textureGUIDs.Add(textureGUID);
                    }
                }
            }

            return textureGUIDs;
        }
    }
}