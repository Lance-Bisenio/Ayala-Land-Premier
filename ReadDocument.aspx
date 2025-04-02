<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="ReadDocument.aspx.vb" Inherits="ReadDocument" %>

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
            border: solid 0px
        }
    </style>
    <script> 

        $(document).ready(function () {

            document.oncontextmenu = function () {
                return false;
            };

            $(document).on("contextmenu", function (e) {
                e.preventDefault();
            });

            $(document).keydown(function (event) {
                if (event.keyCode == 123) { // Prevent F12
                    return false;
                } else if (event.ctrlKey && event.shiftKey && event.keyCode == 73) { // Prevent Ctrl+Shift+I        
                    return false;
                }
            });

            $(function () {
                $(document).bind('contextmenu', function (e) {
                    e.preventDefault();
                    alert('Right Click is not allowed');
                });
            });

            $('#MainContent_BtnAdd').click(function () {
                $("#DivEdit").hide();
                $("#DivAdd").show();

                $('#MainContent_TxtDescr').val("");

                $('#MainContent_BtnSubmit').show();
                $('#MainContent_BtnUpdatePolicy').hide();
            });

            $('#MainContent_BtnUpdate').click(function () {
                $("#DivEdit").show();
                $("#DivAdd").hide();

                $('#MainContent_BtnUpdatePolicy').show();
                $('#MainContent_BtnSubmit').hide();
            });


            //$(".custom-file-input").on("change", function () {
            //    var fileName = $(this).val().split("\\").pop();
            //    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
            //});
        });

        function previewfile(pFilename) {
            document.getElementById("frmPreview").src = pFilename;
        }

        window.frames["frmPreview"].document.oncontextmenu = function () { return false; };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <br />
        <h3>Company Policy</h3>
        <div class="row">
            <div class="col-sm-4">
            </div>
            <div class="col-sm-6"></div>
        </div>
        <div class="row">
            <div class="col-sm-3">
                <br />
                <br />
                <br />
                <div class="btn-group" style="visibility: hidden">
                    <button type="button" name="BtnAdd" id="BtnAdd" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#myModalEdit" disabled="disabled" runat="server">Add New</button>
                    <button type="button" id="BtnUpdate" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#myModalEdit" disabled="disabled" runat="server">Edit</button>
                    <button type="button" id="BtnDelete" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#myModalDel" disabled="disabled" runat="server">Delete</button>
                </div>

            </div>
            <div class="col-sm-2">
                <small>Document Type:</small>
                <asp:DropDownList ID="CmdDocType" runat="server" Width="" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                <small>Document Owner:</small>
                <asp:DropDownList ID="CmdDocOwner" runat="server" Width="" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
            </div>
            <div class="col-sm-7">
                <small>Other Filter:</small>
                <div class="form-check">
                    <label class="form-check-label">
                        <input type="radio" id="RdoOpt1" runat="server" style="margin-top: 7px" class="form-check-input" name="optradio">
                        <small>Show all documents based on the selected filter</small>
                    </label>
                </div>
                <div class="form-check" style="margin-bottom: 7px">
                    <label class="form-check-label">
                        <%--<asp:RadioButton ID="RdoOpt2" runat="server" CssClass="form-check-input" />--%>
                        <input type="radio" id="RdoOpt2" runat="server" style="margin-top: 7px" class="form-check-input" checked name="optradio">
                        <small>Show all unread documents based on the selected filter</small>
                    </label>
                </div>
                <div class="form-inline">
                    <input type="text" id="TxtKeywords" runat="server" class="form-control form-control-sm col-3" placeholder="Enter document description">&nbsp;
                    <div class="btn-group">
                        <asp:Button ID="BtnSearch" runat="server" CssClass="btn btn-sm btn-primary" Text="Search"></asp:Button>
                        <button type="button" id="BtnIAgree" runat="server" class="btn btn-success btn-sm align-bottom" visible="false">I agree and understand the selected document</button>
                    </div>
                </div>

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
                                <HeaderStyle CssClass="hideGridColumn"></HeaderStyle>
                                <ItemStyle CssClass="hideGridColumn" Width="80px"></ItemStyle>
                            </asp:BoundField>

                            <asp:BoundField DataField="Descr" HeaderText="Description">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>

                            <asp:BoundField DataField="TranId" HeaderText="Tran ID" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                                <HeaderStyle CssClass="hideGridColumn"></HeaderStyle>
                                <ItemStyle CssClass="hideGridColumn" Width="80px"></ItemStyle>
                            </asp:BoundField>

                            <asp:BoundField DataField="Group_Id" HeaderText="Group_Id" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                                <HeaderStyle CssClass="hideGridColumn"></HeaderStyle>
                            </asp:BoundField>

                            <asp:BoundField DataField="DocType_Id" HeaderText="DocType_Id" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                                <HeaderStyle CssClass="hideGridColumn"></HeaderStyle>
                                <ItemStyle CssClass="hideGridColumn" Width="80px"></ItemStyle>
                            </asp:BoundField>

                            <asp:BoundField DataField="Owner" HeaderText="Owner">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>

                            <asp:BoundField DataField="PolicyFileLocation" HeaderText="DocType_Id" HeaderStyle-CssClass="hideGridColumn" ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="80px">
                                <HeaderStyle CssClass="hideGridColumn"></HeaderStyle>
                                <ItemStyle CssClass="hideGridColumn" Width="80px"></ItemStyle>
                            </asp:BoundField>

                            <asp:CommandField ButtonType="Button" ShowSelectButton="True" SelectText="Read">
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
                <div style="position: absolute; background-color: transparent; width: 96.5%; height: 99%"></div>
                <iframe id="frmPreview" width="100%" height="100%" onload="disableContextMenu();" style="border: double 1px #808080; height: 700px; z-index: 200"></iframe>
                <%--<div id="divPrev" style=" border: solid 0px #1c1c1c; position:absolute; top:40px; right:10px; left:415px; bottom:10px; ">
        	        
                </div>--%>
            </div>
        </div>
    </div>


    <div id="myModalEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">Document Management</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div id="DivEdit">
                                Document Type:
                                <asp:DropDownList ID="CmdEDocType" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                Document Owner:
                                <asp:DropDownList ID="CmdEDocOwner" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                Description:
                                <input type="text" runat="server" id="TxtEDescr" name="txtCode" class="form-control col-12 border border-danger"
                                    value="" placeholder="Enter document description" />
                                <br />
                                Browse File/Upload New File: 
                                <asp:FileUpload ID="TxtEFileName" runat="server" CssClass="form-control-file border" />
                            </div>

                            <div id="DivAdd">
                                Document Type:
                                <asp:DropDownList ID="CmdRefDocType" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                Document Owner:
                                <asp:DropDownList ID="CmdRefDocOwner" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                Description:
                                <input type="text" runat="server" id="TxtDescr" name="txtCode" class="form-control col-12 border border-danger"
                                    value="" placeholder="Enter document description" />
                                <br />
                                Browse File: 
                                <asp:FileUpload ID="TxtFileName" runat="server" CssClass="form-control-file border" />
                                <%--<div class="custom-file">
                                    <asp:FileUpload ID="TxtFileName" runat="server" CssClass="custom-file-input" />
                                    <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control-file border" />
                                    <label class="custom-file-label" for="customFile">Choose file</label>
                                </div>--%>
                            </div>

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
    <div id="myModalDel" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary"></h5>
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
                    <h5 class="modal-title text-primary"></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div>
                                Select Department:
                                <asp:DropDownList ID="CmdDLDept" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                                Select Document:
                                <asp:DropDownList ID="CmdDLDocList" runat="server" CssClass="form-control form-control-sm col-12"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="Button1" value="YES" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

