using Penguin.FileStreams;

namespace Penguin.Extensions.Data
{
    /// <summary>
    /// Settings to use when converting a DataTable to a CSV
    /// </summary>
    public class ToCsvSettings
    {
        /// <summary>
        /// Serialize the headers to the first row of the file. Default true
        /// </summary>
        public bool IncludeHeaders { get; set; } = true;

        /// <summary>
        /// An optional character to use to quote items. defaults to "
        /// </summary>
        public char? QuoteCharacter { get; set; } = '"';

        /// <summary>
        /// Compression to use when writing CSV files. Default None.
        /// The output file will have the correct extension appended
        /// </summary>
        public FileStreamCompression Compression { get; set; } = FileStreamCompression.None;
    }
}
