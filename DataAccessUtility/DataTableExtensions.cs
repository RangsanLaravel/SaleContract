﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataAccessUtility
{
    public static class DataTableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            //if (table.Rows.Count==0)
            //{

            //    yield return new T();
            //}
            //else
            //{
            foreach (var row in table.Rows)
            {
                yield return CreateItemFromRow<T>(row as DataRow, properties);
            }
            //}
        }
        public static T AsEnumerable<T>(this DataRow row) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            return CreateItemFromRow<T>(row as DataRow, properties);
        }
        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite) continue;
                if (!row.Table.Columns.Contains(property.Name)) continue;

                if (DBNull.Value != row[property.Name])
                {
                    property.SetValue(item, TypeExtension.ChangeType(row[property.Name], property.PropertyType), null);
                }
            }

            return item;
        }
    }
}
