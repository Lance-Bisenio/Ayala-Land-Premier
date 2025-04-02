Imports System.Data
Imports HelperClass
Imports System.Net.Mail

Partial Class DocManagement
    Inherits System.Web.UI.Page
    Dim vScript As String = ""
    Dim vSQL As String = ""
    Public DataLogs As String = ""

    Private Sub DocManagement_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session("uid") = "18041604" '"19060302"

        If Session("uid") = "" Then
            Response.Redirect("~/Login")
            Exit Sub
        End If

        If Not IsPostBack Then
            Session("UserType") = 0
            Session("IsApprover") = 0
            Session("CanViewController") = ""
            Session("CanViewAuthor") = ""
            Session("TranID") = ""
            Session("UserEmail") = ""

            Dim CanViewAsAdmin As Integer = 0
            Dim CanViewAsAuthor As Integer = 0
            Dim CanViewAsController As Integer = 0
            Dim UserType As Integer = 0


            ' =============================================================================
            ' Check the user access if administrator
            ' Property_Value='8000'
            ' =============================================================================
            vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='8000' and User_Id='" & Session("uid") & "'"
            CanViewAsAdmin = GetRef(vSQL, 0)
            UserType = IIf(CanViewAsAdmin = 1, 1, 0)


            ' =============================================================================
            ' Check the user access if an Author
            ' Property_Value='20'
            ' =============================================================================
            If UserType = 0 Then
                vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='20' and Property='DOCOWNERACCESS' and User_Id='" & Session("uid") & "'"
                CanViewAsAuthor = GetRef(vSQL, 0)
                UserType = IIf(CanViewAsAuthor = 1, 2, 0)
            End If


            ' =============================================================================
            ' Check the user access if a Document Controller
            ' Property_Value='1000'
            ' =============================================================================
            If UserType = 0 Then
                vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='1000' and Property='DOCOWNERACCESS' and User_Id='" & Session("uid") & "'"
                CanViewAsController = GetRef(vSQL, 0)
                UserType = IIf(CanViewAsController = 1, 3, 0)
            End If

            If UserType = 1 Or UserType = 0 Or UserType = 3 Then
                BuildCombo("select Type_Cd, Descr from dm_document_type order by Descr", CmdDocType)
                CmdDocType.SelectedValue = "28"

                BuildCombo("select Type_Cd, Descr from dm_document_type order by Descr", CmdRefDocType)

            Else
                BuildCombo("select Type_Cd, Descr from dm_document_type " _
                    & "where Type_Cd in (select Property_Value from rights_list where user_id='" & Session("uid") & "' and Property='DOCTYPEFILTER') " _
                    & "order by Descr", CmdDocType)

                BuildCombo("select Type_Cd, Descr from dm_document_type " _
                    & "where Type_Cd in (select Property_Value from rights_list where user_id='" & Session("uid") & "' and Property='DOCTYPEFILTER') " _
                    & "order by Descr", CmdRefDocType)
            End If



            ' =============================================================================
            ' Check the user access if a Reviewer or Apporver
            ' =============================================================================
            If UserType = 0 Then
                CheckAccess()
                UserType = IIf(Session("IsApprover") = 1, 4, 0)
            End If



            'Response.Write(UserType)



            BuildCombo("select PolicyCd, Remarks from PolicyList where DocType_id='" & CmdDocType.SelectedValue & "' order by Remarks", CmdDLDocList)
            CmdDLDocList.Items.Add("All")
            CmdDLDocList.SelectedValue = "All"

            If UserType = 0 Then
                Response.Redirect("~/AccessDenied")
            End If

            GetAllDocumentController()

            Session("UserEmail") = GetRef("select Emp_Email from employee_master where Emp_Cd='" & Session("uid") & "'", "")

            'Response.Write("Controller List: " & Session("ControllerEmailList") & "<br>") 
            Response.Write("UserType: " & Session("UserEmail") & "<br>" & UserType)

            Select Case UserType
                Case 1, 3 'Admin
                    BtnAdd.Disabled = False
                    BtnUpdate.Disabled = False
                    BtnDelete.Disabled = False
                    BtnGenReport.Disabled = False

                    Session("CanViewController") = "YES"
                Case 2 'Document Author  
                    BtnAdd.Disabled = False
                    BtnUpdate.Disabled = False
                    BtnDelete.Disabled = True

                    BtnForCorrection.Visible = False
                    BtnApproved.Visible = False

                    Session("CanViewAuthor") = "YES"
                Case 4 'Reviewer or Approver
            End Select


            Session("UserType") = UserType




            If UserType = 2 Then 'Document Author  
                BuildCombo("Select Group_Id, Descr from emp_group_ref " _
                        & "where Group_Id in (select Property_Value from rights_list where Property='DOCGROUPACCESS' and " _
                            & "User_Id='" & Session("uid") & "') " _
                        & "order by Descr", CmdDocOwner) ' and Property_Value not in ('20','21'))


                BuildCombo("select Group_Id, Descr from emp_group_ref " _
                    & "where Group_Id in (select Property_Value from rights_list where Property='DOCGROUPACCESS' and " _
                        & "User_Id='" & Session("uid") & "') " _
                    & "order by Descr", CmdRefDocOwner)
            Else
                BuildCombo("select Group_Id, Descr from emp_group_ref order by Descr", CmdDocOwner)
                CmdDocOwner.Items.Add("All")
                CmdDocOwner.SelectedValue = "All"

                BuildCombo("select Group_Id, Descr from emp_group_ref order by Descr", CmdRefDocOwner)
            End If





            BuildCombo("select Emp_Cd, Emp_Fname+' '+ Emp_Lname  from employee_master where Date_Resign is null order by Emp_Fname", CmdRefForReview)
            CmdRefForReview.Items.Add("None")
            CmdRefForReview.Items.Add("1001")
            CmdRefForReview.SelectedValue = "1001"

            BuildCombo("select Emp_Cd, Emp_Fname+' '+ Emp_Lname  from employee_master where Date_Resign is null order by Emp_Fname", CmdRefForApproval)
            CmdRefForApproval.Items.Add("None")
            CmdRefForApproval.Items.Add("1002")
            CmdRefForApproval.SelectedValue = "1002"

            BuildCombo("select DeptCd, Descr from emp_department_ref order by Descr", CmdDLDept)
            CmdDLDept.Items.Add("All")
            CmdDLDept.SelectedValue = "All"

            BuildCombo("select Status_Cd, Descr from DocumentStatus order by Descr", CmdDocStatus)




            GetDocumentList()
        End If

    End Sub

    Private Sub GetAllDocumentController()

        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim ControllerList As String = ""

        c.ConnectionString = ConnStr

        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        ' =============================================================================
        ' Property_Value='1000' is the Document Controller
        ' =============================================================================
        vSQL = "select User_Id, " _
                & "(select Emp_Email from employee_master where Emp_Cd=User_Id) As EmailAdd from rights_list " _
                & "where Property_Value='1000' and Property='DOCOWNERACCESS'"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            ControllerList += rs("EmailAdd") & ","
        Loop
        rs.Close()

        c.Close()
        c.Dispose()
        cm.Dispose()

        ControllerList = "btgqa@bposerve.com," & ControllerList

        Session("ControllerEmailList") = ControllerList.Substring(0, ControllerList.Length - 1)

    End Sub

    Private Sub CheckAccess()
        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim DocList As String = ""
        Dim SubSQL As String = ""

        c.ConnectionString = ConnStr

        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        Session("IsApprover") = 0

        If Session("IsApprover") = 0 Then
            vSQL = "select distinct(PolicyCd) as PolCd, " _
                & "(Select top 1 StatusId from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) As StatusId, " _
                & "(select top 1 Approver from DocumentAppover a where a.PolicyCd=b.PolicyCd And OrderBy=1 order by TranId desc) as Review, " _
                & "(select top 1 Approver from DocumentAppover a where a.PolicyCd=b.PolicyCd And OrderBy=2 order by TranId desc) as Approve " _
                & "from DocumentLedger b " _
                & "where PolicyCd is not null "

            SubSQL = vSQL
            'Response.Write(vSQL)
            cm.CommandText = vSQL

            rs = cm.ExecuteReader
            Do While rs.Read

                If Not IsDBNull(rs("Review")) Or Not IsDBNull(rs("Approve")) Then

                    If rs("StatusId") = 15 And Session("uid") = rs("Review").ToString Or
                        rs("StatusId") = 16 And Session("uid") = rs("Approve").ToString Then

                        Session("IsApprover") = 1

                    End If
                End If
            Loop
            rs.Close()
        End If

        'Response.Write(vSQL)
        'Session("IsApprover") = 1
        'Exit Sub

        If Session("IsApprover") = 0 Then
            Exit Sub
        End If


        vSQL = SubSQL

        vSQL += "and DocTypeId=" & CmdDocType.SelectedValue

        cm.CommandText = vSQL

        rs = cm.ExecuteReader
        Do While rs.Read

            If Not IsDBNull(rs("Review")) Or Not IsDBNull(rs("Approve")) Then
                If rs("StatusId") = 15 And Session("uid") = rs("Review").ToString Or rs("StatusId") = 16 And Session("uid") = rs("Approve").ToString Then
                    DocList += "'" & rs("PolCd") & "',"
                End If
            End If
        Loop

        If DocList = "" Then
            Session("DocList") = "'000',"
        Else
            Session("DocList") = DocList
        End If


        rs.Close()

        c.Close()
        c.Dispose()
        cm.Dispose()
    End Sub

    Private Sub GetDocumentList()
        Dim c As New SqlClient.SqlConnection
        Dim da As SqlClient.SqlDataAdapter
        Dim ds As New DataSet
        Dim vFilter As String = ""
        Dim vTableName As String = ""
        Dim vSQL As String = ""

        c.ConnectionString = ConnStr

        If Session("IsApprover") = 1 And Session("DocList") <> "" Then
            vFilter = " PolicyCd in (" & Session("DocList").ToString.Substring(0, Session("DocList").ToString.Length - 1) & ") "
        Else
            vFilter = " PolicyCd Is Not null "
        End If

        If CmdDocType.SelectedValue <> "" Then
            vFilter += " and DocTypeId=" & CmdDocType.SelectedValue
        End If

        If CmdDocOwner.SelectedValue <> "All" Then
            vFilter += " and Group_id=" & CmdDocOwner.SelectedValue
        End If


        'If TxtKeywords.Value.Trim <> "" Then
        '    vFilter += " And Remarks Like '%" & TxtKeywords.Value.Trim & "%'"
        'End If

        vSQL = "select distinct(PolicyCd), " _
            & "(select top 1 TranId from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) As TranId, " _
            & "(select top 1 Descr from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) As vDescr, " _
            & "(select top 1 Remarks from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) as Remarks, " _
            & "(Select top 1 PolicyFileLocation from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) As PolicyFileLocation, " _
            & "(select top 1 FileLocation from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) as FileLocation, " _
            & "(Select top 1 Group_Id from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) As Group_Id, " _
            & "(select top 1 DocTypeId from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) as DocTypeId, " _
            & "(Select top 1 StatusId from DocumentLedger a where a.PolicyCd=b.PolicyCd order by TranId desc) As StatusId, " _
            & "(select top 1 Approver from DocumentAppover a where a.PolicyCd=b.PolicyCd And OrderBy=1 order by TranId desc) as Review, " _
            & "(select top 1 Approver from DocumentAppover a where a.PolicyCd=b.PolicyCd And OrderBy=2 order by TranId desc) as Approve, " _
            & "(select GroupCd from emp_group_ref a where a.Group_Id=b.Group_Id) as Owner  " _
            & "from DocumentLedger b " _
            & "where " & vFilter _
            & " order by vDescr "

        'Response.Write("<br><br>" & vSQL)
        da = New SqlClient.SqlDataAdapter(vSQL, c)

        da.Fill(ds, "tblEmployees")
        tblDocList.DataSource = ds.Tables("tblEmployees")
        tblDocList.DataBind()

        da.Dispose()
        ds.Dispose()

        lblFilename.Text = "Total Documents: " & tblDocList.DataSource.Rows.Count
    End Sub

    Function GetStatusName(StatusId As Integer) As String

        Dim ReturnVal As String = ""
        ReturnVal = GetRef("select Descr from DocumentStatus where Status_Cd=" & StatusId, "")
        Return ReturnVal

    End Function

    Private Sub CmdDocType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmdDocType.SelectedIndexChanged
        GetDocumentList()
    End Sub

    Private Sub CmdDocOwner_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmdDocOwner.SelectedIndexChanged
        GetDocumentList()
    End Sub

    Private Sub tblDocList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblDocList.SelectedIndexChanged
        GetDocumentHistory()
    End Sub

    Private Sub GetDocumentHistory()
        Dim vFileName As String = "Uploaded/BPOI/Policy/" & tblDocList.SelectedRow.Cells(7).Text & "?page=hsn#toolbar=0"
        'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "previewfile(""" & vFileName & """);", True)

        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim Remarks As String = ""
        Dim Approver As String = ""
        Dim Ctr As Integer = 1
        Dim ApproverLabel As String = ""
        Dim ApproverEmail As String = ""
        Dim BtnList As String = ""

        Session("ReviewEmail") = ""
        Session("ApproverEmail") = ""

        c.ConnectionString = ConnStr


        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try


        Session("TranID") = tblDocList.SelectedRow.Cells(3).Text
        CmdRefDocType.SelectedValue = tblDocList.SelectedRow.Cells(5).Text
        CmdRefDocOwner.SelectedValue = tblDocList.SelectedRow.Cells(4).Text
        TxtDescr.Value = tblDocList.SelectedRow.Cells(2).Text.Trim

        Session("PolicyId") = tblDocList.SelectedRow.Cells(1).Text
        Session("PolicyFileLocation") = tblDocList.SelectedRow.Cells(7).Text
        Session("FileLocation") = tblDocList.SelectedRow.Cells(8).Text
        Session("DocStatus") = tblDocList.SelectedRow.Cells(9).Text
        TxtRemarks.Text = tblDocList.SelectedRow.Cells(10).Text.ToString.Trim.Replace("&nbsp;", "")



        CmdDocStatus.SelectedValue = tblDocList.SelectedRow.Cells(9).Text

        If Session("CanViewController") = "YES" Then
            TxtKeyChanges.ReadOnly = True
        End If


        Try
            CmdRefForReview.SelectedValue = tblDocList.SelectedRow.Cells(11).Text
        Catch ex As Exception
            CmdRefForReview.SelectedValue = "None"
        End Try

        Try
            CmdRefForApproval.SelectedValue = tblDocList.SelectedRow.Cells(12).Text
        Catch ex As Exception
            CmdRefForApproval.SelectedValue = "None"
        End Try

        ' =============================================================================
        ' Validate the selected document if New or Old
        ' =============================================================================
        vSQL = "select count(PolicyCd) as IsPublish " _
            & "from DocumentLedger where PolicyCd='" & tblDocList.SelectedRow.Cells(1).Text & "' and StatusId=11"

        Session("PublishCnt") = GetRef(vSQL, 0)
        ' =============================================================================
        ' GET Document Author FullName and Email address
        ' =============================================================================
        vSQL = "select top 1 PolicyCd, KeyNote, " _
            & "(select Emp_Fname +' '+ Emp_Lname from employee_master where Emp_Cd=CreatedBy) as FullName, " _
            & "(select Emp_email from employee_master where Emp_Cd=CreatedBy) as EmailAdd " _
            & "from DocumentLedger where PolicyCd='" & tblDocList.SelectedRow.Cells(1).Text & "' and StatusId=35 order by TranId desc"

        'Response.Write(vSQL)

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        If rs.Read Then
            Session("AuthorEmailadd") = rs("EmailAdd") & ""
            Session("AuthorFullName") = rs("FullName") & ""
            Session("KeyNote") = rs("KeyNote") & ""
        Else
            Session("AuthorEmailadd") = ""
            Session("AuthorFullName") = ""
            Session("KeyNote") = ""
        End If
        rs.Close()

        TxtKeyChanges.Text = Session("KeyNote")
        TxtAuthorKeyNote.Text = Session("KeyNote")

        ' =============================================================================
        ' GET transantion history
        ' =============================================================================
        vSQL = "select TranId, PolicyCd, Descr, Remarks, PolicyFileLocation, FileLocation, Group_Id, DocTypeId, StatusId, DateCreated," _
            & "(select Emp_Fname+' '+Emp_Lname from employee_master where Emp_Cd=CreatedBy) as FullName," _
            & "(select Descr from DocumentStatus where Status_Cd=StatusId) as DocStatus," _
            & "(select GroupCd from emp_group_ref a where a.Group_Id=b.Group_Id) as Owner  " _
            & "from DocumentLedger b where PolicyCd='" & Session("PolicyId") & "' order by TranId"

        'Response.Write(vSQL)

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read

            vFileName = "Uploaded/BPOI/Policy/" & rs("PolicyFileLocation") & "?page=hsn#toolbar=0"
            Remarks = Security.SecurityElement.Escape(rs("Remarks").ToString.Trim)

            DataLogs += "<tr>"
            DataLogs += "<td>" & Ctr & "</td>"
            DataLogs += "<td>" & rs("PolicyCd") & "</td>"
            DataLogs += "<td>" & rs("Descr") & "</td>"
            DataLogs += "<td style='width:400px;'>" & rs("Remarks") & "</td>"
            DataLogs += "<td>" & rs("FullName") & "</td>"
            DataLogs += "<td>" & rs("DateCreated") & "</td>"
            DataLogs += "<td>" & rs("DocStatus") & "</td>"

            DataLogs += "<td>"

            DataLogs += ApproverDetails(15, rs("TranId"))
            DataLogs += ApproverDetails(16, rs("TranId"))


            DataLogs += "</td>"

            DataLogs += "<td><input type='button' id='Btn" & Ctr & "' class='btn btn-sm btn-primary' value='View' " _
                & "onclick='ViewDocument(""" & vFileName & """,""" & Remarks.ToString.Replace(vbCr, "").Replace(vbLf, "") & """)'></td>"
            DataLogs += "</tr>"

            BtnList += "$('#Btn" & Ctr - 1 & "').hide();"

            ApproverLabel = ""
            Ctr += 1

        Loop

        rs.Close()

        If Session("IsApprover") = 1 Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", BtnList, True)
        End If


        TxtNewRemarks.Text = ""

        c.Close()
        c.Dispose()
        cm.Dispose()

    End Sub

    Function ApproverDetails(ApproverType As String, TranId As Int64) As String
        Dim Val As String = ""

        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        c.ConnectionString = ConnStr

        Try
            c.Open()
            cm.Connection = c
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Function
        End Try

        DataLogs += ""

        vSQL = "select " _
            & "(select Emp_Fname+' '+Emp_Lname from employee_master where Approver=Emp_Cd) as EName, " _
            & "(select Emp_Email from employee_master where Approver=Emp_Cd) as EEmail " _
                & "from DocumentAppover where StatusId=" & ApproverType & " and DocTranId=" & TranId

        'Response.Write(vSQL)

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        If rs.Read Then
            If Not IsDBNull(rs("EName")) And ApproverType = 15 Then
                Val = "For Review: " & rs("EName") & "<br>"
                Session("ReviewEmail") = rs("EEmail")
                Session("ReviewFullName") = rs("EName")
            End If

            If Not IsDBNull(rs("EName")) And ApproverType = 16 Then
                Val = "For Approval: " & rs("EName")
                Session("ApproverEmail") = rs("EEmail")
                Session("ApproverFullName") = rs("EName")
            End If
        Else
            Val = ""
        End If

        rs.Close()

        c.Close()
        c.Dispose()
        cm.Dispose()

        Return Val
    End Function

    Private Sub BtnSubmit_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmit.ServerClick

        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        Dim PolicyCtr As Integer = 0
        Dim CurrentDate As String = Format(Now, "MMddyyyyHHmmss")
        Dim DocId As String
        Dim DocTranId As Int16 = 0
        Dim TargetFilename As String
        Dim ControllerEmailList As String = ""

        If TxtDescr.Value.Trim = "" Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Please enter document description.');", True)
            Exit Sub
        End If

        If TxtFileName.FileName = "" Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Please select file to upload.');", True)
            Exit Sub
        End If

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        DocId = CurrentDate

        vSQL = " insert into DocumentLedger values ('" _
            & DocId & "','" & TxtDescr.Value.Trim & "','" & TxtRemarks.Text.ToString.Trim & "','" & DocId & ".pdf','" _
            & Session("uid") & "','" & Now() & "','" & TxtFileName.FileName & "','" _
            & CmdRefDocOwner.SelectedValue & "','','" & CmdRefDocType.SelectedValue & "',35,'" & TxtKeyChanges.Text.Replace("'", " ") & "')"

        cm.Connection = c
        cm.CommandText = vSQL

        Try
            cm.ExecuteNonQuery()

            'vSQL = "select TranId from DocumentLedger where PolicyCd='" & DocId & "'"
            'DocTranId = GetRef(vSQL, 0)

            'If CmdRefForReview.SelectedValue <> "None" Then
            '    vSQL = "insert into DocumentAppover values (" & DocTranId & ",'" & DocId & "','" & Session("uid") & "','" & Now & "'," _
            '        & CmdRefDocOwner.SelectedValue & "," & CmdRefDocType.SelectedValue & ",15,1,'" & CmdRefForReview.SelectedValue & "')"
            '    CreateRecords(vSQL)
            'End If

            'If CmdRefForApproval.SelectedValue <> "None" Then
            '    vSQL = "insert into DocumentAppover values (" & DocTranId & ",'" & DocId & "','" & Session("uid") & "','" & Now & "'," _
            '        & CmdRefDocOwner.SelectedValue & "," & CmdRefDocType.SelectedValue & ",16,2,'" & CmdRefForApproval.SelectedValue & "')"
            '    CreateRecords(vSQL)
            'End If

            ' Get Controller email list
            vSQL = "select User_Id, " _
                & "(select Emp_Email from employee_master where Emp_Cd=User_Id) As EmailAdd from rights_list " _
                & "where Property_Value='1000' and Property='DOCOWNERACCESS'"

            cm.CommandText = vSQL
            rs = cm.ExecuteReader
            Do While rs.Read
                ControllerEmailList += rs("EmailAdd") & ","
            Loop
            rs.Close()

            SendEmail(ControllerEmailList.Substring(0, ControllerEmailList.Length - 1), "Document Controller", 35)

            TargetFilename = Server.MapPath(".") & "\Uploaded\BPOI\Policy\" & DocId & ".pdf"
            TxtFileName.SaveAs(TargetFilename)

            tblDocList.SelectedIndex = -1
            GetDocumentList()

            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)

        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('SQL error:" & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');", True)
        Finally
            c.Close()
            c.Dispose()
            cm.Dispose()
        End Try


    End Sub

    Private Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        Session("TranID") = ""
        tblDocList.SelectedIndex = -1

        If Session("IsApprover") = 1 Then
            CheckAccess()
        End If

        GetDocumentList()
        'BtnIAgree.Visible = False

        BuildCombo("select PolicyCd, Remarks from PolicyList where DocType_id='" & CmdDocType.SelectedValue & "' order by Remarks", CmdDLDocList)
        CmdDLDocList.Items.Add("All")
        CmdDLDocList.SelectedValue = "All"

    End Sub

    Private Sub BtnUpdatePolicy_ServerClick(sender As Object, e As EventArgs) Handles BtnUpdatePolicy.ServerClick
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        Dim ControllerEmailList As String = ""

        Dim PolicyCtr As Integer = 0
        Dim CurrentDate As String = Format(Now, "MMddyyyyHHmmss")
        Dim DocId As String
        Dim DocTranId As Int64 = 0
        Dim TargetFilename As String

        Dim ReviewerEmail As String = ""
        Dim ApproverEmail As String = ""

        If TxtDescr.Value.Trim = "" Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Please enter document description.');", True)
            Exit Sub
        End If

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try



        If Session("CanViewAuthor") = "YES" Then
            CmdDocStatus.SelectedValue = 35
        End If

        DocId = CurrentDate

        vSQL = " insert into DocumentLedger values ('" & Session("PolicyId") & "','" _
            & TxtDescr.Value.Trim & "','" & TxtRemarks.Text.Trim & "',"


        If TxtFileName.FileName = "" Then
            vSQL += "'" & Session("PolicyFileLocation") & "',"
        Else
            vSQL += "'" & DocId & ".pdf',"
        End If

        vSQL += "'" & Session("uid") & "','" & Now() & "',"

        If TxtFileName.FileName = "" Then
            vSQL += "'" & Session("FileLocation") & "',"
        Else
            vSQL += "'" & TxtFileName.FileName & "',"
        End If

        vSQL += "'" & CmdRefDocOwner.SelectedValue & " ','','" & CmdRefDocType.SelectedValue & "'," _
            & CmdDocStatus.SelectedValue & ",'" & TxtKeyChanges.Text.Replace("'", " ") & "')"

        cm.Connection = c
        cm.CommandText = vSQL

        'Response.Write(vSQL)

        Try
            cm.ExecuteNonQuery()

            DocTranId = GetRef("select top 1 TranId from DocumentLedger where PolicyCd='" & Session("PolicyId") & "' order by TranId desc", "")

            ' =========================================================================================
            ' GET THE DOCUMENT CONTROLLER LIST
            ' =========================================================================================
            vSQL = "select User_Id, " _
                    & "(select Emp_Email from employee_master where Emp_Cd=User_Id) As EmailAdd from rights_list " _
                    & "where Property_Value='1000' and Property='DOCOWNERACCESS'"

            cm.CommandText = vSQL
            rs = cm.ExecuteReader
            Do While rs.Read
                ControllerEmailList += rs("EmailAdd") & ","
            Loop
            rs.Close()

            Session("ControllerEmailList") = ControllerEmailList.Substring(0, ControllerEmailList.Length - 1)


            ' =========================================================================================
            ' SET FOR REVIEW
            ' =========================================================================================
            If CmdRefForReview.SelectedValue <> "None" And CmdDocStatus.SelectedValue = "15" Then
                vSQL = "insert into DocumentAppover values (" & DocTranId & ",'" & Session("PolicyId") & "','" & Session("uid") & "','" & Now & "'," _
                    & CmdRefDocOwner.SelectedValue & "," & CmdRefDocType.SelectedValue & ",15,1,'" & CmdRefForReview.SelectedValue & "')"
                CreateRecords(vSQL)

                vSQL = "select Emp_Email from employee_master where Emp_Cd='" & CmdRefForReview.SelectedValue & "'"
                ReviewerEmail = GetRef(vSQL, "")
                SendEmail(ReviewerEmail, CmdRefForReview.SelectedItem.Text, 15)
            End If


            ' =========================================================================================
            ' SET FOR APPROVAL
            ' =========================================================================================
            If CmdRefForApproval.SelectedValue <> "None" And CmdDocStatus.SelectedValue = "15" Or
                CmdRefForApproval.SelectedValue <> "None" And CmdDocStatus.SelectedValue = "16" Then

                vSQL = "insert into DocumentAppover values (" & DocTranId & ",'" & Session("PolicyId") & "','" & Session("uid") & "','" & Now & "'," _
                    & CmdRefDocOwner.SelectedValue & "," & CmdRefDocType.SelectedValue & ",16,2,'" & CmdRefForApproval.SelectedValue & "')"
                'Response.Write(vSQL)

                CreateRecords(vSQL)

                If CmdRefForApproval.SelectedValue <> "None" And CmdDocStatus.SelectedValue = "16" Then
                    vSQL = "select Emp_Email from employee_master where Emp_Cd='" & CmdRefForApproval.SelectedValue & "'"
                    ApproverEmail = GetRef(vSQL, "")

                    SendEmail(ApproverEmail, CmdRefForApproval.SelectedItem.Text, 16)
                End If
            End If


            ' Draft document policy
            If CmdDocStatus.SelectedValue = "35" Then
                SendEmail(ControllerEmailList.Substring(0, ControllerEmailList.Length - 1), "Document Controller", 35)
            End If

            ' Publish the document policy
            If CmdDocStatus.SelectedValue = "11" Then

                vSQL = "delete from policy_logs where PolicyCd='" & Session("PolicyId") & "'"
                CreateRecords(vSQL)

                vSQL = "delete from policylist where PolicyCd='" & Session("PolicyId") & "'"
                CreateRecords(vSQL)

                vSQL = "insert into policylist (PolicyCd, Remarks, PolicyFileLocation, CreatedBy, DateCreated, FileLocation, Group_Id, DocType_Id, Descr) " _
                    & "select PolicyCd, Remarks, PolicyFileLocation, CreatedBy, DateCreated, FileLocation, Group_Id, DocTypeId, Descr " _
                    & "from DocumentLedger where TranId=" & DocTranId
                CreateRecords(vSQL)

                SendEmail("", "Colleagues", 11)
            End If

            TargetFilename = Server.MapPath(".") & "\Uploaded\BPOI\Policy\" & DocId & ".pdf"
            TxtFileName.SaveAs(TargetFilename)

            tblDocList.SelectedIndex = -1
            GetDocumentList()

            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)

        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('SQL error:" & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');", True)
        Finally
            c.Close()
            c.Dispose()
            cm.Dispose()
        End Try

    End Sub

    Private Sub BtnDeleteDoc_ServerClick(sender As Object, e As EventArgs) Handles BtnDeleteDoc.ServerClick

        vSQL = "delete from DocumentLedger " _
            & "where PolicyCd='" & tblDocList.SelectedRow.Cells(1).Text & "'"
        CreateRecords(vSQL)

        vSQL = "delete from DocumentAppover " _
            & "where PolicyCd='" & tblDocList.SelectedRow.Cells(1).Text & "'"
        CreateRecords(vSQL)

        tblDocList.SelectedIndex = -1
        GetDocumentList()

        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
    End Sub

    Private Sub tblDocList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblDocList.PageIndexChanging
        tblDocList.PageIndex = e.NewPageIndex
        tblDocList.SelectedIndex = -1
        GetDocumentList()
    End Sub

    Private Sub BtnClickDownload_ServerClick(sender As Object, e As EventArgs) Handles BtnClickDownload.ServerClick
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "window.open('download/report/StatisticsReport.csv');", True)
    End Sub

    Private Sub BtnForCorrection_ServerClick(sender As Object, e As EventArgs) Handles BtnForCorrection.ServerClick

        If TxtNewRemarks.Text.Trim = "" Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('This action requires a remark.'); ViewDocument(""" & frmValue.Value & """,""" & TxtRemarksVal.Value & """)", True)
            GetDocumentHistory()
            Exit Sub
        End If

        vSQL = " insert into DocumentLedger values ('" _
            & Session("PolicyId") & "','" & tblDocList.SelectedRow.Cells(2).Text & "','" & TxtNewRemarks.Text.ToString.Trim & "','" _
            & Session("PolicyFileLocation") & "','" & Session("uid") & "','" & Now() & "','" _
            & Session("FileLocation") & "','" & tblDocList.SelectedRow.Cells(4).Text & "','','" _
            & tblDocList.SelectedRow.Cells(5).Text & "',9, null)"

        CreateRecords(vSQL)
        GetDocumentList()



        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        Dim ControllerEmailList As String = ""

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        cm.Connection = c
        cm.CommandText = vSQL

        vSQL = "select User_Id, " _
                & "(select Emp_Email from employee_master where Emp_Cd=User_Id) As EmailAdd from rights_list " _
                & "where Property_Value='1000' and Property='DOCOWNERACCESS'"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            ControllerEmailList += rs("EmailAdd") & ","
        Loop
        rs.Close()

        c.Close()
        c.Dispose()
        cm.Dispose()

        SendEmail(Session("AuthorEmailadd"), Session("AuthorFullName"), 9)

        'If Session("UserType") = 1 Or Session("UserType") = 3 Then
        '    SendEmail(Session("AuthorEmailadd"), Session("AuthorFullName"), 9)
        'End If

        'If Session("UserType") = 4 Then
        '    SendEmail(Session("AuthorEmailadd"), Session("AuthorFullName"), 9)
        'End If

        TxtNewRemarks.Text = ""

        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
    End Sub

    Private Sub BtnApproved_ServerClick(sender As Object, e As EventArgs) Handles BtnApproved.ServerClick

        Dim StatusId As Integer = 0
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        Dim ControllerEmailList As String = ""
        Dim ApproverEmail As String = ""

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        cm.Connection = c
        cm.CommandText = vSQL

        'vSQL = "select User_Id, " _
        '            & "(select Emp_Email from employee_master where Emp_Cd=User_Id) As EmailAdd from rights_list " _
        '            & "where Property_Value='1000' and Property='DOCOWNERACCESS'"

        'cm.CommandText = vSQL
        'rs = cm.ExecuteReader
        'Do While rs.Read
        '    ApproverEmail += rs("EmailAdd") & ","
        'Loop
        'rs.Close()

        vSQL = "select User_Id, " _
                    & "(select Emp_Email from employee_master where Emp_Cd=User_Id) As EmailAdd from rights_list " _
                    & "where Property_Value='1000' and Property='DOCOWNERACCESS'"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            ControllerEmailList += rs("EmailAdd") & ","
        Loop
        rs.Close()

        c.Close()
        c.Dispose()
        cm.Dispose()


        If Session("DocStatus") = 15 Then
            StatusId = 16
            SendEmail(Session("ApproverEmail"), Session("ApproverFullName"), StatusId)
        End If

        If Session("DocStatus") = 16 Then
            StatusId = 30
            SendEmail(ControllerEmailList.Substring(0, ControllerEmailList.Length - 1), "Document Controller", StatusId)
        End If


        'If Session("DocStatus") = 15 Then
        '    StatusId = 16

        '    If Session("ApproverEmail") <> "" Then
        '        SendEmail(Session("ApproverEmail"), Session("ApproverName"), 30)
        '    Else
        '        SendEmail(ControllerEmailList.Substring(0, ControllerEmailList.Length - 1), "Document Controller", 30)
        '    End If
        'Else
        '    StatusId = 30
        '    SendEmail(ControllerEmailList.Substring(0, ControllerEmailList.Length - 1), "Document Controller", 30)
        'End If

        vSQL = " insert into DocumentLedger values ('" _
            & Session("PolicyId") & "','" & tblDocList.SelectedRow.Cells(2).Text & "','" & TxtNewRemarks.Text.ToString.Trim & "','" _
            & Session("PolicyFileLocation") & "','" & Session("uid") & "','" & Now() & "','" _
            & Session("FileLocation") & "','" & tblDocList.SelectedRow.Cells(4).Text & "','','" _
            & tblDocList.SelectedRow.Cells(5).Text & "'," & StatusId & ", null)"
        'Response.Write(vSQL)

        CreateRecords(vSQL)

        GetDocumentList()

        TxtNewRemarks.Text = ""
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
    End Sub

    Private Sub SendEmail(ToEmpCode As String, ToName As String, EmailType As Integer)

        'Response.Write(ToEmpCode & "<br>AuthorEmailadd: " & Session("AuthorEmailadd") & "<br>ControllerEmailList: " & Session("ControllerEmailList"))
        'Exit Sub

        Try
            Dim Smtp_Server As New SmtpClient
            Dim e_mail As New MailMessage()
            Dim Tbl As String = ""
            Dim ToEmail As String = ""
            Dim DocStatus As String = ""
            Dim Remarks As String = ""

            Smtp_Server.UseDefaultCredentials = False
            Smtp_Server.Credentials = New Net.NetworkCredential("rdbisenio@bposerve.com", "RBi43n1011")
            Smtp_Server.Port = 587
            Smtp_Server.EnableSsl = True
            Smtp_Server.Host = "smtp.gmail.com"

            e_mail = New MailMessage()
            e_mail.From = New MailAddress("rdbisenio@bposerve.com")


            'ToEmail = GetRef("select Emp_Email from employee_master where Emp_Cd ='" & ToEmpCode & "'", "")


            Tbl = "<html><body style='font-size:14px'><head><style>" _
                        & "td {border:1px solid #F2F3F4; padding:8px} " _
                        & ".lbl {color:#007BFF; font-weight:bold; font-size:14px} " _
                        & ".lbl2 {color:#000; font-size:14px} " _
                        & ".lbl3 {color:#000; font-size:14px} " _
                        & ".lbl4 {color:#7b7b7b; font-size:14px} " _
                        & ".lbl5 {color:#000; font-size:14px; width:100%; padding-buttom: 20px; border: solid 0px #fff;height: 200px } " _
                        & "</style></head>"



            Tbl += "<div style='width:80%; margin:auto;'><label class='lbl3'>" & IIf(ToName = "Colleagues", "Dear ", "Hi ") & ToName & ",</label><br /><br />"
            Tbl += "<Label Class='lbl3'>"

            Select Case EmailType
                Case 15 'For Review
                    Tbl += "This is to inform that we have appointed you as the Reviewer of a draft document in the Insight portal.<br /><br />"
                    Tbl += "You have (1) new document for review. Kindly assess and provide feedback, if necessary. Otherwise, you may submit it for approval. Details are enumerated as follows:<br />"
                    DocStatus = "For Review"
                    e_mail.To.Add(ToEmpCode)
                    e_mail.CC.Add(Session("AuthorEmailadd"))
                    Remarks = TxtRemarks.Text.Trim

                Case 16 'For Approval
                    Tbl += "This is to inform that we have appointed you as the Approver of a draft document in the Insight portal.<br /><br />"
                    Tbl += "You have (1) new document for approval. Kindly assess and provide feedback, if necessary. Otherwise, you may approve it for publication. Details are enumerated as follows:<br />"
                    DocStatus = "For Approval"
                    e_mail.To.Add(ToEmpCode)
                    e_mail.CC.Add(Session("AuthorEmailadd") _
                                  & IIf(Session("ControllerEmailList") = "", "", "," & Session("ControllerEmailList")) _
                                  & IIf(Session("UserEmail") = "", "", "," & Session("UserEmail")))
                    Remarks = TxtRemarks.Text.Trim

                Case 9 'For Correction
                    Tbl += "You have (1) new document for correction. Kindly re-upload the revised document in the Insight Portal for proper review and approval. Please check the remarks below."

                    DocStatus = "For Correction"

                    If Session("UserType") = 1 Or Session("UserType") = 3 Then
                        e_mail.To.Add(ToEmpCode)
                    End If

                    If Session("UserType") = 4 Then

                        e_mail.To.Add(Session("AuthorEmailadd"))
                        e_mail.CC.Add(Session("ControllerEmailList") _
                            & IIf(Session("UserEmail") = "", "", "," & Session("UserEmail")))

                        '& IIf(Session("ReviewEmail") = "", "", "," & Session("ReviewEmail")) _
                        '& IIf(Session("ApproverEmail") = "", "", "," & Session("ApproverEmail")))
                    End If

                    Remarks = TxtNewRemarks.Text

                Case 30 'Approved
                    Tbl += "You have (1) new approved document that is now ready for publishing. Please see details below."
                    DocStatus = "Approved"
                    e_mail.To.Add(IIf(Session("ControllerEmailList") = "", "", "," & Session("ControllerEmailList")))
                    e_mail.CC.Add(Session("AuthorEmailadd") _
                        & IIf(Session("UserEmail") = "", "", "," & Session("UserEmail")))

                    Remarks = TxtNewRemarks.Text

                Case 35 'Draft 
                    Tbl += "I have uploaded a draft document for your review and control. Please review and provide feedback, if necessary. Otherwise, kindly assign the designated Reviewer and Approver for their proper handling. Details are enumerated as follows:"
                    DocStatus = "Draft"
                    e_mail.To.Add(IIf(Session("ControllerEmailList") = "", "", "," & Session("ControllerEmailList")))
                    e_mail.CC.Add(IIf(Session("UserEmail") = "", "", "," & Session("UserEmail")))
                    Remarks = TxtRemarks.Text.Trim

                Case 40 'Document Author For Correction
                    'Tbl += "We have reviewed your drafted documentation. Before we proceed with the review and approval process, please check the feedback provided with details below. Kindly revise if necessary."
                    'DocStatus = "Publish"
                    'e_mail.To.Add("neurbano@bposerve.com")

                Case 11 'Publish

                    If Session("PublishCnt") > 1 Then
                        Tbl += "Please be informed that the revised <b>" & TxtDescr.Value.Trim & "</b> under <b>" & CmdRefDocType.SelectedItem.Text & "</b> is now available in the Insight Portal for our common reference.<br />"
                    Else
                        Tbl += "Please be informed that the new <b>" & TxtDescr.Value.Trim & "</b> under <b>" & CmdRefDocType.SelectedItem.Text & "</b> is now available in the Insight Portal for our common reference.<br />"
                    End If

                    DocStatus = "Publish"
                    e_mail.To.Add("lancebisenio@gmail.com,rdbisenio@bposerve.com, rfaldossary@bposerve.com") ' "neurbano@bposerve.com,jjdavid@bposerve.com,rfaldossary@bposerve.com"
                    Remarks = TxtRemarks.Text.Trim
            End Select

            Tbl += "<br /><br /></label>"
            Tbl += "<table style='border:1px solid #F2F3F4; font-size:12px; width:95%; margin:auto; border-collapse:collapse;' border='1'>"

            Tbl += "<tr>" _
                    & "<td><label class='lbl'>Description:</label><br /><label class='lbl2'>" & TxtDescr.Value.Trim & "</label><br /><br /></td>" _
                    & "<td style='width:80px' rowspan='4'></td>" _
                    & "<td style='width:200px'><label class='lbl'>Document Type:</label><br /><label class='lbl2'>" & CmdRefDocType.SelectedItem.Text & "</label><br /></td>" _
                    & "</tr>" _
                    & "<tr><td>"

            If EmailType = 11 Then
                Tbl += "<label class='lbl' style='text-align: justify'>Please see key changes in the document:</label><br />" _
                    & "<textarea class='lbl5'>" & TxtKeyChanges.Text & "</textarea>"


                If TxtRemarks.Text.Trim <> "" Then
                    Tbl += "<br /><br /><label class='lbl' style='text-align: justify'>Remarks:</label><br /><label class='lbl2'>" & Remarks & "</label>"
                End If
            Else
                Tbl += "<label class='lbl' style='text-align: justify'>Remarks:</label><br /><label class='lbl2'>" & Remarks & "</label>"
            End If

            Tbl += "<br /></td>"
            Tbl += "<td style='vertical-align: top;'><label class='lbl'>Document Owner:</label><br /><label class='lbl2'>" & CmdRefDocOwner.SelectedItem.Text & "</label><br /></td>" _
                    & "</tr>" _
                    & "<tr>" _
                    & "<td><label class='lbl'>Created By:</label><br /><label class='lbl2'>" & Session("sFName") & "</label><br /></td>" _
                    & "<td><label class='lbl'>Status:</label><br /><label class='lbl2'>" & DocStatus & "</label><br /></td>" _
                    & "</tr>" _
                    & "<tr>" _
                    & "<td><label class='lbl'>Date Created:</label><br /><label class='lbl2'>" & Now & "</label><br /></td>" _
                    & "<td><label class='lbl'></label><br /></td>" _
                    & "</tr>" _
                    & "" _
                    & "</table><br />"

            Tbl += "<label class='lbl3'>Click the link to access the portal:&nbsp;<a href='http://insight.bposerve.com/hr-portalbeta'>http://insight.bposerve.com</a></label><br><br>"

            Select Case EmailType
                Case 15, 16 'For Review
                    Tbl += "<label class='lbl3'><b>NOTE:</b> Upon logging in to the portal, go to <b>Menu</b> > <b>Document Management</b> and set the filter based on the document type indicated above. Find the document title and click on <b>Select</b> and <b>Edit</b> button. Do not forget to set the <b>current status</b> of the document before hitting the <b>Update button</b>.</label><br /><br />"
                Case 9, 30
                    Tbl += "<label class='lbl3'><b>NOTE:</b> Upon logging in to the portal, click <b>Menu</b> > <b>Document Management</b> > <b>Add New</b> and select the document type</label><br><br>"
                Case 11
                    Tbl += "<label class='lbl3'><b>NOTE:</b> All BPOI employees are required to read, agree and attest the document.</label><br><br>"
            End Select

            Tbl += "<label class='lbl3'>Thank you!</label><br><br />"

            Tbl += "<label class='lbl3'>Quality Assurance Team | Business Transformation Group</label><br><br />"

            Tbl += "<label class='lbl4'>Confidentiality Statement and Disclaimer</label><br><br>"

            Tbl += "<label class='lbl4'>Proprietary or confidential information belonging to BPO International, Inc. may be contained in this message. If you are not the addressee indicated in this message (or responsible for the delivery of the message to such person), please do not copy or deliver this message to anyone. In such case, please destroy this message and notify the sender by reply e-mail. Please advise the sender immediately if you or your employer do not consent to Internet e-mail for messages of this kind. Whilst all reasonable steps are taken to ensure the accuracy and integrity of information and data transmitted electronically, and to preserve the confidentiality thereof, no liability or responsibility whatsoever is accepted if information or data is, for whatever reason, corrupted or does not reach its intended destination.</label><br><br />"

            Tbl += "<label class='lbl4'>End statement</label><br><br />"









            'Tbl += "<label class='lbl'>Click the link below to access the portal:</label><br>"
            'Tbl += "<a href='h ttp://insight.bposerve.com/hr-portalbeta'>h ttp://insight.bposerve.com</a><br><br>"
            'Tbl += "<label class='lbl'>NOTE: Once you’ve successfully logged in, go to Menu > Document Management and set the filter based on the document type.</label><br><br>"
            'Tbl += "<label class='lbl3'>Thank you!</label><br>"

            Tbl += "</div></body></html>"


            e_mail.Subject = "POLICY PORTAL"
            e_mail.IsBodyHtml = True
            e_mail.Body = Tbl


            Smtp_Server.Send(e_mail)
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully');", True)

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Sending error: " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');", True)
        End Try
    End Sub
End Class
