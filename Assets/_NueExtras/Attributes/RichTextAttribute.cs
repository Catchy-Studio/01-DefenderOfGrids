using System;
using UnityEngine;

namespace _NueExtras.Attributes
{
    /// <summary>
    /// String field'lara HTML tag ve renk ekleme toolbar'ı ekler.
    /// TextArea, Multiline gibi diğer attribute'larla birlikte kullanılabilir.
    /// Ana field'ı değiştirmez, sadece üstüne toolbar ekler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RichTextAttribute : PropertyAttribute
    {
        public bool ShowColorPicker { get; }
        public bool ShowBoldButton { get; }
        public bool ShowItalicButton { get; }
        public bool ShowSizeSlider { get; }
        public bool ShowPreview { get; }
        public bool ShowUnderlineButton { get; }
        public bool ShowStrikethroughButton { get; }

        public RichTextAttribute(
            bool showColorPicker = true,
            bool showBoldButton = true,
            bool showItalicButton = true,
            bool showUnderlineButton = false,
            bool showStrikethroughButton = false,
            bool showSizeSlider = true,
            bool showPreview = true)
        {
            ShowColorPicker = showColorPicker;
            ShowBoldButton = showBoldButton;
            ShowItalicButton = showItalicButton;
            ShowUnderlineButton = showUnderlineButton;
            ShowStrikethroughButton = showStrikethroughButton;
            ShowSizeSlider = showSizeSlider;
            ShowPreview = showPreview;
        }
    }
}

