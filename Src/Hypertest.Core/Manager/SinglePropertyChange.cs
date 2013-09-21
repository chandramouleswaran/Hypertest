using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Hypertest.Core.Tests;
using Wide.Utils;

namespace Hypertest.Core.Manager
{
    public class SinglePropertyChange : Change
    {
        public SinglePropertyChange(object obj1, object obj2, string property)
        {
            this.Object1 = obj1;
            this.Object2 = obj2;
            this.property = property;
        }

        public object Object1 { get; internal set; }

        public object Object2 { get; internal set; }

        public string property { get; internal set; }

        internal override void Undo()
        {
            object backup = Object1.GetType().GetProperty(this.property).GetValue(Object1);
            Object1.GetType().GetProperty(this.property).SetValue(Object1, Object2.GetType().GetProperty(this.property).GetValue(Object2), null);
            Object2.GetType().GetProperty(this.property).SetValue(Object2, backup, null);
        }

        internal override void Redo()
        {
            object backup = Object1.GetType().GetProperty(this.property).GetValue(Object1);
            Object1.GetType().GetProperty(this.property).SetValue(Object1, Object2.GetType().GetProperty(this.property).GetValue(Object2), null);
            Object2.GetType().GetProperty(this.property).SetValue(Object2, backup, null);
        }
    }
}
