/******************************************************************************
 * DESCRIPTION: 富文本标签解析类
 * 
 *     Copyright (c) 2019, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2019.08.28, 16:25, CST
*******************************************************************************/

using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class TextTagParser : MonoBehaviour
{
    #region TextSymbol
    public class TextSymbol
    {
        #region Properties
        /// <summary>
        /// 文本标记存储的字符
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// 文本标记存储的富文本标签
        /// </summary>
        public RichTextTag Tag { get; set; }

        /// <summary>
        /// 文本标记存储的文本的长度
        /// </summary>
        public int Length => Text.Length;

        /// <summary>
        /// 文本标记存储的文本
        /// </summary>
        public string Text
        {
            get
            {
                if (IsTag)
                {
                    return Tag.TagText;
                }

                return Character;
            }
        }

        /// <summary>
        /// 文本标记存储的是否是富文本标签
        /// </summary>
        public bool IsTag => null != Tag;
        #endregion

        #region Methods
        public TextSymbol()
        {
        }
        #endregion
    }
    #endregion

    #region TypedText
    public struct TypedText
    {
        public string Text;
        public char LastTypedChar;
        public bool IsFinished;
    }
    #endregion

    #region Variables
    /// <summary>
    /// Unity定义的富文本标签
    /// </summary>
    private static readonly string[] UNITY_TAG_TYPES =
    {
        "b"
      , "i"
      , "size"
      , "color"
      , "style"
    };

    /// <summary>
    /// 自定义的富文本标签
    /// </summary>
    public static readonly string[] CUSTOM_TAG_TYPES =
    {
    };
    
    private static StringBuilder m_ShownText = new StringBuilder();
    private static StringBuilder m_HiddenText = new StringBuilder();

    private static int m_ValidTextSymbolCount = 0;  // 有效的文本标记数量
    private static List<TextSymbol> m_TextSymbols = new List<TextSymbol>();  // 文本标记列表

    private static List<RichTextTag> m_ActiveTags = new List<RichTextTag>();
    #endregion
    
    #region Methods
    /// <summary>
    /// 获取在指定位置的可打印文本
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_visibleCharacterIndex"></param>
    /// <returns></returns>
    public static TypedText GetTypedTextAt(string _text, int _visibleCharacterIndex)
    {
        CreateSymbolsFromText(_text);

        // 根据指定位置拆分为可见的字符串和不可见的字符串
        int typedCharCount = 0;
        var shownText = string.Empty;
        var hiddenText = string.Empty;
        var lastVisibleCharacter = char.MinValue;
        for (int i = 0; i < m_ValidTextSymbolCount; ++i)
        {
            var symbol = m_TextSymbols[i];

            if (typedCharCount <= _visibleCharacterIndex)
            {
                shownText += symbol.Text;

                if (!symbol.IsTag)
                {
                    lastVisibleCharacter = symbol.Character[0];
                }
            }
            else
            {
                hiddenText += symbol.Text;
            }

            if (!symbol.IsTag)
            {
                ++typedCharCount;
            }
        }
        
        ParseActiveTags(_visibleCharacterIndex);
        for (int i = m_ActiveTags.Count - 1; i >= 0; --i)
        {
            shownText = shownText.Insert(shownText.Length, m_ActiveTags[i].ClosingTagText);
        }
        
        /*
        foreach (var activeTag in m_ActiveTags)
        {
            hiddenText = RemoveFirstOccurance(hiddenText, activeTag.ClosingTagText);
        }
        
        // 移除颜色标签，否则会产生显示错误
        // 例如：<color=clear>ABC <color=red>DEF</color></color> DEF会显示为红色，不会被隐藏
        hiddenText = RichTextTag.RemoveTagsFromString(hiddenText, "color");
        
        // 添加隐藏标签
        if (!string.IsNullOrEmpty(hiddenText))
        {
            var hiddenTag = RichTextTag.CLEAR_COLOR_TAG;
            hiddenText = hiddenText.Insert(0, hiddenTag.TagText);
            hiddenText = hiddenText.Insert(hiddenText.Length, hiddenTag.ClosingTagText);
        }
        
        // 加回富文本标签的结束文本
        for (int i = 0; i < m_ActiveTags.Count; ++i)
        {
            hiddenText = hiddenText.Insert(0, m_ActiveTags[i].ClosingTagText);
        }
        */

        var typedText = new TypedText
        {
            Text = shownText
          , LastTypedChar = lastVisibleCharacter
          , IsFinished = string.IsNullOrEmpty(hiddenText)
        };
        
        return typedText;
    }

    /// <summary>
    /// 从字符串中移除所有富文本标签
    /// </summary>
    /// <param name="_textWithTags"></param>
    /// <returns></returns>
    public static string RemoveAllTags(string _textWithTags)
    {
        var textWithoutTags = _textWithTags;
        textWithoutTags = RemoveUnityTags(textWithoutTags);
        textWithoutTags = RemoveCustomTags(textWithoutTags);

        return textWithoutTags;
    }

    /// <summary>
    /// 从字符串中移除Unity定义的富文本标签
    /// </summary>
    /// <param name="_textWithTags"></param>
    /// <returns></returns>
    public static string RemoveUnityTags(string _textWithTags)
    {
        return RemoveTags(_textWithTags, UNITY_TAG_TYPES);
    }

    /// <summary>
    /// 从字符串中移除自定义的富文本标签
    /// </summary>
    /// <param name="_textWithTags"></param>
    /// <returns></returns>
    public static string RemoveCustomTags(string _textWithTags)
    {
        return RemoveTags(_textWithTags, CUSTOM_TAG_TYPES);
    }

    /// <summary>
    /// 从字符串中移除富文本标签
    /// </summary>
    /// <param name="_textWithTags"></param>
    /// <param name="_tags"></param>
    /// <returns></returns>
    private static string RemoveTags(string _textWithTags, string[] _tags)
    {
        var textWithoutTags = _textWithTags;
        foreach (var tag in _tags)
        {
            textWithoutTags = RichTextTag.RemoveTagsFromString(textWithoutTags, tag);
        }

        return textWithoutTags;
    }

    /// <summary>
    /// 获取文本标记，如果超过最大数量，则新建文本标记
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    private static TextSymbol GetTextSymbol(int _index)
    {
        if (_index >= m_TextSymbols.Count)
        {
            var textSymbol = new TextSymbol();
            m_TextSymbols.Add(textSymbol);

            return textSymbol;
        }

        return m_TextSymbols[_index];
    }
    
    /// <summary>
    /// 根据字符串创建文本标记
    /// </summary>
    /// <param name="_text"></param>
    /// <returns></returns>
    private static void CreateSymbolsFromText(string _text)
    {
        int textSymbolIndex = 0;
        int parsedCharacters = 0;
        while (parsedCharacters < _text.Length)
        {
            TextSymbol symbol = null;
            
            // 检查富文本标签
            var remainingText = _text.Substring(parsedCharacters, _text.Length - parsedCharacters);
            if (RichTextTag.StringStartsWithTag(remainingText))
            {
                var tag = RichTextTag.ParseNext(remainingText);
                if (null != tag)
                {
                    // 富文本标签的文本标记
                    symbol = GetTextSymbol(textSymbolIndex++);
                    symbol.Tag = tag;
                }
            }

            if (null == symbol)
            {
                // 普通字符的文本标记
                symbol = GetTextSymbol(textSymbolIndex++);
                symbol.Character = remainingText.Substring(0, 1);
            }

            parsedCharacters += symbol.Length;
        }

        m_ValidTextSymbolCount = textSymbolIndex;
    }

    /// <summary>
    /// 解析到指定位置为止的被激活的富文本标签
    /// </summary>
    /// <param name="_visibleCharacterIndex"></param>
    private static void ParseActiveTags(int _visibleCharacterIndex)
    {
        m_ActiveTags.Clear();
        
        int typedCharCount = 0;
        for (int i = 0; i < m_ValidTextSymbolCount; ++i)
        {
            var symbol = m_TextSymbols[i];

            if (symbol.IsTag)
            {
                if (symbol.Tag.IsOpeningTag)
                {
                    m_ActiveTags.Add(symbol.Tag);
                }
                else
                {
                    var poppedTag = m_ActiveTags[m_ActiveTags.Count - 1];
                    if (poppedTag.TagType != symbol.Tag.TagType)
                    {
                        Debug.LogError($"富文本标签不匹配，起始：{poppedTag.TagType}，结束：{symbol.Tag.TagType}");
                    }
                    
                    m_ActiveTags.RemoveAt(m_ActiveTags.Count - 1);
                }
            }
            else
            {
                ++typedCharCount;
                if (typedCharCount > _visibleCharacterIndex)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 从源字符串中移除找到的第一个搜索字符串
    /// </summary>
    /// <param name="_source"></param>
    /// <param name="_searchString"></param>
    /// <returns></returns>
    private static string RemoveFirstOccurance(string _source, string _searchString)
    {
        var index = _source.IndexOf(_searchString);
        if (index >= 0)
        {
            return _source.Remove(index, _searchString.Length);
        }

        return _source;
    }
    #endregion
}
