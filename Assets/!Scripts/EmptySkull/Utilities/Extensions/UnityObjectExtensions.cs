using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EmptySkull.Utilities
{
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Removes an assigned GameObject from the list and additionally destroyes it. This will only work,
        /// when the GameObject is part of the List.
        /// </summary>
        /// <param name="objList">The GameObject-List.</param>
        /// <param name="obj">The list-object to destroy.</param>
        public static void DestoryGameObjectFromList(this List<GameObject> objList, GameObject obj)
        {
            if(!objList.Contains(obj))
                return;

            objList.Remove(obj);
            Object.Destroy(obj);
        }

        /// <summary>
        /// Removes an assigned GameObject by its index from the list and additionally destroyes it. This will only work,
        /// when the assigned index is valid.
        /// </summary>
        /// <param name="objList">The GameObject-List.</param>
        /// <param name="objIndex">The index of the list-object to destroy.</param>
        public static void DestoryGameObjectFromList(this List<GameObject> objList, int objIndex)
        {
            if(objIndex < 0 || objIndex >= objList.Count)
                return;

            GameObject obj = objList[objIndex];

            objList.Remove(obj);
            Object.Destroy(obj);
        }

        /// <summary>
        /// Removes and destorys all GameObjects from a List. The objects can be filtered by a predicate to only remove
        /// and destroy specific GameObjects.
        /// </summary>
        /// <param name="objList">The GameObject-List.</param>
        /// <param name="filterPredicate">The predicate by which the GameObjects are filtered. When 'null' (like as default) no 
        /// GameObject will be filtered - meaning all GameObjects will be removed and destroyed.</param>
        public static void DestroyAllGameObjectsFromList(this List<GameObject> objList, Func<GameObject, bool> filterPredicate = null)
        {
            for (int i = 0; i < objList.Count; i++)
            {
                GameObject obj = objList[i];
                if (!(filterPredicate?.Invoke(obj) ?? true))
                    continue;

                DestoryGameObjectFromList(objList, obj);
                i--;
            }
        }
    }
}
