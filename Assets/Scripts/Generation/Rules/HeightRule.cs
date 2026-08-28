using System;
using UnityEngine;

public class HeightRule : Rule<float>
{
    public float minHeight;
    public float maxHeight;

    public float[] excludedStarts;
    public float[] excludedEnds;

    public HeightRule(float minHeight, float maxHeight, float[] excludedStarts, float[] excludedEnds)
    {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.excludedStarts = excludedStarts;
        this.excludedEnds = excludedEnds;
    }

    public override bool enforceRule(float playerHeight)
    {
        //Check allowed range
        if (playerHeight < minHeight || playerHeight > maxHeight)
            return false;

        //Check excluded ranges
        for (int i = 0; i < excludedStarts.Length; i++)
        {
            if (playerHeight >= excludedStarts[i] &&
                playerHeight <= excludedEnds[i])
            {
                return false;
            }
        }

        return true;
    }

}
