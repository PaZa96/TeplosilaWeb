'use strict';

const RDT = new TeplosilaRDT({
    baseUrl: 'https://ts-btp.techinby.com', //staging
});

const eorRadioButtonList = [
    "#eorRadioButtonList1_0",
    "#eorRadioButtonList1_1",
    "#eorRadioButtonList1_2",
    "#eorRadioButtonList1_3",
];

RDT.onSet((payload) => {
    console.log('payload: ', payload);

    const blockTypeCode = payload.block.block_type_code;

    switch (blockTypeCode) {
        case "TBV_TBVU":
            if (payload.block.method_setting_regulator_differential !== undefined) {
                checkRadioButtons(eorRadioButtonList, 0);
            }
            if (payload.block.method_setting_regulator_after !== undefined) {
                checkRadioButtons(eorRadioButtonList, 1);
            }
            if (payload.block.method_setting_regulator_before !== undefined) {
                checkRadioButtons(eorRadioButtonList, 2);
            }
            break;
        case "TBGV":
            checkRadioButtons(eorRadioButtonList, 0);
            break;
        case "TBO":
            checkRadioButtons(eorRadioButtonList, 0);
            break;
        case "TBR":
            checkRadioButtons(eorRadioButtonList, 0);
            break;
        case "TBSV":
            checkRadioButtons(eorRadioButtonList, 0);
            break;
        default:
            console.log('Incorect RDT Type');
    }
});

function checkRadioButtons(radioButtonList, index) {
    radioButtonList.forEach((button, i) => {
        $(button).prop('checked', i === index);
        $(button).prop('disabled', i !== index);
    });
}

// Отправка значений в родительское окно (программу БТП)
const data = { /* в data результат подбора */ };
RDT.save(data);
