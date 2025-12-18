using System;
using UnityEngine;
using System.Collections;
using R3;

namespace UniRxWorkBook
{
    public static class ObservableExtension
    {
        public static Observable<T> _____<T>(this Observable<T> source)
        {
            return source;
        }
    }
}
