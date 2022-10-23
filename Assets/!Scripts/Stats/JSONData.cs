using System.IO;
using UnityEngine;

namespace JSONData
{
    public static class Access
    {
        public static Stats GetStats(string subpath)
        {
            string path = $"{Application.dataPath}/Stats/{subpath}.json";
            string file = File.ReadAllText(path);
            return JsonUtility.FromJson<Stats>(file);
        }
    }

    [System.Serializable]
    public struct Stats
    {
        public float Health;
        /// <summary>
        /// Unity-units per second
        /// </summary>
        public float MaxSpeed; 
        public float DamagePerCannon;
    }
}
