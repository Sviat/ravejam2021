

enum HeightValues { R0, R2, R3, R4, R5, R6, R8 }
enum TempValues { G0, G2, G3, G4, G5, G6, G8 }
enum WaterValues { B0, B2, B3, B4, B5, B6, B8 }

public struct HeightRGB
{
    public static readonly int MAX_HEIGHT = 6;
    public static readonly int DEFAULT_HEIGHT = 4;

    public int HeightR { get; }
    public int HeightG { get; }
    public int HeightB { get; }

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
    public HeightRGB Normalized2X()
    {
        return Normalized() * 2;
    }

    public static HeightRGB operator +(HeightRGB a, HeightRGB b)
    {
        int tmpR, tmpG, tmpB;
        tmpR = a.HeightR + b.HeightR;
        tmpG = a.HeightG + b.HeightG;
        tmpB = a.HeightB + b.HeightB;
        return new HeightRGB(tmpR <= MAX_HEIGHT ? tmpR : MAX_HEIGHT, tmpG <= MAX_HEIGHT ? tmpG : MAX_HEIGHT, tmpB <= MAX_HEIGHT ? tmpB : MAX_HEIGHT);
    }

    public static HeightRGB operator /(HeightRGB a, HeightRGB b)
    {
        int tmpR, tmpG, tmpB;
        tmpR = b.HeightR != 0 ? a.HeightR / b.HeightR : a.HeightR;
        tmpG = b.HeightG != 0 ? a.HeightG / b.HeightG : a.HeightG;
        tmpB = b.HeightB != 0 ? a.HeightB / b.HeightB : a.HeightB;
        return new HeightRGB(tmpR, tmpG, tmpB);
    }

    public static HeightRGB operator *(HeightRGB a, int b)
        => new HeightRGB(a.HeightR * b, a.HeightG * b, a.HeightB * b);
    public static HeightRGB operator /(HeightRGB a, int b)
        => new HeightRGB(a.HeightR / b, a.HeightG / b, a.HeightB / b);
};