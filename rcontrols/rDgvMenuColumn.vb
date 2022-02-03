Public Class rDgvMenuColumn
    Private mPopup As New ToolStripDropDown()
    Private rDGV As rDataGridView = Nothing

    Private TSMFreeze As New ToolStripButton()
    Private TSMFilterAdd As New ToolStripButton()

    Private TSMFilterDel As New ToolStripButton() ' Remover Filtros da coluna
    Private TSMFilterClear As New ToolStripButton() ' Remover todos filtros da Tabela

    Private colclickidx As Integer
    Private rowclickidx As Integer

    Private filterbd As rDgvFilterBuilder

    Public Event ColFrozenChangebyUser(ByVal colidx As Integer)
    Public Sub New(dgv As rDataGridView)
        rDGV = dgv
        filterbd = New rDgvFilterBuilder(rDGV)

        mPopup.Items.Add(TSMFreeze)
        mPopup.Items.Add(TSMFilterAdd)
        mPopup.Items.Add(TSMFilterDel)
        mPopup.Items.Add(TSMFilterClear)

        TSMFilterDel.Text = "Remove Filter(s)"
        TSMFilterClear.Text = "Clear All Filters"

        AddHandler rDGV.CellMouseDown, AddressOf DGV_CellMouseDown
        AddHandler TSMFreeze.Click, AddressOf TSMFreeze_Click
        AddHandler TSMFilterAdd.Click, AddressOf TSMFilterAdd_Click
        AddHandler TSMFilterDel.Click, Sub() filterbd.Remove(colclickidx)
        AddHandler TSMFilterClear.Click, Sub() filterbd.RemoveAll()

    End Sub

    Private Sub DGV_CellMouseDown(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs)

        If e.Button = MouseButtons.Right AndAlso e.RowIndex = -1 AndAlso e.ColumnIndex > -1 Then
            colclickidx = e.ColumnIndex
            rowclickidx = e.RowIndex
            ' mPopup.AutoClose = False

            Dim colfrozen As Integer = (From cl As DataGridViewColumn In rDGV.Columns).Where(Function(c) c.Frozen).Select(Of Integer)(Function(cl) cl.Index).LastOrDefault(Function(b) -2)
            TSMFreeze.Text = If(colfrozen = colclickidx, "Un", "") & "Freeze Column"
            TSMFilterAdd.Text = "Add Filter"

            TSMFilterDel.Visible = filterbd.containsfilter(colclickidx)
            TSMFilterClear.Visible = filterbd.Count > 0

            mPopup.Show(System.Windows.Forms.Cursor.Position)
        End If
    End Sub

    Private Sub TSMFreeze_Click(sender As Object, e As EventArgs)
        Dim colfrozen As Integer = (From cl As DataGridViewColumn In rDGV.Columns).Where(Function(c) c.Frozen).Select(Of Integer)(Function(cl) cl.Index).LastOrDefault(Function(b) -2)
        Dim idx = If(colfrozen = colclickidx, -2, colclickidx)
        SetFreeze(idx)
        RaiseEvent ColFrozenChangebyUser(idx)
    End Sub
    Private Sub TSMFilterAdd_Click(sender As Object, e As EventArgs)
        Dim mpop As New ToolStripDropDown()
        Dim pn = New pncond(filterbd, colclickidx, (rDGV.DataView.Table.Columns(colclickidx).DataType.Name <> "String"))
        mpop.Items.Add(New ToolStripControlHost(pn))
        mpop.Show(Cursor.Position)
        pn.initfocus()
    End Sub

    Public Sub SetFreeze(colidx As Integer)
        For Each c As DataGridViewColumn In rDGV.Columns
            c.Frozen = (colidx = c.Index)
        Next
    End Sub

    Private Class pncond
        Inherits Panel

        Private lbcond As New Label()
        Private WithEvents cbcond As New ComboBox()

        Private lbvlr As New Label()
        Private WithEvents txvlr As New TextBox()

        Private fbuilder As rDgvFilterBuilder
        Private isnumber As Boolean
        Private colidx As Integer

        Public Sub New(ByRef _fbuilder As rDgvFilterBuilder, _colidx As Integer, Optional _isnumber As Boolean = True)
            fbuilder = _fbuilder
            isnumber = _isnumber
            colidx = _colidx
            Me.Size = New Size(200, 200)

            lbcond.Text = "Cond: "
            lbcond.Size = New Size(40, 20)

            cbcond.Items.AddRange(If(isnumber, {"Igual", "Maior", "Menor"}, {"Inicio", "Contém"}))
            cbcond.Location = New Point(lbcond.Size.Width + 10, lbcond.Location.Y)
            cbcond.SelectedIndex = 0
            cbcond.DropDownStyle = ComboBoxStyle.DropDownList

            lbvlr.Text = "Valor: "
            lbvlr.Size = New Size(40, 20)
            lbvlr.Location = New Point(lbcond.Location.X, lbcond.Size.Height + lbcond.Location.Y + 5)

            txvlr.Location = New Point(lbvlr.Size.Width + 10, lbvlr.Location.Y)
            txvlr.Size = cbcond.Size

            Me.Controls.Add(lbcond)
            Me.Controls.Add(cbcond)
            Me.Controls.Add(lbvlr)
            Me.Controls.Add(txvlr)
        End Sub

        Private Sub cbcond_KeyDown(sender As Object, e As KeyEventArgs) Handles cbcond.KeyDown
            If e.KeyCode = Keys.Return Or e.KeyCode = Keys.Tab Then
                txvlr.Focus()
            ElseIf e.KeyCode = Keys.Escape Then
                CType(Me.Parent, ToolStripDropDown).Close()
            End If
        End Sub
        Private Sub txvlr_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txvlr.KeyPress
            If Asc(e.KeyChar) = 13 Then
                SetFilter()
            ElseIf Asc(e.KeyChar) = 27 Then
                CType(Me.Parent, ToolStripDropDown).Close()
            End If
        End Sub

        Private Sub SetFilter()
            If isnumber And Not IsNumeric(txvlr.Text) Then MsgBox("Insira um numero válido!") : txvlr.Focus() : Exit Sub

            Dim tp As rDgvFilterBuilder.FilterType

            If isnumber Then
                Select Case cbcond.SelectedIndex
                    Case 0 : tp = rDgvFilterBuilder.FilterType.Igual
                    Case 1 : tp = rDgvFilterBuilder.FilterType.Maior
                    Case 2 : tp = rDgvFilterBuilder.FilterType.Menor
                End Select
            Else
                Select Case cbcond.SelectedIndex
                    Case 0 : tp = rDgvFilterBuilder.FilterType.Inicio
                    Case 1 : tp = rDgvFilterBuilder.FilterType.Contem
                End Select
            End If

            Dim vl = If(isnumber, txvlr.Text.Replace(",", "."), txvlr.Text)

            fbuilder.Add(New rDgvFilterBuilder.ItemColFilter() With {.colidx = colidx, .Ftype = tp, .Vlr = vl})
            CType(Me.Parent, ToolStripDropDown).Close()
        End Sub
        Public Sub initfocus()
            cbcond.Focus()
        End Sub


    End Class
End Class
