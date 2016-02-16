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
    public partial class CurrentQueriesPanel : UserControl
    {
        ServerManager myServer;
        private string queriesQuery = "SELECT SESSION_SPID, COMMAND_CPU_TIME_MS, COMMAND_ELAPSED_TIME_MS, COMMAND_READ_KB, COMMAND_WRITE_KB, COMMAND_TEXT FROM $system.DISCOVER_COMMANDS WHERE COMMAND_ELAPSED_TIME_MS > 0 ORDER BY COMMAND_CPU_TIME_MS DESC";

        public CurrentQueriesPanel(ServerManager server)
        {
            InitializeComponent();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
| System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            myServer = server;
        }

        private void refreshQueriesTable()
        {
            QueriesDataGrid.DataSource = myServer.ASMan.DataTableFromQuery(queriesQuery);
            foreach (DataGridViewTextBoxColumn col in QueriesDataGrid.Columns)
            {
                col.ToolTipText = ActivityViewer.ProgramManager.GetToolTipText(col.HeaderText);
            }
            QueriesDataGrid.AutoResizeColumns();
        }

        private void QueriesRefreshTimer_Tick(object sender, EventArgs e)
        {
            refreshQueriesTable();
        }

        private void CurrentQueriesPanel_VisibleChanged(object sender, EventArgs e)
        {

            if (this.Visible)
            {
                if (myServer.dataRefreshInterval != 0)
                {
                    QueriesRefreshTimer.Interval = myServer.dataRefreshInterval;
                    QueriesRefreshTimer.Start();
                }
                refreshQueriesTable();
                QueriesDataGrid.AutoResizeColumns();
            }
            else
            {
                if (myServer.dataRefreshInterval != 0)
                    QueriesRefreshTimer.Stop();
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            refreshQueriesTable();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            myServer.ASMan.CancelSession((int)QueriesDataGrid.CurrentRow.Cells["SESSION_SPID"].Value);
        }
    }
}
