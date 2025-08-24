using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using VideoGui.Models.delegates;
using VideoGui.Models;
using System.Collections.Generic;
using System.Linq;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for TitleSelectFrm.xaml
    /// </summary>
    public partial class TitleSelectFrm : Window
    {
        public bool IsShorts = false, IsClosed = false, IsClosing = false, IsTitleChanged = false;

        public delegate void ShowEditor();
        public ShowEditor _ShowEditor;
        OnFinish OnFinished;
        databasehook<object> Invoker;
        public int TitleId = -1, LinkedId = -1;
        string Title = "";
        string TagTitle = "";
        public string BaseTitle = "";
        CustomStringEntry CPS = null;

        bool IsUploadsBuilder = false;
        public TitleSelectFrm(OnFinishIdObj OnFinished, databasehook<Object> _Invoker,
            bool _IsUploadsBuilder = false, int _TitleId = -1, int _LinkedId = -1)
        {
            try
            {
                Invoker = _Invoker;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; IsTitleChanged = false; OnFinished?.Invoke(this, -1); };
                IsUploadsBuilder = _IsUploadsBuilder;
                InitializeComponent();
                TitleId = _TitleId;
                LinkedId = _LinkedId;
                Invoker?.Invoke(this, new CustomParams_SetFilterId(_LinkedId, _TitleId));
                Invoker?.Invoke(this, new CustomParams_Initialize(IsUploadsBuilder));  
                /*txtTitle.Text = _Title; Handle this in onload?.Invoke.
                BaseTitle = _Title;
                txtBaseTitle.Content = $"({_Title})";*/
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public int GetTitleTag()
        {
            try
            {
                return TitleId;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetTitleTag {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return -1;
            }
        }
        public void SetTitleTag(int tag)
        {
            try
            {
                TitleId = tag;
                //btnAddTag.IsEnabled = (tag != -1);
                // btnRemTag.IsEnabled = (tag != -1);
                // ImgRemove.IsEnabled = (tag != -1);
                ImgAdd.IsEnabled = (tag != -1);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetTitleTag {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnClose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnRemoveTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTitle.Text.Trim() == "") return;
                foreach (var st in TagsGrp.SelectedItems.OfType<TitleTags>().ToList())
                {
                    var p = new CustomParams_Remove(st.Id);
                    int TextLength = txtTitle.Text.Length + p.TitleLength;
                    lblTitleLength.Content = TextLength.ToString();

                    Invoker?.Invoke(this, p);
                    Invoker?.Invoke(this, new CustomParams_Refresh());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"frmTitleEditor_Closing {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void TagAvailable_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    e.Handled = true; // Prevent selection change
                    if (TagsGrp.ContextMenu != null)
                    {
                        TagAvailable.ContextMenu.PlacementTarget = TagAvailable;
                        TagAvailable.ContextMenu.IsOpen = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TagAvailable_PreviewMouseRightButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void TagsGrp_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    e.Handled = true; // Prevent selection change
                    if (TagsGrp.ContextMenu != null)
                    {
                        TagsGrp.ContextMenu.PlacementTarget = TagsGrp;
                        TagsGrp.ContextMenu.IsOpen = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TagsGrp_PreviewMouseRightButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void frmTitleEditor_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Invoker?.Invoke(this, new CustomParams_Initialize(IsUploadsBuilder));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"frmTitleEditor_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void frmTitleEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                OnFinished?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"frmTitleEditor_Closing {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnInsertTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTitle.Text.Trim() == "") return;
                foreach (var item in TagAvailable.SelectedItems.OfType<AvailableTags>().ToList())
                {
                    var p = new CustomParams_InsertWithId(item.Id, TitleId);
                    Invoker?.Invoke(this, p);
                    Invoker?.Invoke(this, new CustomParams_Refresh());
                    int TextLength = txtTitle.Text.Length;
                    if (TextLength < 101)
                    {
                        lblTitleLength.Content = TextLength.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Title And Tags Exceeds 100 Characters");
                    }
                    TagsGrp.Items.Refresh();
                    TagAvailable.Items.Refresh();
                    Invoker?.Invoke(this, new CustomParams_Refresh());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnRemTag_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTitle.Text.Trim() != "")
                {
                    Invoker?.Invoke(this, new CustomParams_Get(TitleId, BaseTitle));
                    //DoOnTagGroupInsert?.Invoke(txtTitle.Text, this);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelect_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void txtTitle_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    var p = new CustomParams_Remove(-1);
                    Invoker?.Invoke(this, p);
                    Invoker?.Invoke(this, new CustomParams_Refresh());
                    TagsGrp.Items.Refresh();
                    int TextLength = txtTitle.Text.Length + p.TitleLength;
                    lblTitleLength.Content = TextLength.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"txtTitle_KeyUp {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    var p = new CustomParams_Add("AVAILTAGADD", txtNewTag.Text);
                    Invoker?.Invoke(this, p);
                    Thread.Sleep(100);
                    txtNewTag.Text = "";
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"btnAddNew_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ImgSelect_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void lblTitleName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CPS is not null)
                {
                    CPS.Close();
                    CPS = new("Select Base Title", BaseTitle, null, OnFinishBaseTitle);
                    CPS.ShowActivated = true;
                    CPS.Show();
                }
                else 
                {
                    CPS = new("Select Base Title", BaseTitle, null, OnFinishBaseTitle);
                    CPS.ShowActivated = true;
                    CPS.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"lblTitleName_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void OnFinishBaseTitle(object sender, int i)
        {
            try
            {
                if (sender is CustomStringEntry frm)
                {
                    if (frm.txtData.Text != BaseTitle)
                    {
                        IsTitleChanged = true;
                        txtBaseTitle.Content = frm.txtData.Text;
                        BaseTitle = frm.txtData.Text;
                        Invoker?.Invoke(this, new CustomParams_EditName(TitleId, frm.txtData.Text));
                    }

                    frm = null;
                    // check cps != frm
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"OnFinishBaseTitle {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuUseTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstTitles.SelectedItem is GroupTitleTags GTT)
                {
                    Invoker?.Invoke(this, new CustomParams_InsertTags(
                        GTT.Ids.Split(',').ToArray<string>().Where(s =>
                        s.ToInt(-1) != -1).Select(s => s.ToInt()).ToList<int>(), TitleId));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuNewItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void TagAvailable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<AvailableTags> Tags = [.. TagAvailable.SelectedItems.Cast<AvailableTags>()];
                if (txtTitle.Text.Trim() == "") return;
                foreach (var item in Tags)
                {
                    var p = new CustomParams_InsertWithId(item.Id, TitleId);
                    Invoker?.Invoke(this, p);
                    Invoker?.Invoke(this, new CustomParams_Refresh());
                    int TextLength = txtTitle.Text.Length;
                    if (TextLength < 101)
                    {
                        lblTitleLength.Content = TextLength.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Title And Tags Exceeds 100 Characters");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TagAvailable_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void lstTitles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtTitle.Text.Trim() == "") return;
                if (lstTitles.SelectedItem is GroupTitleTags GTT)
                {
                    Invoker?.Invoke(this, new CustomParams_InsertTags(
                        GTT.Ids.Split(',').ToArray<string>().Where(s =>
                        s.ToInt(-1) != -1).Select(s => s.ToInt()).ToList<int>(), TitleId));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"lstTitles_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void mnuUseAddTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnInsertTags_Click(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuUseAddTags_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void TagsGrp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtTitle.Text.Trim() == "") return;
                List<TitleTags> Tags = new List<TitleTags>();
                Tags.AddRange(TagsGrp.SelectedItems.Cast<TitleTags>());
                foreach (var item in Tags)
                {
                    var p = new CustomParams_Remove(item.Id);
                    int TextLength = txtTitle.Text.Length + p.TitleLength;
                    lblTitleLength.Content = TextLength.ToString();
                    Invoker?.Invoke(this, p);
                    Invoker?.Invoke(this, new CustomParams_Refresh());
                    TagsGrp.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TagsGrp_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");

            }
        }

        private void txtBaseTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                CPS = new("Select Base Title", BaseTitle, null, OnFinishBaseTitle);
                CPS.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"txtBaseTitle_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }



        private void mnuNewItem_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
