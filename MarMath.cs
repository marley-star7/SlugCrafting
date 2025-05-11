using UnityEngine;
using RWCustom;

namespace MarMath;

public static class MarMathf
{
    public static float InverseLerpNegToPos(float a, float b, float c)
    {
        var inverseLerpResult = Mathf.InverseLerp(a, b, c);
        inverseLerpResult -= 0.5f; // Inverse lerp is 0 to 1, so make -0.5f to 0.5f.
        return inverseLerpResult * 2f; // Scale to -1 to 1.
    }
}
