using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Penguin.Extensions.Data
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class XNodeExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        /// <summary>
        /// Returns a list of all child nodes by name
        /// </summary>
        /// <param name="node">The node to search</param>
        /// <param name="childName">The child name to search for</param>
        /// <returns>All children matching the name</returns>
        public static List<XNode> GetAllChildrenByName(this XNode node, string childName)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (childName is null)
            {
                throw new ArgumentNullException(nameof(childName));
            }

            if (node is XElement xelementnode)
            {
                xelementnode.GetAllChildrenByName(childName);
            }
            else
            {
                throw new ArgumentException("Node must be a valid XElement", nameof(node));
            }

            throw new Exception("I have no idea how this error occurred");
        }

        /// <summary>
        /// Returns a list of all child nodes by name
        /// </summary>
        /// <param name="node">The node to search</param>
        /// <param name="childName">The child name to search for</param>
        /// <returns>All children matching the name</returns>

        public static List<XNode> GetAllChildrenByName(this XElement node, string childName)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (string.IsNullOrEmpty(childName))
            {
                throw new ArgumentException($"'{nameof(childName)}' cannot be null or empty", nameof(childName));
            }

            return node.DescendantNodes().Where(n => string.Equals((n as XElement)?.Name?.LocalName, childName, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        #endregion Methods
    }
}