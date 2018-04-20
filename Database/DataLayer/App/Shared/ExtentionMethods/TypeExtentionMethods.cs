using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Shared.ExtentionMethods
{
    internal static class TypeExtentionMethods
    {
        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            else
            {
                return null;
            }
        }
       

    }
}
