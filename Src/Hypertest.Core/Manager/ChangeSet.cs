using System.Collections.Generic;
using System.Linq;

namespace Hypertest.Core.Manager
{
    public class ChangeSet
    {
        public ChangeSet(string description)
        {
            Changes = new List<Change>();
            this.Description = description;
        }

        public ChangeSet(Change change, string description) : this(description)
        {
            Changes.Add(change);
        }
        internal IList<Change> Changes { get; set; }

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

        public string Description { get; private set; }
    }
}
