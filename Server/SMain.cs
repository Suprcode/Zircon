﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using MirDB;
using Server.DBModels;
using Server.Envir;
using Server.Views;

namespace Server
{
    public partial class SMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public List<Control> Windows = new List<Control>();
        public static Session Session;
       
        public SMain()
        {
            InitializeComponent();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
        }

        private void SMain_Load(object sender, EventArgs e)
        {
            ShowView(typeof(SystemLogView));

            Session = new Session(SessionMode.System)
            {
                BackUpDelay = 60
            };


            /*
                        MapInfoList = Session.GetCollection<MapInfo>();
                        MovementInfoList = Session.GetCollection<MovementInfo>();
                        GuardInfoList = Session.GetCollection<GuardInfo>();
                        SafeZoneInfoList = Session.GetCollection<SafeZoneInfo>();
                        RespawnInfoList = Session.GetCollection<RespawnInfo>();
                        ItemInfoList = Session.GetCollection<ItemInfo>();
                        ItemInfoStatList = Session.GetCollection<ItemInfoStat>();

                        MonsterInfoList = Session.GetCollection<MonsterInfo>();
                        NPCInfoList = Session.GetCollection<NPCInfo>();
                        NPCPageList = Session.GetCollection<NPCPage>();
                        MagicInfoList = Session.GetCollection<MagicInfo>();
                        SetInfoList = Session.GetCollection<SetInfo>();
                        StoreInfoList = Session.GetCollection<StoreInfo>();
                        BaseStatList = Session.GetCollection<BaseStat>();
                        MapRegionList = Session.GetCollection<MapRegion>();*/

            UpdateInterface();

            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            try
            {
                while (AppStillIdle)
                {
                    MapViewer.CurrentViewer?.Process();
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (XtraMessageBox.Show(this, "Are you sure you want to close the server?", "Close Server", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            Session.BackUpDelay = 0;
            Session?.Save(true);

            if (SEnvir.EnvirThread == null) return;
            
            SEnvir.Started = false;

            while (SEnvir.EnvirThread != null) Thread.Sleep(1);
        }
        private void ShowView(Type type)
        {
            try
            {
                foreach (Control item in Windows)
                    if (item.GetType() == type)
                    {
                        tabbedView1.ActivateDocument(item);
                        return;
                    }

                Form view = (Form)Activator.CreateInstance(type);
                view.MdiParent = this;
                view.Disposed += View_Disposed;
                view.Tag = type.Name;
                Windows.Add(view);
                view.Show();
            }
            finally
            { }
        }
        private void View_Disposed(object sender, EventArgs e)
        {
            Windows.Remove((Control)sender);
        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            UpdateInterface();

            if (!SEnvir.Started && SEnvir.EnvirThread == null)
                InterfaceTimer.Enabled = false;
        }

        private void UpdateInterface()
        {
            StartServerButton.Enabled = SEnvir.EnvirThread == null;
            StopServerButton.Enabled = SEnvir.Started;

            ConnectionLabel.Caption = string.Format(@"Connections: {0:#,##0}", SEnvir.Connections.Count);
            ObjectLabel.Caption = string.Format(@"Objects: {0} of {1:#,##0}", SEnvir.ActiveObjects.Count, SEnvir.Objects.Count);
            ProcessLabel.Caption = string.Format(@"Process Count: {0:#,##0}", SEnvir.ProcessObjectCount);
            LoopLabel.Caption = string.Format(@"Loop Count: {0:#,##0}", SEnvir.LoopCount);
            EMailsSentLabel.Caption = string.Format(@"E-Mails Sent: {0:#,##0}", SEnvir.EMailsSent);

            ConDelay.Caption = string.Format(@"Con Delay: {0:#,##0}ms", SEnvir.ConDelay);
            SaveDelay.Caption = string.Format(@"Save Delay: {0:#,##0}ms", SEnvir.SaveDelay);

            const decimal KB = 1024;
            const decimal MB = KB * 1024;
            const decimal GB = MB * 1024;

            if (SEnvir.TotalBytesReceived > GB)
                TotalDownloadLabel.Caption = string.Format(@"Downloaded: {0:#,##0.0}GB", SEnvir.TotalBytesReceived / GB);
            else if (SEnvir.TotalBytesReceived > MB)
                TotalDownloadLabel.Caption = string.Format(@"Downloaded: {0:#,##0.0}MB", SEnvir.TotalBytesReceived / MB);
            else if (SEnvir.TotalBytesReceived > KB)
                TotalDownloadLabel.Caption = string.Format(@"Downloaded: {0:#,##0}KB", SEnvir.TotalBytesReceived / KB);
            else
                TotalDownloadLabel.Caption = string.Format(@"Downloaded: {0:#,##0}B", SEnvir.TotalBytesReceived);

            if (SEnvir.TotalBytesSent > GB)
                TotalUploadLabel.Caption = string.Format(@"Uploaded: {0:#,##0.0}GB", SEnvir.TotalBytesSent / GB);
            else if (SEnvir.TotalBytesSent > MB)
                TotalUploadLabel.Caption = string.Format(@"Uploaded: {0:#,##0.0}MB", SEnvir.TotalBytesSent / MB);
            else if (SEnvir.TotalBytesSent > KB)
                TotalUploadLabel.Caption = string.Format(@"Uploaded: {0:#,##0}KB", SEnvir.TotalBytesSent / KB);
            else
                TotalUploadLabel.Caption = string.Format(@"Uploaded: {0:#,##0}B", SEnvir.TotalBytesSent);


            if (SEnvir.DownloadSpeed > GB)
                DownloadSpeedLabel.Caption = string.Format(@"D/L Speed: {0:#,##0.0}GBps", SEnvir.DownloadSpeed / GB);
            else if (SEnvir.DownloadSpeed > MB)
                DownloadSpeedLabel.Caption = string.Format(@"D/L Speed: {0:#,##0.0}MBps", SEnvir.DownloadSpeed / MB);
            else if (SEnvir.DownloadSpeed > KB)
                DownloadSpeedLabel.Caption = string.Format(@"D/L Speed: {0:#,##0}KBps", SEnvir.DownloadSpeed / KB);
            else
                DownloadSpeedLabel.Caption = string.Format(@"D/L Speed: {0:#,##0}Bps", SEnvir.DownloadSpeed);

            if (SEnvir.UploadSpeed > GB)
                UploadSpeedLabel.Caption = string.Format(@"U/L Speed: {0:#,##0.0}GBps", SEnvir.UploadSpeed / GB);
            else if (SEnvir.UploadSpeed > MB)
                UploadSpeedLabel.Caption = string.Format(@"U/L Speed: {0:#,##0.0}MBps", SEnvir.UploadSpeed / MB);
            else if (SEnvir.UploadSpeed > KB)
                UploadSpeedLabel.Caption = string.Format(@"U/L Speed: {0:#,##0}KBps", SEnvir.UploadSpeed / KB);
            else
                UploadSpeedLabel.Caption = string.Format(@"U/L Speed: {0:#,##0}Bps", SEnvir.UploadSpeed);
        }

        private void StartServerButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InterfaceTimer.Enabled = true;
            SEnvir.StartServer();
            UpdateInterface();
        }

        private void StopServerButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SEnvir.Started = false;
            UpdateInterface();
        }


        private void LogNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(SystemLogView));
        }

        private void ConfigButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ConfigView));
        }

        private void MapInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MapInfoView));
        }
        private void MonsterInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MonsterInfoView));
        }

        public static void SetUpView(GridView view)
        {
            view.BestFitColumns();
            view.KeyPress += PasteData_KeyPress;
            view.KeyDown += DeleteRows_KeyDown;
            view.OptionsSelection.MultiSelect = true;
            view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
        private static void DeleteRows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;

            if (MessageBox.Show("Delete rows?", "Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            GridView view = (GridView)sender;
            //view.DeleteSelectedRows();

            int[] rows = view.GetSelectedRows();

            List<DBObject> objects = new List<DBObject>();

            foreach (int index in rows)
                objects.Add((DBObject)view.GetRow(index));

            foreach (DBObject ob in objects)
                ob?.Delete();
        }
        public static void PasteData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x16)
            {
                e.Handled = true;

                GridView view = (GridView)sender;
                string data = Clipboard.GetText();
                string[] copied = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                var rows = view.GetSelectedRows();

                if (rows.Length == 0)
                {
                    //Pasting Column
                    for (int i = 1; i < copied.Length; i++) //Avoid Header
                    {
                        view.AddNewRow();
                        string[] row = copied[i].Split('\t');


                        for (int c = 0; c < row.Length; c++)
                        {
                            if (c >= view.Columns.Count) break;

                            GridColumn column = view.GetVisibleColumn(view.FocusedColumn.VisibleIndex + c);

                            if (column.ColumnType.IsSubclassOf(typeof(DBObject)))
                            {
                                RepositoryItemLookUpEdit tmep = column.ColumnEdit as
                                    RepositoryItemLookUpEdit;

                                if (tmep == null) return;

                                view.SetRowCellValue(view.FocusedRowHandle, column, Session.GetObject(column.ColumnType, tmep.DisplayMember, row[c]));

                            }
                            else if (column.ColumnType == typeof(bool))
                                view.SetRowCellValue(view.FocusedRowHandle, column, row[c] == "Checked");
                            else if (column.ColumnType == typeof(decimal) && row[c].EndsWith("%"))
                                view.SetRowCellValue(view.FocusedRowHandle, column, decimal.Parse(row[c].TrimEnd('%', ' ')) / 100M);
                            else
                                view.SetRowCellValue(view.FocusedRowHandle, column, row[c]);
                        }
                    }
                    return;
                }

                for (int i = 0; i < rows.Length; i++)
                {
                    //Could paste multiple cells;
                    if (i + 1 >= copied.Length) break;
                    string[] row = copied[i + 1].Split('\t');

                    var cells = view.GetSelectedCells(rows[i]);

                    if (cells.Length != row.Length)
                    {
                        XtraMessageBox.Show("Column Count does not Copied Column Count");
                        return;
                    }

                    for (int c = 0; c < cells.Length; c++)
                    {
                        GridColumn column = view.Columns[cells[c].FieldName];

                        if (column.ColumnType.IsSubclassOf(typeof(DBObject)))
                        {
                            RepositoryItemLookUpEdit tmep = column.ColumnEdit as RepositoryItemLookUpEdit;

                            if (tmep == null) return;

                            view.SetRowCellValue(rows[i], column, Session.GetObject(column.ColumnType, tmep.DisplayMember, row[c]));

                        }
                        else if (column.ColumnType == typeof(bool))
                            view.SetRowCellValue(rows[i], column, row[c] == "Checked");
                        else if (column.ColumnType == typeof(decimal) && row[c].EndsWith("%"))
                            view.SetRowCellValue(rows[i], column, decimal.Parse(row[c].TrimEnd('%', ' ')) / 100M);
                        else
                            view.SetRowCellValue(rows[i], column, row[c]);
                    }
                }

            }
        }

        private void ItemInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ItemInfoView));
        }

        private void NPCInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(NPCInfoView));
        }

        private void NPCPageButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(NPCPageView));
        }

        private void MagicInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MagicInfoView));
        }

        private void CharacterInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CharacterView));
        }

        private void AccountInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(AccountView));
        }

        private void MovementInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MovementInfoView));
        }
        

        private void ItemInfoStatButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ItemInfoStatView));
        }

        private void SetInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(SetInfoView));

        }

        private void StoreInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(StoreInfoView));
        }

        private void BaseStatButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(BaseStatView));
        }




        #region Idle Check
        private static bool AppStillIdle
        {
            get
            {
                PeekMsg msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }


        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin,
                                               uint messageFilterMax, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            private readonly IntPtr hWnd;
            private readonly Message msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly Point p;
        }
        #endregion

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void SafeZoneInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(SafeZoneInfoView));
        }

        private void RespawnInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        { 
            ShowView(typeof(RespawnInfoView));
        }

        private void MapRegionButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MapRegionView));
        }

        private void DropInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(DropInfoView));
        }

        private void UserDropButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserDropView));
        }

        private void QuestInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(QuestInfoView));
        }

        private void CompanionInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CompanionInfoView));
        }

        private void EventInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {

            ShowView(typeof(EventInfoView));
        }

        private void MonsterInfoStatButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MonsterInfoStatView));
        }

        private void CastleInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CastleInfoView));
        }

        private void PaymentButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(GameGoldPaymentView));
        }

        private void StoreSalesButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(GameStoreSaleView));
        }

        private void DiagnosticButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(DiagnosticView));
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserItemView));
        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserConquestStatsView));
        }

        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserMailView));
        }

        private void navBarItem5_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(WeaponCraftStatInfoView));
        }
    }

}
