<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RDT.aspx.cs" Inherits="RDT" %>
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
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="row jumbotron">
                <div class="col-10">
                    <div class="col border">
                        <asp:Label ID="Label1" runat="server" Text="Место установки:"></asp:Label>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:RadioButtonList ID="sprRadioButtonList1" runat="server" style="margin-left: 0px"
                                    AutoPostBack="True" required="required">
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
                                Исполнение регулятора<asp:RadioButtonList ID="eorRadioButtonList1" required="required"
                                    runat="server" AutoPostBack="True" OnSelectedIndexChanged="eorRadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>Регулятор перепада давления</asp:ListItem>
                                    <asp:ListItem>Регулятор давления &quot;после себя&quot;</asp:ListItem>
                                    <asp:ListItem>Регулятор давления &quot;до себя&quot;</asp:ListItem>
                                    <asp:ListItem>Регулятор &quot;перепуска&quot;</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:RequiredFieldValidator ID="eorRequiredFieldValidator1" runat="server"
                                    ControlToValidate="eorRadioButtonList1" Display="Dynamic"
                                    ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True"> Выберите необходимое значение</asp:RequiredFieldValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="Label5" runat="server" Text="Рабочая среда:"></asp:Label>
                                <asp:RadioButtonList ID="ws1RadioButtonList1" runat="server" AutoPostBack="True"
                                    required="required" OnSelectedIndexChanged="ws1RadioButtonList1_SelectedIndexChanged">
                                    
                                    <asp:ListItem>Вода</asp:ListItem>
                                    <asp:ListItem>Этиленгликоль</asp:ListItem>
                                    <asp:ListItem>Пропиленгликоль</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:TextBox ID="ws1TextBox1" runat="server" step="0.01" Enabled="False" type="number"
                                    required="required" TextMode="Number"></asp:TextBox>
                                <asp:Label ID="Label6" runat="server" Text="%(от 5% до 65%)"></asp:Label>
                                &nbsp;&nbsp;&nbsp;
                                <asp:RangeValidator ID="wsRangeValidator1" runat="server" Display="Dynamic"
                                    ControlToValidate="ws1TextBox1"
                                    ErrorMessage="Значение должно находится в диапазоне от 5% до 65%" ForeColor="Red"
                                    MaximumValue="65" MinimumValue="5" SetFocusOnError="True" Type="Double"></asp:RangeValidator>
                                <br />
                                <asp:TextBox ID="ws1TextBox2" runat="server" step="0.01" Enabled="False" type="number"
                                    required="required" TextMode="Number"></asp:TextBox>
                                <asp:Label ID="Label7" runat="server" Text="&#8451; (от 0&#8451; до 150&#8451;)">
                                </asp:Label>&nbsp;&nbsp;&nbsp;
                                <asp:RangeValidator ID="wsRangeValidator2" runat="server"
                                    ControlToValidate="ws1TextBox2" Display="Dynamic"
                                    ErrorMessage="Значение должно находится в диапазоне от 0&amp;#8451 до 150&amp;#8451"
                                    ForeColor="Red" MaximumValue="150" MinimumValue="0" SetFocusOnError="True"
                                    Type="Double"></asp:RangeValidator>
                                <br />
                                <asp:RequiredFieldValidator ID="ws1RequiredFieldValidator1" runat="server"
                                    ControlToValidate="ws1RadioButtonList1" Display="Dynamic"
                                    ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True">
                                    Выберите необходимое значение</asp:RequiredFieldValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:Label ID="Label20" runat="server" Text="Потери давления на клапане регулятора:">
                        </asp:Label>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label26" runat="server" Text="Регулятор перепада давления:"></asp:Label>
                                    <br />
                                    <div>
                                        <asp:Label ID="lpLabel1" runat="server" Text="Потери давления на регуляторе:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label54" runat="server" Text="ΔPрд ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp1TextBox1" runat="server" Enabled="False" step="0.01"  required="required" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList1" runat="server" Enabled="False" OnSelectedIndexChanged="lp1DropDownList1_SelectedIndexChanged" AutoPostBack="True">
                                           <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="lp1TextBox1" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,001" SetFocusOnError="True" CultureInvariantValues="True"></asp:RangeValidator>
                                        <br />
                                        <asp:Label ID="lpLabel2" runat="server" Text="Перепад давлений, поддерживаемый регулятором на регулируемом участке:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label55" runat="server" Text="ΔPру ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp1TextBox2" runat="server" Enabled="False" step="0.01" required="required" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList2" runat="server" Enabled="False" OnSelectedIndexChanged="lp1DropDownList2_SelectedIndexChanged" AutoPostBack="True">
                                               <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="lp1TextBox2" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True"></asp:RangeValidator>
                                        <asp:CustomValidator ID="CustomValidator8" runat="server" ControlToValidate="lp1TextBox2" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator8_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="lpLabel3" runat="server" Text="Давление в подающем трубопроводе на вводе ТП:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label56" runat="server" Text="P1 ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp1TextBox3" runat="server"  Enabled="False" step="0.01" required="required" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList3" runat="server" Enabled="False" OnSelectedIndexChanged="lp1DropDownList3_SelectedIndexChanged" AutoPostBack="True">
                                               <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="lp1TextBox3" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True"></asp:RangeValidator>
                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="lp1TextBox3" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator1_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="lpLabel4" runat="server" Text="Давление в обратном трубопроводе на вводе в ТП:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label57" runat="server" Text="P2 ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp1TextBox4" step="0.01" runat="server" Enabled="False" TextMode="Number" required="required"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp1DropDownList4" runat="server" Enabled="False" OnSelectedIndexChanged="lp1DropDownList4_SelectedIndexChanged" AutoPostBack="True">
                                               <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator4" runat="server" ControlToValidate="lp1TextBox4" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True"></asp:RangeValidator>
                                        <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="lp1TextBox4" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator2_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="lpLabel5" runat="server" Text="Максимальный перепад на регуляторе:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label58" runat="server" Text="ΔPрд(max) = Р1 - Р2 - ΔРру ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp1TextBox5" runat="server"  Enabled="False" ReadOnly="True" ></asp:TextBox>
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
                                    <asp:Label ID="Label2" runat="server" Text="Регулятор давления <q>после себя</q>"></asp:Label>
                                    <br />
                                    <div>
                                        <asp:Label ID="Label9" runat="server" Text="Давление перед регулятором:"></asp:Label>
                                        <br />
                                       <asp:Label ID="Label3" runat="server" Text="P1 ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp2TextBox1" runat="server" step="0.01" Enabled="False" TextMode="Number" required="required"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp2DropDownList1" runat="server" Enabled="False" OnSelectedIndexChanged="lp2DropDownList1_SelectedIndexChanged" AutoPostBack="True">
                                           <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator5" runat="server" ControlToValidate="lp2TextBox1" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True" CultureInvariantValues="True"></asp:RangeValidator>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator3" runat="server" ControlToValidate="lp2TextBox1" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator3_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="Label4" runat="server" Text="Требуемое давление после регулятора:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label8" runat="server" Text="Р(треб) ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp2TextBox2" runat="server" step="0.01" Enabled="False" TextMode="Number" required="required"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp2DropDownList2" runat="server" Enabled="False" OnSelectedIndexChanged="lp2DropDownList2_SelectedIndexChanged" AutoPostBack="True">
                                               <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator6" runat="server" ControlToValidate="lp2TextBox2" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True"></asp:RangeValidator>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator4" runat="server" ControlToValidate="lp2TextBox2" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator4_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                         <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label10" runat="server" Text="Регулятор давления <q>до себя</q>"></asp:Label>
                                    <br />
                                    <div>
                                        <asp:Label ID="Label11" runat="server" Text="Требуемое давление перед регулятором:"></asp:Label>
                                        <br />
                                       <asp:Label ID="Label12" runat="server" Text="Р(треб)"></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp3TextBox1" runat="server" step="0.01" Enabled="False" TextMode="Number" required="required"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp3DropDownList1" runat="server" Enabled="False" OnSelectedIndexChanged="lp3DropDownList1_SelectedIndexChanged" AutoPostBack="True">
                                           <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator7" runat="server" ControlToValidate="lp3TextBox1" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True" CultureInvariantValues="True"></asp:RangeValidator>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator5" runat="server" ControlToValidate="lp3TextBox1" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator5_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="Label13" runat="server" Text="Давление после регулятора:"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label14" runat="server" Text="Р2 ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp3TextBox2" runat="server" step="0.01" Enabled="False" TextMode="Number" required="required"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp3DropDownList2" runat="server" Enabled="False" OnSelectedIndexChanged="lp3DropDownList2_SelectedIndexChanged" AutoPostBack="True">
                                               <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RangeValidator ID="RangeValidator8" runat="server" ControlToValidate="lp3TextBox2" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True"></asp:RangeValidator>
                                        
                                        &nbsp;<asp:CustomValidator ID="CustomValidator6" runat="server" ControlToValidate="lp3TextBox2" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator6_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label15" runat="server" Text="Регулятор <q>перепуска</q>"></asp:Label>
                                    <br />
                                    <div>
                                        <asp:Label ID="Label16" runat="server" Text="Давление перед регулятором:"></asp:Label>
                                        <br />
                                       <asp:Label ID="Label17" runat="server" Text="РΔPру ="></asp:Label>
                                        &nbsp;<asp:TextBox ID="lp4TextBox2" runat="server" Enabled="False" step="0.01" TextMode="Number" required="required"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lp4DropDownList2" runat="server" Enabled="False" OnSelectedIndexChanged="lp4DropDownList2_SelectedIndexChanged" AutoPostBack="True">
                                           <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:RangeValidator ID="RangeValidator9" runat="server" ControlToValidate="lp4TextBox2" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True"></asp:RangeValidator>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator7" runat="server" ControlToValidate="lp4TextBox2" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator7_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                                        <br />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                     <div class="col border">
                        <asp:Label ID="Label18" runat="server" Text="Расчет регулятора давления на кавитацию:">
                        </asp:Label>
                        <div class="col">
                            <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label19" runat="server" Text="Давление перед регулятором:"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label21" runat="server" Text="P' = "></asp:Label>
                                    <asp:TextBox ID="calcrTextBox1" runat="server" step="0.01" Enabled="False" type="number"
                                        required="required" TextMode="Number"></asp:TextBox>
                                    <asp:DropDownList ID="calcrDropDownList1" runat="server" AutoPostBack="True"
                                        Enabled="False" OnSelectedIndexChanged="calcrDropDownList1_SelectedIndexChanged">
                                        <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                    </asp:DropDownList>&nbsp;<asp:RangeValidator ID="calcrRangeValidator1" runat="server"
                                        ControlToValidate="calcrTextBox1" Display="Dynamic"
                                        ErrorMessage="Неверно указано значение давления" ForeColor="Red"
                                        MaximumValue="99999999" MinimumValue="0,0000001" SetFocusOnError="True">Неверно указано
                                        значение давления</asp:RangeValidator>

                                    <asp:CustomValidator ID="CustomValidator9" runat="server" ControlToValidate="calcrTextBox1" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red" OnServerValidate="CustomValidator9_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>

                                    <br />
                                    <asp:Label ID="Label22" runat="server"
                                        Text="Температура теплоносителя через регулятор (максимальная):"></asp:Label><br />
                                    <asp:Label ID="Label23" runat="server" Text="T1 = "></asp:Label>
                                    <asp:TextBox ID="calcrTextBox2" runat="server" step="0.01" Enabled="False" type="number"
                                        required="required" TextMode="Number"></asp:TextBox>
                                    <asp:Label ID="Label24" runat="server" Text=" &#8451;"></asp:Label>
                                    <br />
                                    <asp:RangeValidator ID="calcrRangeValidator2" runat="server"
                                        ErrorMessage="Неверно указано значение температуры"
                                        ControlToValidate="calcrTextBox2" Display="Dynamic" ForeColor="Red"
                                        MaximumValue="150" MinimumValue="0,000001" SetFocusOnError="True">Неверно указано значение температуры</asp:RangeValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <div class="col border">
                        <div>
                             <asp:Label ID="Label31" runat="server" Text="Расход через регулятор давления:"></asp:Label>
                            <asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:RadioButton ID="fprRadioButton1" runat="server"
                                        Text="Задать max величину расхода через регулятор давления:" AutoPostBack="True" OnCheckedChanged="fprRadioButton1_CheckedChanged"
                                         /><br />
                                    <div class="col">
                                        <asp:Label ID="Label28" runat="server" Text="Gкл = "></asp:Label>
                                        <asp:TextBox ID="fprTextBox1" runat="server" step="0.01" Enabled="False" type="number"
                                            required="required" TextMode="Number"></asp:TextBox>
                                        <asp:DropDownList ID="fprDropDownList1" runat="server" AutoPostBack="True"
                                            Enabled="False" OnSelectedIndexChanged="fprDropDownList1_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>м3/ч</asp:ListItem>
                                            <asp:ListItem>л/с</asp:ListItem>
                                            <asp:ListItem>л/мин</asp:ListItem>
                                            <asp:ListItem>л/ч</asp:ListItem>
                                            <asp:ListItem>кг/с</asp:ListItem>
                                            <asp:ListItem>кг/ч</asp:ListItem>
                                            <asp:ListItem>т/ч</asp:ListItem>
                                        </asp:DropDownList>
                                        <br />
                                        <asp:RangeValidator ID="fprRangeValidator9" runat="server"
                                            ControlToValidate="fprTextBox1" Display="Dynamic"
                                            ErrorMessage="Неверно указано значение давления" ForeColor="Red"
                                            MaximumValue="99999999" MinimumValue="0,0000001" SetFocusOnError="True">Неверно
                                            указано значение давления</asp:RangeValidator>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div>
                            <asp:UpdatePanel ID="UpdatePanel14" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButton ID="fprRadioButton2" runat="server"
                                        Text="Вычислить max величину расхода через клапан:" AutoPostBack="True" OnCheckedChanged="fprRadioButton2_CheckedChanged"/>
                                    <div class="col">
                                        <asp:Label ID="Label25" runat="server" Text="Температура подающего теплоносителя Т1 =">
                                        </asp:Label>
                                        <asp:TextBox ID="fprTextBox2" runat="server" Enabled="False" required="required" TextMode="Number"></asp:TextBox>
                                        <asp:Label ID="Label27" runat="server" Text="&#8451;"></asp:Label>
                                        &nbsp;<asp:RangeValidator ID="RangeValidator10" runat="server" ControlToValidate="fprTextBox2" Display="Dynamic" ErrorMessage="Неверно указано значение температуры" ForeColor="Red" MaximumValue="150" MinimumValue="0,00000001" SetFocusOnError="True" CultureInvariantValues="True"></asp:RangeValidator>
                                        <br />
                                        <asp:Label ID="Label29" runat="server" Text="Температура обратного теплоносителя T2 = ">
                                        </asp:Label>
                                        <asp:TextBox ID="fprTextBox3" runat="server" Enabled="False" step="0.01" required="required" TextMode="Number"></asp:TextBox>
                                        <asp:Label ID="Label30" runat="server" Text="&#8451;"></asp:Label>
                                        
                                        &nbsp;<asp:RangeValidator ID="RangeValidator11" runat="server" ControlToValidate="fprTextBox3" Display="Dynamic" ErrorMessage="Неверно указано значение температуры" ForeColor="Red" MaximumValue="9999999" MinimumValue="0,00000001" SetFocusOnError="True"></asp:RangeValidator>
                                        
                                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="fprTextBox2" ControlToValidate="fprTextBox3" Display="Dynamic" ErrorMessage="Неверно указано значение температуры" ForeColor="Red" Operator="LessThan" SetFocusOnError="True" Type="Integer"></asp:CompareValidator>
                                        
                                        <br />
                                        
                                        <asp:Label ID="Label45" runat="server" Text="Тепловая мощность Q = ">
                                        </asp:Label>
                                        <asp:TextBox ID="fprTextBox4" runat="server" Enabled="False" step="0.01" required="required" TextMode="Number"></asp:TextBox>
                                        <asp:DropDownList ID="fprDropDownList2" runat="server" AutoPostBack="True"
                                            Enabled="False" OnSelectedIndexChanged="fprDropDownList2_SelectedIndexChanged">
                                            <asp:ListItem class="dropdown-item">выбрать</asp:ListItem>
                                            <asp:ListItem Value="кВт" class="dropdown-item">кВт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">МВт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">Вт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">Гкал/ч</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">ккал/ч</asp:ListItem>
                                        </asp:DropDownList>&nbsp;<asp:RangeValidator ID="fprRangeValidator2" runat="server"
                                            ControlToValidate="fprTextBox4" Display="Dynamic"
                                            ErrorMessage="Неверно указано значение давления" ForeColor="Red"
                                            MaximumValue="99999999" MinimumValue="0,0000001">Неверно указано значение давления
                                        </asp:RangeValidator>
                                        <br />

                                        <asp:Label ID="Label46" runat="server" Text="Максимальный расход Gкл = ">
                                        </asp:Label>
                                        <asp:TextBox ID="fprTextBox5" runat="server" step="0.01" Enabled="False" ReadOnly="True"  TextMode="Number"></asp:TextBox>
                                        <asp:Label ID="Label48" runat="server" Text=" кг/ч"></asp:Label>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    

                    <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                        <ContentTemplate>
                            <br />
                            <asp:Label ID="LabelError" runat="server" Font-Bold="True" Font-Size="Medium"
                                Font-Strikeout="False" ForeColor="Red"></asp:Label>
                            <br />
                            <asp:Button ID="rButton" runat="server" type="submit" Text="Рассчитать"
                                 Width="100%" OnClick="rButton_Click" />



                            <asp:Label ID="Label52" runat="server" Enabled="False" Text="Результаты расчёта"
                                Visible="False" Font-Bold="True" Font-Size="Medium"></asp:Label>

                            <br />
                            <asp:Label ID="ws1ResultLabel" runat="server" Visible="False"></asp:Label>
                            <br />
                            <asp:Label ID="maxt1ResultLabel" runat="server" Visible="False"></asp:Label>
                            <br />
                            <asp:Label ID="maxp1ResultLabel" runat="server" Visible="False"></asp:Label>
                            <br />

                            <div class="table-responsive-lg" onclick="ShowBTN()">
                                <asp:GridView ID="GridView1" CssClass="table" runat="server"
                                    
                                    AutoGenerateSelectButton="True" Font-Size="X-Small" Visible="False" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                                    <RowStyle Font-Size="Small" />
                                    <SelectedRowStyle BackColor="#ff7d00" Font-Bold="True" ForeColor="White" />
                                </asp:GridView>
                            </div>

                            <asp:Label ID="Label53" runat="server" CssClass="show-btn" Text="Объект:" Visible="False">
                            </asp:Label>

                            <asp:TextBox ID="objTextBox1" runat="server" Enabled="False" Visible="False"></asp:TextBox>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                        <script>
                            function ShowBTN() {
                                var btn2 = document.getElementById('Button2');
                                var btn3 = document.getElementById('Button3');
                                btn2.classList.add("show-btn");
                                btn3.classList.add("show-btn");
                        };
                    </script>
                    <asp:Button ID="Button2" runat="server" Text="Сохранить в PDF" CssClass="btn btn-primary hide-btn" OnClick="Button2_Click"
                         />
                    <asp:Button ID="Button3" runat="server" Text="Сохранить в Excel" CssClass="btn btn-primary hide-btn"
                        OnClick="Button3_Click"/>
                </div>
            </div>
            <div class="col-2">
                <asp:UpdatePanel ID="UpdatePanel16" runat="server">
                    <ContentTemplate>
                        <asp:Image ID="rPictureBox" runat="server" class="valve-image" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        </form>
    </div>

</body>

</html>