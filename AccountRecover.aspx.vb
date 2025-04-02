Imports System.Data
Imports HelperClass
Imports System.Net.Mail
Partial Class Acctrecover
    Inherits System.Web.UI.Page
    Dim vSQL As String = ""

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Response.Redirect("~/", True)
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        If TxtEmpCode.Value.Trim = "" Or TxtEmail.Value.Trim = "" Or TxtEmpCode.Value.Trim = "" And TxtEmail.Value.Trim = "" Then
            TxtEmpCode.Value = ""
            TxtEmail.Value = ""
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The employee code or e-mail address you entered value is incorrect.');", True)
            Exit Sub
        End If


        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim vEmpCode As String = System.Security.SecurityElement.Escape(TxtEmpCode.Value.Trim)
        Dim EmailAdd As String = System.Security.SecurityElement.Escape(TxtEmail.Value.Trim)
        Dim IsExists As String = ""
        Dim Token As String = ""
        Dim ValidToken As String = ""


        c.ConnectionString = ConnStr

        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            'vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        ' =============================================================================
        ' Property_Value='1000' is the Document Controller
        ' =============================================================================
        vSQL = "select FirstName, FullName, EmailAddress from tblEmployees where EmailAddress='" & EmailAdd & "' and EmployeeCode ='" & vEmpCode & "'"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        If rs.Read Then

            If Not IsDBNull(rs("EmailAddress")) Then

                Token = GenerateRandomString(125)
                ValidToken = getEncryptedCode256("@neTw03456Sev3n" & Token)


                vSQL = "insert into tblGenToken_AccessLog (PublicToken, PrivateToken, ValidToken, DateGenerated, DateUsed, IsLock, EmpCode, Email) values " _
                        & "('" & Token & "','" & ValidToken & "','" & ValidToken & "','" & Now & "',null, 0,'" & vEmpCode & "','" & EmailAdd & "')"

                CreateRecords(vSQL)

                SendEmail(rs("FirstName"), rs("EmailAddress"), Token)

            End If
        Else
            TxtEmpCode.Value = ""
            TxtEmail.Value = ""
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The employee code or e-mail address you entered value is incorrect.');", True)
        End If
        rs.Close()

        c.Close()
        c.Dispose()
        cm.Dispose()
    End Sub

    Private Sub SendEmail(ToName As String, ToEmail As String, Token As String)

        Try
            Dim Smtp_Server As New SmtpClient
            Dim e_mail As New MailMessage()
            Dim Tbl As String = ""
            Dim DocStatus As String = ""
            Dim Remarks As String = ""

            Smtp_Server.UseDefaultCredentials = False
            Smtp_Server.Credentials = New Net.NetworkCredential("no-reply@bposerve.com", "uPzGNB&649@2022!")
            Smtp_Server.Port = 25 '587
            Smtp_Server.EnableSsl = True
            Smtp_Server.Host = "smtp.gmail.com"

            e_mail = New MailMessage()
            e_mail.From = New MailAddress("no-reply@bposerve.com")
            e_mail.To.Add(ToEmail)

            Tbl = "<html><body style='font-size:14px'><head><style>" _
                        & "td {border:1px solid #F2F3F4; padding:8px} " _
                        & ".lbl {color:#007BFF; font-weight:bold; font-size:14px} " _
                        & ".lbl2 {color:#000; font-size:14px} " _
                        & ".lbl3 {color:#000; font-size:14px} " _
                        & ".lbl4 {color:#7b7b7b; font-size:14px} " _
                        & ".lbl5 {color:#000; font-size:14px; width:100%; padding-buttom: 20px; border: solid 0px #fff;height: 200px } " _
                        & "</style></head>"

            Tbl += "<div style='width:80%; margin:auto;'><label class='lbl3'>Hi " & ToName & ",</label><br /><br />"
            Tbl += "<Label Class='lbl3'>"

            Tbl += "You have received this email notification because you’ve attempted to reset your password.<br /><br />"

            Tbl += "<label class='lbl3'>To reset your password, click on this link :&nbsp;<a href='https://ess-apps.bposerve.com/ALSI/ResetPassword?Token=" & Token & "'>https://ess-apps.bposerve.com/ALSI/</a></label><br><br>"

            Tbl += "If you didn’t trigger the password reset, please disregard this email.<br /><br />"

            Tbl += "Thank you.<br /><br />"

            Tbl += "Notice: This is a system-generated email. Do not reply.<br /><br />"

            Tbl += "</div></body></html>"


            e_mail.Subject = "Account Recovery"
            e_mail.IsBodyHtml = True
            e_mail.Body = Tbl


            Smtp_Server.Send(e_mail)
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Your request is approved. Please check your email to complete this process.');", True)
            'Response.Redirect("~/", True)
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Sending error: " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');", True)
        End Try
        TxtEmail.Value = ""
        TxtEmpCode.Value = ""
    End Sub

    Public Function GenerateRandomString(ByRef iLength As Integer) As String
        Dim rdm As New Random()
        Dim allowChrs() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
        Dim sResult As String = ""

        For i As Integer = 0 To iLength - 1
            sResult += allowChrs(rdm.Next(0, allowChrs.Length))
        Next

        Return sResult
    End Function
    Public Function getEncryptedCode256(ByVal inputString As String) As String

        Dim Hash As Byte() = New System.Security.Cryptography.SHA256Managed().ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(inputString))
        Dim outputString As New System.Text.StringBuilder()

        For i As Integer = 0 To Hash.Length - 1
            outputString.Append(Hash(i).ToString("X2"))
        Next

        Return outputString.ToString()

    End Function
End Class
