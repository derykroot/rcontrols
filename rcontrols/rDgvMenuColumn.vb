Public Class rDgvMenuColumn
    Private mPopup As New ToolStripDropDown()
    Private rDGV As DataGridView = Nothing

    Private TSMFreeze As New ToolStripButton()

    Private colclickidx As Integer
    Private rowclickidx As Integer

    Public Event ColFrozenChangebyUser(ByVal colidx As Integer)
    Public Sub New(dgv As DataGridView)
        rDGV = dgv

        mPopup.Items.Add(TSMFreeze)

        AddHandler rDGV.CellMouseDown, AddressOf DGV_CellMouseDown
        AddHandler TSMFreeze.Click, AddressOf TSMFreeze_Click

    End Sub

    Private Sub DGV_CellMouseDown(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs)

        If e.Button = MouseButtons.Right AndAlso e.RowIndex = -1 AndAlso e.ColumnIndex > -1 Then
            colclickidx = e.ColumnIndex
            rowclickidx = e.RowIndex
            ' mPopup.AutoClose = False

            Dim colfrozen As Integer = (From cl As DataGridViewColumn In rDGV.Columns).Where(Function(c) c.Frozen).Select(Of Integer)(Function(cl) cl.Index).LastOrDefault(Function(b) -2)
            TSMFreeze.Text = If(colfrozen = colclickidx, "Un", "") & "Freeze Column"
            mPopup.Show(System.Windows.Forms.Cursor.Position)
        End If
    End Sub

    Private Sub TSMFreeze_Click(sender As Object, e As EventArgs)
        Dim colfrozen As Integer = (From cl As DataGridViewColumn In rDGV.Columns).Where(Function(c) c.Frozen).Select(Of Integer)(Function(cl) cl.Index).LastOrDefault(Function(b) -2)
        Dim idx = If(colfrozen = colclickidx, -2, colclickidx)
        SetFreeze(idx)
        RaiseEvent ColFrozenChangebyUser(idx)
    End Sub

    Public Sub SetFreeze(colidx As Integer)
        For Each c As DataGridViewColumn In rDGV.Columns
            c.Frozen = (colidx = c.Index)
        Next
    End Sub
End Class
