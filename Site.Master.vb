Imports System.Data
Imports HelperClass
Public Class SiteMaster
    Inherits MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load


        If Not IsPostBack Then
            'LblUserFullName. = Session("sFName")
        End If
    End Sub
End Class