'use strict';
const defaultPort = "60024";
const portRegex = new RegExp("^([1-9][0-9]{0,4})(\/\\w+)*$");
function constructTextField() {
    let portField = document.getElementById('portField');
    portField.value = chrome.storage.local.get(['port'], function (result) {
        portField.value = result.port;
        if (!portField.value) {
            portField.value = defaultPort;
            chrome.storage.local.set({"port": defaultPort});
        }
    });
    saveButton.addEventListener('click', function() {
        let address = document.getElementById('portField').value;
        if (!portRegex.test(address)) {
            setStatus(false, "invalid address");
        } else {
            let value = portRegex.exec(address)[1];
            if (Number(value) < 80 || Number(value) > 65536)
            setStatus(false, "Portnumber out of range: " + value);
            else {
                chrome.storage.local.set({"port": address}, function() {
                    setStatus(true, "Saved");
                });
            }
        }
    });
    defaultButton.addEventListener('click', function() {
        portField.value = defaultPort;
    })
}

function setStatus(ok, text) {
    let statusText = document.getElementById("saveStatus");
    statusText.innerHTML = text;
    if (ok)
    statusText.style.color = "green";
    else
    statusText.style.color = "red";
}
constructTextField();