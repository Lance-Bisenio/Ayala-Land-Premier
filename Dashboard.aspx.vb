Imports System.Data
Imports HelperClass
Partial Class Dashboard
    Inherits System.Web.UI.Page
    Dim vSQL As String = ""
    Public BasicDash As String = ""
    Public AttestDetailsHeader As String = ""
    Public AttestRead As String = ""
    Public AttestUnRead As String = ""

    Public ForMyReview As String = ""
    Public ForMyApproval As String = ""
    Public ForMyAttension As String = ""

    Public DocType As String = ""

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles Me.Load

        '- Attestation % per policy per team
        '- Number of team members per team
        '- Attestation % per employee per team
        '- Number of policies per group
        '- Number of policies for review
        '- Number of policies for approval
        '- Number for revisions


        If Not IsPostBack Then

            'Dim CanViewApp As Integer = 0
            'vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='7000' and User_Id='" & Session("uid") & "'"

            'CanViewApp = GetRef(vSQL, "")

            'If CanViewApp = 0 Then
            '    '    Response.Redirect("~/AccessDenied")
            'End If

            ''NoActiveEmp = ""
            'DocType = ""

            'CmdReportList.Items.Add("Attestation % per policy per team")
            'CmdReportList.Items.Add("Number of team members per team")
            'CmdReportList.Items.Add("Attestation % per employee per team")
            'CmdReportList.Items.Add("Number of policies per group")
            'CmdReportList.Items.Add("Number of policies for review")
            'CmdReportList.Items.Add("Number of policies for approval")
            'CmdReportList.Items.Add("Number for revisions")


            'Dim CanViewAsAuthor As String = ""
            'Session("IsAuthor") = ""

            'vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='1000' and Property='DOCOWNERACCESS' and User_Id='" & Session("uid") & "'"
            'CanViewAsAuthor = GetRef(vSQL, 0)
            'Session("IsController") = IIf(CanViewAsAuthor = 1, 1, 0)

            'Response.Write(vSQL)


            'vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='1000' and Property='DOCOWNERACCESS' and User_Id='" & Session("uid") & "'"
            'CanViewAsController = GetRef(vSQL, 0)
            'UserType = IIf(CanViewAsController = 1, 3, 0)

            'CheckAccess()

            'GetAllPublishDocument()
        End If
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
            'vScript = "alert('Error occurred while trying to connect to Host Database.');"
            Exit Sub
        End Try

        Session("IsApprover") = 0
        Session("IsReview") = 0

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

                    If rs("StatusId") = 16 And Session("uid") = rs("Approve").ToString Then
                        Session("IsApprover") = 1
                    End If

                    If rs("StatusId") = 15 And Session("uid") = rs("Review").ToString Then
                        Session("IsReview") = 1
                    End If
                End If
            Loop
            rs.Close()
        End If

        'Response.Write(vSQL)
        'Session("IsApprover") = 1
        'Exit Sub

        c.Close()
        c.Dispose()
        cm.Dispose()
    End Sub

    Private Sub GetAllPublishDocument()
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim IsAggreed As Integer = 0
        Dim TotalDocsPerDocType As Integer = 0

        Try
            c.Open()
            cm.Connection = c
        Catch ex As Exception
            Response.Write("SQL connection error")
            c.Close()
            cm.Dispose()
            c.Dispose()
        End Try

        vSQL = "select count(PolicyCd) as Ctr from policylist "
        cm.CommandText = vSQL

        Try
            rs = cm.ExecuteReader
            If rs.Read Then
                BasicDash = "<div class='col-sm-2' style='text-align:center;'>"
                BasicDash += "<h6 style='padding:0px; margin-top:0px; margin-bottom:-20px'>Total Published Documents</h6><br>" _
                    & "<span class='rounded-circle CirSpan' style='margin:auto'>" _
                    & "<h1 style='text-align:center; margin-top:25px; color:#fff;'>" & rs("Ctr") & "</h1></span></div><br>"

                Session("HeadCnt") = rs("Ctr")
            End If
            rs.Close()


            If Session("IsReview") = 1 Then
                ForMyReview = "<tr><td class='text-primary'>Number of documents to be reviewed</td>"
            End If

            If Session("IsApprover") = 1 Then
                ForMyApproval = "<tr><td class='text-primary'>Number of documents to be approved</td>"
            End If

            If Session("IsController") = 1 Then
                ForMyAttension = "<tr><td class='text-primary'>Number of pending documents to be published</td>"
            End If

            vSQL = "select Type_Cd, Descr from dm_document_type " _
                & "order by Descr"

            cm.CommandText = vSQL
            rs = cm.ExecuteReader
            Do While rs.Read

                BasicDash += "<div class='col-sm-2' style='text-align:center;'>"

                vSQL = "select count(PolicyCd) as Ctr from policylist where DocType_Id=" & rs("Type_Cd")
                TotalDocsPerDocType = GetRef(vSQL, 0)

                BasicDash += "<h6 style='padding:0px; margin-top:0px; margin-bottom:-20px;'>" & rs("Descr") & "</h6><br>" _
                    & "<span class='rounded-circle CirSpan' style='margin:auto'>" _
                    & "<h1 style='text-align:center; margin-top:25px; color:#fff;'>" & TotalDocsPerDocType & "</h1></span></div><br>"


                AttestDetailsHeader += "<td style='width:130px'>" & rs("Descr") & "</td>"


                vSQL = "select count(PolicyCd) as Ctr from policylist where DocType_Id=" & rs("Type_Cd") _
                    & "and PolicyCd in (select PolicyCd from policy_logs where ViewBy='" & Session("uid") & "' and " _
                    & "IsAgree is not null and DateAgree is not null and DocType_Id=" & rs("Type_Cd") & ")"

                IsAggreed = GetRef(vSQL, 0)

                AttestRead += "<td><h2><span Class='badge badge-pill badge-success'>" & IsAggreed & "</span></h2></td>"

                'vSQL = "select count(PolicyCd) as Ctr from policylist where DocType_Id=" & rs("Type_Cd")
                AttestUnRead += "<td><h2><span class='badge badge-pill badge-danger'>" & TotalDocsPerDocType - IsAggreed & "</span></h2></td>"

                GetAllDocumentAssigntoMe(rs("Type_Cd"))

            Loop
            rs.Close()

        Catch ex As Exception
            c.Close()
            cm.Dispose()
            c.Dispose()
            Exit Sub
        End Try


        c.Close()
        cm.Dispose()
        c.Dispose()

    End Sub

    Private Sub GetAllDocumentAssigntoMe(DocType As String)
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim ForApproval As Integer = 0
        Dim ForReview As Integer = 0
        Dim ForPublis As Integer = 0

        Try
            c.Open()
            cm.Connection = c
        Catch ex As Exception
            Response.Write("SQL connection error")
            c.Close()
            cm.Dispose()
            c.Dispose()
        End Try


        vSQL = "select distinct(PolicyCd) as DocId, " _
            & "(select top 1 StatusId from DocumentLedger b where b.PolicyCd=a.PolicyCd order by TranId desc) as DocStatus, " _
            & "(select top 1 Approver from DocumentAppover c where c.PolicyCd=a.PolicyCd and StatusId=15 order by TranId desc) as ForReview, " _
            & "(select top 1 Approver from DocumentAppover c where c.PolicyCd=a.PolicyCd and StatusId=16 order by TranId desc) as ForApproval " _
            & "from DocumentLedger a where DocTypeId=" & DocType

        cm.CommandText = vSQL

        Try
            cm.CommandText = vSQL
            rs = cm.ExecuteReader
            Do While rs.Read

                If Session("uid").ToString.ToUpper = rs("ForReview").ToString.ToUpper And rs("DocStatus") = 15 Then
                    ForReview += 1
                End If

                If Session("uid").ToString.ToUpper = rs("ForApproval").ToString.ToUpper And rs("DocStatus") = 16 Then
                    ForApproval += 1
                End If

                If rs("DocStatus") = 30 Then
                    ForPublis += 1
                End If

            Loop
            rs.Close()


            If Session("IsReview") = 1 Then
                ForMyReview += "<td><h2><span Class='badge badge-pill badge-primary'>" & ForReview & "</span></h2></td>"
            End If

            If Session("IsApprover") = 1 Then
                ForMyApproval += "<td><h2><span Class='badge badge-pill badge-primary'>" & ForApproval & "</span></h2></td>"
            End If

            If Session("IsController") = 1 Then
                ForMyAttension += "<td><h2><span Class='badge badge-pill badge-primary'>" & ForPublis & "</span></h2></td>"
            End If

        Catch ex As Exception

        End Try


        c.Close()
        cm.Dispose()
        c.Dispose()

    End Sub
    'Private Sub GetAllPublishDocument()
    '    Dim c As New SqlClient.SqlConnection(ConnStr)
    '    Dim cm As New SqlClient.SqlCommand
    '    Dim rs As SqlClient.SqlDataReader

    '    Try
    '        c.Open()
    '        cm.Connection = c
    '    Catch ex As Exception
    '        Response.Write("SQL connection error")
    '        c.Close()
    '        cm.Dispose()
    '        c.Dispose()
    '    End Try

    '    vSQL = "select count(PolicyCd) as Ctr from policylist "

    '    cm.CommandText = vSQL
    '    Try
    '        rs = cm.ExecuteReader
    '        If rs.Read Then
    '            NoActiveEmp = "<div class='col-sm-2'>" _
    '                & "<div Class='card bg-info text-white'>" _
    '                    & "<div Class='card-body'>Total Publish Document" _
    '                        & "<h1 style='padding:0px; margin-top:-10px; margin-bottom:0px'>" & rs("Ctr") & "</h1>" _
    '                    & "</div>" _
    '                & "</div>" _
    '                & "</div>"

    '            Session("HeadCnt") = rs("Ctr")
    '        End If
    '        rs.Close()

    '        vSQL = "select Type_Cd, Descr from dm_document_type " _
    '            & "order by Descr"

    '        cm.CommandText = vSQL
    '        rs = cm.ExecuteReader
    '        Do While rs.Read
    '            NoActiveEmp += "<div class='col-sm-2'>" _
    '                & "<div Class='card bg-info text-white'>" _
    '                    & "<div Class='card-body'>" & rs("Descr") & ""

    '            vSQL = "select count(PolicyCd) as Ctr from policylist where DocType_Id=" & rs("Type_Cd")

    '            NoActiveEmp += "<h1 style='padding:0px; margin-top:-10px; margin-bottom:0px'>" & GetRef(vSQL, 0) & "</h1>"

    '            NoActiveEmp += "</div></div></div>"
    '        Loop
    '        rs.Close()




    '    Catch ex As Exception
    '        c.Close()
    '        cm.Dispose()
    '        c.Dispose()
    '        Exit Sub
    '    End Try


    '    c.Close()
    '    cm.Dispose()
    '    c.Dispose()

    'End Sub


    'Private Sub GetActiveEmployees()
    '    Dim c As New SqlClient.SqlConnection(ConnStr)
    '    Dim cm As New SqlClient.SqlCommand
    '    Dim rs As SqlClient.SqlDataReader

    '    Try
    '        c.Open()
    '        cm.Connection = c
    '    Catch ex As Exception
    '        Response.Write("SQL connection error")
    '        c.Close()
    '        cm.Dispose()
    '        c.Dispose()
    '    End Try

    '    vSQL = "select count(Emp_Cd) as Ctr from employee_master where Date_Resign is null "

    '    cm.CommandText = vSQL
    '    Try
    '        rs = cm.ExecuteReader
    '        If rs.Read Then
    '            NoActiveEmp = "<div class='col-sm-2'>" _
    '            & "<div Class='card bg-info text-white'>" _
    '                & "<div Class='card-body'>Total Active Employees" _
    '                    & "<h1>" & rs("Ctr") & "</h1>" _
    '                & "</div>" _
    '            & "</div>" _
    '            & "</div>"

    '            Session("HeadCnt") = rs("Ctr")
    '        End If
    '        rs.Close()

    '    Catch ex As Exception
    '        c.Close()
    '        cm.Dispose()
    '        c.Dispose()
    '        Exit Sub
    '    End Try


    '    c.Close()
    '    cm.Dispose()
    '    c.Dispose()

    'End Sub

    Private Sub GetDocumentType()

        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim Ctr As Integer = 0

        Try
            c.Open()
            cm.Connection = c
        Catch ex As Exception
            Response.Write("SQL connection error")
            c.Close()
            cm.Dispose()
            c.Dispose()

        End Try

        vSQL = "select Type_Cd, Descr, " _
            & "(select count(DocType_Id) from policylist where Type_Cd=DocType_Id) as Ctr " _
            & "from dm_document_type order by Descr"

        cm.CommandText = vSQL
        Try
            rs = cm.ExecuteReader
            Do While rs.Read

                DocType += "<div Class='col-sm-2'>" _
                & "<div Class='card bg-warning text-dark'>" _
                    & "<div Class='card-body'>" & rs("Descr") _
                        & "<h1 class='display-3'>" & rs("Ctr") & "</h1>"


                'DocType += "<br />" _
                '    & "<div class='row'>" _
                '        & "<div class='col-sm-8 line-height'><small>Number of employees attested</small></div>" _
                '        & "<div Class='col-sm-4'>88</div>" _
                '    & "</div><br />" _
                '    & "<div Class='row'>" _
                '        & "<div class='col-sm-8 line-height'><small>Number of employees unread</small></div>" _
                '        & "<div Class='col-sm-4'>99</div>" _
                '    & "</div>"



                GetEmployeesCompletedThePolicy(rs("Type_Cd"), rs("Ctr"))

                DocType += "</div></div></div>"



            Loop
            rs.Close()

        Catch ex As Exception
            c.Close()
            cm.Dispose()
            c.Dispose()
            Exit Sub
        End Try


        c.Close()
        cm.Dispose()
        c.Dispose()
    End Sub


    Private Sub GetEmployeesCompletedThePolicy(DocTypeCd As Integer, DocCtr As Integer)

        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader
        Dim TotalCompletedCtr As Integer = 0

        Try
            c.Open()
            cm.Connection = c
            cm.CommandTimeout = 500

        Catch ex As Exception
            Response.Write("SQL connection error")
            c.Close()
            cm.Dispose()
            c.Dispose()

        End Try


        If DocCtr = 0 Then
            DocType += "<br />" _
                    & "<div class='row'>" _
                        & "<div class='col-sm-8 line-height'><small>Number of employees with complete attestation</small></div>" _
                        & "<div Class='col-sm-4'>0</div>" _
                    & "</div><br />" _
                    & "<div Class='row'>" _
                        & "<div class='col-sm-8 line-height'><small>Number of employees with incomplete attestation</small></div>" _
                        & "<div Class='col-sm-4'>0</div>" _
                    & "</div>"
        Else

            vSQL = "select Emp_Cd, " _
                & "(select count(distinct(PolicyFileLocation)) from Policy_Logs where " _
                    & "PolicyFileLocation in (select PolicyFileLocation from PolicyList where DocType_Id=" & DocTypeCd & ") and " _
                    & "IsAgree=Emp_Cd And DocType_Id=" & DocTypeCd & ") As Completed " _
                & "from employee_master where Date_Resign is null"

            cm.CommandText = vSQL
            Try
                rs = cm.ExecuteReader
                Do While rs.Read

                    If rs("Completed") = DocCtr Then
                        TotalCompletedCtr += 1
                    End If

                Loop
                rs.Close()

                DocType += "<br />" _
                    & "<div class='row'>" _
                        & "<div class='col-sm-8 line-height'><small>Number of employees with complete attestation</small></div>" _
                        & "<div Class='col-sm-4'>" & TotalCompletedCtr & "</div>" _
                    & "</div><br />" _
                    & "<div Class='row'>" _
                        & "<div class='col-sm-8 line-height'><small>Number of employees with incomplete attestation</small></div>" _
                        & "<div Class='col-sm-4'>" & Session("HeadCnt") - TotalCompletedCtr & "</div>" _
                    & "</div>"

            Catch ex As Exception
                Response.Write("Error occurred while trying to clean-up the rights list. Error is: " _
                    & ex.Message.Replace(vbCrLf, "\n").Replace("'", ""))

                c.Close()
                cm.Dispose()
                c.Dispose()
                Exit Sub
            End Try
        End If

        c.Close()
        cm.Dispose()
        c.Dispose()
    End Sub

End Class
