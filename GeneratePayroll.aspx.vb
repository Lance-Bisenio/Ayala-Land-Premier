Imports System.Data
Imports HelperClass
Imports System.Net.Mail
'Imports Microsoft.Office.Interop
Imports System.Data.OleDb
Imports Microsoft.Office.Interop
Partial Class GeneratePayroll
    Inherits System.Web.UI.Page
    Dim vSQL As String = ""
    Public EarningOneTime As String = ""
    Public EarningRecurring As String = ""
    Public DeductionOneTime As String = ""
    Public DeductionRecurring As String = ""
    Public SummaryComputation As String = ""
    Public DuplicateWageType As String = ""
    Public TagAsResigned As String = ""
    Public InActiveEmp As String = ""
    Public EmpNotFound As String = ""
    Public WageTypeNotFound As String = ""
    Private Sub GeneratePayroll_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("uid") = "" Then
            Response.Redirect("~/Login")
        End If
        If Not IsPostBack Then
            Dim CanViewApp As Integer = 0
            vSQL = "select Count(User_Id) as Ctr from Tblrightslist where Property_Value='2000' and User_Id='" & Session("uid") & "'"
            CanViewApp = GetRef(vSQL, "")
            If CanViewApp = 0 Then
                Response.Redirect("~/AccessDenied")
            End If
            CmdPayPeriod.Items.Add("Monthly")
            CmdPayPeriod.Items.Add("1st Period")
            CmdPayPeriod.Items.Add("2nd Period")
            CmdPayPeriod.Items.Add("Special Run")
            GetPayrollRunList()
            BtnPost.Disabled = True
            BtnException.Enabled = False
            BtnDownloadPayReg.Enabled = False
            BtnJVReport.Enabled = False
            BtnReUpload.Disabled = True
            BtnPost.Disabled = True
            BtnLock.Disabled = True
            BtnGenerateBankReport.Disabled = True
            TxtCFrom.Text = Format(CDate(Now().Month & "/01/" & Now().Year), "MM/dd/yyyy")
            TxtCTo.Text = Format(CDate(Now().Month & "/01/" & Now().Year).AddMonths(1).AddDays(-1), "MM/dd/yyyy")
            TxtTargetPaydate.Text = TxtCTo.Text
        End If

        'Response.Write(CountDays("02/28/2024", "03/01/2024") & "<br>")
        'Response.Write(CountDays("01/01/2025", "04/10/2025") & "<br>")
        'Response.Write(CountDays("02/28/2025", "03/01/2025") & "<br>")

        'Response.Write(Now.Month & "-" & Now.Day)

    End Sub
    Private Sub BtnSubmit_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitFileInstruction.ServerClick
        Dim TargetFilenameRecurring As String = ""
        Dim TargetFilenameOneTime As String
        Dim RecurringFileName As String = ""
        Dim OneTimeFileName As String = ""
        Dim BatchNo As Int64 = 0
        Dim LockCount As Integer = 0
        Dim TempDate As Date
        Try
            TempDate = CDate(TxtCFrom.Text.Trim)
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid date cutoff value. \nPlease enter correct and valid date format.'); $('#UploadFiles').modal();", True)
            Exit Sub
        End Try
        Try
            TempDate = CDate(TxtCTo.Text.Trim)
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid date cutoff value.  \nPlease enter correct and valid date format.'); $('#UploadFiles').modal();", True)
            Exit Sub
        End Try
        Try
            TempDate = CDate(TxtTargetPaydate.Text.Trim)
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Invalid payout date value. \nPlease enter correct and valid date format.'); $('#UploadFiles').modal();", True)
            Exit Sub
        End Try


        ' To check if the target payout date is already locked
        vSQL = "select count(BatchNo) as Lock from tblPayInstructionHeader where DatePosted is not null and " _
& "PayDate='" & Format(CDate(TxtTargetPaydate.Text), "MM/dd/yyyy") & "'"
        LockCount = GetRef(vSQL, 0)
        If LockCount > 0 Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The selected target payout date release is already locked.')", True)
            Exit Sub
        End If
        ' End of checking if the target payout date is already locked


        ' Generate Batch Number
        BatchNo = Format(Now(), "MMddyyyyHHmmss")
        If TxtFileNameOneTime.FileName <> "" Then
            TargetFilenameOneTime = Server.MapPath(".") & "\Uploaded\SystemInputFiles\" & Format(Now(), "MMddyyyyHHmmss") & "-PayInstruction-" & TxtFileNameOneTime.FileName
            TxtFileNameOneTime.SaveAs(TargetFilenameOneTime)
            LoadOnetimeData(TargetFilenameOneTime, "tblPayInstruction", BatchNo, "ONETIME", "")
            LoadRecurringData(TargetFilenameOneTime, "tblPayInstruction", BatchNo, "Recurring", "")
            OneTimeFileName = TxtFileNameOneTime.FileName
        Else
            OneTimeFileName = "None"
        End If
        vSQL = "insert into tblPayInstructionHeader (BatchNo,PayrollPeriod,PayDate,FileNameRecurring,FileNameOneTime,Remarks,CreatedBy,DateCreated,CutOffFromDate,CutOffToDate) values (" _
& BatchNo & ",'" & CmdPayPeriod.SelectedValue & "','" & TxtTargetPaydate.Text.Trim & "','" _
& RecurringFileName & "','" & OneTimeFileName & "','" & TxtRemarks.Text.Trim _
& "','" & Session("uid") & "','" & Now() & "','" & CDate(TxtCFrom.Text) & "','" & CDate(TxtCTo.Text) & "')"
        CreateRecords(vSQL)
        GetPayrollRunList()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
    End Sub
    Private Sub LoadOnetimeData(FilePath As String, TblName As String, BatchNo As Int64, IntructionType As String, TranType As String)
        Dim xlApp As Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet
        Dim AmtAdjustFrom As Decimal = 0
        Dim AmtAdjustTo As Decimal = 0
        Dim AmtAdjusted As Decimal = 0
        Dim DaysCnt As Integer = 0
        Dim DateStart As Date
        Dim DateEnd As Date
        Dim IsEmpty As String = ""
        Dim TblColName As String = ""
        Dim TblColData As String = ""
        Dim TblTaxCol As String = ""
        Dim TblTaxValue As String = ""
        Dim EmpList As String = ""
        If TranType = "ReUpload" Then
            vSQL = "delete from tblPayInstructionOnetime where BatchNo='" & BatchNo & "'"
            'Response.Write(vSQL & Now)
            CreateRecords(vSQL)
        End If
        xlApp = New Excel.ApplicationClass
        xlWorkBook = xlApp.Workbooks.Open(FilePath)

        Try
            xlWorkSheet = xlWorkBook.Worksheets("OneTime")
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('OneTime tab not found in the uploaded file. \n\nPlease change the sheet name to OneTime then re-upload.');", True)
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
            IsEmpty = xlWorkSheet.Cells(i, 1).value
            If IsEmpty = "" Then
                Exit For
            End If

            'If IntructionType = "ONETIME" Then
            '    TblColName = "BatchNo,"
            '    TblColData += "(" & BatchNo & ","
            'Else
            '    TblColName = ""
            '    TblColData += "("
            'End If

            Try
                AmtAdjustFrom = xlWorkSheet.Cells(i, 6).value.ToString
            Catch ex As Exception
                AmtAdjustFrom = 0
            End Try
            Try
                AmtAdjustTo = xlWorkSheet.Cells(i, 7).value.ToString
            Catch ex As Exception
                AmtAdjustTo = 0
            End Try

            AmtAdjusted = 0

            TblColData += "(" & BatchNo & ",'"
            TblColData += xlWorkSheet.Cells(i, 1).value & "',"      'EmpCode
            TblColData += 0 & ",'"                                  'ElementType
            TblColData += xlWorkSheet.Cells(i, 3).value & "','"     'PayElement
            TblColData += xlWorkSheet.Cells(i, 4).value & "','"     'ValidFrom
            TblColData += xlWorkSheet.Cells(i, 5).value & "',"      'ValidTo
            TblColData += AmtAdjustFrom & ","                       'AmtAdjustFrom
            TblColData += AmtAdjustTo & ","                         'AmtAdjustTo

            DateStart = xlWorkSheet.Cells(i, 4).value
            DateEnd = xlWorkSheet.Cells(i, 5).value

            DaysCnt = CountDays(DateStart, DateEnd)

            Select Case xlWorkSheet.Cells(i, 8).value.ToString
                Case "Release"
                    '(16000/2/15*75)
                    AmtAdjusted = AmtAdjustTo / 2 / 15 * DaysCnt
                Case "Adjust"
                    '(16000/2/15*19)-(13500/2/15*19)
                    AmtAdjusted = (AmtAdjustTo / 2 / 15 * DaysCnt) - (AmtAdjustFrom / 2 / 15 * DaysCnt)

                    'Response.Write("(" & AmtAdjustTo & "/ 2 / 15 * " & DaysCnt & ") - ")
                    'Response.Write("(" & AmtAdjustFrom & "/ 2 / 15 * " & DaysCnt & ") & <br><br>")

                Case "Deduction"
                    AmtAdjusted = 0
            End Select

            TblColData += AmtAdjusted & ","                         'AmtAdjusted
            TblColData += DaysCnt & ",'"                            'DaysCnt
            TblColData += xlWorkSheet.Cells(i, 8).value & "','"     'Remarks
            TblColData += Session("uid") & "','"                    'CreatedBy
            TblColData += Now & "'),"

            ''===============================================================================================================
            '' SELECT EXCEL PROPERTIES
            ''=============================================================================================================== 
            'vSQL = "select TblName, TblColName, SourceCol, Remarks from tblExcelImportProperties " _
            '		& "where Active=0 and TblName='" & TblName & "' "

            'If IntructionType = "ONETIME" Then
            '	vSQL += "and Remarks='ONETIME' order by SourceCol"
            'Else
            '	vSQL += "and Remarks='RECURRING' order by SourceCol"
            'End If

            'cm.CommandText = vSQL

            'rs = cm.ExecuteReader
            'Do While rs.Read
            '	TblColName += rs("TblColName") & ","
            '	TblColData += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
            'Loop
            'rs.Close()

            ''=============================================================================================================== 
            ''BUILD SQL QUERY
            ''=============================================================================================================== 
            'If IntructionType = "ONETIME" Then
            '	TblColName += "CreatedBy,DateCreated,ElementType "
            '	TblColData += "'" & Session("uid") & "','" & Now & "'," & IIf(IntructionType = "ONETIME", 0, 1) & "),"
            'End If

            'If IntructionType = "RECURRING" Then
            '	TblColName += "CreatedBy,DateCreated,IsEarnings,IsActive "
            '	TblColData += "'" & Session("uid") & "','" & Now & "',1,1),"

            '	'=============================================================================================================== 
            '	'UPDATE OLD RECURRING EARNINGS TO IN-ACTIVE
            '	vSQL = "update " & TblName & " set IsActive=0 where " _
            '		& "EmpCode='" & xlWorkSheet.Cells(i, 1).value & "' and " _
            '		& "PayElementId='" & xlWorkSheet.Cells(i, 3).value & "'"
            '	CreateRecords(vSQL)
            'End If
            ''=============================================================================================================== 

        Next
        c.Close()
        c.Dispose()
        cm.Dispose()

        xlWorkBook.Close()
        xlApp.Quit()
        releaseObject(xlApp)
        releaseObject(xlWorkBook)
        releaseObject(xlWorkSheet)

        TblName = "insert into tblPayInstructionOnetime "
        TblColName = "BatchNo,EmpCode,ElementType,PayElement,ValidFrom,ValidTo,AmtAdjustFrom,AmtAdjustTo,AmtAdjusted,DaysCnt,Remarks,CreatedBy,DateCreated"
        TblColData = TblColData.Substring(0, TblColData.Length - 1)
        vSQL = TblName & "(" & TblColName & ") values " & TblColData
        CreateRecords(vSQL)

        'Response.Write(vSQL)

        'TblColName = TblColName.Substring(0, TblColName.Length - 1)
        'TblColData = TblColData.Substring(0, TblColData.Length - 1)
        'If IntructionType = "ONETIME" Then
        '	vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
        '	CreateRecords(vSQL)
        'End If
        ''=============================================================================================================== 
        ''INSERT NEW LIST OF RECURRING EARNINGS
        ''=============================================================================================================== 
        'If IntructionType = "RECURRING" Then
        '	vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
        '	CreateRecords(vSQL)

        '	vSQL = "update " & TblName & " set ValidFrom=null, ValidTo=null where ValidFrom='1900-01-01 00:00:00.000' and ValidTo='1900-01-01 00:00:00.000'"
        '	CreateRecords(vSQL)
        'End If
        ''=============================================================================================================== 


    End Sub

    Private Sub LoadRecurringData(FilePath As String, TblName As String, BatchNo As Int64, IntructionType As String, TranType As String)
        Dim xlApp As Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet


        Dim IsEmpty As String = ""
        Dim TblColName As String = ""
        Dim TblColData As String = ""
        Dim TblTaxCol As String = ""

        If TranType = "ReUpload" Then
            vSQL = "delete from tblPayInstructionRecurring where BatchNo='" & BatchNo & "'"
            CreateRecords(vSQL)
        End If

        xlApp = New Excel.ApplicationClass
        xlWorkBook = xlApp.Workbooks.Open(FilePath)

        Try
            xlWorkSheet = xlWorkBook.Worksheets("Recurring")
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Recurring tab not found in the uploaded file. \n\nPlease change the sheet name to Recurring then re-upload.');", True)
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
            IsEmpty = xlWorkSheet.Cells(i, 1).value
            If IsEmpty = "" Then
                Exit For
            End If

            TblColData += "('"
            TblColData += xlWorkSheet.Cells(i, 1).value & "',"      'EmpCode
            TblColData += IIf(xlWorkSheet.Cells(i, 8).value.ToString.Trim = "Earnings", 0, 1) & ",'" 'IsEarnings 
            TblColData += xlWorkSheet.Cells(i, 3).value & "','"     'PayElementId
            TblColData += xlWorkSheet.Cells(i, 4).value & "',"      'Amount
            TblColData += "'',"                                     'ValidFrom
            TblColData += "'',"                                     'ValidTo
            TblColData += IIf(xlWorkSheet.Cells(i, 6).value.ToString.Trim.ToUpper = "FALSE", 1, 0) & ",'"      'IsActive
            TblColData += Session("uid") & "','"                    'CreatedBy
            TblColData += Now & "','"                               'DateCreated
            TblColData += xlWorkSheet.Cells(i, 5).value & "',"      'AmtPerPay
            TblColData += xlWorkSheet.Cells(i, 7).value & ",'"      'Terms
            TblColData += xlWorkSheet.Cells(i, 8).value & "'),"     'Remarks

        Next
        c.Close()
        c.Dispose()
        cm.Dispose()

        xlWorkBook.Close()
        xlApp.Quit()
        releaseObject(xlApp)
        releaseObject(xlWorkBook)
        releaseObject(xlWorkSheet)


        TblName = "insert into tblPayInstructionRecurring "
        TblColName = "EmpCode,IsEarnings,PayElementId,Amount,ValidFrom,ValidTo,IsActive,CreatedBy,DateCreated,AmountPerPay,PayTerms,Remarks"
        TblColData = TblColData.Substring(0, TblColData.Length - 1)

        vSQL = TblName & "(" & TblColName & ") values " & TblColData
        CreateRecords(vSQL)
        'Response.Write(vSQL)

    End Sub
    Function CountDays(startDate As DateTime, endDate As DateTime) As Integer
        If endDate < startDate Then
            Throw New ArgumentException("End date must be greater than or equal to start date.")
        End If
        Dim totalDays As Integer = (endDate - startDate).Days + 1 ' Inclusive
        Dim has31st As Boolean = False
        Dim lessDays As Integer = 0
        Dim addDays As Integer = 0

        totalDays = DateDiff("d", startDate, endDate)
        For i As Integer = 0 To totalDays
            If startDate.AddDays(i).Day = 31 Then
                lessDays += 1
            End If

            If startDate.AddDays(i).Month = 2 And startDate.AddDays(i).Day = 28 Then
                addDays = 2
            End If

            If startDate.AddDays(i).Month = 2 And startDate.AddDays(i).Day = 29 Then
                addDays = addDays - 1
            End If
        Next
        totalDays = totalDays - lessDays + addDays


        Return totalDays + 1
    End Function

    'Private Sub ReadExcelData(FilePath As String, TblName As String, BatchNo As Int64, IntructionType As String, TranType As String)

    '	Dim xlApp As Excel.Application
    '	Dim xlWorkBook As Excel.Workbook
    '	Dim xlWorkSheet As Excel.Worksheet

    '	Dim IsEmpty As String = ""
    '	Dim TblColName As String = ""
    '	Dim TblColData As String = ""
    '	Dim TblTaxCol As String = ""
    '	Dim TblTaxValue As String = ""
    '	Dim EmpList As String = ""

    '	If TranType = "ReUpload" Then
    '		vSQL = "delete from tblPayInstruction where BatchNo='" & BatchNo & "'"
    '		'Response.Write(vSQL & Now)
    '		CreateRecords(vSQL)
    '	End If
    '	'Response.Write(vSQL & Now)

    '	xlApp = New Excel.ApplicationClass
    '	xlWorkBook = xlApp.Workbooks.Open(FilePath)

    '	Try
    '		xlWorkSheet = xlWorkBook.Worksheets("Sheet1")
    '	Catch ex As Exception
    '		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Sheet1 cannot be found in the uploaded file. \n\nPlease change the sheet name to Sheet1 then re-upload.');", True)
    '		Exit Sub
    '	End Try

    '	Dim c As New SqlClient.SqlConnection
    '	Dim cm As New SqlClient.SqlCommand
    '	Dim rs As SqlClient.SqlDataReader
    '	c.ConnectionString = ConnStr

    '	Try
    '		c.Open()
    '		cm.Connection = c
    '	Catch ex As SqlClient.SqlException
    '		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
    '		Exit Sub
    '	End Try

    '	For i As Integer = 2 To 5000
    '		IsEmpty = xlWorkSheet.Cells(i, 1).value

    '		If IsEmpty = "" Then
    '			Exit For
    '		End If

    '		If IntructionType = "ONETIME" Then
    '			TblColName = "BatchNo,"
    '			TblColData += "(" & BatchNo & ","
    '		Else
    '			TblColName = ""
    '			TblColData += "("
    '		End If


    '		'===============================================================================================================
    '		' SELECT EXCEL PROPERTIES
    '		'=============================================================================================================== 
    '		vSQL = "select TblName, TblColName, SourceCol, Remarks from tblExcelImportProperties " _
    '				& "where Active=0 and TblName='" & TblName & "' "

    '		If IntructionType = "ONETIME" Then
    '			vSQL += "and Remarks='ONETIME' order by SourceCol"
    '		Else
    '			vSQL += "and Remarks='RECURRING' order by SourceCol"
    '		End If

    '		cm.CommandText = vSQL

    '		rs = cm.ExecuteReader
    '		Do While rs.Read
    '			TblColName += rs("TblColName") & ","
    '			TblColData += "'" & xlWorkSheet.Cells(i, rs("SourceCol")).value & "',"
    '		Loop
    '		rs.Close()





    '		'=============================================================================================================== 
    '		'BUILD SQL QUERY
    '		'=============================================================================================================== 
    '		If IntructionType = "ONETIME" Then
    '			TblColName += "CreatedBy,DateCreated,ElementType "
    '			TblColData += "'" & Session("uid") & "','" & Now & "'," & IIf(IntructionType = "ONETIME", 0, 1) & "),"
    '		End If

    '		If IntructionType = "RECURRING" Then
    '			TblColName += "CreatedBy,DateCreated,IsEarnings,IsActive "
    '			TblColData += "'" & Session("uid") & "','" & Now & "',1,1),"

    '			'=============================================================================================================== 
    '			'UPDATE OLD RECURRING EARNINGS TO IN-ACTIVE
    '			vSQL = "update " & TblName & " set IsActive=0 where " _
    '				& "EmpCode='" & xlWorkSheet.Cells(i, 1).value & "' and " _
    '				& "PayElementId='" & xlWorkSheet.Cells(i, 3).value & "'"
    '			CreateRecords(vSQL)
    '		End If
    '		'=============================================================================================================== 

    '	Next

    '	c.Close()
    '	c.Dispose()
    '	cm.Dispose()

    '	TblColName = TblColName.Substring(0, TblColName.Length - 1)
    '	TblColData = TblColData.Substring(0, TblColData.Length - 1)


    '	If IntructionType = "ONETIME" Then
    '		vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
    '		CreateRecords(vSQL)
    '	End If


    '	'=============================================================================================================== 
    '	'INSERT NEW LIST OF RECURRING EARNINGS
    '	'=============================================================================================================== 
    '	If IntructionType = "RECURRING" Then
    '		vSQL = "insert into " & TblName & " (" & TblColName & ") values " & TblColData
    '		CreateRecords(vSQL)

    '		vSQL = "update " & TblName & " set ValidFrom=null, ValidTo=null where ValidFrom='1900-01-01 00:00:00.000' and ValidTo='1900-01-01 00:00:00.000'"
    '		CreateRecords(vSQL)
    '	End If
    '	'=============================================================================================================== 


    '	xlWorkBook.Close()
    '	xlApp.Quit()

    '	releaseObject(xlApp)
    '	releaseObject(xlWorkBook)
    '	releaseObject(xlWorkSheet)

    'End Sub
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
    Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
        tblPayrollRun.SelectedIndex = -1
        tblPayrollRunDetails.DataSource = Nothing
        tblPayrollRunDetails.DataBind()
        GetPayrollRunList()
        BtnPost.Disabled = True
        BtnDownloadPayReg.Enabled = False
        BtnJVReport.Enabled = False
        BtnLock.Disabled = True
        BtnException.Enabled = False
        BtnReUpload.Disabled = True
    End Sub
    Private Sub GetPayrollRunList()
        Dim c As New SqlClient.SqlConnection
        Dim da As SqlClient.SqlDataAdapter
        Dim ds As New DataSet
        Dim vFilter As String = ""
        Dim vTableName As String = ""
        c.ConnectionString = ConnStr
        vSQL = "select BatchNo,PayrollPeriod,Format(PayDate,'MM/dd/yyyy') as PayDate,FileNameRecurring,FileNameOneTime,Remarks,CreatedBy," _
& "DateCreated,PostedBy,DatePosted,ProcessBy,DateProcess  " _
& "from tblPayInstructionHeader " _
& "where BatchNo is not null " & vFilter & " order by Id desc"
        da = New SqlClient.SqlDataAdapter(vSQL, c)
        da.Fill(ds, "tblEmployees")
        tblPayrollRun.DataSource = ds.Tables("tblEmployees")
        tblPayrollRun.DataBind()
        da.Dispose()
        ds.Dispose()
    End Sub
    Private Sub tblPayrollRun_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblPayrollRun.SelectedIndexChanged
        BtnPost.Disabled = False
        Session("Ctr") = 0
        Dim IsLock As String = tblPayrollRun.SelectedRow.Cells(8).Text.Trim
        Dim IsProcess As String = tblPayrollRun.SelectedRow.Cells(10).Text.Trim
        Session("BatchNo") = ""
        Session("PostingEmpList") = ""
        tblPayrollRunDetails.SelectedIndex = -1
        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim EmpList As String = ""
        Dim PostingEmpList As String = ""
        c.ConnectionString = ConnStr
        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
            Exit Sub
        End Try
        Session("BatchNo") = tblPayrollRun.SelectedRow.Cells(1).Text
        Session("PayDate") = tblPayrollRun.SelectedRow.Cells(3).Text
        vSQL = "select EmployeeCode as EmpCd from tblEmployees WHERE DateSeparated is null and Active=1 and CustomField1 is null order by FullName"
        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            EmpList += "'" & rs("EmpCd") & "',"
            PostingEmpList += rs("EmpCd") & ","
        Loop
        rs.Close()

        'Response.Write(vSQL & EmpList)

        Try
            Session("PostingEmpList") = PostingEmpList.Substring(0, PostingEmpList.Length - 1)
        Catch ex As Exception
            tblPayrollRunDetails.DataSource = Nothing
            tblPayrollRunDetails.DataBind()
            'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('No records found');", True)
            Exit Sub
        End Try
        c.Close()
        c.Dispose()
        cm.Dispose()
        EmpList = EmpList.Substring(0, EmpList.Length - 1)
        BtnPost.Disabled = False
        BtnDownloadPayReg.Enabled = True
        BtnJVReport.Enabled = True
        BtnReUpload.Disabled = False
        If IsProcess.Replace("&nbsp;", "") <> "" Then
            GetPayrollRunDetails()
            BtnException.Enabled = True
            BtnLock.Disabled = False
            BtnDownloadPayReg.Enabled = True
            BtnJVReport.Enabled = True
        Else
            tblPayrollRunDetails.DataSource = Nothing
            tblPayrollRunDetails.DataBind()
            BtnException.Enabled = False
            BtnLock.Disabled = True
            BtnDownloadPayReg.Enabled = False
            BtnJVReport.Enabled = False
        End If
        If IsLock.Replace("&nbsp;", "") <> "" Then
            BtnLock.InnerText = "Unlock Payroll"
            BtnPost.Disabled = True
            BtnGenerateBankReport.Disabled = False
            BtnReUpload.Disabled = True
        Else
            BtnLock.InnerText = "Lock Payroll"
            BtnPost.Disabled = False
            BtnGenerateBankReport.Disabled = True
            BtnReUpload.Disabled = False
        End If
    End Sub
    Private Sub GetPayrollRunDetails()
        Dim da As SqlClient.SqlDataAdapter
        Dim ds As New DataSet
        Dim vFilter As String = ""
        Dim vTableName As String = ""
        Dim c As New SqlClient.SqlConnection
        c.ConnectionString = ConnStr
        vSQL = "select distinct(EmpCode) as EmpCd," _
& "(select FullName from tblEmployees where EmpCode=EmployeeCode) as FullName," _
& "(select MonthlyRate from tblEmployees where EmpCode=EmployeeCode) As MonthlyRate " _
& "from tblPayInstruction " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' order by FullName"
        da = New SqlClient.SqlDataAdapter(vSQL, c)
        da.Fill(ds, "tblPayrollRunDetails")
        tblPayrollRunDetails.DataSource = ds.Tables("tblPayrollRunDetails")
        tblPayrollRunDetails.DataBind()
        da.Dispose()
        ds.Dispose()
    End Sub
    Private Sub tblPayrollRunDetails_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblPayrollRunDetails.SelectedIndexChanged
        LblEmpCode.InnerText = tblPayrollRunDetails.SelectedRow.Cells(1).Text
        LblFullNme.InnerText = tblPayrollRunDetails.SelectedRow.Cells(2).Text
        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim EmpList As String = ""
        Dim IsTaxable As Decimal = 0
        Dim TotalEarning As Decimal = 0
        Dim TotalDeduction As Decimal = 0
        Dim InputVAT As Decimal = 0
        Dim GrandTotal As Decimal = 0
        Dim NetPay As Decimal = 0
        c.ConnectionString = ConnStr
        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
            Exit Sub
        End Try
        vSQL = "select MonthlyRate, TINNo, FullName, EmployeeCode, " _
& "(select top 1 TaxPercent from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode order by id desc) as TaxPercent, " _
& "(select top 1 IsNonVat from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode order by id desc) as IsNonVat, " _
& "(select top 1 VatPercent from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode order by id desc) as VatPercent " _
& "from tblEmployees b where EmployeeCode='" & tblPayrollRunDetails.SelectedRow.Cells(1).Text & "'"
        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        If rs.Read Then
            LblFullNme.InnerText = rs("FullName")
            LblEmpCode.InnerText = rs("EmployeeCode")
            LblBasicAllowance.InnerText = Format(CDec(rs("MonthlyRate")), "#,###,##0.00")
            LblTin.InnerText = rs("TINNo")
            LblTaxRate.InnerText = Format(CDec(rs("TaxPercent")), "##0.00")
            LblPayDate.InnerText = Format(CDate(tblPayrollRun.SelectedRow.Cells(3).Text), "MM/dd/yyyy")
            LblInNonVat.InnerText = rs("IsNonVat")
            LblVatPercent.InnerText = Format(CDec(rs("VatPercent")), "##0.00")
        End If
        rs.Close()



        ' ==================================================================================================================================================
        ' ==================================================================================================================================================


        vSQL = "select (select Name from tblPayElements where Code=PayElement) as ElementName, " _
& "(select IsTaxable from tblPayElements where Code=PayElement) as IsTaxable," _
& "Amount, ElementType " _
& "from tblPayInstruction where " _
& "BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' and " _
& "EmpCode='" & tblPayrollRunDetails.SelectedRow.Cells(1).Text & "' and " _
& "PayElement in (select Code from tblPayElements where IsEarning=0) order by ElementName"

        '& "ElementType=1 and " _

        'Response.Write(vSQL)
        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            If Not IsDBNull(rs("IsTaxable")) = 1 Then
                IsTaxable += rs("Amount")
            End If
            If rs("ElementType") = 1 Then
                EarningRecurring += "<div class='row'>" _
& "<div class='col-sm-1'></div><div class='col-sm-7'><small>" & rs("ElementName") & "</small></div>" _
& "<div class='col-sm-4 text-right'><small>" & Format(CDec(rs("Amount")), "#,###,##0.00") & "</small></div>" _
& "</div>"
            Else
                EarningOneTime += "<div class='row'>" _
& "<div class='col-sm-1'></div><div class='col-sm-7'><small>" & rs("ElementName") & "</small></div>" _
& "<div class='col-sm-4 text-right'><small>" & Format(CDec(rs("Amount")), "#,###,##0.00") & "</small></div>" _
& "</div>"
            End If
            TotalEarning += CDec(rs("Amount"))
        Loop
        EarningOneTime += "<div class='row'>" _
& "<div class='col-sm-1'></div><div class='col-sm-7'><small><b>TOTAL:</b></small></div>" _
& "<div class='col-sm-4 text-right'><small><b>" & Format(CDec(TotalEarning), "#,###,##0.00") & "</b></small></div>" _
& "</div>"
        rs.Close()

        ' ==================================================================================================================================================
        ' ==================================================================================================================================================

        vSQL = "select (select Name from tblPayElements where Code=PayElement) as ElementName, " _
& "(select IsTaxable from tblPayElements where Code=PayElement) as IsTaxable, " _
& "Amount, ElementType " _
& "from tblPayInstruction where " _
& "BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' and " _
& "EmpCode='" & tblPayrollRunDetails.SelectedRow.Cells(1).Text & "' and " _
& "PayElement in (select Code from tblPayElements where IsEarning=1 and Active=1) order by ElementName"
        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            If rs("ElementType") = 1 Then
                DeductionRecurring += "<div class='row'>" _
& "<div class='col-sm-1'></div><div class='col-sm-7'><small>" & rs("ElementName") & "</small></div>" _
& "<div class='col-sm-4 text-right'><small>" & Format(CDec(rs("Amount")), "#,###,##0.00") & "</small></div>" _
& "</div>"
            Else
                DeductionOneTime += "<div class='row'>" _
& "<div class='col-sm-1'></div><div class='col-sm-7'><small>" & rs("ElementName") & "</small></div>" _
& "<div class='col-sm-4 text-right'><small>" & Format(CDec(rs("Amount")), "#,###,##0.00") & "</small></div>" _
& "</div>"
            End If
            TotalDeduction += CDec(rs("Amount"))
        Loop
        rs.Close()
        DeductionOneTime += "<div class='row'>" _
& "<div class='col-sm-1'></div><div class='col-sm-7'><small><b>TOTAL:</b></small></div>" _
& "<div class='col-sm-4 text-right'><small><b>" & Format(CDec(TotalDeduction), "#,###,##0.00") & "</b></small></div>" _
& "</div>"

        ' ==================================================================================================================================================
        ' ==================================================================================================================================================

        SummaryComputation += "<div class='row'>" _
& "<div class='col-sm-1'></div>" _
& "<div class='col-sm-6 text-left'><small>Total Taxable:</small></div>" _
& "<div class='col-sm-4 text-right'><small>" & Format(CDec(TotalEarning), "#,###,##0.00") & "</small></div>" _
& "<div class='col-sm-1'></div>" _
& "</div>"
        InputVAT = TotalEarning * LblVatPercent.InnerText
        SummaryComputation += "<div class='row'>" _
& "<div class='col-sm-1'></div>" _
& "<div class='col-sm-6 text-left'><small>Input VAT:</small></div>" _
& "<div class='col-sm-4 text-right'><small>" & Format(CDec(InputVAT), "#,###,##0.00") & "</small></div>" _
& "<div class='col-sm-1'></div>" _
& "</div>"
        GrandTotal = TotalEarning + InputVAT
        SummaryComputation += "<div class='row'>" _
& "<div class='col-sm-1'></div>" _
& "<div class='col-sm-6 text-left'><small><b>Grand Total:</b></small></div>" _
& "<div class='col-sm-4 text-right'><small><b>" & Format(CDec(GrandTotal), "#,###,##0.00") & "</b></small></div>" _
& "<div class='col-sm-1'></div>" _
& "</div><br/>"
        IsTaxable = IsTaxable * LblTaxRate.InnerText
        SummaryComputation += "<div class='row'>" _
& "<div class='col-sm-1'></div>" _
& "<div class='col-sm-6 text-left'><small>Withholding Tax (EWT):</small></div>" _
& "<div class='col-sm-4 text-right'><small>(" & Format(CDec(IsTaxable), "#,###,##0.00") & ")</small></div>" _
& "<div class='col-sm-1'></div>" _
& "</div>"
        SummaryComputation += "<div class='row'>" _
& "<div class='col-sm-1'></div>" _
& "<div class='col-sm-6 text-left'><small>Total Duduction:</small></div>" _
& "<div class='col-sm-4 text-right'><small>(" & Format(CDec(TotalDeduction), "#,###,##0.00") & ")</small></div>" _
& "<div class='col-sm-1'></div>" _
& "</div><br/>"
        NetPay = GrandTotal - (TotalDeduction + IsTaxable)
        SummaryComputation += "<div class='row'>" _
& "<div class='col-sm-1'></div>" _
& "<div class='col-sm-6 text-left'><small><b>Net Amount:</b></small></div>" _
& "<div class='col-sm-4 text-right'><small><b>" & Format(CDec(NetPay), "#,###,##0.00") & "</b></small></div>" _
& "<div class='col-sm-1'></div>" _
& "</div>"

        ' ==================================================================================================================================================
        ' ==================================================================================================================================================


        c.Close()
        c.Dispose()
        cm.Dispose()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "ShowDetails();", True)
    End Sub
    Private Sub tblPayrollRun_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblPayrollRun.PageIndexChanging
        tblPayrollRun.PageIndex = e.NewPageIndex
        tblPayrollRun.SelectedIndex = -1
        tblPayrollRunDetails.DataSource = Nothing
        tblPayrollRunDetails.DataBind()
        BtnPost.Disabled = True
        BtnDownloadPayReg.Enabled = False
        BtnJVReport.Enabled = False
        BtnLock.Disabled = True
        BtnException.Enabled = False
        BtnGenerateBankReport.Disabled = True
        GetPayrollRunList()
        Session("TranID") = ""
    End Sub
    Private Sub tblPayrollRunDetails_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblPayrollRunDetails.PageIndexChanging
        tblPayrollRunDetails.PageIndex = e.NewPageIndex
        tblPayrollRunDetails.SelectedIndex = -1
        GetPayrollRunDetails()
        Session("TranID") = ""
    End Sub
    Private Sub BtnClose_ServerClick(sender As Object, e As EventArgs) Handles BtnClose.ServerClick
        tblPayrollRun.SelectedIndex = -1
        tblPayrollRunDetails.DataSource = Nothing
        tblPayrollRunDetails.DataBind()
        GetPayrollRunList()
        BtnPost.Disabled = True
        BtnPost.Disabled = True
        BtnDownloadPayReg.Enabled = False
        BtnJVReport.Enabled = False
        BtnLock.Disabled = True
        BtnException.Enabled = False
        BtnGenerateBankReport.Disabled = True
    End Sub
    Private Sub BtnDownloadPayReg_Click(sender As Object, e As EventArgs) Handles BtnDownloadPayReg.Click
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
        xlWorkSheet.Range("A2:B2").Merge()
        xlWorkSheet.Cells(1, 1) = "AYALA LAND SALES, INC."
        xlWorkSheet.Cells(2, 1) = "Payroll Register"
        xlWorkSheet.Cells(3, 1) = "Pay Date: " & Format(CDate(tblPayrollRun.SelectedRow.Cells(3).Text), "MM/dd/yyyy")
        xlWorkSheet.Cells(5, 1) = "EMPLOYEE INFO"
        xlWorkSheet.Cells(6, 1) = "Employee Code"
        xlWorkSheet.Cells(6, 2) = "Employee Name"
        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim DeductionCodeTemp As String = ""
        Dim EarningsCodeTemp As String = ""
        Dim DeductionCodeList() As String
        Dim EarningsCodeList() As String
        Dim DeductionTemp As String = ""
        Dim EarningsTemp As String = ""
        Dim DeductionList() As String
        Dim EarningsList() As String
        Dim CellLetterTemp As String = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,AA,AB,AC,AD,AE,AF,AG,AH,AI,AJ,AK,AL,AM,AN,AO,AP,AQ,AR,AS,AT,AU,AV,AW,AX,AY,AZ"
        Dim CellLetterList() As String
        Dim CellHdrCtr As Integer = 2
        Dim CellResultValCtr As Integer = 0
        Dim CellCtr As Integer = 3
        Dim CellRowCtr As Integer = 1
        Dim TotalVal1 As Decimal = 0
        Dim TotalVal2 As Decimal = 0
        Dim TotalVal3 As Decimal = 0
        Dim FileName As String = ""
        c.ConnectionString = ConnStr
        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
            Exit Sub
        End Try
        vSQL = "select Code as ElementCode, Name as PayName, IsEarning, IsTaxable from tblPayElements where Active=1 order by Name "
        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            If Not IsDBNull(rs("IsEarning")) Then
                If rs("IsEarning") = 0 And rs("PayName") <> "" Then
                    EarningsTemp += rs("PayName") & ","
                    EarningsCodeTemp += rs("ElementCode") & ","
                End If
                If rs("IsEarning") = 1 And rs("PayName") <> "" Then
                    DeductionTemp += rs("PayName") & ","
                    DeductionCodeTemp += rs("ElementCode") & ","
                End If
            End If
        Loop
        rs.Close()


        ' ------------------------------------------------------------------------------------------------------------
        ' ------------------------------------------------------------------------------------------------------------

        EarningsTemp = EarningsTemp.Substring(0, EarningsTemp.Length - 1)
        EarningsList = EarningsTemp.Split(",")
        For i As Integer = 0 To EarningsTemp.Split(",").Length - 1
            xlWorkSheet.Cells(6, CellCtr) = EarningsList(i)
            CellCtr += 1
        Next
        CellResultValCtr += EarningsTemp.Split(",").Length
        CellLetterList = CellLetterTemp.Split(",")
        CellHdrCtr += EarningsTemp.Split(",").Length
        xlWorkSheet.Range("C5:" & CellLetterList(CellHdrCtr - 1).ToString & "5").Merge()
        xlWorkSheet.Cells(5, 3) = "TAXABLE EARNINGS"
        xlWorkSheet.Cells(5, CellCtr) = "VAT"
        CellHdrCtr += 1
        Session("CellHdrCtr") = CellHdrCtr
        xlWorkSheet.Cells(6, CellCtr) = "VAT"
        CellCtr += 1
        xlWorkSheet.Cells(6, CellCtr) = "WTAX(EWT)"
        CellCtr += 1



        ' ------------------------------------------------------------------------------------------------------------

        DeductionTemp = DeductionTemp.Substring(0, DeductionTemp.Length - 1)
        DeductionList = DeductionTemp.Split(",")

        'xlWorkSheet.Range(CellList(CellCtr).ToString & "2" & CellList(CellCtr + DeductionList.Length).ToString & "2").Merge()
        'xlWorkSheet.Cells(2, CellCtr + 1) = "DEDUCTION"


        For i As Integer = 0 To DeductionTemp.Split(",").Length - 1
            xlWorkSheet.Cells(6, CellCtr) = DeductionList(i)
            CellCtr += 1
        Next



        '

        CellResultValCtr = DeductionTemp.Split(",").Length
        CellHdrCtr += CellResultValCtr
        xlWorkSheet.Range(CellLetterList(Session("CellHdrCtr")).ToString & "5:" & CellLetterList(CellHdrCtr).ToString & "5").Merge()
        xlWorkSheet.Cells(5, Session("CellHdrCtr") + 1) = "DEDUCTIONS"
        CellHdrCtr += 1

        'Response.Write(CellHdrCtr + 1)

        xlWorkSheet.Range(CellLetterList(CellHdrCtr).ToString & "5:" & CellLetterList(CellHdrCtr + 2).ToString & "5").Merge()
        xlWorkSheet.Cells(5, CellHdrCtr + 1) = "TOTAL"
        xlWorkSheet.Cells(6, CellCtr) = "Gross"
        CellCtr += 1
        xlWorkSheet.Cells(6, CellCtr) = "Deductions"
        CellCtr += 1
        xlWorkSheet.Cells(6, CellCtr) = "Net Pay"

        ' ------------------------------------------------------------------------------------------------------------
        ' ------------------------------------------------------------------------------------------------------------

        vSQL = "select distinct(EmpCode) as EmpCode, " _
& "(select FullName from tblEmployees where EmpCode=EmployeeCode) As FullName " _
& "from tblPayInstruction " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "'" _
& "order by FullName"
        cm.CommandText = vSQL
        CellRowCtr = 7
        rs = cm.ExecuteReader
        Do While rs.Read
            xlWorkSheet.Cells(CellRowCtr, 1) = rs("EmpCode")
            xlWorkSheet.Cells(CellRowCtr, 2) = rs("FullName")
            CellRowCtr += 1
        Loop
        rs.Close()



        '-------------------------------------------------------------------------------------------------
        'EARNINGS VALUE
        '-------------------------------------------------------------------------------------------------
        EarningsCodeTemp = EarningsCodeTemp.Substring(0, EarningsCodeTemp.Length - 1)
        EarningsCodeList = EarningsCodeTemp.Split(",")
        For i As Integer = 0 To EarningsCodeList.Length - 1
            vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select sum(Amount) from tblPayInstruction b where a.EmpCode=b.EmpCode and b.PayElement='" _
& EarningsCodeList(i) & "' and b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as AmtVal " _
& "from tblPayInstruction a " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "order by FullName"
            cm.CommandText = vSQL
            CellCtr = 3
            CellRowCtr = 7
            rs = cm.ExecuteReader
            Do While rs.Read
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = Format(CDec(IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), 0.00)), "#,###,##0.00")
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
                TotalVal1 += IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), 0.00)
                CellRowCtr += 1
            Loop
            rs.Close()
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = Format(CDec(TotalVal1), "#,###,##0.00")
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
            TotalVal1 = 0
        Next




        '-------------------------------------------------------------------------------------------------
        'INPUT VAT AND WH-TAX VALUE
        '-------------------------------------------------------------------------------------------------
        vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select InputVAT from tblPayrollSummary b where a.EmpCode=b.EmpCode and b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as InputVat, " _
& "(Select WHTax from tblPayrollSummary b where a.EmpCode=b.EmpCode And b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as WHTax " _
& "From tblPayInstruction a  " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "'  " _
& "Order By FullName"
        cm.CommandText = vSQL
        CellCtr = 2 + EarningsCodeList.Length + 1
        CellRowCtr = 7
        rs = cm.ExecuteReader
        Do While rs.Read
            xlWorkSheet.Cells(CellRowCtr, CellCtr) = IIf(Not IsDBNull(rs("InputVat")), rs("InputVat"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1) = IIf(Not IsDBNull(rs("WHTax")), rs("WHTax"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1).NumberFormat = "#,###,##0.00"
            TotalVal1 += IIf(Not IsDBNull(rs("InputVat")), rs("InputVat"), 0.00)
            TotalVal2 += IIf(Not IsDBNull(rs("WHTax")), rs("WHTax"), 0.00)
            CellRowCtr += 1
        Loop
        rs.Close()
        xlWorkSheet.Cells(CellRowCtr, CellCtr) = TotalVal1
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 1) = TotalVal2
        xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 1).NumberFormat = "#,###,##0.00"
        TotalVal1 = 0
        TotalVal2 = 0
        CellCtr += 2



        '-------------------------------------------------------------------------------------------------
        'DEDUCTION VALUE
        '-------------------------------------------------------------------------------------------------
        DeductionCodeTemp = DeductionCodeTemp.Substring(0, DeductionCodeTemp.Length - 1)
        DeductionCodeList = DeductionCodeTemp.Split(",")
        For i As Integer = 0 To DeductionCodeList.Length - 1
            vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select sum(Amount) from tblPayInstruction b where a.EmpCode=b.EmpCode and b.PayElement='" _
& DeductionCodeList(i) & "' and b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as AmtVal " _
& "from tblPayInstruction a " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "order by FullName"
            cm.CommandText = vSQL
            CellRowCtr = 7
            rs = cm.ExecuteReader
            Do While rs.Read
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), 0.00)
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
                TotalVal1 += IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), 0.00)
                CellRowCtr += 1
            Loop
            rs.Close()
            Session("CellCtr") = i + CellCtr
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = TotalVal1
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
            TotalVal1 = 0
        Next




        '-------------------------------------------------------------------------------------------------
        'TOTAL VALUE
        '-------------------------------------------------------------------------------------------------
        vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select TotalDeduction + WHTax from tblPayrollSummary b where a.EmpCode=b.EmpCode And b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as TotalDeduction, " _
& "(Select GrandTotal from tblPayrollSummary b where a.EmpCode=b.EmpCode And b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as GrossPay, " _
& "(Select NetPay from tblPayrollSummary b where a.EmpCode=b.EmpCode And b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as NetPay " _
& "From tblPayInstruction a " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "Order By FullName"
        cm.CommandText = vSQL
        CellCtr = Session("CellCtr") + 1
        CellRowCtr = 7
        rs = cm.ExecuteReader
        Do While rs.Read
            xlWorkSheet.Cells(CellRowCtr, CellCtr) = IIf(Not IsDBNull(rs("GrossPay")), rs("GrossPay"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1) = IIf(Not IsDBNull(rs("TotalDeduction")), rs("TotalDeduction"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 2) = IIf(Not IsDBNull(rs("NetPay")), rs("NetPay"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1).NumberFormat = "#,###,##0.00"
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 2).NumberFormat = "#,###,##0.00"
            TotalVal1 += IIf(Not IsDBNull(rs("TotalDeduction")), rs("TotalDeduction"), 0.00)
            TotalVal2 += IIf(Not IsDBNull(rs("GrossPay")), rs("GrossPay"), 0.00)
            TotalVal3 += IIf(Not IsDBNull(rs("NetPay")), rs("NetPay"), 0.00)
            CellRowCtr += 1
        Loop
        rs.Close()
        xlWorkSheet.Cells(CellRowCtr, CellCtr) = TotalVal1
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 1) = TotalVal2
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 2) = TotalVal3
        xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 1).NumberFormat = "#,###,##0.00"
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 2).NumberFormat = "#,###,##0.00"
        c.Close()
        c.Dispose()
        cm.Dispose()
        FileName = Format(Now(), "MMddyyyyHHmmss") & "-PayRegister.xls"
        xlWorkBook.SaveAs(TargetPath & FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue)
        xlWorkBook.Close(True, misValue, misValue)
        xlApp.Quit()
        releaseObject(xlWorkSheet)
        releaseObject(xlWorkBook)
        releaseObject(xlApp)
        Response.Redirect("~/Downloads/Report/" & FileName)
    End Sub
    Private Sub BtnJVReport_Click(sender As Object, e As EventArgs) Handles BtnJVReport.Click
        Dim TargetPath As String = ""
        TargetPath = Server.MapPath(".") & "\Downloads\Report\" '& Format(Now(), "MMddyyyyHHmmss") & "-PayRegister" & TxtFileName.FileName

        Dim xlApp As Excel.Application = New Excel.Application()
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet

        'Dim xlApp As Excel.Application = New Excel.Application()
        'Dim xlApp As Excel.Application

        If xlApp Is Nothing Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Excel is not properly installed!!');", True)
            Return
        End If


        'Dim xlWorkBook As Excel.Application
        'Dim xlWorkSheet As Excel.Worksheet
        Dim misValue As Object = System.Reflection.Missing.Value
        xlWorkBook = xlApp.Workbooks.Add(misValue)
        xlWorkSheet = xlWorkBook.Sheets("sheet1")
        xlWorkSheet.Range("A2:B2").Merge()
        xlWorkSheet.Cells(1, 1) = "AYALA LAND SALES, INC."
        xlWorkSheet.Cells(2, 1) = "Pay Sum"
        xlWorkSheet.Cells(3, 1) = "Pay Date: " & Format(CDate(tblPayrollRun.SelectedRow.Cells(3).Text), "MM/dd/yyyy")
        xlWorkSheet.Cells(4, 1) = "Name"
        xlWorkSheet.Cells(4, 2) = "EE Number"
        xlWorkSheet.Cells(4, 3) = "DIVISION"
        xlWorkSheet.Cells(4, 4) = "GROUP"
        xlWorkSheet.Cells(4, 5) = "TIN"
        xlWorkSheet.Cells(4, 6) = "ADDRESS"
        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim DeductionCodeTemp As String = ""
        Dim EarningsCodeTemp As String = ""
        Dim DeductionCodeList() As String
        Dim EarningsCodeList() As String
        Dim DeductionTemp As String = ""
        Dim EarningsTemp As String = ""
        Dim DeductionList() As String
        Dim EarningsList() As String
        Dim CellLetterTemp As String = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,AA,AB,AC,AD,AE,AF,AG,AH,AI,AJ,AK,AL,AM,AN,AO,AP,AQ,AR,AS,AT,AU,AV,AW,AX,AY,AZ"
        Dim CellLetterList() As String
        Dim CellHdrCtr As Integer = 2
        Dim CellResultValCtr As Integer = 0
        Dim CellCtr As Integer = 7
        Dim CellRowCtr As Integer = 1
        Dim TotalVal1 As Decimal = 0
        Dim TotalVal2 As Decimal = 0
        Dim TotalVal3 As Decimal = 0
        Dim SubTotal As Decimal = 0
        Dim SubTotal2 As Decimal = 0
        Dim FileName As String = ""
        Dim BrkLine As String = ""
        Dim BLCtr As Integer = 0
        Dim DivCtr As Integer = 1
        c.ConnectionString = ConnStr
        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
            Exit Sub
        End Try
        vSQL = "select Code as ElementCode, Name as PayName, IsEarning, IsTaxable from tblPayElements where Active=1 order by Name "
        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            If Not IsDBNull(rs("IsEarning")) Then
                If rs("IsEarning") = 0 And rs("PayName") <> "" Then
                    EarningsTemp += rs("PayName") & ","
                    EarningsCodeTemp += rs("ElementCode") & ","
                End If
                If rs("IsEarning") = 1 And rs("PayName") <> "" Then
                    DeductionTemp += rs("PayName") & ","
                    DeductionCodeTemp += rs("ElementCode") & ","
                End If
            End If
        Loop
        rs.Close()
        CellResultValCtr += EarningsTemp.Split(",").Length
        CellLetterList = CellLetterTemp.Split(",")
        ' ------------------------------------------------------------------------------------------------------------
        ' CREATE RECURRING HEADERS
        ' ------------------------------------------------------------------------------------------------------------
        EarningsTemp = EarningsTemp.Substring(0, EarningsTemp.Length - 1)
        EarningsList = EarningsTemp.Split(",")
        CellHdrCtr = 6
        For i As Integer = 0 To EarningsTemp.Split(",").Length - 1
            xlWorkSheet.Range(CellLetterList(CellHdrCtr) & "4:" & CellLetterList(CellHdrCtr) & "5").Merge()
            xlWorkSheet.Cells(4, CellCtr) = EarningsList(i)
            CellCtr += 1
            CellHdrCtr += 1
        Next
        xlWorkSheet.Range(CellLetterList(CellHdrCtr) & "4:" & CellLetterList(CellHdrCtr) & "5").Merge()
        CellHdrCtr += 1
        xlWorkSheet.Range(CellLetterList(CellHdrCtr) & "4:" & CellLetterList(CellHdrCtr) & "5").Merge()
        CellHdrCtr += 1

        'CellHdrCtr += EarningsTemp.Split(",").Length

        'xlWorkSheet.Range("C5:" & CellLetterList(CellHdrCtr - 1).ToString & "5").Merge()
        'xlWorkSheet.Cells(5, 3) = "TAXABLE EARNINGS"

        'xlWorkSheet.Cells(5, CellCtr) = "VAT"


        'CellHdrCtr += 1
        'Session("CellHdrCtr") = CellHdrCtr



        xlWorkSheet.Cells(4, CellCtr) = "VAT"
        CellCtr += 1
        xlWorkSheet.Cells(4, CellCtr) = "WTAX(EWT)"
        CellCtr += 1



        ' ------------------------------------------------------------------------------------------------------------
        ' CREATE DEDUCTION HEADER
        ' ------------------------------------------------------------------------------------------------------------ 
        DeductionTemp = DeductionTemp.Substring(0, DeductionTemp.Length - 1)
        DeductionList = DeductionTemp.Split(",")
        For i As Integer = 0 To DeductionTemp.Split(",").Length - 1
            xlWorkSheet.Range(CellLetterList(CellHdrCtr) & "4:" & CellLetterList(CellHdrCtr) & "5").Merge()
            xlWorkSheet.Cells(4, CellCtr) = DeductionList(i)
            CellCtr += 1
            CellHdrCtr += 1
        Next
        xlWorkSheet.Cells(4, CellCtr) = "Total as of"
        CellHdrCtr += 1
        CellResultValCtr = DeductionTemp.Split(",").Length

        'CellHdrCtr += CellResultValCtr
        'xlWorkSheet.Range(CellLetterList(Session("CellHdrCtr")).ToString & "5:" & CellLetterList(CellHdrCtr).ToString & "5").Merge()
        'xlWorkSheet.Cells(5, Session("CellHdrCtr") + 1) = "DEDUCTIONS"
        'CellHdrCtr += 1
        'xlWorkSheet.Range(CellLetterList(CellHdrCtr).ToString & "5:" & CellLetterList(CellHdrCtr + 2).ToString & "5").Merge()
        'xlWorkSheet.Cells(5, CellHdrCtr + 1) = "TOTAL"

        xlWorkSheet.Cells(5, CellCtr) = Format(CDate(tblPayrollRun.SelectedRow.Cells(3).Text), "MM/dd/yyyy")
        CellCtr += 1
        xlWorkSheet.Range(CellLetterList(CellHdrCtr) & "4:" & CellLetterList(CellHdrCtr) & "5").Merge()
        xlWorkSheet.Cells(4, CellCtr) = "STATUS"
        CellCtr += 1
        'xlWorkSheet.Cells(4, CellCtr) = "Net Pay"

        ' ------------------------------------------------------------------------------------------------------------
        ' 
        ' ------------------------------------------------------------------------------------------------------------
        vSQL = "select distinct(EmpCode) as EmpCode, " _
& "(select FullName from tblEmployees where EmpCode=EmployeeCode) As FullName, " _
& "(select (select Name from tblDivision where id=DivisionId) As DivName " _
& "From tblEmployees Where EmpCode = EmployeeCode) As DivDescr, " _
& "(select (select Name from tblLocations where id=LocationId) as PayGName " _
& "From tblEmployees Where EmpCode = EmployeeCode) As GroupDescr, " _
& "(select AddressRegistered from tblEmployees where EmpCode=EmployeeCode) As EmpAddress, " _
& "(select TINNo from tblEmployees where EmpCode=EmployeeCode) As TINNo " _
& "from tblPayInstruction " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "order by DivDescr, FullName"
        cm.CommandText = vSQL
        CellRowCtr = 6
        rs = cm.ExecuteReader
        Do While rs.Read
            If BLCtr = 0 Or BLCtr = 1 Then
                BrkLine = rs("DivDescr")
            End If
            If BrkLine <> rs("DivDescr") Then
                CellRowCtr += 1
                BLCtr = 0
            End If
            xlWorkSheet.Range("A4:A5").Merge()
            xlWorkSheet.Range("B4:B5").Merge()
            xlWorkSheet.Range("C4:C5").Merge()
            xlWorkSheet.Range("D4:D5").Merge()
            xlWorkSheet.Range("E4:E5").Merge()
            xlWorkSheet.Range("F4:F5").Merge()
            xlWorkSheet.Cells(CellRowCtr, 1) = rs("FullName")
            xlWorkSheet.Cells(CellRowCtr, 2) = rs("EmpCode")
            xlWorkSheet.Cells(CellRowCtr, 3) = rs("DivDescr")
            xlWorkSheet.Cells(CellRowCtr, 4) = rs("GroupDescr")
            xlWorkSheet.Cells(CellRowCtr, 5) = rs("TINNo")
            xlWorkSheet.Cells(CellRowCtr, 6) = rs("EmpAddress")
            CellRowCtr += 1
            xlWorkSheet.Cells(CellRowCtr, 1) = rs("DivDescr")
            BLCtr += 1
            DivCtr += 1
            xlWorkSheet.Cells(CellRowCtr, 3) = BLCtr
        Loop
        rs.Close()
        CellRowCtr += 1
        xlWorkSheet.Cells(CellRowCtr, 1) = "Grand Total"
        xlWorkSheet.Cells(CellRowCtr, 3) = DivCtr - 1






        '-------------------------------------------------------------------------------------------------
        'EARNINGS VALUE
        '------------------------------------------------------------------------------------------------- 
        EarningsCodeTemp = EarningsCodeTemp.Substring(0, EarningsCodeTemp.Length - 1)
        EarningsCodeList = EarningsCodeTemp.Split(",")
        For i As Integer = 0 To EarningsCodeList.Length - 1
            CellCtr = 7
            CellRowCtr = 6
            BLCtr = 0
            DivCtr = 1
            vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select (select Name from tblDivision where id=DivisionId) As DivName " _
& "From tblEmployees Where a.EmpCode = EmployeeCode) As DivDescr, " _
& "(select sum(Amount) from tblPayInstruction b where a.EmpCode=b.EmpCode and b.PayElement='" _
& EarningsCodeList(i) & "' and b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as AmtVal " _
& "from tblPayInstruction a " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "order by DivDescr, FullName"
            'Response.Write(vSQL & "<br><br>")
            cm.CommandText = vSQL
            rs = cm.ExecuteReader
            Do While rs.Read
                If BLCtr = 0 Or BLCtr = 1 Then
                    BrkLine = rs("DivDescr")
                    SubTotal = 0
                End If
                If BrkLine <> rs("DivDescr") Then
                    CellRowCtr += 1
                    BLCtr = 0
                    SubTotal = 0
                End If

                '----------------------------------------------------------------------------------------------
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = Format(CDec(IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), "0.00")), "#,###,##0.00")
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
                TotalVal1 += IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), "0.00")
                SubTotal += IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), "0.00")
                CellRowCtr += 1
                '----------------------------------------------------------------------------------------------

                BLCtr += 1
                DivCtr += 1

                ' Build Sub Total Per Division 
                '----------------------------------------------------------------------------------------------
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = Format(CDec(SubTotal), "#,###,##0.00")
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
            Loop
            rs.Close()
            CellRowCtr += 1
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = Format(CDec(TotalVal1), "#,###,##0.00")
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
            TotalVal1 = 0
        Next




        '-------------------------------------------------------------------------------------------------
        'INPUT VAT AND WH-TAX VALUE
        '-------------------------------------------------------------------------------------------------
        vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select (select Name from tblDivision where id=DivisionId) As DivName " _
& "From tblEmployees Where a.EmpCode = EmployeeCode) As DivDescr, " _
& "(select InputVAT from tblPayrollSummary b where a.EmpCode=b.EmpCode and b.BatchNo='" _
& tblPayrollRun.SelectedRow.Cells(1).Text & "') as InputVat, " _
& "(Select WHTax from tblPayrollSummary b where a.EmpCode=b.EmpCode And b.BatchNo='" _
& tblPayrollRun.SelectedRow.Cells(1).Text & "') as WHTax " _
& "From tblPayInstruction a  " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "'  " _
& "Order By DivDescr, FullName"
        cm.CommandText = vSQL
        CellCtr = 7 + EarningsCodeList.Length
        CellRowCtr = 6
        BLCtr = 0
        DivCtr = 1
        SubTotal = 0
        SubTotal2 = 0
        rs = cm.ExecuteReader
        Do While rs.Read
            If BLCtr = 0 Or BLCtr = 1 Then
                BrkLine = rs("DivDescr")
            End If
            If BrkLine <> rs("DivDescr") Then
                CellRowCtr += 1
                BLCtr = 0
                SubTotal = 0
                SubTotal2 = 0
            End If

            '-------------------------------------------------------------------------------------------------
            xlWorkSheet.Cells(CellRowCtr, CellCtr) = IIf(Not IsDBNull(rs("InputVat")), rs("InputVat"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1) = IIf(Not IsDBNull(rs("WHTax")), rs("WHTax"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1).NumberFormat = "#,###,##0.00"
            TotalVal1 += IIf(Not IsDBNull(rs("InputVat")), rs("InputVat"), 0.00)
            TotalVal2 += IIf(Not IsDBNull(rs("WHTax")), rs("WHTax"), 0.00)
            SubTotal += IIf(Not IsDBNull(rs("InputVat")), rs("InputVat"), 0.00)
            SubTotal2 += IIf(Not IsDBNull(rs("WHTax")), rs("WHTax"), 0.00)
            CellRowCtr += 1
            '-------------------------------------------------------------------------------------------------

            BLCtr += 1
            DivCtr += 1

            ' Build Sub Total Per Division 
            '----------------------------------------------------------------------------------------------
            xlWorkSheet.Cells(CellRowCtr, CellCtr) = SubTotal
            xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1) = SubTotal2
            xlWorkSheet.Cells(CellRowCtr, CellCtr + 1).NumberFormat = "#,###,##0.00"
        Loop
        rs.Close()
        CellRowCtr += 1
        xlWorkSheet.Cells(CellRowCtr, CellCtr) = Format(CDec(TotalVal1), "#,###,##0.00")
        xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 1) = Format(CDec(TotalVal2), "#,###,##0.00")
        xlWorkSheet.Cells(CellRowCtr, CellCtr + 1).NumberFormat = "#,###,##0.00"
        TotalVal1 = 0
        TotalVal2 = 0
        CellCtr += 2



        ''-------------------------------------------------------------------------------------------------
        ''DEDUCTION VALUE
        ''-------------------------------------------------------------------------------------------------
        DeductionCodeTemp = DeductionCodeTemp.Substring(0, DeductionCodeTemp.Length - 1)
        DeductionCodeList = DeductionCodeTemp.Split(",")
        For i As Integer = 0 To DeductionCodeList.Length - 1
            vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select (select Name from tblDivision where id=DivisionId) As DivName " _
& "From tblEmployees Where a.EmpCode = EmployeeCode) As DivDescr, " _
& "(select sum(Amount) from tblPayInstruction b where a.EmpCode=b.EmpCode and b.PayElement='" _
& DeductionCodeList(i) & "' and b.BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as AmtVal " _
& "from tblPayInstruction a " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "order by DivDescr, FullName"
            cm.CommandText = vSQL
            CellCtr = 9 + EarningsCodeList.Length
            CellRowCtr = 6
            BLCtr = 0
            DivCtr = 1
            SubTotal = 0
            rs = cm.ExecuteReader
            Do While rs.Read
                If BLCtr = 0 Or BLCtr = 1 Then
                    BrkLine = rs("DivDescr")
                End If
                If BrkLine <> rs("DivDescr") Then
                    CellRowCtr += 1
                    BLCtr = 0
                    SubTotal = 0
                End If

                '-------------------------------------------------------------------------------------------------
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), 0.00)
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
                TotalVal1 += IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), 0.00)
                CellRowCtr += 1
                '-------------------------------------------------------------------------------------------------
                SubTotal += IIf(Not IsDBNull(rs("AmtVal")), rs("AmtVal"), 0.00)
                BLCtr += 1
                DivCtr += 1

                ' Build Sub Total Per Division 
                '----------------------------------------------------------------------------------------------
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = SubTotal
                xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
            Loop
            rs.Close()
            Session("CellCtr") = i + CellCtr
            CellRowCtr += 1
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr) = Format(CDec(TotalVal1), "#,###,##0.00")
            xlWorkSheet.Cells(CellRowCtr, i + CellCtr).NumberFormat = "#,###,##0.00"
            TotalVal1 = 0
        Next



        '-------------------------------------------------------------------------------------------------
        ' Total as of header
        '-------------------------------------------------------------------------------------------------
        vSQL = "select  distinct(EmpCode), " _
& "(select FullName from tblEmployees where a.EmpCode=EmployeeCode) As FullName, " _
& "(select (select Name from tblDivision where id=DivisionId) As DivName " _
& "From tblEmployees Where a.EmpCode = EmployeeCode) As DivDescr, " _
& "(Select NetPay from tblPayrollSummary b where a.EmpCode=b.EmpCode And b.BatchNo='" _
& tblPayrollRun.SelectedRow.Cells(1).Text & "') as NetPay " _
& "From tblPayInstruction a " _
& "Where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "Order By DivDescr, FullName"
        cm.CommandText = vSQL
        CellCtr = Session("CellCtr") + 1
        CellRowCtr = 6
        BLCtr = 0
        DivCtr = 1
        SubTotal = 0
        rs = cm.ExecuteReader
        Do While rs.Read
            If BLCtr = 0 Or BLCtr = 1 Then
                BrkLine = rs("DivDescr")
            End If
            If BrkLine <> rs("DivDescr") Then
                CellRowCtr += 1
                BLCtr = 0
                SubTotal = 0
            End If

            '----------------------------------------------------------------------------------------------
            xlWorkSheet.Cells(CellRowCtr, CellCtr) = IIf(Not IsDBNull(rs("NetPay")), rs("NetPay"), 0.00)
            xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
            TotalVal3 += IIf(Not IsDBNull(rs("NetPay")), rs("NetPay"), 0.00)
            CellRowCtr += 1
            '----------------------------------------------------------------------------------------------
            SubTotal += IIf(Not IsDBNull(rs("NetPay")), rs("NetPay"), 0.00)
            BLCtr += 1
            DivCtr += 1

            ' Build Sub Total Per Division 
            '----------------------------------------------------------------------------------------------
            xlWorkSheet.Cells(CellRowCtr, CellCtr) = SubTotal
            xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
        Loop
        rs.Close()
        CellRowCtr += 1
        xlWorkSheet.Cells(CellRowCtr, CellCtr) = CDec(TotalVal3)
        xlWorkSheet.Cells(CellRowCtr, CellCtr).NumberFormat = "#,###,##0.00"
        c.Close()
        c.Dispose()
        cm.Dispose()
        FileName = Format(Now(), "MMddyyyyHHmmss") & "-JVReport.xls"
        xlWorkBook.SaveAs(TargetPath & FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue)
        xlWorkBook.Close(True, misValue, misValue)
        xlApp.Quit()
        releaseObject(xlWorkSheet)
        releaseObject(xlWorkBook)
        releaseObject(xlApp)
        Response.Redirect("~/Downloads/Report/" & FileName)
    End Sub
    Private Sub BtnSubmitLock_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmitLock.ServerClick
        Dim LockCount As Integer = 0
        If BtnLock.InnerText = "Lock Payroll" Then
            vSQL = "select count(BatchNo) as Lock from tblPayInstructionHeader where DatePosted is not null and " _
& "PayDate='" & Format(CDate(tblPayrollRun.SelectedRow.Cells(3).Text), "MM/dd/yyyy") & "'"
            LockCount = GetRef(vSQL, 0)
            If LockCount > 0 Then
                ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The selected payroll date is already locked. \n\nNOTE: You can only lock one transaction per payroll date.');", True)
                Exit Sub
            End If
            vSQL = "update tblPayInstructionHeader set PostedBy='" & Session("uid") & "', DatePosted='" & Now & "' " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "'"
            CreateRecords(vSQL)
        Else
            vSQL = "update tblPayInstructionHeader set PostedBy=null, DatePosted=null " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "'"
            CreateRecords(vSQL)
        End If
        tblPayrollRun.SelectedIndex = -1
        BtnDownloadPayReg.Enabled = False
        BtnJVReport.Enabled = False
        BtnPost.Disabled = True
        BtnLock.Disabled = True
        BtnGenerateBankReport.Disabled = True
        GetPayrollRunList()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
    End Sub
    Private Sub BtnGenerateBankReport_ServerClick(sender As Object, e As EventArgs) Handles BtnGenerateBankReport.ServerClick
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
        xlWorkSheet.Cells(Ctr, 1) = "*Account Number"
        xlWorkSheet.Cells(Ctr, 2) = "First Name"
        xlWorkSheet.Cells(Ctr, 3) = "Middle Initial"
        xlWorkSheet.Cells(Ctr, 4) = "Last Name"
        xlWorkSheet.Cells(Ctr, 5) = "*Amount"
        xlWorkSheet.Cells(Ctr, 6) = "Employee Code"
        vSQL = "select EmpCode, NetPay, " _
& "(select FirstName from tblemployees where EmpCode=EmployeeCode) As FName, " _
& "(select MiddleName from tblemployees where EmpCode=EmployeeCode) as MName, " _
& "(Select LastName from tblemployees where EmpCode=EmployeeCode) As LName, " _
& "(select BankAccountNo from tblemployees where EmpCode=EmployeeCode) as AcctNo, " _
& "(select Active from tblemployees where EmpCode=EmployeeCode) As IsActive, " _
& "(select DateSeparated from tblemployees where EmpCode=EmployeeCode) As DateSeparated " _
& "from tblPayrollSummary " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' " _
& "and EmpCode in (select EmployeeCode from tblEmployees where Active=1) "
        cm.CommandText = vSQL
        Ctr = 2
        rs = cm.ExecuteReader
        Do While rs.Read
            xlWorkSheet.Cells(Ctr, 1).NumberFormat = "@"
            xlWorkSheet.Cells(Ctr, 1) = rs("AcctNo")
            xlWorkSheet.Cells(Ctr, 2) = rs("FName")
            xlWorkSheet.Cells(Ctr, 3) = rs("MName")
            xlWorkSheet.Cells(Ctr, 4) = rs("LName")
            xlWorkSheet.Cells(Ctr, 5) = rs("NetPay")
            xlWorkSheet.Cells(Ctr, 5).NumberFormat = "#,###,##0.00"
            xlWorkSheet.Cells(Ctr, 6) = rs("EmpCode")
            Ctr += 1
        Loop
        rs.Close()
        c.Close()
        c.Dispose()
        cm.Dispose()
        FileName = Format(Now(), "MMddyyyyHHmmss") & "-BankReport.xls"
        xlWorkBook.SaveAs(TargetPath & FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue)
        xlWorkBook.Close(True, misValue, misValue)
        xlApp.Quit()
        releaseObject(xlWorkSheet)
        releaseObject(xlWorkBook)
        releaseObject(xlApp)
        Response.Redirect("~/Downloads/Report/" & FileName)
    End Sub
    Private Sub BtnException_Click(sender As Object, e As EventArgs) Handles BtnException.Click
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
        DuplicateWageType = ""
        TagAsResigned = ""
        InActiveEmp = ""

        ' ----------------------------------------------------------------------------------------------------------
        vSQL = "select EmpCode,PayElement," _
& "(select FullName from tblEmployees where EmployeeCode=EmpCode) As FullName," _
& "(Select count(PayElement) from tblPayInstruction a " _
& "where a.PayElement=b.PayElement And a.EmpCode=b.EmpCode And BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "') as WTCtr," _
& "Amount " _
& "from tblPayInstruction b " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "'"
        'Response.Write(vSQL)
        cm.CommandText = vSQL
        Ctr = 2
        rs = cm.ExecuteReader
        Do While rs.Read
            If rs("WTCtr") > 1 Then
                DuplicateWageType += "<tr><td>" & rs("EmpCode") & "</td>" _
& "<td>" & rs("FullName") & "</td>" _
& "<td>" & rs("PayElement") & "</td>" _
& "<td class='text-right'>" & rs("Amount") & "</td></tr>"
            End If
        Loop
        rs.Close()
        If DuplicateWageType = "" Then
            DuplicateWageType = "<tr><td colspan='4'>No records found</td></tr>"
        End If
        ' ----------------------------------------------------------------------------------------------------------

        vSQL = "select distinct(EmpCode) as EmpCode," _
& "(select FullName from tblEmployees where EmpCode=EmployeeCode) As FullName," _
& "(select format(DateSeparated,'MM/dd/yyyy') from tblEmployees where EmpCode=EmployeeCode) as DateSeparated " _
& "from tblPayInstruction " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' and " _
& "EmpCode in (select EmployeeCode from tblEmployees where DateSeparated is not null)"
        cm.CommandText = vSQL
        Ctr = 2
        rs = cm.ExecuteReader
        Do While rs.Read
            TagAsResigned += "<tr><td>" & rs("EmpCode") & "</td>" _
& "<td>" & rs("FullName") & "</td>" _
& "<td>" & rs("DateSeparated") & "</td></tr>"
        Loop
        rs.Close()
        If TagAsResigned = "" Then
            TagAsResigned = "<tr><td colspan='3'>No records found</td></tr>"
        End If
        ' ----------------------------------------------------------------------------------------------------------

        vSQL = "select distinct(EmpCode) as EmpCode," _
& "(select FullName from tblEmployees where EmpCode=EmployeeCode) As FullName " _
& "from tblPayInstruction " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' and " _
& "EmpCode in (select EmployeeCode from tblEmployees where Active=0)"
        cm.CommandText = vSQL
        Ctr = 2
        rs = cm.ExecuteReader
        Do While rs.Read
            InActiveEmp += "<tr><td>" & rs("EmpCode") & "</td>" _
& "<td>" & rs("FullName") & "</td>" _
& "<td>InActive</td></tr>"
        Loop
        rs.Close()
        If InActiveEmp = "" Then
            InActiveEmp = "<tr><td colspan='3'>No records found</td></tr>"
        End If
        ' ----------------------------------------------------------------------------------------------------------

        vSQL = "select distinct(EmpCode) as EmpCode," _
& "(select FullName from tblEmployees where EmpCode=EmployeeCode) As FullName " _
& "from tblPayInstruction " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' and " _
& "EmpCode not in (select EmployeeCode from tblEmployees)"
        cm.CommandText = vSQL
        Ctr = 2
        rs = cm.ExecuteReader
        Do While rs.Read
            EmpNotFound += "<tr><td>" & rs("EmpCode") & "</td>" _
& "<td>No records reference found in employee master data</tr>"
        Loop
        rs.Close()
        If EmpNotFound = "" Then
            EmpNotFound = "<tr><td colspan='2'>No records found</td></tr>"
        End If
        ' ----------------------------------------------------------------------------------------------------------


        vSQL = "select distinct(PayElement) as Element " _
& "from tblPayInstruction " _
& "where BatchNo='" & tblPayrollRun.SelectedRow.Cells(1).Text & "' and " _
& "PayElement not in (select Code from tblPayElements)"
        cm.CommandText = vSQL
        Ctr = 2
        rs = cm.ExecuteReader
        Do While rs.Read
            WageTypeNotFound += "<tr><td>" & rs("Element") & "</td>" _
& "<td>No records reference found in wage type reference</tr>"
        Loop
        rs.Close()
        If WageTypeNotFound = "" Then
            WageTypeNotFound = "<tr><td colspan='2'>No records found</td></tr>"
        End If
        ' ----------------------------------------------------------------------------------------------------------
        c.Close()
        c.Dispose()
        cm.Dispose()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "$('#ExceptionReport').modal();", True)
    End Sub
    Private Sub BtnReUploadTempalte_ServerClick(sender As Object, e As EventArgs) Handles BtnReUploadTempalte.ServerClick
        Dim TargetFilenameRecurring As String = ""
        Dim TargetFilenameOneTime As String
        Dim OneTimeFileName As String = ""
        Dim BatchNo As String = tblPayrollRun.SelectedRow.Cells(1).Text
        If TxtReUploadOneTime.FileName <> "" Then
            TargetFilenameOneTime = Server.MapPath(".") & "\Uploaded\SystemInputFiles\" & Format(Now(), "MMddyyyyHHmmss") & "-PAYInstructionOneTime-" & TxtFileNameOneTime.FileName
            TxtReUploadOneTime.SaveAs(TargetFilenameOneTime)
            'ReadExcelData(TargetFilenameOneTime, "tblPayInstruction", BatchNo, "ONETIME", "ReUpload")
            OneTimeFileName = TxtReUploadOneTime.FileName
        Else
            OneTimeFileName = "None"
        End If
        GetPayrollRunList()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)

    End Sub
End Class
