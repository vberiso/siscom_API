using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Services.Extension
{
    public static class CompareListExtensions
    {
        public static bool CompareList<T>(this List<T> list1, List<T> list2)
        {
            //if any of the list is null, return false

            if ((list1 == null && list2 != null) || (list2 == null && list1 != null))
                return false;

            //if both lists are null, return true, since its same
            else if (list1 == null && list2 == null)
                return true;
            //if count don't match between 2 lists, then return false
            if (list1.Count != list2.Count)
                return false;
            bool IsEqual = true;
            foreach (T item in list1)
            {
                T Object1 = item;
                T Object2 = list2.ElementAt(list1.IndexOf(item));
                Type type = typeof(T);
                //if any of the object inside list is null and other list has some value for the same object  then return false
                if ((Object1 == null && Object2 != null) || (Object2 == null && Object1 != null))
                {
                    IsEqual = false;
                    break;
                }

                foreach (System.Reflection.PropertyInfo property in type.GetProperties())
                {
                    if (property.Name != "ExtensionData")
                    {
                        string Object1Value = string.Empty;
                        string Object2Value = string.Empty;
                        if (type.GetProperty(property.Name).GetValue(Object1, null) != null)

                            Object1Value = type.GetProperty(property.Name).GetValue(Object1, null).ToString();
                        if (type.GetProperty(property.Name).GetValue(Object2, null) != null)
                            Object2Value = type.GetProperty(property.Name).GetValue(Object2, null).ToString();
                        //if any of the property value inside an object in the list didnt match, return false
                        if (Object1Value.Trim() != Object2Value.Trim())
                        {
                            IsEqual = false;
                            break;
                        }
                    }

                }

            }
            //if all the properties are same then return true
            return IsEqual;
        }
    }
}
