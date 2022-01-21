using System;
using UnityEngine;

public static class Functions
{
    public static U TryGenericConversion<T, U>(T value)
    {
        U output = default(U);

        try
        {
            output = (U)Convert.ChangeType(value, typeof(U));
        }
#pragma warning disable CS0168
        catch (Exception e) { }
#pragma warning restore CS0168

        return output;
    }

    public static string PointToString(Vector2 point) =>
        "(" + point.x + "," + point.y + ")";

    public static Vector2 StringToPoint(string s)
    {
        Vector2 output = new Vector2();
        string[] data = s.Split(',');

        if (data.Length >= 1)
        {
            float.TryParse(data[0].Substring(1, data[0].Length - 1), out float x);
            output.x = x;
        }

        if (data.Length >= 2)
        {
            float.TryParse(data[1].Substring(1, data[1].Length - 1), out float y);
            output.y = y;
        }

        return output;
    }
}