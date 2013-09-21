using Wide.Utils;

namespace Hypertest.Core.Manager
{
    public class PropertyChange : Change
    {
        public PropertyChange(object target, PropertyChangedExtendedEventArgs args)
        {
            this.args = args;
            this.target = target;
        }

        public object target { get; internal set; }

        public PropertyChangedExtendedEventArgs args { get; internal set; }

        internal override void Undo()
        {
            PropertyUndo();
        }

        internal override void Redo()
        {
            PropertyRedo();
        }

        internal void PropertyUndo()
        {
            PropertyChangedExtendedEventArgs e = args as PropertyChangedExtendedEventArgs;
            target.GetType().GetProperty(e.PropertyName).SetValue(target, e.OldValue, null);
        }
        internal void PropertyRedo()
        {
            PropertyChangedExtendedEventArgs e = args as PropertyChangedExtendedEventArgs;
            target.GetType().GetProperty(e.PropertyName).SetValue(target, e.NewValue, null);
        }
    }
}
