using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using SkdAdminModel;


namespace SkdAdminClient.Tool
{
   public class ObjectToDataTable
    {
        public  static DataTable ToDataTable<T>(List<T> items)
        {
            try
            {
                var dt = new DataTable(typeof(T).Name);

                PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo prop in props)
                {
                    Type t = GetCoreType(prop.PropertyType);
                    object[] attrs = prop.GetCustomAttributes(typeof(RemarkAttribute), false);
                    foreach (RemarkAttribute attr in attrs)
                    {
                        if (attr.NeedExport)
                            dt.Columns.Add(attr.Remark, t);
                    }
                }

                foreach (T item in items)
                {
                    List<object> values = new List<object>();
                    foreach (PropertyInfo prop in props)
                    {
                        object valueObg = prop.GetValue(item, null);
                        string value = valueObg == null ? "" : valueObg.ToString();
                        object[] attrs = prop.GetCustomAttributes(typeof(RemarkAttribute), false);
                        if (dt.Columns.Cast<DataColumn>().Select(x => x.Caption).Contains(((RemarkAttribute)attrs[0]).Remark))
                        {
                            values.Add(value);
                        }
                    }
                    dt.Rows.Add(values.ToArray());
                }
                return dt;
            }
            catch (Exception err)
            {

                XMessageBox.ShowDialog(err.Message,"错误");
                return null;
            }
           

        
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

    }
}
