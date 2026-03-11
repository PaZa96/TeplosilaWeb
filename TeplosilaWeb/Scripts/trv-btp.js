'use strict'
const TRV = new TeplosilaTRV({
    baseUrl: 'https://ts-btp.techinby.com',
});

let onSetTriggered = false;

TRV.onSet((payload) => {
    onSetTriggered = true;

    const hfPayloadElement = document.getElementById('hfPayload');
    const hfVersionElement = document.getElementById('hfPayloadVersion');
    const formElement = document.getElementById('form2');

    if (!hfPayloadElement || !hfVersionElement || !formElement) {
        console.error('Необходимые элементы DOM не найдены.');
        return;
    }

    const newPayload = JSON.stringify(payload.block);

    if (hfVersionElement.value === newPayload) {
        console.log('Payload уже обработан');
        formElement.style.display = 'block';
        return;
    }

    hfPayloadElement.value = newPayload;
    hfVersionElement.value = newPayload;

    setTimeout(() => __doPostBack('hfPayload', ''), 0);
});

// fallback: если onSet не вызвался
setTimeout(() => {
    if (!onSetTriggered) {
        console.log('onSet не вызвался — показываем форму вручную');
        document.getElementById('form2').style.display = 'block';
    }
}, 1000);

function returnData() {
    const hfResultElement = document.getElementById('hfResult');

    if (!hfResultElement) {
        console.error('Элемент hfResult не найден.');
        return;
    }


    try {
        const data = JSON.parse(hfResultElement.value.trim());

        console.log('Отправляемые данные:', JSON.stringify(data, null, 2));

        TRV.save(data);
    } catch (e) {
        console.error('Ошибка при парсинге JSON:', e);
    }
}