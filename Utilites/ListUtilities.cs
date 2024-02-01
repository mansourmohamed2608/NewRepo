using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{
    
        public static class ListUtilities
        {
            public static DataTable ConvertToDataTable<T>(List<T> list)
            {
                DataTable dataTable = new DataTable();
                if (list == null || list.Count == 0)
                    return dataTable;

                
                var properties = typeof(T).GetProperties();

                
                foreach (var property in properties)
                {
                    dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }

                
                foreach (var item in list)
                {
                    DataRow row = dataTable.NewRow();
                    foreach (var property in properties)
                    {
                        object value = property.GetValue(item);
                        row[property.Name] = value ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }

            public static List<T> ConvertToList<T>(DataTable dataTable) where T : new()
            {
                List<T> list = new List<T>();

                if (dataTable == null || dataTable.Rows.Count == 0)
                    return list;

                var properties = typeof(T).GetProperties();

                foreach (DataRow row in dataTable.Rows)
                {
                    T item = new T();
                    foreach (var property in properties)
                    {
                        if (dataTable.Columns.Contains(property.Name))
                        {
                            object value = row[property.Name];
                            if (value != DBNull.Value)
                            {
                                property.SetValue(item, value);
                            }
                            else if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                            {
                                property.SetValue(item, null);
                            }
                        }
                    }
                    list.Add(item);
                }

                return list;
            }
        }
    
}
