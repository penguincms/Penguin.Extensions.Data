using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
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
        /// <param name="includeHeaders">Write column headers to the output string</param>
        /// <param name="quoteValues">Should the values be quoted</param>
        public static void ToCsv(this DataTable dataTable, string filePath, bool includeHeaders = true, bool quoteValues = true) => System.IO.File.WriteAllText(filePath, dataTable.ToCsv(includeHeaders, quoteValues));

        /// <summary>
        /// Writes a DataTable to a CSV file specified by the path
        /// </summary>
        /// <param name="dataTable">The datatable to write</param>
        /// <param name="filePath">The location of the CSV to create</param>
        /// <param name="includeHeaders">Write column headers to the output string</param>
        /// <param name="quoteValues">Should the values be quoted</param>
        [Obsolete("Use ToCsv", false)]
        public static void ToCSV(this DataTable dataTable, string filePath, bool includeHeaders = true, bool quoteValues = true) => System.IO.File.WriteAllText(filePath, dataTable.ToCsv(includeHeaders, quoteValues));

        /// <summary>
        /// Writes a datatable as a CSV string and returns the string
        /// </summary>
        /// <param name="dataTable">The data source</param>
        /// <returns>A CSV representation of the data table</returns>
        /// <param name="includeHeaders">Write column headers to the output string</param>
        /// <param name="quoteValues">Should the values be quoted</param>
        [Obsolete("Use ToCsv", false)]
        public static string ToCSV(this DataTable dataTable, bool includeHeaders = true, bool quoteValues = true) => dataTable.ToCsv(includeHeaders, quoteValues);

        /// <summary>
        /// Writes a datatable as a CSV string and returns the string
        /// </summary>
        /// <param name="dataTable">The data source</param>
        /// <returns>A CSV representation of the data table</returns>
        /// <param name="includeHeaders">Write column headers to the output string</param>
        /// <param name="quoteValues">Should the values be quoted</param>
        public static string ToCsv(this DataTable dataTable, bool includeHeaders = true, bool quoteValues = true)
        {
            Contract.Requires(dataTable != null);

            StringBuilder fileContent = new StringBuilder();

            bool firstLine = true;
            if (includeHeaders)
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
                
                fileContent.Append(string.Join(",", dr.ItemArray.Select(item => quoteValues ? $"\"{$"{item}".Replace("\"", "\"\"")}\"" : $"{item}")));

                firstLine = false;
            }

            return fileContent.ToString();
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