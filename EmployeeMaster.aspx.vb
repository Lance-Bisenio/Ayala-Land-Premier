Imports System.Data
Imports HelperClass
Imports System.Net.Mail
Imports Microsoft.Office.Interop
Imports System.Data.OleDb

Partial Class EmployeeMaster
    Inherits System.Web.UI.Page
	Dim vSQL As String = ""
	Public TblOwner As String
	Public DuplicateEmpCode As String
	Public DuplicateBankAcct As String
	Public DuplicateTIN As String
	Public DuplicateEmail As String

	Private Sub EmployeeMaster_Load(sender As Object, e As EventArgs) Handles Me.Load

		If Session("uid") = "" Then
			Response.Redirect("~/Login")
			Exit Sub
		End If

		If Not IsPostBack Then
			Session("RoleType") = ""
			Dim CanViewApp As Integer = 0

			vSQL = "select Count(User_Id) as Ctr from Tblrightslist where Property_Value='1000' and User_Id='" & Session("uid") & "'"
			CanViewApp = GetRef(vSQL, "")

			If CanViewApp = 0 Then
				Response.Redirect("~/AccessDenied")
			End If

			CmdResign.Items.Add("Resigned")
			CmdResign.Items.Add("Active")
			CmdResign.SelectedValue = "Active"

			CmdStatus.Items.Add("Active")
			CmdStatus.Items.Add("In-Active")
			CmdStatus.SelectedValue = "Active"

			CmdEditStatus.Items.Add("Active")
			CmdEditStatus.Items.Add("In-Active")


			GetEmployeeList()
			ExceptionReports()
			BtnUpdate.Disabled = True
		End If
	End Sub


	Private Sub releaseObject(ByVal obj As Object)
		Try
			System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
			obj = Nothing
		Catch ex As Exception
			obj = Nothing
		Finally
			GC.Collect()
		End Try
	End Sub

	Private Sub GetEmployeeList()
		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""
		Dim vTableName As String = ""


		c.ConnectionString = ConnStr

		'vFilter += " Emp_Cd is not null " 'and Date_Resign is not null 

		If TxtKeywords.Text.Trim <> "" Then
			vFilter += "and (EmployeeCode like '%" & TxtKeywords.Text.Trim & "%' or " _
				& "LastName like '%" & TxtKeywords.Text.Trim & "%' or " _
				& "FirstName like '%" & TxtKeywords.Text.Trim & "%') "
		End If

		If CmdResign.SelectedValue = "Resigned" Then
			vFilter += "and DateSeparated is not null "
		Else
			vFilter += "and DateSeparated is null "
		End If

		If CmdStatus.SelectedValue = "In-Active" Then
			vFilter += "and Active=0 "
		Else
			vFilter += "and Active=1 "
		End If


		vSQL = "select EmployeeCode, FullName, MiddleName, AddressRegistered, FORMAT(BirthDate,'MM/dd/yyyy') as BirthDate, Active, " _
			& "FORMAT(DateHired, 'MM/dd/yyyy') as DateHired, format(DateSeparated,'MM/dd/yyyy') as DateSeparated, TaxCodeId, RegionId, Monthlyrate, PositionId,  " _
			& "GenderId, TINNo, EmailAddress, BankAccountId, BankAccountTypeId, " _
			& "BankAccountNo, BankAccountNo, CostCenterId, PayGroupid, " _
			& "(select Name from tblPositions where Id=PositionId) as PosName, " _
			& "(select Name from tblLocations where Id=LocationId) as LocName, " _
			& "(select TaxPercent*100 from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as TaxCode, " _
			& "(select IsNonVat from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as Vat, " _
			& "(select  VatPercent*100 from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as VatPercent, " _
			& "(select Name from tblRegions where id=RegionId) as Region, " _
			& "(select SysCode from tblBanks where id=BankAccountId) as Bank, " _
			& "(select Name from tblCostCenters where id=CostCenterId) as CostCenter, " _
			& "(select Name from tblPayGroup where id=PayGroupId) as PayGroup, " _
			& "(select Name from tblGenders where id=GenderId) as Gender, " _
			& "(select Name from tblDivision where id=DivisionId) as Div, CustomField2,Remarks " _
			& "from tblEmployees b " _
			& "where CustomField1 is null " & vFilter & " order by LastName"

		'Response.Write(vSQL)

		da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "tblEmployees")
		tblEmployees.DataSource = ds.Tables("tblEmployees")
		tblEmployees.DataBind()
		LblRowCount.Text = tblEmployees.DataSource.Rows.Count
		da.Dispose()
		ds.Dispose()
	End Sub

	Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
		BtnUpdate.Disabled = True
		BtnSendEmail.Enabled = False
		tblEmployees.SelectedIndex = -1
		GetEmployeeList()
		Session("TranID") = ""
		ExceptionReports()
	End Sub

	Private Sub tblEmployees_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblEmployees.PageIndexChanging
		tblEmployees.PageIndex = e.NewPageIndex
		tblEmployees.SelectedIndex = -1

		GetEmployeeList()

		Session("TranID") = ""

	End Sub

	Private Sub BtnSubmitUpload_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitUpload.ServerClick
		BtnUpdate.Disabled = False
		BtnSendEmail.Enabled = False
		Dim TargetFilename As String

		If TxtFileUpdateMasterData.FileName = "" Then 'TxtFileName.FileName = "" And 
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Please select file to upload.');", True)
			Exit Sub
		End If

		'If TxtFileName.FileName <> "" Then
		'	TargetFilename = Server.MapPath(".") & "\Uploaded\SystemInputFiles\" & Format(Now(), "MMddyyyyHHmmss") & "-" & TxtFileName.FileName
		'	TxtFileName.SaveAs(TargetFilename)
		'	ReadExcelData(TargetFilename, "tblEmployees")
		'End If

		If TxtFileUpdateMasterData.FileName <> "" Then
			TargetFilename = Server.MapPath(".") & "\Uploaded\SystemInputFiles\" & Format(Now(), "MMddyyyyHHmmss") & "-" & TxtFileUpdateMasterData.FileName
			TxtFileUpdateMasterData.SaveAs(TargetFilename)
			'ReadExcelAddNewData(TargetFilename, "tblEmployees")
			ReadExcelUpdateData(TargetFilename, "tblEmployees")
		End If



		'GetEmployeeList()
		'ExceptionReports()
	End Sub

	Private Sub ReadExcelData(FilePath As String, TblName As String)

		Dim xlApp As Excel.Application
		Dim xlWorkBook As Excel.Workbook
		Dim xlWorkSheet As Excel.Worksheet

		Dim IsEmpty As String = ""
		Dim TblColName As String = ""
		Dim TblColData As String = ""
		Dim TblTaxCol As String = ""
		Dim TblTaxValue As String = ""
		Dim EmpList As String = ""

		xlApp = New Excel.ApplicationClass
		xlWorkBook = xlApp.Workbooks.Open(FilePath)
		xlWorkSheet = xlWorkBook.Worksheets("Sheet1")

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

		For i As Integer = 2 To 500
			IsEmpty = xlWorkSheet.Cells(i, 1).value

			If IsEmpty = "" Then
				Exit For
			End If

			EmpList += "'" & xlWorkSheet.Cells(i, 2).value & "',"

			'Collect Region
			vSQL = "select TblColName, SourceCol from tblExcelImportProperties " _
					& "where Remarks='GetRef' and Active=0 order by SourceCol"

			'Response.Write(vSQL)
			cm.CommandText = vSQL

			rs = cm.ExecuteReader
			Do While rs.Read

				Select Case rs("TblColName")
					Case "PositionId"
						BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblPositions")

					Case "CostCenterId"
						BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblCostCenters")

					Case "PayGroupId"
						BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblPayGroup")

					Case "LocationId"
						BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblLocations")

					Case "DivisionId"
						BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblDivision")

				End Select
			Loop
			rs.Close()
		Next

		For i As Integer = 2 To 500
			IsEmpty = xlWorkSheet.Cells(i, 1).value

			If IsEmpty = "" Then
				Exit For
			End If

			TblColName = ""
			TblColData += "("

			TblTaxCol = ""
			TblTaxValue += "("
			'===============================================================================================================
			vSQL = "select TblName, TblColName, SourceCol, Remarks from tblExcelImportProperties " _
				& "where Active=0 order by SourceCol"
			'Response.Write(vSQL)
			cm.CommandText = vSQL

			rs = cm.ExecuteReader
			Do While rs.Read

				If rs("TblName") = "tblEmployeeTaxRef" Then
					TblTaxCol += rs("TblColName") & ","
					TblTaxValue += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
				End If

				If rs("TblName") = "tblEmployees" Then
					TblColName += rs("TblColName") & ","

					If rs("Remarks") = "GetRef" Then
						Select Case rs("TblColName")
							Case "RegionId"
								vSQL = "Select id From tblRegions where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							Case "BankAccountId"
								vSQL = "Select id From tblBanks where SysCode='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							Case "LocationId"
								vSQL = "Select id From tblLocations where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							Case "PositionId"
								vSQL = "Select id From tblPositions where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							Case "CostCenterId"
								vSQL = "Select id From tblCostCenters where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							Case "PayGroupId"
								vSQL = "Select id From tblPayGroup where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							Case = "GenderId"
								vSQL = "Select id From tblGenders where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							Case "DivisionId"
								vSQL = "Select id From tblDivision where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
						End Select


						'Response.Write(vSQL & "<br><br>")
					End If

					If rs("Remarks") = "GetRef" Then
						TblColData += "'" & GetRef(vSQL, 0) & "',"
					Else
						TblColData += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
					End If
					vSQL = ""

					'Response.Write(TblColData & "<br>")

				End If

			Loop
			rs.Close()


			'===============================================================================================================
			TblColData = TblColData.Substring(0, TblColData.Length - 1) & "),"
			'TblColData += "),"

			TblTaxValue = TblTaxValue.Substring(0, TblTaxValue.Length - 1) & "),"
			'TblTaxValue += "),"
		Next

		c.Close()
		c.Dispose()
		cm.Dispose()


		TblColName = TblColName.Substring(0, TblColName.Length - 1)
		TblColData = TblColData.Substring(0, TblColData.Length - 1)

		vSQL = "delete from " & TblName & " where EmployeeCode in (" & EmpList.Substring(0, EmpList.Length - 1) & ")"
		CreateRecords(vSQL)

		vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
		CreateRecords(vSQL)
		response.Write(vSQL)


		TblTaxCol = TblTaxCol.Substring(0, TblTaxCol.Length - 1)
		TblTaxValue = TblTaxValue.Substring(0, TblTaxValue.Length - 1)

		vSQL = "insert into tblEmployeeTaxRef (" & TblTaxCol & ") values " & TblTaxValue
		CreateRecords(vSQL)

		vSQL = "update tblEmployees set DateSeparated=null where DateSeparated='1900-01-01 00:00:00.000'"
		CreateRecords(vSQL)


		xlWorkBook.Close()
		xlApp.Quit()

		releaseObject(xlApp)
		releaseObject(xlWorkBook)
		releaseObject(xlWorkSheet)

	End Sub

	Private Sub ReadExcelAddNewData(FilePath As String, TblName As String)

		Dim xlApp As Excel.Application
		Dim xlWorkBook As Excel.Workbook
		Dim xlWorkSheet As Excel.Worksheet

		Dim IsEmpty As String = ""
		Dim TblColName As String = ""
		Dim TblVatColName As String = ""
		Dim TblColData As String = ""
		Dim TblTaxCol As String = ""
		Dim TblTaxValue As String = ""
		Dim EmpList As String = ""
		Dim TempVal As String = ""

		xlApp = New Excel.ApplicationClass
		xlWorkBook = xlApp.Workbooks.Open(FilePath)
		Try
			xlWorkSheet = xlWorkBook.Worksheets("Sheet1")
		Catch ex As Exception
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Sheet1 cannot be found in the uploaded file. \n\nPlease change the sheet name to Sheet1 then re-upload.');", True)
			Exit Sub
		End Try


		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim TinNo As String = ""

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try


		Dim CntEmpcode As Integer = 0

		For i As Integer = 2 To 1000
			IsEmpty = xlWorkSheet.Cells(i, 1).value

			If IsEmpty = "" Then
				Exit For
			End If

			'===============================================================================================================

			If xlWorkSheet.Cells(i, 1).value = "NewHire" Then
				'For i As Integer = 2 To 500
				IsEmpty = xlWorkSheet.Cells(i, 1).value

				If IsEmpty = "" Then
					Exit For
				End If

				' Check if the employee code exsist in the employee master table
				vSQL = "select count(EmployeeCode) from tblEmployees where EmployeeCode='" & xlWorkSheet.Cells(i, 2).value & "'"
				CntEmpcode = GetRef(vSQL, 0)

				'Response.Write(CntEmpcode & " - " & vSQL & "<br>")


				If CntEmpcode = 0 Then
					EmpList += "'" & xlWorkSheet.Cells(i, 2).value & "',"

					'Check all employee reference if exist in the employee reference table. If not, create new records
					'-----------------------------------------------------------------------------------------------------------
					vSQL = "select TblColName, SourceCol from tblExcelImportProperties " _
						& "where TblName='tblEmployeesUpdate' and Remarks='GetRef' and Active=0 order by SourceCol"
					cm.CommandText = vSQL

					rs = cm.ExecuteReader
					Do While rs.Read

						Select Case rs("TblColName")
							Case "PositionId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblPositions")

							Case "CostCenterId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblCostCenters")

							Case "PayGroupId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblPayGroup")

							Case "LocationId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblLocations")

							Case "DivisionId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblDivision")

						End Select
					Loop
					rs.Close()
					'-----------------------------------------------------------------------------------------------------------


					'Build Colume name and Colume data for the employee record
					TblColName = ""
					TblColData += "("


					'Build Colume name and Colume data for the tax record
					TblTaxCol = ""
					TblTaxValue += "("


					'===============================================================================================================
					vSQL = "select TblName, TblColName, SourceCol, Remarks from tblExcelImportProperties " _
						& "where Active=0 order by SourceCol"
					cm.CommandText = vSQL

					rs = cm.ExecuteReader
					Do While rs.Read

						' Use to add item on the employee tax table
						If rs("TblName") = "tblEmployeeTaxRef" Then
							TblTaxCol += rs("TblColName") & ","
							TblTaxValue += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
						End If

						' Use to collect fields to add new item
						If rs("TblName") = "tblEmployeesUpdate" Then
							TblColName += rs("TblColName") & ","

							If rs("Remarks") = "GetRef" Then
								Select Case rs("TblColName")
									Case "RegionId"
										vSQL = "Select id From tblRegions where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
									Case "BankAccountId"
										vSQL = "Select id From tblBanks where SysCode='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
									Case "LocationId"
										vSQL = "Select id From tblLocations where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
									Case "PositionId"
										vSQL = "Select id From tblPositions where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
									Case "CostCenterId"
										vSQL = "Select id From tblCostCenters where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
									Case "PayGroupId"
										vSQL = "Select id From tblPayGroup where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
									Case = "GenderId"
										vSQL = "Select id From tblGenders where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
									Case "DivisionId"
										vSQL = "Select id From tblDivision where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								End Select


								'Response.Write(vSQL & "<br><br>")
							End If

							If rs("Remarks") = "GetRef" Then
								TblColData += "'" & GetRef(vSQL, 0) & "',"
							Else
								TblColData += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
							End If
							vSQL = ""

						End If

					Loop
					rs.Close()




					'===============================================================================================================
					TblColData = TblColData.Substring(0, TblColData.Length - 1) & "),"

					'Response.Write(TblColName & " test<br><br>")

					TblTaxValue = TblTaxValue.Substring(0, TblTaxValue.Length - 1) & "),"
					'TblTaxValue += "),"
				Else
					'Response.Write(xlWorkSheet.Cells(i, 39).value & "--<br>")


					'TinNo = CStr(xlWorkSheet.Cells(i, 39).value.ToString.Trim)

					vSQL = "insert into tblEmployeesException (EmployeeCode, FirstName, LastName, TINNo, EmailAddress, BankAccountNo) values (" _
						& "'" & xlWorkSheet.Cells(i, 2).value & "'," _
						& "'" & xlWorkSheet.Cells(i, 3).value & "'," _
						& "'" & xlWorkSheet.Cells(i, 4).value.ToString & "'," _
						& "'" & TinNo & "'," _
						& "'" & xlWorkSheet.Cells(i, 44).value & "'," _
						& "'" & xlWorkSheet.Cells(i, 59).value & "')"
					'Response.Write(vSQL & "<br>")
					CreateRecords(vSQL)
				End If
			End If
		Next

		If EmpList <> "" And TblColName <> "" Then

			TblColName = TblColName.Substring(0, TblColName.Length - 1)
			TblColData = TblColData.Substring(0, TblColData.Length - 1)

			'vSQL = "delete from " & TblName & " where EmployeeCode in (" & EmpList.Substring(0, EmpList.Length - 1) & ")"
			'CreateRecords(vSQL)

			vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
			CreateRecords(vSQL)

			TblTaxCol = TblTaxCol.Substring(0, TblTaxCol.Length - 1)
			TblTaxValue = TblTaxValue.Substring(0, TblTaxValue.Length - 1)


			vSQL = "delete from tblEmployeeTaxRef where EmployeeCode in (" & EmpList.Substring(0, EmpList.Length - 1) & ")"
			CreateRecords(vSQL)

			vSQL = "insert into tblEmployeeTaxRef (" & TblTaxCol & ") values " & TblTaxValue
			CreateRecords(vSQL)
			'Response.Write(vSQL)

			vSQL = "update tblEmployees set DateSeparated=null where DateSeparated='1900-01-01 00:00:00.000'"
			CreateRecords(vSQL)
		End If

		c.Close()
		c.Dispose()
		cm.Dispose()

		xlWorkBook.Close()
		xlApp.Quit()

		releaseObject(xlApp)
		releaseObject(xlWorkBook)
		releaseObject(xlWorkSheet)

		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
	End Sub

	Private Sub ReadExcelUpdateData(FilePath As String, TblName As String)

		Dim xlApp As Excel.Application
		Dim xlWorkBook As Excel.Workbook
		Dim xlWorkSheet As Excel.Worksheet

		Dim IsEmpty As String = ""
		Dim TblColName As String = ""
		Dim TblVatColName As String = ""
		Dim TblColData As String = ""
		Dim TblTaxCol As String = ""
		Dim TblTaxValue As String = ""
		Dim EmpList As String = ""
		Dim TempVal As String = ""

		xlApp = New Excel.ApplicationClass
		xlWorkBook = xlApp.Workbooks.Open(FilePath)
		Try
			xlWorkSheet = xlWorkBook.Worksheets("Sheet1")
		Catch ex As Exception
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Sheet1 cannot be found in the uploaded file. \n\nPlease change the sheet name to Sheet1 then re-upload.');", True)
			Exit Sub
		End Try

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

		'Response.Write("ok")
		'Exit Sub


		Dim CntEmpcode As Integer = 0

		For i As Integer = 2 To 1000
			'Response.Write(i & "<br>")
			Try
				IsEmpty = xlWorkSheet.Cells(i, 1).value

				If IsEmpty = "" Then
					Exit For
				End If

				''===============================================================================================================

				If xlWorkSheet.Cells(i, 1).value = "Update" Then

					vSQL = "select TblColName, SourceCol from tblExcelImportProperties " _
							& "where TblName='tblEmployeesUpdate' and Remarks='GetRef' and Active=0 order by SourceCol"
					cm.CommandText = vSQL

					rs = cm.ExecuteReader
					Do While rs.Read

						Select Case rs("TblColName")
							Case "PositionId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblPositions")

							Case "CostCenterId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblCostCenters")

							Case "PayGroupId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblPayGroup")

							Case "LocationId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblLocations")

							Case "DivisionId"
								BuildRef(xlWorkSheet.Cells(i, rs("SourceCol")).value, "tblDivision")

						End Select
					Loop
					rs.Close()
					vSQL = ""












					vSQL = "select TblName, TblColName, SourceCol, Remarks from tblExcelImportProperties " _
						& "where TblName in ('tblEmployeesUpdate','tblEmployeeTaxRef') and Active=0 order by SourceCol"
					'Response.Write(vSQL)

					cm.CommandText = vSQL

					rs = cm.ExecuteReader
					Do While rs.Read
						TempVal = xlWorkSheet.Cells(i, rs("SourceCol")).value


						If TempVal <> "" And rs("Remarks") = "" Then

							Select Case rs("TblColName")
								Case "TAXPercent", "IsNonVAT", "VATPercent"

									If Not String.IsNullOrEmpty(xlWorkSheet.Cells(i, rs("SourceCol")).value.ToString.Trim) Then
										TblVatColName += rs("TblColName") & "='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
									End If

								Case "Active"
									If xlWorkSheet.Cells(i, rs("SourceCol")).value = "TRUE" Then
										TblColName += rs("TblColName") & "=1,"
									Else
										TblColName += rs("TblColName") & "=0,"
									End If

								Case "DateSeparated"
									If IsDate(CDate(xlWorkSheet.Cells(i, rs("SourceCol")).value)) Then
										TblColName += rs("TblColName") & "='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
									Else
										TblColName += rs("TblColName") & "=null,"
									End If

								Case Else
									If rs("TblName") = "tblEmployeesUpdate" Then
										TblColName += rs("TblColName") & "='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
									End If
							End Select
						End If

						'If rs("TblName") = "tblEmployeesUpdate" Then
						If TempVal <> "" And rs("Remarks") = "GetRef" Then
							Select Case rs("TblColName")
								Case "RegionId"
									vSQL = "Select id From tblRegions where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								Case "BankAccountId"
									vSQL = "Select id From tblBanks where SysCode='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								Case "LocationId"
									vSQL = "Select id From tblLocations where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								Case "PositionId"
									vSQL = "Select id From tblPositions where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								Case "CostCenterId"
									vSQL = "Select id From tblCostCenters where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								Case "PayGroupId"
									vSQL = "Select id From tblPayGroup where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								Case = "GenderId"
									vSQL = "Select id From tblGenders where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
								Case "DivisionId"
									vSQL = "Select id From tblDivision where Code='" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "'"
							End Select

							TblColName += rs("TblColName") & "='" & GetRef(vSQL, 0) & "',"

						End If

					Loop
					rs.Close()


					If TblColName <> "" Then
						TblColName = TblColName.Substring(0, TblColName.Length - 1)
						vSQL = "update tblEmployees set " & TblColName
						vSQL += " where EmployeeCode='" & xlWorkSheet.Cells(i, 2).value & "'"
						'CreateRecords(vSQL)
						Response.Write(vSQL & "<br><br>")
					End If

					If TblVatColName <> "" Then
						TblVatColName = TblVatColName.Substring(0, TblVatColName.Length - 1)
						vSQL = "update tblEmployeeTaxRef set " & TblVatColName
						vSQL += " where EmployeeCode='" & xlWorkSheet.Cells(i, 2).value & "'"
						'CreateRecords(vSQL)
						'Response.Write(vSQL & "<br><br>")
					End If

					vSQL = ""
					TblColName = ""
					TblVatColName = ""
				End If

			Catch ex As Exception
				Response.Write("Error: Excel Line No " & i & ex.Message.Replace(",", "").Replace(";", "") & "')<br>")
				Exit For
			End Try





		Next



		'If EmpList <> "" And TblColName <> "" Then

		'	TblColName = TblColName.Substring(0, TblColName.Length - 1)
		'	TblColData = TblColData.Substring(0, TblColData.Length - 1)

		'	'vSQL = "delete from " & TblName & " where EmployeeCode in (" & EmpList.Substring(0, EmpList.Length - 1) & ")"
		'	'CreateRecords(vSQL)

		'	vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
		'	CreateRecords(vSQL)

		'	TblTaxCol = TblTaxCol.Substring(0, TblTaxCol.Length - 1)
		'	TblTaxValue = TblTaxValue.Substring(0, TblTaxValue.Length - 1)


		'	vSQL = "delete from tblEmployeeTaxRef where EmployeeCode in (" & EmpList.Substring(0, EmpList.Length - 1) & ")"
		'	CreateRecords(vSQL)

		'	vSQL = "insert into tblEmployeeTaxRef (" & TblTaxCol & ") values " & TblTaxValue
		'	CreateRecords(vSQL)
		'	'Response.Write(vSQL)

		'	vSQL = "update tblEmployees set DateSeparated=null where DateSeparated='1900-01-01 00:00:00.000'"
		'	CreateRecords(vSQL)
		'End If

		c.Close()
		c.Dispose()
		cm.Dispose()

		xlWorkBook.Close()
		xlApp.Quit()

		releaseObject(xlApp)
		releaseObject(xlWorkBook)
		releaseObject(xlWorkSheet)
		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
	End Sub

	Private Sub BuildRef(ParamCode As String, TblName As String)

		Dim Count As Integer = 0
		Count = GetRef("select count(Code) from " & TblName & " where Code='" & ParamCode & "'", 0)

		If Count = 0 Then
			vSQL = "insert into " & TblName & " values ('" & ParamCode & "','" & ParamCode & "',0,0,0)"
			CreateRecords(vSQL)

		End If
	End Sub

	Private Sub tblEmployees_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblEmployees.SelectedIndexChanged
		BtnUpdate.Disabled = False
		BtnSendEmail.Enabled = True
		Session("TranID") = tblEmployees.SelectedRow.Cells(1).Text

		Dim DateResign As String = tblEmployees.SelectedRow.Cells(7).Text.ToString.Replace("&nbsp;", "")

		If DateResign <> "" Then
			TxtDateResign.Text = Format(CDate(DateResign), "MM/dd/yyyy")
		End If

		If tblEmployees.SelectedRow.Cells(6).Text = "True" Then
			CmdEditStatus.SelectedValue = "Active"
		Else
			CmdEditStatus.SelectedValue = "In-Active"
		End If
	End Sub

	Private Sub BtnSubmitUpdate_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitUpdate.ServerClick

		Dim ResignDate As Date

		If CmdEditStatus.SelectedValue = "Active" Then
			vSQL = "update tblemployees set Active=1 where EmployeeCode='" & tblEmployees.SelectedRow.Cells(1).Text & "'"
		Else
			vSQL = "update tblemployees set Active=0 where EmployeeCode='" & tblEmployees.SelectedRow.Cells(1).Text & "'"
		End If
		CreateRecords(vSQL)

		If TxtDateResign.Text.Trim <> "" Then
			Try
				ResignDate = CDate(TxtDateResign.Text)
				vSQL = "update tblemployees set " _
					& "DateSeparated='" & ResignDate & "',Active=0 " _
					& "where EmployeeCode='" & tblEmployees.SelectedRow.Cells(1).Text & "'"
				CreateRecords(vSQL)

			Catch ex As Exception
				ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid date Resign.');", True)
			End Try
		Else
			vSQL = "update tblemployees set " _
				& "DateSeparated=null " _
				& "where EmployeeCode='" & tblEmployees.SelectedRow.Cells(1).Text & "'"
			CreateRecords(vSQL)
		End If
		TxtDateResign.Text = ""
		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
	End Sub

	Private Sub BtnExport_Click(sender As Object, e As EventArgs) Handles BtnExport.Click
		Dim TargetPath As String = ""
		TargetPath = Server.MapPath(".") & "\Downloads\Report\" '& Format(Now(), "MMddyyyyHHmmss") & "-PayRegister" & TxtFileName.FileName

		Dim xlApp As Excel.Application = New Excel.Application()

		If xlApp Is Nothing Then
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Excel is not properly installed!!');", True)
			Return
		End If

		Dim xlWorkBook As Excel.Workbook
		Dim xlWorkSheet As Excel.Worksheet
		Dim misValue As Object = System.Reflection.Missing.Value

		xlWorkBook = xlApp.Workbooks.Add(misValue)
		xlWorkSheet = xlWorkBook.Sheets("sheet1")

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim Ctr As Integer = 1
		Dim FileName As String = ""
		Dim vFilter As String = ""

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try

		If TxtKeywords.Text.Trim <> "" Then
			vFilter += "and (EmployeeCode like '%" & TxtKeywords.Text.Trim & "%' or " _
				& "LastName like '%" & TxtKeywords.Text.Trim & "%' or " _
				& "FirstName like '%" & TxtKeywords.Text.Trim & "%') "
		End If

		If CmdResign.SelectedValue = "Resigned" Then
			vFilter += "and DateSeparated is not null "
		Else
			vFilter += "and DateSeparated is null "
		End If

		If CmdStatus.SelectedValue = "In-Active" Then
			vFilter += "and Active=0 "
		Else
			vFilter += "and Active=1 "
		End If

		xlWorkSheet.Cells(Ctr, 1) = "Emp Code"
		xlWorkSheet.Cells(Ctr, 2) = "Full Name"
		xlWorkSheet.Cells(Ctr, 3) = "Registered Address"
		xlWorkSheet.Cells(Ctr, 4) = "Birth Date"
		xlWorkSheet.Cells(Ctr, 5) = "Date Hired"
		xlWorkSheet.Cells(Ctr, 6) = "Active"
		xlWorkSheet.Cells(Ctr, 7) = "Date Separated"
		xlWorkSheet.Cells(Ctr, 8) = "Tax Code"
		xlWorkSheet.Cells(Ctr, 9) = "Vat"
		xlWorkSheet.Cells(Ctr, 10) = "Vat Percent"
		xlWorkSheet.Cells(Ctr, 11) = "Basic Allowance"
		xlWorkSheet.Cells(Ctr, 12) = "Position"
		xlWorkSheet.Cells(Ctr, 13) = "Gender"
		xlWorkSheet.Cells(Ctr, 14) = "TIN"
		xlWorkSheet.Cells(Ctr, 15) = "Email Address"
		xlWorkSheet.Cells(Ctr, 16) = "Bank"
		xlWorkSheet.Cells(Ctr, 17) = "Bank Acct No"
		xlWorkSheet.Cells(Ctr, 18) = "Cost Center"
		xlWorkSheet.Cells(Ctr, 19) = "Pay Group"
		xlWorkSheet.Cells(Ctr, 20) = "Division"
		xlWorkSheet.Cells(Ctr, 21) = "Group"
		xlWorkSheet.Cells(Ctr, 22) = "ACH Code"
		xlWorkSheet.Cells(Ctr, 23) = "Income Payments Subject to ExpandedWithholding Tax"

		vSQL = "select EmployeeCode, FullName, MiddleName, AddressRegistered, BirthDate, Active, " _
			& "DateHired, DateSeparated, TaxCodeId, RegionId, Monthlyrate, PositionId,  " _
			& "GenderId, TINNo, EmailAddress, BankAccountId, BankAccountTypeId, " _
			& "BankAccountNo, BankAccountNo, CostCenterId, PayGroupid, " _
			& "(select Name from tblPositions where Id=PositionId) as PosName, " _
			& "(select Name from tblLocations where Id=LocationId) as LocName, " _
			& "(select TaxPercent*100 from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as TaxCode, " _
			& "(select IsNonVat from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as Vat, " _
			& "(select  VatPercent*100 from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as VatPercent, " _
			& "(select Name from tblRegions where id=RegionId) as Region, " _
			& "(select SysCode from tblBanks where id=BankAccountId) as Bank, " _
			& "(select Name from tblCostCenters where id=CostCenterId) as CostCenter, " _
			& "(select Name from tblPayGroup where id=PayGroupId) as PayGroup, " _
			& "(select Name from tblGenders where id=GenderId) as Gender, " _
			& "(select Name from tblDivision where id=DivisionId) as Div, " _
			& "CustomField2, Remarks " _
			& "from tblEmployees b " _
			& "where TINNo<>'' and EmailAddress<>'' " & vFilter & " order by LastName"

		cm.CommandText = vSQL
		Ctr = 2
		rs = cm.ExecuteReader
		Do While rs.Read

			xlWorkSheet.Cells(Ctr, 1).NumberFormat = "@"
			xlWorkSheet.Cells(Ctr, 3).NumberFormat = "@"
			xlWorkSheet.Cells(Ctr, 14).NumberFormat = "@"
			xlWorkSheet.Cells(Ctr, 17).NumberFormat = "@"
			xlWorkSheet.Cells(Ctr, 18).NumberFormat = "@"


			xlWorkSheet.Cells(Ctr, 1) = rs("EmployeeCode")
			xlWorkSheet.Cells(Ctr, 2) = rs("FullName")
			xlWorkSheet.Cells(Ctr, 3) = rs("AddressRegistered")
			xlWorkSheet.Cells(Ctr, 4) = rs("BirthDate")
			xlWorkSheet.Cells(Ctr, 5) = rs("DateHired")
			xlWorkSheet.Cells(Ctr, 6) = rs("Active")
			xlWorkSheet.Cells(Ctr, 7) = rs("DateSeparated")
			xlWorkSheet.Cells(Ctr, 8) = rs("TaxCode")
			xlWorkSheet.Cells(Ctr, 9) = rs("Vat")
			xlWorkSheet.Cells(Ctr, 10) = rs("VatPercent")
			xlWorkSheet.Cells(Ctr, 11) = rs("Monthlyrate")
			xlWorkSheet.Cells(Ctr, 12) = rs("PosName")
			xlWorkSheet.Cells(Ctr, 13) = rs("Gender")
			xlWorkSheet.Cells(Ctr, 14) = rs("TINNo")
			xlWorkSheet.Cells(Ctr, 15) = rs("EmailAddress")
			xlWorkSheet.Cells(Ctr, 16) = rs("Bank")
			xlWorkSheet.Cells(Ctr, 17) = rs("BankAccountNo")
			xlWorkSheet.Cells(Ctr, 18) = rs("CostCenter")
			xlWorkSheet.Cells(Ctr, 19) = rs("PayGroup")
			xlWorkSheet.Cells(Ctr, 20) = rs("Div")
			xlWorkSheet.Cells(Ctr, 21) = rs("LocName")
			xlWorkSheet.Cells(Ctr, 22) = rs("CustomField2")
			xlWorkSheet.Cells(Ctr, 23) = rs("Remarks")
			Ctr += 1
		Loop

		rs.Close()


		c.Close()
		c.Dispose()
		cm.Dispose()

		FileName = Format(Now(), "MMddyyyyHHmmss") & "-MasterData.xls"

		xlWorkBook.SaveAs(TargetPath & FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
		 Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue)
		xlWorkBook.Close(True, misValue, misValue)
		xlApp.Quit()

		releaseObject(xlWorkSheet)
		releaseObject(xlWorkBook)
		releaseObject(xlApp)


		Response.Redirect("~/Downloads/Report/" & FileName)
	End Sub

	Private Sub ExceptionReports()

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

		vSQL = "select EmployeeCode, FirstName, LastName, " _
			& "(select EmployeeCode from tblemployees a where a.EmployeeCode=b.EmployeeCode) As MEmpCode, " _
			& "(select FirstName from tblemployees a where a.EmployeeCode=b.EmployeeCode) As MFName, " _
			& "(Select LastName from tblemployees a where a.EmployeeCode=b.EmployeeCode) As MLName " _
			& "from tblEmployeesException b"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read
			DuplicateEmpCode += "<tr><td>" & rs("MEmpCode") & "</td><td>" & rs("MFName") & "</td>"
			DuplicateEmpCode += "<td>" & rs("MLName") & "</td><td>" & rs("EmployeeCode") & "</td>"
			DuplicateEmpCode += "<td>" & rs("FirstName") & "</td><td>" & rs("LastName") & "</td></tr>"
		Loop
		If DuplicateEmpCode = "" Then
			DuplicateEmpCode = "<tr><td colspan='6'>No records found</td></tr>"
			BtnClearDuplicateEmpCode.Disabled = True
		Else
			BtnClearDuplicateEmpCode.Disabled = False
		End If
		rs.Close()

		'-------------------------------------------------------------------------------------------------------------------------

		vSQL = "select EmployeeCode, FullName, BankAccountNo," _
			& "(select count(BankAccountNo) from tblemployees a where a.BankAccountNo=b.BankAccountNo) as DataCount " _
			& "from tblemployees b"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read
			If rs("DataCount") > 1 Then
				DuplicateBankAcct += "<tr><td>" & rs("EmployeeCode") & "</td><td>" & rs("FullName") & "</td><td>" & rs("BankAccountNo") & "</td></tr>"
			End If
		Loop
		If DuplicateBankAcct = "" Then
			DuplicateBankAcct = "<tr><td colspan='3'>No records found</td></tr>"
		End If
		rs.Close()

		'-------------------------------------------------------------------------------------------------------------------------

		vSQL = "select EmployeeCode, FullName, TINNo," _
			& "(select count(TINNo) from tblemployees a where a.TINNo=b.TINNo) as DataCount " _
			& "from tblemployees b"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read
			If rs("DataCount") > 1 Then
				DuplicateTIN += "<tr><td>" & rs("EmployeeCode") & "</td><td>" & rs("FullName") & "</td><td>" & rs("TINNo") & "</td></tr>"
			End If
		Loop
		If DuplicateTIN = "" Then
			DuplicateTIN = "<tr><td colspan='3'>No records found</td></tr>"
		End If
		rs.Close()

		'-------------------------------------------------------------------------------------------------------------------------

		vSQL = "select EmployeeCode, FullName, EmailAddress," _
			& "(select count(EmailAddress) from tblemployees a where a.EmailAddress=b.EmailAddress) as DataCount " _
			& "from tblemployees b"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read
			If rs("DataCount") > 1 Then
				DuplicateEmail += "<tr><td>" & rs("EmployeeCode") & "</td><td>" & rs("FullName") & "</td><td>" & rs("EmailAddress") & "</td></tr>"
			End If
		Loop
		If DuplicateEmail = "" Then
			DuplicateEmail = "<tr><td colspan='3'>No records found</td></tr>"
		End If
		rs.Close()

		c.Close()
		c.Dispose()
		cm.Dispose()

	End Sub

	Private Sub BtnClearDuplicateEmpCode_ServerClick(sender As Object, e As EventArgs) Handles BtnClearDuplicateEmpCode.ServerClick
		vSQL = "delete from tblEmployeesException"
		CreateRecords(vSQL)

		GetEmployeeList()
		ExceptionReports()
		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
	End Sub

	Private Sub BtnSendEmail_Click(sender As Object, e As EventArgs) Handles BtnSendEmail.Click

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader

		Dim PassValue As String = GenerateRandomString(12)
		Dim Password As String = PassValue

		PassValue = getEncryptedCode256("BP0I@202OQ" & PassValue)
		vSQL = "update tblEmployees set EmployeePassword='" & PassValue & "' where EmployeeCode='" & tblEmployees.SelectedRow.Cells(1).Text & "'"
		CreateRecords(vSQL)

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try



		vSQL = "select EmailAddress, FullName, FirstName, EmployeeCode from tblEmployees where EmployeeCode='" & tblEmployees.SelectedRow.Cells(1).Text & "'"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		If rs.Read Then

			If Not IsDBNull(rs("EmailAddress")) Then


				SendEmail(rs("EmployeeCode"), rs("EmailAddress"), rs("FirstName"), Password)
			End If

		End If
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

	Public Function getEncryptedCode256(ByVal inputString As String) As String

		Dim Hash As Byte() = New System.Security.Cryptography.SHA256Managed().ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(inputString))
		Dim outputString As New System.Text.StringBuilder()

		For i As Integer = 0 To Hash.Length - 1
			outputString.Append(Hash(i).ToString("X2"))
		Next

		Return outputString.ToString()

	End Function
	Private Sub SendEmail(EmployeeCode As String, Emailadd As String, FName As String, PasswordVal As String)
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
			e_mail.To.Add(Emailadd)

			Tbl = "<html><body style='font-size:14px'><head><style>" _
						& "td {border:1px solid #F2F3F4; padding:8px} " _
						& ".lbl {color:#007BFF; font-weight:bold; font-size:14px} " _
						& ".lbl2 {color:#000; font-size:14px} " _
						& ".lbl3 {color:#000; font-size:14px} " _
						& ".lbl4 {color:#7b7b7b; font-size:14px} " _
						& ".lbl5 {color:#000; font-size:14px; width:100%; padding-buttom: 20px; border: solid 0px #fff;height: 200px } " _
						& "</style></head>"

			Tbl += "<Label Class='lbl3'>"



			DueDate = GetRef(vSQL, "")

			Tbl += "Hi " & FName & ",<br /><br />"

			'Tbl += "We received a request to reset your password. Enter the following password below:<br /><br />"
			Tbl += "Your online portal account is activated. You may now access your payslip online.<br /><br />"

			Tbl += "<label class='lbl3'>Click this link to access the Payment Slip Portal: &nbsp;<a href='https://ess-apps.bposerve.com/ALSI/'>https://ess-apps.bposerve.com/ALSI/</a></label><br><br>"

			Tbl += "Enter the username and password credentials below. Please change your password upon log in.<br /><br />"

			Tbl += "User Account Code: <b>" & EmployeeCode & "</b><br /><br />"

			Tbl += "Password: <b>" & PasswordVal & "</b><br /><br />"

			Tbl += "Thank you.<br /><br />"

			Tbl += "Notice: This is a system-generated email. Do not reply.<br /><br />"

			Tbl += "</div></body></html>"


			e_mail.Subject = "New Account Notification"
			e_mail.IsBodyHtml = True
			e_mail.Body = Tbl


			Smtp_Server.Send(e_mail)
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Message Sent Successfully.');", True)
			'Response.Redirect("~/", True)
		Catch ex As Exception
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Sending error: " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');", True)
		End Try
	End Sub
End Class

