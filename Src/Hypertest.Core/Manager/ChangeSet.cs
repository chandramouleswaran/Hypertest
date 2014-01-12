#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Hypertest.Core.Manager
{
    public class ChangeSet
    {
        public ChangeSet(string description)
        {
            Changes = new List<Change>();
            Description = description;
        }

        public ChangeSet(Change change, string description) : this(description)
        {
            Changes.Add(change);
        }

        internal IList<Change> Changes { get; set; }
        public string Description { get; private set; }

        internal void Undo()
        {
            foreach (Change change in Changes.Reverse())
            {
                change.Undo();
            }
        }

        internal void Redo()
        {
            foreach (Change change in Changes)
            {
                change.Redo();
            }
        }

        public override string ToString()
        {
            return Description;
        }
    }
}