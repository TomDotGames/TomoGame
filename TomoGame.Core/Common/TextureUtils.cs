using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core;

/// <summary>Pixel operations for building textures at runtime.</summary>
public static class TextureUtils
{
    /// <summary>Returns a new texture holding the given region of <paramref name="texture"/>.</summary>
    public static Texture2D Crop(Texture2D texture, Rectangle rect)
    {
        Color[] data = new Color[rect.Width * rect.Height];
        texture.GetData(0, rect, data, 0, data.Length);

        Texture2D cropped = new Texture2D(texture.GraphicsDevice, rect.Width, rect.Height);
        cropped.SetData(data);
        return cropped;
    }

    /// <summary>Returns a new texture holding the given region of <paramref name="texture"/>.</summary>
    public static Texture2D Crop(Texture2D texture, int x, int y, int width, int height)
    {
        return Crop(texture, new Rectangle(x, y, width, height));
    }

    /// <summary>Returns a copy of <paramref name="baseTexture"/> with <paramref name="stamp"/> blended over it
    /// at the given offset. The stamp must fit inside the base.</summary>
    public static Texture2D Stamp(Texture2D baseTexture, Texture2D stamp, int x, int y)
    {
        // the original wrapped an overhanging stamp around to the far edge, which only ever hid a mistake
        Dbg.Assert(x >= 0 && y >= 0);
        Dbg.Assert(x + stamp.Width <= baseTexture.Width);
        Dbg.Assert(y + stamp.Height <= baseTexture.Height);

        Rectangle baseRect = new Rectangle(0, 0, baseTexture.Width, baseTexture.Height);
        Color[] baseData = new Color[baseTexture.Width * baseTexture.Height];
        baseTexture.GetData(0, baseRect, baseData, 0, baseData.Length);

        Color[] stampData = new Color[stamp.Width * stamp.Height];
        stamp.GetData(0, new Rectangle(0, 0, stamp.Width, stamp.Height), stampData, 0, stampData.Length);

        for (int i = 0; i < stampData.Length; ++i)
        {
            int destX = (i % stamp.Width) + x;
            int destY = (i / stamp.Width) + y;
            int dest = destX + (baseTexture.Width * destY);

            baseData[dest] = ColorUtils.Blend(baseData[dest], stampData[i]);
        }

        Texture2D stamped = new Texture2D(baseTexture.GraphicsDevice, baseTexture.Width, baseTexture.Height,
            false, baseTexture.Format);
        stamped.SetData(0, baseRect, baseData, 0, baseData.Length);
        return stamped;
    }

    /// <summary>Returns a copy of <paramref name="texture"/> mirrored on the given axes.</summary>
    public static Texture2D Mirror(Texture2D texture, bool mirrorHorizontal, bool mirrorVertical)
    {
        Dbg.Assert(mirrorHorizontal || mirrorVertical);

        Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
        Color[] data = new Color[texture.Width * texture.Height];
        texture.GetData(0, rect, data, 0, data.Length);

        Color[] mirrored = new Color[data.Length];
        for (int i = 0; i < data.Length; ++i)
        {
            int x = i % texture.Width;
            int y = i / texture.Width;

            int outX = mirrorHorizontal ? texture.Width - x - 1 : x;
            int outY = mirrorVertical ? texture.Height - y - 1 : y;
            mirrored[outX + (outY * texture.Width)] = data[i];
        }

        Texture2D result = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height, false,
            texture.Format);
        result.SetData(0, rect, mirrored, 0, mirrored.Length);
        return result;
    }
}
