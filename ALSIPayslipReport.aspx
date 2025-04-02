<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="ALSIPayslipReport.aspx.vb" Inherits="ALSIPayslipReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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

            var w = window.open("ALSIPayslip.aspx?Token=" + Token, "popupWindow", "width=1200, height=750, scrollbars=yes, top=50, left=50");
            var $w = $(w.document.body);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <br />
        <h3>AYALA LAND SALES INC.</h3>
         
        <br />

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
                        <%--                        <asp:BoundField DataField="Remarks" HeaderText="Remarks"></asp:BoundField>
                        <asp:BoundField DataField="CreatedBy" HeaderText="Created By"></asp:BoundField>
                        <asp:BoundField DataField="DateCreated" HeaderText="Date Created"></asp:BoundField>
                        <asp:BoundField DataField="PostedBy" HeaderText="Posted By"></asp:BoundField>
                        <asp:BoundField DataField="DatePosted" HeaderText="Date Posted"></asp:BoundField>--%>
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
            <%--<div id="Payslip" class="col-sm-8">


                <%--<div class="row">
                    <div class="col-sm-10 border1">
                        <div class="row">
                            <div class="col-sm-12 text-center"><b>AYALA LAND SALES INC.</b></div>
                            <div class="col-sm-12 text-center"><b>VAT TIN: 216-919-045-000</b></div>
                            <div class="col-sm-12 text-center"><b>18F Tower One and Exchange Plaza, Ayala Avenue, Makati City</b></div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="col-sm-12 border1 text-center"><b>SELLERS OPERATING FUND PAYMENT SLIP</b></div>
                            </div>
                        </div>
                        <br />

                        <div class="row" style="font-size:14px">
                            <div class="col-sm-6">
                                <div class="col-sm-12 border1" style="height: 170px">
                                    <div class="row">
                                        <div class="col-sm-12 text-left"><small><b><u>Seller's Information:</u></b></small></div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4 text-right" style="padding-right: 0px"><small>BP/Vendor Code:</small></div>
                                        <div class="col-sm-8 text-left">
                                            <small><b>
                                                <label id="LblVendorCode" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-4 text-right" style="padding-right: 0px"><small>Name:</small></div>
                                        <div class="col-sm-8 text-left">
                                            <small><b>
                                                <label id="LblName" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-4 text-right" style="padding-right: 0px"><small>TIN:</small></div>
                                        <div class="col-sm-8 text-left">
                                            <small><b>
                                                <label id="LblTin" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-4 text-right" style="padding-right: 0px"><small>VAT Registration:</small></div>
                                        <div class="col-sm-8 text-left">
                                            <small><b>
                                                <label id="LblVatReg" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-4 text-right" style="padding-right: 0px"><small>Registered Address:</small></div>
                                        <div class="col-sm-8 text-left">
                                            <small><b>
                                                <label id="LblAddress" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="col-sm-12 border1" style="height: 170px">
                                    <div class="row">
                                        <div class="col-sm-12 text-left"><small><b><u>Seller's Information:</u></b></small></div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Status:</small></div>
                                        <div class="col-sm-7 text-left">
                                            <small>
                                                <b><label id="LblStatus" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Division:</small></div>
                                        <div class="col-sm-7 text-left">
                                            <small>
                                                <b><label id="LblDiv" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Group:</small></div>
                                        <div class="col-sm-7 text-left">
                                            <small>
                                                <b><label id="LblGroup" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Monthly Rate:</small></div>
                                        <div class="col-sm-7 text-left">
                                            <small>
                                                <b><label id="LblMRate" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Withholding Tax Rate:</small></div>
                                        <div class="col-sm-7 text-left">
                                            <small>
                                                <b><label id="LblWHTax" runat="server">%</label></b></small>
                                        </div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Pay Date:</small></div>
                                        <div class="col-sm-7 text-left">
                                            <small>
                                                <b><label id="LblPayDate" runat="server"></label></b>
                                            </small>
                                        </div>
                                    </div>
                                    <br />
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="row" style="font-size:14px">
                            <div class="col-sm-6">
                                <div class="row">
                                    <div class="col-sm-6 text-left"></div>
                                    <div class="col-sm-3 text-left"><small><b><u>This Period</u></b></small></div>
                                    <div class="col-sm-3 text-left" style="padding-left: 0px"><small><b><u>Year To Date</u></b></small></div>
                                </div>
                                <div class="row Pad1">
                                    <div class="col-sm-12 text-left"><small><b>Taxable:</b></small></div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left"><small>Basic:</small></div>
                                    <div class="col-sm-3 text-right"><small><label id="LblBasic" runat="server"></label></small></div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right"><small><label id="Label8" runat="server" >-</label>&nbsp;&nbsp;</small></div>
                                        </div>
                                    </div>
                                </div>
                                <%=Earnings %>
                            </div>
                            <div class="col-sm-6">
                                <div class="row">
                                    <div class="col-sm-6 text-left"></div>
                                    <div class="col-sm-3 text-left"><small><b><u>This Period</u></b></small></div>
                                    <div class="col-sm-3 text-left" style="padding-left: 0px"><small><b><u>Year To Date</u></b></small></div>
                                </div>
                                <div class="row Pad1">
                                    <div class="col-sm-12 text-left"><small><b>Less: Deductions:</b></small></div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left"><small>Withholding Tax (EWT):</small></div>
                                    <div class="col-sm-3 text-right"><small>(<label id="LblEWT" runat="server"></label>)</small></div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right"><small><label id="Label4" runat="server" >-</label>&nbsp;&nbsp;</small></div>
                                        </div>
                                    </div>
                                </div>
                                <%=Deduction %>
                            </div>
                        </div>

                        <div class="row" style="font-size:14px">
                            <div class="col-sm-6"> 
                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left">&nbsp;</div>
                                    <div class="col-sm-3 text-right BtmLine">&nbsp;</div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right BtmLine"><small><b>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left"><small><b>Total Taxable:</b></small></div>
                                    <div class="col-sm-3 text-right"><small><b><label id="LblTtlTaxable" runat="server"></label></b></small></div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right"><small><b><label id="Label3" runat="server" >0.00</label>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left"><small><b>Add: 12% Input VAT:</b></small></div>
                                    <div class="col-sm-3 text-right BtmLine"><small><b><label id="LblInputVat" runat="server"></label></b></small></div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right BtmLine"><small><b><label id="Label1" runat="server" >0.00</label>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left"><small><b>Grand Total:</b></small></div>
                                    <div class="col-sm-3 text-right BtmLine1"><small><b><label id="LblGrandTtl" runat="server"></label></b></small></div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right BtmLine1"><small><b><label id="Label2" runat="server" >0.00</label>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div> 
                            </div>
                            <div class="col-sm-6"> 
                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left">&nbsp;</div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-12 text-right BtmLine"><small><b>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right BtmLine"><small><b>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left"><small><b>Total Deduction:</b></small></div>
                                    <div class="col-sm-3 text-right"><small><b><label id="LblTtlDeduc" runat="server"></label></b></small></div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right"><small><b><label id="Label5" runat="server" >0.00</label>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left">&nbsp;</div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-12 text-right BtmLine"><small><b>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right BtmLine"><small><b>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div>
                                 

                                <div class="row Pad2">
                                    <div class="col-sm-6 text-left"><small><b>Net Amount:</b></small></div>
                                    <div class="col-sm-3 text-right BtmLine1"><small><b><label id="LblNetPay" runat="server"></label></b></small></div>
                                    <div class="col-sm-3 text-right">
                                        <div class="row">
                                            <div class="col-sm-10 text-right BtmLine1"><small><b><label id="Label6" runat="server" >0.00</label>&nbsp;&nbsp;</b></small></div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <br />
                        <div class="row" style="font-size:14px">
                            <div class="col-sm-12">
                                <div class="col-sm-12 border1 text-left"><b><small>Remarks:</small></b></div>
                            </div>
                        </div>
                        <br />
                        <div class="row" style="font-size:14px">
                            <div class="col-sm-12">

                                <div class="row Pad1">
                                    <div class="col-sm-12 text-left">
                                        <small>Please issue <b>BIR Registered Official Receipts</b> upon receipt of Operating Fund with the following details: </small>

                                    </div>
                                </div>
                                <br />
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small class="Pad3">Company Name:</small></div>
                                    <div class="col-sm-9 text-left"><small><b>AYALA LAND SALES INC.</b></small></div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small class="Pad3">TIN:</small></div>
                                    <div class="col-sm-9 text-left"><small>216-919-045-000</small></div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small class="Pad3">Address:</small></div>
                                    <div class="col-sm-9 text-left"><small>18F Tower One and Exchange Plaza, Ayala Avenue, Makati City</small></div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small class="Pad3">Business Style:</small></div>
                                    <div class="col-sm-9 text-left"><small>Real Estate Brokerage</small></div>
                                </div>
                                <br />

                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small class="Pad3">VATAble Sales: </small></div>
                                    <div class="col-sm-2 text-right"><small><label id="LblBVATable" runat="server" /></small></div>
                                    <div class="col-sm-7 text-left">&nbsp;</div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small class="Pad3">VAT Amount:</small></div>
                                    <div class="col-sm-2 text-right BtmLine"><small><label id="LblBVatAmt" runat="server" class="Pad3" /></small></div>
                                    <div class="col-sm-7 text-left">&nbsp;</div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small><b class="Pad3">Total Sales</b></small></div>
                                    <div class="col-sm-2 text-right"><small><b><label id="LblBGrandTtl" runat="server" class="Pad3" /></b></small></div>
                                    <div class="col-sm-7 text-left">&nbsp;</div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small><b class="Pad3">Less: EWT</b></small></div>
                                    <div class="col-sm-2 text-right BtmLine"><small><b><label id="LblBEWT" runat="server" class="Pad3" /></b></small></div>
                                    <div class="col-sm-7 text-left">&nbsp;</div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-3 text-left"><small><b class="Pad3">Net Amount</b></small></div>
                                    <div class="col-sm-2 text-right BtmLine"><small><b><label id="LblBNetPay" runat="server" class="Pad3" /></b></small></div>
                                    <div class="col-sm-7 text-left">&nbsp;</div>
                                </div>
                                <br />
                                <div class="row Pad2">
                                    <div class="col-sm-12 text-left">
                                        <small><b>Amount of withholding tax is supported by BIR Form 2307 as certification that Ayala Land Sales Inc. withheld the said amount.</b></small>
                                    </div>
                                </div>
                                <div class="row Pad2">
                                    <div class="col-sm-12 text-left">
                                        <small><b>The said 2307 form is scheduled to be released on or before 3rd week of the month after the applicable calendar quarter. (State BIR RMC)</b></small>
                                    </div>
                                </div>
                                <br />
                                <div class="row Pad2">
                                    <div class="col-sm-12 text-left">
                                        <small style="font-size: 11px"><b><i>Note: Please keep this slip with high confidentiality and for any related purpose(s) it may serve. Thank you!</i></b></small>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <br />



                    </div>
                    <div class="col-sm-1"></div>
                </div>-- %>
            </div>--%>
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

