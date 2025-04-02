<%@ Page Title="Job Order" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="JobOrderTask.aspx.vb" Inherits="JobOrderTask" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .textPad1 {
            padding-top: -10px;
            margin-top: -10px
        }

        .textPad2 {
            line-height: 1.2;
        }

        .pads {
            margin-left: 5px;
        }

        h6, h6 small {
            padding-bottom: -5px;
            margin-bottom: -5px
        }
    </style>

    <script type="text/javascript"> 

        function ExecuteForm(str) {
            var xhttp;
            if (str == "") {
                document.getElementById("txtHint").innerHTML = "";
                return;
            }

            xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200) {
                    document.getElementById("txtHint").innerHTML = this.responseText;
                }
            };

            var pRecTranId = $("#h_RecTranId").val();
            var pPONO = $("#h_PONO").val();
            var pItemCode = $("#DDLReceivingItemList").val();
            var pInvoiceNo = $("#TxtInvoiceNum").val();
            var pLotNum = $("#TxtLotNum").val();
            var pExpDate = $("#TxtExpDate").val();
            var pSerialNo = $("#TxtSerialNum").val();
            var pQty = $("#TxtQty").val();
            var pUnitCost = $("#TxtUnitCost").val();
            var pRemarks = $("#TxtRecRemarks").val();

            var DataParams = 'PONO=' + pPONO +
                '&ItemCd=' + pItemCode +
                '&InvoiceNo=' + pInvoiceNo +
                '&LotNum=' + pLotNum +
                '&ExpDate=' + pExpDate +
                '&SerialNo=' + pSerialNo +
                '&Qty=' + pQty +
                '&UnitCost=' + pUnitCost +
                '&Remarks=' + pRemarks +
                '&RecTranId=' + pRecTranId;

            xhttp.open("GET", "purchase-orderexecute.aspx?" + DataParams, true);
            xhttp.send();

            if (pRecTranId != "") {
                Alert("Successfully saved");
                $("#ModalReceiving").modal("hide");
            }
        }

        $("#DDLReceivingItemList").change(function (e) {
            document.getElementById("txtHint").innerHTML = "";
        });

        function ModifyReceiving(pTranId, pItemCd, InvoiceNum, LotNum, ExpDate, SerialNum, Item_Qty, Item_UnitCost, Remarks) {
            $("#h_RecTranId").val(pTranId);
            $("#DDLReceivingItemList").val(pItemCd);
            $("#TxtInvoiceNum").val(InvoiceNum);
            $("#TxtLotNum").val(LotNum);
            $("#TxtExpDate").val(ExpDate);
            $("#TxtSerialNum").val(SerialNum);
            $("#TxtQty").val(Item_Qty);
            $("#TxtUnitCost").val(Item_UnitCost);
            $("#TxtRecRemarks").val(Remarks);

            $("#ModalReceiving").modal("show");
        }

        function ReOpenModal() {
            $("#myModal").modal("show");
            $("#txtDateFrom").focus();
        }

<%--    function invoke() {
        <%=vScript %>
    }--%>

        function ModifyJoHeader(pId, pJONO, pSupp) {
            $("#h_TranId").val(pId);
            $("#h_PONO").val(pJONO);
            $("#h_Supplier").val(pSupp);
            $('#form1').submit();
        }

        $(document).ready(function () {
            $(document).ready(function () {
                $("#txtDateFrom").datepicker();
                $("#txtDateTo").datepicker();
                $("#TxtTargetDelDate").datepicker();
                $("#TxtExpDate").datepicker();
            });

            var vProperties = "width=1200px, height=550px, top=50px, left=80px, scrollbars=yes";
            var vParam = "&pJONO=" + $("#h_PONO").val() + "&pSuppCode=" + $("#h_Supplier").val();
            var vDeleteParam = ""

            $('#BtnViewPrint').click(function (event) {
                event.preventDefault();
                winPop = window.open("purchase-viewprint.aspx?pMode=viewprint" + vParam + "", "popupWindow", vProperties);
                winPop.focus();
            });

            $('#BtnReceiveItem').click(function (event) {
                $("#h_RecTranId").val("");
                $("#TxtInvoiceNum").val("");
                $("#TxtLotNum").val("");
                $("#TxtExpDate").val("");
                $("#TxtSerialNum").val("");
                $("#TxtQty").val("");
                $("#TxtUnitCost").val("");
                $("#TxtRecRemarks").val("");
            });
        });
    </script>
    <link href="Content/calendar/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" />
    <script src="Scripts/jquery-ui/jquery-ui.min.js"></script>

</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br />

    <div class="container-fluid">
        <div class="row">
            <div class="col-sm-12">
                <h2 class="text-warning">Job Order</h2>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-3">
                <div class="col-md-12">
                    <small>Job Order Date Created From:</small>
                    <asp:TextBox ID="txtDateFrom" runat="server" Width="" CssClass="form-control form-control-sm" placeholder="MM/DD/YYYY"></asp:TextBox>
                </div>
                <div class="col-md-12">
                    <small>Job Order Date Created To:</small>
                    <asp:TextBox ID="txtDateTo" runat="server" Width="" CssClass="form-control form-control-sm" placeholder="MM/DD/YYYY"></asp:TextBox>
                </div>

            </div>
            <div class="col-sm-3">
                <div class="col-md-12">
                    <small>Job Order Status:</small>
                    <asp:DropDownList ID="DDLPOStatus" runat="server" CssClass="form-control form-control-sm" AutoPostBack="True">
                    </asp:DropDownList>
                </div>
                <div class="col-md-12">
                    <small>Search Job Order Number:</small>
                    <asp:TextBox ID="TxtPONumber" runat="server" Width="" CssClass="form-control form-control-sm" placeholder="Enter Order Number"></asp:TextBox>
                </div>
                <div class="col-md-12">
                </div>
            </div>
            <div class="col-sm-3">
                <div class="col-md-12">
                    <small>Client Name:</small>
                    <asp:DropDownList ID="DDLSupplier" runat="server" CssClass="form-control form-control-sm">
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="row" style="margin-top: 5px; margin-bottom: 5px">
            <div class="col-md-12">
                <div class="col-md-12">
                    <div class="btn-group">
                        <asp:Button ID="BtnAddNew" CssClass="btn btn-primary btn-sm" runat="server" Text="Create New JO" />
                        <asp:Button ID="BtnEdit" CssClass="btn btn-primary btn-sm" runat="server" Text="Edit JO" />
                        <asp:Button ID="BtnSearch" CssClass="btn btn-success btn-sm" runat="server" Text="Search" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12">
                <div class="col-sm-12">
                    <table id="Table2" class="table table-bordered" style="width: 100%; font-size: 12px">
                        <thead>
                            <tr class="bg-light text-center">
                                <th style="width: 50px"></th>
                                <th style="width: 40px">#</th>
                                <th>Job Order No#</th>
                                <th>Client Name</th>
                                <th>Target Delivery Date</th>

                                <th>Date of Loss</th>
                                <th>Insurer Name</th>
                                <th>Insured Name</th>
                                <th>Policy Number</th>
                                <th>Vehicle Description</th>
                                <th>Remarks</th>
                                <th>Status</th>
                                <th>Created By</th>
                                <th>Date Created</th>
                                <%--<th>Total Item</th>
                                    <th>Total Cost</th> --%>
                            </tr>
                        </thead>
                        <%=vRecordData %>
                    </table>
                </div>
            </div>
        </div>

        <div class="row" style="margin-top: 5px; margin-bottom: 5px">
            <div class="col-md-12">
                <div class="col-md-12">

                    <ul class="nav nav-tabs">
                        &nbsp;&nbsp;&nbsp;
                            <li class="nav-item">
                                <asp:Button ID="BtnJODetails" CssClass="nav-link active" runat="server" Text="Job Order Details" />
                            </li>
                        <li class="nav-item">
                            <asp:Button ID="BtnAttachment" CssClass="nav-link" runat="server" Text="Attachment" />
                        </li>
                        <li class="nav-item">
                            <asp:Button ID="BtnEstimate" CssClass="nav-link" runat="server" Text="Estimate" />
                        </li>
                        <li class="nav-item">
                            <asp:Button ID="BtnCategories" CssClass="nav-link" runat="server" Text="Categories" Visible="false" />
                        </li>
                        <li class="nav-item"></li>
                    </ul>

                    <%--<div class="btn-group"> 
                        <asp:Button ID="BtnReceiveItem" CssClass="btn btn-primary btn-sm" runat="server" Text="Receive Item" />
                        <asp:Button ID="BtnPOST" CssClass="btn btn-primary btn-sm" runat="server" Text="POST Transaction" />
                    </div>--%>
                </div>
            </div>
        </div>

        <div class="row" id="DivJODetails" runat="server" visible="True">
            <div class="col-sm-12">
                <div class="col-sm-12">


                    <div class="row">
                        <div class="col-md-4">
                            <div class="col-md-12">
                                <h6><small class="text-muted">Item Code | Product Code or GCAS</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblItemCode" runat="server" CssClass="text-primary" Text="Label"></asp:Label>&nbsp;|
                                <asp:Label ID="lblGCAS" runat="server" CssClass="labelL" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                            <div class="col-md-12">
                                <h6><small class="text-muted">Item Description</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblItemDescr" runat="server" CssClass="labelL" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                            <div class="col-md-12">
                                <h6><small class="text-muted">Customer Details</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblCust_Cd" runat="server" Text="Label"></asp:Label>&nbsp;|
                                <asp:Label ID="lblCustDescr" runat="server" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="col-md-12">
                                <h6><small class="text-muted">Job Order Number</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblJO" runat="server" CssClass="text-primary" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                            <div class="col-md-12">
                                <h6><small class="text-muted">Sale Order Number</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblSO" runat="server" CssClass="labelL" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                            <div class="col-md-12">
                                <h6><small class="text-muted">Purchase Order Number</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblPO" runat="server" CssClass="labelL" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="col-md-12">
                                <h6><small class="text-muted">BOM Code | Revision</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblBOM" runat="server" CssClass="labelL" Text="Label"></asp:Label>&nbsp;|
                                <asp:Label ID="lblBOMRev" runat="server" CssClass="labelL" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                            <div class="col-md-12">
                                <h6><small class="text-muted">Production Start Date</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblProdDate" runat="server" CssClass="labelL" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                            <div class="col-md-12">
                                <h6><small class="text-muted">Qty Order</small></h6>
                                <h5>
                                    <small>
                                        <asp:Label ID="lblQtyOrder" runat="server" CssClass="labelL" Text="Label"></asp:Label>
                                    </small>
                                </h5>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="row" id="DivJOAttachment" runat="server" visible="false">
            <div class="col-sm-1"></div>
            <div class="col-sm-10">
                <br />
                <div class="row" style="margin-bottom: 5px">
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="inputGroupFileAddon01">Insurance Policy w/ O.R.</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="imp001"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>

                            </div>
                            <input type="button" class="btn btn-primary" id="" value="View">
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp002">Policy Report / Affidavit of Driver</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="_imp002"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="_imp002">Choose file</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp003">Registration with O.R.</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="_imp003"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="_imp003">Choose file</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row" style="margin-bottom: 5px">
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Driver's License with O.R</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Pictures</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Chassis No and Motor No. Stencil</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row" style="margin-bottom: 5px">
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Letter of Authority</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Motor Car Claims Unit Evaluation Sheet</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Invoice Number</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row" style="margin-bottom: 5px">
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Release of Claim</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="imp004">Affidavit of Subrogation Docs</span>
                            </div>
                            <div class="custom-file">
                                <input type="file" class="custom-file-input" id="inputGroupFile01"
                                    aria-describedby="inputGroupFileAddon01">
                                <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-1"></div>
        </div>

        <div class="row" id="DivEstimate" runat="server" visible="false">
            <div class="col-sm-12">
                <div class="col-sm-12">
                    <%--<div class="row">
                        <div class="col-sm-12">
                            <div class="btn-group">
                                <asp:Button ID="Button2" CssClass="btn btn-primary btn-sm" runat="server" Text="Create New" />
                                <asp:Button ID="Button3" CssClass="btn btn-primary btn-sm" runat="server" Text="Edit" />
                                <asp:Button ID="Button4" CssClass="btn btn-primary btn-sm" runat="server" Text="Delete" />
                                <asp:Button ID="Button5" CssClass="btn btn-primary btn-sm" runat="server" Text="Reload" />
                            </div>
                        </div>
                    </div> --%>
                    <div class="row">
                        <div class="col-sm-12"  style="width: 100%; font-size: 12px">
                            <table class="table table-bordered">
                                <tr>
                                    <th style="width:200px"></th>
                                    <th>ITEM DESCRIPTION</th>
                                    <th>QTY</th>
                                    <th>AMOUNT</th>
                                    <th style="width:60px"></th>
                                </tr>
                                <tr>
                                    <th colspan="4">TINSMITH</th> 
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button6" CssClass="btn btn-primary btn-sm" runat="server" Text="Add Item" />
                                            <asp:Button ID="Button7" CssClass="btn btn-primary btn-sm" runat="server" Text="Assign" /> 
                                        </div></th>
                                </tr>
                                <tr>
                                    <th></th>
                                    <th>FRONT BUMPER FRAME</th>
                                    <th>1</th>
                                    <th>10,000</th>
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button16" CssClass="btn btn-info btn-sm" runat="server" Text="Edit" />
                                            <asp:Button ID="Button17" CssClass="btn btn-info btn-sm" runat="server" Text="Remove" /> 
                                        </div>
                                    </th>
                                </tr>
                                <tr>
                                    <th></th>
                                    <th>RADIATOR STAY BASE</th>
                                    <th>1</th>
                                    <th>16,300</th>
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button18" CssClass="btn btn-info btn-sm" runat="server" Text="Edit" />
                                            <asp:Button ID="Button19" CssClass="btn btn-info btn-sm" runat="server" Text="Remove" /> 
                                        </div>
                                    </th>
                                </tr>
                                <tr>
                                    <th></th>
                                    <th>LH/RH HEADLIGHT BASEE</th>
                                    <th>2</th>
                                    <th>14,040</th>
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button20" CssClass="btn btn-info btn-sm" runat="server" Text="Edit" />
                                            <asp:Button ID="Button21" CssClass="btn btn-info btn-sm" runat="server" Text="Remove" /> 
                                        </div>
                                    </th>
                                </tr>
                                <tr>
                                    <th colspan="4">PAINTING</th> 
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button2" CssClass="btn btn-primary btn-sm" runat="server" Text="Add Item" />
                                            <asp:Button ID="Button3" CssClass="btn btn-primary btn-sm" runat="server" Text="Assign" /> 
                                        </div></th>
                                </tr>
                                
                                <tr>
                                    <th colspan="4">MECHANICAL</th> 
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button4" CssClass="btn btn-primary btn-sm" runat="server" Text="Add Item" />
                                            <asp:Button ID="Button5" CssClass="btn btn-primary btn-sm" runat="server" Text="Assign" /> 
                                        </div></th>
                                </tr>
                                <tr>
                                    <th></th>
                                    <th>RADIATOR TANK</th>
                                    <th>1</th>
                                    <th>10,200</th>
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button22" CssClass="btn btn-info btn-sm" runat="server" Text="Edit" />
                                            <asp:Button ID="Button23" CssClass="btn btn-info btn-sm" runat="server" Text="Remove" /> 
                                        </div>
                                    </th>
                                </tr>
                                <tr>
                                    <th></th>
                                    <th>RADIATOR SHROUD</th>
                                    <th>1</th>
                                    <th>6,340</th>
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button24" CssClass="btn btn-info btn-sm" runat="server" Text="Edit" />
                                            <asp:Button ID="Button25" CssClass="btn btn-info btn-sm" runat="server" Text="Remove" /> 
                                        </div>
                                    </th>
                                </tr>
                                <tr>
                                    <th></th>
                                    <th>INTERCOOLER</th>
                                    <th>2</th>
                                    <th>4,670</th>
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button26" CssClass="btn btn-info btn-sm" runat="server" Text="Edit" />
                                            <asp:Button ID="Button27" CssClass="btn btn-info btn-sm" runat="server" Text="Remove" /> 
                                        </div>
                                    </th>
                                </tr>
                                <tr>
                                    <th colspan="4">ELECTRICAL</th> 
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button8" CssClass="btn btn-primary btn-sm" runat="server" Text="Add Item" />
                                            <asp:Button ID="Button9" CssClass="btn btn-primary btn-sm" runat="server" Text="Assign" /> 
                                        </div></th>
                                </tr>
                                <tr>
                                    <th colspan="4">A/C WORKS</th> 
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button10" CssClass="btn btn-primary btn-sm" runat="server" Text="Add Item" />
                                            <asp:Button ID="Button11" CssClass="btn btn-primary btn-sm" runat="server" Text="Assign" /> 
                                        </div></th>
                                </tr>
                                <tr>
                                    <th colspan="4">CLASS WOKS</th> 
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button12" CssClass="btn btn-primary btn-sm" runat="server" Text="Add Item" />
                                            <asp:Button ID="Button13" CssClass="btn btn-primary btn-sm" runat="server" Text="Assign" /> 
                                        </div></th>
                                </tr>
                                <tr>
                                    <th colspan="4">OUT SIDE WORKS</th> 
                                    <th>
                                        <div class="btn-group">
                                            <asp:Button ID="Button14" CssClass="btn btn-primary btn-sm" runat="server" Text="Add Item" />
                                            <asp:Button ID="Button15" CssClass="btn btn-primary btn-sm" runat="server" Text="Assign" /> 
                                        </div></th>
                                </tr>
                            </table>
                        </div>
                        </div>
                </div>
            </div>
        </div>

        <div class="row" id="DivCategory" runat="server" visible="false"> 
            <div class="col-sm-12">
                <div class="col-sm-12">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="btn-group">
                                <asp:Button ID="btnAdd" CssClass="btn btn-primary btn-sm" runat="server" Text="Create New" />
                                <asp:Button ID="Button1" CssClass="btn btn-primary btn-sm" runat="server" Text="Edit" />
                                <asp:Button ID="btnDelete" CssClass="btn btn-primary btn-sm" runat="server" Text="Delete" />
                                <asp:Button ID="BtnRefresh" CssClass="btn btn-primary btn-sm" runat="server" Text="Reload" />
                            </div>
                        </div>
                    </div> 
                </div>
            </div>
        </div>

        <!-- The Modal -->

        <div id="myModal" class="modal fade bd-example-modal-xl" tabindex="-1" role="dialog" aria-labelledby="myExtraLargeModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-sm">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h4 class="modal-title">Job Order Form</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12">
                                <small>Job Order Number:</small>
                                <asp:TextBox ID="TxtJONO" runat="server" CssClass="form-control form-control-sm border border-danger"
                                    placeholder="Enter Job Order Number"></asp:TextBox>
                            </div>
                            <div class="col-md-12">
                                <small>Client Name / Insurance Name:</small>
                                <asp:DropDownList ID="DDLClientList" runat="server" CssClass="form-control form-control-sm border border-success">
                                </asp:DropDownList>
                            </div>


                            <div class="col-md-12">
                                <small>Insurer Name:</small>
                                <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control form-control-sm border border-danger"
                                    placeholder="Enter Insurer Name"></asp:TextBox>
                            </div>

                            <div class="col-md-12">
                                <small>Insured Name:</small>
                                <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control form-control-sm border border-danger"
                                    placeholder="Enter Insured Name"></asp:TextBox>
                            </div>

                            <div class="col-md-12">
                                <small>Policy Number:</small>
                                <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control form-control-sm border border-danger"
                                    placeholder="Enter Policy Number"></asp:TextBox>
                            </div>

                            <div class="col-md-12">
                                <small>Vehicle Description:</small>
                                <asp:TextBox ID="TextBox5" runat="server" Width="" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>

                            <div class="col-md-12">
                                <small>Plate Number:</small>
                                <asp:TextBox ID="TextBox6" runat="server" CssClass="form-control form-control-sm border border-danger"
                                    placeholder="Enter Plate Number"></asp:TextBox>
                            </div>
                            <div class="col-md-12">
                                <small>Date of Loss:</small>
                                <asp:TextBox ID="TextBox4" runat="server" Width="" CssClass="form-control form-control-sm border border-danger" placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>
                            <div class="col-md-12">
                                <small>Completion Target Date</small>
                                <asp:TextBox ID="TxtTargetDelDate" runat="server" Width="" CssClass="form-control form-control-sm border border-danger" placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>
                            <div class="col-md-12">
                                <small>Job Order Status:</small>
                                <asp:DropDownList ID="DDLPOStatusList" runat="server" CssClass="form-control form-control-sm"></asp:DropDownList>
                            </div>

                            <div class="col-md-12">
                                <small>Remarks:</small>
                                <asp:TextBox ID="TxtRemarks" runat="server" Width="" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="col-md-12">

                                <asp:Button ID="BtnSave" CssClass="btn btn-success btn-sm" runat="server" Text="Submit" />
                                <button type="button" class="btn btn-primary btn-sm" data-dismiss="modal">Close</button>
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                </div>
            </div>
        </div>

        <div id="ItemModal" class="modal fade bd-example-modal-xl" tabindex="-1" role="dialog" aria-labelledby="myExtraLargeModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h4 class="modal-title">Select Supplier Item</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12">
                                <table class="table table-bordered table-sm">
                                    <tr>
                                        <td>Item Description</td>
                                        <td style="width: 150px">Item QTY</td>
                                        <td>UOM</td>
                                    </tr>

                                </table>
                            </div>

                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="col-md-12">

                                <asp:Button ID="BtnSaveItem" CssClass="btn btn-success btn-sm" runat="server" Text="Submit" />
                                <button type="button" class="btn btn-primary btn-sm" data-dismiss="modal">Close</button>
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                </div>
            </div>
        </div>

        <div id="ModalReceiving" class="modal fade bd-example-modal-xl" tabindex="-1" role="dialog" aria-labelledby="myExtraLargeModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h4 class="modal-title">Purchase Receiving Form</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12">
                                <small>Select Item:</small>
                                <asp:DropDownList ID="DDLReceivingItemList" runat="server" CssClass="form-control form-control-sm">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <small>Proforma or Invoice Number:</small>
                                <asp:TextBox ID="TxtInvoiceNum" runat="server" CssClass="form-control form-control-sm border border-danger" placeholder="Enter Proforma or Invoice Number"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <small>Lotnumber:</small>
                                <asp:TextBox ID="TxtLotNum" runat="server" CssClass="form-control form-control-sm border border-success" placeholder="Enter Lotnumber"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <small>Expiration Date:</small>
                                <asp:TextBox ID="TxtExpDate" runat="server" Width="" CssClass="form-control form-control-sm border border-success" placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <small>Serial Number:</small>
                                <asp:TextBox ID="TxtSerialNum" runat="server" CssClass="form-control form-control-sm border border-success" placeholder="Enter Serial Number"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <small>Quantity:</small>
                                <asp:TextBox ID="TxtQty" runat="server" CssClass="form-control form-control-sm border border-danger" placeholder="Enter Quantity"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <small>Unit Cost:</small>
                                <asp:TextBox ID="TxtUnitCost" runat="server" CssClass="form-control form-control-sm border border-success" placeholder="Enter Unit Cost"></asp:TextBox>
                            </div>
                            <div class="col-md-12">
                                <small>Remarks:</small>
                                <asp:TextBox ID="TxtRecRemarks" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="col-md-12">
                                <%--<asp:Button ID="BtnSaveReceiving" CssClass="btn btn-success btn-sm" Text="Submit" /> --%>
                                <input type="button" id="BtnSaveReceiving" name="BtnSaveReceiving" class="btn btn-primary btn-sm" value="Save" onclick="ExecuteForm()" />
                                <button type="button" class="btn btn-primary btn-sm" data-dismiss="modal">Close</button>
                                <div id="txtHint" style="margin-top: 8px"></div>
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="ModalConfirm" class="modal fade bd-example-modal-xl" tabindex="-1" role="dialog" aria-labelledby="myExtraLargeModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h5 class="modal-title">Confirmation</h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <div class="col-md-12">
                            <div class="row">
                                <h6 class="modal-title">Are you sure you to POST all pending item?</h6>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="col-md-12">
                                <asp:Button ID="BntPostYES" CssClass="btn btn-success btn-sm" runat="server" Text="YES" />
                                <button type="button" class="btn btn-primary btn-sm" data-dismiss="modal">Cancel</button>
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                </div>
            </div>
        </div>


        <input type="hidden" id="h_TranId" runat="server" />
        <input type="hidden" id="h_RecTranId" runat="server" />
        <input type="hidden" id="h_PONO" runat="server" />
        <input type="hidden" id="h_Supplier" runat="server" />
    </div>

</asp:Content>

