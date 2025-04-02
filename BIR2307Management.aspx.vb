Imports System.Data
Imports HelperClass
Imports System.Net.Mail
Imports Microsoft.Office.Interop
Imports System.Data.OleDb
Partial Class BIR2307Septup
    Inherits System.Web.UI.Page
	Dim vSQL As String = ""
	Public PayrollRunList As String = ""
	Public Payroll2307Data As String = ""
	Public BIR2307PostedList As String = ""

	Dim Batch2st As String = ""
	Dim Batch3rd As String = ""
	Dim BatchNo As String = ""

	Public PayrollList As String = ""
	Private Sub BIR2307Septup_Load(sender As Object, e As EventArgs) Handles Me.Load
		'TempGetPayrollList()

		If Session("uid") = "" Then
			Response.Redirect("~/Login")
			Exit Sub
		End If

		If Not IsPostBack Then
			Session("RoleType") = ""
			Dim CanViewApp As Integer = 0

			vSQL = "select Count(User_Id) as Ctr from Tblrightslist where Property_Value='6000' and User_Id='" & Session("uid") & "'"
			CanViewApp = GetRef(vSQL, "")

			If CanViewApp = 0 Then
				Response.Redirect("~/AccessDenied")
			End If

			GetPayrollListHeader()
		End If
		Posted2307List()
	End Sub

	Protected Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
		GetPayrollListHeader()
	End Sub

	Private Sub GetPayrollListHeader()

		Session("BatchNolist") = ""

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim Ctr As Integer = 6
		Dim Checked As String = ""

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try

		vSQL = "select top 10 BatchNo, PayrollPeriod, PayDate, DatePosted, DatePublish " _
			& "from tblPayInstructionHeader where DatePosted is not null order by Id desc"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read

			If Request.Item("chk" & rs("BatchNo")) = "on" Then
				Checked = "Checked='checked'"
			Else
				Checked = ""
			End If

			PayrollRunList += "<tr>" _
				& "<td><input name='chk" & rs("BatchNo") & "' id='chk" & rs("BatchNo") & "' class='form-check' type='checkbox' " & Checked & "></td>" _
				& "<td>" & rs("BatchNo") & "</td>" _
				& "<td>" & rs("PayrollPeriod") & "</td>" _
				& "<td>" & rs("PayDate") & "</td>" _
				& "<td>" & rs("DatePosted") & "</td>" _
				& "<td>" & rs("DatePublish") & "</td>" _
			& "</tr>"



			If Request.Item("chk" & rs("BatchNo")) = "on" Then
				Session("BatchNolist") += "'" & rs("BatchNo") & "',"

				Session("Batch" & Ctr) = rs("BatchNo")
				Ctr -= 1
			End If

		Loop

		rs.Close()

		c.Close()
		c.Dispose()
		cm.Dispose()

		If Session("BatchNolist") <> "" Then
			'Response.Write(Session("BatchNolist").ToString.Substring(0, Session("BatchNolist").ToString.Length - 1) & "<br>")
			BatchNo = Format(Now, "MMddyyyyHHmmss")
			GetPayrollEmployees()
		End If

	End Sub

	Private Sub Posted2307List()

		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""
		Dim vTableName As String = ""

		c.ConnectionString = ConnStr

		vSQL = "select distinct(BatchNumber) as BatchNo, Periodfrom, Periodto, IsPosted, year(Periodfrom), Month(Periodfrom)" _
			& "from tbl2307 " _
			& "where IsPosted=1 order by year(Periodfrom), Month(Periodfrom) "

		'Response.Write(vSQL)

		da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "tblEmployees")
		tblPosted2307.DataSource = ds.Tables("tblEmployees")
		tblPosted2307.DataBind()
		'LblRowCount.Text = tblEmployees.DataSource.Rows.Count
		da.Dispose()
		ds.Dispose()

	End Sub
	Private Sub tblPosted2307_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblPosted2307.SelectedIndexChanged
		Response.Redirect("~/Downloads/2307/" & tblPosted2307.SelectedRow.Cells(3).Text & ".zip")
	End Sub

	Private Sub GetPayrollEmployees()

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim Ctr As Integer = 1

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try

		vSQL = "select distinct(EmpCode) as EmpCode, " _
			& "(select Fullname from tblEmployees where EmpCode=employeeCode) as FullName " _
			& "from tblPayrollSummary " _
			& "where batchno in (" _
			& Session("BatchNolist").ToString.Substring(0, Session("BatchNolist").ToString.Length - 1) & ") " _
			& "order by FullName"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read
			Payroll2307Data += "<tr>" _
				& "<td>" & Ctr & "</td>" _
				& "<td>" & rs("EmpCode") & "</td>" _
				& "<td>" & rs("FullName") & "</td>"

			GetATCList(rs("EmpCode"))

			Payroll2307Data += "</tr>"

			Ctr += 1
		Loop

		rs.Close()



		c.Close()
		c.Dispose()
		cm.Dispose()


	End Sub

	Private Sub GetATCList(EmpCode As String)

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim ATCCode As String = ""
		Dim ATCDisPlay As String = ""
		Dim ATCCtr As Integer = 0

		Dim M1Val As Decimal = 0
		Dim M2Val As Decimal = 0
		Dim M3Val As Decimal = 0
		Dim TaxTotal As Decimal = 0
		Dim TotalWHTax As Decimal = 0
		Dim WI139 As String = "WI139"
		Dim WI140 As String = "WI140"

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try

		vSQL = "select distinct(ATCCode) as ATCCode " _
			& "from tblTaxATCHistory " _
			& "where EmpCode='" & EmpCode & "' and batchno in (" _
			& Session("BatchNolist").ToString.Substring(0, Session("BatchNolist").ToString.Length - 1) & ") " _
			& " "

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read
			If rs("ATCCode") <> "0" Then
				ATCCode += rs("ATCCode")
				ATCDisPlay += rs("ATCCode") & "<br /><br />"
				ATCCtr += 1

				Session("ATCCode" & ATCCtr) = rs("ATCCode")
			End If
		Loop

		Payroll2307Data += "<td>" & ATCDisPlay & "</td>"
		rs.Close()



		If ATCCtr = 2 Then
			For i As Integer = 1 To 6
				vSQL = "select BatchNo, ATCCode, " _
					& "(select TotalTaxable from tblPayrollSummary b where a.BatchNo=b.BatchNo and a.EmpCode=b.EmpCode) As Amt, " _
					& "(select WHTax from tblPayrollSummary b where a.BatchNo=b.BatchNo and a.EmpCode=b.EmpCode) as WHTax " _
					& "from tblTaxATCHistory a " _
					& "where EmpCode='" & EmpCode & "' and BatchNo='" & Session("Batch" & i) & "'"


				cm.CommandText = vSQL
				rs = cm.ExecuteReader
				If rs.Read() Then

					If rs("ATCCode") = WI139 Then
						Select Case i
							Case 1, 2
								M1Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
							Case 3, 4
								M2Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
							Case 5, 6
								M3Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
						End Select
					End If

					If rs("ATCCode") = WI140 Then
						Select Case i
							Case 1, 2
								M1Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
							Case 3, 4
								M2Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
							Case 5, 6
								M3Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
						End Select
					End If



					TotalWHTax += IIf(IsDBNull(rs("WHTax")), 0, rs("WHTax"))

					'Payroll2307Data += "<td>" & IIf(IsDBNull(rs("Amt")), 0, rs("Amt")) & "</td>"
				End If

				Select Case i
					Case 2
						Payroll2307Data += "<td class'text-right'>" & Format(M1Val, "#,###,##0.00") & "</td>"
					Case 4
						Payroll2307Data += "<td class'text-right'>" & Format(M2Val, "#,###,##0.00") & "</td>"
					Case 6
						Payroll2307Data += "<td class'text-right'>" & Format(M3Val, "#,###,##0.00") & "</td>"
				End Select
				rs.Close()
			Next

			TaxTotal = M1Val + M2Val + M3Val

			Payroll2307Data += "<td>" & Format(TaxTotal, "#,###,##0.00") & "</td>"
			Payroll2307Data += "<td>" & Format(TotalWHTax, "#,###,##0.00") & "</td>"

		Else
			For i As Integer = 1 To 6
				vSQL = "select BatchNo, ATCCode, " _
					& "(select TotalTaxable from tblPayrollSummary b where a.BatchNo=b.BatchNo and a.EmpCode=b.EmpCode) As Amt, " _
					& "(select WHTax from tblPayrollSummary b where a.BatchNo=b.BatchNo and a.EmpCode=b.EmpCode) as WHTax " _
					& "from tblTaxATCHistory a " _
					& "where EmpCode='" & EmpCode & "' and BatchNo='" & Session("Batch" & i) & "'"

				cm.CommandText = vSQL
				rs = cm.ExecuteReader
				If rs.Read() Then

					Select Case i
						Case 1, 2
							M1Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
						Case 3, 4
							M2Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
						Case 5, 6
							M3Val += IIf(IsDBNull(rs("Amt")), 0, rs("Amt"))
					End Select

					TotalWHTax += IIf(IsDBNull(rs("WHTax")), 0, rs("WHTax"))
				End If

				Select Case i
					Case 2
						Payroll2307Data += "<td class'text-right'>" & Format(M1Val, "#,###,##0.00") & "</td>"
					Case 4
						Payroll2307Data += "<td class'text-right'>" & Format(M2Val, "#,###,##0.00") & "</td>"
					Case 6
						Payroll2307Data += "<td class'text-right'>" & Format(M3Val, "#,###,##0.00") & "</td>"
				End Select

				rs.Close()
			Next

			TaxTotal = M1Val + M2Val + M3Val

			Payroll2307Data += "<td>" & Format(TaxTotal, "#,###,##0.00") & "</td>"
			Payroll2307Data += "<td>" & Format(TotalWHTax, "#,###,##0.00") & "</td>"

		End If

		CreateBIRData(ATCCode, EmpCode, M1Val, M2Val, M3Val, TaxTotal, TotalWHTax)

		c.Close()
		c.Dispose()
		cm.Dispose()


	End Sub



	Private Sub CreateBIRData(ATCCode As String, EmpId As String,
							  M1Val As Decimal, M2Val As Decimal, M3Val As Decimal,
							  TaxTotal As Decimal, TotalWHTax As Decimal)

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim TotalTaxable As Decimal = 0
		Dim PDFCode As String = ""

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try


		vSQL = "select FullName,TINno, " _
			& "AddressRegistered As EmpAddress, " _
			& "Remarks as ATCDescr1, " _
			& "(select top 1 ATCDescr from tblTaxATCHistory where EmpCode=EmployeeCode and ATCCode='" & ATCCode & "') As ATCDescr, " _
			& "(select Name from tblPositions where id=PositionId) as Position " _
			& "from tblEmployees " _
			& "where EmployeeCode='" & EmpId & "'"

		'Response.Write(vSQL)
		cm.CommandText = vSQL

		rs = cm.ExecuteReader
		Do While rs.Read

			PDFCode = GenerateRandomString(40).ToUpper

			'vSQL = "delete from tbl2307 where EmployeeCode='" & EmpId & "'"
			'CreateRecords(vSQL)



			vSQL = "insert into tbl2307 (Periodfrom,PeriodTo,TaxpayerEmpTINNo,TaxpayerTINNo,EmployeeCode,FullName,PositionDescr,EmpAddress," _
					& "PayorsName,PayorsAddress,IncomePaymentSubjectDescr,ATC," _
					& "[1StMonth],[2ndMonth],[3RdMonth],Total,TaxWithHeld,AutoFileName,BatchNumber," _
					& "CreatedBy,DateCreated,POC,POCPosition,POCTIN) values "

			vSQL += "('07/01/2021','09/30/2021','" & rs("TINno") & "','216-919-045-000','" _
					& EmpId & "','" & rs("FullName") & "','" & rs("Position") & "','" & rs("EmpAddress") & "', " _
					& "'AYALA LAND SALES, INC.','18F Tower One and Exchange Plaza, Ayala Avenue, Makati City','" _
					& rs("ATCDescr") & "','" & ATCCode & "'," _
					& "'" _
					& IIf(M1Val = 0, 0, M1Val) & "','" _
					& IIf(M2Val = 0, 0, M2Val) & "','" _
					& IIf(M3Val = 0, 0, M3Val) & "','" _
					& TaxTotal & "','" & TotalWHTax & "'," _
					& "'" & PDFCode & "','" & BatchNo & "','Admin','" & Now() _
					& "','JOCELYN C. PEDEGLORIO','HR AND FACILITIES MANAGER','106-911-944-00')"

			CreateRecords(vSQL)
			'Response.Write(vSQL & "<br><br>")
			'If Session("TempId") <> EmpId Then
			'End If
			'Session("TempId") = EmpId
		Loop
		rs.Close()

		c.Close()
		c.Dispose()
		cm.Dispose()


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

	Private Sub GetPayrollList()

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try

		vSQL = "select BatchNo, format(PayDate,'MM/dd/yyyy') as Paydate from tblPayInstructionHeader where PostedBy is not null"

		'Response.Write(vSQL)
		cm.CommandText = vSQL

		rs = cm.ExecuteReader
		Do While rs.Read
			PayrollList += "<tr><td></td>"
			PayrollList += "<td>" & rs("BatchNo") & "</td>"
			PayrollList += "<td>" & rs("Paydate") & "</td>"
			PayrollList += "</tr>"
		Loop
		rs.Close()

		c.Close()
		c.Dispose()
		cm.Dispose()


	End Sub


End Class
