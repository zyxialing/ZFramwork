using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ZFramework {

    public class ZStringUtil
    {
       public static List<string> ArrayStringToList(string[] strs)
        {
            List<string> tempList = new List<string>();
            if (strs == null)
            {
                return tempList;
            }
            for (int i = 0; i < strs.Length; i++)
            {
                tempList.Add(strs[i]);
            }
            return tempList;
        }


    }
}