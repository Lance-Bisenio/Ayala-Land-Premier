Imports System.Data
Imports HelperClass
Imports System.Net.Mail
Imports System.Data.OleDb
Imports Microsoft.Office.Interop
Partial Class ReportSetup
	Inherits System.Web.UI.Page
	Dim vSQL As String = ""

	Private Sub ReportSetup_Load(sender As Object, e As EventArgs) Handles Me.Load
		If Session("uid") = "" Then
			Response.Redirect("~/Login")
		End If

		If Not IsPostBack Then
			Session("EmpId") = ""
			Dim CanViewApp As Integer = 0
			Dim PendingPublishCtn As Integer = 0

			vSQL = "select Count(User_Id) as Ctr from Tblrightslist where Property_Value='5000' and User_Id='" & Session("uid") & "'"

			CanViewApp = GetRef(vSQL, "")

			If CanViewApp = 1 Then
				EmpFilter.Visible = True
			Else
				'EmpFilter.Visible = False
				'Session("EmpId") = Session("uid") 
				Response.Redirect("~/AccessDenied")
			End If

			BuildCombo("select EmployeeCode, FullName from tblEmployees order by FullName", CmdEmployeeList)
			'where Active=1 and DateSeparated is null 

			CmdEmployeeList.Items.Add(" ")
			CmdEmployeeList.SelectedValue = " "

			vSQL = "select count(BatchNo) as Ctr " _
				& "from tblPayInstructionHeader " _
				& "where DatePosted is not null and DatePublish is null "

			PendingPublishCtn = GetRef(vSQL, 0)

			If PendingPublishCtn >= 1 Then
				BtnPublishPayroll.Disabled = False
			Else
				BtnPublishPayroll.Disabled = True
			End If


			vSQL = "select BatchNo, (cast(BatchNo as varchar)+ ' - PayDate: ' + cast(FORMAT(PayDate, 'MM/dd/yyyy')as varchar)) as test  " _
				& "from tblPayInstructionHeader " _
				& "where DatePosted is not null and DatePublish is null " _
				& "order by BatchNo desc"

			BuildCombo(vSQL, CmdLockedPayrollRun)
			'DmdLockedPayrollRun.Items.Add(" ")
			'DmdLockedPayrollRun.SelectedValue = " "


			GetPayrollRunList()

		End If
	End Sub

	Private Sub GetPayrollRunList()
		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""
		Dim vDateNow As Date

		c.ConnectionString = ConnStr

		vSQL = "select BatchNo,PayrollPeriod, FORMAT(PayDate, 'MM/dd/yyyy') as PayDate,FileNameRecurring,FileNameOneTime,Remarks,CreatedBy,DateCreated,PostedBy,DatePosted " _
			& "from tblPayInstructionHeader " _
			& "where BatchNo is not null and PublishBy is not null " & vFilter _
			& "order by BatchNo desc"

		'& "and PayDate >='" & Now.AddDays(-1) & "'" _
		da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "tblEmployees")
		tblPayrollRun.DataSource = ds.Tables("tblEmployees")
		tblPayrollRun.DataBind()

		da.Dispose()
		ds.Dispose()
	End Sub

	Private Sub tblPayrollRun_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblPayrollRun.SelectedIndexChanged
		If Session("EmpId") = "" Then
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Please select employee.');", True)
			Exit Sub
		End If

		Dim PublicToken As String = GenerateRandomString(64)
		Dim PrivateToken As String = getEncryptedCode256("@neTw03456Sev3n" & PublicToken)

		Session("Token") = PublicToken

		vSQL = "insert into tblPayslipToken (PublicToken,PrivateToken,BatchNo,EmpCode,PayDate,IsActive,CreatedBy,DateCreated) values (" _
			& "'" & PublicToken & "'," _
			& "'" & PrivateToken & "'," _
			& "'" & tblPayrollRun.SelectedRow.Cells(1).Text.Trim & "'," _
			& "'" & Session("EmpId") & "'," _
			& "'" & tblPayrollRun.SelectedRow.Cells(3).Text.Trim & "'," _
			& "1," _
			& "'" & Session("uid") & "'," _
			& "'" & Now & "')"
		CreateRecords(vSQL)

		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "OpenForm();", True)

		'GetEmployeeInfo()
	End Sub

	Private Sub tblPayrollRun_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblPayrollRun.PageIndexChanging
		tblPayrollRun.PageIndex = e.NewPageIndex
		tblPayrollRun.SelectedIndex = -1
		GetPayrollRunList()
	End Sub
	Private Sub CmdEmployeeList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmdEmployeeList.SelectedIndexChanged
		Session("EmpId") = CmdEmployeeList.SelectedValue
		Get2307List()
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

	Private Sub SendEmail(ToEmail As String)
		Try
			Dim Smtp_Server As New SmtpClient
			Dim e_mail As New MailMessage()
			Dim Tbl As String = ""
			Dim DueDate As String = ""

			Smtp_Server.UseDefaultCredentials = False
			Smtp_Server.Credentials = New Net.NetworkCredential("No-Reply@bposerve.com", "GxsA2u1Ln6bhbaSV")
			Smtp_Server.Port = 25 '587
			Smtp_Server.EnableSsl = True
			Smtp_Server.Host = "smtp.gmail.com"

			e_mail = New MailMessage()
			e_mail.From = New MailAddress("No-Reply@bposerve.com")
			e_mail.To.Add(ToEmail)

			Tbl = "<html><body style='font-size:14px'><head><style>" _
						& "td {border:1px solid #F2F3F4; padding:8px} " _
						& ".lbl {color:#007BFF; font-weight:bold; font-size:14px} " _
						& ".lbl2 {color:#000; font-size:14px} " _
						& ".lbl3 {color:#000; font-size:14px} " _
						& ".lbl4 {color:#7b7b7b; font-size:14px} " _
						& ".lbl5 {color:#000; font-size:14px; width:100%; padding-buttom: 20px; border: solid 0px #fff;height: 200px } " _
						& "</style></head>"

			Tbl += "<Label Class='lbl3'>"


			vSQL = "select format(PayDate,'MMM dd, yyyy') as PayDate from tblPayInstructionHeader where BatchNo='" & CmdLockedPayrollRun.SelectedValue & "'"
			DueDate = GetRef(vSQL, "")

			Tbl += "Congratulations! Your payment slip for " & DueDate & " is now published through the BPOI Employee Self-Service System.<br /><br />"

			Tbl += "<label class='lbl3'>To access the Payment Slip portal, click on this Link: &nbsp;<a href='https://ess-apps.bposerve.com/ALSI/'>https://ess-apps.bposerve.com/ALSI/</a></label><br><br>"

			Tbl += "Thank you.<br /><br />"

			Tbl += "Notice: This is a system-generated email. Do not reply.<br /><br />"

			Tbl += "</div></body></html>"

			e_mail.Subject = "Online Payslip"
			e_mail.IsBodyHtml = True
			e_mail.Body = Tbl


			Smtp_Server.Send(e_mail)
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Message Sent Successfully.');", True)
			'Response.Redirect("~/", True)
		Catch ex As Exception
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Sending error: " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');", True)
		End Try
	End Sub

	Private Sub BtnSubmitPublish_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitPublish.ServerClick
		'Exit Sub
		Dim c As New SqlClient.SqlConnection(ConnStr)
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim EmailList As String = ""
		c.Open()
		cm.Connection = c

		vSQL = "select EmailAddress from tblEmployees where CustomDateField1 is null and Active=1 and DateSeparated is null"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read
			EmailList += rs("EmailAddress") & ","
		Loop

		EmailList = EmailList.Substring(0, EmailList.Length - 1)
		rs.Close()

		cm.Dispose()
		c.Close()
		c.Dispose()

		vSQL = "update tblPayInstructionHeader set PublishBy='" & Session("uid") & "', DatePublish='" & Now & "' " _
			& "where BatchNo='" & CmdLockedPayrollRun.SelectedValue & "'"

		CreateRecords(vSQL)

		SendEmail(EmailList)

		GetPayrollRunList()

		vSQL = "select BatchNo, (cast(BatchNo as varchar)+ ' - PayDate: ' + cast(FORMAT(PayDate, 'MM/dd/yyyy')as varchar)) as test  " _
				& "from tblPayInstructionHeader " _
				& "where DatePosted is not null and DatePublish is null " _
				& "order by BatchNo desc"

		BuildCombo(vSQL, CmdLockedPayrollRun)

	End Sub

	Private Sub Get2307List()
		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""

		c.ConnectionString = ConnStr

		vSQL = "select Periodfrom, Periodto, BatchNumber, year(Periodfrom), Month(Periodfrom) from tbl2307 " _
				& "where EmployeeCode='" & Session("EmpId") & "' and IsPosted=1 order by year(Periodfrom), Month(Periodfrom) desc"

		da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "Tbl2307")
		Tbl2307.DataSource = ds.Tables("Tbl2307")
		Tbl2307.DataBind()

		da.Dispose()
		ds.Dispose()
	End Sub

	Private Sub Tbl2307_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Tbl2307.SelectedIndexChanged
		Dim TargetFilename As String = ""
		Dim Filename As String = ""

		vSQL = "select AutoFileName from tbl2307 where BatchNumber='" & Tbl2307.SelectedRow.Cells(1).Text & "' and EmployeeCode='" & Session("EmpId") & "'"

		'Response.Write(vSQL)
		Filename = GetRef(vSQL, "")

		'TargetFilename = Server.MapPath(".") & "\Downloads\2307\" & Filename & ".pdf"

		Response.Redirect("~/Downloads/2307/" & Filename & ".pdf")
	End Sub

	Private Sub Tbl2307_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles Tbl2307.PageIndexChanging
		Tbl2307.PageIndex = e.NewPageIndex
		Tbl2307.SelectedIndex = -1
		Get2307List()
	End Sub

End Class
