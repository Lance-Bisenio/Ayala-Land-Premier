<%@ Page Title="" Language="VB" Debug="true" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="GeneratePayroll.aspx.vb" Inherits="GeneratePayroll" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <link href="Content/calendar/jquery-ui-1.10.4.custom.css" rel="stylesheet" />
    <%--<link href="Content/calendar/CalendarControl.css" rel="stylesheet" />--%>


    <%--<script src="Scripts/jquery-ui/jquery-ui.js"></script>--%>
    <script src="Scripts/jquery-3.5.1.js"></script>


    <script>

        $(function () {
            $("#TxtTargetPaydate").datepicker();
        });

        $(document).ready(function () {
            $('#BtnUpload').click(function () {
                $('#UploadFiles').modal();
            });

            $('#MainContent_BtnReUpload').click(function () {
                $('#ReUploadFiles').modal();
            });

            $('#MainContent_BtnLock').click(function () {
                //alert($('#MainContent_BtnLock').text());
                //alert($('#lblLock').text());

                if ($('#MainContent_BtnLock').text() == "Lock Payroll") {
                    $('#lblLock').text("Are you sure you want to lock this payroll transaction?");
                } else {
                    $('#lblLock').text("Are you sure you want to unlock this payroll transaction?");
                }
                $('#LockTransaction').modal();
            });

            $('#MainContent_BtnException').click(function () {
                $('#ExceptionReport').modal();
            });

            $('#BtnPosting').click(function () {
                $("#BtnPosting").hide();

                var BatchNo = "<%=Session("BatchNo") %>";
                var ArrEmpList = "<%=Session("PostingEmpList") %>";
                var PDate = "<%=Session("PayDate") %>";

                ArrEmpList = ArrEmpList.split(",");

                var i = 0;
                while (i < ArrEmpList.length) {
                    var xhttp = new XMLHttpRequest();
                    xhttp.onreadystatechange = function () {
                        if (this.readyState == 4 && this.status == 200) {
                            document.getElementById("demo").innerHTML = this.responseText + ArrEmpList.length;
                        }
                    };

                    xhttp.open("POST", "XMLGenerateReport", true);
                    xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                    xhttp.send("EmpId=" + ArrEmpList[i] + "&PDate=" + PDate + "&BatchNo=" + BatchNo + "&Ctr=" + i);

                    i++;
                }
            });
        });

        function ShowDetails() {
            $('#PayElementDetails').modal();
        }
    </script>

    <style>
        .Browse {
            width: 100%;
        }

        .Lbl {
            padding: 0px;
            margin-bottom: -20px
        }

        .hideGridColumn {
            display: none;
        }

        .Exp {
            padding: 0px;
            font-size: 12px;
            margin: 0px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <br />
        <h3>Payroll</h3>
        <div class="row">
            <div class="col-sm-12">

                <div class="btn-group">
                    <button type="button" id="BtnUpload" class="btn btn-primary btn-sm">Create Payroll Run</button>
                    <button type="button" id="BtnReUpload" runat="server" class="btn btn-primary btn-sm">Re-upload One-Time Template</button>
                    <button type="button" id="BtnPost" runat="server" class="btn btn-success btn-sm" data-toggle="modal" data-target="#PostTransaction">Process Payroll</button>
                </div>
                <div class="btn-group">
                    <asp:Button ID="BtnException" runat="server" CssClass="btn btn-sm btn-primary" Text="Exception Report"></asp:Button>
                    <asp:Button ID="BtnDownloadPayReg" runat="server" CssClass="btn btn-sm btn-primary" Text="Download Pay Register"></asp:Button>
                    <asp:Button ID="BtnJVReport" runat="server" CssClass="btn btn-sm btn-primary" Text="Download JV Report"></asp:Button>
                    <button type="button" id="BtnLock" runat="server" class="btn btn-primary btn-sm">Lock Payroll</button>
                    <button type="button" id="BtnGenerateBankReport" runat="server" class="btn btn-primary btn-sm">Generate Bank Report</button>
                </div>
                <div class="btn-group">
                    <asp:Button ID="BtnReload" runat="server" CssClass="btn btn-sm btn-primary" Text="Reload"></asp:Button>
                </div>

            </div>
        </div>
        <br />

        <div class="row">
            <div class="col-sm-7">
                <asp:GridView ID="tblPayrollRun" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="Small"
                    AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                    CssClass="table table-sm table-bordered table-striped" PageSize="15" EnableModelValidation="True"
                    SelectedRowStyle-CssClass="btn btn-info">

                    <Columns>
                        <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                            <HeaderStyle Width="30px"></HeaderStyle>
                        </asp:TemplateField>

                        <asp:BoundField DataField="BatchNo" HeaderText="Reference No">
                            <HeaderStyle Width="150px"></HeaderStyle>
                        </asp:BoundField>

                        <asp:BoundField DataField="PayrollPeriod" HeaderText="Payroll Period"></asp:BoundField>
                        <asp:BoundField DataField="PayDate" HeaderText="Payroll Date"></asp:BoundField>
                        <asp:BoundField DataField="Remarks" HeaderText="Remarks"></asp:BoundField>
                        <asp:BoundField DataField="CreatedBy" HeaderText="Created By"></asp:BoundField>
                        <asp:BoundField DataField="DateCreated" HeaderText="Date Created"></asp:BoundField>
                        <asp:BoundField DataField="PostedBy" HeaderText="Lock By"></asp:BoundField>
                        <asp:BoundField DataField="DatePosted" HeaderText="Date Lock"></asp:BoundField>

                        <asp:BoundField DataField="ProcessBy" HeaderText="ProcessBy" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" />
                        <asp:BoundField DataField="DateProcess" HeaderText="DateProcess" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" />

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
            </div>
            <div class="col-sm-5">
                <asp:GridView ID="tblPayrollRunDetails" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="Small"
                    AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                    CssClass="table table-sm table-bordered table-striped" PageSize="15" EnableModelValidation="True"
                    SelectedRowStyle-CssClass="btn btn-info">

                    <Columns>
                        <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                            <HeaderStyle Width="30px"></HeaderStyle>
                        </asp:TemplateField>

                        <asp:BoundField DataField="EmpCd" HeaderText="Emp Code">
                            <HeaderStyle Width="150px"></HeaderStyle>
                        </asp:BoundField>

                        <asp:BoundField DataField="FullName" HeaderText="FullName"></asp:BoundField>
                        <asp:BoundField DataField="MonthlyRate" HeaderText="MonthlyRate"></asp:BoundField>
                        <%--<asp:BoundField DataField="Remarks" HeaderText="Remarks"></asp:BoundField>
                            <asp:BoundField DataField="CreatedBy" HeaderText="Created By"></asp:BoundField>
                            <asp:BoundField DataField="DateCreated" HeaderText="Date Created"></asp:BoundField>
                            <asp:BoundField DataField="PostedBy" HeaderText="Posted By"></asp:BoundField>
                            <asp:BoundField DataField="DatePosted" HeaderText="Date Posted"></asp:BoundField>--%>
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
            </div>
        </div>

    </div>
    <div id="UploadFiles" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="LblModalTitle" runat="server" Text="Upload Payroll Instructions"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="alert alert-info">
                                <strong>Note:&nbsp;</strong>Make sure the employee master data is up to date before processing the payroll.
                            </div>
                        </div>

                        <div class="col-12">
                            Payroll Period 
                            <asp:DropDownList ID="CmdPayPeriod" runat="server" Width="" CssClass="form-control form-control-sm"></asp:DropDownList>
                            <br />
                            Enter payroll cut-off: 
                            <div class="form-inline">

                                <asp:TextBox ID="TxtCFrom" runat="server" CssClass="form-control form-control-sm col-5" Placeholder="MM/DD/YYYY"></asp:TextBox>
                                &nbsp; To &nbsp;
                                <asp:TextBox ID="TxtCTo" runat="server" CssClass="form-control form-control-sm col-5" Placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>

                            Target Payout Date Release: 
                            <asp:TextBox ID="TxtTargetPaydate" runat="server" CssClass="form-control form-control-sm col-5" Placeholder="MM/DD/YYYY"></asp:TextBox>
                            <%--<br />
                            <label class="text-primary">Pay Element Recurring Template: </label>
                            <asp:FileUpload ID="TxtFileName" runat="server" CssClass="form-control-file border" />--%>
                            <br />
                            <label class="text-primary">Provide your pay instruction template: </label>
                            <asp:FileUpload ID="TxtFileNameOneTime" runat="server" CssClass="form-control-file border" />

                            <br />
                            Remarks (Optional):
                            <asp:TextBox ID="TxtRemarks" runat="server" placeholder="Enter remarks, if any" CssClass="form-control form-control-sm col-12 border" Rows="3" TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>

                </div>

                <div class="modal-footer">
                    <input type="button" id="BtnSubmitFileInstruction" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="ReUploadFiles" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label20" runat="server" Text="Re-Upload One-Time Template"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>

                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="alert alert-info">
                                <strong>Note:&nbsp;</strong>Once you re-upload the one-time deduction template, make sure you click the "Process Payroll" Button.
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <label class="text-primary">Pay Element One-Time Template: </label>
                            <asp:FileUpload ID="TxtReUploadOneTime" runat="server" CssClass="form-control-file border" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnReUploadTempalte" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    <div id="ExceptionReport" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label19" runat="server" Text="Exception Report"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body" style="background-color: #eeeded">
                    <div class="row">
                        <div class="col-sm-12">
                            <ul class="list-group Exp">
                                <li class="list-group-item active" style="font-size: 12px; padding: 8px; margin: 0px">Duplicate instruction/wage type</li>
                                <li class="list-group-item" style="font-size: 12px; padding: 0px; margin: 0px">
                                    <table class="table table-bordered table-sm" style="font-size: 12px; padding: 0px; margin: 0px">
                                        <thead>
                                            <tr>
                                                <th style="width: 120px">Employee Code</th>
                                                <th>Full Name</th>
                                                <th>Wage Type</th>
                                                <th>Amount</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%=DuplicateWageType %>
                                        </tbody>
                                    </table>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12">
                            <ul class="list-group Exp">
                                <li class="list-group-item active" style="font-size: 12px; padding: 8px; margin: 0px">Resigned employees</li>
                                <li class="list-group-item" style="font-size: 12px; padding: 0px; margin: 0px">
                                    <table class="table table-bordered table-sm" style="font-size: 12px; padding: 0px; margin: 0px">
                                        <thead>
                                            <tr>
                                                <th style="width: 120px">Employee Code</th>
                                                <th>Full Name</th>
                                                <th>Date Resigned</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%=TagAsResigned %>
                                        </tbody>
                                    </table>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12">
                            <ul class="list-group Exp">
                                <li class="list-group-item active" style="font-size: 12px; padding: 8px; margin: 0px">Inactive employees</li>
                                <li class="list-group-item" style="font-size: 12px; padding: 0px; margin: 0px">
                                    <table class="table table-bordered table-sm" style="font-size: 12px; padding: 0px; margin: 0px">
                                        <thead>
                                            <tr>
                                                <th style="width: 120px">Employee Code</th>
                                                <th>Full Name</th>
                                                <th>Employess Status</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%=InActiveEmp %>
                                        </tbody>
                                    </table>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12">
                            <ul class="list-group Exp">
                                <li class="list-group-item active" style="font-size: 12px; padding: 8px; margin: 0px">Employee not found in Masterdata</li>
                                <li class="list-group-item" style="font-size: 12px; padding: 0px; margin: 0px">
                                    <table class="table table-bordered table-sm" style="font-size: 12px; padding: 0px; margin: 0px">
                                        <thead>
                                            <tr>
                                                <th style="width: 120px">Employee Code</th>
                                                <th>Remarks</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%=EmpNotFound %>
                                        </tbody>
                                    </table>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12">
                            <ul class="list-group Exp">
                                <li class="list-group-item active" style="font-size: 12px; padding: 8px; margin: 0px">Wage Type not existing</li>
                                <li class="list-group-item" style="font-size: 12px; padding: 0px; margin: 0px">
                                    <table class="table table-bordered table-sm" style="font-size: 12px; padding: 0px; margin: 0px">
                                        <thead>
                                            <tr>
                                                <th style="width: 120px">Wage Type Code</th>
                                                <th>Remarks</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%=WageTypeNotFound %>
                                        </tbody>
                                    </table>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    <div id="PayElementDetails" class="modal fade" role="dialog">
        <div class="modal-dialog modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label1" runat="server" Text="Pay Element Details"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-4">
                            <label class="text-Default Lbl" id="Label3" runat="server"><small>Employee Code:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblEmpCode" runat="server"></label>
                            </small>
                            <br />
                            <label class="text-Default Lbl" id="Label2" runat="server"><small>Employee Name:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblFullNme" runat="server"></label>
                            </small>
                        </div>
                        <div class="col-sm-2">
                            <label class="text-Default Lbl" id="Label4" runat="server"><small>Monthly Basic Allowance:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblBasicAllowance" runat="server"></label>
                            </small>
                            <br />
                            <label class="text-Default Lbl" id="Label6" runat="server"><small>TIN No:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblTin" runat="server"></label>
                            </small>
                        </div>
                        <div class="col-sm-2">
                            <label class="text-Default Lbl" id="Label14" runat="server"><small>VAT Registration:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblInNonVat" runat="server"></label>
                            </small>

                            <br />
                            <label class="text-Default Lbl" id="Label15" runat="server"><small>Pay Date:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblPayDate" runat="server"></label>
                            </small>
                        </div>
                        <div class="col-sm-2">
                            <label class="text-Default Lbl" id="Label17" runat="server"><small>Vat Percent:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblVatPercent" runat="server">%</label>
                            </small>
                            <br />
                            <label class="text-Default Lbl" id="Label13" runat="server"><small>Withholding Tax Rate:</small></label><br />
                            <small>
                                <label class="text-primary" id="LblTaxRate" runat="server">%</label>
                            </small>
                        </div>

                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-4">
                            <h4><small>
                                <asp:Label ID="Label5" runat="server" Text="Earning"></asp:Label></small></h4>

                            <h5><small>
                                <asp:Label ID="Label8" runat="server" CssClass="text-info" Text="Recurring"></asp:Label></small></h5>
                            <%=EarningRecurring %>

                            <h5><small>
                                <asp:Label ID="Label9" runat="server" CssClass="text-info" Text="One-Time"></asp:Label></small></h5>
                            <%=EarningOneTime %>
                        </div>

                        <div class="col-4">
                            <h4><small>
                                <asp:Label ID="Label7" runat="server" Text="Deduction"></asp:Label></small></h4>
                            <h5><small>
                                <asp:Label ID="Label10" runat="server" CssClass="text-info" Text="Recurring"></asp:Label></small></h5>
                            <%=DeductionRecurring %>

                            <h5><small>
                                <asp:Label ID="Label11" runat="server" CssClass="text-info" Text="One-Time"></asp:Label></small></h5>
                            <%=DeductionOneTime %>
                        </div>

                        <div class="col-4">
                            <h4><small>
                                <asp:Label ID="Label12" runat="server" Text="Summary"></asp:Label></small></h4>
                            <%=SummaryComputation%>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="PostTransaction" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label16" runat="server" Text="Process Payroll"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="alert alert-info">
                                <%--<strong>Note:&nbsp;</strong>--%>
                                Are you sure you want to process this transaction?
                                <br />
                                If Yes, click the submit button.
                            </div>
                        </div>
                        <div class="col-sm-12">
                            <p id="demo" class="text-info"></p>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnPosting" value="Submit" class="btn btn-sm btn-primary" />
                    <input type="button" id="BtnClose" runat="server" value="Close" class="btn btn-sm btn-danger" />
                </div>
            </div>
        </div>
    </div>
    <div id="LockTransaction" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label18" runat="server" Text="Lock and Unlock Payroll Transaction"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="alert alert-danger">
                                <%--<strong>Note:&nbsp;--%>
                                <label id="lblLock"></label>
                                <br />
                                If Yes, click the submit button.
                            </div>
                        </div>

                    </div>

                </div>

                <div class="modal-footer">
                    <input type="button" id="BtnSubmitLock" value="Submit" runat="server" class="btn btn-sm btn-primary" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

