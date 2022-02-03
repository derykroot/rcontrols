Public Class rDgvFilterBuilder
    Public Enum FilterType
        Igual = 0
        Maior = 1
        Menor = 2
        Inicio = 3
        Contem = 4
    End Enum
    Public Class ItemColFilter
        Public Property colidx As Integer
        Public Property Ftype As FilterType
        Public Property Vlr As String
    End Class

    Private rDGV As rDataGridView = Nothing
    Private Itens As New List(Of ItemColFilter)

    Private chfilter As Char = "⧨"

    Public Sub New(ByRef _dgv As rDataGridView)
        rDGV = _dgv
    End Sub


    Public Sub Add(itf As ItemColFilter)
        Itens.Add(itf)
        CompileFilter()
    End Sub

    Public Sub Remove(colidx As Integer)
        rDGV.Columns(colidx).HeaderText = rDGV.Columns(colidx).HeaderText.Replace(chfilter, "")
        'Itens.Remove(Itens.Find(Function(it) it.colidx = colidx))
        Itens.FindAll(Function(it) it.colidx = colidx).ForEach(Sub(r) Itens.Remove(r))
        CompileFilter()
    End Sub
    Public Sub RemoveAll()
        Itens.Clear()
        For Each c As DataGridViewColumn In rDGV.Columns
            c.HeaderText = c.HeaderText.Replace(chfilter, "")
        Next
        CompileFilter()
    End Sub

    Public Function containsfilter(colidx As Integer) As Boolean
        Return Itens.Exists(Function(it) it.colidx = colidx)
    End Function

    Public ReadOnly Property Count() As Integer
        Get
            Return Itens.Count
        End Get
    End Property



    Private Sub CompileFilter()
        Dim stbuild As String = ""

        Itens.ForEach(Sub(it)
                          Dim nmcol = rDGV.DataView.Table.Columns(it.colidx).ColumnName
                          If Len(stbuild) > 0 Then stbuild += " and "
                          Select Case it.Ftype
                              Case FilterType.Igual : stbuild += nmcol & " = " & it.Vlr
                              Case FilterType.Maior : stbuild += nmcol & " > " & it.Vlr
                              Case FilterType.Menor : stbuild += nmcol & " < " & it.Vlr
                              Case FilterType.Inicio : stbuild += nmcol & " LIKE '" & it.Vlr & "%'"
                              Case FilterType.Contem : stbuild += nmcol & " LIKE '%" & it.Vlr & "%'"
                          End Select
                          rDGV.Columns(it.colidx).HeaderText += If(rDGV.Columns(it.colidx).HeaderText.Contains(chfilter), "", " " & chfilter)
                      End Sub)

        rDGV.DataView.RowFilter = stbuild
    End Sub

End Class
