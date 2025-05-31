using System;

[Serializable]
public struct SeriazbleVector3
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public struct ReplayOutSide
{
    public SeriazbleVector3[] data;
}

