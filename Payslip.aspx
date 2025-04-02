<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Payslip.aspx.vb" Inherits="Payslip" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
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
            $('#BtnPrint').click(function () {
                $('#BtnPrint').hide();
                window.print();
            });
        });

    </script>


</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <div class="row">
                <div class="col-sm-2">
                    <br />
                    <button type="button" name="BtnPrint" id="BtnPrint" class="btn btn-primary btn-sm">Print</button></div>
                <div class="col-sm-8">
                    <br />
                    <div class="row">
                        <div class="col-sm-12 border1">
                            <div class="row">
                                <div class="col-sm-12 text-center">
                                    <h6><small><b>AYALALAND PREMIER, INC.</b></small></h6>
                                </div>
                                <div class="col-sm-12 text-center">
                                    <h6 class="Pad1"><small><b>VAT TIN: 216-919-045-000</b></small></h6>
                                </div>
                                <div class="col-sm-12 text-center">
                                    <h6 class="Pad1"><small><b>18F Tower One and Exchange Plaza, Ayala Avenue, Makati City</b></small></h6>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="col-sm-12 border1 text-center">
                                        <h6><small><b>SELLERS OPERATING FUND PAYMENT SLIP</b></small></h6>
                                    </div>
                                </div>
                            </div>
                            <br />

                            <div class="row" style="font-size: 14px">
                                <div class="col-sm-6">
                                    <div class="col-sm-12 border1" style="height: 170px">
                                        <div class="row">
                                            <div class="col-sm-12 text-left"><small><b><u>Seller's Information:</u></b></small></div>
                                        </div>
                                        <div class="row">
                                            <div class="col-sm-4 text-right" style="padding-right: 0px"><small>BP/Vendor Code:</small></div>
                                            <div class="col-sm-8 text-left">
                                                <small><b>
                                                    <label id="LblVendorCode" runat="server"></label>
                                                </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-4 text-right" style="padding-right: 0px"><small>Name:</small></div>
                                            <div class="col-sm-8 text-left">
                                                <small><b>
                                                    <label id="LblName" runat="server"></label>
                                                </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-4 text-right" style="padding-right: 0px"><small>TIN:</small></div>
                                            <div class="col-sm-8 text-left">
                                                <small><b>
                                                    <label id="LblTin" runat="server"></label>
                                                </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-4 text-right" style="padding-right: 0px"><small>VAT Registration:</small></div>
                                            <div class="col-sm-8 text-left">
                                                <small><b>
                                                    <label id="LblVatReg" runat="server"></label>
                                                </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-4 text-right" style="padding-right: 0px; vertical-align: top"><small>Registered Address:</small></div>
                                            <div class="col-sm-8 text-left" style="vertical-align: top">
                                                <small><b>
                                                    <label id="LblAddress" runat="server"></label>
                                                </b>
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
                                                    <b>
                                                        <label id="LblStatus" runat="server"></label>
                                                    </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Division:</small></div>
                                            <div class="col-sm-7 text-left">
                                                <small>
                                                    <b>
                                                        <label id="LblDiv" runat="server"></label>
                                                    </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Group:</small></div>
                                            <div class="col-sm-7 text-left">
                                                <small>
                                                    <b>
                                                        <label id="LblGroup" runat="server"></label>
                                                    </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Monthly Rate:</small></div>
                                            <div class="col-sm-7 text-left">
                                                <small>
                                                    <b>
                                                        <label id="LblMRate" runat="server"></label>
                                                    </b>
                                                </small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Withholding Tax Rate:</small></div>
                                            <div class="col-sm-7 text-left">
                                                <small>
                                                    <b>
                                                        <label id="LblWHTax" runat="server">%</label></b></small>
                                            </div>
                                        </div>
                                        <div class="row Pad1">
                                            <div class="col-sm-5 text-right" style="padding-right: 0px"><small>Pay Date:</small></div>
                                            <div class="col-sm-7 text-left">
                                                <small>
                                                    <b>
                                                        <label id="LblPayDate" runat="server"></label>
                                                    </b>
                                                </small>
                                            </div>
                                        </div>
                                        <br />
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row" style="font-size: 14px">
                                <div class="col-sm-6">
                                    <div class="row">
                                        <div class="col-sm-6 text-left"></div>
                                        <div class="col-sm-3 text-right"><small><b><u>This Period</u></b></small></div>
                                        <div class="col-sm-3 text-left" style="padding-left: 0px"><small><b><u>Year To Date</u></b></small></div>
                                    </div>
                                    <div class="row Pad1">
                                        <div class="col-sm-12 text-left"><small><b>Taxable:</b></small></div>
                                    </div>
                                    <div class="row Pad2">
                                        <div class="col-sm-6 text-left"><small>Basic Allowance:</small></div>
                                        <div class="col-sm-3 text-right">
                                            <small>
                                                <label id="LblBasic" runat="server"></label>
                                            </small>
                                        </div>
                                        <div class="col-sm-3 text-right">
                                            <div class="row">
                                                <div class="col-sm-10 text-right">
                                                    <small>
                                                        <label id="Label8" runat="server"><label id="LblBasicHisto" runat="server"></label></label></small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%=Earnings %>
                                    <%=EarningsHisto %>
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
                                                <div class="col-sm-10 text-right">
                                                    <small>
                                                        <label id="LblEWTHisto" runat="server">0.00</label></small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%=Deduction %>
                                    <%=DeductionHisto %>
                                </div>
                            </div>

                            <div class="row" style="font-size: 14px">
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
                                        <div class="col-sm-3 text-right">
                                            <small><b>
                                                <label id="LblTtlTaxable" runat="server"></label>
                                            </b></small>
                                        </div>
                                        <div class="col-sm-3 text-right">
                                            <div class="row">
                                                <div class="col-sm-10 text-right">
                                                    <small><b>
                                                        <label id="LblTtlTaxableHisto" runat="server">0.00</label></b></small>
                                                        <%--<label id="LblWHTHisto" runat="server">0.00</label></b></small>--%>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row Pad2">
                                        <div class="col-sm-6 text-left"><small><b>Add: 12% Input VAT:</b></small></div>
                                        <div class="col-sm-3 text-right BtmLine">
                                            <small><b>
                                                <label id="LblInputVat" runat="server"></label>
                                            </b></small>
                                        </div>
                                        <div class="col-sm-3 text-right">
                                            <div class="row">
                                                <div class="col-sm-10 text-right BtmLine">
                                                    <small><b>
                                                        <label id="LblInputVatHisto" runat="server">0.00x</label></b></small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row Pad2">
                                        <div class="col-sm-6 text-left"><small><b>Grand Total:</b></small></div>
                                        <div class="col-sm-3 text-right BtmLine1">
                                            <small><b>
                                                <label id="LblGrandTtl" runat="server"></label>
                                            </b></small>
                                        </div>
                                        <div class="col-sm-3 text-right">
                                            <div class="row">
                                                <div class="col-sm-10 text-right BtmLine1">
                                                    <small><b>
                                                        <label id="LblGrandTotalEarningHisto" runat="server">0.00</label></b></small>
                                                </div>
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
                                        <div class="col-sm-3 text-right">
                                            <small><b>
                                                <label id="LblTtlDeduc" runat="server"></label>
                                            </b></small>
                                        </div>
                                        <div class="col-sm-3 text-right">
                                            <div class="row">
                                                <div class="col-sm-10 text-right">
                                                    <small><b>
                                                        <label id="LblTotalDecucHisto" runat="server"></label></b></small>
                                                </div>
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
                                        <div class="col-sm-3 text-right BtmLine1">
                                            <small><b>
                                                <label id="LblNetPay" runat="server"></label>
                                            </b></small>
                                        </div>
                                        <div class="col-sm-3 text-right">
                                            <div class="row">
                                                <div class="col-sm-10 text-right BtmLine1">
                                                    <small><b>
                                                        <label id="LblNetHisto" runat="server">0.00</label></b></small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <br />
                            <div class="row" style="font-size: 14px">
                                <div class="col-sm-12">
                                    <div class="col-sm-12 border1 text-left"><b><small>Remarks:</small></b></div>
                                </div>
                            </div>
                            <br />
                            <div class="row" style="font-size: 14px">
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
                                        <div class="col-sm-2 text-right">
                                            <small>
                                                <label id="LblBVATable" runat="server" />
                                            </small>
                                        </div>
                                        <div class="col-sm-7 text-left">&nbsp;</div>
                                    </div>
                                    <div class="row Pad2">
                                        <div class="col-sm-3 text-left"><small class="Pad3">VAT Amount:</small></div>
                                        <div class="col-sm-2 text-right BtmLine">
                                            <small>
                                                <label id="LblBVatAmt" runat="server" class="Pad3" />
                                            </small>
                                        </div>
                                        <div class="col-sm-7 text-left">&nbsp;</div>
                                    </div>
                                    <div class="row Pad2">
                                        <div class="col-sm-3 text-left"><small><b class="Pad3">Total Sales</b></small></div>
                                        <div class="col-sm-2 text-right">
                                            <small><b>
                                                <label id="LblBGrandTtl" runat="server" class="Pad3" />
                                            </b></small>
                                        </div>
                                        <div class="col-sm-7 text-left">&nbsp;</div>
                                    </div>
                                    <div class="row Pad2">
                                        <div class="col-sm-3 text-left"><small><b class="Pad3">Less: EWT</b></small></div>
                                        <div class="col-sm-2 text-right BtmLine">
                                            <small><b>
                                                <label id="LblBEWT" runat="server" class="Pad3" />
                                            </b></small>
                                        </div>
                                        <div class="col-sm-7 text-left">&nbsp;</div>
                                    </div>
                                    <div class="row Pad2">
                                        <div class="col-sm-3 text-left"><small><b class="Pad3">Net Amount</b></small></div>
                                        <div class="col-sm-2 text-right BtmLine">
                                            <small><b>
                                                <label id="LblBNetPay" runat="server" class="Pad3" />
                                            </b></small>
                                        </div>
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
                    </div>
                    <br />
                </div>
                <div class="col-sm-2"></div>
            </div>
        </div>
    </form>
</body>
</html>
