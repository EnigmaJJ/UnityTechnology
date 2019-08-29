/******************************************************************************
 * DESCRIPTION: 富文本标签类
 * 
 *     Copyright (c) 2019, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2019.08.28, 16:45, CST
*******************************************************************************/

public class RichTextTag
{
    #region Variables
    public static readonly RichTextTag CLEAR_COLOR_TAG = new RichTextTag("<color=#00000000>");
    
    private static readonly char OPENING_NODE_DELIMETER = '<';
    private static readonly char CLOSING_NODE_DELIMETER = '>';
    private static readonly char END_TAG_DELIMETER = '/';
    private static readonly char PARAMETER_DELIMETER = '=';
    #endregion

    #region Properties
    /// <summary>
    /// 富文本标签文字，如<delay=0.5>
    /// </summary>
    public string TagText { get; private set; }
    
    /// <summary>
    /// 富文本标签长度
    /// </summary>
    public int Length => TagText.Length;

    /// <summary>
    /// 获取富文本标签的结束文本，如果该富文本标签不是结束文本，则创建一个结束文本，如</delay>
    /// </summary>
    public string ClosingTagText => IsClosingTag ? TagText : $"</{TagType}>";

    /// <summary>
    /// 富文本标签类型，以富文本标签体作为类型，如delay
    /// </summary>
    public string TagType
    {
        get
        {
            // 截去起始和结束符号
            var tagType = TagText.Substring(1, TagText.Length - 2);
            tagType = tagType.TrimStart(END_TAG_DELIMETER);
            
            // 截去参数
            var parameterDelimeterIndex = tagType.IndexOf(PARAMETER_DELIMETER);
            if (parameterDelimeterIndex > 0)
            {
                tagType = tagType.Substring(0, parameterDelimeterIndex);
            }

            return tagType;
        }
    }

    /// <summary>
    /// 是否是富文本起始标签
    /// </summary>
    public bool IsOpeningTag => !IsClosingTag;

    /// <summary>
    /// 是否是富文本结束标签
    /// </summary>
    public bool IsClosingTag => (TagText.Length > 2) && (TagText[1] == END_TAG_DELIMETER);
    #endregion
    
    #region Methods
    public RichTextTag()
    {
        TagText = string.Empty;
    }
    
    public RichTextTag(string _tagText)
    {
        TagText = _tagText;
    }

    public override string ToString()
    {
        return TagText;
    }
    
    /// <summary>
    /// 字符串是否以富文本标签起始
    /// </summary>
    /// <param name="_text"></param>
    /// <returns></returns>
    public static bool StringStartsWithTag(string _text)
    {
        return _text.StartsWith(OPENING_NODE_DELIMETER.ToString());
    }

    /// <summary>
    /// 解析字符串以获取下一个RichTextTag
    /// </summary>
    /// <param name="_text"></param>
    /// <returns></returns>
    public static RichTextTag ParseNext(string _text)
    {
        var openingDelimeterIndex = _text.IndexOf(OPENING_NODE_DELIMETER);
        if (openingDelimeterIndex < 0)
        {
            return null;
        }

        var closingDelimeterIndex = _text.IndexOf(CLOSING_NODE_DELIMETER);
        if (closingDelimeterIndex < 0)
        {
            return null;
        }

        var tagText = _text.Substring(openingDelimeterIndex, closingDelimeterIndex - openingDelimeterIndex + 1);
        return new RichTextTag(tagText);
    }

    /// <summary>
    /// 从字符串中移除指定类型的富文本标签
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_tagType"></param>
    /// <returns></returns>
    public static string RemoveTagsFromString(string _text, string _tagType)
    {
        var bodyWithoutTags = _text;
        for (int i = 0; i < _text.Length; ++i)
        {
            var remainingText = _text.Substring(i, _text.Length - i);
            if (StringStartsWithTag(remainingText))
            {
                var parsedTag = ParseNext(remainingText);
                if (null == parsedTag)
                {
                    continue;
                }

                if (parsedTag.TagType == _tagType)
                {
                    bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.TagText, string.Empty);
                    i += (parsedTag.Length - 1);
                }
            }
        }

        return bodyWithoutTags;
    }
    #endregion
}
