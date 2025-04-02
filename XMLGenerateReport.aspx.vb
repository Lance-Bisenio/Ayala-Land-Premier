Imports System.Data
Imports System.IdentityModel.Protocols.WSTrust
Imports HelperClass
Partial Class XMLGenerateReport
    Inherits System.Web.UI.Page
    Dim vSQL As String = ""
    Dim vSQLTemp As String = ""

    Private Sub XMLGenerateReport_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Response.Write(Request.Item("EmpId") & " - " & Request.Item("BatchNo"))
        PostPayrollTransaction(Request.Item("EmpId"), Request.Item("BatchNo"), Request.Item("Ctr"))
    End Sub
    Private Sub PostPayrollTransaction(EmpId As String, BatchNo As String, Ctr As Integer)


        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim EmpList As String = ""
        Dim TaxableAmt As Decimal = 0

        Dim TotalEarning As Decimal = 0
        Dim TotalDeduction As Decimal = 0
        Dim InputVAT As Decimal = 0
        Dim GrandTotal As Decimal = 0
        Dim NetPay As Decimal = 0

        Dim PayDate As Date = Request.Item("PDate")
        Dim PayCutOffFromDate As Date
        Dim PayPayCutOffToDate As Date

        Dim TaxPercent As Decimal
        Dim IsNonVat As String
        Dim VatPercent As Decimal
        Dim PayElementList As String = ""

        c.ConnectionString = ConnStr

        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
            Exit Sub
        End Try


        ' ==================================================================================================================================================
        ' GET PAY INSTRUCTION HEADER
        ' ==================================================================================================================================================
        vSQL = "select PayDate,CutOffFromDate,CutOffToDate " _
            & "from tblPayInstructionHeader where BatchNo='" & BatchNo & "'"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader

        If rs.Read Then
            PayDate = Format(CDate(rs("PayDate")), "MM/dd/yyyy")
            PayCutOffFromDate = Format(CDate(rs("CutOffFromDate")), "MM/dd/yyyy")
            PayPayCutOffToDate = Format(CDate(rs("CutOffToDate")), "MM/dd/yyyy")
        End If

        rs.Close()



        ' ==================================================================================================================================================
        ' GET RECURRING PAY ELEMENT
        ' ==================================================================================================================================================
        vSQL = "select IsEarnings, PayElementId, Amount, ValidFrom, ValidTo from tblPayInstructionRecurring " _
            & "WHERE " _
                & "(ValidFrom <= '" & Format(CDate(PayCutOffFromDate), "MM/dd/yyyy") & "' and " _
                    & "ValidTo >='" & Format(CDate(PayPayCutOffToDate), "MM/dd/yyyy") & "' and EmpCode='" & EmpId & "' and IsActive=1) or " _
                & "(ValidFrom between '" & Format(CDate(PayCutOffFromDate), "MM/dd/yyyy") & "' and '" _
                    & Format(CDate(PayPayCutOffToDate), "MM/dd/yyyy") & "' and EmpCode='" & EmpId & "' and IsActive=1) or " _
                & "(ValidTo between '" & Format(CDate(PayCutOffFromDate), "MM/dd/yyyy") & "' and '" _
                    & Format(CDate(PayPayCutOffToDate), "MM/dd/yyyy") & "' and EmpCode='" & EmpId & "' and IsActive=1)  "
        '& "EmpCode='" & EmpId & "' and IsActive=1"

        'Response.Write(vSQL)

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            PayElementList += "'" & rs("PayElementId") & "',"


            vSQLTemp += "('" & EmpId & "','" & BatchNo & "'," & rs("IsEarnings") & ",'" & rs("PayElementId").ToString & "','" & CDec(rs("Amount")) / 2 & "'," _
                & "'" & rs("ValidFrom") & "','" & rs("ValidTo") & "','" & Session("uid") & "','" & Now & "'),"
        Loop

        rs.Close()


        vSQL = "delete from tblPayInstruction where BatchNo='" & BatchNo & "' and ElementType=1 and " _
            & "EmpCode not in (select EmployeeCode from tblEmployees WHERE DateSeparated is null and Active=1 and CustomField1 is null)"
        CreateRecords(vSQL)

        vSQL = "delete from tblPayInstruction where BatchNo='" & BatchNo & "' and ElementType=1 and " _
            & "EmpCode='" & EmpId & "'"
        CreateRecords(vSQL)

        If vSQLTemp.Trim <> "" Then
            vSQL = "insert into tblPayInstruction (EmpCode, BatchNo, ElementType, PayElement, Amount, ValidFrom, ValidTo, CreatedBy, DateCreated) values "
            vSQLTemp = vSQLTemp.Substring(0, vSQLTemp.Length - 1)
            vSQL += vSQLTemp
            CreateRecords(vSQL)
        End If


        ' ==================================================================================================================================================
        vSQLTemp = ""
        vSQL = "select IsEarnings, PayElementId, Amount, ValidFrom, ValidTo from tblPayInstructionRecurring " _
            & "WHERE EmpCode='" & EmpId & "' AND " _
                & "ValidFrom is null AND " _
                & "ValidTo is null and IsActive=1"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            PayElementList += "'" & rs("PayElementId") & "',"

            vSQLTemp += "('" & EmpId & "','" & BatchNo & "'," & rs("IsEarnings") & ",'" & rs("PayElementId").ToString & "','" & CDec(rs("Amount")) / 2 & "'," _
                & "'" & rs("ValidFrom") & "','" & rs("ValidTo") & "','" & Session("uid") & "','" & Now & "'),"
        Loop
        rs.Close()

        If vSQLTemp.Trim <> "" Then
            vSQL = "insert into tblPayInstruction (EmpCode, BatchNo, ElementType, PayElement, Amount, ValidFrom, ValidTo, CreatedBy, DateCreated) values "
            vSQLTemp = vSQLTemp.Substring(0, vSQLTemp.Length - 1)
            vSQL += vSQLTemp
            CreateRecords(vSQL)
        End If



        ' ==================================================================================================================================================
        ' GET MONTHLY BASIC, TAX AND VAT DETAILS
        ' ==================================================================================================================================================
        vSQL = "select MonthlyRate, TINNo, FullName, EmployeeCode, " _
            & "(select top 1 TaxPercent from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode order by id desc) as TaxPercent, " _
            & "(select top 1 IsNonVat from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode order by id desc) as IsNonVat, " _
            & "(select top 1 VatPercent from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode order by id desc) as VatPercent " _
            & "from tblEmployees b where EmployeeCode='" & EmpId & "'"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader

        If rs.Read Then
            TaxPercent = Format(CDec(rs("TaxPercent")), "##0.00")
            IsNonVat = rs("IsNonVat")
            VatPercent = Format(CDec(rs("VatPercent")), "##0.00")

            vSQL = "insert into tblPayInstruction (EmpCode, BatchNo, ElementType, PayElement, Amount, ValidFrom, ValidTo, CreatedBy, DateCreated) values " _
                & "('" & EmpId & "','" & BatchNo & "',1,'BASIC','" & CDec(rs("MonthlyRate")) / 2 & "'," _
                & "null,null,'" & Session("uid") & "','" & Now & "')"
            CreateRecords(vSQL)
            'Response.Write(vSQL)
        End If

        rs.Close()

        ' ==================================================================================================================================================
        ' EARNING 
        ' ==================================================================================================================================================

        vSQL = "select (select Name from tblPayElements where Code=PayElement) as ElementName, " _
            & "(select IsTaxable from tblPayElements where Code=PayElement) as IsTaxable," _
            & "Amount, ElementType " _
                & "from tblPayInstruction where " _
                & "BatchNo='" & BatchNo & "' and " _
                & "EmpCode='" & EmpId & "' and " _
                & "PayElement in (select Code from tblPayElements where IsEarning=0)"

        '& "ElementType=1 and " _

        'Response.Write(vSQL)
        cm.CommandText = vSQL

        rs = cm.ExecuteReader
        Do While rs.Read
            'EarningRecurring
            If Not IsDBNull(rs("IsTaxable")) = 1 Then
                TaxableAmt += rs("Amount")
            End If

            TotalEarning += CDec(rs("Amount"))
        Loop

        rs.Close()

        ' ==================================================================================================================================================
        ' DEDUCTION
        ' ==================================================================================================================================================

        vSQL = "select (select Name from tblPayElements where Code=PayElement) as ElementName, " _
            & "(select IsTaxable from tblPayElements where Code=PayElement) as IsTaxable, " _
            & "Amount, ElementType " _
                & "from tblPayInstruction where " _
                & "BatchNo='" & BatchNo & "' and " _
                & "EmpCode='" & EmpId & "' and " _
                & "PayElement in (select Code from tblPayElements where IsEarning=1 and Active=1)"

        cm.CommandText = vSQL

        rs = cm.ExecuteReader
        Do While rs.Read
            'DeductionRecurring 
            TotalDeduction += CDec(rs("Amount"))
        Loop
        rs.Close()


        ' ==================================================================================================================================================
        ' ==================================================================================================================================================

        'Total Taxable: 


        'Input VAT:
        InputVAT = TotalEarning * VatPercent

        'Grand Total:
        GrandTotal = TotalEarning + InputVAT


        'Withholding Tax (EWT):
        TaxableAmt = TaxableAmt * TaxPercent

        'Total Duduction: 

        'Net Amount:
        NetPay = GrandTotal - (TotalDeduction + TaxableAmt)
        ' ==================================================================================================================================================
        ' ==================================================================================================================================================

        vSQL = "delete from tblPayrollSummary where EmpCode='" & EmpId & "' and BatchNo='" & BatchNo & "'"
        CreateRecords(vSQL)

        vSQL = "insert into tblPayrollSummary " _
            & "(EmpCode,BatchNo,VatPercent,WTaxPercent,TotalEarning,TotalDeduction,TotalTaxable,InputVAT," _
            & "GrandTotal,WHTax,NetPay,CreatedBy,DateCreated) values ('" _
            & EmpId & "','" & BatchNo & "'," & TaxPercent & "," & VatPercent & ",'" & TotalEarning & "','" & TotalDeduction & "','" _
            & TotalEarning & "','" & InputVAT & "','" & GrandTotal & "','" & TaxableAmt & "','" & NetPay & "','" _
            & Session("uid") & "','" & Now & "')"

        CreateRecords(vSQL)

        vSQL = "update tblPayInstructionHeader set ProcessBy='" & Session("uid") & "', DateProcess='" & Now & "' " _
            & "where BatchNo='" & BatchNo & "'"
        CreateRecords(vSQL)


        c.Close()
        c.Dispose()
        cm.Dispose()

        Session("Ctr") += 1

        Response.Write("Processing payroll " & Session("Ctr") & " of ")

    End Sub


End Class
