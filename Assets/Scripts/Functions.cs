using System;
using System.Collections.Generic;
using UnityEngine;

//Static class for all commonly used functions
public static class Functions
{
    //Method to try and convert a generic type from one type to another
    public static U TryGenericConversion<T, U>(T value)
    {
        //Set the output to the types default value
        U output = default(U);

        //Try and convert the data
        try
        {
            output = (U)Convert.ChangeType(value, typeof(U));
        }
        //Disable warnings, don't log the error
#pragma warning disable CS0168
        catch (Exception e) { }
#pragma warning restore CS0168

        //Return the value
        return output;
    }

    //Method to convert a Vector2 to a string (Unity's base method doesn't always work)
    public static string PointToString(Vector2 point) =>
        "(" + point.x + "," + point.y + ")";

    //Method to convert a string to a Vector2
    public static Vector2 StringToPoint(string s)
    {
        //Make a base output value and split the input data
        Vector2 output = new Vector2();
        string[] data = s.Split(',');

        //If the inputted data is in the correct format, try and parse the data
        if (data.Length >= 1)
        {
            float.TryParse(data[0].Substring(1, data[0].Length - 1), out float x);
            output.x = x;
        }

        //If the inputted data is in the correct format, try and parse the data
        if (data.Length >= 2)
        {
            float.TryParse(data[1].Substring(1, data[1].Length - 1), out float y);
            output.y = y;
        }

        //Return the resulting Vector2
        return output;
    }

    //Method to round a float value to the specified amount of places
    public static float RoundToDecimalPlaces(float f, int places) =>
        Mathf.Round(f * Mathf.Pow(10, places)) / Mathf.Pow(10, places);

    //Method to quick sort a list of data points
    public static void QSortData<T, U>(List<DataPoint<T, U>> data, int min, int max) /*Types must allow comparisons */ where T : IComparable where U : IComparable
    {
        //If the right head has not passed the left head yet, continue sorting
        if (min < max)
        {
            //Find the middle pivot point and make swaps
            int pivot = Partition<T, U>(data, min, max);

            //Sort each unsorted side
            QSortData(data, min, pivot);
            QSortData(data, pivot + 1, max);
        }
    }

    //Method to find a split point in an array where each side is lesser or greater than the split point
    public static int Partition<T, U>(List<DataPoint<T, U>> data, int min, int max) /*Types must allow comparisons */ where T : IComparable where U : IComparable
    {
        //Start at the left value
        int swap = min;

        //Loop until the the right head is reached
        for (int i = min + 1; i < max; i++)
        {
            //If the data is less than the front data, swap them and increment the swapper
            if (data[i] < data[min])
            {
                swap++;

                DataPoint<T, U> holder;
                holder = data[i];
                data[i] = data[swap];
                data[swap] = holder;
            }
        }

        //Make a final swap to place the pivot in the correct position
        DataPoint<T, U> hlder;
        hlder = data[min];
        data[min] = data[swap];
        data[swap] = hlder;

        //Return the pivot point
        return swap;
    }
}