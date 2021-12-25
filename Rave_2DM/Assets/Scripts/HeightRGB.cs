using System;

public enum HeightValues { R0_DEEP_OCEAN, R1, R2_OCEAN, R3_COAST, R4_PLAIN, R5_HILLS, R6_MOUNTAINS, R7, R8_EVEREST }
public enum TempValues { G0_DETH_TEMP, G1, G2_COLD_LIFE_LOW, G3_COLD, G4_BEST, G5_WARM, G6_HEAT, G7, G8_HELL }
public enum WaterValues { B0_OCEAN_OF_WATER, B1, B2_JUNGLE, B3_RESORT, B4_NORMAL_CLIMAT, B5_DRY_CLIMATE, B6_STEPPE, B7, B8_DESERT }


[Serializable] public struct HeightRGB
{ 
    public static readonly HeightValues MAX_HEIGHT = HeightValues.R8_EVEREST;
    public static readonly HeightValues DEFAULT_HEIGHT = HeightValues.R5_HILLS;
    public static readonly TempValues DEFAULT_TEMP = TempValues.G4_BEST;

    public HeightValues R;
    public TempValues G;
    public WaterValues B;

    public HeightRGB(HeightRGB h)
    {
        R = h.R;
        G = h.G;
        B = h.B;
    }

    public HeightRGB(int R, int G, int B)
    {
        this.R = R < (int)MAX_HEIGHT ? (HeightValues)R : HeightValues.R8_EVEREST;
        this.G = G < (int)MAX_HEIGHT ? (TempValues)G : TempValues.G8_HELL;
        this.B = B < (int)MAX_HEIGHT ? (WaterValues)B : WaterValues.B8_DESERT;
    }

    private HeightRGB Normalized()
    {
        return this / this;
    }

    public static HeightRGB operator +(HeightRGB a, HeightRGB b)
    {
        int tmpR, tmpG, tmpB;
        tmpR = (int)a.R + (int)b.R;
        tmpR = (tmpR <= (int) MAX_HEIGHT ? tmpR : (int)MAX_HEIGHT);
        tmpG = (int)a.G + (int)b.G;
        tmpG = (tmpG <= (int)MAX_HEIGHT ? tmpG : (int)MAX_HEIGHT);
        tmpB = (int)a.B + (int)b.B;
        tmpB = (tmpB <= (int)MAX_HEIGHT ? tmpB : (int)MAX_HEIGHT);

        return new HeightRGB(tmpR, tmpG, tmpB);
    }

    public static HeightRGB operator /(HeightRGB a, HeightRGB b)
    {
        int tmpR, tmpG, tmpB;
        tmpR = b.R != 0 ? (int)a.R / (int)b.R : (int)a.R;
        tmpG = b.G != 0 ? (int)a.G / (int)b.G : (int)a.G;
        tmpB = b.B != 0 ? (int)a.B / (int)b.B : (int)a.B;
        return new HeightRGB(tmpR, tmpG, tmpB);
    }

    public static HeightRGB operator * (HeightRGB a, int b)
        => new HeightRGB((int)a.R * b, (int)a.G * b, (int)a.B * b);

    public static HeightRGB operator / (HeightRGB a, int b)
        => new HeightRGB((int)a.R / b, (int)a.G / b, (int)a.B / b);

    public static HeightRGB operator / (HeightRGB a, float b)
        => new HeightRGB(
            (int)((float)a.R / b),
            (int)((float)a.G / b), 
            (int)((float)a.B / b)
        );

    public static bool operator == (HeightRGB a, HeightRGB b) => a.R==b.R && a.G==b.G && a.B==b.B;

    public static bool operator != (HeightRGB a, HeightRGB b) => !(a==b);

    public string PrintHeight()
    {
        return $"R{(int)R}G{(int)G}B{(int)B}";
    }

};