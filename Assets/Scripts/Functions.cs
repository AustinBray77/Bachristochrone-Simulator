using System;
using System.Collections.Generic;
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

    public static float RoundToDecimalPlaces(float f, int places) =>
        Mathf.Round(f * Mathf.Pow(10, places)) / Mathf.Pow(10, places);

    public static void QSortData<T, U>(List<DataPoint<T, U>> data, int min, int max)
    where T : IComparable where U : IComparable
    {
        if (min < max)
        {
            int pivot = Partition<T, U>(data, min, max);
            QSortData(data, min, pivot);
            QSortData(data, pivot + 1, max);
        }
    }

    public static int Partition<T, U>(List<DataPoint<T, U>> data, int min, int max)
    where T : IComparable where U : IComparable
    {
        int swap = min;

        for (int i = min + 1; i < max; i++)
        {
            if (data[i] < data[min])
            {
                swap++;

                DataPoint<T, U> holder;
                holder = data[i];
                data[i] = data[swap];
                data[swap] = holder;
            }
        }

        DataPoint<T, U> hlder;
        hlder = data[min];
        data[min] = data[swap];
        data[swap] = hlder;

        return swap;
    }
}