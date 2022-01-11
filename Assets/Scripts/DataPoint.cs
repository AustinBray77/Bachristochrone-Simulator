//Imports
using System;

//Generic class to store a 2D data point (the yValue must be able to compare)
public class DataPoint<T, U> where U : IComparable {
    //Instance Refrence For X and Y values
    private T xValue;
    private U yValue;

    //Base Constructor
    public DataPoint(T xValue, U yValue) {
        this.xValue = xValue;
        this.yValue = yValue;
    }

    //Getters For X and Y value
    public T GetXValue() {
        return xValue;
    }

    public U GetYValue() {
        return yValue;
    }

    //Implementation for comparison operators, compares the dependant variables(y) to determine which is greater
    public static bool operator>(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.yValue.CompareTo(b.yValue) > 0;

    public static bool operator<(DataPoint<T, U> a, DataPoint<T, U> b) =>
        a.yValue.CompareTo(b.yValue) < 0;

    //Override for to string
    public override string ToString() =>
        xValue + " " + yValue;
}