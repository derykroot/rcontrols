Public Class rDataGridViewColumnSelector
    ' the DataGridView to which the DataGridViewColumnSelector is attached
    Private mDataGridView As DataGridView = Nothing
    ' a CheckedListBox containing the column header text and checkboxes
    Private mCheckedListBox As CheckedListBox
    ' a ToolStripDropDown object used to show the popup
    Private mPopup As ToolStripDropDown
    Private mPopupShowing As Boolean = False

    ''' <summary>
    ''' The max height of the popup
    ''' </summary>
    Public MaxHeight As Integer = 300
    ''' <summary>
    ''' The width of the popup
    ''' </summary>
    Public Width As Integer = 200

    Public Structure ColVisible
        Public idx As Integer
        Public visible As Boolean
    End Structure

    Public Event ColVisibleChangebyUser(ByVal e As List(Of ColVisible))

    ''' <summary>
    ''' Gets or sets the DataGridView to which the DataGridViewColumnSelector is attached
    ''' </summary>
    Public Property DataGridView() As DataGridView
        Get
            Return mDataGridView
        End Get
        Set(ByVal value As DataGridView)
            ' If any, remove handler from current DataGridView 
            If mDataGridView IsNot Nothing Then
                'RemoveHandler mDataGridView.CellMouseClick, New DataGridViewCellMouseEventHandler(AddressOf mDataGridView_CellMouseClick)
                RemoveHandler mDataGridView.CellMouseDown, New DataGridViewCellMouseEventHandler(AddressOf mDataGridView_CellMouseDown)
                RemoveHandler mDataGridView.CellPainting, AddressOf mDGV_CellPainting
                If mDataGridView.ContextMenuStrip IsNot Nothing Then
                    RemoveHandler mDataGridView.ContextMenuStrip.Opening, AddressOf Ctx_opng
                End If
            End If

            ' Set the new DataGridView
            mDataGridView = value
            ' Attach CellMouseClick handler to DataGridView

            If mDataGridView IsNot Nothing Then
                'AddHandler mDataGridView.CellMouseClick, New DataGridViewCellMouseEventHandler(AddressOf mDataGridView_CellMouseClick)
                AddHandler mDataGridView.CellMouseDown, New DataGridViewCellMouseEventHandler(AddressOf mDataGridView_CellMouseDown)
                AddHandler mDataGridView.CellPainting, AddressOf mDGV_CellPainting
                If mDataGridView.ContextMenuStrip IsNot Nothing Then
                    AddHandler mDataGridView.ContextMenuStrip.Opening, AddressOf Ctx_opng
                End If
            End If
        End Set
    End Property

    Public Sub Loadpresets(cols As List(Of ColVisible))
        If cols Is Nothing Then Exit Sub
        cols.ForEach(Sub(c)
                         ' mCheckedListBox.SetItemChecked(c.idx, c.visible)
                         If mDataGridView.Columns.Count > c.idx Then mDataGridView.Columns(c.idx).Visible = c.visible
                     End Sub)
    End Sub

    ' When user right-clicks the cell origin, it clears and fill the CheckedListBox with
    ' columns header text. Then it shows the popup. 
    ' In this way the CheckedListBox items are always refreshed to reflect changes occurred in 
    ' DataGridView columns (column additions or name changes and so on).

#Region "Eventos Adicionados"
    Private Sub mDataGridView_CellMouseDown(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs)

        If e.Button = MouseButtons.Right AndAlso e.RowIndex = -1 AndAlso e.ColumnIndex = -1 Then
            mCheckedListBox.Items.Clear()
            For Each c As DataGridViewColumn In mDataGridView.Columns
                mCheckedListBox.Items.Add(c.HeaderText, c.Visible)
            Next
            Dim PreferredHeight As Integer = (mCheckedListBox.Items.Count * 16) + 20
            mCheckedListBox.Height = If((PreferredHeight < MaxHeight), PreferredHeight, MaxHeight)
            mCheckedListBox.Width = Me.Width
            ' mPopup.AutoClose = False
            mPopup.Show(mDataGridView.PointToScreen(New Point(e.X, e.Y)))
        End If
    End Sub

    Private Sub mDGV_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs)
        Dim c As Color = sender.ColumnHeadersDefaultCellStyle.BackColor
        'e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(CInt(c.R * 0.9), CInt(c.G * 0.9), CInt(c.B * 0.9))), 1, 1, sender.RowHeadersWidth - 2, sender.ColumnHeadersHeight - 2)
        'e.Graphics.DrawString("Click Left", New Font(FontFamily.GenericSansSerif, 6, FontStyle.Regular), Brushes.White, New Rectangle(1, 1, sender.RowHeadersWidth - 1, sender.ColumnHeadersHeight - 1))
        If e.ColumnIndex = -1 And e.RowIndex = -1 Then
            e.CellStyle.BackColor = c 'Color.FromArgb(CInt(c.R * 0.9), CInt(c.G * 0.9), CInt(c.B * 0.9))
            e.Graphics.DrawString("Click Right", New Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular), Brushes.White, New Rectangle(0, 1, sender.RowHeadersWidth + 1, sender.ColumnHeadersHeight - 1))
            e.Handled = True
        End If
    End Sub

    Private Sub Ctx_opng(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        'sender.AutoClose = True
        If mPopupShowing Then e.Cancel = True
        ' MsgBox(mPopup.IsHandleCreated)
    End Sub

    Private Sub mPopup_op(ByVal sender As Object, ByVal e As System.EventArgs)
        mPopupShowing = True
    End Sub
    Private Sub mPopup_closed(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosedEventArgs)
        mPopupShowing = False
    End Sub

    Private Sub BtSel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        For i = 0 To mCheckedListBox.Items.Count - 1
            mCheckedListBox.SetItemChecked(i, True)
        Next
        changecolsbyuser()
    End Sub
    Private Sub BtDSel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        For i = 0 To mCheckedListBox.Items.Count - 1
            mCheckedListBox.SetItemChecked(i, False)
        Next
        changecolsbyuser()
    End Sub
    Private Sub mCheckedListBox_ItemCheck(ByVal sender As Object, ByVal e As ItemCheckEventArgs)
        mDataGridView.Columns(e.Index).Visible = (e.NewValue = CheckState.Checked)
    End Sub
    Private Sub changecolsbyuser()
        Dim lsc As New List(Of ColVisible)
        For i = 0 To mCheckedListBox.Items.Count - 1
            lsc.Add(New ColVisible() With {.idx = i, .visible = (mCheckedListBox.GetItemCheckState(i) = CheckState.Checked)})
        Next
        RaiseEvent ColVisibleChangebyUser(lsc)
    End Sub

#End Region

    Public Sub New()
        mCheckedListBox = New CheckedListBox()
        mCheckedListBox.CheckOnClick = True
        mCheckedListBox.BorderStyle = BorderStyle.None
        mCheckedListBox.BackColor = Color.FromArgb(228, 234, 237)
        mCheckedListBox.ForeColor = Color.FromArgb(60, 80, 90)
        AddHandler mCheckedListBox.ItemCheck, New ItemCheckEventHandler(AddressOf mCheckedListBox_ItemCheck)
        AddHandler mCheckedListBox.MouseUp, New MouseEventHandler(Sub(sender As Object, e As MouseEventArgs)
                                                                      If e.Button = MouseButtons.Left Then changecolsbyuser()
                                                                  End Sub)
        AddHandler mCheckedListBox.KeyUp, New KeyEventHandler(Sub(sender As Object, e As KeyEventArgs)
                                                                  If e.KeyCode = Keys.Space Then changecolsbyuser()
                                                              End Sub)

        Dim mControlHost As New ToolStripControlHost(mCheckedListBox)
        mControlHost.Padding = Padding.Empty
        mControlHost.Margin = Padding.Empty
        mControlHost.AutoSize = False

        mPopup = New ToolStripDropDown()
        mPopup.Padding = New Padding(1)  ' mPopup.Padding = Padding.Empty
        mPopup.BackColor = mCheckedListBox.BackColor
        mPopup.Opacity = 0.9
        'mPopup.Renderer = New MenuRenderer()

        '===================== Adicionar itens ======================
        Dim btSelAll As New Button
        ' btSelAll.Size = New Size(Width / 2, 20) : btSelAll.VSSelectable = False : btSelAll.VSRound = 0 : btSelAll.Text = "Select All"
        btSelAll.Size = New Size(Width / 2, 22) : btSelAll.Text = "Select All"

        Dim btDSelAll As New Button
        btDSelAll.Location = New Point(btSelAll.Width, 0)
        ' btDSelAll.Size = New Size(Width / 2, 20) : btDSelAll.VSSelectable = False : btDSelAll.VSRound = 0 : btDSelAll.Text = "Deselect All"
        btDSelAll.Size = New Size(Width / 2, 22) : btDSelAll.Text = "Deselect All"

        ' InputSBTStyle(btSelAll) : InputSBTStyle(btDSelAll)
        SetBTStyle(btSelAll) : SetBTStyle(btDSelAll)
        AddHandler btSelAll.Click, AddressOf BtSel_Click
        AddHandler btDSelAll.Click, AddressOf BtDSel_Click

        Dim pn As New Panel 'ToolStripContentPanel 'Panel 'painel onde vai os botões
        pn.BackColor = mCheckedListBox.BackColor
        pn.Controls.Add(btSelAll)
        pn.Controls.Add(btDSelAll)

        Dim PnControlHost As New ToolStripControlHost(pn) ' cria um toolstrip que hospeda o controle
        PnControlHost.Padding = Padding.Empty
        PnControlHost.Margin = Padding.Empty
        mPopup.Items.Add(PnControlHost) 'adiciona o toolstripitem
        mPopup.Items.Add(mControlHost)

        AddHandler mPopup.Opened, AddressOf mPopup_op
        AddHandler mPopup.Closed, AddressOf mPopup_closed
    End Sub

    Public Sub New(ByVal dgv As DataGridView)
        Me.New()
        Me.DataGridView = dgv
    End Sub

    ' When user checks / unchecks a checkbox, the related column visibility is 
    ' switched.
    ' Private Sub InputSBTStyle(ByRef bt As VS.SButton)
    Private Sub SetBTStyle(ByRef bt As Button)
        bt.FlatStyle = FlatStyle.Flat
    End Sub
End Class