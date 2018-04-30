using UnityEngine;
using System.Collections.Generic;

public static class Extensions {

    public static T GetRandom<T>(this T[] array) {
        int rnd = Random.Range(0, array.Length);
        return array[rnd];
    }

    public static T GetRandom<T>(this List<T> list) {
        int rnd = Random.Range(0, list.Count);
        return list[rnd];
    }
}
