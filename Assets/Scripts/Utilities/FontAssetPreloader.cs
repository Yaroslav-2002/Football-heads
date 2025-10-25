using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class FontAssetPreloader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PreloadFontAssets()
    {
        PreloadTMPFontAssets();
        PreloadTextCoreFontAssets();
    }

    private static void PreloadTMPFontAssets()
    {
        var tmpSettings = TMP_Settings.instance;
        if (tmpSettings == null)
        {
            return;
        }

        EnsureLoaded(tmpSettings.defaultFontAsset);

        var fallbackFonts = tmpSettings.fallbackFontAssets;
        if (fallbackFonts == null)
        {
            return;
        }

        foreach (var fontAsset in fallbackFonts)
        {
            EnsureLoaded(fontAsset);
        }
    }

    private static void PreloadTextCoreFontAssets()
    {
        var textSettings = TextSettings.defaultTextSettings;
        if (textSettings == null)
        {
            return;
        }

        EnsureLoaded(textSettings.defaultFontAsset);

        var fallbackFonts = textSettings.fallbackFontAssets;
        if (fallbackFonts == null)
        {
            return;
        }

        foreach (var fontAsset in fallbackFonts)
        {
            EnsureLoaded(fontAsset);
        }
    }

    private static void EnsureLoaded(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null)
        {
            return;
        }

        fontAsset.ReadFontAssetDefinition();
    }

    private static void EnsureLoaded(FontAsset fontAsset)
    {
        if (fontAsset == null)
        {
            return;
        }

        fontAsset.ReadFontAssetDefinition();
    }
}
