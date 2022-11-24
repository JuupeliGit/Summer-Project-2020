using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class MyExtension
    {
        // For checking if given indexes are out of range of the given 2D array.
        public static bool IndexOutOfRange<T>(this T[,] arr, Vector2Int indexes)
        {
            bool temp = (indexes.x >= 0 && indexes.x <= arr.GetLength(0) - 1 && indexes.y >= 0 && indexes.y <= arr.GetLength(1) - 1);
            return !temp;
        }

        // For checking if value is equal to the value in given cell in given 2D array.
        public static bool CheckForValue<T>(this T[,] arr, Vector2Int indexes, T value)
        {
            Comparer<T> comp = Comparer<T>.Default;
            return (!arr.IndexOutOfRange(indexes) && comp.Compare(arr[indexes.x, indexes.y], value) == 0);
        }
    }
}
