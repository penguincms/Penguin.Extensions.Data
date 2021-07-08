using Penguin.FileStreams;
using Penguin.FileStreams.Interfaces;
using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Penguin.Extensions.Data
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class DataTableExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Writes a DataTable to a CSV file specified by the path
        /// </summary>
        /// <param name="dataTable">The datatable to write</param>
        /// <param name="filePath">The location of the CSV to create</param>
        /// <param name="settings">Optional csv settings</param>
        public static void ToCsv(this DataTable dataTable, string filePath, ToCsvSettings settings = null)
        {
            settings = settings ?? new ToCsvSettings();

            using (IFileWriter fileWriter = FileWriterFactory.GetFileWriter(filePath, settings.Compression))
            {
                fileWriter.Write(dataTable.ToCsv(settings));
            }
        }

        /// <summary>
        /// Creates a CSV row from the provided data row
        /// </summary>
        /// <param name="dr">The data row to convert</param>
        /// <param name="quoteCharacter">An optional character to use for quoting items</param>
        /// <returns></returns>
        public static string ToCsvRow(this DataRow dr, char? quoteCharacter = '"')
        {


            if (dr is null)
            {
                throw new ArgumentNullException(nameof(dr));
            }

            string qReplace = null;

            if (quoteCharacter.HasValue)
            {
                qReplace = $"{quoteCharacter.Value}{quoteCharacter.Value}";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                if (i != 0)
                {
                    sb.Append(',');
                }

                if (quoteCharacter.HasValue)
                {
                    sb.Append(quoteCharacter.Value);
                }

                string oVal = ObjectToString(dr.ItemArray[i]);

                if (quoteCharacter.HasValue)
                {
                    oVal = oVal.Replace($"{quoteCharacter.Value}", qReplace);
                }

                sb.Append(oVal);

                if (quoteCharacter.HasValue)
                {
                    sb.Append(quoteCharacter.Value);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes a datatable as a CSV string and returns the string
        /// </summary>
        /// <param name="dataTable">The data source</param>
        /// <returns>A CSV representation of the data table</returns>
        /// <param name="settings">Optional csv settings</param>
        public static string ToCsv(this DataTable dataTable, ToCsvSettings settings = null)
        {
            Contract.Requires(dataTable != null);

            StringBuilder fileContent = new StringBuilder();

            bool firstLine = true;

            settings = settings ?? new ToCsvSettings();

            if (settings.IncludeHeaders)
            {
                fileContent.Append(string.Join(",", dataTable.Columns.Cast<DataColumn>())); //ToString for the header name?
                firstLine = false;
            }

            foreach (DataRow dr in dataTable.Rows)
            {
                if (!firstLine)
                {
                    fileContent.Append(System.Environment.NewLine);
                }

                fileContent.Append(dr.ToCsvRow(settings.QuoteCharacter));

                firstLine = false;
            }

            return fileContent.ToString();
        }

        private static string ObjectToString(object o)
        {
            if (o is DateTime dt)
            {
                return $"{dt:yyyy-MM-dd HH:mm:ss.fff}";
            }
            else
            {
                return $"{o}";
            }
        }

        /// <summary>
        /// Adds an item to the data row, adding the column if needed. 
        /// If the column exists, updates the existing value
        /// </summary>
        /// <param name="dr">The data row to update</param>
        /// <param name="column">The column to target</param>
        /// <param name="value">The value to add or update</param>
        public static void AddOrUpdate(this DataRow dr, string column, object value)
        {
            if (dr is null)
            {
                throw new ArgumentNullException(nameof(dr));
            }

            dr.Table.EnsureColumn(column);
            dr[column] = value;
        }

        /// <summary>
        /// Adds an object to the data table as a data row, and then returns the new row
        /// </summary>
        /// <param name="dt">The data table to target</param>
        /// <param name="toAdd">The object to add to the table</param>
        /// <returns>The newly created data row containing the object information</returns>
        public static DataRow Add(this DataTable dt, object toAdd)
        {
            if (dt is null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            if (toAdd is null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            DataRow newRow = dt.NewRow();

            foreach (PropertyInfo pi in toAdd.GetType().GetProperties())
            {
                if (pi.GetGetMethod() != null)
                {
                    object val = pi.GetValue(toAdd);

                    if (val is null)
                    {
                        newRow[pi.Name] = DBNull.Value;
                    }
                    else
                    {
                        newRow[pi.Name] = val;
                    }
                }
            }

            dt.Rows.Add(newRow);

            return newRow;
        }

        /// <summary>
        /// Returns true if the data table contains the requested column
        /// </summary>
        /// <param name="dt">The data table to check</param>
        /// <param name="columnName">The column to check for</param>
        /// <param name="comparison">An optional string comparison to use when checking for the column name. Defaults to OrdinalIgnoreCase</param>
        /// <returns>True if a column with a matching name is found</returns>
        public static bool ContainsColumn(this DataTable dt, string columnName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (dt is null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentException($"'{nameof(columnName)}' cannot be null or empty.", nameof(columnName));
            }

            return dt.Columns.Cast<DataColumn>().Any(dc => string.Equals(columnName, dc.ColumnName, comparison));
        }

        /// <summary>
        /// Sets up a data table to have columns representative of properties found on a given type
        /// </summary>
        /// <param name="dt">The data table to scaffold</param>
        /// <param name="toScaffold">The type to use to create the columns on the table</param>
        public static void Scaffold(this DataTable dt, Type toScaffold)
        {
            if (dt is null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            if (toScaffold is null)
            {
                throw new ArgumentNullException(nameof(toScaffold));
            }

            foreach (PropertyInfo pi in toScaffold.GetProperties())
            {
                if (pi.GetGetMethod() != null)
                {
                    dt.EnsureColumn(pi.Name, pi.PropertyType);
                }
            }
        }

        /// <summary>
        /// Creates a column on a table if it doesn't already exist
        /// </summary>
        /// <param name="dt">The data table to target</param>
        /// <param name="columnName">The name of the column to ensure</param>
        /// <param name="columnType">The type of the data contained in the column</param>
        /// <returns>True if the column already existed. False if it was created</returns>
        public static bool EnsureColumn(this DataTable dt, string columnName, Type columnType = null)
        {
            if (dt is null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentException($"'{nameof(columnName)}' cannot be null or empty.", nameof(columnName));
            }


            columnType = columnType ?? typeof(string);

            if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                columnType = Nullable.GetUnderlyingType(columnType);
            }

            if (dt.ContainsColumn(columnName))
            {
                return true;
            }

            dt.Columns.Add(columnName, columnType);

            return false;
        }

        /// <summary>
        /// Returns an HTML string table representation of a data table
        /// </summary>
        /// <param name="dt">The data table to convert</param>
        /// <returns>An HTML string table representation of a data table</returns>
        public static string ToHtmlTable(this DataTable dt)
        {
            Contract.Requires(dt != null);

            StringBuilder body = new StringBuilder();

            body.Append("<table><tr>");

            foreach (DataColumn dc in dt.Columns)
            {
                body.Append($"<th>{dc.ColumnName}</th>");
            }

            body.Append("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                body.Append("<tr>");

                foreach (object o in dr.ItemArray)
                {
                    body.Append($"<td>{o}</td>");
                }

                body.Append("</tr>");
            }

            body.Append("</table>");

            return body.ToString();
        }
    }
}