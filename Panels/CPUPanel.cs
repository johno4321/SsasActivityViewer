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
using System.Globalization;

namespace Microsoft.AnalysisServices.Samples.ActivityViewer
{
    public partial class SnapshotCPUPanel : UserControl
    {
        ServerManager myServer;
        private string CPUQuery = "select SESSION_SPID, SESSION_CPU_TIME_MS, SESSION_LAST_COMMAND_CPU_TIME_MS, SESSION_ELAPSED_TIME_MS, SESSION_ID from $System.discover_sessions order by SESSION_SPID";
      
        public SnapshotCPUPanel(ServerManager server)
        {
            
            InitializeComponent();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));

            myServer = server;
            refreshCPUTable();
        }

        private void refreshCPUTable()
        {
            CPUDataGrid.DataSource = myServer.ASMan.DataTableFromQuery(CPUQuery);
            foreach (DataGridViewTextBoxColumn col in CPUDataGrid.Columns)
            {
                col.ToolTipText = ActivityViewer.ProgramManager.GetToolTipText(col.HeaderText);

            }
            CPUDataGrid.AutoResizeColumns();
        }

        private void RefreshCPUTimer_Tick(object sender, EventArgs e)
        {
            refreshCPUTable();
        }

        private void SnapshotCPUPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (myServer.dataRefreshInterval != 0)
                {
                    RefreshCPUTimer.Interval = myServer.dataRefreshInterval;
                    RefreshCPUTimer.Start();
                }
                CPUDataGrid.AutoResizeColumns();
            }
            else
            {
                if (myServer.dataRefreshInterval != 0)
                    RefreshCPUTimer.Stop();
            }
        }

        private void CancelCPUButton_Click(object sender, EventArgs e)
        {
            try
            {
                myServer.ASMan.CancelSession((int)CPUDataGrid.SelectedRows[0].Cells["SESSION_SPID"].Value);
                refreshCPUTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            refreshCPUTable();
        }   
    }
}
