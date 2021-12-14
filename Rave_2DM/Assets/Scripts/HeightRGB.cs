public enum HeightValues { R0_DEEP_OCEAN, R2_OCEAN, R3_COAST, R4_PLAIN, R5_HILLS, R6_MOUNTAINS, R8_EVEREST }
public enum TempValues { G0_DETH_TEMP, G2_COLD_LIFE_LOW, G3_COLD, G4_BEST, G5_WARM, G6_HEAT, G8_HELL }
public enum WaterValues { B0_OCEAN_WATER, B2_JUNGLE, B3_RESORT, B4_NORMAL_CLIMAT, B5_DRY_CLIMATE, B6_STEPPE, B8_DESERT }

public struct HeightRGB
{
    public static readonly int MAX_HEIGHT = 6;
    public static readonly int DEFAULT_HEIGHT = 4;

    public HeightValues R { get; }
    public TempValues G { get; }
    public WaterValues B { get; }

    public HeightRGB(int R, int G, int B)
    {
        this.R = (HeightValues) R;
        this.G = (TempValues) G;
        this.B = (WaterValues) B;
    }
    public HeightRGB Normalized()
    {
        return this / this;
    }
    public HeightRGB Normalized2X()
    {
        return Normalized()*2;
    }

    public static HeightRGB operator +(HeightRGB a, HeightRGB b)
    {
        int tmpR, tmpG, tmpB;
        tmpR = (int) a.R + (int) b.R;
        tmpG = (int) a.G + (int) b.G;
        tmpB = (int) a.B + (int) b.B;
        return new HeightRGB(tmpR <= MAX_HEIGHT ? tmpR : MAX_HEIGHT, tmpG <= MAX_HEIGHT ? tmpG : MAX_HEIGHT, tmpB <= MAX_HEIGHT ? tmpB : MAX_HEIGHT);
    }

    public static HeightRGB operator /(HeightRGB a, HeightRGB b)
    {
        int tmpR, tmpG, tmpB;
        tmpR = b.R != 0 ? (int) a.R / (int) b.R : (int) a.R;
        tmpG = b.G != 0 ? (int) a.G / (int) b.G : (int) a.G;
        tmpB = b.B != 0 ? (int) a.B / (int) b.B : (int) a.B;
        return new HeightRGB(tmpR, tmpG, tmpB);
    }

    public static HeightRGB operator *(HeightRGB a, int b)
        => new HeightRGB((int) a.R * (int) b, (int) a.G * (int) b, (int) a.B * b);
    public static HeightRGB operator /(HeightRGB a, int b)
        => new HeightRGB((int) a.R / b, (int) a.G / b, (int) a.B / b);
};