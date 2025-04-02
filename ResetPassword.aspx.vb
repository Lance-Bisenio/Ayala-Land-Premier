Imports System.Data
Imports HelperClass
Partial Class ResetPassword
    Inherits System.Web.UI.Page

    Dim vSQL As String = ""
    Dim Token As String = ""

    Private Sub ResetPassword_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then


            Try
                Token = System.Security.SecurityElement.Escape(Request.Item("Token").Trim)
            Catch ex As Exception
                Response.Redirect("~/")
            End Try

            Token = getEncryptedCode256("@neTw03456Sev3n" & Token)

            Dim ValidToken As Integer = 0

            vSQL = "select count(ValidToken) as Ctr from tblGenToken_AccessLog where ValidToken='" & Token & "' and IsLock=0"
            ValidToken = GetRef(vSQL, 0)

            If ValidToken = 1 Then
                vSQL = "update tblGenToken_AccessLog set IsLock=1, DateUsed='" & Now & "' where ValidToken='" & Token & "' and IsLock=0"
                CreateRecords(vSQL)
            Else
                Response.Redirect("~/ExpiredToken")
            End If

        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim NewPass As String = System.Security.SecurityElement.Escape(txtNew.Value.Trim)
        Dim ConfirmPass As String = System.Security.SecurityElement.Escape(txtConfirm.Value.Trim)
        Dim EmpCode As String = ""
        Dim IsAdmin As String = ""


        If NewPass.Trim <> ConfirmPass.Trim Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The Password value is not match. Please check and try again.');", True)
            txtNew.Value = ""
            txtConfirm.Value = ""

        Else
            Token = System.Security.SecurityElement.Escape(Request.Item("Token").Trim)
            vSQL = "select EmpCode from tblGenToken_AccessLog where PublicToken='" & Token & "' "
            EmpCode = GetRef(vSQL, 0)

            vSQL = "select CustomField1 from tblemployees where EmployeeCode='" & EmpCode & "' "
            IsAdmin = GetRef(vSQL, 0)


            NewPass = System.Security.SecurityElement.Escape(NewPass)
            NewPass = getEncryptedCode256("BP0I@202OQ" & NewPass)

            vSQL = "update tblemployees set EmployeePassword='" & NewPass & "' where EmployeeCode='" & EmpCode & "'"
            CreateRecords(vSQL)
            'Response.Write(vSQL)
            Session("uid") = EmpCode

            If IsAdmin = "Admin" Then
                Response.Redirect("~/GeneratePayroll")
            Else
                Response.Redirect("~/PayslipReport")
            End If

        End If



    End Sub

    Public Function getEncryptedCode256(ByVal inputString As String) As String

        Dim Hash As Byte() = New System.Security.Cryptography.SHA256Managed().ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(inputString))
        Dim outputString As New System.Text.StringBuilder()

        For i As Integer = 0 To Hash.Length - 1
            outputString.Append(Hash(i).ToString("X2"))
        Next

        Return outputString.ToString()

    End Function

End Class
