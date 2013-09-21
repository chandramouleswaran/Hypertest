using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Hypertest.Core.Tests;
using Wide.Utils;

namespace Hypertest.Core.Manager
{
    public abstract class Change
    {
        internal abstract void Undo();
        internal abstract void Redo();
    }
}
