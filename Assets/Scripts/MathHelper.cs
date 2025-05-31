
public static class MathHelper
{
    public static float Map(float n, float start1, float stop1, float start2, float stop2)
    {
        return (n - start1) / (stop1 - start1) * (stop2 - start2) + start2;
    }
}
