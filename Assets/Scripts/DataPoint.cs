#pragma warning disable CS0660
#pragma warning disable CS0661
//Imports
using System;

//Generic class to store a 2D data point (the yValue must be able to compare)
[Serializable]
public class DataPoint<T, U> /*Types must allow comparisons */ where T : IComparable where U : IComparable
{
    //Instance Refrence For X and Y values (public get, private set)
    public T xValue { get; private set; }
    public U yValue { get; private set; }

    //Base Constructor
    public DataPoint(T xValue, U yValue)
    {
        this.xValue = xValue;
        this.yValue = yValue;
    }

    //Constructor from a string
    public DataPoint(string value)
    {
        string[] vals = value.Split(',');

        //If the inputted data is in the correct format, try and parse the data
        if (vals.Length >= 1)
        {
            xValue = Functions.TryGenericConversion<string, T>(vals[0]);
        }
        //Else set the data to its types default value
        else
        {
            xValue = default(T);
        }

        //If the inputted data is in the correct format, try and parse the data
        if (vals.Length >= 2)
        {
            yValue = Functions.TryGenericConversion<string, U>(vals[1]);
        }
        //Else set the data to its types default value
        else
        {
            yValue = default(U);
        }
    }

    //Implementation for comparison operators, compares the dependant variables(y) to determine which is greater
    public static bool operator >(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.yValue.CompareTo(b.yValue) > 0;

    public static bool operator <(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.yValue.CompareTo(b.yValue) < 0;

    public static bool operator >=(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.yValue.CompareTo(b.yValue) >= 0;

    public static bool operator <=(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.yValue.CompareTo(b.yValue) <= 0;

    public static bool operator ==(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.xValue.CompareTo(b.xValue) == 0 && a.yValue.CompareTo(b.yValue) == 0;

    public static bool operator !=(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.xValue.CompareTo(b.xValue) != 0 || a.yValue.CompareTo(b.yValue) != 0;

    //Override for to string
    public override string ToString() =>
        xValue + "," + yValue;
}