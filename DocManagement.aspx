<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="DocManagement.aspx.vb" Inherits="DocManagement" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="Scripts/jquery-3.4.1.js"></script>
    <style type="text/css">
        .iDataFrame {
            width: 99%;
            border: solid 0px #e2e2e2;
            height: 96%; 
            margin: 0px;
        }

        .hideGridColumn {
            display: none;
        }

        div {
            border: solid 0px; 
        }
    </style>
    <script> 

        $(document).ready(function () {
             
            $('#MainContent_CmdDLDept').change(function () {
                $("#BtnGenerate").show();
                $("#BtnProcess").hide();
                $("#MainContent_BtnClickDownload").hide();
                $("#PanelMessageBox").hide();
            });

            $('#MainContent_BtnAdd').click(function () {
                $('#MainContent_TxtDescr').val("");
                $('#MainContent_TxtRemarks').val("");
                $('#MainContent_CmdRefForReview').val("None");
                $('#MainContent_CmdRefForApproval').val("None"); 
                $('#MainContent_BtnUpdatePolicy').hide();
                $('#MainContent_BtnSubmit').show();
                $('#DivStatus').hide();
                $('#LblInfo1').hide();
                $('#LblInfo2').hide();
            });
             
            $('#MainContent_BtnUpdate').click(function () { 
                var TranID = "<%=Session("TranID") %>"; 
                var AuthorView = "<%=Session("CanViewAuthor") %>";  

                if (TranID != "") {
                    $("#DivEdit").show();
                    $("#DivAdd").hide();
                    $('#MainContent_BtnUpdatePolicy').show();
                    $('#MainContent_BtnSubmit').hide();
                    $('#myModalAdd').modal();

                    if (AuthorView == "YES") {
                        $('#DivStatus').hide();
                    } else {
                        $('#DivStatus').show();
                    }
                    
                    $('#LblInfo1').show();
                    $('#LblInfo2').show();
                } else {
                    alert("Please select item to edit.");
                }                
            });

            $('#MainContent_BtnDelete').click(function () { 
                var TranID = "<%=Session("TranID") %>";

                if (TranID != "") { 
                    $('#myModalDel').modal();
                } else {
                    alert("Please select item to delete.");
                }
            });

            $('#MainContent_BtnGenReport').click(function () {

                $("#BtnGenerate").show();
                $("#BtnProcess").hide();
                $("#MainContent_BtnClickDownload").hide();
                $("#PanelMessageBox").hide();

                $("#MainContent_CmdDLDept").val("All")
                $("#MainContent_CmdDLDocList").val("All")

                <%--var x = "<%=Session("uid") %>";
                alert(x);--%>
            });

            $('#BtnGenerate').click(function () {

                $("#BtnGenerate").hide();
                $("#BtnProcess").show();
                $("#MainContent_BtnClickDownload").hide();
                $("#PanelMessageBox").show();

                var xhttp = new XMLHttpRequest();

                xhttp.onreadystatechange = function () {
                    if (this.readyState == 4 && this.status == 200) {
                        document.getElementById("demo").innerHTML = "Validation is complete, ready to process.";
                        EmpList = this.responseText;
                        EmpList = EmpList.substring(0, EmpList.length - 1);
                        $('#TxtEmpList').val(EmpList);
                    }
                };
                xhttp.open("POST", "XMLGenerateReport", true);
                xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                xhttp.send("Type=GetEmplist&Dept=" + $("#MainContent_CmdDLDept").val());
            });

            $('#BtnProcess').click(function () {

                var ArrEmpList = $('#TxtEmpList').val().split(",");  //[$('#TxtEmpList').val()];
                var TotalEmp = ArrEmpList.length;

                var LoopCtr = 0;
                var i = 0;

                while (i < ArrEmpList.length) {
                    //document.getElementById("demo").innerHTML += "The number is " + i + "<br>";
                    var xhttp = new XMLHttpRequest();
                    xhttp.onreadystatechange = function () {
                        if (this.readyState == 4 && this.status == 200) {
                            document.getElementById("demo").innerHTML = this.responseText; // i + " : " + this.responseText;
                        }
                    };

                    xhttp.open("POST", "XMLGenerateReport", true);
                    xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                    xhttp.send("Type=GetEmpLogs&Ctr=" + i + "&EmpId=" + ArrEmpList[i] +
                        "&DocList=" + $("#MainContent_CmdDLDocList").val() +
                        "&DocType=" + $("#MainContent_CmdDocType").val() +
                        "&DocOwner=" + $("#MainContent_CmdDocOwner").val());

                    LoopCtr++;
                    i++;
                }

                $("#BtnGenerate").hide();
                $("#BtnProcess").hide();
                $("#MainContent_BtnClickDownload").show();
            });
        });

        function ViewDocument(DocParam, Remarks) {       
            var ControllerView = "<%=Session("CanViewController") %>";  

            if (ControllerView == "YES") {
                $('#MainContent_BtnApproved').hide();
            } else {
                $('#MainContent_BtnApproved').show();
            }

            $('#myModalView').modal();
            $('#MainContent_frmValue').val(DocParam);
            $('#MainContent_LblRemarks').text(Remarks);
            $('#MainContent_TxtRemarksVal').val(Remarks);
            
            document.getElementById("MainContent_frmPreview").src = DocParam;
        } 

        function previewfile(pFilename) {
            document.getElementById("frmPreview").src = pFilename;
        }
         

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <input type="hidden" id="TxtEmpList" value="" />
    <div class="container-fluid">
        <br />
        <h3>Document Management</h3>
        
        <div class="row">
            <div class="col-sm-3">
                <br />
                <br />
                <br />
                <div class="btn-group">
                    <button type="button" name="BtnAdd" id="BtnAdd" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#myModalAdd" disabled="disabled" runat="server">Add New</button>
                    <button type="button" id="BtnUpdate" class="btn btn-primary btn-sm" disabled="disabled" runat="server">Edit</button>
                    <button type="button" id="BtnDelete" class="btn btn-primary btn-sm" disabled="disabled" runat="server">Delete</button>
                </div>

            </div>
            <div class="col-sm-2">
                <small>Document Type:</small>
                <asp:DropDownList ID="CmdDocType" runat="server" Width="" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                <small>Document Owner:</small>
                <asp:DropDownList ID="CmdDocOwner" runat="server" Width="" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
            </div>
            <div class="col-sm-7">
                <br />
                <br />
                <br />
                <div class="btn-group" style="margin-top: 7px">
                    <asp:Button ID="BtnSearch" runat="server" CssClass="btn btn-sm btn-primary" Text="Search"></asp:Button>
                    <button type="button" id="BtnGenReport" runat="server" class="btn btn-warning btn-sm align-bottom" disabled="disabled" data-toggle="modal" data-target="#myModalGenReport">Generate Report</button>
                </div>
                <%--<asp:DropDownList ID="CmdShowDocs" runat="server" Width="" CssClass="form-control form-control-sm col-10" AutoPostBack="true"></asp:DropDownList>--%>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-sm-3">
                <div class="table-responsive">
                    <asp:GridView ID="tblDocList" runat="server" AllowPaging="True" BorderColor="#CCCCCC"
                        AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px" Font-Size="12px"
                        CssClass="table table-sm table-bordered table-striped" PageSize="15" EnableModelValidation="True"
                        SelectedRowStyle-CssClass="btn btn-info">

                        <Columns>
                            <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                                <HeaderStyle Width="30px"></HeaderStyle>
                            </asp:TemplateField>

                            <asp:BoundField DataField="PolicyCd" HeaderText="Tran ID" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>

                            <asp:BoundField DataField="vDescr" HeaderText="Description">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>

                            <asp:BoundField DataField="TranId" HeaderText="Tran ID" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>

                            <asp:BoundField DataField="Group_Id" HeaderText="Group_Id" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                                <HeaderStyle CssClass="hideGridColumn"></HeaderStyle>
                            </asp:BoundField>

                            <asp:BoundField DataField="DocTypeId" HeaderText="DocTypeId" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>

                            <asp:BoundField DataField="Owner" HeaderText="Owner" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn"> 
                            </asp:BoundField>

                            <asp:BoundField DataField="PolicyFileLocation" HeaderText="PolicyFileLocation" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>

                            <asp:BoundField DataField="FileLocation" HeaderText="FileLocation" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>

                            <asp:BoundField DataField="StatusId" HeaderText="StatusId" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>

                            <asp:BoundField DataField="Remarks" HeaderText="Remarks" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>
                            <asp:BoundField DataField="Review" HeaderText="Review" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>
                            <asp:BoundField DataField="Approve" HeaderText="Approve" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# GetStatusName(Eval("StatusId"))%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle CssClass="labelC" Width="90px" />
                            </asp:TemplateField>

                            <asp:CommandField ButtonType="Button" ShowSelectButton="True" SelectText="Select">
                                <ItemStyle CssClass="labelC" Width="40px" />
                                <ControlStyle CssClass="btn btn-primary btn-sm" />
                            </asp:CommandField>

                        </Columns>
                        <SelectedRowStyle CssClass="bg-warning" />
                        <PagerStyle Font-Size="8pt" />
                        <HeaderStyle CssClass="titleBar" />
                        <RowStyle CssClass="odd" />
                        <AlternatingRowStyle CssClass="even" />
                    </asp:GridView>
                    <span class="badge badge-pill badge-info">&nbsp;<asp:Label ID="lblFilename" CssClass="text-light text-sm-right" runat="server" Text=""></asp:Label>&nbsp;
                    </span>
                </div>
            </div>
            <div class="col-sm-9">
                <table class="table table-sm table-bordered table-striped small" 
                    style="border-color:#CCCCCC;border-width:1px;border-style:Solid;font-size:12px;width:100%;border-collapse:collapse;">
                    <thead>
                        <tr class="table-primary titleBar">
                            <th style="width:30px;"></th>
                            <th>Code</th>
                            <th>Description</th>
                            <th>Remarks</th>
                            <th>Created By</th>
                            <th>Date Created</th>
                            <th>Status</th> 
                            <th>Others</th>
                            <th style="width:50px;"></th>
                        </tr>
                    </thead>
                    <%=DataLogs %>
                </table> 
            </div>
        </div>
    </div>


    <div id="myModalEdit" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">Document Management</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row small">
                        <div class="col-sm-6"> 
                        </div> 
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="Button1" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <input type="button" id="Button2" value="Update" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div> 
     
    <div id="myModalAdd" class="modal fade" role="dialog">
        <div class="modal-dialog modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">Document Management</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row small">
                        <div class="col-sm-4">
                             
                            <div id="DivAdd1">
                                Document Type:
                                <asp:DropDownList ID="CmdRefDocType" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                Document Owner:
                                <asp:DropDownList ID="CmdRefDocOwner" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                <br />
                                Description:
                                <input type="text" runat="server" id="TxtDescr" name="txtCode" class="form-control form-control-sm col-12 border border-danger"
                                    value="" placeholder="Enter document description" />
                                <br />
                                Browse File: 
                                <asp:FileUpload ID="TxtFileName" runat="server" CssClass="form-control-file border" /> 
                                 
                            </div>
                            
                        </div>
                        <div class="col-sm-4">
                            <div id="DivEdit1">
                                Remarks (Optional):
                                <asp:TextBox ID="TxtRemarks" runat="server" placeholder="Enter remarks, if any" CssClass="form-control form-control-sm col-12 border" Rows="5" TextMode="MultiLine"></asp:TextBox>
                                <br />
                                <div id="DivStatus">
                                    For Review:
                                    <asp:DropDownList ID="CmdRefForReview" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                    <small id="LblInfo1" class="text-info">This field will take effect if the status value is "For Review"</small>
                                    <br />

                                    For Approval: 
                                    <asp:DropDownList ID="CmdRefForApproval" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                    <small id="LblInfo2" class="text-info">This field will take effect if the status value is "For Review" or "For Approval"</small>
                                
                                    Current Status:
                                    <asp:DropDownList ID="CmdDocStatus" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4">
                             Key Notes:
                             <asp:TextBox ID="TxtKeyChanges" runat="server" placeholder="Enter overview of changes here" 
                                 CssClass="form-control form-control-sm col-12 border border-danger" Rows="15" TextMode="MultiLine"></asp:TextBox>
                            
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnSubmit" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <input type="button" id="BtnUpdatePolicy" value="Update" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="myModalView" class="modal fade" role="dialog">
        <div class="modal-dialog modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">View Document</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row small">
                        <div class="col-sm-9">
                            <iframe id="frmPreview" class="col-sm-12" style="border: double 1px #808080; height: 700px; z-index:200;" runat="server" ></iframe>
                        </div>
                        <div class="col-sm-3">
                            Remarks: 
                            <asp:TextBox ID="LblRemarks" runat="server" placeholder="Enter remarks, if any" ReadOnly="true" 
                                CssClass="form-control form-control-sm col-12 border text-info" Rows="5" TextMode="MultiLine"></asp:TextBox>
                            <br /> 
                            
                            New remarks (Optional):
                            <asp:TextBox ID="TxtNewRemarks" runat="server" placeholder="Enter remarks, if any" 
                                CssClass="form-control form-control-sm col-12 border" Rows="5" TextMode="MultiLine" Text=""></asp:TextBox>
                            <input type="hidden" id="frmValue" runat="server" value="" />
                            <input type="hidden" id="TxtRemarksVal" runat="server" value="" />
                            <br /> 
                            Author's Key Notes:
                            <asp:TextBox ID="TxtAuthorKeyNote" runat="server" placeholder="" ReadOnly="true"  
                                CssClass="form-control form-control-sm col-12 border" Rows="15" TextMode="MultiLine" Text=""></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnForCorrection" value="For Correction" class="btn btn-sm btn-primary" runat="server" />
                    <input type="button" id="BtnApproved" value="Approve" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="myModalDel" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-danger">Warning</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div>
                                Are you sure you want to delete the selected document?
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnDeleteDoc" value="YES" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="myModalGenReport" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">Filter Option</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row small">
                        <div class="col-sm-12">
                            <div>
                                Select Department:
                                <asp:DropDownList ID="CmdDLDept" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                Select Document:
                                <asp:DropDownList ID="CmdDLDocList" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div id="PanelMessageBox" class="row" style="padding-top: 10px">
                        <div class="col-sm-12">
                            <div class="alert alert-success">
                                <%--<strong>Success!</strong> Indicates a successful or positive action.--%>
                                <p id="demo" class="text-info"></p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">

                    <input type="button" id="BtnGenerate" value="Validate Request" class="btn btn-sm btn-primary" />
                    <input type="button" id="BtnProcess" value="Submit" class="btn btn-sm btn-primary" />
                    <input type="button" id="BtnClickDownload" value="Download" class="btn btn-sm btn-success" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

