using System;
using UnityEngine;

public class HeightRule : Rule<float>
{
    private float minHeight;
    private float maxHeight;
    private float[] excludedRangesStart;   //start of excluded ranges
    private float[] excludedRangesEnd;     //end of excluded ranges

    public HeightRule(float minHeight, float maxHeight, float[] excludedRangesStart, float[] excludedRangesEnd)
    {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.excludedRangesStart = excludedRangesStart;
        this.excludedRangesEnd = excludedRangesEnd;
    }

    public override bool enforceRule(float playerHeight)
    {
        //Check allowed range
        if (playerHeight < minHeight || playerHeight > maxHeight)
            return false;

        //Check excluded ranges
        for (int i = 0; i < excludedRangesStart.Length; i++)
        {
            if (playerHeight >= excludedRangesStart[i] &&
                playerHeight <= excludedRangesEnd[i])
            {
                return false;
            }
        }

        return true;
    }

}
