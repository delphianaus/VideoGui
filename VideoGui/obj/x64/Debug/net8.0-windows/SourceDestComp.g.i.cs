﻿#pragma checksum "..\..\..\..\SourceDestComp.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "46B0ECC090CA5FEADA6A5F58AB86FE4BA11DC570"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using VideoGui;


namespace VideoGui {
    
    
    /// <summary>
    /// SourceDestComp
    /// </summary>
    public partial class SourceDestComp : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Viewbox ViewBix2;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainGrid;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox LstBoxFiles;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnscan;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btncompfiles;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnPurge;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRemoveBadDest;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCurrent;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblMax;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\..\SourceDestComp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnRemoveSource;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VideoGui;V1.0.0.309;component/sourcedestcomp.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\SourceDestComp.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\..\..\SourceDestComp.xaml"
            ((VideoGui.SourceDestComp)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ViewBix2 = ((System.Windows.Controls.Viewbox)(target));
            return;
            case 3:
            this.MainGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.LstBoxFiles = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            this.btnscan = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\..\SourceDestComp.xaml"
            this.btnscan.Click += new System.Windows.RoutedEventHandler(this.Btnscan_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Btncompfiles = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.BtnPurge = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\..\..\SourceDestComp.xaml"
            this.BtnPurge.Click += new System.Windows.RoutedEventHandler(this.BtnPurge_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnRemoveBadDest = ((System.Windows.Controls.Button)(target));
            
            #line 69 "..\..\..\..\SourceDestComp.xaml"
            this.btnRemoveBadDest.Click += new System.Windows.RoutedEventHandler(this.btnRemoveBadDest_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.lblCurrent = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.lblMax = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            this.BtnRemoveSource = ((System.Windows.Controls.Button)(target));
            
            #line 73 "..\..\..\..\SourceDestComp.xaml"
            this.BtnRemoveSource.Click += new System.Windows.RoutedEventHandler(this.BtnRemoveSource_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

