<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Translator.aspx.cs" Inherits="RojakJelah.Translator" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form runat="server">
        <h3>Input:</h3>
        <textarea id="input" runat="server"></textarea>
        <asp:Button ID="btnTranslate" runat="server" type="button" OnClick="BtnTranslate_Click" Text="Translate" />
        <asp:Button ID="btnSave" runat="server" type="button" OnClick="BtnSave_Click" Text="Save" />
        <h3>Output:</h3>
        <p id="output" runat="server"></p>
    </form>
    <hr />
    <p id="translationHistory" runat="server"></p>
</body>
</html>
