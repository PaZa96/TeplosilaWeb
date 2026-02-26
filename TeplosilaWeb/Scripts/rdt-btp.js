const RDT = new TeplosilaRDT({
    baseUrl: 'https://ts-btp.techinby.com', //staging
});


RDT.onSet((payload) => {
    let hfPayload = document.getElementById('hfPayload');
    let hfVersion = document.getElementById('hfPayloadVersion');

    const newPayload = JSON.stringify(payload.block);

    // если payload не изменился — не вызываем PostBack
    if (hfVersion.value === newPayload) {
        console.log("payload уже обработан");
        $('#form2').show();
        return;
    }

    hfPayload.value = newPayload;
    hfVersion.value = newPayload;

    setTimeout(function () {
        __doPostBack('hfPayload', '');
    }, 0);

});


Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
    // Отправка значений в родительское окно (программу БТП)

    let hfResult = document.getElementById('hfResult');
    if (hfResult.value != "") {
        RDT.save(hfResult.value);
        hfResult.value = "";
    }

});
