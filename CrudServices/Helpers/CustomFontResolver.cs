using PdfSharp.Fonts;
using System.IO;

public class CustomFontResolver : IFontResolver
{
    // This method tells PdfSharp the filename for the font family
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Return the name of the font file you added
<<<<<<< HEAD
        return new FontResolverInfo("Montserrat-Italic.ttf");
=======
        return new FontResolverInfo("Montserrat-SemiBold.ttf");
>>>>>>> 85bba0cacabe981a549101b1709ddb3c23bae4e2
    }

    // This method reads the font file from the disk
    public byte[] GetFont(string faceName)
    {
        // Correct path to find the file inside your 'fonts' folder
        string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "fonts", faceName);
        return File.ReadAllBytes(fontPath);
    }
}