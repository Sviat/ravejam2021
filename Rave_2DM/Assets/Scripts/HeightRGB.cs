public struct HeightRGB
{
    public static readonly int MAX_HEIGHT = 6;
    public static readonly int DEFAULT_HEIGHT = 4;

    public int HeightR { get; set; }
    public int HeightG { get; set; }
    public int HeightB { get; set; }
    public HeightRGB(int R, int G, int B)
    {
        HeightR = R;
        HeightG = G;
        HeightB = B;
    }
    public HeightRGB Normalized()
    {
        return this / this;
    }

    public static HeightRGB operator +(HeightRGB a, HeightRGB b)
    {
        HeightRGB tmpHeight = new HeightRGB(a.HeightR + b.HeightR, a.HeightG + b.HeightG, a.HeightB + a.HeightB);
        if (tmpHeight.HeightR > MAX_HEIGHT)
            tmpHeight.HeightR = MAX_HEIGHT;
        if (tmpHeight.HeightG > MAX_HEIGHT)
            tmpHeight.HeightG = MAX_HEIGHT;
        if (tmpHeight.HeightB > MAX_HEIGHT)
            tmpHeight.HeightB = MAX_HEIGHT;
        return tmpHeight;
    }

    public static HeightRGB operator /(HeightRGB a, HeightRGB b)
    {
        if (b.HeightR != 0)
            a.HeightR /= b.HeightR;
        if (b.HeightG != 0)
            a.HeightG /= b.HeightG;
        if (b.HeightB != 0)
            a.HeightB /= b.HeightB;
        return a;
    }

    public static HeightRGB operator *(HeightRGB a, int b)
        => new HeightRGB(a.HeightR * b, a.HeightG * b, a.HeightB * b);
    public static HeightRGB operator /(HeightRGB a, int b)
        => new HeightRGB(a.HeightR / b, a.HeightG / b, a.HeightB / b);
};