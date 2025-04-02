Imports System.Data
Imports HelperClass
Partial Class ReadDocument
    Inherits System.Web.UI.Page
    Dim vScript As String = ""
    Dim vSQL As String = ""

    Private Sub ReadDocument_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Session("uid") = "admin"

        If Session("uid") = "" Then
            Response.Redirect("~/Login")
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim CanViewApp As Integer = 0

            vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='1000' and User_Id='" & Session("uid") & "'"
            CanViewApp = GetRef(vSQL, "")

            If CanViewApp > 0 Then
                BtnAdd.Disabled = False
                BtnUpdate.Disabled = False
                BtnDelete.Disabled = False
                'BtnGenReport.Visible = True
            End If

            BuildCombo("select Type_Cd, Descr from dm_document_type order by Descr", CmdDocType)
            CmdDocType.SelectedValue = "28"

            BuildCombo("select Group_Id, Descr from emp_group_ref order by Descr", CmdDocOwner)
            CmdDocOwner.Items.Add("All")
            CmdDocOwner.SelectedValue = "All"

            BuildCombo("select Type_Cd, Descr from dm_document_type order by Descr", CmdRefDocType)

            BuildCombo("select Group_Id, Descr from emp_group_ref order by Descr", CmdRefDocOwner)

            BuildCombo("select Type_Cd, Descr from dm_document_type order by Descr", CmdEDocType)

            BuildCombo("select Group_Id, Descr from emp_group_ref order by Descr", CmdEDocOwner)

            BuildCombo("select DeptCd, Descr from emp_department_ref order by Descr", CmdDLDept)
            CmdDLDept.Items.Add("All")
            CmdDLDept.SelectedValue = "All"

            BuildCombo("select PolicyCd, Remarks from PolicyList where DocType_id='" & CmdDocType.SelectedValue & "' order by Remarks", CmdDLDocList)
            CmdDLDocList.Items.Add("All")
            CmdDLDocList.SelectedValue = "All"

            GetDocumentList()
        End If
    End Sub

	Private Sub GetDocumentList()
		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""
		Dim vTableName As String = ""
		Dim vSQL As String = ""

        c.ConnectionString = ConnStr

        If CmdDocType.SelectedValue <> "" Then
            vFilter += " and DocType_Id=" & CmdDocType.SelectedValue
        End If

        If CmdDocOwner.SelectedValue <> "All" Then
            vFilter += " and Group_id=" & CmdDocOwner.SelectedValue
        End If

        If RdoOpt2.Checked = True Then
            vFilter += "and PolicyFileLocation not in (select distinct(PolicyFileLocation) as PolicyList from Policy_Logs where IsAgree='" & Session("uid") & "')"
        End If

        If TxtKeywords.Value.Trim <> "" Then
            vFilter += "and Remarks like '%" & TxtKeywords.Value.Trim & "%'"
        End If

        vSQL = "select *, " _
            & "(select GroupCd from emp_group_ref a where a.Group_Id=PolicyList.Group_Id) as Owner " _
            & "from PolicyList where PolicyCd is not null " & vFilter & " order by Remarks"

        'Response.Write(vSQL)

        da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "tblEmployees")
        tblDocList.DataSource = ds.Tables("tblEmployees")
        tblDocList.DataBind()

        da.Dispose()
        ds.Dispose()

        lblFilename.Text = "Total Documents: " & tblDocList.DataSource.Rows.Count
    End Sub

    Private Sub CmdDocType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmdDocType.SelectedIndexChanged
        GetDocumentList()
    End Sub

    Private Sub CmdDocOwner_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmdDocOwner.SelectedIndexChanged
        GetDocumentList()
    End Sub

    Private Sub tblDocList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblDocList.SelectedIndexChanged

        Dim vFileName As String = "Uploaded/BPOI/Policy/" & tblDocList.SelectedRow.Cells(7).Text & "?page=hsn#toolbar=0"
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "previewfile(""" & vFileName & """);", True)

        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand

        Dim vDisableMe As String = ""

        c.ConnectionString = ConnStr
        cm.Connection = c

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        CmdEDocType.SelectedValue = tblDocList.SelectedRow.Cells(5).Text
        CmdEDocOwner.SelectedValue = tblDocList.SelectedRow.Cells(4).Text
        TxtEDescr.Value = tblDocList.SelectedRow.Cells(2).Text

        'vSQL = " insert into Policy_Logs (PolicyCd,PolicyFileName,PolicyFileLocation,ViewBy,DateView,DocType_Id) values ('" _
        '    & tblDocList.SelectedRow.Cells(1).Text & "','" & tblDocList.SelectedRow.Cells(2).Text & "','" _
        '    & tblDocList.SelectedRow.Cells(7).Text & "','" & Session("uid") & "','" & Now() & "','" & CmdDocType.SelectedValue & "')"

        'cm.CommandText = vSQL
        'Try
        '    cm.ExecuteNonQuery()
        'Catch ex As SqlClient.SqlException
        '    vScript = "alert('Error occurred while trying to clean-up the rights list. Error is: " &
        '        ex.Message.Replace(vbCrLf, "\n").Replace("'", "") & "');"
        '    c.Close()
        '    c.Dispose()
        '    cm.Dispose()
        '    Exit Sub
        'End Try

        c.Close()
        c.Dispose()
        cm.Dispose()
        'vScript = "alert('Access rights were successfully set.');"

        vDisableMe = GetRef("select count(PolicyCd) as vCtr from Policy_Logs where PolicyFileLocation='" _
                            & tblDocList.SelectedRow.Cells(7).Text & "' and IsAgree='" & Session("uid") & "'", 0)

        If vDisableMe > 0 Then
            BtnIAgree.Visible = False
        Else
            BtnIAgree.Visible = True
        End If

        Session("TranID") = tblDocList.SelectedRow.Cells(3).Text
    End Sub

    Private Sub BtnSubmit_ServerClick(sender As Object, e As EventArgs) Handles BtnSubmit.ServerClick
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        Dim PolicyCtr As Integer = 0
        Dim CurrentDate As String = Format(Now, "MMddyyyyHHmmss")
        Dim DocId As String
        Dim TargetFilename As String

        If TxtFileName.FileName = "" Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('No file to upload');", True)
            Exit Sub
        End If

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        DocId = CurrentDate

        vSQL = " insert into policylist values ('" _
            & DocId & "','" & TxtDescr.Value.Trim & "','" & DocId & ".pdf','" _
            & Session("uid") & "admin','" & Now() & "','" & TxtFileName.FileName & "','" _
            & CmdRefDocOwner.SelectedValue & "','','','" & CmdRefDocType.SelectedValue & "')"

        cm.Connection = c
        cm.CommandText = vSQL
        'Response.Write(vSQL)
        Try
            cm.ExecuteNonQuery()

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
        tblDocList.SelectedIndex = -1
        GetDocumentList()
        BtnIAgree.Visible = False

        BuildCombo("select PolicyCd, Remarks from PolicyList where DocType_id='" & CmdDocType.SelectedValue & "' order by Remarks", CmdDLDocList)
        CmdDLDocList.Items.Add("All")
        CmdDLDocList.SelectedValue = "All"

    End Sub

    Private Sub BtnUpdatePolicy_ServerClick(sender As Object, e As EventArgs) Handles BtnUpdatePolicy.ServerClick
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        Dim PolicyCtr As Integer = 0
        Dim CurrentDate As String = Format(Now, "MMddyyyyHHmmss")
        Dim DocId As String = ""
        Dim vTargetFilename As String = ""

        'If TxtEFileName.FileName.ToString.Trim = "" Then
        '    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('No file to upload');", True)
        '    Exit Sub
        'End If

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Error occurred while trying to clean-up the rights list. Error is: " &
                ex.Message.Replace(vbCrLf, "\n").Replace("'", "") & "');", True)
            Exit Sub
        End Try

        PolicyCtr = GetRef("select count(PolicyCd) as vCtr from policylist", 0)
        DocId = CurrentDate

        cm.Connection = c

        vSQL = "update PolicyList set " _
            & "DocType_Id='" & CmdEDocType.SelectedValue & "', " _
            & "Group_Id='" & CmdEDocOwner.SelectedValue & "', " _
            & "Remarks='" & TxtEDescr.Value.Trim & "', " _
            & "ModifyBy='" & Session("uid") & "'," _
            & "DateModify='" & Now() & "' "

        If TxtEFileName.FileName.ToString.Trim <> "" Then
            vSQL += " , PolicyFileLocation='" & DocId & ".pdf' "
        End If

        vSQL += "where TranId=" & tblDocList.SelectedRow.Cells(3).Text

        cm.CommandText = vSQL

        Try
            cm.ExecuteNonQuery()

            If TxtEFileName.FileName.ToString.Trim <> "" Then
                vSQL = "delete from Policy_Logs where PolicyCd='" & tblDocList.SelectedRow.Cells(1).Text & "' "
                cm.CommandText = vSQL
                cm.ExecuteNonQuery()

                vTargetFilename = Server.MapPath(".") & "\Uploaded\BPOI\Policy\" & DocId & ".pdf"
                TxtEFileName.SaveAs(vTargetFilename)
            End If

            tblDocList.SelectedIndex = -1
            GetDocumentList()

            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Error occurred while trying to clean-up the rights list. Error is: " &
                ex.Message.Replace(vbCrLf, "\n").Replace("'", "") & "');", True)
        Finally
            c.Close()
            c.Dispose()
            cm.Dispose()
        End Try
    End Sub

    Private Sub BtnDeleteDoc_ServerClick(sender As Object, e As EventArgs) Handles BtnDeleteDoc.ServerClick
        vSQL = "delete from PolicyList " _
            & "where TranId=" & tblDocList.SelectedRow.Cells(3).Text

        CreateRecords(vSQL)

        tblDocList.SelectedIndex = -1
        GetDocumentList()

        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)
    End Sub

    Private Sub BtnIgree_ServerClick(sender As Object, e As EventArgs) Handles BtnIAgree.ServerClick

        Dim c As New SqlClient.SqlConnection
        Dim cm As New SqlClient.SqlCommand

        c.ConnectionString = ConnStr
        cm.Connection = c
        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Error occurred while trying to clean-up the rights list. Error is: " &
                ex.Message.Replace(vbCrLf, "\n").Replace("'", "") & "');", True)
            Exit Sub
        End Try

        vSQL = " insert into Policy_Logs (PolicyCd,PolicyFileName,PolicyFileLocation,ViewBy,DateView, IsAgree, DateAgree,DocType_Id) values ('" _
            & tblDocList.SelectedRow.Cells(1).Text & "','" & tblDocList.SelectedRow.Cells(2).Text & "','" _
            & tblDocList.SelectedRow.Cells(7).Text & "','" & Session("uid") & "','" & Now() & "','" _
            & Session("uid") & "','" & Now() & "','" & CmdDocType.SelectedValue & "')"

        cm.CommandText = vSQL
        Try
            cm.ExecuteNonQuery()

            tblDocList.SelectedIndex = -1
            GetDocumentList()
        Catch ex As SqlClient.SqlException
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Error occurred while trying to clean-up the rights list. Error is: " &
                ex.Message.Replace(vbCrLf, "\n").Replace("'", "") & "');", True)
            c.Close()
            c.Dispose()
            cm.Dispose()
            Exit Sub
        End Try

        c.Close()
        c.Dispose()
        cm.Dispose()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Successfully saved');", True)

        BtnIAgree.Visible = False
    End Sub

    Private Sub tblDocList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblDocList.PageIndexChanging
        tblDocList.PageIndex = e.NewPageIndex
        tblDocList.SelectedIndex = -1
        GetDocumentList()
    End Sub

    Protected Sub cmdDownload()

        Dim vFile As String = ""
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim cmref As New SqlClient.SqlCommand

        Dim vFilename = Server.MapPath(".") & "/downloads/reports/" & Session.SessionID & "-StatisticsReport.csv"

        Dim vDump As New StringBuilder
        Dim vBuildData As String = ""

        c.Open()
        cm.Connection = c
        cmref.Connection = c

        If IO.File.Exists(vFilename) Then
            Try
                IO.File.Delete(vFilename)
            Catch ex As IO.IOException
                vScript = "alert('Error deleting dump file. Error is: " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');"
                Exit Sub
            End Try
        End If

        vDump.AppendLine("Emp Code,Last Name,First Name,Division,Department,Access,View,Agree")

        'Response.Write(Session("vSql"))

        cm.CommandText = Session("vSql")
        rs = cm.ExecuteReader
        Do While rs.Read

            vBuildData = rs("Emp_Cd") & ", " & rs("vFullName") & ", " &
                rs("vDivDescr") & "," &
                rs("vDeptDescr") & ","

            If GetRef("select count(User_Id) from User_List where User_Id='" & rs("Emp_Cd") & "' ", "") = 1 Then
                vBuildData += "YES,"
            Else
                vBuildData += "NO,"
            End If

            'If GetRef("select count(ViewBy) from policy_logs where ViewBy='" & rs("Emp_Cd") & "' and PolicyCd='" & cmbPolicy.SelectedValue & "' ", "") = 0 Then
            '    vBuildData += "NO,"
            'Else
            '    vBuildData += "YES,"
            'End If

            'If GetRef("select count(IsAgree) from policy_logs where IsAgree='" & rs("Emp_Cd") & "' and PolicyCd='" & cmbPolicy.SelectedValue & "' ", "") = 0 Then
            '    vBuildData += "NO,"
            'Else
            '    vBuildData += "YES,"
            'End If

            vDump.AppendLine(vBuildData)
        Loop

        IO.File.WriteAllText(vFilename, vDump.ToString)
        vScript = "alert('Download complete.'); window.open('downloads/reports/" & "StatisticsReport.csv');"

        rs.Close()
        cm.Dispose()
        cmref.Dispose()

        c.Close()
        c.Dispose()
    End Sub
End Class
