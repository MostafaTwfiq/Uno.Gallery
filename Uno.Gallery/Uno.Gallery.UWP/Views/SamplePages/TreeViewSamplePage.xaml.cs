using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Uno.Gallery.Views.Samples
{
    [SamplePage(SampleCategory.Components, "TreeView", Description = Description, DocumentationLink = "https://learn.microsoft.com/en-us/windows/apps/design/controls/tree-view")]
    public sealed partial class TreeViewSamplePage : Page
    {
        private const string Description = "kk";

        public TreeViewSamplePage()
        {
            this.InitializeComponent();
        }
        
    }
}