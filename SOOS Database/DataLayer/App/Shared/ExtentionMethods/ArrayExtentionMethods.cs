
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataModels.App.Shared.ExtentionMethods
{
    static public class ArrayExtentionMethods
    {
        /// <summary>
        /// Checks if this object array contains only this T values
        /// </summary>
        /// <param name="array"></param>
        /// <param name="T"></param>
        /// <returns></returns>
      public static bool  IsArrayContainOnlyTValues(this object[] array, Type T)
      {
            foreach (var item in array)
            {
                if (item != null)
                    if (item.GetType() != T) return false;
            }
            return true;
      }
        /// <summary>
        /// Is array contains such int value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsArrayContainThisValue(this object[] array, int value)
        {
            foreach (object item in array)
            {
                if(item!=null)
                if ((int)item == value) return true;
            }
            return false;
        }
        /// <summary>
        /// Is array contains such double value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsArrayContainThisValue(this object[] array, double value)
        {
            foreach (object item in array)
            {
                if (item != null)
                    if ((double)item == value) return true;
            }
            return false;
        }
        /// <summary>
        /// Is array contains such bool value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsArrayContainThisValue(this object[] array, bool value)
        {
            foreach (object item in array)
            {
                if (item != null)
                    if ((bool)item == value) return true;
            }
            return false;
        }
        /// <summary>
        /// Is array contains such string value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsArrayContainThisValue(this object[] array, string value)
        {
            foreach (object item in array)
            {
                if (item != null)
                    if ((string)item == value) return true;
            }
            return false;
        }

    }
}