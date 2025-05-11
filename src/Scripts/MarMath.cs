using UnityEngine;

namespace MarMath;

public static class MarMathf
{
    /// <summary>
    /// Returns an Inverse Lerp value from -1 to 1.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float InverseLerpNegToPos(float a, float b, float value)
    {
        var inverseLerpResult = Mathf.InverseLerp(a, b, value);
        inverseLerpResult -= 0.5f; // Inverse lerp is 0 to 1, so make -0.5f to 0.5f.
        return inverseLerpResult * 2f; // Scale to -1 to 1.
    }
}
