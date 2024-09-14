﻿#pragma checksum "..\..\..\..\ShortsCreator.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3A1FCCC36F858E70D0EC936F148447A28BFC11C2"
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
    /// ShortsCreator
    /// </summary>
    public partial class ShortsCreator : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal VideoGui.ShortsCreator frmShortsCreator;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border brdFileInfo;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas CnvMedialElements;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtsrcdir;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSelectSourceDir;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblDuration;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblShortNo;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblProgress;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar pg1;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\ShortsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClose;
        
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
            System.Uri resourceLocater = new System.Uri("/VideoGui;V1.0.0.309;component/shortscreator.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\ShortsCreator.xaml"
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
            this.frmShortsCreator = ((VideoGui.ShortsCreator)(target));
            
            #line 6 "..\..\..\..\ShortsCreator.xaml"
            this.frmShortsCreator.Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\..\ShortsCreator.xaml"
            this.frmShortsCreator.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.brdFileInfo = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.CnvMedialElements = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.txtsrcdir = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.btnSelectSourceDir = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\..\..\ShortsCreator.xaml"
            this.btnSelectSourceDir.Click += new System.Windows.RoutedEventHandler(this.btnSelectSourceDir_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.lblDuration = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.lblShortNo = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.lblProgress = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.pg1 = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 10:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\..\ShortsCreator.xaml"
            this.btnClose.Click += new System.Windows.RoutedEventHandler(this.btnClose_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

