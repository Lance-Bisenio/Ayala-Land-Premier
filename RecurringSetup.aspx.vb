Imports System.Data
Imports HelperClass
Imports Microsoft.Office.Interop
Partial Class Department
    Inherits System.Web.UI.Page
	Dim vSQL As String = ""

	Private Sub DocumentType_Load(sender As Object, e As EventArgs) Handles Me.Load
		If Session("uid") = "" Then
			Response.Redirect("~/Login")
			Exit Sub
		End If

		If Not IsPostBack Then
			Dim CanViewApp As Integer = 0
			vSQL = "select Count(User_Id) as Ctr from tblRightsList where Property_Value='4000' and User_Id='" & Session("uid") & "'"

			CanViewApp = GetRef(vSQL, "")

			If CanViewApp = 0 Then
				'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('You are not authorized to view this page.');", True)
				Response.Redirect("~/AccessDenied")
			End If


			BuildCombo("select Code, Name from tblPayElements where IsRecurring=1 and Code not in ('BASIC','VAT') order by Name ", CmdEarningsList)

			CmdEarningsStatus.Items.Add("Active")
			CmdEarningsStatus.Items.Add("In-Active")

			BuildCombo("select EmployeeCode, FullName from tblEmployees order by FullName", CmdEmployeeList)
			CmdEmployeeList.Items.Add("Select employee")
			CmdEmployeeList.SelectedValue = "Select employee"

            BtnAdd.Disabled = True
            BtnEdit.Disabled = True
			BtnDelete.Disabled = True


		End If
	End Sub

	Private Sub GetDocumentType()
		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""
		Dim vTableName As String = ""
		Dim vSQL As String = ""

		c.ConnectionString = ConnStr
        vSQL = "select id, PayElementId, " _
            & "(select Name from tblPayElements where Code=PayElementId) As Descr, " _
            & "Amount, format(ValidFrom,'MM/dd/yyyy') as ValidFrom, Format(ValidTo,'MM/dd/yyyy') as ValidTo, " _
            & "IsActive, CreatedBy, format(DateCreated,'MM/dd/yyyy') as DateCreated " _
            & "from tblPayInstructionRecurring " _
            & "where EmpCode='" & CmdEmployeeList.SelectedValue & "' " _
            & "order by PayElementId "
        'Response.Write(vSQL)

        da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "tblDocumentType")
        TblRecurringList.DataSource = ds.Tables("tblDocumentType")
        TblRecurringList.DataBind()

        da.Dispose()
		ds.Dispose()
	End Sub

    Protected Sub tblDocType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TblRecurringList.SelectedIndexChanged

        Try
            CmdEarningsList.SelectedValue = TblRecurringList.SelectedRow.Cells(2).Text
        Catch ex As Exception

        End Try


        If TblRecurringList.SelectedRow.Cells(5).Text.ToString.Replace("&nbsp;", "") <> "" Then
            TxtValidFrom.Text = Format(CDate(TblRecurringList.SelectedRow.Cells(5).Text), "MM/dd/yyyy")
        End If

        If TblRecurringList.SelectedRow.Cells(6).Text.ToString.Replace("&nbsp;", "") <> "" Then
            TxtValidTo.Text = Format(CDate(TblRecurringList.SelectedRow.Cells(6).Text), "MM/dd/yyyy")
        End If

        TxtAmount.Text = Format(CDec(TblRecurringList.SelectedRow.Cells(4).Text), "#,###,##0.00")

        If TblRecurringList.SelectedRow.Cells(7).Text = 1 Then
            CmdEarningsStatus.SelectedValue = "Active"
        Else
            CmdEarningsStatus.SelectedValue = "In-Active"
        End If

        BtnAdd.Disabled = False
        BtnEdit.Disabled = False
        BtnDelete.Disabled = False
        BtnSubmitSave.Value = "Save"
    End Sub

    'Private Sub tblDocumentType_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblDocumentType.PageIndexChanging
    '	tblDocumentType.PageIndex = e.NewPageIndex
    '	tblDocumentType.SelectedIndex = -1
    '	GetDocumentType()
    '	txtCode.Value = ""
    '	txtDescr.Value = ""
    'End Sub

    'Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
    '	tblDocumentType.SelectedIndex = -1
    '	GetDocumentType()
    '	txtCode.Value = ""
    '	txtDescr.Value = ""
    'End Sub

    'Private Sub BtnSave_ServerClick(sender As Object, e As EventArgs) Handles BtnSave.ServerClick
    '	If txtAddDescr.Value.Trim <> "" And txtAddCode.Value.Trim <> "" Then
    '		vSQL = "insert into emp_department_ref values ('" & txtAddDescr.Value.Trim & "', '" & txtAddCode.Value.Trim & "')"
    '		CreateRecords(vSQL)
    '		tblDocumentType.SelectedIndex = -1
    '		GetDocumentType()
    '		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Saved successfully');", True)
    '	Else
    '		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Failed to complete. \n\nPlease enter document type description.');", True)
    '	End If
    'End Sub

    'Private Sub BtnUpdate_ServerClick(sender As Object, e As EventArgs) Handles BtnUpdate.ServerClick
    '	If txtDescr.Value.Trim <> "" Then
    '		vSQL = "update emp_department_ref set Descr='" & txtDescr.Value & "', " _
    '			& "DeptCd='" & txtCode.Value & "' " _
    '			& "where Dept_Id=" & tblDocumentType.SelectedRow.Cells(1).Text
    '		CreateRecords(vSQL)
    '		tblDocumentType.SelectedIndex = -1
    '		GetDocumentType()

    '		txtCode.Value = ""
    '		txtDescr.Value = ""
    '		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The selected item has been updated');", True)
    '	Else
    '		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Failed to complete. \n\nPlease enter document type description.');", True)
    '	End If
    'End Sub

    'Private Sub BtnDelAction_ServerClick(sender As Object, e As EventArgs) Handles BtnDelAction.ServerClick
    '	vSQL = "delete from emp_department_ref where Dept_Id=" & tblDocumentType.SelectedRow.Cells(1).Text
    '	CreateRecords(vSQL)
    '	tblDocumentType.SelectedIndex = -1
    '	GetDocumentType()
    '	txtCode.Value = ""
    '	txtDescr.Value = ""
    '	ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The selected item has been deleted');", True)
    'End Sub

    Private Sub CmdEmployeeList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmdEmployeeList.SelectedIndexChanged
        GetDocumentType()
        BtnAdd.Disabled = False
        BtnEdit.Disabled = True
        BtnDelete.Disabled = True
        TblRecurringList.SelectedIndex = -1
    End Sub

    Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
        GetDocumentType()
        TblRecurringList.SelectedIndex = -1
        BtnEdit.Disabled = True
        BtnDelete.Disabled = True
    End Sub

    Private Sub BtnAdd_ServerClick(sender As Object, e As EventArgs) Handles BtnAdd.ServerClick
        BtnSubmitSave.Value = "Submit"
        TblRecurringList.SelectedIndex = -1
        BtnEdit.Disabled = True
        BtnDelete.Disabled = True
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "OperModal();", True)
    End Sub

    Private Sub BtnSubmitSave_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitSave.ServerClick
        Dim ValidDate As String = ""

        If TxtValidFrom.Text.Trim <> "" And TxtValidTo.Text.Trim = "" Or TxtValidFrom.Text.Trim = "" And TxtValidTo.Text.Trim <> "" Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Please complete the validity date From and To'); OperModal();", True)
            Exit Sub
        End If

        If TxtValidFrom.Text.Trim <> "" Then
            Try
                ValidDate = CDate(TxtValidFrom.Text.Trim)
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid Date From'); OperModal();", True)
                Exit Sub
            End Try
        End If

        If TxtValidTo.Text.Trim <> "" Then
            Try
                ValidDate = CDate(TxtValidTo.Text.Trim)
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid Date To'); OperModal();", True)
                Exit Sub
            End Try
        End If

        If TxtAmount.Text.Trim = "" Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid amount'); OperModal();", True)
            Exit Sub
        End If

        If BtnSubmitSave.Value = "Submit" Then
            vSQL = "insert into tblPayInstructionRecurring (EmpCode, IsEarnings, PayElementId, Amount, ValidFrom, ValidTo, IsActive, CreatedBy, DateCreated) values (" _
                & "'" & CmdEmployeeList.SelectedValue & "'," _
                & "'1'," _
                & "'" & CmdEarningsList.SelectedValue & "'," _
                & "'" & CDec(TxtAmount.Text.Trim) & "',"

            If TxtValidFrom.Text.Trim <> "" Then
                vSQL += "'" & Format(CDate(TxtValidFrom.Text.Trim), "MM/dd/yyyy") & "',"
            Else
                vSQL += "null,"
            End If

            If TxtValidTo.Text.Trim <> "" Then
                vSQL += "'" & Format(CDate(TxtValidTo.Text.Trim), "MM/dd/yyyy") & "',"
            Else
                vSQL += "null,"
            End If

            vSQL += "'" & IIf(CmdEarningsStatus.SelectedValue = "Active", 1, 0) & "'," _
                & "'" & Session("uid") & "'," _
                & "'" & Now & "')"

            CreateRecords(vSQL)
        Else
            vSQL = "update tblPayInstructionRecurring set " _
                & "PayElementId='" & CmdEarningsList.SelectedValue & "'," _
                & "Amount='" & CDec(TxtAmount.Text.Trim) & "',"

            If TxtValidFrom.Text.Trim <> "" Then
                vSQL += "ValidFrom='" & Format(CDate(TxtValidFrom.Text.Trim), "MM/dd/yyyy") & "',"
            Else
                vSQL += "ValidFrom=null,"
            End If

            If TxtValidTo.Text.Trim <> "" Then
                vSQL += "ValidTo='" & Format(CDate(TxtValidTo.Text.Trim), "MM/dd/yyyy") & "',"
            Else
                vSQL += "ValidTo=null,"
            End If

            vSQL += "IsActive='" & IIf(CmdEarningsStatus.SelectedValue = "Active", 1, 0) & "'," _
                & "CreatedBy='" & Session("uid") & "'," _
                & "DateCreated='" & Now & "' where id=" & TblRecurringList.SelectedRow.Cells(1).Text
            CreateRecords(vSQL) 
        End If

        GetDocumentType()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)

    End Sub

    Private Sub BtnSubmitDelete_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitDelete.ServerClick

        vSQL = "delete from tblPayInstructionRecurring where id=" & TblRecurringList.SelectedRow.Cells(1).Text
        CreateRecords(vSQL)
        GetDocumentType()
        TblRecurringList.SelectedIndex = -1
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully deleted');", True)
    End Sub

    Private Sub BtnExport_ServerClick(sender As Object, e As EventArgs) Handles BtnExport.ServerClick
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

        xlWorkSheet.Cells(Ctr, 1) = "Emp Code"
        xlWorkSheet.Cells(Ctr, 2) = "Full Name"
        xlWorkSheet.Cells(Ctr, 3) = "Pay Element"
        xlWorkSheet.Cells(Ctr, 4) = "Description"
        xlWorkSheet.Cells(Ctr, 5) = "IsEarnings"
        xlWorkSheet.Cells(Ctr, 6) = "Amount"
        xlWorkSheet.Cells(Ctr, 7) = "ValidFrom"
        xlWorkSheet.Cells(Ctr, 8) = "ValidTo"
        xlWorkSheet.Cells(Ctr, 9) = "IsActive"
        xlWorkSheet.Cells(Ctr, 10) = "CreatedBy"
        xlWorkSheet.Cells(Ctr, 11) = "DateCreated"

        vSQL = "select EmpCode, IsEarnings, PayElementId, Amount, ValidFrom, ValidTo, IsActive, CreatedBy, DateCreated, " _
            & "(select Name from tblPayElements where PayElementId=Code) as Descr, " _
            & "(select FullName from tblEmployees where EmpCode=EmployeeCode) as EmpName " _
            & "from tblPayInstructionRecurring " _
            & "where EmpCode in (select EmployeeCode from tblEmployees where DateSeparated is null) " _
            & "Order by EmpName, Descr, DateCreated"

        cm.CommandText = vSQL
        Ctr = 2
        rs = cm.ExecuteReader
        Do While rs.Read

            'xlWorkSheet.Cells(Ctr, 1).NumberFormat = "@"
            'xlWorkSheet.Cells(Ctr, 3).NumberFormat = "@"
            'xlWorkSheet.Cells(Ctr, 14).NumberFormat = "@"
            'xlWorkSheet.Cells(Ctr, 17).NumberFormat = "@"
            'xlWorkSheet.Cells(Ctr, 18).NumberFormat = "@"

            xlWorkSheet.Cells(Ctr, 6).NumberFormat = "#,###,##0.00"

            xlWorkSheet.Cells(Ctr, 1) = rs("EmpCode")
            xlWorkSheet.Cells(Ctr, 2) = rs("EmpName")
            xlWorkSheet.Cells(Ctr, 3) = rs("PayElementId")
            xlWorkSheet.Cells(Ctr, 4) = rs("Descr")
            xlWorkSheet.Cells(Ctr, 5) = IIf(rs("IsEarnings") = 1, "Yes", "No")
            xlWorkSheet.Cells(Ctr, 6) = rs("Amount")

            If rs("ValidFrom").ToString.Trim <> "" Then
                xlWorkSheet.Cells(Ctr, 7) = Format(CDate(rs("ValidFrom")), "MM/dd/yyyy")
            Else
                xlWorkSheet.Cells(Ctr, 7) = ""
            End If

            If rs("ValidTo").ToString.Trim <> "" Then
                xlWorkSheet.Cells(Ctr, 8) = Format(CDate(rs("ValidTo")), "MM/dd/yyyy")
            Else
                xlWorkSheet.Cells(Ctr, 8) = ""
            End If

            xlWorkSheet.Cells(Ctr, 9) = IIf(rs("IsActive") = 1, "Yes", "No")
            xlWorkSheet.Cells(Ctr, 10) = rs("CreatedBy")
            xlWorkSheet.Cells(Ctr, 11) = rs("DateCreated")
            Ctr += 1
        Loop

        rs.Close()


        c.Close()
        c.Dispose()
        cm.Dispose()

        FileName = Format(Now(), "MMddyyyyHHmmss") & "-RecurringReport.xls"

        xlWorkBook.SaveAs(TargetPath & FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
         Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue)
        xlWorkBook.Close(True, misValue, misValue)
        xlApp.Quit()

        releaseObject(xlWorkSheet)
        releaseObject(xlWorkBook)
        releaseObject(xlApp)


        Response.Redirect("~/Downloads/Report/" & FileName)
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

    Private Sub tblPayrollRun_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles TblRecurringList.PageIndexChanging
        'TblRecurringList_PageIndexChanged(sender As Object, e As EventArgs) Handles TblRecurringList.PageIndexChanged
        TblRecurringList.PageIndex = e.NewPageIndex
        TblRecurringList.SelectedIndex = -1
        GetDocumentType()

    End Sub

    Private Sub BtnSubmitFileInstruction_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitFileInstruction.ServerClick
        Dim TargetFilenameRecurring As String
        Dim TargetFilenameOneTime As String
        Dim RecurringFileName As String = ""
        Dim OneTimeFileName As String = ""
        Dim BatchNo As Int64 = 0
        Dim LockCount As Integer = 0
        Dim TempDate As Date


        'Try
        '    TempDate = CDate(TxtCFrom.Text.Trim)
        'Catch ex As Exception
        '    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid date cutoff value. \nPlease enter correct and valid date format.'); $('#UploadFiles').modal();", True)
        '    Exit Sub
        'End Try

        'Try
        '    TempDate = CDate(TxtCTo.Text.Trim)
        'Catch ex As Exception
        '    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid date cutoff value.  \nPlease enter correct and valid date format.'); $('#UploadFiles').modal();", True)
        '    Exit Sub
        'End Try

        'Try
        '    TempDate = CDate(TxtTargetPaydate.Text.Trim)
        'Catch ex As Exception
        '    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid payout date value. \nPlease enter correct and valid date format.'); $('#UploadFiles').modal();", True)
        '    Exit Sub
        'End Try

        'vSQL = "select count(BatchNo) as Lock from tblPayInstructionHeader where DatePosted is not null and " _
        '    & "PayDate='" & Format(CDate(TxtTargetPaydate.Text), "MM/dd/yyyy") & "'"

        'LockCount = GetRef(vSQL, 0)
        'If LockCount > 0 Then
        '    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The selected target payout date release is already locked.')", True)
        '    Exit Sub
        'End If

        BatchNo = Format(Now(), "MMddyyyyHHmmss")

        If TxtFileName.FileName <> "" Then
            TargetFilenameRecurring = Server.MapPath(".") & "\Uploaded\SystemInputFiles\" & Format(Now(), "MMddyyyyHHmmss") & "-RecurringSetup-" & TxtFileName.FileName
            TxtFileName.SaveAs(TargetFilenameRecurring)
            ReadExcelData(TargetFilenameRecurring, "tblPayInstructionRecurring", BatchNo, "RECURRING")
            RecurringFileName = TxtFileName.FileName
        Else
            RecurringFileName = "None"
        End If

        'If TxtFileNameOneTime.FileName <> "" Then
        '    TargetFilenameOneTime = Server.MapPath(".") & "\Uploaded\SystemInputFiles\" & Format(Now(), "MMddyyyyHHmmss") & "-PAYInstructionOneTime-" & TxtFileNameOneTime.FileName
        '    TxtFileNameOneTime.SaveAs(TargetFilenameOneTime)
        '    ReadExcelData(TargetFilenameOneTime, "tblPayInstruction", BatchNo, "ONETIME")
        '    OneTimeFileName = TxtFileNameOneTime.FileName
        'Else
        '    OneTimeFileName = "None"
        'End If

        'vSQL = "insert into tblPayInstructionHeader (BatchNo,PayrollPeriod,PayDate,FileNameRecurring,FileNameOneTime,Remarks,CreatedBy,DateCreated,CutOffFromDate,CutOffToDate) values (" _
        '    & BatchNo & ",'" & CmdPayPeriod.SelectedValue & "','" & TxtTargetPaydate.Text.Trim & "','" _
        '    & RecurringFileName & "','" & OneTimeFileName & "','" & TxtRemarks.Text.Trim _
        '    & "','" & Session("uid") & "','" & Now() & "','" & CDate(TxtCFrom.Text) & "','" & CDate(TxtCTo.Text) & "')" 
        'CreateRecords(vSQL)

        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
    End Sub

    Private Sub ReadExcelData(FilePath As String, TblName As String, BatchNo As Int64, IntructionType As String)

        Dim xlApp As Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet

        Dim IsEmpty As String = ""
        Dim TblColName As String = ""
        Dim TblColData As String = ""
        Dim TblTaxCol As String = ""
        Dim TblTaxValue As String = ""
        Dim EmpList As String = ""
        Dim DelCell As String = ""
        Dim TempAmt As String = ""

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


        For i As Integer = 2 To 5000

            'Response.Write(xlWorkSheet.Cells(i, 15).value & "<br>")

            IsEmpty = xlWorkSheet.Cells(i, 1).value

            If IsEmpty = "" Then
                Exit For
            End If




            If IntructionType = "ONETIME" Then
                TblColName = "BatchNo,"
                TblColData += "(" & BatchNo & ","
            Else
                TblColName = ""
                TblColData += "("
            End If


            '===============================================================================================================
            ' SELECT EXCEL PROPERTIES
            '=============================================================================================================== 
            vSQL = "select TblName, TblColName, SourceCol, Remarks from tblExcelImportProperties " _
                    & "where Active=0 and TblName='" & TblName & "' "

            If IntructionType = "ONETIME" Then
                vSQL += "and Remarks='ONETIME' order by SourceCol"
            Else
                vSQL += "and Remarks='RECURRING' order by SourceCol"
            End If

            cm.CommandText = vSQL

            rs = cm.ExecuteReader
            Do While rs.Read
                TblColName += rs("TblColName") & ","


                If rs("TblColName") = "Amount" Then
                    TempAmt = xlWorkSheet.Cells(i, rs("SourceCol")).value




                    If TempAmt = "" Then
                        TblColData += "'0',"
                    Else
                        TblColData += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
                    End If
                    'Try
                    '    TempAmt = CDec(xlWorkSheet.Cells(i, rs("SourceCol")).value)
                    '    TblColData += "'x" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
                    'Catch ex As Exception
                    '    
                    'End Try
                Else
                    TblColData += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
                End If

            Loop
            rs.Close()





            '=============================================================================================================== 
            'BUILD SQL QUERY
            '=============================================================================================================== 
            If IntructionType = "ONETIME" Then
                TblColName += "CreatedBy,DateCreated,ElementType "
                TblColData += "'" & Session("uid") & "','" & Now & "'," & IIf(IntructionType = "ONETIME", 0, 1) & "),"
            End If

            If IntructionType = "RECURRING" Then
                TblColName += "CreatedBy,DateCreated,IsEarnings,IsActive "
                TblColData += "'" & Session("uid") & "','" & Now & "',1,1),"

                '=============================================================================================================== 
                'UPDATE OLD RECURRING EARNINGS TO IN-ACTIVE
                vSQL = "update " & TblName & " set IsActive=0 where " _
                            & "EmpCode='" & xlWorkSheet.Cells(i, 1).value & "' and " _
                            & "PayElementId='" & xlWorkSheet.Cells(i, 3).value & "'"
                CreateRecords(vSQL)
            End If
            '=============================================================================================================== 



        Next

        c.Close()
        c.Dispose()
        cm.Dispose()

        TblColName = TblColName.Substring(0, TblColName.Length - 1)
        TblColData = TblColData.Substring(0, TblColData.Length - 1)


        If IntructionType = "ONETIME" Then
            vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
            CreateRecords(vSQL)
        End If


        '=============================================================================================================== 
        'INSERT NEW LIST OF RECURRING EARNINGS
        '=============================================================================================================== 
        If IntructionType = "RECURRING" Then
            vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
            CreateRecords(vSQL)
            'Response.Write(vSQL & "<br><br>")
            vSQL = "update " & TblName & " set ValidFrom=null, ValidTo=null where ValidFrom='1900-01-01 00:00:00.000' and ValidTo='1900-01-01 00:00:00.000'"
            CreateRecords(vSQL)
        End If
        '=============================================================================================================== 




        For i As Integer = 2 To 5000
            'DelCell = xlWorkSheet.Cells(i, 15).value.ToString
            'Response.Write(xlWorkSheet.Cells(i, 1).value.ToString & "<br>")


            If xlWorkSheet.Cells(i, 15).value = "TRUE" Then
                vSQL = "delete from " & TblName & " where " _
                            & "EmpCode='" & xlWorkSheet.Cells(i, 1).value & "' and " _
                            & "PayElementId='" & xlWorkSheet.Cells(i, 3).value & "'"
                CreateRecords(vSQL)
            End If
        Next

        xlWorkBook.Close()
        xlApp.Quit()

        releaseObject(xlApp)
        releaseObject(xlWorkBook)
        releaseObject(xlWorkSheet)

        GetDocumentType()
    End Sub
End Class
