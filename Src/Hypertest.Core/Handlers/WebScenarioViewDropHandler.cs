using System;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Handlers
{
    public class WebScenarioViewDropHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            TreeViewItem item = dropInfo.VisualTargetItem as TreeViewItem;
            if (item != null)
            {
                FolderTestCase ftc = item.DataContext as FolderTestCase;
                if (ftc != null)
                {
                    ftc.IsExpanded = true;
                    base.DragOver(dropInfo);
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.None;
                }
            }
        }
    }
}
