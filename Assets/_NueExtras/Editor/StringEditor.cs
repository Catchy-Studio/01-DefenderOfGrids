using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using _NueExtras.Attributes;

namespace _NueExtras.Editor
{
    /// <summary>
    /// RichText toolbar'ını çizer. String field'ının üstünde bir toolbar gösterir.
    /// TextArea, Multiline gibi diğer attribute'larla uyumlu çalışır.
    /// </summary>
    [CustomPropertyDrawer(typeof(RichTextAttribute))]
    public class RichTextDrawer : PropertyDrawer
    {
        private const float ToolbarHeight = 24f;
        private const float ButtonWidth = 28f;
        private const float ColorFieldWidth = 55f;
        private const float SpaceBetween = 4f;
        private const float PreviewHeight = 45f;
        private const float SliderWidth = 90f;
        private const float Padding = 4f;

        private static Dictionary<string, bool> previewStates = new Dictionary<string, bool>();
        private static Dictionary<string, int> lastSizes = new Dictionary<string, int>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
                return EditorGUIUtility.singleLineHeight;

            // Çoklu seçimde RichText toolbar'ını gösterme
            if (property.serializedObject.isEditingMultipleObjects)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            var attr = attribute as RichTextAttribute;
            float height = ToolbarHeight + Padding;
            
            // Get base height for the string field (respects TextArea, Multiline etc)
            height += EditorGUI.GetPropertyHeight(property, label, true);
            
            string propKey = GetPropertyKey(property);
            if (attr.ShowPreview && GetPreviewState(propKey))
                height += PreviewHeight + Padding;
                
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // Çoklu seçimde RichText toolbar'ını devre dışı bırak
            if (property.serializedObject.isEditingMultipleObjects)
            {
                // Sadece normal field göster (Unity kendi mixed value desteğini kullanır)
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.showMixedValue = false;
                return;
            }

            // Tek object: RichText toolbar ile normal çizim
            var attr = attribute as RichTextAttribute;
            DrawSingleObjectGUI(position, property, label, attr);
        }

        private void DrawSingleObjectGUI(Rect position, SerializedProperty property, GUIContent label, RichTextAttribute attr)
        {
            string propKey = GetPropertyKey(property);
            var currentY = position.y;
            
            // Draw toolbar at the top
            var toolbarRect = new Rect(position.x, currentY, position.width, ToolbarHeight);
            DrawToolbar(toolbarRect, attr, propKey, property);
            currentY += ToolbarHeight + Padding;

            // Draw the actual string field (respects TextArea, Multiline etc)
            var fieldHeight = EditorGUI.GetPropertyHeight(property, label, true);
            var fieldRect = new Rect(position.x, currentY, position.width, fieldHeight);
            EditorGUI.PropertyField(fieldRect, property, label, true);
            currentY += fieldHeight;

            // Draw preview if enabled
            if (attr.ShowPreview && GetPreviewState(propKey))
            {
                currentY += Padding;
                var previewRect = new Rect(position.x, currentY, position.width, PreviewHeight);
                DrawPreviewBox(previewRect, property);
            }
        }


        private void DrawToolbar(Rect rect, RichTextAttribute attr, string propKey, SerializedProperty property)
        {
            // ÖNEMLI: Property'yi kopyalayarak her field için bağımsız işlem yapıyoruz
            var currentText = property.stringValue ?? "";
            
            // Background
            GUI.Box(rect, "", EditorStyles.toolbar);
            
            var currentX = rect.x + Padding;
            var buttonY = rect.y + (rect.height - 18f) * 0.5f;
            var buttonHeight = 18f;

            // Bold Button
            if (attr.ShowBoldButton)
            {
                var boldRect = new Rect(currentX, buttonY, ButtonWidth, buttonHeight);
                var hasBold = currentText.Contains("<b>");
                var buttonStyle = hasBold ? GetButtonStyleActive() : GetButtonStyle();
                if (GUI.Button(boldRect, new GUIContent("<b>B</b>", hasBold ? "Remove <b> tag" : "Add <b> tag"), buttonStyle))
                {
                    ToggleTag("<b>", "</b>", property);
                }
                currentX += ButtonWidth + 2f;
            }

            // Italic Button
            if (attr.ShowItalicButton)
            {
                var italicRect = new Rect(currentX, buttonY, ButtonWidth, buttonHeight);
                var hasItalic = currentText.Contains("<i>");
                var buttonStyle = hasItalic ? GetButtonStyleActive() : GetButtonStyle();
                if (GUI.Button(italicRect, new GUIContent("<i>I</i>", hasItalic ? "Remove <i> tag" : "Add <i> tag"), buttonStyle))
                {
                    ToggleTag("<i>", "</i>", property);
                }
                currentX += ButtonWidth + 2f;
            }

            // Underline Button
            if (attr.ShowUnderlineButton)
            {
                var underlineRect = new Rect(currentX, buttonY, ButtonWidth, buttonHeight);
                var hasUnderline = currentText.Contains("<u>");
                var buttonStyle = hasUnderline ? GetButtonStyleActive() : GetButtonStyle();
                if (GUI.Button(underlineRect, new GUIContent("<u>U</u>", hasUnderline ? "Remove <u> tag" : "Add <u> tag"), buttonStyle))
                {
                    ToggleTag("<u>", "</u>", property);
                }
                currentX += ButtonWidth + 2f;
            }

            // Strikethrough Button
            if (attr.ShowStrikethroughButton)
            {
                var strikeRect = new Rect(currentX, buttonY, ButtonWidth, buttonHeight);
                var hasStrike = currentText.Contains("<s>");
                var buttonStyle = hasStrike ? GetButtonStyleActive() : GetButtonStyle();
                if (GUI.Button(strikeRect, new GUIContent("<s>S</s>", hasStrike ? "Remove <s> tag" : "Add <s> tag"), buttonStyle))
                {
                    ToggleTag("<s>", "</s>", property);
                }
                currentX += ButtonWidth + SpaceBetween;
            }

            // Separator
            if (attr.ShowBoldButton || attr.ShowItalicButton || attr.ShowUnderlineButton || attr.ShowStrikethroughButton)
            {
                currentX += SpaceBetween;
                var separatorRect = new Rect(currentX, rect.y + 4f, 1f, rect.height - 8f);
                EditorGUI.DrawRect(separatorRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
                currentX += SpaceBetween + 2f;
            }

            // Color Picker - Her property için bağımsız renk göster
            if (attr.ShowColorPicker)
            {
                var colorRect = new Rect(currentX, buttonY, ColorFieldWidth, buttonHeight);
                var currentColor = ExtractCurrentColor(currentText);
                
                // Her property için unique ID kullan
                GUI.SetNextControlName(propKey + "_colorPicker");
                EditorGUI.BeginChangeCheck();
                var newColor = EditorGUI.ColorField(colorRect, GUIContent.none, currentColor, true, false, false);
                if (EditorGUI.EndChangeCheck())
                {
                    var colorHex = ColorUtility.ToHtmlStringRGB(newColor);
                    InsertTag($"<color=#{colorHex}>", "</color>", property);
                }
                currentX += ColorFieldWidth + SpaceBetween;
            }

            // Size Slider - Her property için bağımsız boyut göster
            if (attr.ShowSizeSlider)
            {
                var currentSize = ExtractCurrentSize(currentText);
                var displaySize = currentSize > 0 ? currentSize : GetLastSize(propKey);
                var sizeLabel = new Rect(currentX, buttonY + 1f, 30f, buttonHeight);
                EditorGUI.LabelField(sizeLabel, "Size", EditorStyles.miniLabel);
                currentX += 32f;

                var sliderRect = new Rect(currentX, buttonY, SliderWidth, buttonHeight);
                
                // Her property için unique ID kullan
                GUI.SetNextControlName(propKey + "_sizeSlider");
                EditorGUI.BeginChangeCheck();
                var size = EditorGUI.IntSlider(sliderRect, displaySize, 8, 48);
                if (EditorGUI.EndChangeCheck())
                {
                    SetLastSize(propKey, size);
                    if (Event.current.type == EventType.MouseUp)
                    {
                        InsertTag($"<size={size}>", "</size>", property);
                    }
                }
                currentX += SliderWidth + SpaceBetween;
            }

            // Preview Toggle (sağda)
            if (attr.ShowPreview)
            {
                var toggleWidth = 80f;
                var toggleRect = new Rect(rect.xMax - toggleWidth - Padding, buttonY, toggleWidth, buttonHeight);
                var currentState = GetPreviewState(propKey);
                var newState = EditorGUI.ToggleLeft(toggleRect, "Preview", currentState, EditorStyles.miniLabel);
                SetPreviewState(propKey, newState);
            }
        }

        private void DrawPreviewBox(Rect rect, SerializedProperty property)
        {

            var text = property.stringValue ?? "";
            
            GUI.Box(rect, "", EditorStyles.helpBox);
            
            var style = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                alignment = TextAnchor.UpperLeft,
                fontSize = 12,
                wordWrap = true,
                padding = new RectOffset(8, 8, 8, 8)
            };

            var contentRect = new Rect(rect.x + 4, rect.y + 4, rect.width - 8, rect.height - 8);
            
            if (string.IsNullOrEmpty(text))
            {
                style.normal.textColor = Color.gray;
                GUI.Label(contentRect, "Preview will appear here...", style);
            }
            else
            {
                GUI.Label(contentRect, text, style);
            }
        }

        private GUIStyle GetButtonStyle()
        {
            var style = new GUIStyle(EditorStyles.miniButton)
            {
                richText = true,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18f
            };
            return style;
        }

        private GUIStyle GetButtonStyleActive()
        {
            var style = new GUIStyle(EditorStyles.miniButton)
            {
                richText = true,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18f
            };
            style.normal.background = style.active.background;
            style.normal.textColor = new Color(0.3f, 0.7f, 1f);
            return style;
        }

        private void ToggleTag(string startTag, string endTag, SerializedProperty property)
        {
            if (property == null) return;

            // Her property için serializedObject'i güncelle
            property.serializedObject.Update();
            
            // Property'nin KENDİ değerini al
            var text = property.stringValue ?? "";
            
            // Eğer tag zaten varsa kaldır
            if (text.Contains(startTag))
            {
                text = RemoveExistingTag(text, startTag);
                property.stringValue = text;
            }
            else
            {
                // Yoksa ekle
                property.stringValue = startTag + text + endTag;
            }
            
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }


        private void InsertTag(string startTag, string endTag, SerializedProperty property)
        {
            if (property == null) return;

            // Her property için serializedObject'i güncelle
            property.serializedObject.Update();
            
            // Property'nin KENDİ değerini al
            var text = property.stringValue ?? "";
            
            // Önce mevcut tag'i kaldır (tag türüne göre)
            text = RemoveExistingTag(text, startTag);
            
            // Yeni tag'i ekle
            property.stringValue = startTag + text + endTag;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }


        private string RemoveExistingTag(string text, string startTag)
        {
            // Tag türünü belirle
            if (startTag.StartsWith("<b"))
            {
                return RemoveTagPair(text, "<b>", "</b>");
            }
            else if (startTag.StartsWith("<i"))
            {
                return RemoveTagPair(text, "<i>", "</i>");
            }
            else if (startTag.StartsWith("<u"))
            {
                return RemoveTagPair(text, "<u>", "</u>");
            }
            else if (startTag.StartsWith("<s"))
            {
                return RemoveTagPair(text, "<s>", "</s>");
            }
            else if (startTag.StartsWith("<color") || startTag.StartsWith("<#"))
            {
                return RemoveColorTag(text);
            }
            else if (startTag.StartsWith("<size"))
            {
                return RemoveSizeTag(text);
            }
            
            return text;
        }

        private string RemoveTagPair(string text, string openTag, string closeTag)
        {
            // Basit tag'leri kaldır (<b>, <i>, <u>, <s>)
            while (text.Contains(openTag) && text.Contains(closeTag))
            {
                var startIndex = text.IndexOf(openTag);
                var endIndex = text.IndexOf(closeTag, startIndex);
                
                if (startIndex >= 0 && endIndex >= 0)
                {
                    // Tag'leri kaldır ama içeriği koru
                    var before = text.Substring(0, startIndex);
                    var content = text.Substring(startIndex + openTag.Length, endIndex - startIndex - openTag.Length);
                    var after = text.Substring(endIndex + closeTag.Length);
                    text = before + content + after;
                }
                else
                {
                    break;
                }
            }
            
            // Eşi olmayan açılış taglerini temizle
            while (text.Contains(openTag))
            {
                var index = text.IndexOf(openTag);
                if (index >= 0)
                {
                    text = text.Substring(0, index) + text.Substring(index + openTag.Length);
                }
                else
                {
                    break;
                }
            }
            
            // Eşi olmayan kapanış taglerini temizle
            while (text.Contains(closeTag))
            {
                var index = text.IndexOf(closeTag);
                if (index >= 0)
                {
                    text = text.Substring(0, index) + text.Substring(index + closeTag.Length);
                }
                else
                {
                    break;
                }
            }
            
            return text;
        }

        private string RemoveColorTag(string text)
        {
            // <color=#XXXXXX> tag'lerini kaldır
            while (text.Contains("<color="))
            {
                var startIndex = text.IndexOf("<color=");
                if (startIndex < 0) break;
                
                var tagEndIndex = text.IndexOf(">", startIndex);
                if (tagEndIndex < 0) break;
                
                var closeTagIndex = text.IndexOf("</color>", tagEndIndex);
                if (closeTagIndex < 0) break;
                
                // Tag'leri kaldır ama içeriği koru
                var before = text.Substring(0, startIndex);
                var content = text.Substring(tagEndIndex + 1, closeTagIndex - tagEndIndex - 1);
                var after = text.Substring(closeTagIndex + 8); // "</color>" = 8 karakter
                text = before + content + after;
            }
            
            // <#XXXXXX> formatını da destekle
            var regex = new System.Text.RegularExpressions.Regex(@"<#[0-9A-Fa-f]{6}>");
            while (regex.IsMatch(text))
            {
                var match = regex.Match(text);
                var before = text.Substring(0, match.Index);
                var after = text.Substring(match.Index + match.Length);
                text = before + after;
            }
            
            // Eşi olmayan <color=...> açılış taglerini temizle
            var colorRegex = new System.Text.RegularExpressions.Regex(@"<color=#[0-9A-Fa-f]{6}>");
            while (colorRegex.IsMatch(text))
            {
                var match = colorRegex.Match(text);
                text = text.Substring(0, match.Index) + text.Substring(match.Index + match.Length);
            }
            
            // Eşi olmayan </color> kapanış taglerini temizle
            while (text.Contains("</color>"))
            {
                var index = text.IndexOf("</color>");
                if (index >= 0)
                {
                    text = text.Substring(0, index) + text.Substring(index + 8);
                }
                else
                {
                    break;
                }
            }
            
            return text;
        }

        private Color ExtractCurrentColor(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Color.white;

            // <color=#XXXXXX> formatını kontrol et
            var colorIndex = text.IndexOf("<color=#");
            if (colorIndex >= 0)
            {
                var endIndex = text.IndexOf(">", colorIndex);
                if (endIndex > colorIndex)
                {
                    var colorStr = text.Substring(colorIndex + 8, endIndex - colorIndex - 8); // "<color=#" = 8 karakter
                    if (ColorUtility.TryParseHtmlString("#" + colorStr, out Color color))
                    {
                        return color;
                    }
                }
            }

            // <#XXXXXX> formatını kontrol et
            var shortColorRegex = new System.Text.RegularExpressions.Regex(@"<#([0-9A-Fa-f]{6})>");
            var match = shortColorRegex.Match(text);
            if (match.Success)
            {
                var colorStr = match.Groups[1].Value;
                if (ColorUtility.TryParseHtmlString("#" + colorStr, out Color color))
                {
                    return color;
                }
            }

            return Color.white;
        }

        private int ExtractCurrentSize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // <size=XX> formatını kontrol et
            var sizeIndex = text.IndexOf("<size=");
            if (sizeIndex >= 0)
            {
                var endIndex = text.IndexOf(">", sizeIndex);
                if (endIndex > sizeIndex)
                {
                    var sizeStr = text.Substring(sizeIndex + 6, endIndex - sizeIndex - 6); // "<size=" = 6 karakter
                    if (int.TryParse(sizeStr, out int size))
                    {
                        return size;
                    }
                }
            }

            return 0;
        }

        private string RemoveSizeTag(string text)
        {
            // <size=XX> tag'lerini kaldır
            while (text.Contains("<size="))
            {
                var startIndex = text.IndexOf("<size=");
                if (startIndex < 0) break;
                
                var tagEndIndex = text.IndexOf(">", startIndex);
                if (tagEndIndex < 0) break;
                
                var closeTagIndex = text.IndexOf("</size>", tagEndIndex);
                if (closeTagIndex < 0) break;
                
                // Tag'leri kaldır ama içeriği koru
                var before = text.Substring(0, startIndex);
                var content = text.Substring(tagEndIndex + 1, closeTagIndex - tagEndIndex - 1);
                var after = text.Substring(closeTagIndex + 7); // "</size>" = 7 karakter
                text = before + content + after;
            }
            
            // Eşi olmayan <size=...> açılış taglerini temizle
            var sizeRegex = new System.Text.RegularExpressions.Regex(@"<size=\d+>");
            while (sizeRegex.IsMatch(text))
            {
                var match = sizeRegex.Match(text);
                text = text.Substring(0, match.Index) + text.Substring(match.Index + match.Length);
            }
            
            // Eşi olmayan </size> kapanış taglerini temizle
            while (text.Contains("</size>"))
            {
                var index = text.IndexOf("</size>");
                if (index >= 0)
                {
                    text = text.Substring(0, index) + text.Substring(index + 7);
                }
                else
                {
                    break;
                }
            }
            
            return text;
        }

        private string GetPropertyKey(SerializedProperty property)
        {
            return $"{property.serializedObject.targetObject.GetInstanceID()}_{property.propertyPath}";
        }

        private bool GetPreviewState(string propKey)
        {
            if (!previewStates.ContainsKey(propKey))
                previewStates[propKey] = false;
            return previewStates[propKey];
        }

        private void SetPreviewState(string propKey, bool state)
        {
            previewStates[propKey] = state;
        }

        private int GetLastSize(string propKey)
        {
            if (!lastSizes.ContainsKey(propKey))
                lastSizes[propKey] = 14;
            return lastSizes[propKey];
        }

        private void SetLastSize(string propKey, int size)
        {
            lastSizes[propKey] = size;
        }
    }
}