﻿#pragma checksum "..\..\..\..\SelectReleaseSchedule.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CEE1DF607A5E08DE59C36337D1C290C0699B606C"
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
    /// SelectReleaseSchedule
    /// </summary>
    public partial class SelectReleaseSchedule : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\..\SelectReleaseSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border brdschules;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\SelectReleaseSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstSchItems;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\SelectReleaseSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstMainSchedules;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\SelectReleaseSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblScheduleName;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\..\SelectReleaseSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtScheduleName;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\..\SelectReleaseSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClose;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\..\SelectReleaseSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSave;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.5.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VideoGui;V1.0.0.308;component/selectreleaseschedule.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\SelectReleaseSchedule.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 6 "..\..\..\..\SelectReleaseSchedule.xaml"
            ((VideoGui.SelectReleaseSchedule)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\..\SelectReleaseSchedule.xaml"
            ((VideoGui.SelectReleaseSchedule)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.brdschules = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.lstSchItems = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.lstMainSchedules = ((System.Windows.Controls.ListBox)(target));
            
            #line 42 "..\..\..\..\SelectReleaseSchedule.xaml"
            this.lstMainSchedules.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.lstMainSchedules_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 48 "..\..\..\..\SelectReleaseSchedule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.mnuSave_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 49 "..\..\..\..\SelectReleaseSchedule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.mnuEdit_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 51 "..\..\..\..\SelectReleaseSchedule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.mnuNew_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 53 "..\..\..\..\SelectReleaseSchedule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.mnuDelete_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.lblScheduleName = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.txtScheduleName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\..\..\SelectReleaseSchedule.xaml"
            this.btnClose.Click += new System.Windows.RoutedEventHandler(this.btnClose_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.btnSave = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\..\..\SelectReleaseSchedule.xaml"
            this.btnSave.Click += new System.Windows.RoutedEventHandler(this.btnSave_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

