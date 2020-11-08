<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RDT.aspx.cs" Inherits="RDT" Culture="ru-RU"%>
<% @Import Namespace="System.Globalization" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <title></title>
</head>

<body>
    <div class="container">
        <form id="form1" runat="server" novalidate="novalidate">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="row jumbotron">
                <div class="col-10">
                    <div class="col border">
                        <asp:Label ID="Label1" runat="server" Text="Место установки:"></asp:Label>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:RadioButtonList ID="sprRadioButtonList1" runat="server" style="margin-left: 0px"
                                    AutoPostBack="True">
                                    <asp:ListItem>ЦТП</asp:ListItem>
                                    <asp:ListItem>ИТП</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:RequiredFieldValidator ID="sprRequiredFieldValidator1" runat="server"
                                    ControlToValidate="sprRadioButtonList1" Display="Dynamic"
                                    ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True">
                                    Выберите необходимое значение</asp:RequiredFieldValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                Исполнение регулятора:<asp:RadioButtonList ID="eorRadioButtonList1" runat="server"
                                    AutoPostBack="True"
                                    OnSelectedIndexChanged="eorRadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>Регулятор перепада давления</asp:ListItem>
                                    <asp:ListItem>Регулятор давления &quot;после себя&quot;</asp:ListItem>
                                    <asp:ListItem>Регулятор давления &quot;до себя&quot;</asp:ListItem>
                                    <asp:ListItem>Регулятор &quot;перепуска&quot;</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:RequiredFieldValidator ID="eorRequiredFieldValidator1" runat="server"
                                    ControlToValidate="eorRadioButtonList1" Display="Dynamic"
                                    ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True">
                                    Выберите необходимое значение</asp:RequiredFieldValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label5" runat="server" Text="Рабочая среда:"></asp:Label>
                                <div class="row">
                                    <div class="col-3">
                                        <asp:RadioButtonList ID="ws1RadioButtonList1" runat="server" AutoPostBack="True"
                                            OnSelectedIndexChanged="ws1RadioButtonList1_SelectedIndexChanged">
                                            <asp:ListItem>Вода</asp:ListItem>
                                            <asp:ListItem>Этиленгликоль</asp:ListItem>
                                            <asp:ListItem>Пропиленгликоль</asp:ListItem>
                                            <asp:ListItem Enabled="False">Водяной пар</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <div class="col">
                                            <asp:RadioButtonList ID="lp5RadioButtonList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lp5RadioButtonList1_SelectedIndexChanged" Enabled="False" style="height: 46px">
                                                <asp:ListItem>Перегретый</asp:ListItem>
                                                <asp:ListItem>Насыщеный</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <asp:RequiredFieldValidator ID="ws1RequiredFieldValidator1" runat="server"
                                            ControlToValidate="ws1RadioButtonList1" Display="Dynamic"
                                            ErrorMessage="Выберите необходимое значение" ForeColor="Red"
                                            SetFocusOnError="True">
                                            Выберите необходимое значение</asp:RequiredFieldValidator>
                                           <asp:CustomValidator ID="CustomValidator21" runat="server"
                                                ControlToValidate="lp5RadioButtonList1" Display="Dynamic"
                                                EnableClientScript="False" ErrorMessage="CustomValidator" ForeColor="Red"
                                                OnServerValidate="CustomValidator21_ServerValidate" SetFocusOnError="True"
                                                ValidateEmptyText="True"></asp:CustomValidator>
                                    </div>
                                    <div class="col-5">
                                        <br />
                                        <asp:TextBox ID="ws1TextBox1" runat="server" 
                                            Enabled="False" type="number" TextMode="Number" CausesValidation="True">
                                        </asp:TextBox>
                                        <asp:Label ID="Label6" runat="server" Text="% (от 5% до 65%)"></asp:Label>
                                        &nbsp;&nbsp;<asp:CustomValidator ID="CustomValidator16" runat="server"
                                            ControlToValidate="ws1TextBox1" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator16_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                        <asp:TextBox ID="ws1TextBox2" runat="server" 
                                            Enabled="False" type="number" TextMode="Number" CausesValidation="True">
                                        </asp:TextBox>
                                        <asp:Label ID="Label7" runat="server"
                                            Text="&#8451; (от 0&#8451; до 150&#8451;)">
                                        </asp:Label>&nbsp;&nbsp;<asp:CustomValidator ID="CustomValidator17"
                                            runat="server" ControlToValidate="ws1TextBox2" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator17_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                    </div>
                                    
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:Label ID="Label20" runat="server" Text="Потери давления на клапане регулятора:">
                        </asp:Label>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label26" runat="server" Text="Регулятор перепада давления:">
                                    </asp:Label>
                                    <br />
                                    <div class="col panel-hide" id="lp1" runat="server">
                                        <asp:Label ID="lpLabel1" runat="server" Text="Потери давления на регуляторе:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label54" runat="server" Text="ΔPрд ="></asp:Label>
                                        &nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp1TextBox1" runat="server" Enabled="False"
                                            TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList1" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp1DropDownList1_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator10" runat="server"
                                            ControlToValidate="lp1DropDownList1" ErrorMessage="CustomValidator"
                                            Display="Dynamic" ForeColor="Red"
                                            OnServerValidate="CustomValidator10_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <asp:Label ID="LabelCustomValid" runat="server" ForeColor="Red"
                                            Text="Суммарные потери давления на регуляторе и регулируемом участке превышают допустимый перепад давлений на вводе"
                                            Visible="False"></asp:Label>
                                        <br />
                                        <asp:Label ID="lpLabel2" runat="server"
                                            Text="Перепад давлений, поддерживаемый регулятором на регулируемом участке:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label55" runat="server" Text="ΔPру ="></asp:Label>
                                        &nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp1TextBox2" runat="server" Enabled="False"
                                            TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList2" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp1DropDownList2_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator8" runat="server"
                                            ControlToValidate="lp1DropDownList2" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator8_ServerValidate"
                                            SetFocusOnError="True" ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                          <asp:Label ID="lpLabel3" runat="server"
                                            Text="Давление в подающем трубопроводе "></asp:Label>
                                        <br />
                                        <asp:Label ID="Label60" runat="server" Text="(перед регулятором, если он устанавливается на подающем трубопроводе, или в месте врезки импульсной трубки, если регулятор устанавливается на обратном трубопроводе):"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label56" runat="server" Text="P1 = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp1TextBox3" runat="server" Enabled="False"
                                            TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList3" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp1DropDownList3_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator1" runat="server"
                                            ControlToValidate="lp1DropDownList3" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator1_ServerValidate"
                                            SetFocusOnError="True" ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                          <asp:Label ID="lpLabel4" runat="server"
                                            Text="Давление в обратном трубопроводе"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label61" runat="server" Text="(в месте врезки импульсной трубки, если регулятор устанавливается на подающем трубопроводе, или после регулятора, если он устанавливается на обратном трубопроводе):"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label57" runat="server" Text="P2 = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp1TextBox4" runat="server"
                                            Enabled="False" TextMode="Number" AutoPostBack="True"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList4" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp1DropDownList4_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator2" runat="server"
                                            ControlToValidate="lp1DropDownList4" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator2_ServerValidate"
                                            SetFocusOnError="True" ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="lpLabel5" runat="server"
                                            Text="Максимальный перепад на регуляторе:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label58" runat="server" Text="ΔPрд(max) = Р1 - Р2 - ΔРру =">
                                        </asp:Label>&nbsp;&nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp1TextBox5" runat="server" Enabled="False"
                                            ReadOnly="True"></asp:TextBox>
                                        &nbsp;
                                        <asp:Label ID="Label59" runat="server" Text="бар"></asp:Label>
                                        <br />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label2" runat="server" Text="Регулятор давления <q>после себя</q>:">
                                    </asp:Label>
                                    <br />

                                    <div class="col panel-hide" id="lp2" runat="server">
                                        <asp:Label ID="Label9" runat="server" Text="Давление перед регулятором:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label3" runat="server" Text="P'1 = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp2TextBox1" runat="server"
                                            Enabled="False"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp2DropDownList1" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp2DropDownList1_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator3" runat="server"
                                            ControlToValidate="lp2DropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator3_ServerValidate"
                                            SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="Label4" runat="server"
                                            Text="Требуемое давление после регулятора:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label8" runat="server" Text="Р(треб) ="></asp:Label>
                                        <asp:TextBox ID="lp2TextBox2" runat="server"
                                            Enabled="False" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp2DropDownList2" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp2DropDownList2_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator4" runat="server"
                                            ControlToValidate="lp2DropDownList2" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator4_ServerValidate"
                                            SetFocusOnError="True"></asp:CustomValidator>
                                        <br />

                                    </div>
                                    <div class="col panel-hide" id="lp5" runat="server">
                                        <asp:Label ID="Label32" runat="server" Text="Давление пара перед регулятором:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label33" runat="server" Text="P'1 = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp5TextBox1" runat="server"
                                            Enabled="False"></asp:TextBox>&nbsp;
                                        <asp:DropDownList ID="lp5DropDownList1" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp5DropDownList1_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator18" runat="server"
                                            ControlToValidate="lp5DropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" SetFocusOnError="True"
                                            OnServerValidate="CustomValidator18_ServerValidate"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="Label34" runat="server"
                                            Text="Требуемое давление пара после регулятора:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label35" runat="server" Text="Р(треб) ="></asp:Label>
                                        &nbsp;&nbsp;<asp:TextBox ID="lp5TextBox2" runat="server"
                                            Enabled="False" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp5DropDownList2" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp5DropDownList2_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator19" runat="server"
                                            ControlToValidate="lp5DropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator19_ServerValidate"
                                            SetFocusOnError="True"></asp:CustomValidator>
                                        <br />

                                        


                                        <asp:Label ID="Label39" runat="server" Text="Температура пара через регулятор:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label37" runat="server" Text="T1 = "></asp:Label>

                                        &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;<asp:TextBox ID="lp5TextBox3"
                                            runat="server"  Enabled="False" type="number"
                                            TextMode="Number" CausesValidation="True">
                                        </asp:TextBox>
                                        <asp:Label ID="Label38" runat="server" Text=" &#8451;"></asp:Label>
                                        <asp:CustomValidator ID="CustomValidator20" runat="server"
                                            ControlToValidate="lp5TextBox3" Display="Dynamic" EnableClientScript="False"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator20_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>

                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label10" runat="server" Text="Регулятор давления <q>до себя</q>:">
                                    </asp:Label>
                                    <br />
                                    <div class="col panel-hide" id="lp3" runat="server">
                                        <asp:Label ID="Label11" runat="server"
                                            Text="Требуемое давление перед регулятором:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label12" runat="server" Text="Р(треб) ="></asp:Label>
                                        <asp:TextBox ID="lp3TextBox1" runat="server"
                                            Enabled="False" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp3DropDownList1" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp3DropDownList1_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator5" runat="server"
                                            ControlToValidate="lp3DropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator5_ServerValidate"
                                            SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="Label13" runat="server" Text="Давление после регулятора:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label14" runat="server" Text="Р'2 = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp3TextBox2" runat="server" 
                                            Enabled="False" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp3DropDownList2" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp3DropDownList2_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>

                                        &nbsp;<asp:CustomValidator ID="CustomValidator6" runat="server"
                                            ControlToValidate="lp3DropDownList2" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator6_ServerValidate" SetFocusOnError="True">
                                        </asp:CustomValidator>

                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label15" runat="server" Text="Регулятор <q>перепуска</q>:">
                                    </asp:Label>
                                    <br />
                                    <div class="col panel-hide" id="lp4" runat="server">
                                        <asp:Label ID="Label16" runat="server"
                                            Text="Перепад давлений, поддерживаемый регулятором на регулируемом участке:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label17" runat="server" Text="ΔPру ="></asp:Label>
                                        &nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lp4TextBox2" runat="server" Enabled="False"
                                             TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp4DropDownList2" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lp4DropDownList2_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;&nbsp;<asp:CustomValidator ID="CustomValidator7" runat="server"
                                            ControlToValidate="lp4DropDownList2" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator7_ServerValidate"
                                            SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                            <ContentTemplate>
                                <div class="rte panel-hide" id="calcr" runat="server">
                                    <asp:Label ID="Label18" runat="server"
                                        Text="Расчет регулятора давления на кавитацию:">
                                    </asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label19" runat="server" Text="Давление перед регулятором:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label21" runat="server" Text="P' = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:TextBox ID="calcrTextBox1" runat="server" 
                                            Enabled="False" type="number" TextMode="Number" CausesValidation="True">
                                        </asp:TextBox>
                                        <asp:DropDownList ID="calcrDropDownList1" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="calcrDropDownList1_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>&nbsp;<asp:CustomValidator ID="CustomValidator9"
                                            runat="server" ControlToValidate="calcrDropDownList1"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator9_ServerValidate" SetFocusOnError="True">
                                        </asp:CustomValidator>

                                        <br />
                                        <asp:Label ID="Label22" runat="server"
                                            Text="Температура теплоносителя через регулятор (максимальная):">
                                        </asp:Label><br />
                                        <asp:Label ID="Label23" runat="server" Text="T1 = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:TextBox ID="calcrTextBox2" runat="server" 
                                            Enabled="False" type="number" TextMode="Number" CausesValidation="True">
                                        </asp:TextBox>
                                        <asp:Label ID="Label24" runat="server" Text=" &#8451;"></asp:Label>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator11" runat="server"
                                            ControlToValidate="calcrTextBox2" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator11_ServerValidate"
                                            SetFocusOnError="True" Display="Dynamic" EnableClientScript="False"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div class="col border">
                        <asp:Label ID="Label31" runat="server" Text="Расход через регулятор давления:"></asp:Label>

                       <asp:UpdatePanel ID="UpdatePanel13" runat="server">
                            <ContentTemplate>
                                <div class="panel-hide" id="fpr1" runat="server">
                                    <asp:RadioButton ID="fprRadioButton1" runat="server"
                                        Text="Задать максимальную величину расхода через регулятор давления:" AutoPostBack="True"
                                        OnCheckedChanged="fprRadioButton1_CheckedChanged" Enabled="False" /><br />
                                    <div class="col panel-hide" id="fpr1_1" runat="server">
                                        <asp:Label ID="Label28" runat="server" Text="Gрд = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:TextBox ID="fprTextBox1" runat="server"
                                            Enabled="False" type="number" TextMode="Number"></asp:TextBox>
                                        <asp:DropDownList ID="fprDropDownList1" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="fprDropDownList1_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>м³/ч</asp:ListItem>
                                            <asp:ListItem>л/с</asp:ListItem>
                                            <asp:ListItem>л/мин</asp:ListItem>
                                            <asp:ListItem>л/ч</asp:ListItem>
                                            <asp:ListItem>кг/с</asp:ListItem>
                                            <asp:ListItem>кг/ч</asp:ListItem>
                                            <asp:ListItem>т/ч</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator12" runat="server"
                                            ControlToValidate="fprDropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator12_ServerValidate"
                                            SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>


                        <asp:UpdatePanel ID="UpdatePanel14" runat="server">
                            <ContentTemplate>
                                <div class="panel-hide" id="fpr2" runat="server">
                                    <asp:RadioButton ID="fprRadioButton2" runat="server"
                                        Text="Вычислить максимальную величину расхода через регулятор давления:"
                                        AutoPostBack="True" OnCheckedChanged="fprRadioButton2_CheckedChanged"
                                        Enabled="False" />
                                    <div class="col panel-hide" id="fpr2_1" runat="server">
                                        <asp:Label ID="Label25" runat="server"
                                            Text="Температура теплоносителя в подающем трубопроводе Т1 = ">
                                        </asp:Label>
                                        <asp:TextBox ID="fprTextBox2" runat="server" Enabled="False" TextMode="Number"
                                            CausesValidation="True"></asp:TextBox>
                                        <asp:Label ID="Label27" runat="server" Text="&#8451;"></asp:Label>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator13" runat="server"
                                            ControlToValidate="fprTextBox2" Display="Dynamic" EnableClientScript="False"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator13_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="Label29" runat="server"
                                            Text="Температура теплоносителя в обратном трубопроводе T2 = ">
                                        </asp:Label>&nbsp;
                                        <asp:TextBox ID="fprTextBox3" runat="server" Enabled="False"
                                            TextMode="Number"></asp:TextBox>
                                        <asp:Label ID="Label30" runat="server" Text="&#8451;"></asp:Label>

                                        &nbsp;<asp:CustomValidator ID="CustomValidator14" runat="server"
                                            ControlToValidate="fprTextBox3" Display="Dynamic" EnableClientScript="False"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator14_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>

                                        <br />

                                        <asp:Label ID="Label45" runat="server" Text="Тепловая мощность Q = ">
                                        </asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:TextBox ID="fprTextBox4" runat="server" Enabled="False"
                                             TextMode="Number"></asp:TextBox>
                                        <asp:DropDownList ID="fprDropDownList2" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="fprDropDownList2_SelectedIndexChanged">
                                            <asp:ListItem class="dropdown-item">выбрать</asp:ListItem>
                                            <asp:ListItem Value="кВт" class="dropdown-item">кВт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">МВт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">Вт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">Гкал/ч</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">ккал/ч</asp:ListItem>
                                        </asp:DropDownList>&nbsp;<asp:CustomValidator ID="CustomValidator15"
                                            runat="server" ControlToValidate="lp1DropDownList1"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator15_ServerValidate" SetFocusOnError="True">
                                        </asp:CustomValidator>
                                        <br />

                                        <asp:Label ID="Label46" runat="server"
                                            Text="Максимальный расход через регулятор Gрд = ">
                                        </asp:Label>&nbsp;&nbsp;&nbsp;
                                        <asp:TextBox ID="fprTextBox5" runat="server" ReadOnly="True" Enabled="False">
                                        </asp:TextBox>
                                        <asp:Label ID="Label48" runat="server" Text=" кг/ч"></asp:Label>
                                        <br />
                                    </div>
                                    <asp:Label ID="fprLabelError" runat="server" ForeColor="Red"></asp:Label>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>
                <div class="col-2">
                    <asp:UpdatePanel ID="UpdatePanel16" runat="server">
                        <ContentTemplate>
                            <asp:Image ID="rPictureBox" runat="server" class="valve-image" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="col-10">
                    <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                        <ContentTemplate>
                            <div class="col non-padding">
                                <asp:Label ID="LabelError" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Font-Strikeout="False" ForeColor="Red"></asp:Label>
                            </div>

                            <asp:Button ID="rButton" runat="server" type="submit" Text="Рассчитать" Width="100%"
                                OnClick="rButton_Click" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <br />
                <div class="col-12">
                    <div class="col non-padding">
                        <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label52" runat="server" Enabled="False" Text="Результаты расчёта"
                                    Visible="False" Font-Bold="True" Font-Size="Medium"></asp:Label>

                                <div class="col non-padding">
                                    <asp:Label ID="ws1ResultLabel" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="maxt1ResultLabel" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="maxp1ResultLabel" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="calcDNLabel" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="calcCapacityLabel" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="labelOptyV" runat="server" Visible="False"></asp:Label>
                                </div>
                                <div class="table-responsive-lg" onclick="ShowBTN()">
                                    <asp:GridView ID="GridView1" CssClass="table table-result rdt" runat="server"
                                        Font-Size="X-Small" Visible="False"
                                        OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
                                        AutoGenerateSelectButton="True">


                                        <RowStyle Font-Size="Small" />
                                        <SelectedRowStyle BackColor="#ff7d00" Font-Bold="False" ForeColor="White" />
                                    </asp:GridView>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="Label53" runat="server" CssClass="show-btn" Text="Объект:"
                                        Visible="False">
                                    </asp:Label>

                                    <asp:TextBox ID="objTextBox1" runat="server" Enabled="False" Visible="False"
                                        Width="1020px"></asp:TextBox>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <script>
                            var eorRadioBtn0 = document.getElementById('eorRadioButtonList1_0');

                            var eorRadioBtn1 = document.getElementById('eorRadioButtonList1_1');
                            var eorRadioBtn2 = document.getElementById('eorRadioButtonList1_2');
                            var eorRadioBtn3 = document.getElementById('eorRadioButtonList1_3');
                            var fprRadioBtn1 = document.getElementById('fprRadioButton1');
                            var fprRadioBtn2 = document.getElementById('fprRadioButton2');
                            var ws1 = document.querySelector("input[name = ws1RadioButtonList1]");

                            var fpr1 = document.getElementById('fpr1');
                            var fpr2 = document.getElementById('fpr2');
                            var fpr11 = document.getElementById('fpr1_1');
                            var fpr21 = document.getElementById('fpr2_1');



                            function ShowBTN() {
                                var btn2 = document.getElementById('Button2');
                                //var btn3 = document.getElementById('Button3');
                                btn2.classList.add("show-btn");
                                //btn3.classList.add("show-btn");
                            };

                            function HideBTN() {
                                var btn2 = document.getElementById('Button2');
                                //var btn3 = document.getElementById('Button3');
                                btn2.classList.remove("show-btn");

                                //btn3.classList.add("show-btn");
                            };
                        </script>

                        <div class="col non-padding padding-top-bottom">
                            <asp:Button ID="Button2" runat="server" Text="Сохранить в PDF"
                                CssClass="btn btn-primary hide-btn" OnClick="Button2_Click" />
                            <%--                            <asp:Button ID="Button3" runat="server" Text="Сохранить в Excel"
                                CssClass="btn btn-primary hide-btn" Display="None" OnClick="Button3_Click" />--%>
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>

</body>

</html>