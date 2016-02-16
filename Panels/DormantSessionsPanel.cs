//-----------------------------------------------------------------------
//  This file is part of the Microsoft Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.AnalysisServices.Samples.ActivityViewer
{
    public partial class SnapshotDormantSessionsPanel : UserControl
    {
        ServerManager myServer;
        private string dormantQuery = "select TOP 10 SESSION_SPID, SESSION_USER_NAME,SESSION_IDLE_TIME_MS, SESSION_USED_MEMORY, SESSION_LAST_COMMAND_END_TIME, SESSION_ID from $system.DISCOVER_SESSIONS WHERE SESSION_STATUS = 0 ORDER BY SESSION_IDLE_TIME_MS DESC";

        public SnapshotDormantSessionsPanel(ServerManager server)
        {
            InitializeComponent();
            myServer = server;
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
| System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        }

        private void refreshDormantTable()
        {
            DormantDataGrid.DataSource = myServer.ASMan.DataTableFromQuery(dormantQuery);
            foreach (DataGridViewTextBoxColumn col in DormantDataGrid.Columns)
            {
                col.ToolTipText = ActivityViewer.ProgramManager.GetToolTipText(col.HeaderText);
            }
            DormantDataGrid.AutoResizeColumns();
        }

        private void DormantRefreshTimer_Tick(object sender, EventArgs e)
        {
            refreshDormantTable();
        }

        private void SnapshotDormantSessionsPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (myServer.dataRefreshInterval != 0)
                {
                    DormantRefreshTimer.Interval = myServer.dataRefreshInterval;
                    DormantRefreshTimer.Start();
                }
                refreshDormantTable();
                DormantDataGrid.AutoResizeColumns();
            }
            else
            {
                if (myServer.dataRefreshInterval != 0)
                    DormantRefreshTimer.Stop();
            }
        }

        private void DormantCancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                myServer.ASMan.CancelSession((int)DormantDataGrid.SelectedRows[0].Cells["SESSION_SPID"].Value);
                refreshDormantTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            refreshDormantTable();
        }
    }
}
