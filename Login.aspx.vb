Imports System.Data
Imports HelperClass


Public Class Login
	Inherits Page

	' PORDUCTION =================================================================================================================================
	'Dim vConnStr As String = "Data Source=10.58.2.87,2014; Initial Catalog=BPOI_DB; User=sa; Password=jeonsoft_2233#; Connect Timeout=90;"
	'Dim vConnectionStr As String = "Data Source=localhost\sqlexpress; Initial Catalog=JFLoan; User=sa; Password=P@$$w0rd; Integrated Security=SSPI; Connect Timeout=90;"

	Dim vConnStr As String = ProdConnStr '"Data Source=10.58.2.87,2014; Initial Catalog=BPOI_DB; User=sa; Password=jeonsoft_2233#; Connect Timeout=90;"
	Dim vConnectionStr As String = ConnStr '"Data Source=localhost\SQLEXPRESS; Initial Catalog=hr-application; User=sa; Password=P@$$w0rd; Integrated Security=SSPI; Connect Timeout=90;"

	' ============================================================================================================================================
	Dim vSQL As String = ""

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

		If Not IsPostBack Then

			'Response.Write(getEncryptedCode256("BP0I@202OQwe@1234"))

			Dim vURL As String = HttpContext.Current.Request.Url.AbsoluteUri
			Dim vIsHTTPS As String = vURL.Substring(0, 5)

			If vIsHTTPS = "http:" Then
				'Response.Redirect("h ttps://ekiosk.bposerve.com/apps/")
			End If
		End If
	End Sub


	Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click

		Dim vXPass As String = ""
		Dim vActiveUsr As String = ""
		'Dim vClientCode As String = System.Security.SecurityElement.Escape(TxtClientCode.Value.Trim.ToUpper())
		Dim vEmpCode As String = System.Security.SecurityElement.Escape(txtU.Value.Trim)
		Dim vCurrPass As String = System.Security.SecurityElement.Escape(txtP.Value.Trim)

		Dim vPublicToken As String = ""
		Dim vPrivateToken As String = ""
		Dim vSystemRandomKey As String = ""
		Dim CheckEmpCode As Integer

		'TxtClientCode.Value = ""
		txtU.Value = ""
		txtP.Value = ""


		vXPass = getEncryptedCode256("BP0I@202OQ" & vCurrPass)

		vSQL = "SELECT EmployeeCode, FirstName, LastName, MiddleName, DepartmentId, AccessCode, EmailAddress FROM tblEmployees " _
			& "WHERE " _
			& "EmployeeCode='" & vEmpCode & "' and " _
			& "EmployeePassword='" & vXPass & "' and " _
			& "DateSeparated is null"

		vActiveUsr = GetUserInfo(vSQL)

		'Response.Write(vActiveUsr & " - - " & vEmpCode.ToUpper)

		If vActiveUsr.ToUpper = vEmpCode.ToUpper Then
			Session("uid") = vEmpCode


			vSQL = "insert into tblAudit (TranDate, TranTime, User_Id, MachineId, Event, OldValues, NewValues, Remarks, Module) values " _
				& "('" & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','" & Format(Now, "HH:mm:ss") & "'," _
				& "'" & vEmpCode & "','" & Request.ServerVariables("REMOTE_ADDR") & "','LOGIN','',''," _
				& "'Successful LogIn on " & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','LOGIN')"
			'BuildData(vSQL)
			CreateRecords(vSQL)



			'Dim CanViewApp As Integer = 0

			Dim RightsCnt As Integer = 0
			vSQL = "select Count(User_Id) as Ctr from Tblrightslist where Property_Value in ('1000','2000') and User_Id='" & Session("uid") & "'"

			RightsCnt = GetRef(vSQL, "")

			If RightsCnt = 0 Then
				Response.Redirect("~/PayslipReport", True)
			Else
				Response.Redirect("~/GeneratePayroll", True)
			End If


		Else

			vSQL = "insert into tblAudit (TranDate, TranTime, User_Id, MachineId, Event, OldValues, NewValues, Remarks, Module) values " _
				& "('" & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','" & Format(Now, "HH:mm:ss") & "'," _
				& "'" & vEmpCode & "','" & Request.ServerVariables("REMOTE_ADDR") & "','LOGIN','',''," _
				& "'Invalid LogIn Userid Attempt on " & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','LOGIN')"
			'BuildData(vSQL)
			CreateRecords(vSQL)


			txtU.Value = ""
			txtP.Value = ""
			'TxtClientCode.Value = ""
			lblError.Text = "Supply the correct Client Code, Employee Code and Password to access your account."
			dvError.Visible = True
		End If

		'Response.Write(Session("uid") & " " & Session.SessionID)

	End Sub
	Function GetUserInfo(pSQL As String) As String
		Dim vResult As String = ""

		'Dim Custom_ConnStr As String =
		'    "Data Source=ctc-qa01,20143; Initial Catalog=Training; User ID=sa; Password=P@$$w0rd@!@#$%; Connect Timeout=90;"

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader

		c.ConnectionString = vConnectionStr

		Try
			c.Open()
			lblError.Text = "Server Status : Connected"
		Catch ex As SqlClient.SqlException
			lblError.Text = "Error occurred while trying to connect to database. Error is : " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & " "
			c.Dispose()
			cm.Dispose()

			Return vResult
			Exit Function
		End Try

		cm.Connection = c
		cm.CommandText = pSQL

		Try
			rs = cm.ExecuteReader
			If rs.Read Then
				vResult = rs(0)
				Session("EmployeeCode") = rs("EmployeeCode")
				Session("FirstName") = rs("FirstName")
				Session("LastName") = rs("LastName")
				Session("MiddleName") = rs("MiddleName")
				Session("DepartmentId") = rs("DepartmentId")
				Session("AccessCode") = rs("AccessCode")
				Session("EmailAddress") = rs("EmailAddress")

				Session("sFName") = rs(1) & " " & rs(2)
			End If
			rs.Close()
			'lblError.Text = ""
		Catch ex As Exception
			'lblError.Text = "Error is : " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & " "
			c.Close()
			c.Dispose()
			cm.Dispose()

			Return vResult
			Exit Function
		End Try

		c.Close()
		c.Dispose()
		cm.Dispose()

		Return vResult
	End Function

	'Private Sub BuildData(pSQL As String)
	'	Dim c As New SqlClient.SqlConnection
	'	Dim cm As New SqlClient.SqlCommand
	'	'Dim rs As SqlClient.SqlDataReader
	'	'Dim vCurrDateTime As Date = Format(Now, "MM-dd-yyyy HH:mm:ss")
	'	'Dim vDateParam As Date

	'	c.ConnectionString = vConnectionStr

	'	Try
	'		c.Open()
	'	Catch ex As SqlClient.SqlException
	'		c.Dispose()
	'		cm.Dispose()
	'		Exit Sub
	'	End Try

	'	cm.Connection = c
	'	cm.CommandText = pSQL

	'	Try
	'		cm.ExecuteNonQuery()

	'		'vDateParam = vCurrDateTime.AddMinutes(-30)
	'		'cm.CommandText = "delete from db_accounts.dbo.tblBPOISSO where DateConnect < '" _
	'		'& Format(CDate(vDateParam), "yyyy-MM-dd HH:mm") & "'"
	'		'cm.ExecuteNonQuery()

	'	Catch ex As Exception
	'		c.Close()
	'		c.Dispose()
	'		cm.Dispose()
	'		Exit Sub
	'	End Try

	'	c.Close()
	'	c.Dispose()
	'	cm.Dispose()
	'End Sub

	'Private Sub SyncEmployeeList()
	'	Dim c As New SqlClient.SqlConnection
	'	Dim cm As New SqlClient.SqlCommand
	'	Dim rs As SqlClient.SqlDataReader
	'	Dim DateSeparated As String = ""

	'	c.ConnectionString = ProdConnStr

	'	Try
	'		c.Open()
	'	Catch ex As SqlClient.SqlException
	'		c.Dispose()
	'		cm.Dispose()
	'		Exit Sub
	'	End Try

	'	cm.Connection = c

	'	vSQL = "SELECT EmployeeCode, FirstName, LastName, MiddleName, DateSeparated, AccessCode, EmailAddress, " _
	'		& "(select Code from tblDepartments where tblEmployees.DepartmentId = tblDepartments.id) As DeptCode, " _
	'		& "(select Name from tblDepartments where tblEmployees.DepartmentId = tblDepartments.id) as DeptName " _
	'		& "FROM tblEmployees where AccessCode is not null "
	'	cm.CommandText = vSQL

	'	Try
	'		rs = cm.ExecuteReader

	'		Do While rs.Read

	'			If Not IsDBNull(rs("DateSeparated")) Then
	'				DateSeparated = ", Date_Resign='" & rs("DateSeparated") & "' "
	'			Else
	'				DateSeparated = ", Date_Resign=NULL "
	'			End If

	'			vSQL = "update employee_master set " _
	'				& "Emp_Fname='" & rs("FirstName").ToString.Trim & "', Emp_Mname='" & rs("MiddleName").ToString.Trim & "'," _
	'				& "Emp_Lname='" & rs("LastName").ToString.Trim & "', " _
	'				& "DeptCd='" & rs("DeptCode").ToString.Trim & "', " _
	'				& "Emp_Email='" & rs("EmailAddress").ToString.Trim & "' " _
	'				& DateSeparated _
	'				& "where Emp_Cd='" & rs("EmployeeCode") & "'"
	'			CreateRecords(vSQL)

	'			'vSQL = "update employee_master set " _
	'			'	& "Emp_Cd='" & rs("EmployeeCode") & "', ESSEmp_Cd='" & rs("AccessCode") & "' " _
	'			'	& "where Emp_Cd='" & rs("AccessCode") & "'"
	'			'vSQL = "update Policy_logs set IsAgree='" & rs("EmployeeCode") & "' where IsAgree='" & rs("AccessCode") & "'"
	'			'Response.Write(vSQL & "<br>")

	'		Loop
	'		rs.Close()

	'		vSQL = "delete from emp_department_ref"
	'		CreateRecords(vSQL)

	'		vSQL = "select Code, Name from tblDepartments"
	'		cm.CommandText = vSQL

	'		Try
	'			rs = cm.ExecuteReader
	'			Do While rs.Read
	'				vSQL = "insert into emp_department_ref (DeptCd, Descr) values ('" & rs("Code") & "','" & rs("Name") & "')"
	'				CreateRecords(vSQL)
	'			Loop
	'			rs.Close()

	'		Catch ex As Exception
	'			c.Close()
	'			c.Dispose()
	'			cm.Dispose()
	'			Exit Sub
	'		End Try

	'		vSQL = "insert into audit (TranDate, TranTime, User_Id, MachineId, Event, OldValues, NewValues, Remarks, Module) values " _
	'			& "('" & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','" & Format(Now, "HH:mm:ss") & "'," _
	'			& "'" & Session("uid") & "','" & Request.ServerVariables("REMOTE_ADDR") & "','SYNC Employees','',''," _
	'			& "'Update employee details " & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','LOGIN')"
	'		BuildData(vSQL)

	'	Catch ex As Exception
	'		c.Close()
	'		c.Dispose()
	'		cm.Dispose()
	'	End Try

	'	c.Close()
	'	c.Dispose()
	'	cm.Dispose()

	'End Sub

	'Protected Sub lnkForgetPass_Click(sender As Object, e As EventArgs) Handles lnkForgetPass.Click
	'	'Response.Redirect("h ttps://ess.bposerve.com")
	'End Sub

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

	Private Sub LinkForgotAcct_Click(sender As Object, e As EventArgs) Handles LinkForgotAcct.Click
		Response.Redirect("~/AccountRecover", True)
	End Sub
End Class




