function OperationSuccessful(message) {
    iziToast.show({
        title: 'Success!',
        titleColor: 'green',
        position: 'topRight',
        message: message
    });
}

function OperationAborted(message) {
    iziToast.show({
        title: 'Error!',
        titleColor: 'red',
        position: 'topRight',
        message: message
    });
}

function Information(message) {
    iziToast.show({
        title: 'Info!',
        titleColor: 'blue',
        position: 'topRight',
        message: message
    });
}