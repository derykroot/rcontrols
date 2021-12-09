Public Class rDataGridView
    Inherits DataGridView

    Private dv As DataView
    Private bds As BindingSource
    Private WithEvents colsel As New rDataGridViewColumnSelector(Me)
    Private WithEvents colmenu As New rDgvMenuColumn(Me)

    Public Class DgvSettings
        Public ColsVisible As List(Of rDataGridViewColumnSelector.ColVisible) ' Provisório, criar outra estrutura com o campo com nome da coluna
        Public Property ColidxFrozen As Integer = -2
    End Class

    Private _settings As New DgvSettings()

    Public Event DgvSettingsChange(ByVal e As DgvSettings)

    Public ReadOnly Property DataView() As DataView
        Get
            Return dv
        End Get
    End Property

    Public Function listToTable(Of T)(_o As Object) As DataTable
        Dim _ls As List(Of T) = _o
        If _ls Is Nothing OrElse _ls.Count < 1 Then Return Nothing
        Dim tb As New DataTable()
        ' Dim mis = _ls.FirstOrDefault().GetType().GetMethods.Where(Function(m) m.Name.Contains("get_")).ToList()
        Dim prs = _ls.FirstOrDefault.GetType().GetProperties.ToList()
        For Each pr In prs
            If pr.GetIndexParameters().Length < 1 Then tb.Columns.Add(pr.Name, pr.PropertyType)
        Next
        For Each o In _ls
            Dim rw As DataRow = tb.NewRow()
            For Each pr In prs
                If pr.GetIndexParameters().Length < 1 Then
                    rw(pr.Name) = pr.GetValue(o)
                End If
            Next
            tb.Rows.Add(rw)
        Next
        Return tb
    End Function
    Public Function listToTable(_o As Object) As DataTable
        ' Dim _ls As List(Of Object) = (TryCast(_o, IEnumerable(Of Object))).Cast(Of Object)().ToList()
        Dim _ls As List(Of Object) = CType(_o, IEnumerable(Of Object)).ToList()
        If _ls Is Nothing OrElse _ls.Count < 1 Then Return Nothing
        Dim tb As New DataTable()
        ' Dim mis = _ls.FirstOrDefault().GetType().GetMethods.Where(Function(m) m.Name.Contains("get_")).ToList()
        Dim prs = _ls.FirstOrDefault.GetType().GetProperties.ToList()
        For Each pr In prs
            If pr.GetIndexParameters().Length < 1 Then tb.Columns.Add(pr.Name, pr.PropertyType)
        Next
        For Each o In _ls
            Dim rw As DataRow = tb.NewRow()
            For Each pr In prs
                If pr.GetIndexParameters().Length < 1 Then
                    rw(pr.Name) = pr.GetValue(o)
                End If
            Next
            tb.Rows.Add(rw)
        Next
        Return tb
    End Function

    Public Overloads Property DataSource() As Object
        Get
            Return MyBase.DataSource
        End Get
        Set(value As Object)
            If Not value.GetType().GetInterface("IList") Is Nothing Then
                value = listToTable(value)
            End If
            If TypeOf value Is DataTable Then
                value = New DataView(value)
                Me.dv = value
            End If
            If TypeOf value Is DataView Then
                Me.bds = New BindingSource()
                Me.bds.DataSource = value
                value = bds
            End If
            MyBase.DataSource = value
            Me.Settings = Me.Settings
        End Set
    End Property

    Private Sub colsvisiblecg(ByVal e As List(Of rDataGridViewColumnSelector.ColVisible)) Handles colsel.ColVisibleChangebyUser
        _settings.ColsVisible = e
        RaiseEvent DgvSettingsChange(_settings)
    End Sub
    Private Sub colfrozenchange(e As Integer) Handles colmenu.ColFrozenChangebyUser
        _settings.ColidxFrozen = e
        RaiseEvent DgvSettingsChange(_settings)
    End Sub

    Public Property Settings() As DgvSettings
        Get
            Return _settings
        End Get
        Set(value As DgvSettings)
            _settings = value
            colsel.Loadpresets(value.ColsVisible)
            colmenu.SetFreeze(value.ColidxFrozen)
        End Set
    End Property

End Class
