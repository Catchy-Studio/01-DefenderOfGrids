using UnityEngine;
using _NueExtras.Attributes;

namespace _NueExtras.Examples
{
    /// <summary>
    /// RichTextAttribute kullanım örneği
    /// Bu attribute, string field'ların ÜSTÜNE bir toolbar ekler.
    /// TextArea, Multiline gibi diğer attribute'larla birlikte kullanılabilir.
    /// Ana field'ı değiştirmez!
    /// </summary>
    public class RichTextAttributeExample : MonoBehaviour
    {
        [Header("Temel Kullanım - Normal String")]
        [RichText]
        [SerializeField] private string simpleText = "Bu bir örnek metin";
        
        [Header("TextArea ile Birlikte Kullanım")]
        [RichText]
        [TextArea(3, 5)]
        [SerializeField] private string textAreaExample = "Bu text area ile birlikte çalışır!\nÇok satırlı text yazabilirsiniz.";
        
        [Header("Multiline ile Birlikte Kullanım")]
        [RichText]
        [Multiline(3)]
        [SerializeField] private string multilineExample = "Multiline ile de çalışır!";
        
        [Header("Özelleştirilmiş Toolbar")]
        [RichText(showColorPicker: true, showBoldButton: true, showItalicButton: true, 
                  showUnderlineButton: true, showStrikethroughButton: true, 
                  showSizeSlider: false, showPreview: true)]
        [TextArea(2, 4)]
        [SerializeField] private string customToolbar = "Özelleştirilmiş toolbar";
        
        [Header("Sadece Renk - Preview Kapalı")]
        [RichText(showColorPicker: true, showBoldButton: false, showItalicButton: false, 
                  showSizeSlider: false, showPreview: false)]
        [SerializeField] private string colorOnly = "Sadece renk seçici";
        
        [Header("Quest Örneği - TextArea ile")]
        [RichText]
        [TextArea(2, 3)]
        [SerializeField] private string questTitle = "<color=#FFD700><b>Gizemli Adamın Teklifi</b></color>";
        
        [RichText]
        [TextArea(5, 10)]
        [SerializeField] private string questDescription = 
            "Karşınızda gizemli bir adam belirir ve size iki hap gösterir:\n\n" +
            "<color=#0000FF><b>Mavi Hap</b></color>: Gerçeği öğren ve güç kazan\n" +
            "<color=#FF0000><b>Kırmızı Hap</b></color>: Rahatını koru ve zengin ol";
        
        [Header("Choice Örnekleri")]
        [RichText(showSizeSlider: false)]
        [SerializeField] private string choice1 = "<color=#0000FF><b>Mavi Hapı</b></color> Seç";
        
        [RichText(showSizeSlider: false)]
        [SerializeField] private string choice2 = "<color=#FF0000><b>Kırmızı Hapı</b></color> Seç";
        
        [Header("Sonuç Metinleri - Preview Açık")]
        [RichText(showPreview: true)]
        [TextArea(3, 5)]
        [SerializeField] private string blueResult = 
            "<size=18><color=#0000FF><b>7 gün sonra...</b></color></size>\n\n" +
            "Yeni bir <color=#FFD700><i>yetenek</i></color> kazandınız!";
        
        [RichText(showPreview: true)]
        [TextArea(3, 5)]
        [SerializeField] private string redResult = 
            "<size=18><color=#FF0000><b>3 gün sonra...</b></color></size>\n\n" +
            "<color=#FFD700><b>100 Coin</b></color> kazandınız!";
    }
}

