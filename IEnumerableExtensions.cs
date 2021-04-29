using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Penguin.Extensions.Data
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class IEnumerableExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        /// <summary>
        /// Converts an IEnumerable of objects to a data table, with property names as headers and values as items
        /// </summary>
        /// <param name="objList">The IEnumerable of objects to use as a data source</param>
        /// <returns>A data table containing the object values</returns>
        public static DataTable ToDataTable(this System.Collections.Generic.IEnumerable<object> objList)
        {
            if (objList is null)
            {
                throw new ArgumentNullException(nameof(objList));
            }

            DataTable thisTable = new DataTable();

            Type objectType = objList.GetType().GenericTypeArguments[0];

            List<PropertyInfo> Properties = new List<PropertyInfo>();

            Dictionary<PropertyInfo, int> PropertyOrder = new Dictionary<PropertyInfo, int>();

            foreach (PropertyInfo thisProp in objectType.GetProperties().Reverse())
            {
                int index = 0;

                while (index < Properties.Count && PropertyOrder[Properties.ElementAt(index)] > index)
                {
                    index++;
                }
                Properties.Insert(index, thisProp);
                PropertyOrder.Add(thisProp, index);
            }

            foreach (PropertyInfo thisProperty in Properties)
            {
                DisplayNameAttribute displayNameAttribute = thisProperty.GetCustomAttribute<DisplayNameAttribute>();
                string DisplayName;
                if (displayNameAttribute != null)
                {
                    DisplayName = displayNameAttribute.DisplayName;
                }
                else
                {
                    DisplayName = thisProperty.Name;
                }

                thisTable.Columns.Add(DisplayName);
            }

            foreach (object thisObj in objList)
            {
                DataRow thisRow = thisTable.NewRow();
                int i = 0;
                foreach (PropertyInfo thisProperty in Properties)
                {
                    thisRow[i++] = thisProperty.GetValue(thisObj);
                }

                thisTable.Rows.Add(thisRow);
            }
            return thisTable;
        }

        #endregion Methods
    }
}