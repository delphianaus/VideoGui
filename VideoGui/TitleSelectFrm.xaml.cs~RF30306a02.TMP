using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using VideoGui.Models.delegates;
using VideoGui.Models;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for TitleSelectFrm.xaml
    /// </summary>
    public partial class TitleSelectFrm : Window
    {
        public bool IsShorts = false;
        public bool IsClosed = false, IsClosing = false, IsTitleChanged = false;

        public delegate void ShowEditor();
        public ShowEditor _ShowEditor;
        OnFinish OnFinished;
        databasehook<object> dbhookup;
        public int TitleId = -1;
        string Title = "";
        string TagTitle = "";
        public string BaseTitle = "";

        bool IsUploadsBuilder = false;
        public TitleSelectFrm(OnFinish __ShowEditor, databasehook<Object> dbhook,
            SetLists _SetLists,bool _IsUploadsBuilder = false)
        {
            try
            {
                //TagTitle = _Title;
                //IsShorts = IsShortTitle;
                OnFinished = __ShowEditor;
                dbhookup = dbhook;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; IsTitleChanged = false; __ShowEditor?.Invoke(); };
                IsUploadsBuilder = _IsUploadsBuilder;
                InitializeComponent();
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
        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((txtTitle.Text != "") && (TagsGrp.SelectedItem is TitleTags TGX))
                {
                    int Id = TGX.Id;
                    var p = new CustomParams_Remove(Id);
                    IsTitleChanged = true;
                    int TextLength = txtTitle.Text.Length + p.TitleLength;
                    lblTitleLength.Content = TextLength.ToString();
                    dbhookup?.Invoke(this, p);
                    dbhookup?.Invoke(this, new CustomParams_Refresh());

                    TagsGrp.Items.Refresh();
                    if (true)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"frmTitleEditor_Closing {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void frmTitleEditor_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                dbhookup?.Invoke(this, new CustomParams_Initialize(IsUploadsBuilder));
                Thread.Sleep(100);
                if (TagsGrp.Items.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"frmTitleEditor_Closing {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
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

        private void btnRemTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((txtTitle.Text != "") && (TagAvailable.SelectedItem is AvailableTags TGX))
                {
                    int Id = TGX.Id;
                    IsTitleChanged = true;
                    var p = new CustomParams_InsertWithId(Id, TitleId);
                    dbhookup?.Invoke(this, p);
                    dbhookup?.Invoke(this, new CustomParams_Refresh());
                    //Thread.Sleep(500);
                    //dbhookup?.Invoke(this, new CustomParams_Refresh());

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
                ex.LogWrite($"btnRemTag_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTitle.Text.Trim() != "")
                {
                    dbhookup?.Invoke(this, new CustomParams_Get(TitleId, BaseTitle));
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
                    dbhookup?.Invoke(this, p);
                    dbhookup?.Invoke(this, new CustomParams_Refresh());
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
                    dbhookup?.Invoke(this, p);
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

        private void mnuDeleteItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mnuEditItem_Click(object sender, RoutedEventArgs e)
        {

        }
        CustomStringEntry CPS = null;
        private void lblTitleName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CPS is not null)
                {
                    if (CPS.IsClosed)
                    {
                        CPS = new("Select Base Title", BaseTitle, null, OnFinishBaseTitle);
                        CPS.Show();
                    }

                }
                if (CPS is null)
                {
                    CPS = new("Select Base Title", BaseTitle, null, OnFinishBaseTitle);
                    CPS.Show();
                }


            }
            catch (Exception ex)
            {
                ex.LogWrite($"lblTitleName_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void OnFinishBaseTitle()
        {
            try
            {
                if (CPS is not null)
                {
                    if (CPS.txtData.Text != BaseTitle)
                    {
                        IsTitleChanged = true;
                        txtBaseTitle.Content = CPS.txtData.Text;
                        BaseTitle = CPS.txtData.Text;

                        dbhookup?.Invoke(this, new CustomParams_EditName(TitleId, CPS.txtData.Text));
                    }
                    if (CPS.IsClosed)
                    {
                        CPS = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"OnFinishBaseTitle {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuUseTags_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtBaseTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CPS is not null)
                {
                    if (CPS.IsClosed)
                    {
                        CPS = new("Select Base Title", BaseTitle, null, OnFinishBaseTitle);
                        CPS.Show();
                    }

                }
                if (CPS is null)
                {
                    CPS = new("Select Base Title", BaseTitle, null, OnFinishBaseTitle);
                    CPS.Show();
                }
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
