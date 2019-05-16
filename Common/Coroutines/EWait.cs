using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Common.Coroutines {
    /// <summary>
    /// Cache for coroutine wait instructions. Doesn't generate as much garbage.
    /// </summary>
    public static class EWait {
        private static readonly Dictionary<float, WaitForSeconds> ForSecondsCache = new Dictionary<float, WaitForSeconds>(100);

        public static WaitForEndOfFrame ForEndOfFrame { get; } = new WaitForEndOfFrame();

        public static WaitForFixedUpdate ForFixedUpdate { get; } = new WaitForFixedUpdate();

        public static WaitForSeconds ForSeconds(float seconds){
            if(!ForSecondsCache.ContainsKey(seconds))
                ForSecondsCache.Add(seconds, new WaitForSeconds(seconds));
            
            return ForSecondsCache[seconds];
        }
   
    }
}