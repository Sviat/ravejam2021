using System;
using Unity;
using UnityEngine;

public enum HeightValues { R0_DEEP_OCEAN, R1, R2_OCEAN, R3_COAST, R4_PLAIN, R5_HILLS, R6_MOUNTAINS, R7, R8_EVEREST }
public enum TempValues { G0_DETH_TEMP, G1, G2_COLD_LIFE_LOW, G3_COLD, G4_BEST, G5_WARM, G6_HEAT, G7, G8_HELL }
public enum WaterValues { B0_OCEAN_WATER, B1, B2_JUNGLE, B3_RESORT, B4_NORMAL_CLIMAT, B5_DRY_CLIMATE, B6_STEPPE, B7, B8_DESERT }

[Serializable] public struct HeightRGB 
{
    public static readonly int MAX_HEIGHT = 8;
    public static readonly int DEFAULT_HEIGHT = 5;
    public static readonly int MAX_TEMP = 6;

    public HeightValues R;
    public TempValues G;
    public WaterValues B;

    public HeightRGB(int R, int G, int B)
    {
        this.R = R <= MAX_HEIGHT ? (HeightValues)R : HeightValues.R8_EVEREST;
        this.G = G <= MAX_HEIGHT ? (TempValues)G : TempValues.G8_HELL;
        this.B = B <= MAX_HEIGHT ? (WaterValues)B : WaterValues.B8_DESERT;
    }

    private HeightRGB Normalized()
    {
        return this / this;
    }
    public HeightRGB DiagonalConnection()
    {
        return Normalized() * 2;
    }
    public HeightRGB LineConnection()
    {
        return Normalized() * 3;
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

    public static HeightRGB operator /(HeightRGB a, float b)
    => new HeightRGB((int)((int)a.R / b), (int)((int)a.G / b),(int)((int)a.B / b));
};