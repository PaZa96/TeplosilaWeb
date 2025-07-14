<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RDT_ver.aspx.cs" Inherits="RDT_ver" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <title>Программа подбора регулятора давления прямого действия - Поверочный расчет</title>
</head>
<body>
    <div class="container">
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="row jumbotron">
                <div class="col-12">
                    <nav class="nav nav-tabs">
                        <a class="nav-link" href="/RDT.aspx">Стандартный расчет</a>
                        <a class="nav-link active" aria-current="page" href="#">Поверочный расчет</a>
                    </nav>
                </div>
                <div class="col-xs-12 col">
                    <div class="col border border-non-top">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="Label4" runat="server" Text="Исполнение регулятора:"></asp:Label>
                                <br />
                                <asp:RadioButton ID="eorRadioButton1" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton1_CheckedChanged" Text="Регулятор перепада давления" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList1" Enabled="False" AutoPostBack="True" runat="server" OnSelectedIndexChanged="eorRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>RDT</asp:ListItem>
                                        <asp:ListItem>RDT-H</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="eorRadioButton2" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton2_CheckedChanged" Text="Регулятор давления &quot;после себя&quot;" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList2" Enabled="False" AutoPostBack="True" runat="server" OnSelectedIndexChanged="eorRadioButtonList2_SelectedIndexChanged">
                                        <asp:ListItem>RDT-P</asp:ListItem>
                                        <asp:ListItem>RDT-PH</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="eorRadioButton3" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton3_CheckedChanged" Text="Регулятор давления &quot;до себя&quot;" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList3" Enabled="False" runat="server" AutoPostBack="True" OnSelectedIndexChanged="eorRadioButtonList3_SelectedIndexChanged">
                                        <asp:ListItem>RDT-S</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="eorRadioButton4" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton4_CheckedChanged" Text="Регулятор &quot;перепуска&quot;" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList4" Enabled="False" runat="server" AutoPostBack="True" OnSelectedIndexChanged="eorRadioButtonList4_SelectedIndexChanged">
                                        <asp:ListItem>RDT-B</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="Label1" runat="server" Text="Диапазон настройки регулятора:"></asp:Label>
                                <br />
                                <asp:RadioButtonList ID="csrRadioButtonList1" runat="server" AutoPostBack="True">
                                    <asp:ListItem>0.1 (0,08...0,9 бар)</asp:ListItem>
                                    <asp:ListItem>1.1 (0,16...1,8 бар)</asp:ListItem>
                                    <asp:ListItem>1.2 (0,24...3,0 бар)</asp:ListItem>
                                    <asp:ListItem>1.3 (0,4...4,8 бар)</asp:ListItem>
                                    <asp:ListItem>2.1 (0,5...5,8 бар)</asp:ListItem>
                                    <asp:ListItem>2.2 (0,9...10,0 бар)</asp:ListItem>
                                    <asp:ListItem>2.3 (1,4...15,8 бар)</asp:ListItem>
                                </asp:RadioButtonList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label2" runat="server" Text="Номинальное давление PN, бар:"></asp:Label>
                                <div class="col-12 row">
                                    <asp:RadioButtonList ID="pnRadioButtonList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="pnRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>16</asp:ListItem>
                                        <asp:ListItem>25</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label5" runat="server" Text="Диаметр и пропускная способность клапана:"></asp:Label>
                                <div id="aaPanel0" class="col-12" runat="server">
                                    <div class="row">
                                        <div class="col-6 col-md-2">
                                            <asp:Label ID="dnLabel" runat="server" Text="DN, мм:"></asp:Label>
                                        </div>
                                        <div class="col-6 col-md-2">
                                            <asp:DropDownList ID="dnDropDownList1" runat="server" Enabled="False" AutoPostBack="True" OnSelectedIndexChanged="dnDropDownList1_SelectedIndexChanged">
                                                <asp:ListItem>выбрать</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-6 col-md-2">
                                            <asp:Label ID="kvsLabel" runat="server" Text="Kvs, м3/ч:"></asp:Label>
                                        </div>
                                        <div class="col-6 col-md-2">
                                            <asp:DropDownList ID="kvsDropDownList1" runat="server" Enabled="False" AutoPostBack="True" OnSelectedIndexChanged="kvsDropDownList1_SelectedIndexChanged">
                                                <asp:ListItem>выбрать</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <div id="aaPanel1" runat="server">
                                    <asp:Label ID="Label3" runat="server" Text="Рабочая среда:"></asp:Label>
                                    <div class="row">
                                        <div class="col-6 col-md-3">
                                            <asp:RadioButtonList ID="wsRadioButtonList1" runat="server"
                                                AutoPostBack="True" required="required">
                                                <asp:ListItem>Вода</asp:ListItem>
                                                <asp:ListItem>Этиленгликоль</asp:ListItem>
                                                <asp:ListItem>Пропиленгликоль</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <div class="col-12 sub-col">
                                                <asp:RadioButtonList ID="lpvRadioButtonList1" runat="server"
                                                    AutoPostBack="True"
                                                    Enabled="False" Visible="False">
                                                    <asp:ListItem>Перегретый</asp:ListItem>
                                                    <asp:ListItem>Насыщеный</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="col-6 col-md-6">
                                            <div class="row">
                                                <br></br>
                                            </div>
                                            <div class="row">
                                                <asp:TextBox ID="wsTextBox1" runat="server" Enabled="False" type="number"
                                                    required="required" TextMode="Number" placeholder=" от 5% до 65%"></asp:TextBox>&nbsp;
                                                <asp:Label ID="Label6" runat="server" Text="%"></asp:Label>
                                            </div>
                                            <div class="row">
                                                <asp:TextBox ID="wsTextBox2" runat="server" Enabled="False" type="number"
                                                    required="required" TextMode="Number" placeholder=" от 0&#8451; до 150&#8451;"></asp:TextBox>&nbsp;
                                                <asp:Label ID="Label7" runat="server"
                                                    Text="&#8451;">
                                                </asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
        </form>
    </div>
</body>
</html>
