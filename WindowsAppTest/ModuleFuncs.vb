Imports System.IO

Public Module ModuleFuncs
    Public Sub savecf(obj As Object, filename As String)
        Dim xr As New Xml.Serialization.XmlSerializer(obj.GetType())
        Dim fs As New FileStream(filename, FileMode.Create, FileAccess.Write)
        xr.Serialize(fs, obj)
        xr = Nothing
        fs.Close()
    End Sub
    Public Sub opencf(ByRef obj As Object, filename As String)
        If Not (New IO.FileInfo(filename)).Exists Then Exit Sub
        Dim xr As New Xml.Serialization.XmlSerializer(obj.GetType())
        Dim fs As New FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read)
        obj = xr.Deserialize(fs)
        xr = Nothing
        fs.Close()
    End Sub
End Module
