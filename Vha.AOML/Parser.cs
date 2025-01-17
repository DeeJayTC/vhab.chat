/*
* Vha.AOML
* Copyright (C) 2010-2011 Remco van Oosterhout
* See Credits.txt for all aknowledgements.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; version 2 of the License only.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vha.AOML
{
    public enum ParserMode
    {
        /// <summary>
        /// Allows only properly formatted attributes inside tags
        /// </summary>
        Strict,
        /// <summary>
        /// Allows attributes without " or ' wrapper inside tags
        /// </summary>
        Normal,
        /// <summary>
        /// Allows attributes without closing ' inside tags
        /// </summary>
        Compatibility
    }

    /// <summary>
    /// This class allows you to transform a string of AOML into a NodeCollection
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Configures the parser's sensitivity to parse errors
        /// </summary>
        public ParserMode Mode
        {
            get { return this.mode; }
            set
            {
                this.mode = value;
                this.regex = null;
            }
        }
        /// <summary>
        /// Whether to automatically convert newline symbols in a ContentNode to an OpenNode("br", "", true)
        /// </summary>
        public bool NewlineToBreak { get; set; }

        /// <summary>
        /// Whether to automatically convert invalid element nodes to content nodes
        /// </summary>
        public bool InvalidElementsToContent { get; set; }

        /// <summary>
        /// Parses an AOML string into open, close and content nodes
        /// </summary>
        /// <param name="aoml">The aoml string to be parsed</param>
        public NodeCollection Parse(string aoml)
        {
            NodeCollection nodes = new NodeCollection();
            // Create regular expression instance
           
            if (this.regex == null)
            {
                this.regex = new Regex(
                   "[<]\\s*(?<closer>[/]?)\\s*" +
                   "(?<name>[a-z]+)" +
                   "(\\s+((?<attribute>[a-z_\\-]+)\\s*" +
                   "(=\\s*(" +
                   /**/ (Mode == ParserMode.Strict ? "" : "(?<value>[^\\s<>'\"]+)|") +
                   /**/ "[\"](?<value>[^\"]*)[\"]|" +
                   /**/ (Mode == ParserMode.Compatibility ? "['](?<value>[^'>]*)[']?)|" : "['](?<value>[^']*)['])|") +
                   /**/ "(?<value>)" +
                   ")?)\\s*)*" +
                   "\\s*(?<closed>[/]?)[>]",
                    RegexOptions.IgnoreCase|RegexOptions.Compiled|RegexOptions.Multiline);
            }

            // Some setup
            int regexCloser = this.regex.GroupNumberFromName("closer");
            int regexName = this.regex.GroupNumberFromName("name");
            int regexAttribute = this.regex.GroupNumberFromName("attribute");
            int regexValue = this.regex.GroupNumberFromName("value");
            int regexClosed = this.regex.GroupNumberFromName("closed");
            // Convert to unix line endings
            aoml = aoml.Replace("\r\n", "\n");
            // Parse AOML
            MatchCollection matches = this.regex.Matches(aoml);
            int offset = 0;
            foreach (Match match in matches)
            {
                // Extract content
                if (match.Index > offset)
                {
                    string content = aoml.Substring(offset, match.Index - offset);
                    AddContent(nodes, content, content);
                }
                offset = match.Index + match.Length;
                // Extract tag
                string raw = match.Groups[0].Value;
                string name = match.Groups[regexName].Value;
                if (string.IsNullOrEmpty(name))
                {
                    // No name, let's assume it's just text
                    AddContent(nodes, raw, raw);
                    continue;
                }
                bool closer = !string.IsNullOrEmpty(match.Groups[regexCloser].Value);
                bool closed = !string.IsNullOrEmpty(match.Groups[regexClosed].Value);
                if (closer)
                {
                    // Closing tag
                    nodes.Add(new CloseNode(name, raw));
                }
                else
                {
                    // Opening tag
                    OpenNode node = new OpenNode(name, raw, closed);
                    // Add attributes
                    for (int i = 0; i < match.Groups[regexAttribute].Captures.Count; i++)
                    {
                        node.AddAttribute(
                            match.Groups[regexAttribute].Captures[i].Value,
                            match.Groups[regexValue].Captures[i].Value);
                    }
                    nodes.Add(node);
                }

            }
            // Remainder is content
            if (offset < aoml.Length)
            {
                string content = aoml.Substring(offset);
                AddContent(nodes, content, content);
            }
            return nodes;
        }

        /// <summary>
        /// Sanitizes the stream of nodes into exclusively valid AOML.
        /// This process involves changing and removing nodes.
        /// </summary>
        public void Sanitize(NodeCollection nodes)
        {
            if (nodes == null) { throw new ArgumentNullException(); }
            // Loop through all nodes
            nodes.Reset();
            Node node = null;
            while ((node = nodes.Next()) != null)
            {
                // Skip content nodes
                if (node.Type == NodeType.Content) { continue; }
                // Gather info
                OpenNode openNode = null;
                CloseNode closeNode = null;
                string name = null;
                if (node.Type == NodeType.Open)
                {
                    openNode = (OpenNode)node;
                    name = openNode.Name;
                    // Check attributes
                    for (int i = 0; i < openNode.Count; i++)
                    {
                        string attr = openNode.GetAttributeName(i);
                        if (!validAttributes.Contains(attr))
                        {
                            openNode.RemoveAttribute(attr);
                            i--;
                        }
                    }
                }
                else if (node.Type == NodeType.Close)
                {
                    closeNode = (CloseNode)node;
                    name = closeNode.Name;
                }
                // Singular elements
                if (singularElements.Contains(name))
                {
                    // Singular elements don't have closing nodes
                    if (closeNode != null)
                    {
                        nodes.Remove(closeNode);
                    }
                    // Singular elements are always self-closing
                    if (openNode != null)
                    {
                        openNode.Closed = true;
                    }
                    continue;
                }
                else if (inlineElements.Contains(name) || blockElements.Contains(name))
                {
                    if (openNode == null) { continue; }
                    // Let's not use self-closing elements here
                    if (openNode.Closed)
                    {
                        openNode.Closed = false;
                        nodes.InsertAfter(openNode, new CloseNode(name, ""));
                    }
                    continue;
                }
                // Replace node as content node
                if (this.InvalidElementsToContent)
                {
                    // Replace node as content
                    this.ReplaceContent(nodes, node, node.Raw, node.Raw);
                }
                else
                {
                    // Remove node
                    this.ReplaceContent(nodes, node, "", "");
                }
            }
            // And we're done!
            nodes.Reset();
        }

        /// <summary>
        /// Balances out the stream of nodes to form a valid tree.
        /// This process will always try to resolve conflicts by introducing extra inline elements over block elements.
        /// </summary>
        public void Balance(NodeCollection nodes)
        {
            if (nodes == null) { throw new ArgumentNullException(); }
            // Fill in missing elements
            List<string> nameStack = new List<string>();
            nodes.Reset();
            Node node = null;
            while ((node = nodes.Next()) != null)
            {
                switch (node.Type)
                {
                    case NodeType.Open:
                        OpenNode openNode = (OpenNode)node;
                        if (openNode.Closed) { continue; }
                        nameStack.Insert(0, openNode.Name);
                        break;
                    case NodeType.Close:
                        CloseNode closeNode = (CloseNode)node;
                        if (!nameStack.Contains(closeNode.Name))
                        {
                            // Found CloseNode without OpenNode
                            // Fix by insert an OpenNode before it
                            nodes.InsertBefore(
                                closeNode,
                                new OpenNode(closeNode.Name, "", false));
                        }
                        nameStack.Remove(closeNode.Name);
                        break;
                }
            }
            while (nameStack.Count > 0)
            {
                // Found OpenNode without CloseNode
                // Fix by adding a CloseNode at the end of the stream
                string name = nameStack[0];
                nameStack.RemoveAt(0);
                nodes.Add(new CloseNode(name, ""));
            }
            // Resolve tree structure
            List<OpenNode> nodeStack = new List<OpenNode>();
            nodes.Reset();
            while ((node = nodes.Next()) != null)
            {
                switch (node.Type)
                {
                    case NodeType.Open:
                        OpenNode on = (OpenNode)node;
                        if (on.Closed) { continue; }
                        nodeStack.Insert(0, on);
                        break;
                    case NodeType.Close:
                        // A bit of setup
                        CloseNode closeNode = (CloseNode)node;
                        bool block = blockElements.Contains(closeNode.Name);
                        // Find matching OpenNode
                        OpenNode openNode = null;
                        foreach (OpenNode o in nodeStack)
                        {
                            if (o.Name == closeNode.Name)
                            {
                                openNode = o;
                                break;
                            }
                        }
                        if (openNode == null)
                        {
                            throw new InvalidOperationException("Unable to find matching OpenNode to CloseNode");
                        }
                        // Handle incorrect balance
                        int offset = 0;
                        while (nodeStack[offset].Name != closeNode.Name)
                        {
                            OpenNode n = nodeStack[offset];
                            if (block)
                            {
                                nodes.InsertBefore(node, new CloseNode(n.Name, ""));
                                nodes.InsertAfter(node, n.Clone());
                                nodeStack.RemoveAt(offset);
                                offset--;
                            }
                            else
                            {
                                nodes.InsertBefore(n, closeNode.Clone());
                                nodes.InsertAfter(n, openNode.Clone());
                            }
                            offset++;
                        }
                        // All fixed
                        nodeStack.RemoveAt(offset);
                        break;
                }
            }
            // And we're done!
            nodes.Reset();
        }

        #region Constructors
        public Parser()
        {
            // Default values
            this.NewlineToBreak = false;
            this.InvalidElementsToContent = true;

            // Create lists
            this.singularElements = new List<string>(new string[] { 
                "br",
                "img" });
            this.inlineElements = new List<string>(new string[] {
                "font",
                "a",
                "u",
                "i"
            });
            this.blockElements = new List<string>(new string[]{
                "div",
                "center",
                "left",
                "right"
            });
            this.validAttributes = new List<string>(new string[] {
                "color",
                "align",
                "href",
                "style",
                "src"
            });   
        }
        #endregion

        #region Internal
        private Regex regex = null;
        private ParserMode mode = ParserMode.Normal;
        private List<string> singularElements;
        private List<string> inlineElements;
        private List<string> blockElements;
        private List<string> validAttributes;

        private Node[] CreateContent(string content, string raw)
        {
            List<Node> nodes = new List<Node>();
            if (string.IsNullOrEmpty(content))
            {
                return nodes.ToArray();
            }
            if (this.NewlineToBreak)
            {
                string[] contentParts = content.Split('\n');
                string[] rawParts = raw != null ? raw.Split('\n') : new string[] { };
                int i = 0;
                foreach (string c in contentParts)
                {
                    nodes.Add(new ContentNode(c, raw.Length > i ? rawParts[i] : ""));
                    if (i + 1 < contentParts.Length)
                    {
                        nodes.Add(new OpenNode("br", raw.Length > i ? "\n" : "", true));
                    }
                    i++;
                }
            }
            else
            {
                nodes.Add(new ContentNode(content, raw));
            }
            return nodes.ToArray();
        }

        private void AddContent(NodeCollection nodes, string content, string raw)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException();
            }
            Node[] nn = CreateContent(content, raw);
            foreach (Node n in nn)
            {
                nodes.Add(n);
            }
        }

        private void ReplaceContent(NodeCollection nodes, Node target, string content, string raw)
        {
            if (nodes == null || target == null)
            {
                throw new ArgumentNullException();
            }
            Node[] nn = CreateContent(content, raw);
            foreach (Node n in nn)
            {
                nodes.InsertBefore(target, n);
            }
            nodes.Remove(target);
        }
        #endregion
    }
}
