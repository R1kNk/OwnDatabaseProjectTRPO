using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Shared.ExtentionMethods
{
    public static class TypeExtentionMethods
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
        public static bool isObjectEquals(this object arg, object argument, Type type)
        {
            if(type ==typeof(int))
            {
                if ((int)arg == (int)argument) return true;
                return false;
            }
            if (type == typeof(double))
            {
                if ((double)arg == (double)argument) return true;
                return false;
            }
            if (type == typeof(string))
            {
                if ((string)arg == (string)argument) return true;
                return false;
            }
            if (type == typeof(bool))
            {
                if ((bool)arg == (bool)argument) return true;
                return false;
            }
            return false;
        }
       
        public static object[] toObjectArray(this object arg)
        {
            return new object[]{arg};
        }
        public static object[] toObjectArray(this int arg)
        {
            return new object[] { arg };
        }
        public static object[] toObjectArray(this double arg)
        {
            return new object[] { arg };
        }
        public static object[] toObjectArray(this string arg)
        {
            return new object[] { arg };
        }
        public static object[] toObjectArray(this bool arg)
        {
            return new object[] { arg };
        }
    }
}
