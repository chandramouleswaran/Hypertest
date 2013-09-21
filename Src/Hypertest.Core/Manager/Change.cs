namespace Hypertest.Core.Manager
{
    public abstract class Change
    {
        internal abstract void Undo();
        internal abstract void Redo();
    }
}
