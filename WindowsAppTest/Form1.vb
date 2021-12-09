Imports System.IO
Imports rcontrols

Public Class Form1

    Class ctest
        Public Property id As Integer
        Public Property a As String
        Public Property b As String
        Public Property c As String
        Public Property d As Decimal
        Public Property e As String
        Public Property f As String
        Public Property g As String
        Public Property h As String
        Public Property i As String
        Public Property j As String
    End Class
    Dim ls As New List(Of ctest)
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For i = 1 To 10000
            ls.Add(New ctest With {.id = i, .a = "aaa" & i, .b = "abbbaa" & i, .c = "cca" & i, .d = i * 3.3,
                .e = "cca" & i, .f = "FFFFF", .g = "cca" & i, .h = "qweqweqwe", .i = "cca" & i, .j = "hjjjjjjjjj"})
        Next


        RDataGridView1.DataSource = ls
        opencf(RDataGridView1.Settings, "conf.xml")

    End Sub


    Private Sub RDataGridView1_DgvSettingsChange(e As rDataGridView.DgvSettings) Handles RDataGridView1.DgvSettingsChange
        savecf(RDataGridView1.Settings, "conf.xml")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        RDataGridView1.DataView.RowFilter = " id < 1000 and a LIKE '%2'"
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub


End Class
