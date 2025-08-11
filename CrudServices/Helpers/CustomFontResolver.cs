using PdfSharp.Fonts;
using System.IO;

public class CustomFontResolver : IFontResolver
{
    // This method tells PdfSharp the filename for the font family
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {

        return new FontResolverInfo("Montserrat-Italic.ttf");

    }

    // This method reads the font file from the disk
    public byte[] GetFont(string faceName)
    {
        // Correct path to find the file inside your 'fonts' folder
        string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "fonts", faceName);
        return File.ReadAllBytes(fontPath);
    }
}