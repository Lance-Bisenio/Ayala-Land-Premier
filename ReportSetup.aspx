<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="ReportSetup.aspx.vb" Inherits="ReportSetup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     <script src="Scripts/jquery-3.5.1.js"></script>

    <style>
        .border1 {
            border: solid 1px #000
        }

        .Pad1 {
            padding-top: -10px;
            margin-top: -9px;
        }

        .Pad2 {
            margin-top: -5px;
            margin-bottom: -5px;
        }

        .Pad3 {
            padding-left: 20px;
        }

        .Pad4 {
            margin-top: -10px;
            margin-bottom: -10px;
        }

        .BtmLine {
            border-bottom: solid 1px #000;
        }

        .BtmLine1 {
            border-bottom: double 2px #000;
        }

        .PadRZ {
            padding-right: 0px
        }
    </style>
    <script>
        $(document).ready(function () {

            $('#MainContent_BtnPublishPayroll').click(function () {
                $('#PublishDetails').modal();
            });

        });

        function OpenForm() {
            var Token = "<%=Session("Token") %>";

            var w = window.open("Payslip.aspx?Token=" + Token, "popupWindow", "width=1200, height=750, scrollbars=yes, top=50, left=50");
            var $w = $(w.document.body);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <br />
        <h3>Payslip Report Management</h3>
        <div class="row divPad" id="EmpFilter" runat="server">
            <div class="col-sm-3">
                <div class="row">
                    <div class="col-md-12">
                        <button type="button" id="BtnPublishPayroll" runat="server" class="btn btn-primary btn-sm">Publish Payroll</button> 
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <small>Select employee to view payslip:</small>
                        <asp:DropDownList ID="CmdEmployeeList" runat="server" Width="" CssClass="form-control form-control-sm" AutoPostBack="true"></asp:DropDownList>&nbsp;
                    </div>
                </div>

            </div>
            <div class="col-sm-9"></div>
        </div> 

        <div class="row">
            <div class="col-sm-4">
                <h5>Payslip Report</h5>
                <asp:GridView ID="tblPayrollRun" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="12px"
                    AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                    CssClass="table table-sm table-bordered table-striped" PageSize="6" EnableModelValidation="True"
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
                       
                        <asp:CommandField ButtonType="Button" ShowSelectButton="True" SelectText="View">
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
            <div class="col-sm-4">
                <h5>2307 Report</h5>
                <asp:GridView ID="Tbl2307" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="12px"
                    AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                    CssClass="table table-sm table-bordered table-striped" PageSize="6" EnableModelValidation="True"
                    SelectedRowStyle-CssClass="btn btn-info">

                    <Columns>
                        <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                            <HeaderStyle Width="30px"></HeaderStyle>
                        </asp:TemplateField>

                        <asp:BoundField DataField="BatchNumber" HeaderText="Reference No">
                            <HeaderStyle Width="150px"></HeaderStyle>
                        </asp:BoundField>

                        <asp:BoundField DataField="Periodfrom" HeaderText="Period From"></asp:BoundField>
                        <asp:BoundField DataField="PeriodTo" HeaderText="Period To"></asp:BoundField>
                        

                        <asp:CommandField ButtonType="Button" ShowSelectButton="True" SelectText="View">
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

    <div id="PublishDetails" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label18" runat="server" Text="Publish Payroll Payslip"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="alert alert-danger"> 
                                
                                Once you publish a payroll transaction, it will trigger an email notification to all active employees.
                                
                            </div>
                        </div>
                    </div>
                    <div class="row">
                    <div class="col-md-12">
                        <small>Select locked payroll cutoff to publish:</small>
                        <asp:DropDownList ID="CmdLockedPayrollRun" runat="server" Width="" CssClass="form-control form-control-sm" ></asp:DropDownList>&nbsp;
                    </div>
                </div>
                </div>

                <div class="modal-footer">
                    <input type="button" id="BtnSubmitPublish" value="Submit" runat="server" class="btn btn-sm btn-primary" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

